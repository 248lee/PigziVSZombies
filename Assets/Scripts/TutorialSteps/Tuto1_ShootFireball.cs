using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuto1_ShootFireball : MonoBehaviour, ITutorialStep
{
    [SerializeField] List<string> vocabularyList; // 欲顯示的單字
    private bool isEnd = false;
    [SerializeField]
    private ChoiceWindow introWindow; // 教學開始時的提示視窗
    [SerializeField]
    private ChoiceWindow intro2Window; //第二個的提示視窗
    [SerializeField]
    private GameObject blackMask1; // 指導射出火球用的黑色遮罩
    [SerializeField]
    private ChoiceWindow endWindow; // 教學結束時的提示視窗
    private void Start()
    {
        this.isEnd = false;
    }
    public void StartTutorial()
    {
        this.isEnd = false;
        GetComponentInChildren<Canvas>().gameObject.SetActive(true); // 確保 Canvas 啟用
        // 在這裡開始教學步驟
        Debug.Log("Tutorial Step 1: Shoot a fireball!");
        StartCoroutine(tutorialProcess());
    }
    public bool EndCondition()
    {
        // 檢查是否完成教學步驟
        return this.isEnd;
    }
    IEnumerator tutorialProcess()
    {
        yield return null;
        GameflowSystem.instance.SetUnpaused(); // 確保遊戲流程系統處於未暫停狀態
        this.introWindow.gameObject.SetActive(false);
        this.intro2Window.gameObject.SetActive(false);
        this.endWindow.gameObject.SetActive(false);

        yield return VocabularyBoard.instance.UpdateVocabularyBoard(this.vocabularyList);

        yield return new WaitForSeconds(2f);
        // 顯示教學開始的提示視窗
        this.introWindow.gameObject.SetActive(true);
        while (this.introWindow.gameObject.activeSelf)
        {
            yield return null; // 等待直到提示視窗被關閉
        }

        yield return new WaitForSeconds(1f);
        // 顯示第二個的提示視窗
        this.intro2Window.gameObject.SetActive(true);
        while (this.intro2Window.gameObject.activeSelf)
        {
            yield return null; // 等待直到提示視窗被關閉
        }

        // 開始教學步驟
        FireballSysrem.instance.generateFireball(new Question("apple", "John just ate a big red sweet <apple>."));
        bool is_correct = false;
        void inputCompleteHandler(string input)
        {
            Debug.Log("Input received: " + input);
            // 檢查玩家輸入的input是不是等於"apple"
            if (input == "apple")
            {
                is_correct = true;
                // 清除黑色遮罩
                this.blackMask1.SetActive(false);
                // 解除暫停遊戲
                GameflowSystem.instance.SetUnpaused();
                // 移除輸入完成的事件處理器
                AutoCompleteInput.instance.inputCompleteHandler -= inputCompleteHandler;
            }
            else
            {
                // 如果不是，則顯示錯誤提示
                Debug.Log("Incorrect input, please try again.");
            }

        }
        AutoCompleteInput.instance.inputCompleteHandler += inputCompleteHandler;

        yield return new WaitForSeconds(14f);
        if (!is_correct)
        {
            // Pause the game
            GameflowSystem.instance.SetPauseButAllowInput();
            // 顯示黑色遮罩
            this.blackMask1.SetActive(true);
        }

        // yield return new WaitUntil(() => !this.blackMask1.activeSelf); // 等待直到黑色遮罩被關閉

        yield return new WaitForSeconds(1f);
        // 顯示教學結束的提示視窗
        this.endWindow.gameObject.SetActive(true);
        while (this.endWindow.gameObject.activeSelf)
        {
            yield return null; // 等待直到提示視窗被關閉
        }
        // 教學結束
        GetComponentInChildren<Canvas>().gameObject.SetActive(false); // 關掉 Canvas，避免與下個 canvas干擾造成 button無法點擊
        this.isEnd = true;
    }
}
