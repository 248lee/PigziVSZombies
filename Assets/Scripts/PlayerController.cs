using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using JohnUtils;

public class PlayerController : MonoBehaviour
{
    //---------------------public---------------------//
    public string playerValue;
    public int waters;
    public FireballSysrem fireballsystem;

    //---------------------private---------------------//
    //WaterSystem watersystem;
    DragonController dragonController;
    [SerializeField] float frozenTime = 5f;
    [SerializeField] GameObject bullet_empty_shoot;
    [SerializeField] Transform bulletStartPoint;
    [SerializeField] Text waterAmountText;
    [SerializeField] GameObject normalPP, frozenPP;
    [SerializeField] Animator UIanimator;
    bool isFreezing;
    int numOfIncorrect = 0;
    void Start()
    {
        this.isFreezing = false;
        this.waters = 0;
        this.normalPP.layer = 13;
        this.frozenPP.layer = 14;
        this.playerValue = "";
        this.fireballsystem = FindObjectOfType<FireballSysrem>();
        //this.watersystem = FindObjectOfType<WaterSystem>();
        this.dragonController = FindObjectOfType<DragonController>();
        this.waterAmountText.text = "x 0";
        this.numOfIncorrect = 0;
        Physics2D.IgnoreLayerCollision(11, 11, true);
        Physics2D.IgnoreLayerCollision(11, 12, true);
        Physics2D.IgnoreLayerCollision(12, 12, true);
        AutoCompleteInput.instance.inputCompleteHandler += this.ActivateShoot;  // Suscribe the Shoot Activation (previously called "preshoot" onto the AutoCompleteInput class.
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.isFreezing && Input.GetKeyDown(KeyCode.Z))
        {
            if (this.waters > 0)
            {
                StartCoroutine(this.FrozeProcess());
                this.waters--;
            }
        }
        
        // Initialize
        this.playerValue = "";

        // answer
        //if (GameflowSystem.instance.is_pausing == false && Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (valueInput.Count != 0)
        //    {
        //        this.preShoot();
        //    }
        //}

        // pause
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (!GameflowSystem.instance.is_pausing)
                GameflowSystem.instance.SetPause();
            else
                GameflowSystem.instance.SetUnpaused();
        }

        // Water¨t²Î
        this.updateWaterAmountToText();
    }
    void ActivateShoot(string input)
    {
        bool correct = false;
        for (int i = 0; i < this.fireballsystem.fire_onScreen.Count; i++)
        {
            if (correct && !(this.fireballsystem.fire_onScreen[i] is HealFireballController))
                continue;
            if (this.fireballsystem.fire_onScreen[i].question.vocabulary == input && this.fireballsystem.fire_onScreen[i].ableShoot)
            {
                correct = true;
                this.fireballsystem.fire_onScreen[i].correct(); // this completes the shoot animation
                this.shoot(); // this is fake
            }
        }
        //for (int i = 0; i < this.watersystem.waterBall_onScreen.Count; i++)
        //{
        //    if (this.watersystem.waterBall_onScreen[i].question.answer == this.playerValue)
        //    {
        //        correct = true;
        //        this.watersystem.waterBall_onScreen[i].correct();
        //        this.watersystem.waterBall_onScreen.RemoveAt(i);
        //        break;
        //    }
        //}

        if (!correct)
        {
            this.empty_shoot();
            this.numOfIncorrect++;
            Debug.Log($"{this.numOfIncorrect} empty shoots now!");
        }
    }
    void shoot()
    {

    }
    void empty_shoot()
    {
        GameObject temp_bullet = Instantiate(this.bullet_empty_shoot, this.bulletStartPoint.position, Quaternion.identity);
        temp_bullet.transform.LookAt(this.bulletStartPoint.position + new Vector3(0f, 1f, 0f));
        Destroy(temp_bullet, 0.7f);
    }
    void frozen()
    {
        this.fireballsystem.SetFreeze(true);
        this.dragonController.SetFreeze(true);
        AnimatorCleaer.ResetAllTriggers(this.UIanimator);
        this.UIanimator.SetTrigger("frozen");
        this.normalPP.layer = 14;
        this.frozenPP.layer = 13;
    }
    IEnumerator deFrozen()
    {
        AnimatorCleaer.ResetAllTriggers(this.UIanimator);
        this.UIanimator.SetTrigger("defroze");
        this.normalPP.layer = 13;
        this.frozenPP.layer = 14;
        yield return new WaitForSeconds(2f);
        this.fireballsystem.SetFreeze(false);
        this.dragonController.SetFreeze(false);
    }
    IEnumerator FrozeProcess()
    {
        this.isFreezing = true;
        this.frozen();
        yield return new WaitForSeconds(this.frozenTime);
        yield return StartCoroutine(this.deFrozen());
        this.isFreezing = false;
    }
    void updateWaterAmountToText()
    {
        this.waterAmountText.text = "x " + this.waters.ToString();
    }
}
