using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundPlayer))]

public class Effect : MonoBehaviour
{
    [SerializeField] private ParticleSystem usedParticle;

    private ParticleSystem particle;
    private SoundPlayer sound;

    private void Start()
    {
        particle = Instantiate(usedParticle, gameObject.transform);
    }

    private void Awake()
    {   
        sound = GetComponent<SoundPlayer>();
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