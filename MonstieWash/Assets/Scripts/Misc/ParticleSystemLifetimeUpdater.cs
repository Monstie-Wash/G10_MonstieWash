using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemLifetimeUpdater : MonoBehaviour
{
    [SerializeField] private float completionTime = 5f;

    private ParticleSystem m_particleSystem;
    private List<Vector4> m_completionPercentage = new();

    private void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        m_completionPercentage.Clear();
        var particles = new ParticleSystem.Particle[m_particleSystem.particleCount];
        m_particleSystem.GetParticles(particles);

        for (int i = 0; i < m_particleSystem.particleCount; i++)
        {
            var completionPercentage = Mathf.Clamp01((particles[i].startLifetime - particles[i].remainingLifetime) / completionTime);
            m_completionPercentage.Add(new Vector4(completionPercentage, 0, 0, 0));
        }

        m_particleSystem.SetCustomParticleData(m_completionPercentage, ParticleSystemCustomData.Custom1);
    }
}
