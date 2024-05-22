using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField] private List<MoodToAnimation> moodToAnimationMap = new(); // maps the name of moods to their animation names
    [SerializeField] private bool debug = false;

    private MonsterBrain m_monsterAI;
    private Animator m_myAnimator;

    private MoodType m_recentHighestMood;

    [Serializable]
    private struct MoodToAnimation
    {
        public MoodType mood;
        public AnimationClip animation;
    }

    private void Awake()
    {
        m_monsterAI = FindFirstObjectByType<MonsterBrain>();
        m_myAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        m_monsterAI.OnMoodChanged += UpdateAnimations;
        m_monsterAI.MonsterAttack += Attack;
    }

    private void OnDisable()
    {
        m_monsterAI.OnMoodChanged -= UpdateAnimations;
        m_monsterAI.MonsterAttack -= Attack;
    }

    /// <summary>
    /// Changes the monster's animation (using the Animator component) to fit its current mood
    /// </summary>
    /// <param name="currentMood">The name of the mood with the highest float value from mimicAI moodData</param>
    private void UpdateAnimations(MoodType currentMood)
    {
        // If mood hasn't changed from last frame, don't bother updating
        if (currentMood == m_recentHighestMood) return;

        // Update the recent highest mood, then play the exit animation followed by the new animation
        m_recentHighestMood = currentMood;
        if (debug) Debug.Log($"Current highest mood was changed to {m_recentHighestMood}");

        // Set mood_changed to true in the animator
        m_myAnimator.SetBool("mood_changed", true); // begins the exit animation (if there is one).
    }

    public void TransitoryAnimationComplete()
    {
        var highestMood = m_monsterAI.HighestMood;
        var animToPlay = moodToAnimationMap.Find(obj => obj.mood == highestMood).animation;

        if (animToPlay == null)
        {
            Debug.LogWarning($"MoodToAnimationMap for MoodType {highestMood} is missing/incorrect");
            return;
        }

        // Play the entry animation for the new mood
        m_myAnimator.Play(animToPlay.name);
        // Set mood_changed to false in the animator 
        m_myAnimator.SetBool("mood_changed", false);
    }

    private void Attack(object sender, EventArgs e)
    {
        Debug.Log("Monster attack!");
    }
}