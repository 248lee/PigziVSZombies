// TutorialManager.cs
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // 在 Inspector 中將你的數個教學物件拖到這裡
    public GameObject[] tutorialObjects; 
    
    private ITutorialStep currentStep;
    private int currentStepIndex = 0;

    void Start()
    {
        foreach (GameObject obj in tutorialObjects)
        {
            // 確保每個物件都有 ITutorialStep 介面的實作
            if (obj.GetComponent<ITutorialStep>() == null)
            {
                Debug.LogError($"物件 {obj.name} 上找不到 ITutorialStep 腳本！");
            }
            obj.SetActive(false); // 初始時隱藏所有教學物件
        }
        // 啟動第一個教學
        StartNextTutorial();
    }

    void Update()
    {
        // 如果當前有教學步驟，就持續檢查它的結束條件
        if (currentStep != null)
        {
            if (currentStep.EndCondition())
            {
                // 當前教學結束，準備開始下一個
                StartNextTutorial();
            }
        }
    }

    private void StartNextTutorial()
    {
        if (currentStepIndex < tutorialObjects.Length)
        {
            tutorialObjects[currentStepIndex].SetActive(true); // 啟用當前教學物件
            // 從 GameObject 上取得實作了 ITutorialStep 介面的 Component
            currentStep = tutorialObjects[currentStepIndex].GetComponent<ITutorialStep>();

            if (currentStep != null)
            {
                currentStep.StartTutorial();
                currentStepIndex++;
            }
            else
            {
                Debug.LogError($"物件 {tutorialObjects[currentStepIndex].name} 上找不到 ITutorialStep 腳本！");
            }
        }
        else
        {
            Debug.Log("所有教學已完成！");
            currentStep = null; // 清空
            this.enabled = false; // 關閉管理器
            GameflowSystem.instance.StageWin(); // 呼叫遊戲流程系統的勝利方法
        }
    }
}