using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Text.RegularExpressions;
using JohnUtils;

[System.Serializable]
public enum WaveMode
{
    Normal,
    Boss,
    LoopLabel,
    LoopEndCondition
}
public enum Relation
{
    greater,
    geq,
    less,
    leq,
    equal
}
[System.Serializable]
public class Wave
{
    public WaveMode mode;

    public List<string> v_candidates;
    public string waveName = "";
    public int numOfVocabularies = 0;
    public List<Question> questions = new();
    public List<Subwave> subwaves = new();

    public int p_numOfVocabularies = 0;

    public string labelName = "Unnamed";

    public string targetLabelName = "Unnamed";
    public string runtimeVariableA = "UnknownGlobalVariable";
    public Relation relation;
    public string runtimeVariableB = "UnknownGlobalVariable";

    // For Boss
    public Paragraph dragon_paragraph = null;

    public void ShuffleQuestions()
    {
        IListExtensions.Shuffle<Question>(questions);
    }
}
//public class Question
//{
//    public string sentence;
//    public string vocabulary;
//}
public class WaveSystem : MonoBehaviour
{
    private const string exampleMessage_sentence_q_string = "vocabulary: traffic";
    private const string exampleMessage_sentence_ans_string = @"The new <traffic> laws aimed to improve safety on highways.";
    private const string exampleMessage_sentence_o_string = "One more new sentence for vocabulary \"traffic\" that is very different from the previous ones. Also please surround the used vocabulary with a bracket <    >.";
    private const string exampleMessage_sentence_ans2_string = @"The snowstorm caused a standstill in the holiday <traffic> flow.";
    private static ChatMessage exampleMessage_sentence_q = new ChatMessage(ChatMessageRole.User, exampleMessage_sentence_q_string);
    private static ChatMessage exampleMessage_sencence_ans = new ChatMessage(ChatMessageRole.Assistant, exampleMessage_sentence_ans_string);
    private static ChatMessage exampleMessage_sentence_o = new ChatMessage(ChatMessageRole.User, exampleMessage_sentence_o_string);
    private static ChatMessage exampleMessage_sencence_ans2 = new ChatMessage(ChatMessageRole.Assistant, exampleMessage_sentence_ans2_string);
    private List<ChatMessage> default_example = new List<ChatMessage> { exampleMessage_sentence_q, exampleMessage_sencence_ans, exampleMessage_sentence_o, exampleMessage_sencence_ans2};

    [SerializeField] VocabularyBoard vocabularyBoard;
    [SerializeField] StageWordBank wordBankOfThisStage;
    
    public List<Wave> waves;
    public int nowWaveIndex = 0;
    DragonController dragon;
    FireballSysrem fireballsystem;
    private OpenAIAPI gpt;
    private ChatMessage systemMessage_sentence, systemMessage_paragraph;

