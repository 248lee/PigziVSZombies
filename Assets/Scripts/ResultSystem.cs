using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RecordType_inResults
{
    Fireball,
    Healball,
    Boss
}
public class ResultRecord
{
    public string wave;
    public string vocab;
    public string text;
    public RecordType_inResults type;
    public bool mark;
    public ResultRecord(string wave, string vocab, string text, RecordType_inResults type, bool mark)
    {
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
    [SerializeField] GameObject resultContentSortByTime, resultContentSortByDic;
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
        //this.sortByTimeButton.onClick.AddListener(delegate() { this.DrawRecordsByTime(); });  // This is the older C# format.
        //this.sortByDicButton.onClick.AddListener(() => this.DrawRecordsByDictionary());  // This is the modern C# format.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 請查詢所有會需要增加record的情境並列點，我們逐項做處理。
    public void AddRecord(string wave, string vocab, string text, RecordType_inResults type, bool mark)
    {
        this.records.Add(new ResultRecord(wave, vocab, text, type, mark));
    }
    public void OpenResultWindow()
    {
        this.DrawRecordsByTime();
        this.DrawRecordsByDictionary();

        // Show the window
        this.resultWindow.SetActive(true);
    }
    public void CloseResultWindow()
    {
        this.resultWindow.SetActive(false);
    }
    public void DrawRecordsByTime()
    {
        // Clean up all the records in the content
        ResultEntryController[] previousRecords = this.resultContentSortByTime.GetComponentsInChildren<ResultEntryController>();
        foreach (var record in previousRecords)
            Destroy(record.gameObject);

        // Draw the records
        int no = 1;
        foreach (var record in this.records)
        {
            ResultEntryController entryController = Instantiate(this.resultRecordPrefab.gameObject, this.resultContentSortByTime.transform).GetComponent<ResultEntryController>();
            entryController.SetupEntryText(no.ToString(), record);
            no++;
        }
    }
    public void DrawRecordsByDictionary()
    {
        // Clean up all the records in the content
        ResultEntryController[] previousRecords = this.resultContentSortByDic.GetComponentsInChildren<ResultEntryController>();
        foreach (var record in previousRecords)
            Destroy(record.gameObject);

        List<ResultRecord> records_byDic = new List<ResultRecord>(this.records);
        records_byDic.Sort(delegate (ResultRecord x, ResultRecord y)
            {
                if (x.vocab == null && y.vocab == null) return 0;
                else if (x.vocab == null) return -1;
                else if (y.vocab == null) return 1;
                else return x.vocab.CompareTo(y.vocab);
            }
        );

        // Draw the records
        int no = 1;
        foreach (var record in records_byDic)
        {
            ResultEntryController entryController = Instantiate(this.resultRecordPrefab.gameObject, this.resultContentSortByDic.transform).GetComponent<ResultEntryController>();
            entryController.SetupEntryText(no.ToString(), record);
            no++;
        }
    }
}
