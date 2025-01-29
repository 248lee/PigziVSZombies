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
    public ResultRecord(string no, string wave, string vocab, string text, RecordType_inResults type)
    {
        this.no = no;
        this.wave = wave;
        this.vocab = vocab;
        this.text = text;
        this.type = type;
    }
}
public class ResultSystem : MonoBehaviour
{
    public static ResultSystem instance { get; private set; }
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
    public List<ResultRecord> records = new List<ResultRecord>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 請查詢所有會需要增加record的情境並列點，我們逐項做處理。
    public void AddRecord(string no, string wave, string vocab, string text, RecordType_inResults type)
    {
        this.records.Add(new ResultRecord(no, wave, vocab, text, type));
    }
}
