//�K����: ���@�U�O�o���wbullet�MbulletStartPoint
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public enum TypeMode
{
    Fireball,
    EnemyPart,
    Healball
}
public class FireballController : MonoBehaviour
{
    public TypeMode type = TypeMode.Fireball;
    public float speed = 0.1f, realSpeed;
    public Question question = new Question("", "");
    public TextMeshProUGUI questionText;
    public bool ableShoot = true;
    public ParticleSystem graph1, graph2;
    public TreeController burningTree;
    public int index = -1;
    public bool ableToBeDestroyed;
    [SerializeField] private ProgressBarController progressBar;
    [SerializeField] private FireballSysrem fireballSystem;
    [SerializeField] private HealParController healPar;
    private float maxTime = 5f, nowTime = 0f;
    private Rigidbody2D rid;
    private bool is_answered, is_onTree;
    private bool pauseTimer = false;
    private bool shootMeSignal = false;
    private bool isShootingMe = false;
    private GameObject temp_bullet;
    
    
    // Start is called before the first frame update
    void Start()
    {
        this.fireballSystem = FindObjectOfType<FireballSysrem>();
        if (this.type == TypeMode.Fireball)
        {
            this.fireballInit();
        }
        else if (this.type == TypeMode.Healball)
        {
            this.healballInit();
        }
        else
        {
            this.enemyPartInit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Dealing with signals
        if (shootMeSignal || isShootingMe)
        {
            if (shootMeSignal)
            {
                this.shootMeSignal = false;

                // This is the pre-process of answering correctly
                this.is_answered = true;
                this.ableToBeDestroyed = true;
                gameObject.layer = 11;
                if (this.burningTree != null)
                    this.burningTree.SetFireDamage(false);
                if (this.type == TypeMode.EnemyPart)
                    this.correctForPart();

                // shoot initialization 
                this.temp_bullet = Instantiate(fireballSystem.bullet, fireballSystem.bulletStartPosition, Quaternion.identity);
                
                this.temp_bullet.transform.LookAt(transform);
                this.temp_bullet.GetComponent<Rigidbody>().velocity = (transform.position - fireballSystem.bulletStartPosition).normalized * fireballSystem.shootAnimSpeed;
                
                float shootDuration = Vector3.Distance(transform.position, fireballSystem.bulletStartPosition) / fireballSystem.shootAnimSpeed;
                var main = this.temp_bullet.GetComponent<ParticleSystem>().main;
                main.startLifetime = shootDuration;
                Timers.SetTimer("StartShooting" + gameObject.GetInstanceID(), shootDuration);
                this.isShootingMe = true;
            }
            if (isShootingMe)
            {
                this.temp_bullet.transform.LookAt(transform);
                float progress = Timers.GetTimerPrgress("StartShooting" + gameObject.GetInstanceID());
                Vector3 original = fireballSystem.bulletStartPosition;
                temp_bullet.transform.position = original + (transform.position - original) * progress; // Movement of the bullet

                if (Timers.isTimerFinished("StartShooting" + gameObject.GetInstanceID()))
                {
                    this.isShootingMe = false;

                    Destroy(temp_bullet, 0.5f);
                    this.putOutFireball();
                    Instantiate(fireballSystem.dust, transform.position, Quaternion.identity);

                    // This is the post-process of answering correctly
                    if (this.type == TypeMode.EnemyPart)
                        this.fireballSystem.bossDragon.minusBlood();
                    if (this.type == TypeMode.Healball)
                        this.healPar.startFall();
                    this.questionText.text = question.sentence;
                    this.progressBar.gameObject.SetActive(false);
                    Destroy(gameObject, 0.5f);
                }
            }
        }
    }
    private void FixedUpdate()
    {
        this.rid.velocity = new Vector2(0f, this.realSpeed);
    }
    public void wrong()
    {
        this.is_onTree = true;
        this.realSpeed = 0f;
        this.putOutFireball();
        this.burningTree.SetFireDamage(true, 20f);
        this.graph2.gameObject.SetActive(true);
        this.questionText.color = Color.black;
    }
    public void wrong_withFire()
    {
        this.ableShoot = false;
        this.ableToBeDestroyed = true;
        this.putOutFireball();
        this.questionText.text = "";
    }
    public void FireOnDeadTree()
    {
        this.ableShoot = false;
        this.ableToBeDestroyed = true;
        this.putOutWildFire();
        this.questionText.text = "";
    }
    public void partWrong()
    {
        this.ableShoot = false;
        this.ableToBeDestroyed = true;
        this.progressBar.gameObject.SetActive(false);
        this.questionText.text = "";
        this.fireballSystem.currentParts--;
    }
    public void correct()
    {
        this.shootMeSignal = true;
    }
    public void correctForPart()
    {
        this.fireballSystem.currentParts--;
    }
    public void DestroyMe()
    {
        Destroy(gameObject, 0.5f);
    }
    public void setMaxTimeForPart(float duration)
    {
        this.maxTime = duration;
    }
    
    
    
    private void putOutFireball()
    {
        this.rid.velocity = Vector2.zero;
        var em1 = this.graph1.emission;
        em1.enabled = false;
        var em2 = this.graph2.emission;
        em2.enabled = false;
    }
    private void putOutWildFire()
    {
        this.graph2.GetComponent<WildFireTurnOffer>().TurnOffWildFire();
    }
    private void fireballInit()
    {
        gameObject.layer = 12;
        this.is_answered = false;
        this.is_onTree = false;
        this.rid = GetComponent<Rigidbody2D>();
        this.realSpeed = this.speed;
        this.questionText.text = question.sentence;
        this.ableShoot = true;
        this.ableToBeDestroyed = false;
        this.graph1.gameObject.SetActive(true);
        this.progressBar.gameObject.SetActive(false);
        this.healPar.gameObject.SetActive(false);
    }
    private void healballInit()
    {
        gameObject.layer = 12;
        this.is_answered = false;
        this.is_onTree = false;
        this.rid = GetComponent<Rigidbody2D>();
        this.realSpeed = this.speed;
        this.questionText.text = question.sentence;
        this.ableShoot = true;
        this.ableToBeDestroyed = false;
        this.graph1.gameObject.SetActive(true);
        this.progressBar.gameObject.SetActive(false);
        this.healPar.gameObject.SetActive(true);
    }
    private void enemyPartInit()
    {
        gameObject.layer = 12;
        this.is_answered = false;
        this.rid = GetComponent<Rigidbody2D>();
        this.realSpeed = 0f;
        this.questionText.text = question.sentence;
        this.ableShoot = true;
        this.pauseTimer = false;
        this.ableToBeDestroyed = false;
        this.graph1.gameObject.SetActive(false); 
        this.graph2.gameObject.SetActive(false);
        this.healPar.gameObject.SetActive(false);
        this.progressBar.gameObject.SetActive(true);
        if (this.maxTime < 0f)
        {
            Debug.LogError("�г]�wenemy part�s�b�ɶ�!");
        }
        else
        {
            StartCoroutine(this.countTime());
        }
    }
    IEnumerator countTime() //�L�ɬy�{��fireballSystem�I�s�A���B�ȭp�ɥγ~
    {
        this.nowTime = 0f;
        while (this.nowTime <= this.maxTime)
        {
            while (this.pauseTimer)
            {
                yield return null;
            }
            this.nowTime += Time.deltaTime;
            float ratio = this.nowTime / this.maxTime;
            if (ratio > 1f)
                ratio = 1f;
            this.progressBar.setProgressBar(ratio);
            yield return null;
        }
        
    }
    public void SetPause(bool set)
    {
        if (set == true)
        {
            if (this.type == TypeMode.Fireball || this.type == TypeMode.Healball)
            {
                if (this.is_onTree == false)
                {
                    this.realSpeed = 0f;
                    this.graph1.Pause();
                }
                else
                {
                    this.burningTree.burningFire = null;
                    this.ableShoot = false;
                    this.ableToBeDestroyed = true;
                    Destroy(this.graph2.gameObject);
                    this.questionText.text = "";
                }
            }
            else
            {
                this.pauseTimer = true;
            }
        }
        else
        {
            if (this.type == TypeMode.Fireball || this.type == TypeMode.Healball)
            {
                this.realSpeed = this.speed;
                this.graph1.Play();
            }
            else
                this.pauseTimer = false;
        }
    }
}