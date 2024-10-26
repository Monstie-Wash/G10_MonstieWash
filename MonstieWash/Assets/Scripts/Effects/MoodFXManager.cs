using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodFXManager : MonoBehaviour
{
    [System.Serializable]
    private struct MoodParticleSystem
    {
        public MoodType mood;
        public ParticleSystem particleSystem;
    }

    [SerializeField] private List<MoodParticleSystem> moodToSystem = new();

    private Dictionary<MoodType, ParticleSystem> m_moodParticleSystems = new();

    public Dictionary<MoodType, ParticleSystem> MoodParticleSystems { get { return m_moodParticleSystems; } }

    private void Awake()
    {
        foreach (var mps in moodToSystem)
        {
            m_moodParticleSystems.Add(mps.mood, mps.particleSystem);
        }
    }
}
