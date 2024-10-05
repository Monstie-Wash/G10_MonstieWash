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

    [Header("Animatic Settings")]
    [SerializeField] private VideoClip Animatic;
    [SerializeField] private GameSceneManager.Level levelFollowingAnimatic;

    //Private
    private VideoPlayer m_vPlayer;
    private float m_timeSinceLastInput;
    private bool m_moving;

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


    private void MovePerformed(Vector2 movementInput)
    {
        m_timeSinceLastInput = 0;
        m_moving = true;
    }

    private void MoveEnded()
    {
        m_moving = false;

        if (status == Status.Title) StopVideo();
    }


    private void Awake()
    {
        m_vPlayer = GetComponent<VideoPlayer>();

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


    private void PlayTitleVideo()
    {       
        m_vPlayer.clip = TitleVideo;
        m_vPlayer.isLooping = true;
        m_vPlayer.enabled = true;
        m_vPlayer.Play();
    }

    //Called by Slime poster navigation
    public void PlayAnimatic()
    {      
        m_vPlayer.clip = Animatic;
        m_vPlayer.isLooping = false;
        m_vPlayer.enabled = true;
        m_vPlayer.Play();
        status = Status.Animatic;
    }
    private void StopVideo()
    {
        m_vPlayer.enabled = false;
    }

    //Called by main menu start game.
    public void TransitionFromMainMenu()
    {
        StopVideo();
        status = Status.Idle;
        m_vPlayer.loopPointReached += OnVideoFinish;

    }

    private void OnVideoFinish(VideoPlayer vp)
    {
        StopVideo();
        status = Status.Idle;
        GameSceneManager.Instance.StartNewLevel(levelFollowingAnimatic);      
    }
}
