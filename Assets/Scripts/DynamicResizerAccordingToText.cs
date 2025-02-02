using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DynamicResizerAccordingToText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro; // Assign the TextMeshPro component
    public float padding = 10f; // Extra padding for spacing
    RectTransform imageRectTransform; // Assign the Image's RectTransform
    private void Start()
    {
        this.imageRectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        AdjustImageHeight();
    }

    void AdjustImageHeight()
    {
        if (textMeshPro == null || imageRectTransform == null) return;

        // Get the preferred height of the text
        float textHeight = textMeshPro.preferredHeight;

        // Adjust the Image height
        Vector2 newSize = imageRectTransform.sizeDelta;
        newSize.y = textHeight + padding;
        imageRectTransform.sizeDelta = newSize;
    }
}
