using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlashController : MonoBehaviour
{
    public static FlashController instance;
    private Coroutine currentFlashCorotine;
    private Image flashImage;
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }
    private void Start()
    {
        this.flashImage = GetComponent<Image>();
    }
    public void StartHardFlash(Color flashColor, float duration)
    {
        if (this.currentFlashCorotine != null)
            StopCoroutine(this.currentFlashCorotine);
        this.currentFlashCorotine = StartCoroutine(_hardFlash(flashColor, duration));
    }
    private IEnumerator _hardFlash(Color flashColor, float duration)
    {
        this.flashImage.color = flashColor;
        yield return new WaitForSeconds(duration);
        this.flashImage.color = Color.clear;
    }
}
