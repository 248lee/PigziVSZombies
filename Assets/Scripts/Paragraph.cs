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
    public string GetProcessedTextofParagraph(List<string> colorTags)
    {
        if (colorTags.Count != this.vocabularies.Count)
        {
            Debug.LogError("The number of colorTags should be the same as the number of vocabularies in the paragraph!");
            return "---";
        }
        string result = new string(this.article);
        int index_of_tags = 0;
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i] == '<')
            {
                if (index_of_tags >= colorTags.Count)
                {
                    Debug.LogError("The number of tags is supposed to be equal to the number of \'<\'s. Please double check");
                    Debug.Log(this.article);
                    Debug.Log("Count of tags: " + colorTags.Count.ToString());
                    return "";
                }
                if (colorTags[index_of_tags] == null)
                    result = result.Remove(i, 1).Insert(i, "["); // Replace the character '<' into a '['
                else
                {
                    string start_tag = "<color=" + colorTags[index_of_tags] + ">";
                    result = result.Remove(i, 1).Insert(i, start_tag); // Replace the character '<' into the corresponding tag
                    i += start_tag.Length;  // Jump to the next char of the start_tag
                }
            }
            else if (result[i] == '>')
            {
                if (colorTags[index_of_tags] == null)
                    result = result.Remove(i, 1).Insert(i, "]"); // Replace the character '<' into a '['
                else
                {
                    string end_tag = "</color>";
                    result = result.Remove(i, 1).Insert(i, end_tag); // Replace the character '<' into the corresponding tag
                    i += end_tag.Length;  // Jump to the next char of the start_tag
                }

                index_of_tags++;  // Go on to the next tag.
            }
        }
        return result;
    }
}
