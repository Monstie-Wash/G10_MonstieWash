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
    [Header("Damage Controls")]
    [Tooltip("Animation intensity scales with damage taken up to this amount")][SerializeField] private float damageAnimationCap;   // Amount of damage taken scales the damage animation, up to this amount of damage. 
    [Tooltip("Animation curve for the screen shake upon taking damage")][SerializeField] private AnimationCurve damageShake;    // Animation curve that controls damage shake intensity.
    #endregion

    #region Effect Controls
    // Effect controls for designers
    [Header("Effect Controls")]
    [Tooltip("Duration of the damage / healing animation (in seconds)")][SerializeField][Range(0f, 2f)] private float animationDuration;   // Duration of the "take damage" and "heal" animations.
    [Tooltip("Animation curve for color shift upon taking damage/healing")][SerializeField] private AnimationCurve colorShift;  // Animation curve that controls damage/healing intensity. 
    #endregion

    #region Knockback Controls
    //Values to tweak damage knockback functionality.
    [Header("Knockback Controls")]
    [Tooltip("How long the hand is knocked back for.")] [SerializeField] private float knockbackDuration;
    [Tooltip("How strong the knockbakc force is")] [SerializeField] private float knockbackInitialStrength;
    [Tooltip("Controls knockback strength over time")] [SerializeField] private AnimationCurve knockbackCurve;
    [Tooltip("How strongly the hand is slowed after being hit")] [SerializeField] private float slowStrength;
    [Tooltip("How long the hand is slowed for")] [SerializeField] private float slowDuration;

    //Hidden
    private float m_originalMoveSpeed; //Used to restore move speed back to normal after slow takes place.
    private PlayerHand m_hand; //Reference to hand script.

    #endregion

    #region Other Variables
    // Other
    private bool m_isInvincible = false;    // Flags whether the player is currently invincible after taking damage.
    private Component[] m_spriteRenderers;  // Array of SpriteRenderers from the PlayerHand bone Animation.
    private Collider2D m_playerHurtbox;     // The player's hurtbox (where they can be hit by attacks).
    #endregion

    #region Accessors
    public Collider2D PlayerHurtbox { get { return m_playerHurtbox;} }
    #endregion

    private void Start()
    { 
        m_playerHurtbox = GetComponent<Collider2D>();
        m_spriteRenderers = GetComponentsInChildren<SpriteRenderer>(); 
        playerHealth = playerMaxHealth;
        m_hand = FindFirstObjectByType<PlayerHand>();
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

        //playerHealth -= dmgTaken;
        //if (playerHealth < 0f)
        //{
        //    FindFirstObjectByType<MusicManager>().SetMusic(MusicManager.MusicType.Death);
        //    playerHealth = playerMaxHealth;
        //    transform.parent.gameObject.SetActive(false);
        //    m_gameSceneManager.PlayerDied();
        //    return;
        //}

        StartCoroutine(PlayDamageEffects(dmgTaken));
        StartCoroutine(ApplyKnockbackEffects());
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
    
    /// <summary>
    /// Applies a slow and knockback away from the centre of the monster, knockback and slow reduce over time.
    /// </summary>
    /// <returns></returns>
    IEnumerator ApplyKnockbackEffects()
    {
        var timeRunning = 0f;

        //Get the centre of the screen.
        var centre = Camera.main.WorldToScreenPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, Camera.main.nearClipPlane));
        //Calculate knockback direction.
        var dir = gameObject.transform.position - centre;

        //If slow or knockback hasn't completed continue running.
        while (timeRunning < knockbackDuration || timeRunning < slowDuration)
        {
            //Move hand in direction of knockback by animation curve strength at time.
            m_hand.gameObject.transform.position += dir.normalized * knockbackCurve.Evaluate(timeRunning / knockbackDuration) * Time.deltaTime;

            timeRunning += Time.deltaTime;
        }



        yield return null;
    }
}
