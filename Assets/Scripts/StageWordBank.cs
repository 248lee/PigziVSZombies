using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JohnUtils;
using System.Threading;
using System.Threading.Tasks;

public class StageWordBank : MonoBehaviour
{
    public List<string> regularWords = new();
    public Dictionary<string, List<string>> waveSpecifiedWords = new();
    [SerializeField] private ChoiceWindow requestErrorNotification;
    [SerializeField] private ChoiceWindow queueEmptyErrorWindow;
    [SerializeField] private TMPro.TextMeshProUGUI requestErrorMessageText;
    [SerializeField] private GPTProgressUISystem progressPanel;
    private List<ProgressCounter> p_counters;
    private ProgressCounter paragraph_p_counter;
    private Dictionary<string, Queue<string>> sentences;
    private Dictionary<int, Queue<Paragraph>> paragraphs;
    private Dictionary<string, Paragraph> waveSpecifiedParagraphs;
    private CancellationTokenSource cts;

    // Start is called before the first frame update
    async void Start()
    {
        this.p_counters = new List<ProgressCounter>();
        GPTRequester.SetupGPTModel();
        await InitializeWordBank();
    }
    private async Task InitializeWordBank()
    {
        this.sentences = new Dictionary<string, Queue<string>>();
        this.paragraphs = new Dictionary<int, Queue<Paragraph>>();
        this.waveSpecifiedParagraphs = new Dictionary<string, Paragraph>();

        cts = new CancellationTokenSource();
        List<Task> sentenceGPTTasks = new List<Task>();
        try
        {
            await Task.Delay(1000); // A short delay to for fake loading
            // Initialization for sentences, and start the requesting processes concurrently
            foreach (string word in regularWords)
            {
                this.sentences.Add(word, new Queue<string>());

                ProgressCounter p_counter = new ProgressCounter(StaticGlobalVariables.INITIAL_NUM_OF_REFILL_SENTENCE, word);
                this.p_counters.Add(p_counter);

                sentenceGPTTasks.Add(this._RefillSentences(word, StaticGlobalVariables.INITIAL_NUM_OF_REFILL_SENTENCE, p_counter));
            }

            // Initialization for paragraphs
            List<int> may_occur_p_num_of_vocabularies = new List<int>();
            foreach (Wave wave in WaveSystem.instance.waves)
            {
                if (wave.mode == WaveMode.Boss && !may_occur_p_num_of_vocabularies.Contains(wave.p_numOfVocabularies))
                {
                    may_occur_p_num_of_vocabularies.Add(wave.p_numOfVocabularies);
                }
            }

            this.paragraph_p_counter = new ProgressCounter(may_occur_p_num_of_vocabularies.Count, "[Paragraphs]");
            this.p_counters.Add(paragraph_p_counter);

            this.progressPanel.SetupProgressBars(p_counters);

            await Task.WhenAll(sentenceGPTTasks);  // If any task fails, an exception will be thrown here

            await Task.Delay(1000); // A short delay to prevent dense GPT-4 requests

            // Refill paragraphs (this part remains unchanged)
            foreach (int mopnv in may_occur_p_num_of_vocabularies)
            {
                this.paragraphs.Add(mopnv, new Queue<Paragraph>());
                for (int i = 0; i < StaticGlobalVariables.INITIAL_NUM_OF_REFILL_PARAGRAPH; i++)
                {
                    await this._RefillOneParagraph(mopnv);
                }
                this.paragraph_p_counter.CountUp();
            }

            // After everything is ready, start the game
            WaveSystem.instance.StartGameProcess();
        }
        catch (System.Exception ex)
        {
            // Cancel all _RefillSentences tasks immediately
            cts.Cancel();

            // Get a friendly error message based on the exception type
            string friendlyMessage = ExceptionHandler.GetFriendlyMessage(ex);

            // Show a retry/cancel dialog with the friendly message
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            this.requestErrorNotification.gameObject.SetActive(true);
            this.requestErrorMessageText.text = friendlyMessage;  // Setup the error message
            this.requestErrorNotification.AddButtonListener(0, () =>  // the first button is "retry"
            {
                this.requestErrorNotification.gameObject.SetActive(false);
                tcs.SetResult(true);
            }, true);
            this.requestErrorNotification.AddButtonListener(1, () =>  // the second button is "leave"
            {
                this.requestErrorNotification.gameObject.SetActive(false);
                tcs.SetResult(false);
            }, true);

            await tcs.Task;  // Wait for the player to enter the choice
            bool retry = tcs.Task.Result;

            if (retry)
            {
                // Restart the entire process
                await InitializeWordBank();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelection");
                return;
            }
        }
    }

