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
    
    private Dictionary<AnimState, AnimationClip> m_handAnims = new Dictionary<AnimState, AnimationClip>();

    private float m_stillTime = 0f;   // Consecutive time spent still
    private bool m_isMoving = false;    // Whether hand is moving this frame
    private bool m_wasMoving = false;   // Whether had was moving last frame

    private PlayerHand m_playerHand;
    private Animation m_animator;
    private ToolSwitcher m_toolSwitcher;

    private void Awake()
    {
        m_playerHand = GetComponentInParent<PlayerHand>();
        m_animator = GetComponent<Animation>();
        m_toolSwitcher = GetComponentInParent<ToolSwitcher>();
        SetupDictionary();
    }

    private void SetupDictionary()
    {
        m_handAnims.Add(AnimState.Stationary, anim_stationary);
        m_handAnims.Add(AnimState.Idle, anim_idle);
        m_handAnims.Add(AnimState.Grab, anim_grab);
        m_handAnims.Add(AnimState.Pet, anim_pet);
    }

    // Update is called once per frame
    void Update()
    {
        TrackIdleTime();

        // Arranged in order of priority (only 4-5 hand animations, no need to overengineer?)
        if (!m_toolSwitcher.HandFree)
        {
            PlayAnimation(AnimState.Grab);
        }
        else if (!m_isMoving && m_stillTime > 0.25f && (m_toolSwitcher.HandFree))
        {
            PlayAnimation(AnimState.Idle);
        }
        // Petting animation priority should go here
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
