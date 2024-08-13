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
    public Question question = new Question("", "");
    public TextMeshProUGUI questionText;
    public bool ableShoot = true;
    public int index = -1;
    public bool ableToBeDestroyed;
    protected FireballSysrem fireballSystem;
    protected Rigidbody2D rid;
    private bool shootMeSignal = false;
    private bool isShootingMe = false;
    private GameObject temp_bullet;
    
    
    // Start is called before the first frame update
    void Start()
    {
        this.rid = GetComponent<Rigidbody2D>();
        this.fireballSystem = FindObjectOfType<FireballSysrem>();
        this.InitProcess();
        //if (this.type == TypeMode.Fireball)
        //{
        //    this.fireballInit();
        //}
        //if (this.type == TypeMode.Healball)
        //{
        //    this.healballInit();
        //}
        //else
        //{
        //    this.enemyPartInit();
        //}
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
                this.ableShoot = false;
                gameObject.layer = 11;
                

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
            if (isShootingMe) // the variable isShootingMe will be set to True by above
            {
                this.temp_bullet.transform.LookAt(transform);
                float progress = Timers.GetTimerPrgress("StartShooting" + gameObject.GetInstanceID());
                Vector3 original = fireballSystem.bulletStartPosition;
                temp_bullet.transform.position = original + (transform.position - original) * progress; // Movement of the bullet

                if (Timers.isTimerFinished("StartShooting" + gameObject.GetInstanceID())) // If the bullet hits the fire
                {
                    this.isShootingMe = false;
                    this.questionText.SetText(this.question.GetRealSentenceWithColor("red")); // show out the correct answer
                    Destroy(temp_bullet, 0.5f);
                    Instantiate(fireballSystem.dust, transform.position, Quaternion.identity);

                    // This is the post-process of answering correctly
                    this.PostProcessAfterCorrect();
                    if (this.type == TypeMode.EnemyPart)
                    {
                       
                    }
                    //if (this.type == TypeMode.Healball)
                    //    this.healPar.startFall();
                    //if (this.type != TypeMode.EnemyPart)
                    //{
                        
                    //}
                }
            }
        }
    }
    
    public void correct()
    {
        this.shootMeSignal = true;
    }
    public void DestroyMe()
    {
        if (ableToBeDestroyed && gameObject != null)
        {
            this.ableToBeDestroyed = false;
            Destroy(gameObject, 0.5f);
        }
    }
    public void SetAbleToBeDestroyed()
    {
        this.ableToBeDestroyed = true;
    }    
    
    
    
    
    protected virtual void InitProcess()
    {

    }
    protected virtual void PostProcessAfterCorrect()
    {

    }
    protected virtual void pause()
    {

    }
    protected virtual void unpause()
    {

    }
    
    //private void healballInit()
    //{
    //    gameObject.layer = 12;
    //    this.is_onTree = false;
    //    this.realSpeed = this.speed;
    //    this.questionText.text = question.sentence;
    //    this.ableShoot = true;
    //    this.ableToBeDestroyed = false;
    //    this.fireball_particle.gameObject.SetActive(true);
    //    //this.progressBar.gameObject.SetActive(false);
    //    this.healPar.gameObject.SetActive(true);
    //}
    
    public void SetPause(bool set)
    {
        if (set == true)
        {
            this.pause();
            //if (this.type == TypeMode.Fireball || this.type == TypeMode.Healball)
            //{
                
            //}
            //else
            //{
            //    this.pauseTimer = true;
            //}
        }
        else
        {
            this.unpause();
            //if (this.type == TypeMode.Fireball || this.type == TypeMode.Healball)
            //{
                
            //}
            //else
            //    this.pauseTimer = false;
        }
    }
}
