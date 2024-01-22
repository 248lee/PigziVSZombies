using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DragonController : MonoBehaviour
{
    [SerializeField] FireballSysrem fireballSysrem;
    [SerializeField] Transform layPoint;
    [SerializeField] List<Transform> partsList = new List<Transform>();
    [SerializeField] Animator graphAnimator;
    [SerializeField] Animator animator;
    [SerializeField] int currentEnemyParts;
    [SerializeField] List<ParticleSystem> flameParticles = new List<ParticleSystem>();
    [SerializeField] HPBarController hpBar;
    [SerializeField] int maxHP = 100, minusHP = 10;
    [SerializeField] ParticleSystem flameGraphForPause;
    int hp;
    bool pauseTimer;
    float graphAnimatorSpeed, animatorSpeed;
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
        this.currentEnemyParts = 0;
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
    }

    // Update is called once per frame
    void Update()
    {
        this.updateTheNumOfCurrentParts();
        this.updateFireballsInStateMachine();
        this.updateHPbarAndStateMachine();
    }
    public void Born()
    {
        this.graphAnimator.speed = this.graphAnimatorSpeed;
        this.animator.speed = this.animatorSpeed;
        this.hpBar.gameObject.SetActive(true);
    }
    public void dropFireballs(Transform layTrans)
    {
        this.fireballSysrem.generateFireballForDragon(layTrans.position, new Question("tom", "tom"));
    }
    public void partAttackForFlame(int parts, float time)  //生成個數和秒數請至state machine調整
    {
        if (parts > this.partsList.Count)
        {
            Debug.LogError("生成太多個enemy part啦~");
            return;
        }
        StartCoroutine(this._partAttackForFlame(parts, time));
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
    IEnumerator _partAttackForFlame(int parts, float duration)
    {
        List<int> chosenTransNo = this.generateRandomDifferInts(parts, 0, this.partsList.Count - 1);
        /*Delete me later*/
        if (parts != chosenTransNo.Count)
        {
            Debug.LogError("這裡有問題");
        }
        /*_Delete me later_*/
        for (int i = 0; i < parts; i++)
        {
            this.fireballSysrem.generateEnemyPartForDragon(this.partsList[chosenTransNo[i]], duration);
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
            if (this.currentEnemyParts <= 0)
            {
                yield return StartCoroutine(this.damaged());
                break;
            }
            yield return null;
        }
        /*_計算時間_*/

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
