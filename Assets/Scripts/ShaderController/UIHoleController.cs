using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIHoleController : MonoBehaviour
{
    //[Tooltip("In normalized coordinates (0~1 relative to the image rect)")]
    public Vector2 holeCenter = new Vector2(0.5f, 0.5f);
    public float holeRadius = 0.1f;

    private Material holeMaterial;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        holeMaterial = Instantiate(image.material); // avoid modifying shared material
        image.material = holeMaterial;

        UpdateHole();
    }

    void UpdateHole()
    {
        holeMaterial.SetVector("_HoleCenter", holeCenter);
        holeMaterial.SetFloat("_HoleRadius", holeRadius);
    }

    // Optional: Call this if you want to change hole in runtime
    public void SetHole(Vector2 center, float radius)
    {
        holeCenter = center;
        holeRadius = radius;
        UpdateHole();
    }
}
