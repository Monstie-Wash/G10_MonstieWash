using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundPlayer))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] private List<Sound> backgroundMusic;
    [SerializeField] private Sound deathMusic;
    [SerializeField] private Sound victoryMusic;

    private SoundPlayer m_soundPlayer;
    private Sound m_currentMusic;
    private MusicType m_currentMusicType;

    public enum MusicType
    {
        Background,
        Death,
        Victory
    }

    private void Awake()
    {
        m_soundPlayer = GetComponent<SoundPlayer>();
        m_currentMusic = m_soundPlayer.Sound;
        m_currentMusicType = MusicType.Background;
    }

    /// <summary>
    /// Changes the currently playing music for a new sound.
    /// </summary>
    /// <param name="newMusic">The music to play.</param>
    private void ChangeMusic(Sound newMusic)
    {
        m_soundPlayer.SwitchSound(newMusic);
        m_currentMusic = newMusic;
    }

    /// <summary>
    /// Increases the intensity of the background music. Does nothing if background music is not currently playing.
    /// </summary>
    public void EscalateMusic()
    {
        if (m_currentMusicType != MusicType.Background) return;

        var currentIndex = backgroundMusic.FindIndex(s => s.Clip == m_currentMusic.Clip);
        if (currentIndex + 1 >= backgroundMusic.Count) return;
        Sound esclatedMusic = backgroundMusic[currentIndex + 1];

        ChangeMusic(esclatedMusic);
    }

    /// <summary>
    /// Decreases the intensity of the background music. Does nothing if background music is not currently playing.
    /// </summary>
    public void DeescalateMusic()
    {
        if (m_currentMusicType != MusicType.Background) return;

        var currentIndex = backgroundMusic.FindIndex(s => s.Clip == m_currentMusic.Clip);
        if (currentIndex - 1 < 0) return;
        Sound deesclatedMusic = backgroundMusic[currentIndex - 1];

        ChangeMusic(deesclatedMusic);
    }

    /// <summary>
    /// Set the currently playing music to a predefined selection.
    /// </summary>
    /// <param name="type">The music type to play.</param>
    public void SetMusic(MusicType type)
    {
        switch (type)
        {
            case MusicType.Background:
                {
                    ChangeMusic(backgroundMusic[0]);
                    m_currentMusicType = MusicType.Background;
                }
                break;
            case MusicType.Death:
                {
                    ChangeMusic(deathMusic);
                    m_currentMusicType = MusicType.Death;
                }
                break;
            case MusicType.Victory:
                {
                    ChangeMusic(victoryMusic);
                    m_currentMusicType = MusicType.Victory;
                }
                break;
        }
    }
}
