using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MoodType", menuName = "ScriptableObjects/MoodType")]

public class MoodType : ScriptableObject
{

    [Tooltip("Name of Mood, used to reference it")] [SerializeField] private string moodName; //Used to reference.

    [Tooltip("Highest number mood can reach")][SerializeField] private float moodUpperLimit; //Highest point the mood can reach.

    [Tooltip("Lowest number mood can reach (Can be negative)")][SerializeField] private float moodLowerLimit; //Lowest point the mood can reach. 

    [Tooltip("What number the mood will start on when level starts")][SerializeField] private float moodStartingPoint; //Where the mood scale is placed upon game starting.

    [Tooltip("A number the mood will change by every second towards its resting point, Set to 0 if no natural change desired, Shouldn't be negative")][SerializeField] private float moodNaturalChange; //Constant force pushing mood towards resting point.

    [Tooltip("The resting point that the mood will naturally change towards by above number")][SerializeField] private float moodRestingPoint; //The point at which the mood will remain stationary.

    [Tooltip("Introduces random changes to the mood, recomended number 0-10 and can use decimals")][SerializeField] private float chaosMultiplier; //Random interference of mood level to create less certainty.

    [Tooltip("Drag other moodtypes here that the brain has that will be positively affected by this mood")][SerializeField] private List<MoodType> positiveChainReactions; //This mood will push other moods of this type towards their upper limit based on how close this mood itself is to its upper limit.

    [Tooltip("How strongly the positive effect is on other moods")][SerializeField] private float positiveReactionStrength; //How strong positive chain reactions will be.

    [Tooltip("Drag other moodtypes here that the brain has that will be negatively affected by this mood")][SerializeField] private List<MoodType> negativeChainReactions; //This mood will push other moods of this type towards their lower limit based on how close this mood itself is to its lower limit.

    [Tooltip("How strongly the negative effect is on other moods")][SerializeField] private float negativeReactionStrength; //How strong negative chain reactions will be.

    [Tooltip("An optional particle system that portrays the moood to the player")][SerializeField] private ParticleSystem moodParticle; // Particle system associated with the mood (e.g. hearts for happiness)

    [Tooltip("How much the mood changes by when a scene is completed")][SerializeField] private float sceneEffectOnMood; // The value by which the mood changes when the OnSceneCompleted event is called.

    [Tooltip("How many times the monster can flinch in this mood before attacking")][SerializeField] private int flinchCount;   // Number of flinches before the monster attacks (mood specific)

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
    [HideInInspector] public ParticleSystem MoodParticle { get { return moodParticle; } }
    [HideInInspector] public float SceneEffectOnMood { get { return sceneEffectOnMood; } }
    [HideInInspector] public int FlinchCount {  get { return flinchCount; } }

    #region Equality
    public override bool Equals(object other)
    {
        if (other is MoodType otherMoodType) return moodName.Equals(otherMoodType.MoodName);

        return false;
    }

    public override int GetHashCode()
    {
        return moodName.GetHashCode();
    }

    public static bool operator ==(MoodType lhs, MoodType rhs)
    {
        if (lhs is null || rhs is null) return false;

        return lhs.Equals(rhs);
    }

    public static bool operator !=(MoodType lhs, MoodType rhs)
    {
        return !(lhs == rhs);
    }
    #endregion
}
