using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class YesOrNoWindow : MonoBehaviour
{
    public Button yesButton;
    public Button noButton;
    // Start is called before the first frame update
    void Start()
    {
        this.noButton.onClick.AddListener(() => this.gameObject.SetActive(false));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddYesButtonListener(UnityAction call)
    {
        this.yesButton.onClick.AddListener(call);
    }
}
