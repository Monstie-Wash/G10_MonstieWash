using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[CreateAssetMenu(fileName = "InventoryItem", menuName = "ScriptableObjects/Inventory/InventoryItem")]
public class InventoryItem : ScriptableObject
{
    [Tooltip("Name of this item, should be unique")] [SerializeField] private string itemName;

    [Tooltip("Sprite to represent item in UI")] [SerializeField] private Sprite sprite;
    
    [Tooltip("Max amount of consumables that can be held in storage")] [SerializeField] private int maxQuantity;

    [Tooltip("Categories to tag item with for sorting")] [SerializeField] private List<Inventory.ItemTags> sortingTags;

    [HideInInspector] private List<CraftingRequirement> craftingRequirements; //Still needs implementation after confirming if designers want it.


    [HideInInspector] public string ItemName { get { return itemName; } }

    [HideInInspector] public Sprite Sprite { get { return sprite; } }
    [HideInInspector] public List<CraftingRequirement> CraftingRequirements { get { return craftingRequirements; } }
    [HideInInspector] public int MaxQuantity { get { return maxQuantity; } }

    [Serializable]
    public struct CraftingRequirement
    {
        [Tooltip("Craftingmaterial type needed to craft")] public CraftingMaterial material;
        [Tooltip("Amount needed")] public int quantity;
    }

    /// <summary>
    /// Checks whether this item contains a given tag
    /// </summary>
    /// <param name="tag">Tag to check for</param>
    /// <returns></returns>
    public bool ContainsTag(Inventory.ItemTags tag)
    {
        var returnVal = false;
        foreach (Inventory.ItemTags IT in sortingTags)
        {
            if (IT == tag) returnVal = true;
        }

        return returnVal;
    }
}
