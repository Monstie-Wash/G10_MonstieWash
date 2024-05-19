using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class PlayerHealth : MonoBehaviour 
{
    // Player Health
    [Tooltip("Player's max health")][SerializeField] private float playerMaxHealth;     // The max amount of health the player can have.
    [Tooltip("Player's current health")][SerializeField] private float playerHealth;    // The current amount of health the player has.

    //Damage Controls for designers
    [Tooltip("Damage beyond this value won't affect the intensity of damage animations")][SerializeField] private float damageAnimationCap; // Amount of damage taken scales the damage animation, up to this amount of damage.
    [Tooltip("Duration of the damage animation (in seconds)")][SerializeField] private float damageAnimationDuration;                       // Duration of the "take damage" animation.
    [Tooltip("Animation curve for screen shake upon taking damage")][SerializeField] private AnimationCurve damageShake;                    // An Animation Curve 
    [Tooltip("How long the player remains invincible after taking damage (in seconds)")][SerializeField] private float invincibilityDuration;       // After taking damage, the player is immune to further damage for this length of time.
    private bool m_isInvincible = false;

    //Other
    private Collider2D m_playerHitbox;

    private void Start()
    {
        m_playerHitbox = GetComponent<Collider2D>();
        playerHealth = playerMaxHealth;
    }

    /// <summary>
    /// Heals damage from the player. 
    /// </summary>
    /// <param name="dmgHealed"> The amount of damage to be healed.</param> 
    public void HealDamage(float dmgHealed)
    {
        playerHealth = Mathf.Min(playerMaxHealth, playerHealth + dmgHealed);
        StartCoroutine(PlayHealingEffects());
    }

    /// <summary>
    /// Plays sound effects, visuals, etc. for when the player heals damage.
    /// </summary>
    IEnumerator PlayHealingEffects()
    {
        // Empty at the moment - add effects here when they're finished, or delete if designers don't need healing effects
        yield return null;
    }

    /// <summary>
    /// Deals damage to the player. 
    /// </summary>
    /// <param name="dmgTaken"> The amount of damage to be dealt.</param> 
    public void TakeDamage(float dmgTaken)
    {
        // Can't take damage if invincible
        if (m_isInvincible)
            return;

        playerHealth -= dmgTaken;
        // Check if the player is dead - to do when death scene is complete
        StartCoroutine(PlayDamageEffects(dmgTaken));
    }

    /// <summary>
    /// Plays sound effects, visuals, etc. for when the player takes damage.
    /// </summary>
    /// <param name="dmgTaken"> The amount of damage that was dealt.</param> 
    IEnumerator PlayDamageEffects(float dmgTaken)
    {
        // Initialize variables to shake the camera
        var activeCam = Camera.main;
        var camStartPos = activeCam.transform.position;
        var elapsedTime = 0f;
        var dmgScale = Mathf.Clamp((dmgTaken / damageAnimationCap) + 1, 1, 2);

        // Enable the player's invincibility
        StartCoroutine(StartInvincibility());

        // Transform the camera's position based on Animation Curve and the amount of damage taken
        while (elapsedTime < damageAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            var strength = damageShake.Evaluate(elapsedTime / damageAnimationDuration);
            activeCam.transform.position = camStartPos + Random.insideUnitSphere * strength * dmgScale;
            yield return null;
        }

        // Reset the camera
        activeCam.transform.position = camStartPos;
    }

    /// <summary>
    /// Enables the player's invincibility for the specified amount of time, then disables it. 
    /// </summary>
    IEnumerator StartInvincibility()
    {
        var elapsedTime = 0f;
        m_isInvincible = true;

        while (elapsedTime < invincibilityDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_isInvincible = false;
    }
}
