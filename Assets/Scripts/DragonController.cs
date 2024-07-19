using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TMPro;
public class DragonController : MonoBehaviour
{
    [SerializeField] FireballSysrem fireballSystem;
    [SerializeField] Transform layPoint;
    [SerializeField] Transform paragraphStartPoint;
    [SerializeField] Animator graphAnimator;
    [SerializeField] Animator animator;
    [SerializeField] List<ParticleSystem> flameParticles = new List<ParticleSystem>();
    [SerializeField] HPBarController hpBar;
    [SerializeField] int maxHP = 100, minusHP = 10;
    [SerializeField] ParticleSystem flameGraphForPause;
    [SerializeField] TextMeshProUGUI paragraphText;
    [SerializeField] GameObject testball;
    int hp;
    bool pauseTimer;
    float graphAnimatorSpeed, animatorSpeed;
    Wave wave;
    public bool is_on_stage = false;
    // Start is called before the first frame update
    void Start()
    {
        this.pauseTimer = false;
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
    }

    // Update is called once per frame
    void Update()
    {
        this.updateTheNumOfCurrentParts();
        this.updateFireballsInStateMachine();
        this.updateHPbarAndStateMachine();
    }
    public void Born(Wave wave)
    {
        this.is_on_stage = true;  // After the dragon fly away, this variable will become false, and let the wave machine proceed on the following wave.
        this.graphAnimator.speed = this.graphAnimatorSpeed;
        this.animator.speed = this.animatorSpeed;
        this.hpBar.gameObject.SetActive(true);
        this.wave = wave;
    }
    public void dropFireballs(Vector3 layPos)
    {
        this.fireballSystem.generateFireballForDragon(layPos, wave.questions[0]);
        wave.questions.RemoveAt(0); // pop out a question
    }
    public void partAttackForFlame(float countdowntime)  //倒數秒數請至state machine調整
    {
        StartCoroutine(this._partAttackForFlame(countdowntime));
    }
    List<int> generateRandomDifferInts(int n, int min, int max)
    {
        int range = max - min + 1;
        List<bool> tmp = new List<bool>();
        for (int i = 0; i < range; i++)
        {
            tmp.Add(false);
        }
        List<int> result = new List<int>();
        for (int i = 0; i < n; i++)
        {
            int chosen = Random.Range(0, range);
            while (tmp[chosen])
            {
                chosen = Random.Range(0, range);
            }
            tmp[chosen] = true;
            result.Add(min + chosen);
        }

        return result;
    }

    void updateTheNumOfCurrentParts()
    {
        this.animator.SetInteger("currentParts", this.fireballSystem.currentParts);
    }
    IEnumerator _partAttackForFlame(float duration)
    {
        List<int> blank_indexes = new List<int>();
        int pos_of_left_bracket = 0;
        string text_to_show = Regex.Replace(this.wave.dragon_paragraph.article, "<.*?>", "<                  >"); // make the blank modifiable, convenient for me
        bool insideSubstring = false;
        for (int i = 0; i < text_to_show.Length; i++)
        {
            if (text_to_show[i] == '<' && insideSubstring == false)
            {
                pos_of_left_bracket = i;
                insideSubstring = true;
                text_to_show = text_to_show.Remove(i, 1).Insert(i, " "); // Replace the character '<' into a space
            }
            else if (text_to_show[i] == '>')
            {
                blank_indexes.Add((i + pos_of_left_bracket) / 2);
                insideSubstring = false;
                text_to_show = text_to_show.Remove(i, 1).Insert(i, " "); // Replace the character '>' into a space
            }
        }
        this.paragraphText.SetText(text_to_show);
        this.paragraphText.color = Color.white;

        yield return null; // This delay frame is needed for the textinfo to update

        
        //Instantiate(this.testball, worldPosition, Quaternion.identity, this.paragraphText.transform);
        for (int i = 0; i < this.wave.dragon_paragraph.vocabularies.Count; i++)
        {
            TMP_TextInfo textInfo = this.paragraphText.textInfo;
            TMP_CharacterInfo charInfo = textInfo.characterInfo[blank_indexes[i]];
            Vector3 charPosition = (charInfo.topRight + charInfo.bottomRight) * 0.5f;
            Vector3 worldPosition = this.paragraphText.transform.TransformPoint(charPosition);
            this.fireballSystem.generateEnemyPartForDragon(this.transform, worldPosition, duration, this.wave.dragon_paragraph.vocabularies[i]);
            this.updateTheNumOfCurrentParts();
        }

        /*計算時間*/
        float nowTime = 0f;
        while (nowTime < duration)
        {
            while (this.pauseTimer)
            {
                yield return null;
            }
            nowTime += Time.deltaTime;
            if (this.fireballSystem.currentParts <= 0)
            {
                yield return StartCoroutine(this.damaged());
                break;
            }
            yield return null;
        }
        /*_計算時間_*/

        this.paragraphText.color = Color.clear; // Clear the this.wave.dragon_paragraph text
        FireballController[] enemyParts = GetComponentsInChildren<FireballController>();

        int remain_parts = this.fireballSystem.currentParts;
        this.fireballSystem.clearAllParts(); // Clear all the enemy parts, and this sets this.fireballSystem.currentParts = 0
        if (remain_parts > 0)
            yield return StartCoroutine(this.flame());

        AnimatorCleaer.ResetAllTriggers(this.animator);
        this.animator.SetTrigger("finishAttack");
    }
    void updateHPbarAndStateMachine()
    {
        this.hpBar.SetHP(this.hp);
        this.animator.SetFloat("hpRatio", this.hp / this.maxHP);
    }
    public void minusBlood()
    {
        if (this.hp > 0)
        {
            this.hp -= (this.minusHP + Random.Range(-5, 5));
        }
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
        for (int i = this.fireballSystem.generateTransforms.Count - 1; i >= 0; i--)
        {
            while (this.layPoint.position.x > this.fireballSystem.generateTransforms[i].position.x)  // wait until the dragon swifts to the left of the
                                                                                                     // generate transform
            {
                yield return null;
            }
            this.dropFireballs(new Vector3(layPoint.position.x, layPoint.position.y, this.fireballSystem.generateTransforms[i].position.z));  // drop a fireball on the passed point

        }
    }
    IEnumerator _swiftToRight()
    {
        yield return null;
        for (int i = 0; i < this.fireballSystem.generateTransforms.Count; i++)
        {
            while (this.layPoint.position.x < this.fireballSystem.generateTransforms[i].position.x)  // wait until the dragon swifts to the right of the
                                                                                                     // generate transform
            {
                yield return null;
            }
            this.dropFireballs(new Vector3(layPoint.position.x, layPoint.position.y, this.fireballSystem.generateTransforms[i].position.z));  // drop a fireball on the passed point

        }
    }
    IEnumerator flame()
    {
        AnimatorCleaer.ResetAllTriggers(this.graphAnimator);
        this.Graph_SetTrigger("flame");
        
        yield return new WaitForSeconds(2f);
        while (this.graphAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fly Flame Attack"))
        {
            yield return new WaitForSeconds(0.1f);
        }
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
    public void SetPause(bool set)
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
