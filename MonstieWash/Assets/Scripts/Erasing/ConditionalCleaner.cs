using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalCleaner : MonoBehaviour
{
    [SerializeField] private float activeTime = 0.5f;

    private BoxCollider2D m_collider;
    private Eraser m_eraser;
    private float currentStrength = 0f;

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

    private void OnParticleCollision(GameObject other)
    {
        var ps = other.GetComponent<ParticleSystem>();
        var particles = new ParticleSystem.Particle[ps.particleCount];
        float maxLifetimePercentage = 0f;
        List<Vector4> particleData = new();
        ps.GetCustomParticleData(particleData, ParticleSystemCustomData.Custom1);
        ps.GetParticles(particles);

        for (int i = 0; i < particles.Length; i++)
        {
            if (particles[i].remainingLifetime == 0f)
            {
                maxLifetimePercentage = Mathf.Max(maxLifetimePercentage, particleData[i].x);
            }
        }

        currentStrength = maxLifetimePercentage;

        m_eraser.ErasingEnabled = true;
        m_eraser.Tool.InputStrength = currentStrength * 100f;
        m_eraser.Tool.UpdateStrength();

        if (m_activeCoroutine != null) StopCoroutine(m_activeCoroutine);
        m_activeCoroutine = StartCoroutine(ActiveTimeout());
    }

    private IEnumerator ActiveTimeout()
    {
        yield return new WaitForSeconds(activeTime);

        m_eraser.ErasingEnabled = false;
    }
}
