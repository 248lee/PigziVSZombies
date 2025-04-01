using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JohnUtils;

public class EnemypartFireballController : FireballController
{
    public event EventHandlerWithVoid onShoot;
    public event EventHandlerWithVoid onTimeUp;
    private float maxTime = 5f, nowTime = 0f;
    private bool pauseTimer = false;
    [SerializeField] private ProgressBarController progressBar;

    protected override void InitProcess()
    {
        gameObject.layer = 12;
        this.questionText.text = question.sentence;
        this.ableShoot = true;
        this.pauseTimer = false;
        this.ableToBeDestroyed = false;
        this.progressBar.gameObject.SetActive(true);
        if (this.maxTime < 0f)
        {
            Debug.LogError("請設定enemy part存在時間!");
        }
        else
        {
            StartCoroutine(this.countTime());
        }
    }
    protected override void PostProcessAfterCorrect()
    {
        this.progressBar.gameObject.SetActive(false);
        int minusHP = 10 + Random.Range(-5, 5);
        this.fireballSystem.bossDragon.AddHP(-minusHP);
        this.fireballSystem.currentParts--;
        this.onShoot();
    }
    protected override void pause()
    {
        this.pauseTimer = true;
    }
    protected override void unpause()
    {
        this.pauseTimer = false;
    }
    public void setMaxTimeForPart(float duration) // called by FireballSystem
    {
        this.maxTime = duration;
    }
    public void partWrong() // called by FireballSystem
    {
        this.ableShoot = false;
        this.SetAbleToBeDestroyed();
        this.questionText.text = "";
    }
    IEnumerator countTime()
    {
        this.nowTime = 0f;
        while (this.nowTime <= this.maxTime && this.ableShoot)
        {
            while (this.pauseTimer)
            {
                yield return null;
            }
            this.nowTime += Time.deltaTime;
            float ratio = this.nowTime / this.maxTime;
            if (ratio > 1f)
                ratio = 1f;
            this.progressBar.setProgressBar(ratio);
            yield return null;
        }
        this.progressBar.gameObject.SetActive(false);
        if (this.ableShoot)  // If the player has not answer this yet
        {
            this.onTimeUp();
            this.questionText.SetText(this.question.GetRealSentenceWithColor("red"));
        }
    }
}
