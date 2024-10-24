using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private Sound sound;
    [SerializeField, Tooltip("Hides the warning that appears when no sound is set.")] private bool hideWarning = false;

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
        if (sound == null)
        {
            if (!hideWarning) Debug.LogWarning($"No sound set for {name}!");
            return;
        }
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
    public void PlaySound(bool mutate = false, bool canStack = false)
    {
		if (sound == null) return;
		
        if (!mutate)
        {
            if (canStack) m_audioSource.PlayOneShot(m_audioSource.clip);
            else m_audioSource.Play();
        }
        else
        {
            var prevVolume = sound.Volume;
            var prevPitch = sound.Pitch;

            sound.Volume *= Random.Range(0.9f, 1.1f);
            sound.Pitch *= Random.Range(0.9f, 1.1f);

            if (canStack) m_audioSource.PlayOneShot(m_audioSource.clip);
            else m_audioSource.Play();

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

    /// <summary>
    /// Check if this is playing sound.
    /// </summary>
    public bool IsPlaying()
    {
        if (m_audioSource.isPlaying) return true;
        else return false;
    }
}
