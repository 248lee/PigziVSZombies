using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HealParController : MonoBehaviour
{
    Rigidbody2D rid;
    float speed;
    bool fallOnTree;
    [SerializeField] float accelerationValue = .5f;
    [SerializeField] GameObject healEffect;
    public ChangeHPScript healScript = null;
    // Start is called before the first frame update
    void Start()
    {
        this.rid = GetComponent<Rigidbody2D>();
        this.speed = 0f;
        this.fallOnTree = false;
        var emmision = GetComponent<ParticleSystem>().emission;
        emmision.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        this.rid.velocity = new Vector2(0f, -this.speed);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "tree_ground")
        {
            TreeController targetTree = collision.GetComponentInParent<TreeController>();
            if (this.healScript == null)
            {
                Debug.LogError("JohnLee: You should assign a scriptable object called ChangeHPScript to this particle in order to apply damage!");
                return;
            }
            if (targetTree.is_alive)
            {
                float deltaHP = this.healScript.ComputeHPChange(targetTree.max_hp);
                this.healScript.ShowPopup(deltaHP, targetTree.transform);
                targetTree.ApplyHeal(deltaHP);
            }
            else
            {
                float deltaHP = this.healScript.ComputeHPChangeRevive(targetTree.max_hp);
                targetTree.Revive(deltaHP);
                StartCoroutine(this.ReviveTreePopup(deltaHP, targetTree));
            }
            
            GameObject tempEffect = Instantiate(this.healEffect, transform.position, Quaternion.identity);
            Destroy(tempEffect, 1.5f);
            this.fallOnTree = true;
            Destroy(gameObject, 0.3f);
        }
    }
    IEnumerator ReviveTreePopup(float recoverHP, TreeController targetTree)
    {
        this.healScript.ShowPopupRevive(targetTree.transform);
        yield return new WaitForSeconds(.25f);
        this.healScript.ShowPopup(recoverHP, targetTree.transform);
    }
    public void StartFall()
    {
        transform.parent = null;
        var emmision = GetComponent<ParticleSystem>().emission;
        emmision.enabled = true;
        StartCoroutine(this.accelerating());
    }
    IEnumerator accelerating()
    {
        while (!this.fallOnTree)
        {
            this.speed += Time.deltaTime * this.accelerationValue;
            yield return null;
        }
        this.speed = 0;
    }
}
