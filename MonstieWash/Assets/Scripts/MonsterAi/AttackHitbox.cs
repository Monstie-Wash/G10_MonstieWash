using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class AttackHitbox : MonoBehaviour
{
    [Tooltip("The amount of damage dealt by this hitbox/attack")][SerializeField] private float damage;     // The amount of damage dealt by the attacks
    [Tooltip("Only hits objects on this layer")][SerializeField] private LayerMask layerMask;               // Only collides with objects on this layer - for example, if set to PlayerHand, only collides with the player's hand. Performance increase

    // Faster than using GetComponent to determine whether the colliding object is PlayerHand over and over
    private PlayerHand m_playerHand;
    private PlayerHealth m_playerHealth;

    private void Start()
    {
        m_playerHand = FindFirstObjectByType<PlayerHand>();
        m_playerHealth = m_playerHand.GetComponentInChildren<PlayerHealth>();
    }

    // Note: OnTriggerStay2D method only damages the player if the hand is moving, assumedly because the Physics System currently isn't running while the hand is stationary. Look into possible solutions when polishing 

    /// <summary>
    /// If the colliding object is the PlayerHealth hitbox, deal damage to the player.
    /// </summary>
    /// <param name="collision"> The colliding object. </param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (layerMask == (layerMask | (1 << collision.transform.gameObject.layer)))
        {
            if (collision == m_playerHealth.PlayerHurtbox)
            {
                m_playerHealth.TakeDamage(damage);
            }
        }
    }
}