    // Start is called before the first frame update
    void Start()
    {
        this.dragon = FindObjectOfType<DragonController>();
        this.fireballsystem = FindObjectOfType<FireballSysrem>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void StartGameProcess()
    {
        if (Application.isPlaying)
            StartCoroutine(this.gameProcess());
    }
    IEnumerator gameProcess()
    {
        while (this.nowWaveIndex < this.waves.Count)
        {
            Wave wave = this.waves[this.nowWaveIndex];
            if (wave.mode == WaveMode.Boss || wave.mode == WaveMode.Normal)
            {
                if (wave.mode == WaveMode.Normal)
                    this.wordBankOfThisStage.WordsOutgive(wave);
                else
                    this.wordBankOfThisStage.ParagraphAndWordsOutgive(wave);
                this.vocabularyBoard.UpdateVocabularyBoard(wave.v_candidates);
                yield return StartCoroutine(this.implementWaveProcess(wave));
            }
            else if (wave.mode == WaveMode.LoopEndCondition)
            {
                bool is_endloop = false;
                // Check loop condition
                float variableA, variableB;
                if (!float.TryParse(wave.runtimeVariableA, out variableA))  // if the string is in the form of float, just convert it to float
                {
                    variableA = RuntimeGlobalDictionary.GetVariableFloat(wave.runtimeVariableA);  // else, get the actual value from runtime global dictionary
                }
                if (!float.TryParse(wave.runtimeVariableB, out variableB))  // if the string is in the form of float, just convert it to float
                {
                    variableB = RuntimeGlobalDictionary.GetVariableFloat(wave.runtimeVariableB);  // else, get the actual value from runtime global dictionary
                }
                switch (wave.relation)
                {
                    case Relation.greater:
                        is_endloop = variableA > variableB;
                        break;
                    case Relation.geq:
                        is_endloop = variableA >= variableB;
                        break;
                    case Relation.less:
                        is_endloop = variableA < variableB;
                        break;
                    case Relation.leq:
                        is_endloop = variableA <= variableB;
                        break;
                    case Relation.equal:
                        is_endloop = variableA == variableB;
                        break;
                    default:
                        break;
                }
                if (!is_endloop)  // loop back if the condition is not satisfied
                {
                    for (int i = this.nowWaveIndex; i >= 0; i--)  // scan from this wave all the way to the front
                    {
                        if (this.waves[i].mode == WaveMode.LoopLabel && this.waves[i].labelName == wave.targetLabelName)
                        {
                            this.nowWaveIndex = i;  // this is like setting the program counter in the CPU to the loop label
                            break;
                        }
                    }
                }
            }
            this.nowWaveIndex++;
        }
    }
    public Question AskForAQuestion(Wave wave)
    {
        int random_index = UnityEngine.Random.Range(0, wave.v_candidates.Count);
        string vocabulary = wave.v_candidates[random_index];  // Randomly select a vocabulary from wave.v_candidates
        string sentence = this.wordBankOfThisStage.GiveOneSentence(vocabulary);
        return new Question(vocabulary, sentence);
    }
    IEnumerator implementWaveProcess(Wave wave)
    {
        if (wave.mode == WaveMode.Normal)
        {
            foreach (Subwave subwave in wave.subwaves)
            {
                yield return new WaitForSeconds(subwave.startDelay); // Implementing Subwave process here
                for (int i = 0; i < subwave.numOfEmmisions; i++)
                {
                    Question question = this.AskForAQuestion(wave);
                    this.fireballsystem.generateFireball(question);
                    float delayTime = UnityEngine.Random.Range(subwave.durationMin, subwave.durationMax);
                    yield return new WaitForSeconds(delayTime);
                }
            }
        }
        else if (wave.mode == WaveMode.Boss)
        {
            this.dragon.Born(wave);
            while (this.dragon.is_on_stage == true)  // wait until the dragon flies away
            {
                yield return null;
            }
        }
        while (this.fireballsystem.fire_onScreen.Count != 0) // busy waiting until the fire on screen is empty
        {
            yield return null;
        }
    }
    private async Task<List<string>> RequestSentenceGPT(string vocabulary, int num_of_sentence)
    {
        SentenceBank sb = new SentenceBank(vocabulary);
        ChatMessage sentence_q = new ChatMessage(ChatMessageRole.User, "vocabulary: " + vocabulary);
        ChatMessage sentence_o = new ChatMessage(ChatMessageRole.User, "One more new sentence for vocabulary \"" + vocabulary + "\" that is very different from the previous ones. Also please surround the used vocabulary with a bracket <    >.");

        List<string> history = sb.GetAllSentences();
        List<ChatMessage> messages = new List<ChatMessage> { systemMessage_sentence };
        if (history.Count == 0)
        {
            print("Hello " + vocabulary);
            messages.AddRange(new List<ChatMessage>(default_example));
            messages.Add(sentence_q);
        }
        else
        {
            messages.Add(sentence_q);
            messages.Add(new ChatMessage(ChatMessageRole.Assistant, history[0]));
            messages.Add(sentence_o); // The next request after the first sentence should be thorough, others can be simple.
            for (int i = 1; i < history.Count; i++)
            {
                messages.Add(new ChatMessage(ChatMessageRole.Assistant, history[i]));
                messages.Add(new ChatMessage(ChatMessageRole.User, "one more"));
            }
        }
        
        //List<ChatMessage> messages = new List<ChatMessage> { systemMessage_sentence, query };
        List<string> results = new List<string>();
        bool wrong_before = false;
        for (int i = 0; i < num_of_sentence; i++)
        {
            ChatResult chatResult;
            if (!wrong_before)
            {
                chatResult = await gpt.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.ChatGPTTurbo,
                    Temperature = 0.1,
                    MaxTokens = 2000,
                    Messages = messages
                });
            }
            else
            {
                chatResult = await gpt.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.GPT4,
                    Temperature = 0.1,
                    MaxTokens = 2000,
                    Messages = messages
                });
            }

            messages.Add(new ChatMessage(ChatMessageRole.Assistant, chatResult.Choices[0].Message.TextContent));
            int wrong_time = 0, max_wrong_time = 3;
            while (!Regex.IsMatch(chatResult.Choices[0].Message.TextContent, "<.*?>") && wrong_time < max_wrong_time)
            {
                wrong_before = true;
                Debug.LogError("Wrong GPT response format for sentence: " + chatResult.Choices[0].Message.TextContent);
                messages.Add(new ChatMessage(ChatMessageRole.User, "Wrong! Please cover the vocabulary \"" + vocabulary + "\" with a bracket \"<    >\". Give me the correct sentence directly."));
                chatResult = await gpt.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.GPT4,
                    Temperature = 0.05,
                    MaxTokens = 2000,
                    TopP = 0.5,
                    Messages = messages
                });
                messages.Add(new ChatMessage(ChatMessageRole.Assistant, chatResult.Choices[0].Message.TextContent));
                wrong_time++;
                Debug.Log("++++++++++++++++++++++++++++");
                foreach (ChatMessage cm in messages)
                {
                    Debug.Log(cm.TextContent);
                }
                Debug.Log("----------------------------");
            }
            if (wrong_time == max_wrong_time)
            {
                
            }
            else
            {
                results.Add(chatResult.Choices[0].Message.TextContent); // Push the response into the resulting sentences
            }
            if (history.Count == 0 && i == 0)
                messages.Add(sentence_o); // The next request after the first sentence should be thorough, others can be simple.
            else
                messages.Add(new ChatMessage(ChatMessageRole.User, "one more"));
        }

        sb.SetAllSentences(results);
        
        return results;
    }
    private async Task<string> RequestParagraphGPT(string vocabulary)
    {
        ChatMessage query = new ChatMessage(ChatMessageRole.User, vocabulary);
        ChatMessage[] example_vs = { new ChatMessage(ChatMessageRole.User, "complete, homework, happiness") };
        ChatMessage[] example_ps = { new ChatMessage(ChatMessageRole.Assistant, "Sarah experienced <happiness> when she finished her <homework>. The sense of accomplishment filled her with joy, making the effort worthwhile. This tells us always <complete> important task first, so we can relax and enjoy the rest of our day.\n3, 2, 1") };
        List<ChatMessage> messages = new List<ChatMessage> { systemMessage_paragraph, example_vs[0], example_ps[0], query };
        var chatResult = await gpt.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.GPT4,
            Temperature = 0,
            MaxTokens = 500,
            Messages = messages
        });
        return chatResult.Choices[0].Message.TextContent;
    }
    private async Task generateQuestions(Wave wave)
    {
        foreach (string vocabulary in wave.v_candidates)
        {
            List<string> gpt_result_sentences = await RequestSentenceGPT(vocabulary, 5);
            Debug.Log(vocabulary);
            if (!Application.isPlaying)
                return;
            for (int i = 0; i < gpt_result_sentences.Count; i++)
            {
                print(gpt_result_sentences[i]);
                Question tmp = new Question(vocabulary, gpt_result_sentences[i]);
                wave.questions.Add(tmp);
            }
        }
        wave.ShuffleQuestions(); // Shuffle all the questions to generate random questions
    }
    public async Task generateParagraph(Wave wave)
    {
        // Randomly pick 3 vocabularies from wave.v_candidates
        List<string> paragraph_candidates = new List<string>();
        List<int> chosen_indices = new List<int>();
        System.Random random = new System.Random();
        for (int i = 0; i < 3; i++)
        {
            int index = random.Next(0, wave.v_candidates.Count);
            int num_of_trials = 0, max_num_of_trials = 69;
            while (chosen_indices.Contains(index) && num_of_trials < max_num_of_trials)
            {
                index = random.Next(0, wave.v_candidates.Count);
                num_of_trials++;
            }
            chosen_indices.Add(index);
            paragraph_candidates.Add(wave.v_candidates[index]);
        }

        // Start requesting a paragraph
        string input_message = String.Join(", ", paragraph_candidates); // Concatenate the vocabularies into a message like "apple, banana, complete, ice, sister"
        string gpt_result_paragraph_and_order = await RequestParagraphGPT(input_message);
        string[] tmp = gpt_result_paragraph_and_order.Split("\n");
        string gpt_result_paragraph = string.Join("\n", tmp, 0, tmp.Length - 1); ;
        Debug.Log(gpt_result_paragraph_and_order);
        int[] orders = IListExtensions.ConvertStringToIntArray(tmp[tmp.Length - 1]);

        // Reorder the query vocabularies into the order that GPT used.
        List<string> used_vocabularies = new List<string>();
        foreach (int o in orders)
        {
            used_vocabularies.Add(paragraph_candidates[o - 1]); // the first index given by gpt is 0
        }

        // Send the resulting paragraph and vocabularies(answers) to the DuLagooooon!!!!
        wave.dragon_paragraph = new Paragraph(used_vocabularies, gpt_result_paragraph);
    }
}
