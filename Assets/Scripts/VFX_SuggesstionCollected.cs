using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_SuggesstionCollected : MonoBehaviour
{
    public Transform targetPoint; // Assign the target in the Inspector
    public float waitTime = 1f; // Time before moving
    public float moveSpeed = 5f;
    private ParticleSystem particles;
    private bool moveToTarget = false;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        Invoke(nameof(StartMoving), waitTime);
        Invoke(nameof(DestroyVFX), waitTime * 2);
    }

    void StartMoving()
    {
        moveToTarget = true;
    }
    void DestroyVFX()
    {
        this.enabled = false;
        Destroy(gameObject);
    }

    void Update()
    {
        if (moveToTarget)
        {
            ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[particles.particleCount];
            int count = particles.GetParticles(particleArray);

            for (int i = 0; i < count; i++)
            {
                Vector3 direction = (targetPoint.position - particleArray[i].position).normalized;
                particleArray[i].velocity = direction * moveSpeed;
            }

            particles.SetParticles(particleArray, count);
        }
    }
}
