using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JohnUtils;
using UnityEngine.SceneManagement;

public class GameflowSystem : MonoBehaviour
{
    public static GameflowSystem instance { get; private set; }
    public GameObject textOfMissionComplete;
    public GameObject textOfMissionFailed;
    public bool is_pausing { get; private set; }
    private void LateUpdate()
    {
        RuntimeGlobalDictionary.CopyBufferToReal();
    }
    private void Awake()
    {
        this.textOfMissionComplete.SetActive(false);
        this.textOfMissionFailed.SetActive(false);

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
        AlwaysActiveInputField.instance.SetAllowInput(false);
    }
    public void SetUnpaused()
    {
        this.is_pausing = false;
        Time.timeScale = 1;
        ResultSystem.instance.CloseResultWindow();
        AlwaysActiveInputField.instance.SetAllowInput(true);
    }
    public void StageWin()
    {
        StartCoroutine(this._stageWin());
    }
    public void StageLose()
    {
        StartCoroutine(this._stageLose());
    }
    IEnumerator _stageWin()
    {
        yield return new WaitForSecondsRealtime(1f);  // wait for the screen to be clean
        this.SetPause();
        this.textOfMissionComplete.SetActive(true);
        while (!this.textOfMissionComplete.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("End State (This name is dependent in GameflowSystem_cs)"))
            yield return null;  // Wait for the text animation to finish
        this.textOfMissionComplete.SetActive(false);
        yield return new WaitForSecondsRealtime(1f);

        ResultSystem.instance.OpenResultWindow();

    }
    IEnumerator _stageLose()
    {
        this.SetPause();
        yield return new WaitForSecondsRealtime(1f);  // freeze the screen for a second
        this.textOfMissionFailed.SetActive(true);
        while (!this.textOfMissionFailed.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("End State (This name is dependent in GameflowSystem_cs)"))
            yield return null;  // Wait for the text animation to finish
        this.textOfMissionFailed.SetActive(false);
        yield return new WaitForSecondsRealtime(1f);

        ResultSystem.instance.OpenResultWindow();
    }
    public void LeaveStage()
    {
        SceneManager.LoadScene("LevelSelection");
    }
}
