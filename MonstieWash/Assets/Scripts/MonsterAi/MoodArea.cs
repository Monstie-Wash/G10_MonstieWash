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
    [Tooltip("Add new moods to be affected by this, assign the type of mood and by what value it is affected.")] [SerializeField] private List<moodEffect> moodEffects; //Stores which moods will be changed and by how much.
    [Tooltip("How frequently the area will react to being touched. (Every 'x' seconds).")] [SerializeField] private float areaCooldown; //How frequently the area will react to being touched.
    [Tooltip("Toggle on to make the area reduce effectiveness over frequent use.")] [SerializeField] private bool diminishingEffectiveness; //When repeatedly touched will reduce its effects momentarily.
    [Tooltip("How fast its effectiveness diminishes if above bool toggled on.")] [SerializeField] private int diminishStrength; //How quickly the effectiveness diminishes.
    [Tooltip("Produces debug text in console when toggled on.")] [SerializeField] private bool debug;

    //Internal states
    private float currentCooldown;
    private float currentEffectiveness; //Current effectiveness of area.

    [Serializable]
    public struct moodEffect
    {
        public MoodType mt; //Which mood to target.
        public float reactionStrength; //How much the mood will change by.
    }

    private void OnEnable()
    {
        //Assign to Input system.
        InputManager.Inputs.OnActivate_Held += TestTouch;

        m_mb = FindFirstObjectByType<MonsterBrain>();
        m_ph = FindFirstObjectByType<PlayerHand>();
        currentEffectiveness = 100;
    }


    private void Update()
    {
        //Restore Effectiveness over time.
        if (diminishingEffectiveness) currentEffectiveness = Mathf.Clamp(currentEffectiveness +( (diminishStrength/2) * Time.deltaTime), 0 , 100);
        //Reduce Cooldown over time.
        currentCooldown = Mathf.Clamp(currentCooldown -= Time.deltaTime, 0, areaCooldown);
    }


    /// <summary>
    /// Function that is called when a touch is detected by the area.
    /// </summary>
    void OnTouch()
    {
            currentCooldown = areaCooldown; //Reset Cooldown

            //Affect Moods
            foreach (moodEffect me in moodEffects)
            {
                m_mb.UpdateMood(me.reactionStrength * (currentEffectiveness/100), me.mt);
                if (debug) print($"Reaction Strength  {me.reactionStrength}  at effectivness of {currentEffectiveness} for the mood {me.mt.MoodName}");
            }

            //Apply diminishing effect if toggled on.
            if (diminishingEffectiveness) currentEffectiveness = Mathf.Clamp(currentEffectiveness -= diminishStrength, 0, 100);

    }

    /// <summary>
    /// Called by on activate held from input system, checks if the player hand is overlapping the moodArea.
    /// </summary>
    private void TestTouch()
    {
        //Skip test if area still on cooldown;
        if (currentCooldown > 0) return;

        //Check if playerhand is over a mood area and it matches this mood area.
        var colCheck = Physics2D.OverlapCircle(m_ph.transform.position, 0.1f, layerMask, -999999, 999999);
        if (colCheck != null)
        {
            if (colCheck.gameObject != this.gameObject) return;
        }
        else return;
        
        //Call On Touch effect
        OnTouch();
    }

    //Request for Cormac! Currently this just tests based on the hands position but would like it to instead use the tip of the tool if selected, just not sure how tool system works to add it.

}
