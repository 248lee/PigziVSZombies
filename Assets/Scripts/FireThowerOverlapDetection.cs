using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireThowerOverlapDetection : MonoBehaviour
{
    public List<Collider2D> treeColliders; // Assign your 2D trigger collider
    public List<AnimalController> animalControllers;
    private Camera mainCamera;          // Assign the Camera viewing the scene
    [SerializeField] private ParticleSystem flameParticleSystem;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        particles = new ParticleSystem.Particle[flameParticleSystem.main.maxParticles];

        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Automatically assign the main camera if not specified
        }
    }

    void Update()
    {
        int count = flameParticleSystem.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            // Convert particle's world position to screen space
            Vector3 screenPos = mainCamera.WorldToScreenPoint(particles[i].position);

            // Ignore particles behind the camera
            if (screenPos.z < 0) continue;

            // Project particles onto the plane on which the trigger is.
            if (this.treeColliders.Count == 0)
            {
                Debug.LogError("No 2D trigger is assigned");
                continue;
            }
            screenPos.Set(screenPos.x, screenPos.y, mainCamera.WorldToScreenPoint(this.treeColliders[0].transform.position).z);

            // Convert screen space to world point for 2D trigger check
            Vector2 screenToWorldPoint = mainCamera.ScreenToWorldPoint(screenPos);

            // Visualize the point in the scene using Debug.DrawRay
            //if (i == 15)
            //{
                //Instantiate(test, screenToWorldPoint, Quaternion.identity);
                //Debug.Log(treeColliders[0].OverlapPoint(screenToWorldPoint));
                //Debug.Log(screenToWorldPoint);
                //Debug.Log(treeColliders[0].bounds.max);
                //Debug.Log(treeColliders[0].bounds.min);
                //Debug.Log("==============================================");
            //}

            // Check if the particle overlaps with the trees' 2D triggers
            for (int j = 0; j < treeColliders.Count; j++)
            {
                bool is_overlapping = treeColliders[j].OverlapPoint(screenToWorldPoint);
                if (is_overlapping)
                {
                    // Fade out the particle
                    Color32 color = particles[i].startColor;
                    color.a = (byte)Mathf.Clamp(color.a - 5, 0, 255); // Gradually fade alpha
                    particles[i].startColor = color;

                    // Optionally reduce particle lifetime for faster removal
                    particles[i].remainingLifetime *= 0.625f;

                    break;
                }
            }

            // Check if the particle overlaps with the animals' triggers
            for (int j = 0; j < animalControllers.Count; j++)
            {
                bool is_overlapping = animalControllers[j].GetComponent<Collider2D>().OverlapPoint(screenToWorldPoint);
                if (is_overlapping)
                {
                    animalControllers[j].StartBurned();
                }
            }
        }

        flameParticleSystem.SetParticles(particles, count);
    }
}
