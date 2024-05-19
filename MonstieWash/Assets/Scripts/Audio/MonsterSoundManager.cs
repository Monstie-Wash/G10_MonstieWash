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
    private MusicManager m_musicManager;

    private void Awake()
    {
        m_soundPlayer = GetComponent<SoundPlayer>();
        m_monsterBrain = GetComponent<MonsterBrain>();
        m_musicManager = FindFirstObjectByType<MusicManager>();
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

    public void PlayAttackSound()
    {
        var index = Random.Range(0, attackSounds.Count - 1);


        m_soundPlayer.SwitchSound(attackSounds[index]);
        m_soundPlayer.PlaySound(true);
    }

    public void PlayMoodSound(MoodType mood)
    {
        var sound = moodSounds.Find(ms => ms.mood == mood).sound;
        if (sound == null) return;

        m_soundPlayer.SwitchSound(sound);
        m_soundPlayer.PlaySound(true);
    }

    private void UpdateMusic(MoodType mood)
    {
        switch (mood.MoodName)
        {
            case "Happy": m_musicManager.SetMusic(MusicManager.MusicType.Background); break;
            default: m_musicManager.EscalateMusic(); break;
        }
    }
}
