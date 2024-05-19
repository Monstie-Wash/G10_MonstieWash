using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class MonsterController : MonoBehaviour
{

    private MonsterBrain m_monsterAI;
    private Animator m_myAnimator;

    private string m_interruptedAnimation;

    [SerializeField] private List<AnimationClip> attackList;

    private void Awake()
    {
        m_myAnimator = GetComponent<Animator>();
        m_monsterAI = FindFirstObjectByType<MonsterBrain>();
    }

    private void OnEnable()
    {
        m_monsterAI.MonsterAttack += Attack;
    }

    private void OnDisable()
    {
        m_monsterAI.MonsterAttack -= Attack;
    }

    /// <summary>
    /// Prepares an attack to be used; chooses one at random.
    /// </summary>
    /// <param name="sender"> The object that sent the attack event.</param> 
    /// <param name="e"> Arguments included in the attack (CURRENTLY UNUSED).</param> 
    private void Attack(object sender, EventArgs e)
    {   
        var numAttacks = attackList.Count;

        if (numAttacks == 0) // No attacks to use!
        {
            return;
        }

        // Choose which attack to use
        var chosenAttack = UnityEngine.Random.Range(0, numAttacks);
        var attack = attackList[chosenAttack];

        // Get and save the animation that is being interrupted
        var animatorInfo = this.m_myAnimator.GetCurrentAnimatorClipInfo(0);
        var currentAnimation = animatorInfo[0].clip.name;
        m_interruptedAnimation = currentAnimation;

        // Play the attack animation
        m_myAnimator.Play(attack.name);
    }

    /// <summary>
    /// Returns the animation to what it was before the monster attacked (via animation event). 
    /// </summary>
    public void AttackFinished()
    {
        m_myAnimator.Play(m_interruptedAnimation);
    }

}
