using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Sound", menuName = "ScriptableObjects/Sound")]
public class Sound : ScriptableObject
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioMixerGroup mixer;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField, Range(-3f, 3f)] private float pitch = 1f;
    [SerializeField] private bool loop = false;
    [SerializeField] private bool playOnAwake = false;

    public AudioClip Clip { get { return clip; } }
    public AudioMixerGroup Mixer { get { return mixer; } }
    public float Volume { get { return volume; } set { volume = Mathf.Clamp(value, 0f, 1f); } }
    public float Pitch { get { return pitch; } set { pitch = Mathf.Clamp(value, -3f, 3f); } }
    public bool Loop { get { return loop; } }
    public bool PlayOnAwake { get { return playOnAwake; } }
}
