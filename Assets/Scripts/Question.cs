using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;

public class Question
{
    public string vocabulary;
    public string sentence;
    public string real_sentence;
    public Question(string vocabulary, string real_sentence, int num_of_spaces)
    {
        this.vocabulary = vocabulary;
        string blank = new string('\u00A0', num_of_spaces);
        this.sentence = Regex.Replace(real_sentence, "<.*?>", "<" + blank + ">"); // make the blank modifiable, convenient for me;
        this.real_sentence = real_sentence;
    }
    public Question(string vocabulary, string real_sentence)
    {
        this.vocabulary = vocabulary;
        this.sentence = Regex.Replace(real_sentence, "<.*?>", "<\u00A0\u00A0\u00A0\u00A0>"); // this is a blank with constantly 4 spaces
        this.real_sentence = real_sentence;
    }
    public string GetRealSentenceWithColor(string color_tag)
    {
        int start = this.real_sentence.IndexOf('<');
        int end = this.real_sentence.IndexOf('>');
        if (start >= 0)
        {
            return ReplaceWithColorTags(this.real_sentence, start, end, color_tag);
        }
        else
        {
            Debug.LogError("The variable \"start\" should be > 0. The character \'<\' may be not found");
            return "";
        }
    }
    public static string ReplaceWithColorTags(string input, int start, int end, string color_tag)
    {
        // Check for valid indices
        if (start < 0 || end >= input.Length || start >= end)
        {
            Debug.LogError("Invalid start or end index (cannot find the index of < and > and hence not able to change color of the answer).");
        }

        string startTag = "<color=" + color_tag + ">";
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

