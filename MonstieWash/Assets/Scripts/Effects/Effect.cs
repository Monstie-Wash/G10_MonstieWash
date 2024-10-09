using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundPlayer))]

public class Effect : MonoBehaviour
{
    [SerializeField] private ParticleSystem usedParticle;

    private ParticleSystem m_particle;
    private SoundPlayer m_sound;

    private void Awake()
    {   
        m_sound = GetComponent<SoundPlayer>();
    }
    
    private void Start()
    {
        m_particle = Instantiate(usedParticle, gameObject.transform);
    }

    public void Play()
    {
        m_particle.Play();
        m_sound.PlaySound(true, true);
    }

    public void Stop()
    {
        m_particle.Stop();
        m_sound.StopSound();
    }
}
