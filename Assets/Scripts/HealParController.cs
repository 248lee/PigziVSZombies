using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HealParController : MonoBehaviour
{
    Rigidbody2D rid;
    float speed;
    bool fallOnTree;
    [SerializeField] float accelerationValue = 2f;
    [SerializeField] GameObject healEffect;
    // Start is called before the first frame update
    void Start()
    {
        this.rid = GetComponent<Rigidbody2D>();
        this.speed = 0f;
        this.fallOnTree = false;
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
        if (collision.tag == "floor")
        {
            collision.GetComponentInParent<TreeController>().heal();
            GameObject tempEffect = Instantiate(this.healEffect, transform.position, Quaternion.identity);
            Destroy(tempEffect, 0.5f);
            this.fallOnTree = true;
            Destroy(gameObject, 0.3f);
        }
    }
    public void startFall()
    {
        transform.parent = null;
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
