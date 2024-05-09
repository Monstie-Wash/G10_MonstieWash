using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        m_soundPlayer = GetComponent<SoundPlayer>();
    }

    public void PlayAttackSound()
    {
        var index = Random.Range(0, attackSounds.Count - 1);

        m_soundPlayer.SwitchSound(attackSounds[index]);
    }

    public void PlayMoodSound(MoodType mood)
    {
        var sound = moodSounds.Find(ms => ms.mood.MoodName == mood.MoodName).sound;
        if (sound == null) return;

        m_soundPlayer.SwitchSound(sound);
    }
}
