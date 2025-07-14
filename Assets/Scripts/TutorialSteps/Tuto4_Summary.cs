using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuto4_Summary : MonoBehaviour, ITutorialStep
{
    private bool isEnd = false;
    [SerializeField]
    private ChoiceWindow introWindow; // �оǶ}�l�ɪ����ܵ���
    [SerializeField]
    private ChoiceWindow intro2Window; //�ĤG�Ӫ����ܵ���
    [SerializeField]
    private ChoiceWindow intro3Window; //�ĤT�Ӫ����ܵ���
    [SerializeField]
    private ChoiceWindow endWindow1; // �Ĥ@�ӱоǵ����ɪ����ܵ���
    [SerializeField]
    private ChoiceWindow endWindow2; // �ĤG�ӱоǵ����ɪ����ܵ���
    private void Start()
    {
        this.isEnd = false;
    }
    public void StartTutorial()
    {
        this.isEnd = false;
        GetComponentInChildren<Canvas>().gameObject.SetActive(true); // �T�O Canvas �ҥ�
        // �b�o�̶}�l�оǨB�J
        Debug.Log("Tutorial Step 4: Summary!");
        StartCoroutine(tutorialProcess());
    }
    public bool EndCondition()
    {
        // �ˬd�O�_�����оǨB�J
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
        // ��ܱоǶ}�l�����ܵ���
        this.introWindow.gameObject.SetActive(true);
        while (this.introWindow.gameObject.activeSelf)
        {
            yield return null; // ���ݪ��촣�ܵ����Q����
        }

        // ��ܲĤG�Ӫ����ܵ���
        yield return new WaitForSeconds(1f);
        this.intro2Window.gameObject.SetActive(true);
        while (this.intro2Window.gameObject.activeSelf)
        {
            yield return null; // ���ݪ��촣�ܵ����Q����
        }
        // ��ܲĤT�Ӫ����ܵ���
        yield return new WaitForSeconds(1f);
        this.intro3Window.gameObject.SetActive(true);
        while (this.intro3Window.gameObject.activeSelf)
        {
            yield return null; // ���ݪ��촣�ܵ����Q����
        }

        
        yield return new WaitForSeconds(3f);
        // ��ܲĤ@�ӱоǵ��������ܵ���
        this.endWindow1.gameObject.SetActive(true);
        while (this.endWindow1.gameObject.activeSelf)
        {
            yield return null; // ���ݪ��촣�ܵ����Q����
        }
        // ��ܲĤG�ӱоǵ��������ܵ���
        yield return new WaitForSeconds(1f);
        this.endWindow2.gameObject.SetActive(true);
        while (this.endWindow2.gameObject.activeSelf)
        {
            yield return null; // ���ݪ��촣�ܵ����Q����
        }
        // �оǵ���
        GetComponentInChildren<Canvas>().gameObject.SetActive(false); // ���� Canvas�A�קK�P�U�� canvas�z�Z�y�� button�L�k�I��
        this.isEnd = true;
    }
}
