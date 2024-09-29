using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator), typeof(SoundPlayer))]

public class MonsterController : MonoBehaviour
{
    [SerializeField] private List<MoodEffects> moodEffectMap = new(); // maps the name of moods to their animation names
    [Tooltip("Place each of the monster attack animations here.")][SerializeField] private List<AnimationClip> attackList;  // List of monster attack animations to be chosen from randomly when an attack is made. 
    [Tooltip("Place completion particle GameObjects here.")][SerializeField] private List<Effect> completionEffectList; // List of Effect objects to play when a scene is completed
    [SerializeField] private AnimationClip flinch;
    [SerializeField] private bool debug = false;
    [SerializeField] Sound attackSound;

    private MonsterBrain m_monsterAI;
    private Animator m_myAnimator;
    private InterruptAnimation m_interruptAnimation;
    private MoodType m_recentHighestMood;
    private SoundPlayer m_soundPlayer;

    public event Action OnInterruptComplete;
    public event Action OnAttackEnd;

    [Serializable]
    private struct MoodEffects
    {
        public MoodType mood;
        public AnimationClip animation;
        public Sound sound;
    }

    private class InterruptAnimation
    {
        private AnimationClip m_interruptedAnimation;
        private int m_rank;

        public AnimationClip interruptedAnimation { get { return m_interruptedAnimation; } }
        public int Rank { get { return m_rank; } }

        public InterruptAnimation(AnimationClip interruptedAnimation, int priority)
        {
            m_interruptedAnimation = interruptedAnimation;
            m_rank = priority;
        }
    }

    private void Awake()
    {
        m_monsterAI = FindFirstObjectByType<MonsterBrain>();
        m_myAnimator = GetComponent<Animator>();
        m_soundPlayer = GetComponent<SoundPlayer>();
    }

    private void OnEnable()
    {
        m_monsterAI.OnMoodChanged += UpdateAnimations;
        m_monsterAI.MonsterAttack += Attack;
        m_monsterAI.OnFlinch += Flinch;
        m_monsterAI.SceneCompleted += ProcessSceneComplete;
    }

    private void OnDisable()
    {
        m_monsterAI.OnMoodChanged -= UpdateAnimations;
        m_monsterAI.MonsterAttack -= Attack;
        m_monsterAI.OnFlinch -= Flinch;
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
        //if (m_interruptAnimation != null) return;

        // Update the recent highest mood, then play the exit animation followed by the new animation
        m_recentHighestMood = currentMood;
        if (debug) Debug.Log($"Current highest mood was changed to {m_recentHighestMood}");

        // Set mood_changed to true in the animator
        m_myAnimator.SetBool("mood_changed", true); // begins the exit animation (if there is one).
    }

    /// <summary>
    ///  Prepares an attack to be used: chooses one at random.
    /// </summary>
    private void Attack()
    {
        var rank = 2;
        var interruptIsNull = false;

        if (m_interruptAnimation != null)
        {
            if (m_interruptAnimation.Rank > rank)        // A higher priority animation is already in progress
            {
                if (debug) Debug.Log("Interrupted animation not null");
                return;
            }
            else
            {
                // Maintain the animation that's being interrupted
                m_interruptAnimation = new InterruptAnimation(m_interruptAnimation.interruptedAnimation, rank);
            }
        }
        else interruptIsNull = true;

        var numAttacks = attackList.Count;

        if (numAttacks == 0)   // No attacks to use!
        {
            if (debug) Debug.LogWarning("Tried to attack but there was no attack to use!");
            return;
        }

        // Randomly choose which attack to use
        var chosenAttack = UnityEngine.Random.Range(0, numAttacks);
        var attack = attackList[chosenAttack];

        // Get and save the animation that's being interrupted
        var animatorInfo = m_myAnimator.GetCurrentAnimatorClipInfo(0);
        if (interruptIsNull) m_interruptAnimation = new InterruptAnimation(animatorInfo[0].clip, rank);        

        // Play the attack animation
        m_myAnimator.Play(attack.name);
    }

    public void AttackStarted()
    {
        if (attackSound == null) return;
        m_soundPlayer.SwitchSound(attackSound);
        m_soundPlayer.PlaySound();
    }

    /// <summary>
    /// Activates the flinch animation.
    /// </summary>
    private void Flinch()
    {
        var rank = 1;
        var interruptIsNull = false;

        if (m_interruptAnimation != null)
        {
            if (m_interruptAnimation.Rank > rank)        // A higher priority animation is already in progress
            {
                if (debug) Debug.Log("Interrupted animation not null");
                return;
            }
            else
            {
                // Maintain the animation that's being interrupted
                m_interruptAnimation = new InterruptAnimation(m_interruptAnimation.interruptedAnimation, rank);
            }
        }
        else interruptIsNull = true;

        if (flinch == null)
        {
            if (debug) Debug.LogWarning("Tried to flinch but there was no animation to use!");
            return;
        }

        // Get and save the animation that's being interrupted
        var animatorInfo = m_myAnimator.GetCurrentAnimatorClipInfo(0);
        if (interruptIsNull) m_interruptAnimation = new InterruptAnimation(animatorInfo[0].clip, rank);

        // Play the flinch animation
        m_myAnimator.Play(flinch.name);
    }

    public void TransitoryAnimationComplete()
    {
        var highestMood = m_monsterAI.HighestMood;
        var moodEffect = moodEffectMap.Find(obj => obj.mood == highestMood);
        var animToPlay = moodEffect.animation;

        if (animToPlay == null)
        {
            if (debug) Debug.LogWarning($"MoodToAnimationMap for MoodType {highestMood} is missing/incorrect");
            return;
        }

        // Play the entry animation for the new mood
        m_myAnimator.Play(animToPlay.name);
        // Set mood_changed to false in the animator 
        m_myAnimator.SetBool("mood_changed", false);

        if (moodEffect.sound == null) return;
        m_soundPlayer.SwitchSound(moodEffect.sound);
        m_soundPlayer.PlaySound();
    }

    public void InterruptAnimationComplete()
    {
        OnInterruptComplete?.Invoke();

        if (m_interruptAnimation != null) 
        {
            // Return to the animation that was playing previously
            m_myAnimator.Play(m_interruptAnimation.interruptedAnimation.name);
            m_interruptAnimation = null;
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

    public void OnAttackEnded()
    {
        OnAttackEnd?.Invoke();
    }
}