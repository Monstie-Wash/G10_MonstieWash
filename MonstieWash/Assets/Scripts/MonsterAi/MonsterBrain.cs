using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MonsterBrain : MonoBehaviour
{
    [System.Serializable]
    protected class MoodData
    {
        public MoodType mood;
        public float value;
    }

    public System.Action<MoodType> OnMoodChanged;

    [Tooltip("Add all moodtype objects intended for this brain here.")] [SerializeField] protected List<MoodData> moodData = new(); //Scriptable objects holding data about moods.
    private int m_designerSanityBuff = 10; // A multiplier to reduce the tiny size of numbers used in setting up scriptable objects. Recommended set at 10.
    
    [Tooltip("Updates the debug window when turned on")] [SerializeField] private bool debug = false;
    [Tooltip("Pauses the brain when on.")][SerializeField] private bool pause = false;
    [Tooltip("Attach Text Mesh Pro Box here for displaying debug info")][SerializeField] private TextMeshProUGUI debugUi;

    [HideInInspector] public MoodType HighestMood { get; private set; }
    [HideInInspector] public HashSet<MoodType> Moods { get; private set; } = new();

    private void Awake()
    {
        foreach (var data in moodData)
        {
            Moods.Add(data.mood);
        }
    }

    private void Update()
    {
        if (pause) return;

        //Moods move by their natural change value to their resting point value.
        NaturalChange();
        //Moods are affected by Chaos Multipliers.
        ChaoticInterference();
        //Moods affect their positive chain reactions.
        PositiveChainReactions();
        //Moods affect their negative chain reactions.
        NegativeChainReactions();
        //Moods are kept to their upper and lower limits.
        MaintainLimits();
        //Keep up-to-date on the current mood
        UpdateHighestMood();

        //Debug Updates
        if (debug) UpdateDebugText();
    }

    /// <summary>
    /// Accesses the Moods resting point and natural rate of change, then moves mood towards its resting point by its rate of change.
    /// </summary>
    private void NaturalChange()
    {
        for (int i = 0; i < moodData.Count; i++)
        {
            //Skip if doesn't naturally update.
            if (moodData[i].mood.MoodNaturalChange == 0) continue;

            float currentValue = moodData[i].value;
            //Move currentvalue towards resting point by rate of change;
            currentValue = FloatTowardsTarget(currentValue, moodData[i].mood.MoodNaturalChange * Time.deltaTime , moodData[i].mood.MoodRestingPoint);
            //Assign new value to active mood.
            moodData[i].value = currentValue;

            if (debug) print("Active Mood: " + moodData[i].mood.MoodName + " naturally changed to " + currentValue );
        }
    }

    /// <summary>
    ///  Accesses the Moods chaos multiplier, randomly generates a number using the chaos multiplier as the upper and lower bound and then applies the random number to the moods value.
    /// </summary>
    private void ChaoticInterference()
    {
        for (int i = 0; i < moodData.Count; i++)
        {           
            if (moodData[i].mood.ChaosMultiplier == 0) continue; //Skip chaos values of 0

            float currentValue = moodData[i].value;
            //Determine chaotic value
            float chaosVal = Random.Range(-moodData[i].mood.ChaosMultiplier, moodData[i].mood.ChaosMultiplier) * m_designerSanityBuff; //numbers are incredibly small *10 makes it more reasonable for designers.

            //Changes current value by chaos value;
            moodData[i].value = currentValue + (chaosVal * Time.deltaTime);

            if (debug) print("Active Mood: " + moodData[i].mood.MoodName + " chaotically changed by " + chaosVal * Time.deltaTime);
        }
    }

    /// <summary>
    /// Accesses moods list of positive chain reaction moods, applies a positive addition to the listed moods based on the positive reaction strength and how far the 
    /// the current mood is inbetween its lower and upper bounds.
    /// </summary>
    private void PositiveChainReactions()
    {
        //Loop through active moods.
        for (int i = 0; i < moodData.Count; i++)
        {
            if (moodData[i].mood.PositiveReactionStrength == 0) continue; //Skip if reaction strenght is 0
            
            //Determine positive strength of current mood. (How far it is between its lower and upper limit)
            var percentageStrength = ((moodData[i].value - moodData[i].mood.MoodLowerLimit) * 100) / (moodData[i].mood.MoodUpperLimit - moodData[i].mood.MoodLowerLimit);
            float chainAmount = (percentageStrength / 100) * moodData[i].mood.PositiveReactionStrength * m_designerSanityBuff; //numbers are incredibly small *10 makes it more reasonable for designers.

            //Loop through list of positive reactions in mood.
            for (int j = 0; j < moodData[i].mood.PositiveChainReactions.Count; j++)
            {
                var targetMoodData = moodData.Find(item => item.mood == moodData[i].mood.PositiveChainReactions[j]);

                //Apply positive chain amount to each active mood that is in the list of positive reactions.
                targetMoodData.value += chainAmount * Time.deltaTime;

                if (debug) print("Active Mood: " + moodData[i].mood.MoodName + " positively influenced " + targetMoodData.mood.MoodName + "by amount " + chainAmount * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Accesses moodsl ist of negative chain reaction moods, applies a negative reduction to the listed moods based on the negative reaction strength
    /// and how far the current mood is inbetween its lower and upper bounds.
    /// </summary>
    private void NegativeChainReactions()
    {
        //Loop through active moods.
        for (int i = 0; i < moodData.Count; i++)
        {
            if (moodData[i].mood.NegativeReactionStrength == 0) continue; //Skip if reaction strenght is 0

            //Determine positive strength of current mood. (How far it is between its lower and upper limit)
            var percentageStrength = ((moodData[i].value - moodData[i].mood.MoodLowerLimit) * 100) / (moodData[i].mood.MoodUpperLimit - moodData[i].mood.MoodLowerLimit);
            float chainAmount = (percentageStrength / 100) * moodData[i].mood.NegativeReactionStrength * m_designerSanityBuff; //numbers are incredibly small *10 makes it more reasonable for designers.

            //Loop through list of negative reactions in mood.
            for (int j = 0; j < moodData[i].mood.NegativeChainReactions.Count; j++)
            {
                var targetMoodData = moodData.Find(item => item.mood == moodData[i].mood.NegativeChainReactions[j]);

                //Apply negative chain amount to each active mood that is in the list of negative reactions.
                targetMoodData.value -= chainAmount * Time.deltaTime;

                if (debug) print("Active Mood: " + moodData[i].mood.MoodName + " negatively influenced " + targetMoodData.mood.MoodName + " by amount " + chainAmount * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Calls other function maintain limit for each mood currently active.
    /// </summary>
    private void MaintainLimits()
    {
        for (int i = 0; i < moodData.Count; i++)
        {
            MaintainLimit(i);
        }
    }

    /// <summary>
    /// Checks if mood given by its current index in active list has gone over or under its limits and then corrects it back.
    /// </summary>
    /// <param name="moodInt"> Given index of desired mood in active moods.</param> 
    private void MaintainLimit(int moodInt)
    {
        moodData[moodInt].value = Mathf.Clamp(moodData[moodInt].value, moodData[moodInt].mood.MoodLowerLimit, moodData[moodInt].mood.MoodUpperLimit);
    }

    /// <summary>
    /// Tool to move a float value towards a target by a certain amount whether negative or positive.
    /// </summary>
    /// <param name="value"> The value to move</param>
    /// <param name="change"> The amount to change the value by</param>
    /// <param name="target"> The target value to move towards</param>
    /// <returns></returns>
    private float FloatTowardsTarget(float value, float change, float target)
    {
        //Exit early if already at target.
        if (value == target) return value;

        //Determine direction of movement.
        var dir = Mathf.Sign(target - value);

        //Apply Movement.
        value += change * dir;

        //Check value hasn't exceeded target in direction.
        if (Mathf.Sign(target - value) != dir)
        {
            value = target;
        }

        return value;
    }

    /// <summary>
    /// Updates HighestMood with the new highest-value mood, triggering the OnMoodChanged event.
    /// </summary>
    private void UpdateHighestMood()
    {
        var highestMoodData = moodData[0];

        foreach (var data in moodData)
        {
            if (data.value > highestMoodData.value)
            {
                highestMoodData = data;
            }
        }

        if (HighestMood != highestMoodData.mood)
        {
            HighestMood = highestMoodData.mood;
            OnMoodChanged?.Invoke(HighestMood);
            if (debug) Debug.Log($"Highest mood changed to {HighestMood.MoodName}");
        }
    }

    /// <summary>
    /// Updates ui textbox with information useful to debugging.
    /// </summary>
    private void UpdateDebugText()
    {
        debugUi.text = "";
        for (int i = 0; i < moodData.Count; i++)
        {
            debugUi.text += $"MoodName: {moodData[i].mood.MoodName} MoodValue: {Mathf.FloorToInt(moodData[i].value)}\nMood Lower / Upper Limits: {moodData[i].mood.MoodLowerLimit}/{moodData[i].mood.MoodUpperLimit}\n\n";
        }

        debugUi.text += $"Current Mood: {HighestMood.MoodName}";
    }

    /// <summary>
    /// Updates a mood with the given name by the amount given, useful for other scripts to interact with this.
    /// </summary>
    /// <param name="amount"> The float amount to change the mood by.</param>
    /// <param name="name"> The name of the mood desired to change.</param>
    public void UpdateMood(float amount, string name)
    {
        var index = moodData.FindIndex(item => item.mood.name == name);
        moodData[index].value += amount;
        MaintainLimit(index);
    }

    /// <summary>
    /// Updates a mood with the given moodtype object by the amount given, useful for other scripts to interact with this.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="mt"></param>
    public void UpdateMood(float amount, MoodType mt)
    {
        var index = moodData.FindIndex(item => item.mood == mt);
        moodData[index].value += amount;
        MaintainLimit(index);
    }

    /// <summary>
    /// Returns the value of a given moodtype, useful for other scripts.
    /// </summary>
    /// <param name="mt"> A moodtype object.</param>
    /// <returns>The value of the mood.</returns>
    public float ReadMood(MoodType mt)
    {
        var index = moodData.FindIndex(item => item.mood == mt);
        return moodData[index].value;
    }

    /// <summary>
    /// Returns the value of a given mood by its ID, useful for other scripts.
    /// </summary>
    /// <param id="id"> The desired moodtype's ID</param>
    /// <returns>The value of the mood.</returns>
    public float ReadMood(int id)
    {
        return moodData[id].value;
    }

    /// <summary>
    /// Returns the value of a given mood by its name, useful for other scripts.
    /// </summary>
    /// <param name="name"> The desired moodtype</param>
    /// <returns>The value of the mood.</returns>
    public float ReadMood(string name)
    {
        var data = moodData.Find(item => item.mood.MoodName == name);
        return data.value;
    }
}
