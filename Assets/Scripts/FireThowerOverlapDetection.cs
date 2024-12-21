using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireThowerOverlapDetection : MonoBehaviour
{
    public List<Collider2D> triggerColliders; // Assign your 2D trigger collider
    public GameObject test;
    private Camera mainCamera;          // Assign the Camera viewing the scene
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];

        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Automatically assign the main camera if not specified
        }
    }

    void Update()
    {
        int count = ps.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            // Convert particle's world position to screen space
            Vector3 screenPos = mainCamera.WorldToScreenPoint(particles[i].position);

            // Ignore particles behind the camera
            if (screenPos.z < 0) continue;

            // Project particles onto the plane on which the trigger is.
            if (this.triggerColliders.Count == 0)
            {
                Debug.LogError("No 2D trigger is assigned");
                continue;
            }
            foreach (Collider2D trigger in this.triggerColliders)  // Check whether all the triggers are on the same plane
            {
                if (trigger.transform.position.z != this.triggerColliders[0].transform.position.z)
                {
                    Debug.LogWarning("The triggers should suppose be on the same 2D plane so that we know where to project to!");
                    continue;
                }
            }
            screenPos.Set(screenPos.x, screenPos.y, mainCamera.WorldToScreenPoint(this.triggerColliders[0].transform.position).z);

            // Convert screen space to world point for 2D trigger check
            Vector2 screenToWorldPoint = mainCamera.ScreenToWorldPoint(screenPos);

            // Visualize the point in the scene using Debug.DrawRay
            //if (i == 15)
            //{
                //Instantiate(test, screenToWorldPoint, Quaternion.identity);
                //Debug.Log(triggerColliders[0].OverlapPoint(screenToWorldPoint));
                //Debug.Log(screenToWorldPoint);
                //Debug.Log(triggerColliders[0].bounds.max);
                //Debug.Log(triggerColliders[0].bounds.min);
                //Debug.Log("==============================================");
            //}

            // Check if the particle overlaps with the 2D trigger
            bool is_overlapping = false;
            for (int t = 0; !is_overlapping && t < triggerColliders.Count; t++)
            {
                is_overlapping = is_overlapping || triggerColliders[t].OverlapPoint(screenToWorldPoint);
            }
            if (is_overlapping)
            {
                Debug.Log("Flame inside tree");
                // Fade out the particle
                Color32 color = particles[i].startColor;
                color.a = (byte)Mathf.Clamp(color.a - 5, 0, 255); // Gradually fade alpha
                particles[i].startColor = color;

                // Optionally reduce particle lifetime for faster removal
                particles[i].remainingLifetime *= 0.625f;
            }
        }

        ps.SetParticles(particles, count);
    }
}
