using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MoodType", menuName = "ScriptableObjects/MoodType")]

public class MoodType : ScriptableObject
{

    [SerializeField] private string moodName; //Used to reference.

    [SerializeField] private float moodUpperLimit; //Highest point the mood can reach.

    [SerializeField] private float moodLowerLimit; //Lowest point the mood can reach. 

    [SerializeField] private float moodStartingPoint; //Where the mood scale is placed upon game starting.

    [SerializeField] private float moodNaturalChange; //Constant force pushing mood towards resting point.

    [SerializeField] private float moodRestingPoint; //The point at which the mood will remain stationary.

    [SerializeField] private float chaosMultiplier; //Random interference of mood level to create less certainty.

    [SerializeField] private List<MoodType> positiveChainReactions; //This mood will push other moods of this type towards their upper limit based on how close this mood itself is to its upper limit.

    [SerializeField] private float positiveReactionStrength; //How strong positive chain reactions will be.

    [SerializeField] private List<MoodType> negativeChainReactions; //This mood will push other moods of this type towards their lower limit based on how close this mood itself is to its lower limit.

    [SerializeField] private float negativeReactionStrength; //How strong negative chain reactions will be.

    [HideInInspector] public string MoodName { get { return moodName; } }
    [HideInInspector] public float MoodUpperLimit { get { return moodUpperLimit; } }
    [HideInInspector] public float MoodLowerLimit { get { return moodLowerLimit; } }
    [HideInInspector] public float MoodStartingPoint { get { return moodStartingPoint; } }
    [HideInInspector] public float MoodNaturalChange { get { return moodNaturalChange; } }
    [HideInInspector] public float MoodRestingPoint { get { return moodRestingPoint; } }
    [HideInInspector] public float ChaosMultiplier { get { return chaosMultiplier; } }
    [HideInInspector] public List<MoodType> PositiveChainReactions { get { return positiveChainReactions; } }
    [HideInInspector] public float PositiveReactionStrength { get { return positiveReactionStrength; } }
    [HideInInspector] public List<MoodType> NegativeChainReactions { get { return negativeChainReactions; } }
    [HideInInspector] public float NegativeReactionStrength { get { return negativeReactionStrength; } }

}
