using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SentenceBank : MonoBehaviour
{
    private List<string> sentences;
    private string vocabulary;
    private string pathname;
    public SentenceBank(string _vocabulary)
    {
        this.vocabulary = _vocabulary;

        // Create the file for the sentence bank
        string filename = this.vocabulary + ".txt";
        this.pathname = Application.streamingAssetsPath + "/SentenceBanks" + "/" + filename;
        if (!File.Exists(pathname))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/SentenceBanks/");
            File.WriteAllText(pathname, "");  // create an empty file
        }
        else
        {
            this.sentences = File.ReadAllLines(this.pathname).ToList();  // read the old file to the ram
        }
    }
    public void SetAllSentences(List<string> _sentences)
    {
        this.sentences = new List<string>(_sentences);  // copy the parameter to the ram
        if (!File.Exists(pathname))
        {
            Debug.LogError("The file \"" + this.pathname + "\" does not exist!!");
            return;
        }
        File.WriteAllLines(this.pathname, this.sentences);  // write the sentences in the ram into disk
    }
    public List<string> GetAllSentences()
    {
        return new List<string>(this.sentences);
    }
}
