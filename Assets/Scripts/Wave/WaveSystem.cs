using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Text.RegularExpressions;
using JohnUtils;

[System.Serializable]
public enum WaveMode
{
    Normal,
    Boss,
    LoopStartCondition,
    LoopEndLabel
}
public enum Relation
{
    greater,
    geq,
    less,
    leq,
    equal
}
[System.Serializable]

//public class Question
//{
//    public string sentence;
//    public string vocabulary;
//}
public partial class WaveSystem : MonoBehaviour
{
    public static WaveSystem instance { get; private set; }

    [SerializeField] StageWordBank wordBankOfThisStage;
    [SerializeField] TMPro.TextMeshProUGUI nowWaveIndexForPlayerText;
    public GameObject healballCountdownUI;

    public WaveSequence waveSequence;
    public List<Wave> waves;
    public int nowWaveIndex = 0;
    DragonController dragon;
    FireballSysrem fireballsystem;
    GameObject currentBackground;

    public int nowWaveIndexForPlayer { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        this.waves = new List<Wave>();
        foreach (var waveContainer in this.waveSequence.waveContainers)
        {
            Wave wave = waveContainer.currentSelectedWave;
            if (wave != null)
            {
                this.waves.Add(wave);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.nowWaveIndexForPlayer = 1;
        this.dragon = FindObjectOfType<DragonController>();
        this.fireballsystem = FindObjectOfType<FireballSysrem>();
        this.healballCountdownUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void StartGameProcess()
    {
        if (Application.isPlaying)
        {
            GameflowSystem.instance.SetUnpaused();
            StartCoroutine(this.gameProcess());
        }
    }
    IEnumerator gameProcess()
    {
        this.currentBackground = this.waves[0].background;  // Set the initial background
        while (this.nowWaveIndex < this.waves.Count)
        {
            this.nowWaveIndexForPlayerText.SetText(this.nowWaveIndexForPlayer.ToString());
            Wave wave = this.waves[this.nowWaveIndex];
            
            wave.InitializeWave();  // Initialize the wave, such as giving out words and setting the background
            yield return wave.implementWaveProcess();  // Implement the wave process, such as generating fireballs and waiting for them to finish
            this.nowWaveIndex++;
        }
        GameflowSystem.instance.StageWin();  // The game stage is completed!!
    }
    public Question AskForAQuestion(Wave wave)
    {
        int random_index = UnityEngine.Random.Range(0, wave.v_candidates.Count);
        string vocabulary = wave.v_candidates[random_index];  // Randomly select a vocabulary from wave.v_candidates
        string sentence = this.wordBankOfThisStage.GiveOneSentence(vocabulary);
        return new Question(vocabulary, sentence);
    }
    
    
}
