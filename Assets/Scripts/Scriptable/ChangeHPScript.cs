using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ChangeHPScript : ScriptableObject
{
    [Header("Target Alive")]
    [SerializeField] float ratio_of_maxHP;
    [SerializeField] float bias;
    [Header("Target Dead")]
    [SerializeField] float revive_ratio_of_maxHP;
    [SerializeField] float revive_bias;
    [Header("Popup Setting")]
    [SerializeField] bool isShowingPopup;
    public DamagePopupController popup;
    public float ComputeHPChange(float maxHP)
    {
        float deltaHP = ratio_of_maxHP * maxHP + bias;
        return deltaHP;
    }
    public void ShowPopup(float deltaHP, Transform showTransform)
    {
        if (isShowingPopup)
        {
            this.popup.CreateDamagePopup((int)deltaHP, showTransform);
        }
    }
    public float ComputeHPChangeRevive(float maxHP)
    {
        return revive_ratio_of_maxHP * maxHP + revive_bias;
    }
    public void ShowPopupRevive(Transform showTransform)
    {
        if (isShowingPopup)
        {
            this.popup.CreateStringPopup("revive!", showTransform);
        }
    }
}
