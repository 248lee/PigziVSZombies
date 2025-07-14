using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuto2_SaveWildFireTree : MonoBehaviour, ITutorialStep
{
    private bool isEnd = false;
    [SerializeField] GameObject superFastFireballWarning; // �W�֤��yĵ�i���ܰʵe
    [SerializeField] private GameObject blackMask2; // ���ɮg�X���y�Ϊ��¦�B�n
    [SerializeField] private ChoiceWindow endWindow; // �оǵ����ɪ����ܵ���

    // Start is called before the first frame update
    void Start()
    {
        this.isEnd = false;
    }
    public void StartTutorial()
    {
        this.isEnd = false;
        GetComponentInChildren<Canvas>().gameObject.SetActive(true); // �T�O Canvas �ҥ�
        // �b�o�̶}�l�оǨB�J
        Debug.Log("Tutorial Step 2: Save The Wildfired Tree!!");
        StartCoroutine(tutorialProcess());
    }
    public bool EndCondition()
    {
        // �ˬd�O�_�����оǨB�J
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

        // �l��W�֤��y!!
        FireFireballController superFastFireball = FireballSysrem.instance.generateFireball(new Question("wildfire", "The <wildfire> is spreading quickly! Save the tree!"));
        if (superFastFireball != null)
        {
            superFastFireball.speed = -12f; // �]�w�W�֤��y���t��
        }

        bool is_correct = false;
        void inputCompleteHandler(string input)
        {
            // �ˬd���a��J��input�O���O����"apple"
            if (input == "wildfire")
            {
                is_correct = true;
                // �M���¦�B�n
                this.blackMask2.SetActive(false);
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

        yield return new WaitForSeconds(5f);
        if (!is_correct)
        {
            // Pause the game
            GameflowSystem.instance.SetPauseButAllowInput();
            // ��ܶ¦�B�n
            this.blackMask2.SetActive(true);
        }
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
