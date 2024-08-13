using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFireballController : FallingFireballController
{
    private bool is_onTree;
    [SerializeField] private ParticleSystem fireball_particle, wildfire_particle;
    public TreeController burningTree;
    protected override void InitProcess()
    {
        gameObject.layer = 12;
        this.is_onTree = false;
        this.realSpeed = this.speed;
        this.questionText.text = question.sentence;
        this.ableShoot = true;
        this.ableToBeDestroyed = false;
        this.fireball_particle.gameObject.SetActive(true); // Light up the particle effects
        //this.progressBar.gameObject.SetActive(false);
    }
    protected override void PostProcessAfterCorrect()
    {
        this.putOutFireball();
        this.SetAbleToBeDestroyed();
        if (this.burningTree != null)
            this.burningTree.SetPersistentDamage(false);
    }
    protected override void pause()
    {
        if (this.is_onTree == false)
        {
            this.realSpeed = 0f;
            this.fireball_particle.Pause();
        }
        else
        {
            this.burningTree.burningFire = null;
            this.ableShoot = false;
            this.SetAbleToBeDestroyed();
            Destroy(this.wildfire_particle.gameObject);
            this.questionText.text = "";
        }
    }
    protected override void unpause()
    {
        this.realSpeed = this.speed;
        this.fireball_particle.Play();
    }
    public override void wrong()
    {
        if (this.ableShoot)
        {
            this.is_onTree = true;
            this.realSpeed = 0f;
            this.putOutFireball();
            this.burningTree.SetPersistentDamage(true, 20f);
            this.wildfire_particle.gameObject.SetActive(true);
            this.questionText.color = Color.black;
        }
    }
    public override void wrong_withFire()
    {
        Debug.Log("wrong with fire");
        this.questionText.SetText(this.question.GetRealSentenceWithColor("blue")); // show out the correct answer
        this.ableShoot = false;
        this.SetAbleToBeDestroyed();
        this.putOutFireball();
        transform.position += new Vector3(0f, 0f, -1f);
        // this.realSpeed = 0f; // stop the fireball from falling down
    }
    public void WildfireOnDeadTree() // For FireFireballController
    {
        this.questionText.SetText(this.question.GetRealSentenceWithColor("blue")); // show out the correct answer
        this.ableShoot = false;
        this.SetAbleToBeDestroyed();
        this.putOutWildFire();
        // transform.position += new Vector3(0f, 0f, 1f);
        this.realSpeed = 0f; // stop the fireball from falling down
    }
    private void putOutFireball()
    {
        this.rid.velocity = Vector2.zero;
        var em1 = this.fireball_particle.emission;
        em1.enabled = false;
        var em2 = this.wildfire_particle.emission;
        em2.enabled = false;
    }
    private void putOutWildFire()
    {
        this.wildfire_particle.GetComponent<WildFireTurnOffer>().TurnOffWildFire();
    }
}
