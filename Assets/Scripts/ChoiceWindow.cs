using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ChoiceWindow : MonoBehaviour
{
    public List<Button> buttons;
    [SerializeField] private Button closeButton;
    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => this.gameObject.SetActive(false));
        }
    }
    public void SetLastButtonToCloseWindow()
    {
        this.buttons[buttons.Count - 1].onClick.AddListener(() => this.gameObject.SetActive(false));
    }
    public void AddButtonListener(int buttonIndex, UnityAction call, bool is_overwrite)
    {
        if (is_overwrite)
            this.buttons[buttonIndex].onClick.RemoveAllListeners();
        this.buttons[buttonIndex].onClick.AddListener(call);
    }
}
