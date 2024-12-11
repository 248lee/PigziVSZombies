using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeController : MonoBehaviour
{
    public FireFireballController burningFire;
    public bool is_onFire;
    public bool is_alive;
    public float max_hp = 100f, hp, damageRate = 20f;
    [SerializeField] HPBarController hpBar;
    [SerializeField] List<GameObject> Leafs;
    [SerializeField] DamagePopupController healPopup;
    FireballSysrem fs;

    // Start is called before the first frame update
    void Start()
    {
        this.is_alive = true;
        this.hp = this.max_hp;
        this.burningFire = null;
        this.fs = FindObjectOfType<FireballSysrem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.burningFire != null)
        {
            this.DamagingByFire();
        }
        this.SetTreeImage(this.hp / this.max_hp);
        if (this.burningFire != null && this.is_alive == false)
        {
            this.burningFire.WildfireOnDeadTree();  // This turns this.burningFire into null
        }

    }
    void FixedUpdate()
    {
        this.correctHP();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Fireball")
        {
            // Setup the incoming fireball
            FireFireballController fireball = collision.GetComponent<FireFireballController>();
            fireball.burningTree = this;

            if (this.burningFire == null)
            {
                if (!fireball.ableToBeDestroyed)
                {
                    if (fireball is FireFireballController fire)
                    {
                        fire.wrong();
                    }
                }
                
            }
            else
            {
                if (fireball is FireFireballController fire)
                    fire.wrong_withFire();
            }
        }
    }
    private void DamagingByFire()
    {
        if (this.is_alive)
        {
            this.hp -= this.damageRate * Time.deltaTime;
        }
    }
    private void SetTreeImage(float state)
    {
        int count = this.Leafs.Count;
        int leafs = Mathf.CeilToInt(count * state);
        if (leafs > 0)
        {
            for (int i = 0; i < count; i++)
            {
                if (i > leafs)
                {
                    this.Leafs[i].SetActive(false);
                }
                else
                {
                    this.Leafs[i].SetActive(true);
                }
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                this.Leafs[i].SetActive(false);
            }
        }
    }

    public void SetBurningFire(FireFireballController fire)
    {
        this.burningFire = fire;
        if (fire != null)
            this.damageRate = 20f;
        else
            this.damageRate = 0f;
    }
    public void ApplyHeal(float HP)
    {
        if (this.is_alive)
        {
            this.hp += HP;
            this.hp = Mathf.Min(this.hp, this.max_hp);
        }
    }
    public void Revive(float recoverHP)
    {
        if (!this.is_alive)
        {
            this.hp = recoverHP;
            this.is_alive = true;
        }
    }
    void correctHP()
    {
        if (this.hp > 0f)
        {
            this.is_alive = true;
        }
        else
        {
            this.is_alive = false;
            this.hp = 0f;
        }
        if (this.hp > this.max_hp)
        {
            this.hp = this.max_hp;
        }
        this.hpBar.SetMaxHP(this.max_hp);
        this.hpBar.SetHP(this.hp);
    }
}
