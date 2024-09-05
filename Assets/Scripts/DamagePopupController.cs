using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum PopupAnimationType
{
    PopFromUpToDown,
    FloatsAbove
}
public class DamagePopupController : MonoBehaviour
{
    [SerializeField] PopupAnimationType popupAnimationType;
    public void CreateDamagePopup(int damageValue, Transform parent)
    {
        DamagePopupController popup_inScreen = Instantiate(this, parent);  // This gameObject will be destroyed after the popup animation is end by the state machine
        string damageText;
        if (damageValue > 0)
            damageText = "+ " + damageValue.ToString();
        else
            damageText = damageValue.ToString();
        popup_inScreen._Initialize(damageText);
    }
    public void CreateStringPopup(string text, Transform parent)
    {
        DamagePopupController popup_inScreen = Instantiate(this, parent);  // This gameObject will be destroyed after the popup animation is end by the state machine
        popup_inScreen._Initialize(text);
    }
    void _Initialize(string damageText)
    {
        // Decide animation
        if (this.popupAnimationType == PopupAnimationType.PopFromUpToDown)
            GetComponentInChildren<Animator>().SetInteger("animationSelect", 1);
        else if (this.popupAnimationType == PopupAnimationType.FloatsAbove)
            GetComponentInChildren<Animator>().SetInteger("animationSelect", 2);

        // Draw text
        GetComponentInChildren<TextMeshProUGUI>().text = damageText;  // The damagePopup gameObject will be destroyed in its state machine
    }
}
