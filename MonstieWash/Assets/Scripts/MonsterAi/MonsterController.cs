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
        m_monsterAI.MonsterAttack += MakeAttack;
    }

    private void OnDisable()
    {
        m_monsterAI.MonsterAttack -= MakeAttack;
    }

    /// <summary>
    /// Prepares an attack to be used; chooses one at random, etc. 
    /// </summary>
    private void MakeAttack(object sender, EventArgs e)
    {
        var numAttacks = attackList.Count;

        if (numAttacks == 0) // No attacks to use!
        {
            return;
        }

        // Choose which attack to use
        var chosenAttack = UnityEngine.Random.Range(0, numAttacks);
        StartCoroutine(PerformAttack(attackList[chosenAttack]));
    }

    /// <summary>
    /// Executes the logic for an attack over multiple frames. 
    /// </summary>
    /// <param name="attack"> The ID of the attack to be used (from attackList).</param> 
    IEnumerator PerformAttack(MonsterAttack attack)
    {
        yield return null;
    }
}
