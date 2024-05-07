using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimControl : MonoBehaviour
{
    private MonsterBrain m_monsterAI;   
    private Animator m_myAnimator;

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
        // Match that name to the name of that mood's animation
        var animToPlay = moodToAnimationMap.Find(obj => obj.mood.MoodName == highestMood).animation;
        
        if (animToPlay == null)
        {
            Debug.Log($"MoodToAnimationMap for MoodType {highestMood} is missing/incorrect");
            return;
        }
        
        // If mood hasn't changed from last frame, don't bother updating animations (performance increase)
        if (m_myAnimator.GetCurrentAnimatorStateInfo(0).IsName(animToPlay.name))
        {
            return;
        }
        
        // Play that animation
        m_myAnimator.Play(animToPlay.name);
    }
}
