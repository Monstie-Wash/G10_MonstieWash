using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterAttack", menuName = "ScriptableObjects/MonsterAttack")]

public class MonsterAttack : ScriptableObject
{
    [Tooltip("Name of the attack")][SerializeField] private string attackName; // Name of the attack - might be useful for designers
    [Tooltip("Damage of the attack")][SerializeField] private float damage; // How much damage the attack deals to the player.
    [Tooltip("Animation played when attack is performed")][SerializeField] private AnimationClip attackAnimation; // The animation to play when the attack happens. 
    [Tooltip("SFX if the attack hits the player")][SerializeField] private AudioClip hitSFX; // Special soundeffect for the attack if it connects with the player.

    [HideInInspector] public string AttackName { get { return attackName; } }
    [HideInInspector] public float Damage { get { return damage; } }
    [HideInInspector] public AnimationClip AttackAnimation { get { return attackAnimation; } }
    [HideInInspector] public AudioClip HitSFX { get { return hitSFX; } }

}
