using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RecordType_inResults
{
    Fireball,
    Healball,
    Boss
}
public class ResultRecord
{
    public string no;
    public string wave;
    public string vocab;
    public string text;
    public RecordType_inResults type;
    public bool mark;
    public ResultRecord(string no, string wave, string vocab, string text, RecordType_inResults type, bool mark)
    {
        this.no = no;
        this.wave = wave;
        this.vocab = vocab;
        this.text = text;
        this.type = type;
        this.mark = mark;
    }
}
public class ResultSystem : MonoBehaviour
{
    public static ResultSystem instance { get; private set; }
    [SerializeField] GameObject resultWindow;
    [SerializeField] GameObject resultContent;
    [SerializeField] ResultEntryController resultRecordPrefab;
    public List<ResultRecord> records = new List<ResultRecord>();
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        records = new List<ResultRecord>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 請查詢所有會需要增加record的情境並列點，我們逐項做處理。
    public void AddRecord(string wave, string vocab, string text, RecordType_inResults type, bool mark)
    {
        this.records.Add(new ResultRecord((this.records.Count + 1).ToString(), wave, vocab, text, type, mark));
    }
    public void OpenResultWindow()
    {
        // Clean up all the records in the content
        ResultEntryController[] previousRecords = this.resultContent.GetComponentsInChildren<ResultEntryController>();
        foreach (var record in previousRecords)
            Destroy(record.gameObject);

        // Draw the records
        foreach (var record in this.records)
        {
            ResultEntryController entryController = Instantiate(this.resultRecordPrefab.gameObject, this.resultContent.transform).GetComponent<ResultEntryController>();
            entryController.SetupEntryText(record);
        }

        // Show the window
        this.resultWindow.SetActive(true);
    }
    public void CloseResultWindow()
    {
        this.resultWindow.SetActive(false);
    }
}
