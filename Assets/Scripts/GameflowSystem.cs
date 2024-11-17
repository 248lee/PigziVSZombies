using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JohnUtils;

public class GameflowSystem : MonoBehaviour
{
    public static GameflowSystem instance { get; private set; }
    public bool is_pausing { get; private set; }
    private void LateUpdate()
    {
        RuntimeGlobalDictionary.CopyBufferToReal();
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetPause()
    {
        this.is_pausing = true;
        Time.timeScale = 0;
    }
    public void SetUnpaused()
    {
        this.is_pausing = false;
        Time.timeScale = 1;
    }
}
