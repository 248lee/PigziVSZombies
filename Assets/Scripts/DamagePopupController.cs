using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopupController : MonoBehaviour
{
    public void InitializeDamagePopup(int damageValue, Transform parent)
    {
        Instantiate(gameObject, parent);  // This gameObject will be destroyed after the popup animation is end by the state machine
        string damageText;
        if (damageValue > 0)
            damageText = "+ " + damageValue.ToString();
        else
            damageText = damageValue.ToString();
        GetComponent<TextMeshProUGUI>().text = damageText;  // The damagePopup gameObject will be destroyed in its state machine
    }
}
