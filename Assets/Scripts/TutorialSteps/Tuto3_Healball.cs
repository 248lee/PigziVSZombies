using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuto3_Healball : MonoBehaviour, ITutorialStep
{
    private bool isEnd = false;
    [SerializeField] List<TreeController> treeControllers;
    [SerializeField] GameObject healballCountdownUI; // �v���y�˼ƭp��UI
    [SerializeField]
    private ChoiceWindow introWindow; // �оǶ}�l�ɪ����ܵ���
    [SerializeField]
    private ChoiceWindow intro2Window; //�ĤG�Ӫ����ܵ���
    [SerializeField]
    private GameObject blackMask3; // ���ɮg�X���y�Ϊ��¦�B�n
    [SerializeField]
    private GameObject arrows; // ���ɥX�v���y����m
    [SerializeField]
    private ChoiceWindow successWindow; // �v�����\�ɪ����ܵ���
    [SerializeField]
    private ChoiceWindow missedWindow; // �v�����Ѯɪ����ܵ���
    private void Start()
    {
        this.isEnd = false;
    }
    public void StartTutorial()
    {
        this.isEnd = false;
        GetComponentInChildren<Canvas>().gameObject.SetActive(true); // �T�O Canvas �ҥ�
        // �b�o�̶}�l�оǨB�J
        Debug.Log("Tutorial Step 3: Heal the tree!");
        StartCoroutine(tutorialProcess());
    }
    public bool EndCondition()
    {
        // �ˬd�O�_�����оǨB�J
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
        // ��ܱоǶ}�l�����ܵ���
        this.introWindow.gameObject.SetActive(true);
        while (this.introWindow.gameObject.activeSelf)
        {
            yield return null; // ���ݪ��촣�ܵ����Q����
        }

        yield return new WaitForSeconds(1f);
        // ��ܲĤG�Ӫ����ܵ���
        this.intro2Window.gameObject.SetActive(true);
        while (this.intro2Window.gameObject.activeSelf)
        {
            yield return null; // ���ݪ��촣�ܵ����Q����
        }

        // �}�l�оǨB�J
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
                // �ˬd���a��J��input�O���O����"apple"
                if (input == "apple" || input == "tree" || input == "inside" || input == "wildfire")
                {
                    // �M���¦�B�n
                    this.blackMask3.SetActive(false);
                    this.arrows.SetActive(false);
                    // �Ѱ��Ȱ��C��
                    GameflowSystem.instance.SetUnpaused();
                    // ������J�������ƥ�B�z��
                    AutoCompleteInput.instance.inputCompleteHandler -= inputCompleteHandler;
                }
                else
                {
                    // �p�G���O�A�h��ܿ��~����
                    Debug.Log("Incorrect input, please try again.");
                }

            }
            AutoCompleteInput.instance.inputCompleteHandler += inputCompleteHandler;

            yield return new WaitForSeconds(7f);
            if (!is_correct)
            {
                // Pause the game
                GameflowSystem.instance.SetPauseButAllowInput();
                // ��ܶ¦�B�n�P���ɽb�Y
                this.blackMask3.SetActive(true);
                this.arrows.SetActive(true);
            }

            // yield return new WaitUntil(() => !this.blackMask1.activeSelf); // ���ݪ���¦�B�n�Q����

            yield return new WaitForSeconds(5f);
            // ��ܱоǵ��������ܵ���
            if (treeControllers[0].hp == treeControllers[0].max_hp && treeControllers[1].hp == treeControllers[1].max_hp && treeControllers[2].hp == treeControllers[2].max_hp && treeControllers[3].hp == treeControllers[3].max_hp)
            {
                this.successWindow.gameObject.SetActive(true);
                while (this.successWindow.gameObject.activeSelf)
                {
                    yield return null; // ���ݪ��촣�ܵ����Q����
                }
                break; // �����оǬy�{
            }
            else
            {
                this.missedWindow.gameObject.SetActive(true);
                while (this.missedWindow.gameObject.activeSelf)
                {
                    yield return null; // ���ݪ��촣�ܵ����Q����
                }
            }
        }
        // �оǵ���
        GetComponentInChildren<Canvas>().gameObject.SetActive(false); // ���� Canvas�A�קK�P�U�� canvas�z�Z�y�� button�L�k�I��
        this.isEnd = true;
    }
}
