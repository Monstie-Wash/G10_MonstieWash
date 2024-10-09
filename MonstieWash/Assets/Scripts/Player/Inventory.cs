using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    //Tags to use if sorting items is needed later in minigames/crafting;
    public enum ItemTags
    {
        Common,
        Rare,
        Mythical,
        Consumable        
    }

    [Serializable]
    //Converts Data from scriptable inventory objects into stored entries that maintain quantity of a certain item.
    public class InventoryEntry
    {
        [SerializeField] private InventoryItem itemData;
        [SerializeField] private int quantity;

        public InventoryItem ItemData { get { return itemData; }}
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public InventoryEntry(InventoryItem type, int quant)
        {
            itemData = type;
            quantity = quant;
        }
    }

    [SerializeField] private List<InventoryEntry> storedItemsList;
    public List<InventoryEntry> StoredItemsList { get { return storedItemsList;  } }

    //Used as a simple storage to pass the last score earned from the score UI manager to the upgrade manager so it can accurately reflect money earned.
    [SerializeField] private int lastEarnedScore;
    public int LastEarnedScore { get { return lastEarnedScore; } set { lastEarnedScore = value; } }

    [SerializeField] private bool showDebug;

    private void Awake()
    {
        //Ensure there is only one class of this existing and that it persists.
        Singletonize();

        //Create a new list for storing items if nothing has been assigned in unity inspector.
        if (storedItemsList == null) storedItemsList = new List<InventoryEntry>();
    }

    /// <summary>
    /// Takes a type of item and changes its value in storage by the given change amount.
    /// </summary>
    /// <param name="type">The type of item to change.</param> 
    /// <param name="change">The change to apply to that item can be pos or neg</param>
    public void UpdateStorageAmount(InventoryItem type, int change)
    {
        var index = RetrieveItemIndex(type);
        if (index != -1)
        {
            storedItemsList[index].Quantity = Mathf.Clamp(storedItemsList[index].Quantity + change, 0, type.MaxQuantity);
            if (storedItemsList[index].Quantity == 0) RemoveItem(type);
            //Update consumable ui when changing storage amount of them;
            if ((type is Consumable)) RefreshConsumableUI();
        }
        else Debug.LogWarning($"Consumable {type.ItemName} doesn't exist."); ;
    }

    /// <summary>
    /// Sets the amount of a certain item in storage to a given value.
    /// </summary>
    /// <param name="type"> Type of item to set storage of</param>
    /// <param name="amount">Amount to set item to</param>
    public void SetStorageAmount(InventoryItem type, int amount)
    {
        var index = RetrieveItemIndex(type);
        if (index != -1)
        {
            storedItemsList[index].Quantity = Mathf.Clamp(amount, 0, type.MaxQuantity);
            if (storedItemsList[index].Quantity == 0) RemoveItem(type);

            //Update consumable ui when changing storage amount of them;
            if ((type is Consumable)) RefreshConsumableUI();
        }
        else Debug.LogWarning($"Consumable {type.ItemName} doesn't exist."); ;
    }

    /// <summary>
    /// Adds a new item of the given type to storage.
    /// </summary>
    /// <param name="type">Type of item to add</param>
    /// <param name="amount">Amount starting in storage</param>
    public void AddNewItem(InventoryItem type, int amount)
    {
        var index = RetrieveItemIndex(type);
        if (index != -1)
        {
            Debug.LogWarning($"Item {type.ItemName} already exists.");
        }
        else
        {
            storedItemsList.Add(new InventoryEntry(type, amount));

            //Update consumable ui when changing storage amount of them;
            if ((type is Consumable)) RefreshConsumableUI();
        }

    }

    /// <summary>
    /// Removes a given item from storage.
    /// </summary>
    /// <param name="type">Type of item to remove</param>
    public void RemoveItem(InventoryItem type)
    {
        var itemIndex = RetrieveItemIndex(type);
        if (itemIndex != -1)
        {
            //Update consumable ui when changing storage amount of them;
            if ((type is Consumable)) RefreshConsumableUI();
            //Remove dictionary stored type.
            storedItemsList.RemoveAt(itemIndex);
        }
        else Debug.LogWarning($"Item {type.ItemName} doesn't exist.");
    }


    /// <summary>
    /// Takes a consumable and returns the index of it within this inventories list.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public int RetrieveItemIndex(InventoryItem item)
    {
        for (int i = 0; i < storedItemsList.Count; i++)
        {
            if (storedItemsList[i].ItemData.ItemName == item.ItemName)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Simple function for other scripts to read the quanity of a given item;
    /// </summary>
    /// <param name="item">Item type to check for in inventory</param>
    /// <returns></returns>
    public int ReadQuantity(InventoryItem item)
    {
        var itemIndex = RetrieveItemIndex(item);
        if (itemIndex == -1) return 0;
        else return storedItemsList[itemIndex].Quantity;

    }

    /// <summary>
    /// Simple function to check if inventory contains an item;
    /// </summary>
    /// <param name="item">Item type to check for in inventory</param>
    /// <returns></returns>
    public bool ContainsItem(InventoryItem item)
    {
        var itemIndex = RetrieveItemIndex(item);
        if (itemIndex == -1) return false;
        else return true;
    }

    /// <summary>
    /// Updates the Bag ui at the bottom of the screen when called;
    /// </summary>
    /// <param name="type"></param>
    public void RefreshConsumableUI()
    {
            ConsumablesManager cm;
            if (cm = FindFirstObjectByType<ConsumablesManager>())
            {
                if (showDebug) print("refreshing consumable ui from inv");
                cm.RefreshUI();
            }
    }

    /// <summary>
    /// Assigns this object to not be destroyed on load. If a new script is found will then remove it;
    /// </summary>
    private void Singletonize()
    {
        Inventory[] objs = FindObjectsByType<Inventory>(FindObjectsSortMode.None);

        //When class already exists don't create a new script;
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
    }
}
