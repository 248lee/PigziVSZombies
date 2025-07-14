using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using JohnUtils;
using UnityEngine.UI;
using TMPro;
using MilkShake;
public class DragonController : MonoBehaviour
{
    [System.Serializable]
    public class DragonWaveData
    {
        public enum DragonSwiftNumOfVocabularies
        {
            ZeroFireball,
            OneFireball,
            TwoFireballs,
            ThreeFireballs,
            FourFireballs
        }
        public DragonSwiftNumOfVocabularies swiftRightNumOfVocabularies;
        public DragonSwiftNumOfVocabularies swiftLeftNumOfVocabularies;
        public string durationOfEnemyParts;
    }
    [SerializeField] FireballSysrem fireballSystem;
    [SerializeField] Transform layPoint;
    [SerializeField] Transform paragraphStartPoint;
    [SerializeField] Animator graphAnimator;
    [SerializeField] Animator animator;
    [SerializeField] List<ParticleSystem> flameParticles = new List<ParticleSystem>();
    [SerializeField] HPBarController hpBar;
    [SerializeField] int maxHP = 100;
    [SerializeField] ParticleSystem flameGraphForPause;
    [SerializeField] TextMeshProUGUI paragraphText;
    [SerializeField] GameObject testball;
    [SerializeField] List<Transform> flame_targets = new List<Transform>();
    [SerializeField] Color textColor;
    [SerializeField] DamagePopupController damagePopup;
    [SerializeField] ShakePreset DragonFlyAwayShake_preset;
    [SerializeField] ShakePreset DragonDie_preset;
    [SerializeField] Renderer graphRenderer;
    [SerializeField] float z_delta_enemy_part_position = 0.85f;
    int hp;
    bool pauseTimer;
    bool isDying;
    float graphAnimatorSpeed, animatorSpeed;
    Wave wave;
    public bool is_on_stage = false;
    ChildSticker textSticker;
    List<EnemypartFireballController> enemyparts;

    private const string blank = "              ";

    // Start is called before the first frame update
    void Start()
    {
        this.textSticker = GetComponentInChildren<ChildSticker>();
        this.pauseTimer = false;
        this.isDying = false;
        this.hp = this.maxHP;
        this.hpBar.SetMaxHP(this.maxHP);
        if(this.flameParticles.Count == 0)
        {
            Debug.LogError("把flame particle拉進來阿!");
        }
        this.switchFlameParticle(false);
        this.animator = GetComponent<Animator>();
        if (this.graphAnimator == null)
        {
            Debug.LogError("幹你的graphAnimator要記得拉進來阿");
        }
        this.graphAnimatorSpeed = graphAnimator.speed;
        this.animatorSpeed = animator.speed;
        this.animator.speed = 0f;
        this.graphAnimator.speed = 0f;
        this.hpBar.gameObject.SetActive(false);
        this.paragraphText.color = Color.clear;
        RenderingModeChanger.SetMaterialRenderingMode(this.graphRenderer.material, MaterialRenderingMode.Opaque);
    }

    // Update is called once per frame
    void Update()
    {
        this.updateTheNumOfCurrentParts();
        this.updateFireballsInStateMachine();
        this.updateHPbarAndStateMachine();
        RuntimeGlobalDictionary.SetVariable("BossHP", (float)this.hp);
        if (this.hp <= 0 && !this.isDying)
            this.Die();
    }
    public void Born(Wave wave)
    {
        this.is_on_stage = true;  // After the dragon fly away, this variable will become false, and let the wave machine proceed on the following wave.
        this.graphAnimator.speed = this.graphAnimatorSpeed;
        this.animator.speed = this.animatorSpeed;
        this.hpBar.gameObject.SetActive(true);
        this.wave = wave;
    }
    public void Leave()
    {
        this.is_on_stage = false;
        this.graphAnimator.speed = 0f;
        this.animator.speed = 0f;
        this.hpBar.gameObject.SetActive(false);
    }
    public void Die()
    {
        this.isDying = true;
        StartCoroutine(_dieProcess());
    }
    IEnumerator _dieProcess()
    {
        // Stuck the dragon first
        this.SetFreeze(true);
        foreach (EnemypartFireballController part in this.enemyparts)
        {
            part.PauseCountdown();
        }

        // Flash the first time
        FlashController.instance.StartHardFlash(Color.white, 0.06f);
        yield return new WaitForSeconds(.7f);

        // Flash the second time
        FlashController.instance.StartHardFlash(Color.white, 0.06f);
        this.paragraphText.color = Color.clear;
        this.textSticker.UnstickPosition();
        this.fireballSystem.clearAllParts(); // Clear all the enemy parts, and this sets this.fireballSystem.currentParts = 0
        yield return new WaitForSeconds(1.7f);

        // Shake!!!!
        ShakeInstance tmp = Shaker.ShakeAll(this.DragonDie_preset);
        float fadeDuration = 5f;
        float elapsedTime = 0;
        Material graphMaterial = this.graphRenderer.material;
        Color startColor = graphMaterial.color;
        RenderingModeChanger.SetMaterialRenderingMode(graphMaterial, MaterialRenderingMode.Fade);
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            graphMaterial.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        float fadeout_duration = 1f;
        tmp.Stop(fadeout_duration, false);
        yield return new WaitForSeconds(fadeout_duration);
        this.Leave();
    }
    public void ShakeOfLeaving()
    {
        Shaker.ShakeAll(this.DragonFlyAwayShake_preset);
    }
    public void dropFireballs(Vector3 layPos)
    {
        Question question = WaveSystem.instance.AskForAQuestion(wave);
        this.fireballSystem.generateFireballForDragon(layPos, question);
    }
    public void partAttackForFlame()
    {
        StartCoroutine(this._partAttackForFlame());
    }

