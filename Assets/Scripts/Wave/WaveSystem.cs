using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;


[System.Serializable]
public class Wave
{
    public float attr1 = 10;
    public float attr2 = 303f;
    public List<Subwave> subwaves = new List<Subwave>();
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
    public List<Question> questions;
    PlayerController playerController;
    DragonController dragon;
    FireballSysrem fireballsystem;
    private OpenAIAPI gpt;
    private ChatMessage systemMessage;
    private string result = null; // For gpt test

    // Start is called before the first frame update
    void Start()
    {
        this.questions = new List<Question>();
        this.gpt = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_KEY", EnvironmentVariableTarget.User));
        this.systemMessage = new ChatMessage(ChatMessageRole.System, "The user will give you a list of English vocabularies. Your job is to make 5 sentences using these vocabularies, and the sentences you make should not be related each other. Please output the sentence you make on by one. Each output sentence should come up with the vocabulary you use to make that sentence. If you use 2 or more vocabularies to make that sentence, just output exactly one of them. It is encouraged to use only 1 vocabulary for each sentence, but this is in fact up to you. When you output the sentence, output \"s:\" before the sentence; output \"v:\" before the vocabulary you used. Do not output anything else I've not mentioned. ");
        this.playerController = FindObjectOfType<PlayerController>();
        this.dragon = FindObjectOfType<DragonController>();
        this.fireballsystem = FindObjectOfType<FireballSysrem>();
        getSentence();
        StartCoroutine(this.gameProcess());
    }

    // Update is called once per frame
    void Update()
    {
        if (this.result != null)
        {
            //Debug.Log(this.result);
            string[] lines = result.Split('\n');
            bool is_s_waiting_to_pair = false;
            Question tmp = new Question("", "");
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("s:"))
                {
                    tmp.sentence = lines[i].Substring(2); // Pop out the prefix "s:"
                    is_s_waiting_to_pair = true;
                }
                else if (lines[i].StartsWith("v:") && is_s_waiting_to_pair)
                {
                    tmp.vocabulary = lines[i].Substring(2); // Pop out the prefix "v:"
                    is_s_waiting_to_pair = false;
                    questions.Add(tmp);
                    tmp = new Question("", "");
                }
            }
            this.result = null;
        }

    }
    private async Task<string> GenerateExampleSentence(string vocabulary)
    {
        ChatMessage query = new ChatMessage(ChatMessageRole.User, vocabulary);
        List<ChatMessage> messages = new List<ChatMessage> { systemMessage, query };
        var chatResult = await gpt.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.5,
            MaxTokens = 200,
            Messages = messages
        });
        return chatResult.Choices[0].Message.Content;
    }
    public async void getSentence()
    {
        this.result = await GenerateExampleSentence("apple, banana, complete, ice, sister");
    }
    IEnumerator gameProcess()
    {
        foreach (Wave wave in this.waves)
        {
            yield return StartCoroutine(this.implementWaveProcess(wave));
            this.nowWave++;
        }
        this.dragon.Born();
    }
    IEnumerator implementWaveProcess(Wave wave)
    {
        foreach (Subwave subwave in wave.subwaves)
        {
            yield return StartCoroutine(this.implementSubwaveProcess(subwave));
        }
        while (this.fireballsystem.fire_onScreen.Count != 0) // busy waiting until the fire on screen is empty
        {
            yield return null;
        }
    }
    IEnumerator implementSubwaveProcess(Subwave subwave)
    {
        yield return new WaitForSeconds(subwave.startDelay);
        for (int i = 0; i < subwave.numOfEmmisions; i++)
        {
            float delayTime = UnityEngine.Random.Range(subwave.durationMin, subwave.durationMax);
            this.fireballsystem.generateFireball();
            yield return new WaitForSeconds(delayTime);
        }
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
