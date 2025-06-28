using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimalGraphController : MonoBehaviour
{
    // Set random delay for triggering the animator parameter "idle_randomness
    [SerializeField]
    private float randomDelayMin = 4f;
    [SerializeField]
    private float randomDelayMax = 10f;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        this.animator = GetComponent<Animator>();
        StartCoroutine(TriggerIdleRandomCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Repeat triggering the animator parameter "idle_randomness" with random delay
    private IEnumerator TriggerIdleRandomCoroutine()
    {
        float randomDelay = Random.Range(0f, 2f);
        yield return new WaitForSeconds(randomDelay);
        while (true)
        {
            this.animator.SetTrigger("idle_randomness");
            randomDelay = Random.Range(randomDelayMin, randomDelayMax);
            yield return new WaitForSeconds(randomDelay);
        }
    }
}
