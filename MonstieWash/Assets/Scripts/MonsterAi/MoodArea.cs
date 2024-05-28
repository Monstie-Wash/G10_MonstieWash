using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoodArea : MonoBehaviour
{
    //References
    private MonsterBrain m_mb;
    private PlayerHand m_ph;


    [Header("Configurables")]
    [Tooltip("Select the layer of the mood area")] [SerializeField] private LayerMask layerMask;
    [Tooltip("Add new moods to be affected by this, assign the type of mood and by what value it is affected.")] [SerializeField] private List<MoodEffect> moodEffects; //Stores which moods will be changed and by how much.
    [Tooltip("How frequently the area will react to being touched. (Every 'x' seconds).")] [SerializeField] private float areaCooldown; //How frequently the area will react to being touched.
    [Tooltip("Toggle on to make the area reduce effectiveness over frequent use.")] [SerializeField] private bool diminishingEffectiveness; //When repeatedly touched will reduce its effects momentarily.
    [Tooltip("How fast its effectiveness diminishes if above bool toggled on.")] [SerializeField] private int diminishStrength; //How quickly the effectiveness diminishes.
    [Tooltip("Produces debug text in console when toggled on.")] [SerializeField] private bool debug = false;

    //Internal states
    private float currentCooldown;
    private float currentEffectiveness; //Current effectiveness of area.

    [Serializable]
    public struct MoodEffect
    {
        public MoodType mt; //Which mood to target.
        public float reactionStrength; //How much the mood will change by.
    }

    private void Awake()
    {

        m_mb = FindFirstObjectByType<MonsterBrain>();
        m_ph = FindFirstObjectByType<PlayerHand>();
        currentEffectiveness = 100;
    }

    private void OnEnable()
    {
        //Assign to Input system.
        InputManager.Instance.OnActivate_Held += TestTouch;

    }

    private void OnDisable()
    {
        //Unassign from Input system.
        InputManager.Instance.OnActivate_Held -= TestTouch;
    }

    private void Update()
    {
        //Restore Effectiveness over time.
        if (diminishingEffectiveness) currentEffectiveness = Mathf.Clamp(currentEffectiveness +( (diminishStrength/2f) * Time.deltaTime), 0f , 100f);
        //Reduce Cooldown over time.
        currentCooldown = Mathf.Clamp(currentCooldown -= Time.deltaTime, 0f, areaCooldown);
    }


    /// <summary>
    /// Function that is called when a touch is detected by the area.
    /// </summary>
    void OnTouch()
    {
            currentCooldown = areaCooldown; //Reset Cooldown

            //Affect Moods
            foreach (MoodEffect me in moodEffects)
            {
                m_mb.UpdateMood(me.reactionStrength * (currentEffectiveness/100f), me.mt);
                if (debug) print($"Reaction Strength  {me.reactionStrength}  at effectivness of {currentEffectiveness} for the mood {me.mt.MoodName}");
            }

            //Apply diminishing effect if toggled on.
            if (diminishingEffectiveness) currentEffectiveness = Mathf.Clamp(currentEffectiveness -= diminishStrength, 0f, 100f);

    }

    /// <summary>
    /// Called by on activate held from input system, checks if the player hand is overlapping the moodArea.
    /// </summary>
    private void TestTouch()
    {
        //Skip test if area still on cooldown;
        if (currentCooldown > 0) return;

        var toolSwitcher = m_ph.GetComponent<ToolSwitcher>();
        Vector3 toolTipPos = m_ph.transform.position;

        //Get the tool tip as long as you don't have an empty hand
        if (toolSwitcher.CurrentToolIndex != -1) toolTipPos = toolSwitcher.ToolInstances[toolSwitcher.CurrentToolIndex].transform.GetChild(0).position;

        //Check if playerhand is over a mood area and it matches this mood area.
        var colCheck = Physics2D.OverlapCircle(toolTipPos, 0.1f, layerMask, -999999, 999999);

        if (colCheck != null && colCheck.gameObject == this.gameObject)
        {
            //Call On Touch effect
            OnTouch();
        }
    }

}
