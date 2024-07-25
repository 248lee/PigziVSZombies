using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Question
{
    public string vocabulary;
    public string sentence;
    public string real_sentence;
    public Question(string vocabulary, string sentence, int num_of_spaces)
    {
        this.vocabulary = vocabulary;
        string blank = new string(' ', num_of_spaces);
        this.sentence = Regex.Replace(sentence, "<.*?>", "<" + blank + ">"); // make the blank modifiable, convenient for me;
        int start = sentence.IndexOf('<');
        int end = sentence.IndexOf('>');
        if (start >= 0)
        {
            this.real_sentence = ReplaceWithColorTags(sentence, start, end);
        }
    }
    public Question(string vocabulary, string sentence)
    {
        this.vocabulary = vocabulary;
        this.sentence = Regex.Replace(sentence, "<.*?>", "<    >"); // this is a blank with constantly 4 spaces
        int start = sentence.IndexOf('<');
        int end = sentence.IndexOf('>');
        if (start >= 0)
        {
            this.real_sentence = ReplaceWithColorTags(sentence, start, end);
        }
    }
    public static string ReplaceWithColorTags(string input, int start, int end)
    {
        // Check for valid indices
        if (start < 0 || end >= input.Length || start >= end)
        {
            Debug.LogError("Invalid start or end index (cannot find the index of < and > and hence not able to change color of the answer).");
        }

        string startTag = "<color=red>";
        string endTag = "</color>";

        // Build the result string
        string result = input.Substring(0, start)
                        + startTag
                        + input.Substring(start + 1, end - start - 1)
                        + endTag
                        + input.Substring(end + 1);

        return result;
    }
}

