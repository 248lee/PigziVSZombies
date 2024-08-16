using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class HPBarController : MonoBehaviour
{
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;
    Slider slider;
    private TextMeshProUGUI HPText = null;
    private float maxHP;

    // Start is called before the first frame update
    void Start()
    {
        this.slider = GetComponent<Slider>();
        this.fill = GetComponentInChildren<Image>();
        TextMeshProUGUI[] whetherHPTexts = GetComponentsInChildren<TextMeshProUGUI>();
        if (whetherHPTexts.Length > 0)
            this.HPText = whetherHPTexts[0];
    }

    // Update is called once per frame
    void Update()
    {
        this.fill.color = this.gradient.Evaluate(this.slider.normalizedValue);
    }
    public void SetMaxHP(float maxHP)
    {
        this.slider.maxValue = maxHP;
        this.maxHP = maxHP;
    }
    public void SetHP(float HP)
    {
        this.slider.value = HP;
        if (this.HPText != null)
        {
            this.HPText.SetText(((int)HP).ToString() + "/" + ((int)maxHP).ToString());
        }
    }
    public Transform GetFilling()
    {
        return this.fill.transform;
    }
}
