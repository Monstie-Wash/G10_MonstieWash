using System;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private Transform heldItemsTransform;

    private ToolSwitcher m_toolSwitcher;
    private bool m_holding = false;
    private Collider2D m_collider;
    private ContactFilter2D m_contactFilter;
    private Transform m_heldItem;
    private Transform m_itemInitialParent;
    private PlayerHand m_playerHand;

    bool stuck = true;

    private void Awake()
    {
        m_toolSwitcher = GetComponent<ToolSwitcher>();
        m_collider = GetComponent<Collider2D>();
        m_playerHand = GetComponent<PlayerHand>();
        m_contactFilter = new ContactFilter2D();
        m_contactFilter.SetLayerMask(LayerMask.GetMask("Pickupable"));
    }

    private void OnEnable()
    {
        InputManager.Inputs.OnActivate += Inputs_OnActivate;
        InputManager.Inputs.OnSwitchTool += Inputs_OnSwitchTool;
        InputManager.Inputs.OnNavigate += Inputs_OnNavigate;
    }

    private void OnDisable()
    {
        InputManager.Inputs.OnActivate -= Inputs_OnActivate;
        InputManager.Inputs.OnSwitchTool -= Inputs_OnSwitchTool;
        InputManager.Inputs.OnNavigate -= Inputs_OnNavigate;
    }

    private void Inputs_OnActivate()
    {
        if (m_toolSwitcher.CurrentToolIndex == -1) //Empty hand
        {
            Interact();
        }
    }

    private void Inputs_OnSwitchTool(float dirInput)
    {
        var dir = Math.Sign(dirInput);
        if (dir == 0) return;

        if (m_holding) DropItem();
    }

    private void Inputs_OnNavigate()
    {
        if (m_holding) DropItem();
    }

    private void Interact()
    {
        if (!m_holding)
        {
            var item = TryPickup();
            if (item != null)
            {
                m_heldItem = item;
                m_holding = true;
                if (stuck)
                {
                    m_playerHand.SetMoveable(m_playerHand.IsStuck);
                }
                else
                {
                    TransferItem();
                }
            }
        }
        else
        {
            if (stuck)
            {

            }
            else
            {
                TransferItem();
            }
        }
    }

    private void TransferItem()
    {
        if (!m_holding) PickupItem();
        else DropItem();
    }

    /// <summary>
    /// Attempts to pick up an item.
    /// </summary>
    /// <returns>The picked up item or null if no item was found.</returns>
    private Transform TryPickup()
    {
        var results = new Collider2D[20];
        Physics2D.OverlapCollider(m_collider, m_contactFilter, results);

        if (results[0] == null) return null;

        return GetClosestItem(results);
    }

    /// <summary>
    /// Finds the item closest to the hand origin from the list of items.
    /// </summary>
    /// <param name="items">The list of items to check.</param>
    /// <returns>The closest item.</returns>
    private Transform GetClosestItem(Collider2D[] items)
    {
        var closestObj = items[0];
        var closestObjDistFromCentre = Vector3.Distance(transform.position, closestObj.transform.position);

        foreach (var obj in items)
        {
            if (obj == null) continue;

            var distFromCentre = Vector3.Distance(transform.position, obj.transform.position);
            if (distFromCentre < closestObjDistFromCentre)
            {
                closestObj = obj;
                closestObjDistFromCentre = Vector3.Distance(transform.position, closestObj.transform.position);
            }
        }

        return closestObj.transform;
    }

    /// <summary>
    /// Picks up the given item.
    /// </summary>
    /// <param name="item">The item to pick up.</param>
    private void PickupItem()
    {
        m_holding = true; // hold the item
        m_itemInitialParent = m_heldItem.parent;
        m_heldItem.parent = heldItemsTransform;
        m_heldItem.localPosition = Vector3.zero; // snap to centre of hand
    }

    /// <summary>
    /// Drops the held item.
    /// </summary>
    private void DropItem()
    {
        m_holding = false; // release the item
        m_heldItem.parent = m_itemInitialParent;
        m_heldItem = null;
    }
}