    private List<string> UniqueRandomSelectWords(int num)
    {
        List<string> result = new List<string>();
        // Randomly select words
        if (regularWords.Count < num * StaticGlobalVariables.UNIQUE_RANDOM_SELECT_METHOD_RATIO)  // if the number of words of this stage is less, we use O(num_of_stage_words) method
        {
            List<string> copy_of_regularWords = new List<string>(this.regularWords);
            IListExtensions.Shuffle<string>(copy_of_regularWords);
            for (int i = 0; i < num; i++)  // append the words into wave.v_candidates
                result.Add(copy_of_regularWords[i]);
        }
        else  // if the number of words is huge, then we use O(wave.numOfVocabularies^2) method
        {
            for (int i = 0; i < num; i++)
            {
                int random_index;
                bool isRepeated = false;
                do
                {
                    random_index = UnityEngine.Random.Range(0, this.regularWords.Count);
                    foreach (string selected_vocabulary in result)
                    {
                        if (selected_vocabulary == this.regularWords[random_index])
                        {
                            isRepeated = true;
                            break;
                        }
                    }
                } while (isRepeated);  // keep randomly pick a index until no repeat
                result.Add(this.regularWords[random_index]);
            }
        }
        return result;
    }
    private List<string> UniqueRandomSelectWords(int num, List<string> existingWords)
    {
        List<string> result = new List<string>(existingWords);
        // Randomly select words
        if (regularWords.Count < num * StaticGlobalVariables.UNIQUE_RANDOM_SELECT_METHOD_RATIO)  // if the number of words of this stage is less, we use O(num_of_stage_words) method
        {
            List<string> copy_of_regularWords = new List<string>(this.regularWords);
            IListExtensions.Shuffle<string>(copy_of_regularWords);
            for (int i = 0; i < num; i++)  // append the words into wave.v_candidates
                result.Add(copy_of_regularWords[i]);
        }
        else  // if the number of words is huge, then we use O(wave.numOfVocabularies^2) method
        {
            for (int i = 0; i < num; i++)
            {
                int random_index;
                bool isRepeated = false;
                do
                {
                    random_index = UnityEngine.Random.Range(0, this.regularWords.Count);
                    foreach (string selected_vocabulary in result)
                    {
                        if (selected_vocabulary == this.regularWords[random_index])
                        {
                            isRepeated = true;
                            break;
                        }
                    }
                } while (isRepeated);  // keep randomly pick a index until no repeat
                result.Add(this.regularWords[random_index]);
            }
        }
        return result;
    }

