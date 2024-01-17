using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    [SerializeField] Image image;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setProgressBar(float progress)
    {
        if (progress < 0f || progress > 1f)
        {
            Debug.LogWarning("progress¶·¬°¤ñ¨Ò©Ô!");
        }
        this.image.fillAmount = progress;
    }
}
