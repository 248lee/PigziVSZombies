using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemypartFireballController : FireballController
{
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
            Debug.LogError("�г]�wenemy part�s�b�ɶ�!");
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
        this.progressBar.gameObject.SetActive(false);
        this.questionText.text = "";
        this.fireballSystem.currentParts--;
    }
    IEnumerator countTime() //�L�ɬy�{��fireballSystem�I�s�A���B�ȭp�ɥγ~
    {
        this.nowTime = 0f;
        while (this.nowTime <= this.maxTime)
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
    }
}