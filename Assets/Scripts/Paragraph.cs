using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Paragraph
{
    public List<string> vocabularies;
    public string article;
    public Paragraph(List<string> vocabularies, string article)
    {
        this.vocabularies = vocabularies;
        this.article = article;
    }
}
