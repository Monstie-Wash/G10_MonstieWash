using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalCleaner : MonoBehaviour
{
    [SerializeField] private float activeTime = 0.5f;

    private BoxCollider2D m_collider;
    private Eraser m_eraser;
    private float m_currentStrength = 0f;

    private Coroutine m_activeCoroutine;

    private void Awake()
    {
        m_collider = GetComponent<BoxCollider2D>();
        m_eraser = GetComponentInParent<Eraser>();
        m_eraser.ErasingEnabled = false;
    }

    private void OnEnable()
    {
        InputManager.Instance.OnActivate_Started += ActivateStarted;
        InputManager.Instance.OnActivate_Ended += ActivateEnded;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnActivate_Started -= ActivateStarted;
        InputManager.Instance.OnActivate_Ended -= ActivateEnded;
    }

    private void ActivateStarted()
    {
        m_collider.enabled = true;
    }

    private void ActivateEnded()
    {
        m_collider.enabled = false;
    }

    /// <summary>
    /// Called when the tool collides with a particle (bubble)
    /// </summary>
    /// <param name="other">The particle being collided with</param>
    private void OnParticleCollision(GameObject other)
    {
        var particleSystem = other.GetComponent<ParticleSystem>();
        var particles = new ParticleSystem.Particle[particleSystem.particleCount];
        float maxLifetimePercentage = 0f;
        List<Vector4> particleData = new();

        // Populates the particleData list with one entry per particle.
        // The only one we care about is the x component which represents the particle's lifetime as a percentage of the completion time (5s).
        particleSystem.GetCustomParticleData(particleData, ParticleSystemCustomData.Custom1); 
        // Populates the particles array.
        particleSystem.GetParticles(particles);

        for (int i = 0; i < particles.Length; i++)
        {
            // When we collide with a particle, we set its remainingLifetime to 0.
            // So this is just how we find out which particles we collided with.
            if (particles[i].remainingLifetime == 0f) 
            {
                maxLifetimePercentage = Mathf.Max(maxLifetimePercentage, particleData[i].x); // Find the particle with the highest completion percentage.
            }
        }

        m_currentStrength = maxLifetimePercentage; // Set tool strength relative to the completion percentage of the best particle.

        m_eraser.ErasingEnabled = true;
        m_eraser.Tool.InputStrength = m_currentStrength * 100f;
        m_eraser.Tool.UpdateStrength();

        //Restart use timer
        if (m_activeCoroutine != null) StopCoroutine(m_activeCoroutine);
        m_activeCoroutine = StartCoroutine(ActiveTimeout());
    }

    private IEnumerator ActiveTimeout()
    {
        yield return new WaitForSeconds(activeTime);

        m_eraser.ErasingEnabled = false;
    }
}
