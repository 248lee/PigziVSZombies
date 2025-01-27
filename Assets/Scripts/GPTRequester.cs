using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Text.RegularExpressions;
using JohnUtils;

public static class GPTRequester
{
    private const string exampleMessage_sentence_q_string = "vocabulary: traffic";
    private const string exampleMessage_sentence_ans_string = @"The new <traffic> laws aimed to improve safety on highways.";
    private const string exampleMessage_sentence_o_string = "One more new sentence for vocabulary \"traffic\" that is very different from the previous ones. Also please surround the used vocabulary with a bracket <    >.";
    private const string exampleMessage_sentence_ans2_string = @"The snowstorm caused a standstill in the holiday <traffic> flow.";
    private static ChatMessage exampleMessage_sentence_q = new ChatMessage(ChatMessageRole.User, exampleMessage_sentence_q_string);
    private static ChatMessage exampleMessage_sencence_ans = new ChatMessage(ChatMessageRole.Assistant, exampleMessage_sentence_ans_string);
    private static ChatMessage exampleMessage_sentence_o = new ChatMessage(ChatMessageRole.User, exampleMessage_sentence_o_string);
    private static ChatMessage exampleMessage_sencence_ans2 = new ChatMessage(ChatMessageRole.Assistant, exampleMessage_sentence_ans2_string);
    private static List<ChatMessage> default_example = new List<ChatMessage> { exampleMessage_sentence_q, exampleMessage_sencence_ans };
    private static OpenAIAPI gpt;
    private static string systemMessage_sentence_string = File.ReadAllText(Application.streamingAssetsPath + "/GPTSystemMessage.txt");
    private static ChatMessage systemMessage_sentence = new ChatMessage(ChatMessageRole.System, systemMessage_sentence_string);
    private const string systemMessage_paragraph_string = @"The user will give you a list of English vocabularies. 
            Your job is to write a short paragraph using these vocabularies. The paragraph you provided 
            will be used as a fill-in-the-blank problem. Please put the word you use from the provided 
            vocabularies between < and >. Each vocabulary should be used exactly one time.
            In the next line, print out the order of the vocabularies you used in terms of the original order in the list of the given vocabularies, seperated with commas.
            Do not output anything else I've not mentioned.";
    private static ChatMessage systemMessage_paragraph = new ChatMessage(ChatMessageRole.System, systemMessage_paragraph_string);
    public static void SetupGPTModel()  // This should be called every time the stage has started!!
    {
        gpt = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_KEY", EnvironmentVariableTarget.User));
    }
    public static async Task<List<string>> RequestSentenceGPT(string vocabulary, int num_of_sentence)
    {
        SentenceBank sb = new SentenceBank(vocabulary);
        ChatMessage sentence_q = new ChatMessage(ChatMessageRole.User, "vocabulary: " + vocabulary);
        ChatMessage sentence_o = new ChatMessage(ChatMessageRole.User, "One more new sentence for vocabulary \"" + vocabulary + "\" that is very different from the previous ones. Also please surround the used vocabulary with a bracket <    >.");

        List<string> history = sb.GetAllSentences();
        List<ChatMessage> messages = new List<ChatMessage> { systemMessage_sentence };
        if (history.Count == 0)
        {
            Debug.Log("Hello " + vocabulary);
            Debug.LogError(systemMessage_sentence_string);
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
    public static async Task<Paragraph> RequestParagraphGPT(List<string> vocabularies)
    {
        string input_message = String.Join(", ", vocabularies); // Concatenate the vocabularies into a message like "apple, banana, complete, ice, sister"
        ChatMessage query = new ChatMessage(ChatMessageRole.User, input_message);
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
        string gpt_result_paragraph_and_order =  chatResult.Choices[0].Message.TextContent;
        string[] tmp = gpt_result_paragraph_and_order.Split("\n");
        string gpt_result_paragraph = string.Join("\n", tmp, 0, tmp.Length - 1); ;
        Debug.Log(gpt_result_paragraph_and_order);
        int[] orders = IListExtensions.ConvertStringToIntArray(tmp[tmp.Length - 1]);

        // Reorder the query vocabularies into the order that GPT used.
        List<string> used_vocabularies = new List<string>();
        foreach (int o in orders)
        {
            used_vocabularies.Add(vocabularies[o - 1]); // the first index given by gpt is 0
        }

        // Return the resulting paragraph and vocabularies(answers)
        return new Paragraph(used_vocabularies, gpt_result_paragraph);
    }
}
