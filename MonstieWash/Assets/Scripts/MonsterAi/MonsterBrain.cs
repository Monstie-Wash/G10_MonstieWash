using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterBrain : MonoBehaviour
{

    [Tooltip("Add all moodtype objects intended for this brain here.")] [SerializeField] private List<MoodType> moodData; //Scriptable objects holding data about moods.

    private Dictionary<int,float> activeMoods; //Current moods status. int refers to id and number of mood in list, float refers to current value of mood on its own scale.

    private Dictionary<string, int> activeMoodNames; // Current moods and their names. int refers to id and number of mood in list, string refers to name.

    [Tooltip("Updates the debug window when turned on")] [SerializeField] public bool Debug;


    [Tooltip("Pauses the brain when on.")][SerializeField] public bool Pause;

    private int designerSanityBuff; // A multiplier to reduce the tiny size of numbers used in setting up scriptable objects. Recommended set at 10.


    [Tooltip("Attach Text Mesh Pro Box here for displaying debug info")][SerializeField] public TextMeshProUGUI DebugUi;

    [HideInInspector] public Dictionary<int, float> ActiveMoods { get { return activeMoods; } }
    [HideInInspector] public List<MoodType> MoodData { get { return moodData; } }


    private void Awake()
    {
        designerSanityBuff = 10;
        activeMoods = new Dictionary<int, float>();
        activeMoodNames = new Dictionary<string, int>();

        loadMoods();
    }


    private void Update()
    {
        if (Pause) return;

        //Moods move by their natural change value to their resting point value.
        naturalChange();
        //Moods are affected by Chaos Multipliers.
        chaoticInterference();
        //Moods affect their positive chain reactions.
        positiveChainReactions();
        //Moods affect their negative chain reactions.
        negativeChainReactions();
        //Moods are kept to their upper and lower limits.
        maintainLimits();


        //Debug Updates
        if (Debug) updateDebugText();
    }


    /// <summary>
    /// Accesses the Moods resting point and natural rate of change, then moves mood towards its resting point by its rate of change.
    /// </summary>
    private void naturalChange()
    {
        for (int i = 0; i < activeMoods.Count; i++)
        {
            //Skip if doesn't naturally update.
            if (moodData[i].MoodNaturalChange == 0) continue;

            float currentValue = activeMoods[i];
            //Move currentvalue towards resting point by rate of change;
            currentValue = floatTowardsTarget(currentValue, moodData[i].MoodNaturalChange * Time.deltaTime , moodData[i].MoodRestingPoint);
            //Assign new value to active mood.
            activeMoods[i] = currentValue;

            if (Debug) print("Active Mood: " + moodData[i].MoodName + " naturally changed to " + currentValue );
        }
    }

    /// <summary>
    ///  Accesses the Moods chaos multiplier, randomly generates a number using the chaos multiplier as the upper and lower bound and then applies the random number to the moods value.
    /// </summary>
    private void chaoticInterference()
    {
        for (int i = 0; i < activeMoods.Count; i++)
        {           
            if (MoodData[i].ChaosMultiplier == 0) continue; //Skip chaos values of 0

            float currentValue = activeMoods[i];
            //Determine chaotic value
            float chaosVal = Random.Range(-moodData[i].ChaosMultiplier, moodData[i].ChaosMultiplier) * designerSanityBuff; //numbers are incredibly small *10 makes it more reasonable for designers.

            //Changes current value by chaos value;
            activeMoods[i] = currentValue + (chaosVal * Time.deltaTime);

            if (Debug) print("Active Mood: " + moodData[i].MoodName + " chaotically changed by " + chaosVal * Time.deltaTime);
        }
    }

    /// <summary>
    /// Accesses moods list of positive chain reaction moods, applies a positive addition to the listed moods based on the positive reaction strength and how far the 
    /// the current mood is inbetween its lower and upper bounds.
    /// </summary>
    private void positiveChainReactions()
    {
        //Loop through active moods.
        for (int i = 0; i < activeMoods.Count; i++)
        {
            if (MoodData[i].PositiveReactionStrength == 0) continue; //Skip if reaction strenght is 0
            
            //Determine positive strength of current mood. (How far it is between its lower and upper limit)
            var percentageStrength = ((activeMoods[i] - moodData[i].MoodLowerLimit) * 100) / (moodData[i].MoodUpperLimit - moodData[i].MoodLowerLimit);
            float chainAmount = (percentageStrength / 100) * MoodData[i].PositiveReactionStrength * designerSanityBuff; //numbers are incredibly small *10 makes it more reasonable for designers.

            //Loop through list of positive reactions in mood.
            for (int j = 0; j < moodData[i].PositiveChainReactions.Count; j++)
            {
                int targetMood = accessActiveMoodIndexByMoodType(moodData[i].PositiveChainReactions[j]);

                //Apply positive chain amount to each active mood that is in the list of positive reactions.
                activeMoods[targetMood] += chainAmount * Time.deltaTime;

                if (Debug) print("Active Mood: " + moodData[i].MoodName + " positively influenced " + moodData[targetMood].MoodName + "by amount " + chainAmount * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Accesses moodsl ist of negative chain reaction moods, applies a negative reduction to the listed moods based on the negative reaction strength
    /// and how far the current mood is inbetween its lower and upper bounds.
    /// </summary>
    private void negativeChainReactions()
    {
        //Loop through active moods.
        for (int i = 0; i < activeMoods.Count; i++)
        {
            if (MoodData[i].NegativeReactionStrength == 0) continue; //Skip if reaction strenght is 0

            //Determine positive strength of current mood. (How far it is between its lower and upper limit)
            var percentageStrength = ((activeMoods[i] - moodData[i].MoodLowerLimit) * 100) / (moodData[i].MoodUpperLimit - moodData[i].MoodLowerLimit);
            float chainAmount = (percentageStrength / 100) * MoodData[i].NegativeReactionStrength * designerSanityBuff; //numbers are incredibly small *10 makes it more reasonable for designers.

            //Loop through list of negative reactions in mood.
            for (int j = 0; j < moodData[i].NegativeChainReactions.Count; j++)
            {
                int targetMood = accessActiveMoodIndexByMoodType(moodData[i].NegativeChainReactions[j]);

                //Apply negative chain amount to each active mood that is in the list of negative reactions.
                activeMoods[targetMood] -= chainAmount * Time.deltaTime;

                if (Debug) print("Active Mood: " + moodData[i].MoodName + " negatively influenced " + moodData[targetMood].MoodName + " by amount " + chainAmount * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Calls other function maintain limit for each mood currently active.
    /// </summary>
    private void maintainLimits()
    {
        for (int i = 0; i < activeMoods.Count; i++)
        {
            maintainLimit(i);
        }
    }

    /// <summary>
    /// Checks if mood given by its current index in active list has gone over or under its limits and then corrects it back.
    /// </summary>
    /// <param name="moodInt"> Given index of desired mood in active moods.</param> 
    private void maintainLimit(int moodInt)
    {
        //Check upper limit and set back if over.
        if (activeMoods[moodInt] > moodData[moodInt].MoodUpperLimit) activeMoods[moodInt] = moodData[moodInt].MoodUpperLimit;
        //Check lower limit and set back if under.
        if (activeMoods[moodInt] < moodData[moodInt].MoodLowerLimit) activeMoods[moodInt] = moodData[moodInt].MoodLowerLimit;
    }

    /// <summary>
    /// Takes all moodtypes and stores them in activeMood dictionary, where first value is their index in moodtypes list and the second is their current value;
    /// </summary>
    private void loadMoods()
    {
        for(int i = 0; i < moodData.Count; i++)
        {
            activeMoods.Add(i, moodData[i].MoodStartingPoint);
            activeMoodNames.Add(moodData[i].MoodName,i);
        }
    }


    /// <summary>
    /// Updates ui textbox with information useful to debugging.
    /// </summary>
    private void updateDebugText()
    {
        DebugUi.text = "";
        for (int i = 0; i < activeMoods.Count; i++)
        {
            DebugUi.text += "MoodName: " + moodData[i].MoodName + " MoodValue: " + Mathf.FloorToInt(activeMoods[i]).ToString() + "\n" + 
            " Mood Lower/Upper Limits: " + moodData[i].MoodLowerLimit.ToString() + "/" + moodData[i].MoodUpperLimit.ToString();
            DebugUi.text += "\n";
            DebugUi.text += "\n";
        }

    }


    /// <summary>
    /// Returns the index of a moodtype within active moods by its string name.
    /// </summary>
    /// <param name="name"> The name of the desired mood index</param>
    /// <returns></returns>
    /// <exception cref="System.Exception"> When no mood exists with that name </exception>
    private int accessActiveMoodIndexByName(string name)
    {
        return activeMoodNames[name];
    }

    /// <summary>
    /// Returns the index of a moodtype within active moods by a scriptable object reference.
    /// </summary>
    /// <param name="name"> The name of the desired mood index</param>
    /// <returns></returns>
    /// <exception cref="System.Exception"> When no mood exists with that name </exception>
    private int accessActiveMoodIndexByMoodType(MoodType refMT)
    {
        return activeMoodNames[refMT.MoodName];
    }


    /// <summary>
    /// Updates a mood with the given name by the amount given, useful for other scripts to interact with this.
    /// </summary>
    /// <param name="amount"> The float amount to change the mood by.</param>
    /// <param name="name"> The name of the mood desired to change.</param>
    public void updateMoodByName(float amount, string name)
    {
        var index = accessActiveMoodIndexByName(name);
        activeMoods[index] += amount;
        maintainLimit(index);
    }

    /// <summary>
    /// Updates a mood with the given moodtype object by the amount given, useful for other scripts to interact with this.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="mt"></param>
    public void updateMoodByType(float amount, MoodType mt)
    {
        var index = accessActiveMoodIndexByMoodType(mt);
        activeMoods[index] += amount;
        maintainLimit(index);

    }

    /// <summary>
    /// Returns the value of a given moodtype, useful for other scripts.
    /// </summary>
    /// <param name="mt"> A moodtype object.</param>
    /// <returns></returns>
    public float readMood(MoodType mt)
    {
        var index = accessActiveMoodIndexByMoodType(mt);
        return activeMoods[index];
    }

    /// <summary>
    /// Returns the value of a given mood by its name, useful for other scripts.
    /// </summary>
    /// <param name="name"> The desired moodtype</param>
    /// <returns></returns>
    public float readMood(string name)
    {
        var index = accessActiveMoodIndexByName(name);
        return activeMoods[index];
    }

    /// <summary>
    /// Tool to move a float value towards a target by a certain amount whether negative or positive.
    /// </summary>
    /// <param name="value"> The value to move</param>
    /// <param name="change"> The amount to change the value by</param>
    /// <param name="target"> The target value to move towards</param>
    /// <returns></returns>
    private float floatTowardsTarget(float value, float change, float target)
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


}
