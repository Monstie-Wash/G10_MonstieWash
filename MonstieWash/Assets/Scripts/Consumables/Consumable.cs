using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class Consumable : ScriptableObject
{
    [Tooltip("Name of this consumable, should be unique")] [SerializeField] private string consumableName;
    [Tooltip("Sprite to represent consumable in UI")] [SerializeField] private Sprite sprite;
    [Tooltip("If the consumable weakens effectiveness after multiple uses")] [SerializeField] private bool diminishingReturns; //Still needs implementation after confirming if designers want it.
    [SerializeField] private List<CraftingRequirement> craftingRequirements; //Still needs implementation after confirming if designers want it.
    [Tooltip("Max amount of consumables that can be held in storage")] [SerializeField] private int maxQuantity;
    [HideInInspector] public string ConsumableName { get { return consumableName; } }
    [HideInInspector] public Sprite Sprite { get { return sprite; } }
    [HideInInspector] public bool DiminishingReturns { get { return diminishingReturns; } }
    [HideInInspector] public List<CraftingRequirement> CraftingRequirements { get { return craftingRequirements; } }
    [HideInInspector] public int MaxQuantity { get { return maxQuantity; } }


    [Serializable]
    public struct CraftingRequirement
    {
        [Tooltip("Craftingmaterial type needed to craft")] public CraftingMaterial material;
        [Tooltip("Amount needed")] public int quantity;
    }


    /// <summary>
    /// Each type of consumable will uniquely implement this based on what they should do. i.e treats simply remove one of themselves and change a mood.
    /// </summary>
    public abstract void Consume();
    
}

