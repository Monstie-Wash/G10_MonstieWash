using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private Sound sound;

    private AudioSource m_audioSource;

    public Sound Sound { get { return sound; } }

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();

        SetupAudio();
    }

    /// <summary>
    /// Sets up the AudioSource to play the sound, playing it if required.
    /// </summary>
    private void SetupAudio()
    {
        m_audioSource.playOnAwake = false;
        m_audioSource.clip = sound.Clip;
        m_audioSource.outputAudioMixerGroup = sound.Mixer;
        m_audioSource.volume = sound.Volume;
        m_audioSource.pitch = sound.Pitch;
        m_audioSource.loop = sound.Loop;

        if (sound.PlayOnAwake) PlaySound();
    }

    /// <summary>
    /// Plays the sound.
    /// </summary>
    /// <param name="mutate">Whether or not to make minor modifications to add variety.</param>
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

    /// <summary>
    /// Stops the sound.
    /// </summary>
    public void StopSound()
    {
        m_audioSource.Stop();
    }

    /// <summary>
    /// Switches the sound for a new one.
    /// </summary>
    /// <param name="newSound">The new sound to replace the old one with.</param>
    public void SwitchSound(Sound newSound)
    {
        if (newSound == sound) return;

        StopSound();
        sound = newSound;
        SetupAudio();
    }
}
