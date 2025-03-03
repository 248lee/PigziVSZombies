using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using System.Threading;
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
    private static string systemMessage_sentence_string = File.ReadAllText(Application.streamingAssetsPath + "prompts/GPTSystemMessage.txt");
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
    public static async Task<List<string>> RequestSentenceGPT(string vocabulary, int num_of_sentence, CancellationTokenSource cts, ProgressCounter p_counter, bool forceGPT4 = false)
    {
        SentenceBank sb = new SentenceBank(vocabulary);
        ChatMessage sentence_q = new ChatMessage(ChatMessageRole.User, "vocabulary: " + vocabulary);
        ChatMessage sentence_o_old = new ChatMessage(ChatMessageRole.User, "One more sentence. It is okay for this sentence to be similar to the previous ones.");
        ChatMessage sentence_o = new ChatMessage(ChatMessageRole.User, "One more new sentence for vocabulary \"" + vocabulary + "\" that is very different from the previous ones. Also please surround the used vocabulary with brackets <    >.");

        List<string> history = sb.GetAllSentences();
        List<ChatMessage> messages = new List<ChatMessage> { systemMessage_sentence };
        if (history.Count == 0)
        {
            Debug.Log("Hello " + vocabulary);
            Debug.Log(systemMessage_sentence_string);
            messages.AddRange(new List<ChatMessage>(default_example));
            messages.Add(sentence_q);
        }
        else
        {
            messages.Add(sentence_q);
            for (int i = 0; i < history.Count; i++)
            {
                messages.Add(new ChatMessage(ChatMessageRole.Assistant, history[i]));
                messages.Add(sentence_o_old);
            }
        }

        //List<ChatMessage> messages = new List<ChatMessage> { systemMessage_sentence, query };
        List<string> results = new List<string>();
        bool wrong_before = false;
        for (int i = 0; i < num_of_sentence; i++)
        {
            if (cts.Token.IsCancellationRequested)  // If the cancelation source is triggered, cancel the tasks immediately.
                return null;
            ChatResult chatResult;
            StatusEntry statusEntry;
            if (!wrong_before && !forceGPT4 && vocabulary != "hostage")
            {
                statusEntry = StatusStackSystem.instance.AddStatusEntry($"剛剛向GPT3.5請求了 {vocabulary} 的例句，正在等待回應...");
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
                statusEntry = StatusStackSystem.instance.AddStatusEntry($"剛剛向GPT4請求了 {vocabulary} 的例句，正在等待回應...");
                chatResult = await gpt.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.GPT4,
                    Temperature = 0.1,
                    MaxTokens = 2000,
                    Messages = messages
                });
            }

            // 只檢查法第一行的話可以避免GPT講的廢話被採用
            string resultText = (chatResult.Choices[0].Message.TextContent + "\n").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[0];
            messages.Add(new ChatMessage(ChatMessageRole.Assistant, resultText));
            int wrong_time = 0, max_wrong_time = 3;
            while (!Regex.IsMatch(resultText, "<.*?>") && wrong_time < max_wrong_time)
            {
                // Update the status
                statusEntry.SetDone();
                statusEntry = StatusStackSystem.instance.AddStatusEntry($"<color=orange>偵測到 {vocabulary} 的例句格式錯誤，重新發送請求中...", true);

                wrong_before = true;
                Debug.LogError("Wrong GPT response format for sentence: " + resultText);
                messages.Add(new ChatMessage(ChatMessageRole.User, "Wrong! Please cover the vocabulary \"" + vocabulary + "\" with a bracket \"<    >\". Give me the correct sentence directly without saying anything unnecessary."));
                chatResult = await gpt.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.GPT4,
                    Temperature = 0.05,
                    MaxTokens = 2000,
                    TopP = 0.5,
                    Messages = messages
                });
                resultText = (chatResult.Choices[0].Message.TextContent + "\n").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[0];
                messages.Add(new ChatMessage(ChatMessageRole.Assistant, resultText));
                wrong_time++;
                Debug.Log("++++++++++++++++++++++++++++");
                foreach (ChatMessage cm in messages)
                {
                    Debug.Log(cm.TextContent);
                }
                Debug.Log("----------------------------");
            }
            if (wrong_time > max_wrong_time)
            {
                // Delete the history sentence bank and try again.
                sb.ResetSentenceBankOfThisVocabulary();
                statusEntry.SetDone();
                statusEntry = StatusStackSystem.instance.AddStatusEntry($"<color=red>{vocabulary} 的例句格式錯誤已經超過{max_wrong_time}次，正在重設歷史例句。可能的原因是GPT無法理解您所提供的單字，請double check拼字是否正確。", true);
                await RequestSentenceGPT(vocabulary, num_of_sentence, cts, p_counter);
                statusEntry.SetDone();
            }
            else
            {
                results.Add(resultText); // Push the response into the resulting sentences

                // The vocabulary is successfully added, updata status and progress.
                statusEntry.SetDone();
                if (p_counter != null)
                    p_counter.CountUp();  // Finish one sentence. Let's count up the progress!
            }
            messages.Add(sentence_o);
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

        StatusEntry statusEntry = StatusStackSystem.instance.AddStatusEntry($"GPT4正在生成包含{vocabularies.Count}個單字的文章");

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

        statusEntry.SetDone();
        // Return the resulting paragraph and vocabularies(answers)
        return new Paragraph(used_vocabularies, gpt_result_paragraph);
    }
}
