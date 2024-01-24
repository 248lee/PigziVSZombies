using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

[System.Serializable]
public enum WaveMode
{
    Normal,
    Boss
}
[System.Serializable]
public class Wave
{
    public WaveMode mode;
    public List<string> v_candidates;
    public List<Question> questions = new();
    public List<Subwave> subwaves = new();

    // For Boss
    public Paragraph dragon_paragraph = null;
}
//public class Question
//{
//    public string sentence;
//    public string vocabulary;
//}
public class WaveSystem : MonoBehaviour
{
    public List<Wave> waves;
    public int nowWave = 0;
    PlayerController playerController;
    DragonController dragon;
    FireballSysrem fireballsystem;
    private OpenAIAPI gpt;
    private ChatMessage systemMessage_sentence, systemMessage_paragraph;
    private string gpt_result_paragraph = null;
    private bool gpt_finish_generating_sentence = false;

    // Start is called before the first frame update
    void Start()
    {
        this.gpt = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_KEY", EnvironmentVariableTarget.User));
        this.systemMessage_sentence = new ChatMessage(ChatMessageRole.System, "The user will give you a list of English vocabularies. Your job is to make 5 sentences using these vocabularies, and the sentences you make should not be related each other. Please output the sentence you make on by one. Each output sentence should come up with the vocabulary you use to make that sentence. If you use 2 or more vocabularies to make that sentence, just output exactly one of them. It is encouraged to use only 1 vocabulary for each sentence, but this is in fact up to you. Also, the sentence you provided will be used as a fill-the-blank problem. Please represent the blank with the string \"<  >\". When you output the sentence, output \"s:\" before the sentence; output \"v:\" before the vocabulary you used. Do not output anything else I've not mentioned.");
        this.systemMessage_paragraph = new ChatMessage(ChatMessageRole.System, "The user will give you a list of English vocabularies. Your job is to write a short paragraph using these vocabularies. The paragraph you provided will be used as a fill-the-blank problem. Please represent the blank with the string \"<  >\". After outputting the paragraph, please output the vocabulary in the order of them being used in the paragraph, seperated with a comma. Do not output anything else I've not mentioned.");
        this.playerController = FindObjectOfType<PlayerController>();
        this.dragon = FindObjectOfType<DragonController>();
        this.fireballsystem = FindObjectOfType<FireballSysrem>();
        StartCoroutine(this.gameProcess());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator gameProcess()
    {
        foreach (Wave wave in this.waves)
        {
            if (wave.mode == WaveMode.Normal)
                generateQuestions(wave); // when this process is finished, this.gpt_finish_generating_sentence will become true
            else if (wave.mode == WaveMode.Boss)
                generateParagraph(wave);
            while (!this.gpt_finish_generating_sentence)
                yield return null;
            this.gpt_finish_generating_sentence = false;
            yield return StartCoroutine(this.implementWaveProcess(wave));
            this.nowWave++;
        }
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
                    float delayTime = UnityEngine.Random.Range(subwave.durationMin, subwave.durationMax);
                    this.fireballsystem.generateFireball(wave.questions[0]); // pop out a question
                    wave.questions.RemoveAt(0); // pop out a question
                    yield return new WaitForSeconds(delayTime);
                }
            }
        }
        else
        {
            this.dragon.Born(wave.dragon_paragraph);
        }
        while (this.fireballsystem.fire_onScreen.Count != 0) // busy waiting until the fire on screen is empty
        {
            yield return null;
        }
    }
    private async Task<string> GenerateExampleSentence(string vocabulary)
    {
        ChatMessage query = new ChatMessage(ChatMessageRole.User, vocabulary);
        List<ChatMessage> messages = new List<ChatMessage> { systemMessage_sentence, query };
        var chatResult = await gpt.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.1,
            MaxTokens = 2000,
            Messages = messages
        });
        return chatResult.Choices[0].Message.Content;
    }
    private async Task<string> GenerateExampleParagraph(string vocabulary)
    {
        ChatMessage query = new ChatMessage(ChatMessageRole.User, vocabulary);
        ChatMessage[] example_vs = { new ChatMessage(ChatMessageRole.User, "complete, homework, happiness") };
        ChatMessage[] example_ps = { new ChatMessage(ChatMessageRole.Assistant, "Sarah experienced <  > <  > when she finished her <  >. The sense of accomplishment filled her with joy, making the effort worthwhile. Now, with her tasks completed, she could relax and enjoy the rest of her day.\ncomplete, happiness, homework") };
        List<ChatMessage> messages = new List<ChatMessage> { systemMessage_paragraph, example_vs[0], example_ps[0], query };
        var chatResult = await gpt.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.1,
            MaxTokens = 500,
            Messages = messages
        });
        return chatResult.Choices[0].Message.Content;
    }
    private async void generateQuestions(Wave wave)
    {
        string input_message = String.Join(", ", wave.v_candidates); // Concatenate the vocabularies into a message like "apple, banana, complete, ice, sister"
        Debug.Log("hello1");
        string gpt_result_sentence = await GenerateExampleSentence(input_message);
        Debug.Log(gpt_result_sentence);
        string[] lines = gpt_result_sentence.Split('\n');
        bool is_s_waiting_to_pair = false;
        Question tmp = new Question("", "");
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("s:"))
            {
                tmp.sentence = lines[i].Substring(3); // Pop out the prefix "s: "
                is_s_waiting_to_pair = true;
            }
            else if (lines[i].StartsWith("v:") && is_s_waiting_to_pair)
            {
                tmp.vocabulary = lines[i].Substring(3); // Pop out the prefix "v: "
                is_s_waiting_to_pair = false;
                wave.questions.Add(tmp);
                tmp = new Question("", "");
            }
        }
        this.gpt_finish_generating_sentence = true;
    }
    public async void generateParagraph(Wave wave)
    {
        string input_message = String.Join(", ", wave.v_candidates); // Concatenate the vocabularies into a message like "apple, banana, complete, ice, sister"
        this.gpt_result_paragraph = await GenerateExampleParagraph(input_message);
        string[] lines = gpt_result_paragraph.Split('\n');
        List<string> vocabularies = new List<string>(lines[1].Split(", "));
        wave.dragon_paragraph = new Paragraph(vocabularies, lines[0]);
        this.gpt_finish_generating_sentence = true;
    }

    float getTimeRandPos(float duration, float td)
    {
        float tmp = UnityEngine.Random.Range(0, duration);
        if (tmp < td)
        {
            int rePick = UnityEngine.Random.Range(0, 2);
            if (rePick == 0)
                tmp = getTimeRandPos(duration, td);
        }
        return tmp;
    }
}
