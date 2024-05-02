using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoodType", menuName = "ScriptableObjects/CraftingMaterial")]

public class CraftingMaterial : ScriptableObject 
{
    //Don't worry about this script for now, placeholder so my consumables will be easy to become craftable later if the designers do want to add crafting.
    [SerializeField] private string materialName;
    [SerializeField] private int maxQuantity;

    [HideInInspector] public string MaterialName { get { return materialName; } }
    [HideInInspector] public float MaxQuantity { get { return maxQuantity; } }
}