    void updateTheNumOfCurrentParts()
    {
        this.animator.SetInteger("currentParts", this.fireballSystem.currentParts);
    }
    IEnumerator _partAttackForFlame()
    {
        this.enemyparts = new List<EnemypartFireballController>();  // Clear the enemyparts-object-recorder.

        // Pickup surrounded vocabularies
        List<string> vocabularies_to_show = new List<string>();
        MatchCollection matches = Regex.Matches(this.wave.dragon_paragraph.article, "<.*?>");
        foreach (Match match in matches)
        {
            vocabularies_to_show.Add(match.Value);
        }

        // Initially process the article (Replace <vocabularies> with [    ])
        List<string> initial_tags = new List<string>();
        for (int i = 0; i < this.wave.dragon_paragraph.vocabularies.Count; i++)
            initial_tags.Add(null);
        string text_to_show = new string(this.wave.dragon_paragraph.article);
        text_to_show = Regex.Replace(text_to_show, @"<.*?>", "[" + blank + "]"); // make the blank size fixed, convenient for me

        // Get the indices of blanks
        List<int> blank_indexes = new List<int>();
        int pos_of_left_bracket = 0;
        for (int i = 0; i < text_to_show.Length; i++)
        {
            if (text_to_show[i] == '[')
            {
                pos_of_left_bracket = i;
            }
            else if (text_to_show[i] == ']')
            {
                blank_indexes.Add((pos_of_left_bracket + i) / 2);
            }
        }

        // Clean up the brackets
        for (int i = 0; i < text_to_show.Length; i++)
            if (text_to_show[i] == '[' || text_to_show[i] == ']')
                text_to_show = text_to_show.Remove(i, 1).Insert(i, " ");

        // Show the text(paragraph)
        this.paragraphText.SetText(text_to_show);
        this.paragraphText.color = this.textColor;
        this.textSticker.SetStickPosition();

        yield return null; // This delay frame is needed for the textinfo to update

        float[] durations = this.wave.dragonData.durationOfEnemyParts.Split(',')
                                  .Select(s => float.Parse(s.Trim()))
                                  .ToArray();
        if (durations.Length == 0)
        {
            Debug.LogError("JOHNLEE: Please specify the duration of each enemy part!!");
            while (true)
                yield return null;
        }

        bool isTimeUp = false;

        // Instantiate paragraph's to-answer-blanks(fireballs)
        for (int i = 0; i < vocabularies_to_show.Count; i++)
        {
            TMP_TextInfo textInfo = this.paragraphText.textInfo;
            TMP_CharacterInfo charInfo = textInfo.characterInfo[blank_indexes[i]];
            Vector3 charPosition = (charInfo.topRight + charInfo.bottomRight) * 0.5f;
            Vector3 worldPosition = this.paragraphText.transform.TransformPoint(charPosition) - new Vector3(0f, this.z_delta_enemy_part_position, 0f);
            float duration = durations[Mathf.Min(durations.Length - 1, i)];  // Get the ith duration, if possible.
            var part = this.fireballSystem.generateEnemyPartForDragon(
                this.paragraphText.transform, 
                worldPosition,
                duration,
                vocabularies_to_show[i],
                this.wave.dragon_paragraph.vocabularies[i]
            );
            part.onShoot += RecalculatePartPositionAndText;
            part.onTimeUp += () => {
                isTimeUp = true;
            };
            this.enemyparts.Add(part);
            this.updateTheNumOfCurrentParts();
        }

        /*等待玩家射擊*/
        while (!isTimeUp)
        {
            while (this.pauseTimer)
            {
                yield return null;
            }
            if (this.fireballSystem.currentParts <= 0 || this.hp <= 0)
            {
                yield return StartCoroutine(this.damaged());
                break;
            }
            yield return null;
        }
        /*_等待玩家射擊_*/

        this.AddParagraphRecordToResultSystem();
        yield return new WaitForSeconds(3f);
        this.paragraphText.color = Color.clear; // Clear the this.wave.dragon_paragraph text
        this.textSticker.UnstickPosition();

        int remain_parts = this.fireballSystem.currentParts;
        this.fireballSystem.clearAllParts(); // Clear all the enemy parts, and this sets this.fireballSystem.currentParts = 0
        if (remain_parts > 0)
        {
            yield return StartCoroutine(this.flame());
        }

        AnimatorCleaer.ResetAllTriggers(this.animator);
        this.animator.SetTrigger("finishAttack");
    }
    private void RecalculatePartPositionAndText()
    {
        StartCoroutine(_RecalculatePartPositionAndText());
    }
    IEnumerator _RecalculatePartPositionAndText()
    {
        // Initially process the article (Replace <vocabularies> with [    ])
        bool insideSubstring = false;
        int index_of_parts = 0;
        string text_to_show = new string(this.wave.dragon_paragraph.article);

        // Convert the answered blank into [various-sized] in advance 
        for (int i = 0; i < text_to_show.Length; i++)
        {
            if (text_to_show[i] == '<' && insideSubstring == false)
            {
                if (index_of_parts >= this.enemyparts.Count)
                {
                    Debug.LogError("The number of tags is supposed to be equal to the number of \'[\'s. Please double check");
                    Debug.Log(text_to_show);
                    Debug.Log("Count of tags: " + this.enemyparts.Count.ToString());
                }

                insideSubstring = true;
                if (!this.enemyparts[index_of_parts].ableShoot)  // If the enemy part is corrected
                    text_to_show = text_to_show.Remove(i, 1).Insert(i, "["); // Replace the character '<' into '['
            }
            else if (text_to_show[i] == '>')
            {
                insideSubstring = false;
                if (!this.enemyparts[index_of_parts].ableShoot)  // If the enemy part is corrected
                {
                    text_to_show = text_to_show.Remove(i, 1).Insert(i, "]"); // Replace the character '>' into ']'
                }
                index_of_parts++;  // Go on to the next part
            }
            else if (insideSubstring)
            {
                if (!this.enemyparts[index_of_parts].ableShoot)
                {
                    text_to_show = text_to_show.Remove(i, 1).Insert(i, " \u200A\u200A\u200A"); // Replace the character inside into '+'
                    i += 3;
                }
            }
        }
        text_to_show = Regex.Replace(text_to_show, @"<.*?>", "[" + blank + "]"); // make the blank size fixed, convenient for me
        //string pattern = @"(\+(?:\u200B\+)+)";  // Regular expression to match sequences of "+\u200B"
        //text_to_show = Regex.Replace(text_to_show, pattern, "<color=#0000>$1</color>");  // Make the "++++" become tranparent

        // Get the indices of blanks
        List<int> blank_indexes = new List<int>();
        int pos_of_left_bracket = 0;
        for (int i = 0; i < text_to_show.Length; i++)
        {
            if (text_to_show[i] == '[')
            {
                pos_of_left_bracket = i;
            }
            else if (text_to_show[i] == ']')
            {
                blank_indexes.Add((pos_of_left_bracket + i) / 2);
            }
        }

        // Clean up the brackets
        for (int i = 0; i < text_to_show.Length; i++)
            if (text_to_show[i] == '[' || text_to_show[i] == ']')
                text_to_show = text_to_show.Remove(i, 1).Insert(i, " ");

        // Show the text(paragraph)
        this.paragraphText.SetText(text_to_show);
        this.paragraphText.color = this.textColor;
        this.textSticker.SetStickPosition();

        yield return null; // This delay frame is needed for the textinfo to update

        // Re-position the parts
        for (int i = 0; i < this.enemyparts.Count; i++)
        {
            TMP_TextInfo textInfo = this.paragraphText.textInfo;
            TMP_CharacterInfo charInfo = textInfo.characterInfo[blank_indexes[i]];
            Vector3 charPosition = (charInfo.topRight + charInfo.bottomRight) * 0.5f;
            Vector3 worldPosition = this.paragraphText.transform.TransformPoint(charPosition) - new Vector3(0f, this.z_delta_enemy_part_position, 0f);
            this.enemyparts[i].transform.position = worldPosition;
        }
    }
    void AddParagraphRecordToResultSystem()
    {
        List<string> tags = new List<string>();
        bool mark = true;
        foreach (EnemypartFireballController part in this.enemyparts)
        {
            if (part.ableShoot)
                tags.Add("red");
            else
                tags.Add("#0000FF");
            mark = mark && (!part.ableShoot);  // only if all the parts are correct then mark will be true
        }
        string text = this.wave.dragon_paragraph.GetProcessedTextofParagraph(tags);
        if (WaveSystem.instance != null)
            ResultSystem.instance.AddRecord(
                    WaveSystem.instance.nowWaveIndexForPlayer.ToString(),
                    System.String.Join(", ", this.wave.dragon_paragraph.vocabularies),
                    text,
                    RecordType_inResults.Boss,
                    mark
            );
        else
			ResultSystem.instance.AddRecord(
					"1",
					System.String.Join(", ", this.wave.dragon_paragraph.vocabularies),
					text,
					RecordType_inResults.Boss,
					mark
			);
	}
    void updateHPbarAndStateMachine()
    {
        this.hpBar.SetHP(this.hp);
        this.animator.SetFloat("hpRatio", this.hp / this.maxHP);
    }
    public void AddHP(int deltaHP)
    {
        if (this.hp > 0)
        {
            StartCoroutine(this._AddHP(deltaHP));
        }
    }

