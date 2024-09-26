using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemLifetimeUpdater : MonoBehaviour
{
    [SerializeField] private float completionTime = 5f;

    private ParticleSystem m_particleSystem;
    private ParticleSystem.Particle[] m_particles;
    private List<Vector4> m_particleData = new();
    private int uniqueID;

    private void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
        m_particles = new ParticleSystem.Particle[m_particleSystem.main.maxParticles];
    }

    private void FixedUpdate()
    {
        m_particleSystem.GetCustomParticleData(m_particleData, ParticleSystemCustomData.Custom1);
        m_particleSystem.GetParticles(m_particles);

        for (int i = 0; i < m_particleData.Count; i++)
        {
            var ID = m_particleData[i].z;
            var completionPercentage = Mathf.Clamp01((m_particles[i].startLifetime - m_particles[i].remainingLifetime) / completionTime);
            var particleImageIndex = (ID == 0f) ? Random.Range(0, 3) : m_particleData[i].y;
            var particleID = (ID == 0f) ? ++uniqueID : ID;

            m_particleData[i] = new Vector4(completionPercentage, particleImageIndex, particleID, 0);
        }

        m_particleSystem.SetCustomParticleData(m_particleData, ParticleSystemCustomData.Custom1);
    }
}
