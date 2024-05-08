using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private Sound sound;

    private AudioSource m_audioSource;

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();

        SetupAudio();
    }

    private void SetupAudio()
    {
        m_audioSource.playOnAwake = false;
        m_audioSource.clip = sound.Clip;
        m_audioSource.outputAudioMixerGroup = sound.Mixer;
        m_audioSource.volume = sound.Volume;
        m_audioSource.pitch = sound.Pitch;
        m_audioSource.loop = sound.Loop;
    }

    public void PlaySound(bool mutate = false)
    {
        if (!mutate) m_audioSource.Play();
        else
        {
            var prevVolume = sound.Volume;
            var prevPitch = sound.Pitch;

            sound.Volume *= Random.Range(0.9f, 1.1f);
            sound.Pitch *= Random.Range(0.9f, 1.1f);

            m_audioSource.Play();

            sound.Volume = prevVolume;
            sound.Pitch = prevPitch;
        }
    }

    public void StopSound()
    {
        m_audioSource.Stop();
    }
}
