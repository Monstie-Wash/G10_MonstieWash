using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundPlayer))]
public class MonsterSoundManager : MonoBehaviour
{
    [System.Serializable]
    private struct MoodSound
    {
        public MoodType mood;
        public Sound sound;
    }

    [SerializeField] private List<MoodSound> moodSounds = new();
    [SerializeField] private List<Sound> attackSounds = new();

    private SoundPlayer m_soundPlayer;
    private MonsterBrain m_monsterBrain;

    private void Awake()
    {
        m_soundPlayer = GetComponent<SoundPlayer>();
        m_monsterBrain = GetComponent<MonsterBrain>();
    }

    private void OnEnable()
    {
        m_monsterBrain.OnMoodChanged += PlayMoodSound;
        m_monsterBrain.OnMoodChanged += UpdateMusic;
    }

    private void OnDisable()
    {
        m_monsterBrain.OnMoodChanged -= PlayMoodSound;
        m_monsterBrain.OnMoodChanged -= UpdateMusic;
    }

    /// <summary>
    /// Plays a random attack sound. Will be updated when attacks are implemented.
    /// </summary>
    public void PlayAttackSound()
    {
        var index = Random.Range(0, attackSounds.Count);


        m_soundPlayer.SwitchSound(attackSounds[index]);
        m_soundPlayer.PlaySound(true);
    }

    /// <summary>
    /// Plays the sound corresponding to the new current mood.
    /// </summary>
    /// <param name="mood">The mood that is now active.</param>
    private void PlayMoodSound(MoodType mood)
    {
        var sound = moodSounds.Find(ms => ms.mood == mood).sound;
        if (sound == null) return;

        m_soundPlayer.SwitchSound(sound);
        m_soundPlayer.PlaySound(true);
    }

    /// <summary>
    /// Escalates or de-escalates the music based on the new mood.
    /// NOTE: This is a temporary implementation and will be changed to work properly next semester.
    /// </summary>
    /// <param name="mood">The mood that is now active.</param>
    private void UpdateMusic(MoodType mood)
    {
        
    }
}
