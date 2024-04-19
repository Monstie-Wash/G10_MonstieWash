using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Consumable : ScriptableObject
{
    [SerializeField] private string consumableName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private bool diminishingReturns;
    [SerializeField] private List<CraftingMaterial> craftingRequirements;
    [SerializeField] private List<int> craftingQuantities;
    [SerializeField] private int maxQuantity;
    [HideInInspector] public string ConsumableName { get { return consumableName; } }
    [HideInInspector] public Sprite Sprite { get { return sprite; } }
    [HideInInspector] public bool DiminishingReturns { get { return diminishingReturns; } }
    [HideInInspector] public List<CraftingMaterial> CraftingRequirements { get { return craftingRequirements; } }
    [HideInInspector] public List<int> CraftingQuantities { get { return craftingQuantities; } }
    [HideInInspector] public int MaxQuantity { get { return maxQuantity; } }

    public abstract void Consume();
    
}

