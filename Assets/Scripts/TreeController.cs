using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeController : MonoBehaviour
{
    public FireballController burningFire;
    public bool is_onFire;
    public bool is_alive;
    [SerializeField] float max_hp = 100f, hp, damageRate = 20f;
    [SerializeField] HPBarController hpBar;
    [SerializeField] List<GameObject> Leafs;
    bool isDamagedByFire;
    FireballSysrem fs;

    // Start is called before the first frame update
    void Start()
    {
        this.is_alive = true;
        this.hp = this.max_hp;
        this.burningFire = null;
        this.isDamagedByFire = false;
        this.fs = FindObjectOfType<FireballSysrem>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (this.isDamagedByFire)
        {
            this.DamagingByFire();
        }
        this.SetTreeImage(this.hp / this.max_hp);
        if (this.burningFire != null && this.is_alive == false)
        {
            this.burningFire.FireOnDeadTree();
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
            if (this.burningFire == null)
            {
                FireballController fireball = collision.GetComponent<FireballController>();
                if (!fireball.ableToBeDestroyed)
                {
                    this.burningFire = fireball;
                    fireball.burningTree = this;
                    fireball.wrong();
                }
                
            }
            else
            {
                FireballController fireball = collision.GetComponent<FireballController>();
                fireball.wrong_withFire();
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
        int leafs = Mathf.FloorToInt(count * state);
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

    public void SetFireDamage(bool isFired, float rate)
    {
        this.isDamagedByFire = isFired;
        this.damageRate = rate;
    }
    public void SetFireDamage(bool isFired)
    {
        this.isDamagedByFire = isFired;
        this.damageRate = 20f;
    }
    public void heal()
    {
        if (this.is_alive)
        {
            this.hp += this.max_hp * this.fs.healRatio;
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