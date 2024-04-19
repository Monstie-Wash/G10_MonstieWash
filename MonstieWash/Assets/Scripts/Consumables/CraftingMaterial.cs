using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoodType", menuName = "ScriptableObjects/CraftingMaterial")]

public class CraftingMaterial : ScriptableObject 
{
    [SerializeField] private string materialName;
    [SerializeField] private int maxQuantity;


    [HideInInspector] public string MaterialName { get { return materialName; } }
    [HideInInspector] public float MaxQuantity { get { return maxQuantity; } }
}
