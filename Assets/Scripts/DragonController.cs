using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class DragonController : MonoBehaviour
{
    [SerializeField] FireballSysrem fireballSysrem;
    [SerializeField] Transform layPoint;
    [SerializeField] Transform paragraphStartPoint;
    [SerializeField] Animator graphAnimator;
    [SerializeField] Animator animator;
    [SerializeField] int currentEnemyParts;
    [SerializeField] List<ParticleSystem> flameParticles = new List<ParticleSystem>();
    [SerializeField] HPBarController hpBar;
    [SerializeField] int maxHP = 100, minusHP = 10;
    [SerializeField] ParticleSystem flameGraphForPause;
    [SerializeField] TextMeshProUGUI paragraphText;
    [SerializeField] GameObject testball;
    int hp;
    bool pauseTimer;
    float graphAnimatorSpeed, animatorSpeed;
    Paragraph paragraph;
    // Start is called before the first frame update
    void Start()
    {
        this.pauseTimer = false;
        this.hp = this.maxHP;
        this.hpBar.SetMaxHP(this.maxHP);
        if(this.flameParticles.Count == 0)
        {
            Debug.LogError("��flame particle�Զi�Ӫ�!");
        }
        this.switchFlameParticle(false);
        this.currentEnemyParts = 0;
        this.animator = GetComponent<Animator>();
        if (this.graphAnimator == null)
        {
            Debug.LogError("�F�A��graphAnimator�n�O�o�Զi�Ӫ�");
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
    public void Born(Paragraph paragraph)
    {
        this.graphAnimator.speed = this.graphAnimatorSpeed;
        this.animator.speed = this.animatorSpeed;
        this.hpBar.gameObject.SetActive(true);
        this.paragraph = paragraph;
        this.paragraphText.SetText(paragraph.article);

    }
    public void dropFireballs(Transform layTrans)
    {
        this.fireballSysrem.generateFireballForDragon(layTrans.position, new Question("tom", "tom"));
    }
    public void partAttackForFlame(float time)  //�ͦ��ӼƩM��ƽЦ�state machine�վ�
    {
        StartCoroutine(this._partAttackForFlame(time));
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
        this.currentEnemyParts = this.fireballSysrem.currentParts;
        this.animator.SetInteger("currentParts", this.currentEnemyParts);
    }
    IEnumerator _partAttackForFlame(float duration)
    {
        this.paragraphText.color = Color.white;
        TMP_TextInfo textInfo = this.paragraphText.textInfo;
        TMP_CharacterInfo charInfo = textInfo.characterInfo[10];
        Debug.Log(charInfo.character);
        Vector3 charPosition = (charInfo.topRight + charInfo.bottomRight) * 0.5f;
        Vector3 worldPosition = this.paragraphText.transform.TransformPoint(charPosition);
        Instantiate(this.testball, worldPosition, Quaternion.identity, this.paragraphText.transform);
        for (int i = 0; i < paragraph.vocabularies.Count; i++)
        {
            this.fireballSysrem.generateEnemyPartForDragon(this.paragraphStartPoint, duration, paragraph.vocabularies[i]);
            this.updateTheNumOfCurrentParts();
        }

        /*�p��ɶ�*/
        float nowTime = 0f;
        while (nowTime < duration)
        {
            while (this.pauseTimer)
            {
                yield return null;
            }
            nowTime += Time.deltaTime;
            if (this.currentEnemyParts <= 0)
            {
                yield return StartCoroutine(this.damaged());
                break;
            }
            yield return null;
        }
        /*_�p��ɶ�_*/

        this.paragraphText.color = Color.clear;
        if (this.currentEnemyParts > 0)
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
    IEnumerator flame()
    {
        this.fireballSysrem.clearAllParts();
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
        this.fireballSysrem.clearAllParts();
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
        this.animator.SetInteger("curBalls", this.fireballSysrem.fire_onScreen.Count);
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
