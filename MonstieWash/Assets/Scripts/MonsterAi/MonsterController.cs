using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]

public class MonsterController : MonoBehaviour
{
    [SerializeField] private List<MoodToAnimation> moodToAnimationMap = new(); // maps the name of moods to their animation names
    [SerializeField] private bool debug = false;

    private MonsterBrain m_monsterAI;
    private Animator m_myAnimator;

    private AnimationClip m_interruptedAnimation = null;
    [Tooltip("Place each of the monster attack animations here.")][SerializeField] private List<AnimationClip> attackList;  // List of monster attack animations to be chosen from randomly when an attack is made. 

    [Tooltip("Place completion particle GameObjects here.")][SerializeField] private List<Effect> completionEffectList; // List of Effect objects to play when a scene is completed

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
        m_monsterAI.SceneCompleted += ProcessSceneComplete;
    }

    private void OnDisable()
    {
        m_monsterAI.OnMoodChanged -= UpdateAnimations;
        m_monsterAI.MonsterAttack -= Attack;
        m_monsterAI.SceneCompleted -= ProcessSceneComplete;
    }

    /// <summary>
    /// Changes the monster's animation (using the Animator component) to fit its current mood
    /// </summary>
    /// <param name="currentMood">The name of the mood with the highest float value from mimicAI moodData</param>
    private void UpdateAnimations(MoodType currentMood)
    {
        // If mood hasn't changed from last frame, don't bother updating
        if (currentMood == m_recentHighestMood) return;

        // If currently performing an attack, should NOT update
        if (m_interruptedAnimation != null) return;

        // Update the recent highest mood, then play the exit animation followed by the new animation
        m_recentHighestMood = currentMood;
        if (debug) Debug.Log($"Current highest mood was changed to {m_recentHighestMood}");

        // Set mood_changed to true in the animator
        m_myAnimator.SetBool("mood_changed", true); // begins the exit animation (if there is one).
    }

    /// <summary>
    ///  Prepares an attack to be used: chooses one at random.
    /// </summary>
    /// <param name="sender"> The object that sent the attack event.</param>
    /// <param name="e"> Arguments included in the attack event (CURRENTLY UNUSED).</param>
    private void Attack()
    {
        if (m_interruptedAnimation != null)        // Another attack is already in progress
        {
            if (debug) Debug.Log("Interrupted animation not null");
            return;
        }
        var numAttacks = attackList.Count;

        if (numAttacks == 0)   // No attacks to use!
        {
            if (debug) Debug.LogWarning("Tried to attack but there was no attack to use");
            return;
        }

        // Randomly choose which attack to use
        var chosenAttack = UnityEngine.Random.Range(0, numAttacks);
        var attack = attackList[chosenAttack];

        // Get and save the animation that's being interrupted
        var animatorInfo = this.m_myAnimator.GetCurrentAnimatorClipInfo(0);
        m_interruptedAnimation = animatorInfo[0].clip;

        // Play the attack animation
        m_myAnimator.Play(attack.name);
    }

    public void TransitoryAnimationComplete()
    {
        var highestMood = m_monsterAI.HighestMood;
        var animToPlay = moodToAnimationMap.Find(obj => obj.mood == highestMood).animation;

        if (animToPlay == null)
        {
            if (debug) Debug.LogWarning($"MoodToAnimationMap for MoodType {highestMood} is missing/incorrect");
            return;
        }

        // Play the entry animation for the new mood
        m_myAnimator.Play(animToPlay.name);
        // Set mood_changed to false in the animator 
        m_myAnimator.SetBool("mood_changed", false);
    }

    public void AttackAnimationComplete()
    {
        if (m_interruptedAnimation != null) 
        {
            // Return to the animation that was playing previously
            m_myAnimator.Play(m_interruptedAnimation.name);
            m_interruptedAnimation = null;
        }
    }

    /// <summary>
    /// Executes the logic required when a scene is completed (the last task is finished). Event is received from MonsterBrain <-- TaskTracker
    /// </summary>
    /// <param name="activeScene"> The scene that was completed. </param>
    private void ProcessSceneComplete(Scene activeScene)
    {
        // More required logic can go here
        StartCoroutine(PlayCompletionSparkles());
    }

    IEnumerator PlayCompletionSparkles()
    {
        var randTime = 0f;
        foreach (var effect in completionEffectList)
        {
            randTime = UnityEngine.Random.Range(0.15f, 0.45f);
            effect.Play();
            yield return new WaitForSeconds(randTime);
        }
        
    }
}