using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MoodType", menuName = "ScriptableObjects/Consumables/Treat")]

public class Treat : Consumable
{
    [Tooltip("Which moods this treat will effect")] [SerializeField] private List<MoodType> moodTargets;
    [Tooltip("How much the affected moods will alter by, should match values 1-1 with values in moodtarget list")] [SerializeField] private List<float> moodEffects;

    [HideInInspector] public List<MoodType> MoodTargets { get { return MoodTargets; } }
    [HideInInspector] public float MoodEffects { get { return MoodEffects; } }

    /// <summary>
    /// Removes one of this consumable from storage and apply its effect to its targeted mood.
    /// </summary>
    public override void Consume()
    {
        //Find Monster Brain
        var brain = FindFirstObjectByType<MonsterBrain>();

        //Loop through mood targets and update them by the effect amount;
        for (var i = 0; i < moodEffects.Count; i++) 
        {            
            brain.UpdateMood(moodEffects[i], moodTargets[i]);
        }

        //Remove one of this consumable type from the manager.
        FindFirstObjectByType<ConsumablesManager>().UpdateStorageAmount(this,-1);
    }
}
