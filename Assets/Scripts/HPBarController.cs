using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    [SerializeField] Gradient gradient;
    Image fill;
    Slider slider;
    
    // Start is called before the first frame update
    void Start()
    {
        this.slider = GetComponent<Slider>();
        this.fill = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        this.fill.color = this.gradient.Evaluate(this.slider.normalizedValue);
    }
    public void SetMaxHP(float maxHP)
    {
        this.slider.maxValue = maxHP;
    }
    public void SetHP(float HP)
    {
        this.slider.value = HP;
    }
}