using System;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private Transform heldItemsTransform;
    [SerializeField] private float throwSpeedMultiplier = 10f;

    private ToolSwitcher m_toolSwitcher;
    private Collider2D m_handCollider;
    private ContactFilter2D m_contactFilter;
    private StuckItem m_heldItem;

    private Vector3 m_prevPos = Vector3.zero;
    private Vector3 m_minimumDir;
    private bool m_lastWiggleUp = false;

    private bool m_holding { get { return m_heldItem != null; } }

    private void Awake()
    {
        m_toolSwitcher = GetComponent<ToolSwitcher>();
        m_handCollider = GetComponent<Collider2D>();
        m_contactFilter = new ContactFilter2D();
        m_contactFilter.SetLayerMask(LayerMask.GetMask("Pickupable"));
    }

    private void OnEnable()
    {
        InputManager.Instance.OnActivate_Held += Inputs_OnActivate_Held;
        InputManager.Instance.OnSwitchTool += Inputs_OnSwitchTool;
        InputManager.Instance.OnActivate_Ended += Inputs_OnActivate_Ended;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnActivate_Held -= Inputs_OnActivate_Held;
        InputManager.Instance.OnSwitchTool -= Inputs_OnSwitchTool;
        InputManager.Instance.OnActivate_Ended -= Inputs_OnActivate_Ended;
    }

    private void Inputs_OnActivate_Held()
    {
        if (m_toolSwitcher.CurrentToolIndex == -1) //Empty hand
        {
            Interact();
        }
    }

    private void Inputs_OnSwitchTool(int dirInput)
    {
        if (dirInput == 0) return;

        if (m_holding)
        {
            if (m_heldItem.Stuck) LetGoItem();
            else DropItem();
        }
    }

    private void Inputs_OnActivate_Ended()
    {
        if (m_holding)
        {
            if (m_heldItem.Stuck) LetGoItem();
            else DropItem();
        }
    }

    private void LateUpdate()
    {
        if (!m_holding) return;

        if (m_heldItem.Stuck) SetHandPosition();
        else m_heldItem.transform.position = transform.position;
    }

    /// <summary>
    /// Apply the context-relevant action to interact with an item.
    /// </summary>
    private void Interact()
    {
        if (!m_holding)
        {
            var item = TryGrabItem();
            if (item == null) return;
            m_heldItem = item;

            if (m_heldItem.Stuck)
            {
                SetupHoldingStuckItem();                
                SetHandPosition();
            }
            else if (!m_heldItem.Stuck)
            {
                LetGoItem();
            }

            else PickupItem();
        }
        else
        {
            if (m_heldItem.Stuck) LetGoItem();
            else DropItem();
        }
    }

    /// <summary>
    /// Attempts to pick up an item.
    /// </summary>
    /// <returns>The picked up item or null if no item was found.</returns>
    private StuckItem TryGrabItem()
    {
        var results = new Collider2D[20];
        Physics2D.OverlapCollider(m_handCollider, m_contactFilter, results);

        if (results[0] == null) return null;

        return GetClosestItem(results);
    }

    /// <summary>
    /// Finds the item closest to the hand origin from the list of items.
    /// </summary>
    /// <param name="items">The list of items to check.</param>
    /// <returns>The closest item.</returns>
    private StuckItem GetClosestItem(Collider2D[] items)
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

        return closestObj.GetComponent<StuckItem>();
    }

    /// <summary>
    /// Lets go of a held item.
    /// </summary>
    private void LetGoItem()
    {
        m_heldItem = null;
    }

    /// <summary>
    /// Picks up the given item.
    /// </summary>
    /// <param name="item">The item to pick up.</param>
    private void PickupItem()
    {
        m_heldItem.transform.parent = heldItemsTransform;
        m_heldItem.transform.localPosition = Vector3.zero; // snap to centre of hand
        m_heldItem.Rb.angularVelocity = 0f;
    }

    /// <summary>
    /// Drops the held item.
    /// </summary>
    private void DropItem()
    {
        m_heldItem.transform.parent = m_heldItem.InitialParent;

        var handMovement = GetComponent<PlayerHand>().Velocity.normalized;
        handMovement *= throwSpeedMultiplier;

        var rotationalVelocity = UnityEngine.Random.Range(90f, 540f) * Mathf.Sign(handMovement.x);
        m_heldItem.Rb.velocity = handMovement;
        m_heldItem.Rb.angularVelocity = rotationalVelocity;

        m_heldItem = null;
    }

    /// <summary>
    /// Sets up the appropriate variables for wiggling.
    /// </summary>
    private void SetupHoldingStuckItem()
    {
        SetMinimumDir();

        m_prevPos = transform.position;
    }

    /// <summary>
    /// Sets the hands position to the point on the holding circle dictated by the held item.
    /// </summary>
    private void SetHandPosition()
    {
        if (!m_holding) return;

        var movement = transform.position - m_prevPos;
        var dir = (transform.position + movement - m_heldItem.transform.position).normalized;

        var relativeAngle = Vector3.SignedAngle(m_minimumDir, dir, Vector3.forward);
        var relativeOffset = m_heldItem.InitialRotation - (m_heldItem.MaxRotation / 2f);

        var overMaxAngle = relativeAngle > m_heldItem.MaxRotation;
        var underMinAngle = relativeAngle < 0f;

        var finalRelativeAngle = relativeAngle + relativeOffset;

        if (overMaxAngle || underMinAngle)
        {
            Wiggled(overMaxAngle);

            var baseTheta = overMaxAngle ? m_heldItem.MaxRotation : 0f;
            var relativeTheta = baseTheta + relativeOffset;
            var x = Mathf.Cos(relativeTheta * Mathf.Deg2Rad);
            var y = Mathf.Sin(relativeTheta * Mathf.Deg2Rad);

            dir = new Vector3(x, y, 0f).normalized;
            finalRelativeAngle = relativeTheta;
        }

        transform.position = m_heldItem.transform.position + dir * m_heldItem.GrabDistance;
        m_heldItem.transform.rotation = Quaternion.Euler(0f, 0f, finalRelativeAngle);

        m_prevPos = transform.position;
    }

    /// <summary>
    /// Applies a wiggle to the held item if it is in the opposite direction to the previous wiggle call.
    /// </summary>
    /// <param name="upwardsWiggle">Whether the wiggle was in the upwards direction.</param>
    private void Wiggled(bool upwardsWiggle)
    {
        if (m_lastWiggleUp == upwardsWiggle) return;
        m_lastWiggleUp = upwardsWiggle;

        m_heldItem.Wiggle();

        SetMinimumDir();
    }

    /// <summary>
    /// Sets the m_minimumDir field to the appropriate value based on the held item's max rotation.
    /// </summary>
    private void SetMinimumDir()
    {
        var theta = m_heldItem.InitialRotation - (m_heldItem.MaxRotation / 2f);
        var x = Mathf.Cos(theta * Mathf.Deg2Rad);
        var y = Mathf.Sin(theta * Mathf.Deg2Rad);

        m_minimumDir = new Vector3(x, y, 0).normalized;
    }
}