using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AnimalController : MonoBehaviour
{
    private bool isBurned_momentary;
    private bool is_counting_down;
    // Start is called before the first frame update
    void Start()
    {
        this.isBurned_momentary = false;
        this.is_counting_down = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
        if (!isBurned_momentary)  // Check whither it is legal for keeping countdown
            this.is_counting_down = false;
        this.isBurned_momentary = false;
    }
    public void Death()
    {
        Debug.Log(gameObject.name + "is dying!");
        GameflowSystem.instance.SetPause();
    }
    public bool StartBurned()
    {
        this.isBurned_momentary = true;
        if (!is_counting_down)
        {
            StartCoroutine(BurnedCountdown());
            return true;
        }
        return false;
    }
    IEnumerator BurnedCountdown()
    {
        string timer_name = gameObject.name + "_burn_countdown";
        this.is_counting_down = true;
        Timers.SetTimer(timer_name, .3f);

        while (is_counting_down && Timers.isTimerFinished(timer_name))
        {
            yield return null;
        }
        
        if (is_counting_down)  // If the countdown is complete
        {
            this.Death();
        }
        else  // If the countdown is interupted
        {
            
        }
    }
}
