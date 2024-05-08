using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] List<Sound> backgroundMusic;
    [SerializeField] Sound deathMusic;
    [SerializeField] Sound victoryMusic;

    private SoundPlayer m_soundPlayer;
    private Sound currentMusic;
    private MusicType currentMusicType;

    public enum MusicType
    {
        Background,
        Death,
        Victory
    }

    private void Awake()
    {
        m_soundPlayer = GetComponent<SoundPlayer>();
        currentMusic = m_soundPlayer.Sound;
        currentMusicType = MusicType.Background;
    }

    private void ChangeMusic(Sound newMusic)
    {
        m_soundPlayer.SwitchSound(newMusic);
        currentMusic = newMusic;
    }

    public void EscalateMusic()
    {
        if (currentMusicType != MusicType.Background) return;

        var currentIndex = backgroundMusic.FindIndex(s => s.Clip == currentMusic.Clip);
        if (currentIndex + 1 >= backgroundMusic.Count) return;
        Sound esclatedMusic = backgroundMusic[currentIndex + 1];

        ChangeMusic(esclatedMusic);
    }

    public void DeescalateMusic()
    {
        if (currentMusicType != MusicType.Background) return;

        var currentIndex = backgroundMusic.FindIndex(s => s.Clip == currentMusic.Clip);
        if (currentIndex - 1 < 0) return;
        Sound deesclatedMusic = backgroundMusic[currentIndex - 1];

        ChangeMusic(deesclatedMusic);
    }

    public void SetMusic(MusicType type)
    {
        switch (type)
        {
            case MusicType.Background:
                {
                    ChangeMusic(backgroundMusic[0]);
                    currentMusicType = MusicType.Background;
                }
                break;
            case MusicType.Death:
                {
                    ChangeMusic(deathMusic);
                    currentMusicType = MusicType.Death;
                }
                break;
            case MusicType.Victory:
                {
                    ChangeMusic(victoryMusic);
                    currentMusicType = MusicType.Victory;
                }
                break;
        }
    }
}
