using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterAttack", menuName = "ScriptableObjects/MonsterAttack")]

public class MonsterAttack : ScriptableObject
{

    [Tooltip("Damage of the attack")][SerializeField] private float damage; // How much damage the attack deals to the player.
    //[Tooltip("Animation played when attack is performed")][SerializeField] private AnimationClip telegraphAnimation; // The animation to play to telegraph the attack to the player. 
    //[Tooltip("SFX played when attack is performed")][SerializeField] private AudioClip telegraphSFX; // The sound clip to play to telegraph the attack to the player.
    [Tooltip("Animation played when attack is performed")][SerializeField] private AnimationClip attackAnimation; // The animation to play when the attack logic actually happens. 
    //[Tooltip("SFX played when attack is performed")][SerializeField] private AudioClip attackSFX; // The sound clip to play when the attack logic actually happens.
    //[Tooltip("Hurtbox for the attack")][SerializeField] private Collider2D hurtbox; // The hurtbox for the attack (what the player is hit by).
    [Tooltip("SFX if the attack hits the player")][SerializeField] private AudioClip hitSFX; // Special soundeffect for the attack if it connects with the player.

    [HideInInspector] public float Damage { get { return damage; } }
    //[HideInInspector] public AnimationClip TelegraphAnimation { get { return telegraphAnimation; } }
    //[HideInInspector] public AudioClip TelegraphSFX { get { return telegraphSFX; } }
    [HideInInspector] public AnimationClip AttackAnimation { get { return attackAnimation; } }
    //[HideInInspector] public AudioClip AttackSFX { get { return attackSFX; } }
    //[HideInInspector] private Collider2D Hurtbox { get { return hurtbox; } }
    [HideInInspector] private AudioClip HitSFX { get { return hitSFX; } }

}
