using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuto2_SaveWildFireTree : MonoBehaviour, ITutorialStep
{
    private bool isEnd = false;
    [SerializeField] GameObject superFastFireballWarning; // 超快火球警告提示動畫
    [SerializeField] private GameObject blackMask2; // 指導射出火球用的黑色遮罩
    [SerializeField] private ChoiceWindow endWindow; // 教學結束時的提示視窗

    // Start is called before the first frame update
    void Start()
    {
        this.isEnd = false;
    }
    public void StartTutorial()
    {
        this.isEnd = false;
        GetComponentInChildren<Canvas>().gameObject.SetActive(true); // 確保 Canvas 啟用
        // 在這裡開始教學步驟
        Debug.Log("Tutorial Step 2: Save The Wildfired Tree!!");
        StartCoroutine(tutorialProcess());
    }
    public bool EndCondition()
    {
        // 檢查是否完成教學步驟
        return this.isEnd;
    }
    IEnumerator tutorialProcess()
    {
        this.blackMask2.SetActive(false);
        this.endWindow.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        this.superFastFireballWarning.SetActive(true);
        yield return new WaitForSeconds(3f);
        this.superFastFireballWarning.SetActive(false);
        yield return new WaitForSeconds(.7f);

        // 召喚超快火球!!
        FireFireballController superFastFireball = FireballSysrem.instance.generateFireball(new Question("wildfire", "The <wildfire> is spreading quickly! Save the tree!"));
        if (superFastFireball != null)
        {
            superFastFireball.speed = -12f; // 設定超快火球的速度
        }

        bool is_correct = false;
        void inputCompleteHandler(string input)
        {
            // 檢查玩家輸入的input是不是等於"apple"
            if (input == "wildfire")
            {
                is_correct = true;
                // 清除黑色遮罩
                this.blackMask2.SetActive(false);
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

        yield return new WaitForSeconds(5f);
        if (!is_correct)
        {
            // Pause the game
            GameflowSystem.instance.SetPauseButAllowInput();
            // 顯示黑色遮罩
            this.blackMask2.SetActive(true);
        }
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
