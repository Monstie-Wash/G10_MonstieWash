using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MoodToAnimName
{
    public string moodName;
    public string animName;
}

public class MonsterAnimControl : MonoBehaviour
{
    private MonsterBrain m_mimicAI;   
    private string m_currentMood;   // tracks the monster's current mood (accurate to last frame)
    private Animator m_myAnimator;

    [SerializeField] private List<MoodToAnimName> moodToAnimationMap; // maps the name of moods to their animation names

    void Start()
    {
        m_myAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (m_mimicAI == null)
        {
            m_mimicAI = FindFirstObjectByType<MonsterBrain>();
        }
    }

    void Update()
    {
        UpdateAnimations();
    }

    /// <summary>
    // Changes the monster's animation (using the Animator component) to fit its current mood
    /// </summary>
    void UpdateAnimations()
    {
        // Get the name of the mood with the highest float value from mimicAI MoodData
        string highestMood = m_mimicAI.GetHighestMood();
        // Match that name to the name of that mood's animation
        MoodToAnimName animToPlay = moodToAnimationMap.Find(x => x.moodName == highestMood);
        if (animToPlay == null)
        {
            Debug.Log($"MoodToAnimationMap for MoodType {highestMood} is missing/incorrect");
            return;
        }
        // If mood hasn't changed from last frame, don't bother updating animations
        if (m_myAnimator.GetCurrentAnimatorStateInfo(0).IsName(animToPlay.animName))
        {
            return;
        }
        // Play that animation
        m_myAnimator.Play(animToPlay.animName);

    }
}
