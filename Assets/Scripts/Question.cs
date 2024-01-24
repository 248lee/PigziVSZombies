using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Question
{
    public string vocabulary;
    public string sentence;
    public Question(string vocabulary, string sentence)
    {
        this.vocabulary = vocabulary;
        this.sentence = sentence;
    }
}