    /// <summary>
    /// For regular wave, randomly selects wave.numOfVocabularies words from regularWords and uniquily-append them to wave.v_candidates.
    /// For specified wave, copy the words in waveSpecifiedWords[wave.waveName] into wave.v_candidates.
    /// <para>NOTICE: This method is expected to be called right before the wave is executed.</para>
    /// </summary>
    /// <param name="wave">NOTICE: The wave's mode can ONLY be "Normal"!!</param>
    public void WordsOutgive(Wave wave)
    {
        if (wave.mode != WaveMode.Normal)
        {
            Debug.LogError("JohnLee: The wave's mode here should be \"Normal\", not " + wave.mode + "!!!");
            return;
        }
        if (wave.waveName == "")  // if the wave's a regularWave
        {
            if (wave.numOfVocabularies > regularWords.Count)
            {
                Debug.LogError("JohnLee: We expect that you should have more words in this stage. This doesn't even enough for a wave!!");
                return;
            }

            // Randomly select words
            wave.v_candidates = this.UniqueRandomSelectWords(wave.numOfVocabularies);
        }
        else  // if it is a specified wave
        {
            if (!this.waveSpecifiedWords.ContainsKey(wave.waveName))
            {
                Debug.LogError("You didn't specify the vocabularies for the specified-wave named " + wave.waveName + "!!!!");
                return;
            }
            wave.v_candidates = new List<string>(this.waveSpecifiedWords[wave.waveName]);
        }
    }
    /// <summary>
    /// For regular wave, randomly selects wave.numOfVocabularies words from regularWords and uniquily-append them to wave.v_candidates,
    /// and assign a paragraph using wave.p_numOfVocabularies of these chosen words.
    /// For specified wave, copy the words in waveSpecifiedWords[wave.waveName] into wave.v_candidates,
    /// and assign a paragraph using wave.p_numOfVocabularies of these chosen words.
    /// <para>NOTICE: This method is expected to be called right before the wave is executed.</para>
    /// </summary>
    /// <param name="wave">NOTICE: The wave's mode can ONLY be "Boss"!!</param>
    public void ParagraphAndWordsOutgive(Wave wave)
    {
        if (wave.mode != WaveMode.Boss)
        {
            Debug.LogError("The wave's mode here should be \"Boss\", not " + wave.mode + "!!!");
            return;
        }
        if (wave.waveName == "")  // if the wave's a regularWave
        {
            if (wave.numOfVocabularies > regularWords.Count)
            {
                Debug.LogError("JohnLee: We expect that you should have more words in this stage. This doesn't even enough for a wave!!");
                return;
            }
            if (this.paragraphs[wave.p_numOfVocabularies].Count == 0)  // If the paragraph queue is empty, pop up the error message and leave the game
            {
                this.ShowQueueEmptyErrorWindow();
                return;
            }
            Paragraph paragraph = this.paragraphs[wave.p_numOfVocabularies].Dequeue();
            wave.dragon_paragraph = paragraph;
            wave.v_candidates = new List<string>();
            foreach (string voc in paragraph.vocabularies)  // uniquely-append the vocabularies of paragraph into wave.v_candidates
            {
                bool isRepeated = false;
                foreach (string used_voc in wave.v_candidates)
                {
                    if (voc == used_voc)
                    {
                        isRepeated = true;
                        break;
                    }
                }
                if (isRepeated)
                    continue;
                wave.v_candidates.Add(voc);
            }
            // Random select words for the remaining space
            if (wave.p_numOfVocabularies != paragraph.vocabularies.Count)
            {
                Debug.LogError("The real number of paragraph-paragraphs is different from the one you've specified. There must be bugs exist!!");
                return;
            }
            wave.v_candidates = this.UniqueRandomSelectWords(wave.numOfVocabularies - wave.p_numOfVocabularies, wave.v_candidates);

            // refill regular paragraphs of the number if the paragraphs are not enough
            if (this.paragraphs[wave.p_numOfVocabularies].Count < StaticGlobalVariables.REFILL_PARAGRAPH_THRESHOLD)
            {
                _ = this._RefillOneParagraphDuringStage(wave.p_numOfVocabularies);  // Here _RefillOneParagrpah keeps running in the background until it completes its task of refilling the paragraph.
                // We don't wait for the paragraph refilling.
            }
        }
        else  // if it is a specified wave
        {
            if (!this.waveSpecifiedWords.ContainsKey(wave.waveName))
            {
                Debug.LogError("You didn't specify the vocabularies for the specified-wave named " + wave.waveName + "!!!!");
                return;
            }
            wave.dragon_paragraph = this.waveSpecifiedParagraphs[wave.waveName];
            wave.v_candidates = new List<string>(this.waveSpecifiedWords[wave.waveName]);
            this._RefillSpecifiedParagraph(wave);
        }
    }
    /// <summary>
    /// Give a sentence(string) of the given vocabulary. Notice that when the sentence of the vocabulary become less,
    /// this method will request sentences from gpt.
    /// </summary>
    /// <param name="vocabulary"></param>
    /// <returns></returns>
    public string GiveOneSentence(string vocabulary)
    {
        if (this.sentences[vocabulary].Count == 0)  // If the queue is empty, pop up the error message and leave the game.
        {
            this.ShowQueueEmptyErrorWindow();
            return "";
        }
        string resulting_sentence = this.sentences[vocabulary].Dequeue();
        if (this.sentences[vocabulary].Count <= StaticGlobalVariables.REFILL_SENTENCE_THRESHOLD)
            _ = this._RefillSentencesDuringStage(vocabulary, this.sentences[vocabulary].Count, null);  // Here _RefillSentences keeps running in the background until it completes its task of refilling the sentences.
        
        return resulting_sentence;  // We don't wait for the sentences refilling
    }
    private async Task _RefillSentences(string word, int num_to_refill, ProgressCounter p_counter)
    {
        if (cts.Token.IsCancellationRequested)
            return;

        try
        {
            List<string> resulting_sentences = await GPTRequester.RequestSentenceGPT(word, num_to_refill, cts, p_counter);
            if (cts.Token.IsCancellationRequested)
                return;

            foreach (string result_sentence in resulting_sentences)
            {
                this.sentences[word].Enqueue(result_sentence);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error while fetching sentences for {word}: {ex.Message}");
            // Rethrow the exception so that Task.WhenAll catches it (and aggregates if necessary)
            throw;
        }
    }
    private async Task _RefillSentencesDuringStage(string word, int num_to_refill, ProgressCounter p_counter)
    {
        try
        {
            await this._RefillSentences(word, num_to_refill, p_counter);
        }
        catch (System.Exception ex)
        {
            // Cancel all _RefillSentences tasks immediately
            cts.Cancel();

            // Get a friendly error message based on the exception type
            string friendlyMessage = ExceptionHandler.GetFriendlyMessage(ex);
            StatusStackSystem.instance.AddLevel3EmergencyEntry("<color=red>¿ù»~: " + friendlyMessage + "</color>");

            return;
        }
        StatusStackSystem.instance.Level3EmergencyEntrySetDone();  // If there exist any successful refill, clear the error message.
    }
    private async Task _RefillOneParagraph(int num_of_vocabularies)
    {
        try
        {
            List<string> selected_words_for_paragraph = this.UniqueRandomSelectWords(num_of_vocabularies);
            Paragraph resulting_paragrpah = await GPTRequester.RequestParagraphGPT(selected_words_for_paragraph);
            this.paragraphs[num_of_vocabularies].Enqueue(resulting_paragrpah);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error while fetching paragraph: {ex.Message}");
            // Rethrow the exception so that Task.WhenAll catches it (and aggregates if necessary)
            throw;
        }
    }
    private async Task _RefillOneParagraphDuringStage(int num_of_vocabularies)
    {
        try
        {
            await this._RefillOneParagraph(num_of_vocabularies);
        }
        catch (System.Exception ex)
        {
            // Cancel all _RefillSentences tasks immediately
            cts.Cancel();

            // Get a friendly error message based on the exception type
            string friendlyMessage = ExceptionHandler.GetFriendlyMessage(ex);
            StatusStackSystem.instance.AddLevel3EmergencyEntry("<color=red>¿ù»~: " + friendlyMessage + "</color>");

            return;
        }
        StatusStackSystem.instance.Level3EmergencyEntrySetDone();  // If there exist any successful refill, clear the error message.
    }
    private void _RefillSpecifiedParagraph(Wave wave)
    {

    }
    private void ShowQueueEmptyErrorWindow()
    {
        GameflowSystem.instance.SetPause();
        this.queueEmptyErrorWindow.gameObject.SetActive(true);
        this.queueEmptyErrorWindow.SetLastButtonToCloseWindow();
        this.queueEmptyErrorWindow.AddButtonListener(0, () => ResultSystem.instance.OpenResultWindow(), false);
    }

}
