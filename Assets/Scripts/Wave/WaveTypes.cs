using JohnUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    public WaveMode mode;

    public List<string> v_candidates { get; set; }
    public string waveName = "";
    public int numOfVocabularies = 0;
    public List<Question> questions = new();
    public List<Subwave> subwaves = new();

    public int p_numOfVocabularies = 0;
    public DragonController.DragonWaveData dragonData;

    public string labelName = "Unnamed";
    public GameObject background;

    public string targetLabelName = "Unnamed";
    public string runtimeVariableA = "UnknownGlobalVariable";
    public Relation relation;
    public string runtimeVariableB = "UnknownGlobalVariable";

    // For Boss
    public Paragraph dragon_paragraph = null;

    public void ShuffleQuestions()
    {
        IListExtensions.Shuffle<Question>(questions);
    }
}
