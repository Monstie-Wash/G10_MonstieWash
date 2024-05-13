using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimControl : MonoBehaviour
{
    private MonsterBrain m_monsterAI;   
    private Animator m_myAnimator;

    private string m_recentHighestMood;

    [SerializeField] private List<MoodToAnimation> moodToAnimationMap = new(); // maps the name of moods to their animation names

    [Serializable]
    private struct MoodToAnimation
    {
        public MoodType mood;
        public AnimationClip animation;
    }

    private void Start()
    {
        m_monsterAI = FindFirstObjectByType<MonsterBrain>();
        m_myAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimations();
    }

    /// <summary>
    // Changes the monster's animation (using the Animator component) to fit its current mood
    /// </summary>
    void UpdateAnimations()
    {
        // Get the name of the mood with the highest float value from mimicAI MoodData
        var highestMood = m_monsterAI.GetHighestMood();

        // If mood hasn't changed from last frame, don't bother updating
        if (highestMood == m_recentHighestMood)
        {
            return;
        }

        // Update the recent highest mood, then play the exit animation followed by the new animation
        m_recentHighestMood = highestMood;
        Debug.Log($"Current highest mood was changed to {m_recentHighestMood}");

        // Set mood_changed to true in the animator
        m_myAnimator.SetBool("mood_changed", true); // begins the exit animation (if there is one).
    }

    public void TransitoryAnimationComplete()
    {
        var highestMood = m_monsterAI.GetHighestMood();
        var animToPlay = moodToAnimationMap.Find(obj => obj.mood.MoodName == highestMood).animation;

        if (animToPlay == null)
        {
            Debug.Log($"MoodToAnimationMap for MoodType {highestMood} is missing/incorrect");
            return;
        }

        // Play the entry animation for the new mood
        m_myAnimator.Play(animToPlay.name); 
        // Set mood_changed to false in the animator 
        m_myAnimator.SetBool("mood_changed", false);
    }
}
