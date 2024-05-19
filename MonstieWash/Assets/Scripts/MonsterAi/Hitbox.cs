using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestHitbox : MonoBehaviour
{

    [Tooltip("The amount of damage dealt by this hitbox/attack")][SerializeField] private float damage;

    /// <summary>
    /// If the colliding object is the PlayerHealth hitbox, deal damage to the player. 
    /// </summary>
    /// <param name="collision"> The colliding object.</param> 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var isHand = collision.gameObject.GetComponentInChildren<PlayerHealth>();
        if (isHand != null)
        {
            isHand.TakeDamage(damage);
        }
    }
}
