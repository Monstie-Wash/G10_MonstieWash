using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider2D))]

public class PlayerHealth : MonoBehaviour
{
    #region Player Health
    // Player health
    [Tooltip("Player's max health")][SerializeField] private float playerMaxHealth; // The max amount of health the player can have.
    [Tooltip("Player's current health")][SerializeField] private float playerHealth;    // The current amount of health the player has.
    #endregion

    #region Damage Controls
    // Damage controls for designers
    [Tooltip("Animation intensity scales with damage taken up to this amount")][SerializeField] private float damageAnimationCap;   // Amount of damage taken scales the damage animation, up to this amount of damage. 
    [Tooltip("Animation curve for the screen shake upon taking damage")][SerializeField] private AnimationCurve damageShake;    // Animation curve that controls damage shake intensity.
    #endregion

    #region Effect Controls
    // Effect controls for designers
    [Tooltip("Duration of the damage / healing animation (in seconds)")][SerializeField][Range(0f, 2f)] private float animationDuration;   // Duration of the "take damage" and "heal" animations.
    [Tooltip("Animation curve for color shift upon taking damage/healing")][SerializeField] private AnimationCurve colorShift;  // Animation curve that controls damage/healing intensity. 
    #endregion

    #region Other Variables
    // Other
    private bool m_isInvincible = false;    // Flags whether the player is currently invincible after taking damage.
    private Component[] m_spriteRenderers;  // Array of SpriteRenderers from the PlayerHand bone Animation.
    private Collider2D m_playerHurtbox;     // The player's hurtbox (where they can be hit by attacks).
    #endregion

    private void Start()
    {
        m_playerHurtbox = GetComponent<Collider2D>();
        m_spriteRenderers = GetComponentsInChildren<SpriteRenderer>(); 
        playerHealth = playerMaxHealth;
    }

    /// <summary>
    /// Heals damage from the player. 
    /// </summary>
    /// <param name="dmgHealed"> The amount of damage to be healed. </param>
    public void HealDamage(float dmgHealed)
    {
        playerHealth = Mathf.Min(playerMaxHealth, playerHealth + dmgHealed);
        StartCoroutine(PlayHealingEffects());
    }    

    /// <summary>
    /// Plays sound effects, visuals, etc. when the player heals damage. 
    /// </summary>
    IEnumerator PlayHealingEffects()
    {
        var elapsedTime = 0f;

        // Initialize variables needed to shift colors.
        var currentColor = Color.green;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            var overallTime = elapsedTime / animationDuration;

            // Color shifting.
            currentColor.b = colorShift.Evaluate(overallTime);
            currentColor.r = colorShift.Evaluate(overallTime);
            foreach (SpriteRenderer sr in m_spriteRenderers)
            {
                sr.color = currentColor;
            }

            yield return null;
        }

    }

    /// <summary>
    /// Deals damage to the player.
    /// </summary>
    /// <param name="dmgTaken"> The amount of damage to be taken.</param>
    public void TakeDamage(float dmgTaken)
    {
        // Can't take damage if invincible.
        if (m_isInvincible) return;

        playerHealth -= dmgTaken;
        // Check if the player is dead - TO DO when death scene is complete.
        StartCoroutine(PlayDamageEffects(dmgTaken));
    }

    /// <summary>
    /// Plays sound effects, visuals, etc. when the player takes damage.
    /// </summary>
    /// <param name="dmgTaken"> The amount of damage that was dealt. </param>
    IEnumerator PlayDamageEffects(float dmgTaken)
    {
        var elapsedTime = 0f;

        // Initialize variables needed to shake the camera.
        var activeCam = Camera.main;
        var camStartPos = activeCam.transform.position;
        var dmgScale = Mathf.Clamp((dmgTaken / damageAnimationCap) + 1, 1, 2);

        // Initialize variables needed to shift colors.
        var currentColor = Color.red;

        // Enable the player's invincibility
        StartCoroutine(StartInvincibility());

        // Transform the camera's position based on the Animation Curve and the amount of damage taken.
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            var overallTime = elapsedTime / animationDuration;

            // Screen shake.
            var strength = damageShake.Evaluate(overallTime);
            activeCam.transform.position = camStartPos + Random.insideUnitSphere * strength * dmgScale;

            // Color shifting.
            currentColor.b = colorShift.Evaluate(overallTime);
            currentColor.g = colorShift.Evaluate(overallTime);
            foreach (SpriteRenderer sr in m_spriteRenderers)
            {
                sr.color = currentColor;
            }

            yield return null;
        }

        // Reset the camera.
        activeCam.transform.position = camStartPos;
    }

    /// <summary>
    /// Enables the player's invincibility for the specified amount of time, then disables it. 
    /// </summary>
    IEnumerator StartInvincibility()
    {
        var elapsedTime = 0f;
        m_isInvincible = true;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_isInvincible = false;
    }    
}
