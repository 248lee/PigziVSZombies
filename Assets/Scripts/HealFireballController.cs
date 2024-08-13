using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealFireballController : FallingFireballController
{
    [SerializeField] HealParController falling_potion;
    [SerializeField] ParticleSystem healball_particle;
    protected override void Update()
    {
        if (this.ableShoot && transform.position.y < this.fireballSystem.healballBound)
            this.wrong();
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
        falling_potion.StartFall();
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
    public override void wrong()
    {
        var em = this.healball_particle.emission;
        em.enabled = false;
        this.ableShoot = false;
        this.SetAbleToBeDestroyed();
        this.questionText.SetText(this.question.GetRealSentenceWithColor("blue")); // show out the correct answer
    }
    public override void wrong_withFire()
    {
        Debug.LogError("This method shouldn't be called. There may be some bugs.");
    }
}
