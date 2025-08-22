using JohnUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Wave
{
    public string waveName = "";
    public GameObject background;
    public List<string> v_candidates { get; set; }
    public List<Question> questions = new();

    public abstract WaveMode Mode { get; }
    public void ShuffleQuestions()
    {
        IListExtensions.Shuffle<Question>(questions);
    }

    public abstract void InitializeWave();
    public abstract IEnumerator implementWaveProcess();
}
