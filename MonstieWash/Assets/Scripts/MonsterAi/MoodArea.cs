using System.Collections.Generic;
using UnityEngine;
using System;

public class MoodArea : MonoBehaviour
{
    //References
    private MonsterBrain m_monsterBrain;
    private PlayerHand m_playerHand;

    [Header("Configurables")]
    [Tooltip("Select the layer of the mood area")] [SerializeField] private LayerMask layerMask;
    [Tooltip("Add new moods to be affected by this, assign the type of mood and by what value it is affected.")] [SerializeField] private List<MoodEffect> moodEffects; //Stores which moods will be changed and by how much.
    [Tooltip("How frequently the area will react to being touched. (Every 'x' seconds).")] [SerializeField] private float areaCooldown; //How frequently the area will react to being touched.
    [Tooltip("Toggle on to make the area reduce effectiveness over frequent use.")] [SerializeField] private bool diminishingEffectiveness; //When repeatedly touched will reduce its effects momentarily.
    [Tooltip("How fast its effectiveness diminishes if above bool toggled on.")] [SerializeField] private int diminishStrength; //How quickly the effectiveness diminishes.
    [Tooltip("Whether touching this should cause the monster to flinch. When false, will enable petting.")] [SerializeField] private bool causesFlinch;
    [Tooltip("Whether or not this area can be petted.")][SerializeField] private bool isPettable;
    [Tooltip("Produces debug text in console when toggled on.")] [SerializeField] private bool debug = false;

    [Header("OutlineGlow")]
    [Tooltip("Which Sprite will receive the outline material")] [SerializeField] private SpriteRenderer spriteToOutline;
    [Tooltip("Create a variant of the better sprite outline shader material with chosen colour and thickness for  this slot.")] [SerializeField] private Material materialToApply;
    [Tooltip("How long the outline should appear after touching area (Multiple duration by amount of areas that affect the same sprite.)")] [SerializeField] private float outlineAppearanceTime;
    private Material m_OriginalMat; //Stores original material to return it to normal after finished touching.

    [Header("FadingSprites")]
    [Tooltip("Fading sprite scripts that will fade in when this area is touched.")] [SerializeField] private List<FadingSprite> spritesToActivate;

    //Internal states
    private float currentCooldown;
    private float currentEffectiveness; //Current effectiveness of area.

    private bool m_isPetting;
    private BoxCollider2D m_collider;
    private bool m_isOnCooldown { get { return currentCooldown != 0f; } }
    private MoodFXManager m_moodFXManager;

    public event Action OnPetStarted;
    public event Action OnPetEnded;

    public bool IsPettable { get { return isPettable; } }

    [Serializable]
    public struct MoodEffect
    {
        public MoodType mood; //Which mood to target.
        public float reactionStrength; //How much the mood will change by.
    }

    private void Awake()
    {

        m_monsterBrain = FindFirstObjectByType<MonsterBrain>();
        m_playerHand = FindFirstObjectByType<PlayerHand>();
        m_collider = GetComponent<BoxCollider2D>();
        currentEffectiveness = 100;

        //Assign values for creating outline
        if (spriteToOutline != null)
        {
            m_OriginalMat = spriteToOutline.material;
        }
    }

    private void Start()
    {
        m_moodFXManager = FindFirstObjectByType<MoodFXManager>();
    }

    private void OnEnable()
    {
        //Assign to Input system.
        InputManager.Instance.OnActivate_Held += TestTouch;
        InputManager.Instance.OnActivate_Ended += StopTouch;
    }

    private void OnDisable()
    {
        //Unassign from Input system.
        InputManager.Instance.OnActivate_Held -= TestTouch;
        InputManager.Instance.OnActivate_Ended += StopTouch;
    }

    private void Update()
    {
        //Restore Effectiveness over time.
        if (diminishingEffectiveness) currentEffectiveness = Mathf.Clamp(currentEffectiveness +( (diminishStrength/2f) * Time.deltaTime), 0f , 100f);
        //Reduce Cooldown over time.
        currentCooldown = Mathf.Clamp(currentCooldown -= Time.deltaTime, 0f, areaCooldown);

        //Update shader based on recent touch
        if (spriteToOutline != null && spriteToOutline.material.HasProperty("_TimeActive"))
        {
            var timeProperty = spriteToOutline.material.GetFloat("_TimeActive");

            //Reset shader to original after timer runs out.
            if (timeProperty <= 0) spriteToOutline.material = m_OriginalMat;

            //Reduce Shaders TimeActive Property
            spriteToOutline.material.SetFloat("_TimeActive", Mathf.MoveTowards(timeProperty,0,Time.deltaTime));
        };
    }


    /// <summary>
    /// Function that is called when a touch is detected by the area.
    /// </summary>
    private void ApplyTouch()
    {
        currentCooldown = areaCooldown; //Reset Cooldown

        //Affect Moods
        foreach (MoodEffect moodEffect in moodEffects)
        {
            m_monsterBrain.UpdateMood(moodEffect.reactionStrength * (currentEffectiveness/100f), moodEffect.mood);
            if (debug) print($"Reaction Strength  {moodEffect.reactionStrength}  at effectivness of {currentEffectiveness} for the mood {moodEffect.mood.MoodName}");

            m_moodFXManager.MoodParticleSystems[moodEffect.mood]?.Play();
        }

        //Apply diminishing effect if toggled on.
        if (diminishingEffectiveness) currentEffectiveness = Mathf.Clamp(currentEffectiveness -= diminishStrength, 0f, 100f);

        //Cause a flinch
        if (causesFlinch) m_monsterBrain.Flinch();

        if (isPettable && !m_isPetting)
        {
            OnPetStarted?.Invoke();
            m_isPetting = true;
        }

    }

    /// <summary>
    /// Called by on activate held from input system, checks if the player hand is overlapping the moodArea.
    /// </summary>
    private void TestTouch()
    {
        var toolSwitcher = m_playerHand.GetComponent<ToolSwitcher>();
        Vector2 toolTipPos = m_playerHand.transform.position;

        //Get the tool tip as long as you don't have an empty hand
        if (toolSwitcher.CurrentToolIndex != -1) toolTipPos = toolSwitcher.ToolInstances[toolSwitcher.CurrentToolIndex].transform.GetChild(0).position;

        //Check if playerhand is over this mood area
        if (m_collider.bounds.Contains(toolTipPos))
        {
            if (m_isOnCooldown) return;

            //Call On Touch effect
            ApplyTouch();

            if (spritesToActivate != null && spritesToActivate.Count > 0)
            {
                foreach (FadingSprite fs in spritesToActivate)
                {
                    fs.FadeIn();
                }
            }

            if (spriteToOutline != null)
            {
                spriteToOutline.material = materialToApply;
                if (spriteToOutline.material.HasProperty("_TimeActive")) spriteToOutline.material.SetFloat("_TimeActive", outlineAppearanceTime);
            }
        }
        else StopTouch();
    }

    private void StopTouch()
    {
        if (m_isPetting)
        {
            OnPetEnded?.Invoke();
            m_isPetting = false;
        }
    }
}
