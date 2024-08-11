using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class Consumable : InventoryItem
{
    
    [Tooltip("If the consumable weakens effectiveness after multiple uses")] [SerializeField] private bool diminishingReturns; //Still needs implementation after confirming if designers want it.

    [HideInInspector] public string ConsumableName { get { return ItemName; } }
    [HideInInspector] public bool DiminishingReturns { get { return diminishingReturns; } }

    /// <summary>
    /// Each type of consumable will uniquely implement this based on what they should do. i.e treats simply remove one of themselves and change a mood.
    /// </summary>
    public abstract void Consume();
    
}

