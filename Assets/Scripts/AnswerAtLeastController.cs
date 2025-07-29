using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ProgressBarController))]
public class AnswerAtLeastController : MonoBehaviour
{
    public string vocabulary { get; private set; }
    public ProgressCounter answeredCounter { get; private set; }
    private ProgressBarController progressBarUI;
    // Start is called before the first frame update
    void Start()
    {
        this.progressBarUI = GetComponent<ProgressBarController>();
    }
    public void Setup(string vocabulary, int maxTimes)
    {
        this.vocabulary = vocabulary;
        this.answeredCounter = new ProgressCounter(maxTimes, vocabulary);
        this.progressBarUI = GetComponent<ProgressBarController>();
    }
    public bool Answered()
    {
        if (this.answeredCounter != null)
        {
            bool is_effective = this.answeredCounter.CountUp();
            float progress = (float)this.answeredCounter.currentCount / (float)this.answeredCounter.fullCount;
            this.progressBarUI.setProgressBar(progress);
            return is_effective; // Return true if the count was successful, false if already at full count
        }
        else
        {
            Debug.LogError("This answer_at_least_controller is not initialized.");
            return false;
        }
    }
}
