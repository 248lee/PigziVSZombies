using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static KeyCode[] keys = { KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z };
    //---------------------public---------------------//
    public string playerValue;
    public int waters;
    public FireballSysrem fireballsystem;

    //---------------------private---------------------//
    //WaterSystem watersystem;
    DragonController dragonController;
    Stack valueInput = new Stack();
    [SerializeField] float frozenTime = 5f;
    [SerializeField]Text valueText;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletStartPoint;
    [SerializeField] Text waterAmountText;
    [SerializeField] GameObject normalPP, frozenPP;
    [SerializeField] Animator UIanimator;
    bool isFreezing;
    // Start is called before the first frame update
    void Start()
    {
        this.isFreezing = false;
        this.waters = 0;
        this.normalPP.layer = 13;
        this.frozenPP.layer = 14;
        this.playerValue = "";
        this.valueInput = new Stack();
        this.fireballsystem = FindObjectOfType<FireballSysrem>();
        //this.watersystem = FindObjectOfType<WaterSystem>();
        this.dragonController = FindObjectOfType<DragonController>();
        this.waterAmountText.text = "x 0";
        Physics2D.IgnoreLayerCollision(11, 11, true);
        Physics2D.IgnoreLayerCollision(11, 12, true);
        Physics2D.IgnoreLayerCollision(12, 12, true);
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
        
        //Initialize
        this.playerValue = "";
        //InputButton
        if (valueInput.Count < 20)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                valueInput.Push(1);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                valueInput.Push(2);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                valueInput.Push(3);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                valueInput.Push(4);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                valueInput.Push(5);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                valueInput.Push(6);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                valueInput.Push(7);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                valueInput.Push(8);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                valueInput.Push(9);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                valueInput.Push(0);
            }
            for (int i = 0; i < 26; i++)
            {
                if (Input.GetKeyDown(keys[i]))
                    valueInput.Push((char)('a' + i));
            }
        }
        if (valueInput.Count != 0)
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                valueInput.Pop();
            }
        }

        //valueUpdate
        if (valueInput.Count != 0)
        {
            foreach (char item in valueInput)
            {
                this.playerValue = item.ToString() + this.playerValue;
            }
        }
        this.valueText.text = this.playerValue;
        //answer
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (valueInput.Count != 0)
            {
                this.preShoot();
            }
        }

        //清除垃圾
        for (int i = 0; i < this.fireballsystem.fire_onScreen.Count; i++)
        {
            if (this.fireballsystem.fire_onScreen[i].ableToBeDestroyed)
            {
                this.fireballsystem.fire_onScreen[i].DestroyMe();
                this.fireballsystem.fire_onScreen.RemoveAt(i);
                i--;
            }

        }
        //Water系統
        this.updateWaterAmountToText();
    }
    void preShoot()
    {
        bool correct = false;
        this.valueInput.Clear();
        this.valueText.text = "";
        for (int i = 0; i < this.fireballsystem.fire_onScreen.Count; i++)
        {
            if (this.fireballsystem.fire_onScreen[i].question.vocabulary == this.playerValue && !this.fireballsystem.fire_onScreen[i].ableToBeDestroyed)
            {
                correct = true;
                this.fireballsystem.fire_onScreen[i].correct();
                this.fireballsystem.fire_onScreen.RemoveAt(i);
                this.shoot();
                break;
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
        }
    }
    void shoot()
    {

    }
    void empty_shoot()
    {
        GameObject temp_bullet = Instantiate(this.bullet, this.bulletStartPoint.position, Quaternion.identity);
        temp_bullet.transform.LookAt(this.bulletStartPoint.position + new Vector3(0f, 1f, 0f));
        Destroy(temp_bullet, 0.7f);
    }
    void frozen()
    {
        this.fireballsystem.SetPause(true);
        this.dragonController.SetPause(true);
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
        this.fireballsystem.SetPause(false);
        this.dragonController.SetPause(false);
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
