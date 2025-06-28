using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AnimalController : MonoBehaviour
{
    private bool isBurned_momentary;
    private bool is_counting_down;
    [SerializeField]
    private float burn_time = 0.3f; // Time to wait before death after being burned
    [SerializeField]
    private Animator graphAnimator; // Animator for the animal's graphical representation
    private List<string> requiredAnimatorStates = new List<string>
    {
        "idle",
        "walk",
        "run",
        "help"
    };
    // Start is called before the first frame update
    void Start()
    {
        this.isBurned_momentary = false;
        this.is_counting_down = false;
        // Check if the graphAnimator contains the following state-names: idle, walk, run, jump, death
        if (graphAnimator == null)
        {
            Debug.LogError("AnimalController: graphAnimator is not assigned!");
            return;
        }
        foreach (string state in this.requiredAnimatorStates)
        {
            if (!graphAnimator.HasState(0, Animator.StringToHash(state)))
            {
                Debug.LogError($"AnimalController: Animator does not contain the state '{state}'!");
            }
        }
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
        GameflowSystem.instance.StageLose();
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
        Timers.SetTimer(timer_name, this.burn_time);

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
    public void SetGraphAnimatorState(string state_name)
    {
        if (this.requiredAnimatorStates.Contains(state_name))
        {
            graphAnimator.Play(state_name);
        }
        else
        {
            Debug.LogError($"AnimalController: Animator does not contain the state '{state_name}'!");
        }
    }
    public void SetFaceRight()
    {
        transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0f, 1f), transform.up);  // Assuming the animal's facing right when the normal forward direction is along the Z-axis
    }
    public void SetFaceLeft()
    {
        transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0f, -1f), transform.up);  // Assuming the animal's facing left when the normal forward direction is along the Z-axis
    }
}
