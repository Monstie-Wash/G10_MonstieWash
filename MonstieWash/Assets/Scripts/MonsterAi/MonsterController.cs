using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class MonsterController : MonoBehaviour
{

    private MonsterBrain m_monsterAI;
    private Animator m_myAnimator;

    [SerializeField] private List<MonsterAttack> attackList;

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
    private void Attack(object sender, EventArgs e)
    {   
        var numAttacks = attackList.Count;

        if (numAttacks == 0) // No attacks to use!
        {
            return;
        }

        // Choose which attack to use
        var chosenAttack = UnityEngine.Random.Range(0, numAttacks);
        Debug.Log($"Chose to use {attackList[chosenAttack].AttackAnimation.name}!");

        // Use the attack
        StartCoroutine(PerformAttack(attackList[chosenAttack]));
    }

    private IEnumerator PerformAttack(MonsterAttack attack)
    {
        m_myAnimator.Play(attack.AttackAnimation.name);

        while (m_myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
    }
}
