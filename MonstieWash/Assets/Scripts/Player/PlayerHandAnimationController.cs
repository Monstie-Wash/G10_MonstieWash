using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand_AnimationController : MonoBehaviour
{
    private enum AnimState { Stationary, Idle, Grab, Pet };
    private AnimState m_currentAnim = AnimState.Idle;

    [Tooltip("PlayerHand stationary animation")][SerializeField] private AnimationClip anim_stationary;
    [Tooltip("PlayerHand idle animation")][SerializeField] private AnimationClip anim_idle;
    [Tooltip("PlayerHand grab animation")][SerializeField] private AnimationClip anim_grab;
    [Tooltip("PlayerHand petting animation")][SerializeField] private AnimationClip anim_pet;

    private Dictionary<AnimState, AnimationClip> m_handAnims = new();

    private float m_stillTime = 0f;   // Consecutive time spent still
    private bool m_isMoving = false;    // Whether hand is moving this frame
    private bool m_wasMoving = false;   // Whether had was moving last frame
    private bool m_isPetting = false;

    private PlayerHand m_playerHand;
    private Animation m_animator;
    private ToolSwitcher m_toolSwitcher;
    private ItemPickup m_itemPickup;
    private List<MoodArea> m_moodAreas = new();

    private void Awake()
    {
        m_playerHand = GetComponentInParent<PlayerHand>();
        m_animator = GetComponent<Animation>();
        m_toolSwitcher = GetComponentInParent<ToolSwitcher>();
        m_itemPickup = GetComponentInParent<ItemPickup>();
        SetupDictionary();

        GameSceneManager.Instance.OnMonsterScenesLoaded += OnScenesLoaded;
    }

    private void SetupDictionary()
    {
        m_handAnims.Add(AnimState.Stationary, anim_stationary);
        m_handAnims.Add(AnimState.Idle, anim_idle);
        m_handAnims.Add(AnimState.Grab, anim_grab);
        m_handAnims.Add(AnimState.Pet, anim_pet);
    }

    private void OnScenesLoaded()
    {
        GameSceneManager.Instance.OnMonsterScenesLoaded -= OnScenesLoaded;

        foreach (var moodArea in FindObjectsByType<MoodArea>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (moodArea.IsPettable)
            {
                m_moodAreas.Add(moodArea);
                moodArea.OnPetStarted += () => m_isPetting = true;
                moodArea.OnPetEnded += () => m_isPetting = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        TrackIdleTime();

        // Arranged in order of priority (only 4-5 hand animations, no need to overengineer?)
        if (m_itemPickup.HoldingItem || m_toolSwitcher.CurrentToolIndex != -1)
        {
            PlayAnimation(AnimState.Grab);
        }
        else if (m_isPetting)
        {
            PlayAnimation(AnimState.Pet);
        }
        else if (!m_isMoving && m_stillTime > 0.1f && (!m_itemPickup.HoldingItem))
        {
            PlayAnimation(AnimState.Idle);
        }
        else 
        {
            // No animation
            PlayAnimation(AnimState.Stationary);
        }
    }

    private void TrackIdleTime()
    {
        m_wasMoving = m_isMoving;
        m_isMoving = m_playerHand.IsMoving;

        if (!m_isMoving)    // If stationary, start tracking time
        {
            m_stillTime += Time.deltaTime;
        }
        else if (!m_wasMoving && m_isMoving)  // If started moving, reset time spent stationary
        {
            m_stillTime = 0f;
        }
    }

    private void PlayAnimation(AnimState animation)
    {
        if (m_currentAnim == animation) return;

        m_animator.Play(m_handAnims[animation].name);
        m_currentAnim = animation;
    }
}
