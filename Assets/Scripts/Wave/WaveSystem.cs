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
//public class Question
//{
//    public string sentence;
//    public string vocabulary;
//}
public class WaveSystem : MonoBehaviour
{
    public static WaveSystem instance { get; private set; }

    [SerializeField] VocabularyBoard vocabularyBoard;
    [SerializeField] StageWordBank wordBankOfThisStage;
    [SerializeField] TMPro.TextMeshProUGUI nowWaveIndexForPlayerText;
    [SerializeField] GameObject healballCountdownUI;

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
            if (wave.mode == WaveMode.Boss || wave.mode == WaveMode.Normal)
            {
                if (wave.mode == WaveMode.Normal)
                    this.wordBankOfThisStage.WordsOutgive(wave);
                else
                    this.wordBankOfThisStage.ParagraphAndWordsOutgive(wave);

                // Update the background
                if (wave.background != null)
                {
                    this.currentBackground.SetActive(false);  // Deactivate the previous background
                    wave.background.SetActive(true);  // Activate the new background
                    this.currentBackground = wave.background;  // Update the current background reference
                }
                
                yield return this.vocabularyBoard.UpdateVocabularyBoard(wave.v_candidates);  // This plays the animation of setting up the vocabulary board
                yield return StartCoroutine(this.implementWaveProcess(wave));  // This waits the main process
                yield return new WaitForSeconds(3f);  // After the wave ends, rest for a while~
                this.nowWaveIndexForPlayer++;
            }
            else if (wave.mode == WaveMode.LoopEndLabel)
            {
                // Loop end label, jump back to the loop start condition
                for (int i = this.nowWaveIndex; i >= 0; i--)  // scan from this wave all the way to the front
                {
                    if (this.waves[i].mode == WaveMode.LoopStartCondition && this.waves[i].targetLabelName == wave.labelName)
                    {
                        this.nowWaveIndex = i - 1;  // this is like setting the program counter in the CPU to the loop label
                        break;
                    }
                }
            }
            else if (wave.mode == WaveMode.LoopStartCondition)
            {
                bool is_goInLoop = false;
                // Check loop condition
                float variableA, variableB;
                if (!float.TryParse(wave.runtimeVariableA, out variableA))  // if the string is in the form of float, just convert it to float
                {
                    variableA = RuntimeGlobalDictionary.GetVariableFloat(wave.runtimeVariableA);  // else, get the actual value from runtime global dictionary
                }
                if (!float.TryParse(wave.runtimeVariableB, out variableB))  // if the string is in the form of float, just convert it to float
                {
                    variableB = RuntimeGlobalDictionary.GetVariableFloat(wave.runtimeVariableB);  // else, get the actual value from runtime global dictionary
                }
                switch (wave.relation)
                {
                    case Relation.greater:
                        is_goInLoop = variableA > variableB;
                        break;
                    case Relation.geq:
                        is_goInLoop = variableA >= variableB;
                        break;
                    case Relation.less:
                        is_goInLoop = variableA < variableB;
                        break;
                    case Relation.leq:
                        is_goInLoop = variableA <= variableB;
                        break;
                    case Relation.equal:
                        is_goInLoop = variableA == variableB;
                        break;
                    default:
                        break;
                }
                if (!is_goInLoop)  // loop back if the condition is not satisfied
                {
                    for (int i = 0; i < this.waves.Count; i++)  // scan from this wave all the way to the front
                    {
                        if (this.waves[i].mode == WaveMode.LoopEndLabel && this.waves[i].labelName == wave.targetLabelName)
                        {
                            this.nowWaveIndex = i;  // this is like setting the program counter in the CPU to the end label
                            break;
                        }
                    }
                }
            }
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
    IEnumerator implementWaveProcess(Wave wave)
    {
        if (wave.mode == WaveMode.Normal)
        {
            foreach (Subwave subwave in wave.subwaves)
            {
                Coroutine waitingForHealballCoroutine = null;
                if (subwave.healBallDelay >= 0)
                    waitingForHealballCoroutine = StartCoroutine(this.WaitForHealball(wave, subwave.healBallDelay));
                yield return new WaitForSeconds(subwave.startDelay); // Implementing Subwave process here
                for (int i = 0; i < subwave.numOfEmmisions; i++)
                {
                    Debug.Log($"Now emmiting {i}th fireball");
                    Question question = this.AskForAQuestion(wave);
                    this.fireballsystem.generateFireball(question);
                    float delayTime = UnityEngine.Random.Range(subwave.durationMin, subwave.durationMax);
                    if (i < subwave.numOfEmmisions - 1)  // If this is not the last fireball, wait for a random time
                        yield return new WaitForSeconds(delayTime);
                }
                //if (waitingForHealballCoroutine != null)
                //{
                //    StopCoroutine(waitingForHealballCoroutine);  // If the healball delay is too long, just simply cancel it.
                //    this.healballCountdownUI.SetActive(false);
                //    Debug.LogWarning("The healball delay is too long. It is canceled");
                //}
            }
        }
        else if (wave.mode == WaveMode.Boss)
        {
            this.dragon.Born(wave);
            while (this.dragon.is_on_stage == true)  // wait until the dragon flies away
            {
                yield return null;
            }
        }
        while (this.fireballsystem.fire_onScreen.Count != 0) // busy waiting until the fire on screen is empty
        {
            yield return null;
        }
    }
    IEnumerator WaitForHealball(Wave wave, float seconds)
    {
        this.healballCountdownUI.SetActive(true);
        float countdown = seconds;
        while (countdown > 0)
        {
            this.healballCountdownUI.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(((int)countdown + 1).ToString());  // Draw the countdown UI (+1 for graphic delay)
            countdown -= Time.deltaTime;
            yield return null;
        }

        Question[] questions = new Question[4];
        for (int i = 0; i < 4; i++)
            questions[i] = this.AskForAQuestion(wave);

        this.fireballsystem.generateFourHealballs(questions);  // Generate Healballs

        // Show the second 0 in the UI for graphic delay
        this.healballCountdownUI.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText("0");
        yield return new WaitForSeconds(1f);
        this.healballCountdownUI.SetActive(false);
    }
}