    IEnumerator _AddHP(int deltaHP)
    {
        this.hp += deltaHP;
        yield return null;
        // popup damage number
        this.damagePopup.CreateDamagePopup(deltaHP, this.hpBar.GetFilling());
    }

    //Graph Animation Controller
    public void Graph_SetTrigger(string triggerName)
    {
        AnimatorCleaer.ResetAllTriggers(this.graphAnimator);
        this.graphAnimator.SetTrigger(triggerName);
    }
    public void SwiftToLeftStarter()
    {
        StartCoroutine(this._swiftToLeft());
    }
    public void SwiftToRightStarter()
    {
        StartCoroutine(this._swiftToRight());
    }
    IEnumerator _swiftToLeft()
    {
        yield return null;
        for (int i = this.fireballSystem.generateTransforms.Count - 1; i >= 4 - (int)this.wave.dragonData.swiftLeftNumOfVocabularies; i--)
        {
            while (this.layPoint.position.x > this.fireballSystem.generateTransforms[i].position.x)  // wait until the dragon swifts to the left of the
                                                                                                     // generate transform
            {
                yield return null;
            }
            this.dropFireballs(new Vector3(this.fireballSystem.generateTransforms[i].position.x, layPoint.position.y, this.fireballSystem.generateTransforms[i].position.z));  // drop a fireball on the passed point

        }
    }
    IEnumerator _swiftToRight()
    {
        yield return null;
        for (int i = 0; i < (int)this.wave.dragonData.swiftRightNumOfVocabularies; i++)
        {
            while (this.layPoint.position.x < this.fireballSystem.generateTransforms[i].position.x)  // wait until the dragon swifts to the right of the
                                                                                                     // generate transform
            {
                yield return null;
            }
            this.dropFireballs(new Vector3(this.fireballSystem.generateTransforms[i].position.x, layPoint.position.y, this.fireballSystem.generateTransforms[i].position.z));  // drop a fireball on the passed point

        }
    }
    IEnumerator flame()
    {
        AnimatorCleaer.ResetAllTriggers(this.graphAnimator);
        this.Graph_SetTrigger("flame");
        
        yield return new WaitForSeconds(2f);
        while (this.graphAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fly Flame Attack (This name is dependent in DragonController_cs)"))
        {
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void SetWildFireOnTree(int target)
    {
        this.dropFireballs(this.flame_targets[target].position);
    }
    IEnumerator damaged()
    {
        AnimatorCleaer.ResetAllTriggers(this.graphAnimator);
        this.Graph_SetTrigger("damaged");
        yield return new WaitForSeconds(2f);
        while (this.graphAnimator.GetCurrentAnimatorStateInfo(0).IsName("Damaged"))
        {
            yield return new WaitForSeconds(0.1f);
        }
        this.Graph_SetTrigger("damagedFin");
    }
    public void switchFlameParticle(bool st)
    {
        foreach (var item in this.flameParticles)
        {
            var em = item.emission;
            em.enabled = st;
        }
    }
    void updateFireballsInStateMachine()
    {
        this.animator.SetInteger("curBalls", this.fireballSystem.fire_onScreen.Count);
    }
    public void SetFreeze(bool set)
    {
        if (set == true)
        {
            this.pauseTimer = true;
            this.graphAnimator.speed = 0f;
            this.animator.speed = 0f;
            this.flameGraphForPause.Pause();
        }
        else
        {
            this.pauseTimer = false;
            this.graphAnimator.speed = this.graphAnimatorSpeed;
            this.animator.speed = this.animatorSpeed;
            this.flameGraphForPause.Play();
        }
    }
}
