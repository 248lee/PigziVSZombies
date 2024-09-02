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
        popup_inScreen._Initialize(damageValue);
    }
    void _Initialize(int damageValue)
    {
        // Decide animation
        if (this.popupAnimationType == PopupAnimationType.PopFromUpToDown)
            GetComponent<Animator>().SetInteger("animationSelect", 1);
        else if (this.popupAnimationType == PopupAnimationType.FloatsAbove)
            GetComponent<Animator>().SetInteger("animationSelect", 2);

        // Draw text
        string damageText;
        if (damageValue > 0)
            damageText = "+ " + damageValue.ToString();
        else
            damageText = damageValue.ToString();
        GetComponent<TextMeshProUGUI>().text = damageText;  // The damagePopup gameObject will be destroyed in its state machine
    }
}
