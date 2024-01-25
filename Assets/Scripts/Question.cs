using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Question
{
    public string vocabulary;
    public string sentence;
    public string real_sentence;
    public Question(string vocabulary, string sentence)
    {
        this.vocabulary = vocabulary;
        this.sentence = sentence;
        int start = sentence.IndexOf('<');
        int end = sentence.IndexOf('>');
        if (start >= 0)
        {
            string substring = sentence.Substring(start, end - start + 1);
            this.real_sentence = sentence.Replace(substring, "<color=red>" + vocabulary + "</color>");
        }
    }
}

