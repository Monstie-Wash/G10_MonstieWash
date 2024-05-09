using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "MoodType", menuName = "ScriptableObjects/Consumables/Treat")]
public class Treat : Consumable
{
    [Tooltip("Which moods this treat will effect")] [SerializeField] private List<MoodType> moodTargets;
    [Tooltip("How much the affected moods will alter by, should match values 1-1 with values in moodtarget list")] [SerializeField] private List<float> moodEffects;

    [HideInInspector] public List<MoodEffect> moods;

    [Serializable]
    public struct MoodEffect
    {
        public MoodType target;
        public float effect;
    }
    /// <summary>
    /// Removes one of this consumable from storage and apply its effect to its targeted mood.
    /// </summary>
    public override void Consume()
    {
        //Find Monster Brain
        var brain = FindFirstObjectByType<MonsterBrain>();

        //Loop through mood targets and update them by the effect amount;
        for (var i = 0; i < moods.Count; i++) 
        {            
            brain.UpdateMood(moods[i].effect, moods[i].target);
        }

        //Remove one of this consumable type from the manager.
        FindFirstObjectByType<ConsumablesManager>().UpdateStorageAmount(this,-1);
    }
}
