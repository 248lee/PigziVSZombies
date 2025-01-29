using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultEntryController : MonoBehaviour
{
    public List<Image> icons = new List<Image>(System.Enum.GetValues(typeof(RecordType_inResults)).Length);
    public TextMeshProUGUI textOfNo, textOfWave, textOfVocab, textOfText;
    public void SetupEntryText(string no, string wave, string vocab, string text, RecordType_inResults type)
    {
        this.textOfNo.text = no;
        this.textOfWave.text = wave;
        this.textOfVocab.text = vocab;
        this.textOfText.text = text;

        // Show the appropriate icon
        for (int i = 0; i < System.Enum.GetValues(typeof(RecordType_inResults)).Length; i++)
        {
            if (i != (int)type)
                icons[i].gameObject.SetActive(false);
            else
                icons[i].gameObject.SetActive(true);
        }
    }
}
