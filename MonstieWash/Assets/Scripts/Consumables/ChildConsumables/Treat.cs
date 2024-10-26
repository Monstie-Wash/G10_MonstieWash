using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "MoodType", menuName = "ScriptableObjects/Consumables/Treat")]
public class Treat : Consumable
{
    [SerializeField] private List<MoodEffect> moodEffects;
    [SerializeField] private List<MoodType> moodFXOnUse;
    [SerializeField] private bool showDebug;

    public static event Action UseTreat;

    [Serializable]
    public struct MoodEffect
    {
        [Tooltip("Which moods this treat will effect")] public MoodType target;
        [Tooltip("How much the affected moods will alter by, should match values 1-1 with values in moodtarget list")] public float effect;
    }

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
            if (!brain.Moods.Contains(moodEffects[i].target)) continue;

            brain.UpdateMood(moodEffects[i].effect, moodEffects[i].target);
            if (showDebug) Debug.Log("Updating: " + moodEffects[i].target + " by " + moodEffects[i].effect);

            UseTreat?.Invoke();
        }

        //Remove one of this consumable type from the manager.
        FindFirstObjectByType<Inventory>().UpdateStorageAmount(this,-1);

        //Play mood FX
        var moodFXManager = FindFirstObjectByType<MoodFXManager>();

        foreach (var mood in moodFXOnUse)
        {
            ParticleSystem ps;
            if (moodFXManager.MoodParticleSystems.TryGetValue(mood, out ps)) ps.Play();
        }
    }
}
