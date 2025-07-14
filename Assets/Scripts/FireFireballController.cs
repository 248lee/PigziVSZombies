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
        if (this.burningTree != null && this.burningTree.burningFire == this)
            this.burningTree.SetBurningFire(null);

        // Add record to ResultSystem
        string text;
        if (this.is_onTree)
            text = this.question.GetRealSentenceWithColor("#000080");
        else
            text = this.question.GetRealSentenceWithColor("#0000FF");
        if (WaveSystem.instance != null)
            ResultSystem.instance.AddRecord (
                    WaveSystem.instance.nowWaveIndexForPlayer.ToString(),
                    this.question.vocabulary,
                    text,
                    RecordType_inResults.Fireball,
                    true
            );
        else
            ResultSystem.instance.AddRecord(
                    "1",
                    this.question.vocabulary,
                    text,
                    RecordType_inResults.Fireball,
                    true
            );
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
            this.burningTree.SetBurningFire(null);
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
    public override void Wrong()  // Ignite the wild fire.
    {
        if (this.ableShoot)
        {
            this.is_onTree = true;
            this.realSpeed = 0f;
            this.putOutFireball();
            this.burningTree.SetBurningFire(this);
            this.wildfire_particle.gameObject.SetActive(true);
            this.questionText.color = Color.black;
        }
    }
    public override void Wrong_onFireTree()
    {
        string text = this.question.GetRealSentenceWithColor("red");
        this.questionText.SetText(text); // show out the correct answer
        this.ableShoot = false;
        this.SetAbleToBeDestroyed();
        this.putOutFireball();
        transform.position += new Vector3(0f, 0f, -1f);
        // this.realSpeed = 0f; // stop the fireball from falling down
        if (WaveSystem.instance != null)
            ResultSystem.instance.AddRecord(
                WaveSystem.instance.nowWaveIndexForPlayer.ToString(),
                this.question.vocabulary,
                text,
                RecordType_inResults.Fireball,
                false
            );
        else
            ResultSystem.instance.AddRecord(
                "1",
                this.question.vocabulary,
                text,
                RecordType_inResults.Fireball,
                false
            );
    }
    public override void Wrong_onFloor()
    {
        string text = this.question.GetRealSentenceWithColor("red");
        this.questionText.SetText(text); // show out the correct answer
        this.ableShoot = false;
        this.SetAbleToBeDestroyed();
        this.putOutFireball();
        this.realSpeed = 0f;
        if (WaveSystem.instance != null)
            ResultSystem.instance.AddRecord(
                WaveSystem.instance.nowWaveIndexForPlayer.ToString(),
                this.question.vocabulary,
                text,
                RecordType_inResults.Fireball,
                false
            );
        else
            ResultSystem.instance.AddRecord(
                "1",
                this.question.vocabulary,
                text,
                RecordType_inResults.Fireball,
                false
            );
    }
    public void WildfireOnDeadTree() // Called when the wildfire burned all of the tree
    {
        string text = this.question.GetRealSentenceWithColor("red");
        this.questionText.SetText(text); // show out the correct answer
        this.ableShoot = false;
        this.SetAbleToBeDestroyed();
        this.putOutWildFire();
        this.burningTree.SetBurningFire(null);
        // transform.position += new Vector3(0f, 0f, 1f);
        this.realSpeed = 0f; // stop the fireball from falling down
        if (WaveSystem.instance != null)
            ResultSystem.instance.AddRecord(
                WaveSystem.instance.nowWaveIndexForPlayer.ToString(),
                this.question.vocabulary,
                text,
                RecordType_inResults.Fireball,
                false
            );
        else
            ResultSystem.instance.AddRecord(
                "1",
                this.question.vocabulary,
                text,
                RecordType_inResults.Fireball,
                false
            );
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<AnimalController>() != null)
        {
            collision.GetComponent<AnimalController>().StartBurned();

            string text = this.question.GetRealSentenceWithColor("red");
            if (WaveSystem.instance != null)
                ResultSystem.instance.AddRecord(
                    WaveSystem.instance.nowWaveIndexForPlayer.ToString(),
                    this.question.vocabulary,
                    text,
                    RecordType_inResults.Fireball,
                    false
                );
            else
                ResultSystem.instance.AddRecord(
                    "1",
                    this.question.vocabulary,
                    text,
                    RecordType_inResults.Fireball,
                    false
                );
        }
    }
}
