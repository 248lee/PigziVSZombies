using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuto1_ShootFireball : MonoBehaviour, ITutorialStep
{
    [SerializeField] List<string> vocabularyList; // ����ܪ���r
    private bool isEnd = false;
    [SerializeField]
    private ChoiceWindow introWindow; // �оǶ}�l�ɪ����ܵ���
    [SerializeField]
    private ChoiceWindow intro2Window; //�ĤG�Ӫ����ܵ���
    [SerializeField]
    private GameObject blackMask1; // ���ɮg�X���y�Ϊ��¦�B�n
    [SerializeField]
    private ChoiceWindow endWindow; // �оǵ����ɪ����ܵ���
    private void Start()
    {
        this.isEnd = false;
    }
    public void StartTutorial()
    {
        this.isEnd = false;
        GetComponentInChildren<Canvas>().gameObject.SetActive(true); // �T�O Canvas �ҥ�
        // �b�o�̶}�l�оǨB�J
        Debug.Log("Tutorial Step 1: Shoot a fireball!");
        StartCoroutine(tutorialProcess());
    }
    public bool EndCondition()
    {
        // �ˬd�O�_�����оǨB�J
        return this.isEnd;
    }
    IEnumerator tutorialProcess()
    {
        yield return null;
        GameflowSystem.instance.SetUnpaused(); // �T�O�C���y�{�t�γB�󥼼Ȱ����A
        this.introWindow.gameObject.SetActive(false);
        this.intro2Window.gameObject.SetActive(false);
        this.endWindow.gameObject.SetActive(false);

        yield return VocabularyBoard.instance.UpdateVocabularyBoard(this.vocabularyList);

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
        FireballSysrem.instance.generateFireball(new Question("apple", "John just ate a big red sweet <apple>."));
        bool is_correct = false;
        void inputCompleteHandler(string input)
        {
            Debug.Log("Input received: " + input);
            // �ˬd���a��J��input�O���O����"apple"
            if (input == "apple")
            {
                is_correct = true;
                // �M���¦�B�n
                this.blackMask1.SetActive(false);
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

        yield return new WaitForSeconds(14f);
        if (!is_correct)
        {
            // Pause the game
            GameflowSystem.instance.SetPauseButAllowInput();
            // ��ܶ¦�B�n
            this.blackMask1.SetActive(true);
        }

        // yield return new WaitUntil(() => !this.blackMask1.activeSelf); // ���ݪ���¦�B�n�Q����

        yield return new WaitForSeconds(1f);
        // ��ܱоǵ��������ܵ���
        this.endWindow.gameObject.SetActive(true);
        while (this.endWindow.gameObject.activeSelf)
        {
            yield return null; // ���ݪ��촣�ܵ����Q����
        }
        // �оǵ���
        GetComponentInChildren<Canvas>().gameObject.SetActive(false); // ���� Canvas�A�קK�P�U�� canvas�z�Z�y�� button�L�k�I��
        this.isEnd = true;
    }
}
