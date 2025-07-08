using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyScriptUnscaledTime : MonoBehaviour
{
    [SerializeField] private float secondsToDestroy = .7f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(secondsToDestroy));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Destroy(gameObject);
    }
}
