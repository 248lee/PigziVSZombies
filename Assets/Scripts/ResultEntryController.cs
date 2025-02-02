using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultEntryController : MonoBehaviour
{
    public List<Image> icons = new List<Image>(System.Enum.GetValues(typeof(RecordType_inResults)).Length);
    public TextMeshProUGUI textOfNo, textOfWave, textOfVocab, textOfText;
    public void SetupEntryText(string no, ResultRecord record)
    {
        this.textOfNo.text = no;
        this.textOfWave.text = record.wave;
        this.textOfVocab.text = record.vocab;
        this.textOfText.text = record.text;

        // Show the appropriate icon
        for (int i = 0; i < System.Enum.GetValues(typeof(RecordType_inResults)).Length; i++)
        {
            if (i != (int)record.type)
                icons[i].gameObject.SetActive(false);
            else
                icons[i].gameObject.SetActive(true);
        }
    }
}
