using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBrain : MonoBehaviour
{
    [SerializeField] private List<MoodType> moodData; //Scriptable objects holding data about moods.

    [SerializeField] private Dictionary<int,float> activeMoods; //Current moods status. int refers to id and number of mood in list, float refers to current value of mood on its own scale.

    [SerializeField] public bool Debug;

    [SerializeField] public bool Pause;

    [HideInInspector] public Dictionary<int, float> ActiveMoods { get { return activeMoods; } }
    [HideInInspector] public List<MoodType> MoodData { get { return moodData; } }


    private void Awake()
    {
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
    }


    //Natural progression of all moods towards their resting point.
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

            if (Debug) print("Active Mood: " + moodData[i].name + "naturally changed to " + currentValue );
        }
    }

    //Random interference in moods based on chaos amount.
    private void chaoticInterference()
    {
        for (int i = 0; i < activeMoods.Count; i++)
        {           
            if (MoodData[i].ChaosMultiplier == 0) continue; //Skip chaos values of 0

            float currentValue = activeMoods[i];
            //Determine chaotic value
            float chaosVal = Random.Range(-moodData[i].ChaosMultiplier, moodData[i].ChaosMultiplier);

            //Changes current value by chaos value;
            activeMoods[i] = currentValue + (chaosVal * Time.deltaTime);

            if (Debug) print("Active Mood: " + moodData[i].name + "chaotically changed by " + chaosVal * Time.deltaTime);
        }
    }

    //Checks each mood for any positive chain reactions and then affects other moods listed.
    private void positiveChainReactions()
    {
        //Loop through active moods.
        for (int i = 0; i < activeMoods.Count; i++)
        {
            if (MoodData[i].PositiveReactionStrength == 0) continue; //Skip if reaction strenght is 0

            //Determine positive strength of current mood. (How far it is between its lower and upper limit)
            var percentageStrength = ((activeMoods[i] - moodData[i].MoodLowerLimit) * 100) / (moodData[i].MoodUpperLimit - moodData[i].MoodLowerLimit);
            float chainAmount = (percentageStrength / 100) * MoodData[i].PositiveReactionStrength * Time.deltaTime;

            //Loop through list of positive reactions in mood.
            for (int j = 0; j < moodData[i].PositiveChainReactions.Count; j++)
            {
                int targetMood = accessActiveMoodIndexByMoodType(moodData[i].PositiveChainReactions[j]);

                //Apply positive chain amount to each active mood that is in the list of positive reactions.
                activeMoods[targetMood] += chainAmount * Time.deltaTime;

                if (Debug) print("Active Mood: " + moodData[i].name + "positively influenced " + activeMoods[targetMood] + "by amount " + chainAmount * Time.deltaTime);
            }
        }
    }

    //Checks each mood for any negative chain reactions and then affects other moods listed.
    private void negativeChainReactions()
    {
        //Loop through active moods.
        for (int i = 0; i < activeMoods.Count; i++)
        {
            if (MoodData[i].NegativeReactionStrength == 0) continue; //Skip if reaction strenght is 0

            //Determine positive strength of current mood. (How far it is between its lower and upper limit)
            var percentageStrength = ((activeMoods[i] - moodData[i].MoodLowerLimit) * 100) / (moodData[i].MoodUpperLimit - moodData[i].MoodLowerLimit);
            float chainAmount = (percentageStrength / 100) * MoodData[i].NegativeReactionStrength;

            //Loop through list of negative reactions in mood.
            for (int j = 0; j < moodData[i].NegativeChainReactions.Count; j++)
            {
                int targetMood = accessActiveMoodIndexByMoodType(moodData[i].NegativeChainReactions[j]);

                //Apply negative chain amount to each active mood that is in the list of negative reactions.
                activeMoods[targetMood] -= chainAmount * Time.deltaTime;

                if (Debug) print("Active Mood: " + moodData[i].name + "negatively influenced " + activeMoods[targetMood] + "by amount " + chainAmount * Time.deltaTime);
            }
        }
    }

    //Keeps moods to their limits.
    private void maintainLimits()
    {
        for (int i = 0; i < activeMoods.Count; i++)
        {
            maintainLimit(i);
        }
    }

    private void maintainLimit(int moodInt)
    {
        //Check upper limit and set back if over.
        if (activeMoods[moodInt] > moodData[moodInt].MoodUpperLimit) activeMoods[moodInt] = moodData[moodInt].MoodUpperLimit;
        //Check lower limit and set back if under.
        if (activeMoods[moodInt] < moodData[moodInt].MoodLowerLimit) activeMoods[moodInt] = moodData[moodInt].MoodLowerLimit;
    }

    //Add data from mood scriptable objects to dictionary to store live values.
    private void loadMoods()
    {
        for(int i = 0; i < moodData.Count; i++)
        {
            activeMoods.Add(i, moodData[i].MoodStartingPoint);
        }
    }


    //Finds and returns active mood index by given string name.
    private int accessActiveMoodIndexByName(string name)
    {
        for (int i = 0; i < activeMoods.Count; i++)
        {
            if (name == MoodData[i].name) return i;
        }
        throw new System.Exception("No mood index found by that name");
    }

    //Finds and returns active mood index by given moodtype.
    private int accessActiveMoodIndexByMoodType(MoodType refMT)
    {
        for (int i = 0; i < activeMoods.Count; i++)
        {
            if (refMT.MoodName == MoodData[i].name) return i;
        }
        throw new System.Exception("No mood index found by that name");
    }


    //Functions for use by other scripts to influence the monsters mood. amount is how much to change mood by, and then referenced by either string name or moodtype object.
    public void updateMoodByName(float amount, string name)
    {
        var index = accessActiveMoodIndexByName(name);
        activeMoods[index] += amount;
        maintainLimit(index);
    }

    public void updateMoodByType(float amount, MoodType mt)
    {
        var index = accessActiveMoodIndexByMoodType(mt);
        activeMoods[index] += amount;
        maintainLimit(index);

    }

    //Moves a float value towards a target by a set amount of change and returns it.
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
