using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Consumable : ScriptableObject
{
    [Tooltip("Name of this consumable, should be unique")] [SerializeField] private string consumableName;
    [Tooltip("Sprite to represent consumable in UI")] [SerializeField] private Sprite sprite;
    [Tooltip("If the consumable weakens effectiveness after multiple uses")] [SerializeField] private bool diminishingReturns; //Still needs implementation after confirming if designers want it.
    [Tooltip("List of craftingmaterial types needed to craft")] [SerializeField] private List<CraftingMaterial> craftingRequirements; //Still needs implementation after confirming if designers want it.
    [Tooltip("List of quantities 1-1 of each crafting material")] [SerializeField] private List<int> craftingQuantities; //Still needs implementation after confirming if designers want it.
    [Tooltip("Max amount of consumables that can be held in storage")] [SerializeField] private int maxQuantity;
    [HideInInspector] public string ConsumableName { get { return consumableName; } }
    [HideInInspector] public Sprite Sprite { get { return sprite; } }
    [HideInInspector] public bool DiminishingReturns { get { return diminishingReturns; } }
    [HideInInspector] public List<CraftingMaterial> CraftingRequirements { get { return craftingRequirements; } }
    [HideInInspector] public List<int> CraftingQuantities { get { return craftingQuantities; } }
    [HideInInspector] public int MaxQuantity { get { return maxQuantity; } }

    /// <summary>
    /// Each type of consumable will uniquely implement this based on what they should do. i.e treats simply remove one of themselves and change a mood.
    /// </summary>
    public abstract void Consume();
    
}

