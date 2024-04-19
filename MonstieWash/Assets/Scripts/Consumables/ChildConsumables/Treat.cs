using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MoodType", menuName = "ScriptableObjects/Consumables/Treat")]

public class Treat : Consumable
{
   [SerializeField] private List<MoodType> moodTargets;
   [SerializeField] private List<float> moodEffects;


    [HideInInspector] public List<MoodType> MoodTargets { get { return MoodTargets; } }
    [HideInInspector] public float MoodEffects { get { return MoodEffects; } }

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
