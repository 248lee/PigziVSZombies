using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuto4_Summary : MonoBehaviour, ITutorialStep
{
    private bool isEnd = false;
    [SerializeField]
    private ChoiceWindow introWindow; // 教學開始時的提示視窗
    [SerializeField]
    private ChoiceWindow intro2Window; //第二個的提示視窗
    [SerializeField]
    private ChoiceWindow intro3Window; //第三個的提示視窗
    [SerializeField]
    private ChoiceWindow endWindow1; // 第一個教學結束時的提示視窗
    [SerializeField]
    private ChoiceWindow endWindow2; // 第二個教學結束時的提示視窗
    private void Start()
    {
        this.isEnd = false;
    }
    public void StartTutorial()
    {
        this.isEnd = false;
        GetComponentInChildren<Canvas>().gameObject.SetActive(true); // 確保 Canvas 啟用
        // 在這裡開始教學步驟
        Debug.Log("Tutorial Step 4: Summary!");
        StartCoroutine(tutorialProcess());
    }
    public bool EndCondition()
    {
        // 檢查是否完成教學步驟
        return this.isEnd;
    }
    IEnumerator tutorialProcess()
    {
        this.introWindow.gameObject.SetActive(false);
        this.intro2Window.gameObject.SetActive(false);
        this.intro3Window.gameObject.SetActive(false);
        this.endWindow1.gameObject.SetActive(false);
        this.endWindow2.gameObject.SetActive(false);


        yield return new WaitForSeconds(2f);
        // 顯示教學開始的提示視窗
        this.introWindow.gameObject.SetActive(true);
        while (this.introWindow.gameObject.activeSelf)
        {
            yield return null; // 等待直到提示視窗被關閉
        }

        // 顯示第二個的提示視窗
        yield return new WaitForSeconds(1f);
        this.intro2Window.gameObject.SetActive(true);
        while (this.intro2Window.gameObject.activeSelf)
        {
            yield return null; // 等待直到提示視窗被關閉
        }
        // 顯示第三個的提示視窗
        yield return new WaitForSeconds(1f);
        this.intro3Window.gameObject.SetActive(true);
        while (this.intro3Window.gameObject.activeSelf)
        {
            yield return null; // 等待直到提示視窗被關閉
        }

        
        yield return new WaitForSeconds(3f);
        // 顯示第一個教學結束的提示視窗
        this.endWindow1.gameObject.SetActive(true);
        while (this.endWindow1.gameObject.activeSelf)
        {
            yield return null; // 等待直到提示視窗被關閉
        }
        // 顯示第二個教學結束的提示視窗
        yield return new WaitForSeconds(1f);
        this.endWindow2.gameObject.SetActive(true);
        while (this.endWindow2.gameObject.activeSelf)
        {
            yield return null; // 等待直到提示視窗被關閉
        }
        // 教學結束
        GetComponentInChildren<Canvas>().gameObject.SetActive(false); // 關掉 Canvas，避免與下個 canvas干擾造成 button無法點擊
        this.isEnd = true;
    }
}
