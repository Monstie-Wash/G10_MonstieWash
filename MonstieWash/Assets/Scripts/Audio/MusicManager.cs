using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundPlayer))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] private Sound menuMusic;
    [SerializeField] private Sound morningMusic;
    [SerializeField] private Sound eveningMusic;
    [SerializeField] private List<Sound> backgroundMusic;
    [SerializeField] private Sound deathMusic;
    [SerializeField] private Sound victoryMusic;
    [SerializeField] private Sound nurseSting;
    [SerializeField] private Sound upgradeSting;

    private SoundPlayer m_soundPlayer;
    private Sound m_currentMusic;
    private MusicType m_currentMusicType;

    public enum MusicType
    {
        Menu,
        Morning,
        Evening,
        Background,
        Death,
        Victory,
        Nurse,
        ToolUpgrade
    }

    private void Awake()
    {
        m_soundPlayer = GetComponent<SoundPlayer>();
        m_currentMusic = m_soundPlayer.Sound;
        m_currentMusicType = MusicType.Morning;
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
            case MusicType.Menu:
                {                    
                    ChangeMusic(menuMusic);
                    m_currentMusicType = MusicType.Menu;
                }
                break;
            case MusicType.Morning:
                {
                    ChangeMusic(morningMusic);
                    m_currentMusicType = MusicType.Morning;
                }
                break;
            case MusicType.Evening:
                {
                    ChangeMusic(eveningMusic);
                    m_currentMusicType = MusicType.Evening;
                }
                break;
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
            case MusicType.Nurse:
                {
                    ChangeMusic(nurseSting);
                    m_currentMusicType = MusicType.Nurse;
                }
                break;
            case MusicType.ToolUpgrade:
                {
                    ChangeMusic(upgradeSting);
                    m_currentMusicType = MusicType.ToolUpgrade;
                }
                break;
        }
    }
}
