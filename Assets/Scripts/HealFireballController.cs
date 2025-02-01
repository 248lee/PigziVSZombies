using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HealFireballController : FallingFireballController
{
    [SerializeField] HealParController falling_potion;
    [SerializeField] ParticleSystem healball_particle;
    [SerializeField] ChangeHPScript healScript = null;
    protected override void Update()
    {
        if (this.ableShoot && transform.position.y < this.fireballSystem.healballBound)
        {
            this.Wrong();
            string text = this.question.GetRealSentenceWithColor("red");
            ResultSystem.instance.AddRecord(
                WaveSystem.instance.nowWaveIndex.ToString(),
                this.question.vocabulary,
                text,
                RecordType_inResults.Healball,
                false
            );
        }
        base.Update();
    }
    protected override void InitProcess()
    {
        this.realSpeed = this.speed;
        this.questionText.text = question.sentence;
        this.ableShoot = true;
        this.ableToBeDestroyed = false;
        this.healball_particle.gameObject.SetActive(true); // Light up the particle effects
    }
    protected override void PostProcessAfterCorrect()
    {
        this.SetAbleToBeDestroyed();
        this.healball_particle.gameObject.SetActive(false); // Turn off the particle effects
        falling_potion.healScript = this.healScript;
        falling_potion.StartFall();

        // Add record to ResultSystem
        string text = this.question.GetRealSentenceWithColor("#0000FF");
        ResultSystem.instance.AddRecord(
            WaveSystem.instance.nowWaveIndex.ToString(),
            this.question.vocabulary,
            text,
            RecordType_inResults.Healball,
            true
        );

        // Wrong out other fireballs on the same row
        List<HealFireballController> healballs_onScreen = this.fireballSystem.fire_onScreen.OfType<HealFireballController>().ToList();
        foreach(HealFireballController healball in healballs_onScreen)
        {
            if (healball != this && healball.transform.position.y == transform.position.y)
            {
                healball.Wrong();
                // Add record to ResultSystem
                text = healball.question.GetRealSentenceWithColor("#000080");
                ResultSystem.instance.AddRecord(
                    WaveSystem.instance.nowWaveIndex.ToString(),
                    healball.question.vocabulary,
                    text,
                    RecordType_inResults.Healball,
                    true
                );
            }
        }
    }
    protected override void pause()
    {
        this.realSpeed = 0f;
        this.healball_particle.Pause();
    }
    protected override void unpause()
    {
        this.realSpeed = this.speed;
        this.healball_particle.Play();
    }
    public override void Wrong()
    {
        // Take out the particle system. It should be destroyed independently in order to wait for fading-out.
        this.healball_particle.transform.parent = null;
        var particle_effects = this.healball_particle.GetComponentsInChildren<ParticleSystem>();
        var em = this.healball_particle.emission;
        em.enabled = false;
        foreach (var pe in particle_effects)
        {
            em = pe.emission;
            em.enabled = false;
        }
        Destroy(this.healball_particle.gameObject, 1.5f);

        this.ableShoot = false;
        this.SetAbleToBeDestroyed();
        this.questionText.SetText(this.question.GetRealSentenceWithColor("red")); // show out the correct answer
    }
    public override void Wrong_onFireTree()
    {
        Debug.LogError("This method shouldn't be called. There may be some bugs.");
    }
    public override void Wrong_onFloor()
    {
        Debug.LogError("This method shouldn't be called. There may be some bugs.");
    }
}
