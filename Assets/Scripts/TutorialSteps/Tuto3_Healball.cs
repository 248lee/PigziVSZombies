using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuto3_Healball : MonoBehaviour, ITutorialStep
{
    private bool isEnd = false;
    [SerializeField] List<TreeController> treeControllers;
    [SerializeField] GameObject healballCountdownUI; // 治療球倒數計時UI
    [SerializeField]
    private ChoiceWindow introWindow; // 教學開始時的提示視窗
    [SerializeField]
    private ChoiceWindow intro2Window; //第二個的提示視窗
    [SerializeField]
    private GameObject blackMask3; // 指導射出火球用的黑色遮罩
    [SerializeField]
    private GameObject arrows; // 指導出治療球的位置
    [SerializeField]
    private ChoiceWindow successWindow; // 治療成功時的提示視窗
    [SerializeField]
    private ChoiceWindow missedWindow; // 治療失敗時的提示視窗
    private void Start()
    {
        this.isEnd = false;
    }
    public void StartTutorial()
    {
        this.isEnd = false;
        GetComponentInChildren<Canvas>().gameObject.SetActive(true); // 確保 Canvas 啟用
        // 在這裡開始教學步驟
        Debug.Log("Tutorial Step 3: Heal the tree!");
        StartCoroutine(tutorialProcess());
    }
    public bool EndCondition()
    {
        // 檢查是否完成教學步驟
        return this.isEnd;
    }
    IEnumerator tutorialProcess()
    {
        this.healballCountdownUI.SetActive(false);
        this.introWindow.gameObject.SetActive(false);
        this.intro2Window.gameObject.SetActive(false);
        this.blackMask3.SetActive(false);
        this.arrows.SetActive(false);
        this.successWindow.gameObject.SetActive(false);
        this.missedWindow.gameObject.SetActive(false);

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
        while (true)
        {
            this.healballCountdownUI.SetActive(true);
            float countdown = 3f;
            while (countdown > 0)
            {
                this.healballCountdownUI.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(((int)countdown + 1).ToString());  // Draw the countdown UI (+1 for graphic delay)
                countdown -= Time.deltaTime;
                yield return null;
            }
            this.healballCountdownUI.SetActive(false);
            Question[] questions = new Question[]
            {
                new Question("apple", "An <apple> a day keeps the doctor away."),
                new Question("tree", "The green <tree> is a symbol of life and growth."),
                new Question("inside", "She opened the box and looked <inside> it."),
                new Question("wildfire", "The <wildfires> have destroyed the forest.")
            };
            FireballSysrem.instance.generateFourHealballs(questions);

            bool is_correct = false;
            void inputCompleteHandler(string input)
            {
                is_correct = true;
                Debug.Log("Input received: " + input);
                // 檢查玩家輸入的input是不是等於"apple"
                if (input == "apple" || input == "tree" || input == "inside" || input == "wildfire")
                {
                    // 清除黑色遮罩
                    this.blackMask3.SetActive(false);
                    this.arrows.SetActive(false);
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

            yield return new WaitForSeconds(7f);
            if (!is_correct)
            {
                // Pause the game
                GameflowSystem.instance.SetPauseButAllowInput();
                // 顯示黑色遮罩與指導箭頭
                this.blackMask3.SetActive(true);
                this.arrows.SetActive(true);
            }

            // yield return new WaitUntil(() => !this.blackMask1.activeSelf); // 等待直到黑色遮罩被關閉

            yield return new WaitForSeconds(5f);
            // 顯示教學結束的提示視窗
            if (treeControllers[0].hp == treeControllers[0].max_hp && treeControllers[1].hp == treeControllers[1].max_hp && treeControllers[2].hp == treeControllers[2].max_hp && treeControllers[3].hp == treeControllers[3].max_hp)
            {
                this.successWindow.gameObject.SetActive(true);
                while (this.successWindow.gameObject.activeSelf)
                {
                    yield return null; // 等待直到提示視窗被關閉
                }
                break; // 結束教學流程
            }
            else
            {
                this.missedWindow.gameObject.SetActive(true);
                while (this.missedWindow.gameObject.activeSelf)
                {
                    yield return null; // 等待直到提示視窗被關閉
                }
            }
        }
        // 教學結束
        GetComponentInChildren<Canvas>().gameObject.SetActive(false); // 關掉 Canvas，避免與下個 canvas干擾造成 button無法點擊
        this.isEnd = true;
    }
}
