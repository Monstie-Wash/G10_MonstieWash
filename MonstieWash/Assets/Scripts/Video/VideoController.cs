using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    private enum Status
    {
        Title,
        Animatic,
        Idle          
    }

    [SerializeField] private Status status;

    [Header("Title Video Settings.")]
    [Tooltip("Video to play on title screen.")][SerializeField] private VideoClip TitleVideo;
    [Tooltip("Whether title plays on initial game load.")] [SerializeField] bool playTitleOnLoad;
    [Tooltip("Whether title video will play after no user input for certain time.")] [SerializeField] bool playOnAfk;
    [Tooltip("How long the video will play after afking.")] [SerializeField] float afkTimeToPlay;
    [Tooltip("Background on title to turn on and off while video is/isn't playing")] [SerializeField] List<GameObject> backgroundObjs;

    [Header("Animatic Settings")]
    [Tooltip("Animatic video to play.")] [SerializeField] private VideoClip Animatic;
    [Tooltip("Level to load after Animatic.")] [SerializeField] private GameSceneManager.Level levelFollowingAnimatic;
   

    //Private
    private VideoPlayer m_vPlayer;
    private float m_timeSinceLastInput; //How long since player moved mouse/joystick.
    private bool m_moving; //Whether player is moving mouse/joystick currently.
    private SoundPlayer soundPlayer;

    private void OnEnable()
    {
        InputManager.Instance.OnMove += MovePerformed;
        InputManager.Instance.OnMove_Ended += MoveEnded;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnMove -= MovePerformed;
        InputManager.Instance.OnMove_Ended -= MoveEnded;
    }

    /// <summary>
    /// Reset move counter whenever input received.
    /// </summary>
    private void MovePerformed(Vector2 movementInput)
    {
        m_timeSinceLastInput = 0;
        m_moving = true;
    }

    /// <summary>
    /// //Stop video when moving stops.
    /// </summary>
    private void MoveEnded()
    {
        m_moving = false;
        if (status == Status.Title) StopVideo();
    }


    private void Awake()
    {
        soundPlayer = FindFirstObjectByType<SoundPlayer>();
        m_vPlayer = GetComponent<VideoPlayer>();
        m_vPlayer.renderMode = VideoRenderMode.CameraFarPlane;
        status = Status.Title;
        if (playTitleOnLoad) PlayTitleVideo();


    }

    private void Update()
    {
        //Increment time since last input.
        if(!m_moving) m_timeSinceLastInput += Time.deltaTime;

        //See if video should play again.
        if (status == Status.Title && playOnAfk && m_timeSinceLastInput > afkTimeToPlay && !m_vPlayer.isPlaying) PlayTitleVideo();

    }

    /// <summary>
    /// Sets correct settings for title vid and plays.
    /// </summary>
    private void PlayTitleVideo()
    {       
        m_vPlayer.clip = TitleVideo;
        m_vPlayer.isLooping = true;
        m_vPlayer.enabled = true;
        m_vPlayer.Play();
        soundPlayer.StopSound();
        foreach (GameObject g in backgroundObjs)
        {
            g.SetActive(false);
        }
    }

    //Called by Slime poster navigation
    /// <summary>
    /// Sets correct settings and plays animatic.
    /// </summary>
    public void PlayAnimatic()
    {      
        m_vPlayer.clip = Animatic;
        m_vPlayer.isLooping = false;
        m_vPlayer.enabled = true;
        m_vPlayer.Play();
        status = Status.Animatic;
        FindFirstObjectByType<SoundPlayer>().StopSound();
    }

    /// <summary>
    /// Disables video player.
    /// </summary>
    private void StopVideo()
    {
        if (status == Status.Title)
        {
            if(!soundPlayer.IsPlaying()) soundPlayer.PlaySound();
            foreach (GameObject g in backgroundObjs)
            {
                g.SetActive(true);
            }
        }
        m_vPlayer.enabled = false;
    }

    /// <summary>
    /// Sets correct settings after leaving main menu, assigns animatic finish function.
    /// </summary>
    //Called by main menu start game.
    public void TransitionFromMainMenu()
    {
        StopVideo();
        status = Status.Idle;
        m_vPlayer.loopPointReached += OnVideoFinish;
        m_vPlayer.renderMode = VideoRenderMode.CameraNearPlane;

    }

    /// <summary>
    /// Transition to slime scene and delete video player.
    /// </summary>
    private void OnVideoFinish(VideoPlayer vp)
    {
        FinishAnimatic();
    }

    public void FinishAnimatic()
    {
        m_vPlayer.loopPointReached -= OnVideoFinish;
        StopVideo();
        status = Status.Idle;
        GameSceneManager.Instance.StartNewLevel(levelFollowingAnimatic);
        m_vPlayer.enabled = false;
    }
}
