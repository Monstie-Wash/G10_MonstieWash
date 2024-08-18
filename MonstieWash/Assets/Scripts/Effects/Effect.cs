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
        sound = GetComponent<SoundPlayer>();
    }
    
    private void Start()
    {
        particle = Instantiate(usedParticle, gameObject.transform);
    }

    public void Play()
    {
        particle.Play();
        sound.PlaySound(true, true);
    }

    public void Stop()
    {
        particle.Stop();
        sound.StopSound();
    }
}
