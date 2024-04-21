using System;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private Transform heldItemsTransform;

    private ToolSwitcher m_toolSwitcher;
    private Collider2D m_collider;
    private ContactFilter2D m_contactFilter;
    private Item m_heldItem;
    private Transform m_itemInitialParent;

    private float m_initialRotation;
    private Vector3 m_prevPos = Vector3.zero;
    private Vector3 m_minimumDir = Vector3.zero;
    private bool m_lastWiggleUp = false;

    private bool m_holding { get { return m_heldItem != null; } }

    private void Awake()
    {
        m_toolSwitcher = GetComponent<ToolSwitcher>();
        m_collider = GetComponent<Collider2D>();
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

    private void LateUpdate()
    {
        if (!m_holding) return;

        if (m_heldItem.Stuck) SetHandPosition();
        else
        {
            m_heldItem.transform.position = transform.position;
        }
    }

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
            else
            {
                PickupItem();
            }
        }
        else
        {
            if (m_heldItem.Stuck)
            {
                LetGoItem();
            }
            else
            {
                DropItem();
            }
        }
    }

    /// <summary>
    /// Attempts to pick up an item.
    /// </summary>
    /// <returns>The picked up item or null if no item was found.</returns>
    private Item TryGrabItem()
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
    private Item GetClosestItem(Collider2D[] items)
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

        return closestObj.GetComponent<Item>();
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
        m_itemInitialParent = m_heldItem.transform.parent;
        m_heldItem.transform.parent = heldItemsTransform;
        m_heldItem.transform.localPosition = Vector3.zero; // snap to centre of hand
    }

    /// <summary>
    /// Drops the held item.
    /// </summary>
    private void DropItem()
    {
        m_heldItem.transform.parent = m_itemInitialParent;

        var handMovement = GetComponent<PlayerHand>().Velocity;
        var rotationalVelocity = UnityEngine.Random.Range(90f, 540f) * Mathf.Sign(handMovement.x);
        m_heldItem.Rb.velocity = handMovement;
        m_heldItem.Rb.angularVelocity = rotationalVelocity;

        m_heldItem = null;
    }

    private void SetupHoldingStuckItem()
    {
        m_initialRotation = m_heldItem.transform.rotation.eulerAngles.z;

        var theta = m_initialRotation - (m_heldItem.MaxRotation / 2f);
        var x = Mathf.Cos(theta * Mathf.Deg2Rad) * m_heldItem.GrabDistance;
        var y = Mathf.Sin(theta * Mathf.Deg2Rad) * m_heldItem.GrabDistance;

        m_minimumDir = new Vector3(x, y, 0).normalized;

        m_prevPos = Vector3.zero;
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
        var relativeOffset = m_initialRotation - (m_heldItem.MaxRotation / 2f);

        var overMaxAngle = relativeAngle > m_heldItem.MaxRotation;
        var underMinAngle = relativeAngle < 0f;

        var finalRelativeAngle = relativeAngle + relativeOffset;

        if (overMaxAngle || underMinAngle)
        {
            Wiggled(overMaxAngle);

            var baseTheta = overMaxAngle ? m_heldItem.MaxRotation : 0f;
            var relativeTheta = baseTheta + relativeOffset;
            var x = Mathf.Cos(relativeTheta * Mathf.Deg2Rad) * m_heldItem.GrabDistance;
            var y = Mathf.Sin(relativeTheta * Mathf.Deg2Rad) * m_heldItem.GrabDistance;

            dir = new Vector3(x, y, 0f).normalized;
            finalRelativeAngle = relativeTheta;
        }

        transform.position = m_heldItem.transform.position + dir * m_heldItem.GrabDistance;
        m_heldItem.transform.rotation = Quaternion.Euler(0f, 0f, finalRelativeAngle);

        m_prevPos = transform.position;
    }

    private void Wiggled(bool upwardsWiggle)
    {
        if (m_lastWiggleUp == upwardsWiggle) return;
        m_lastWiggleUp = upwardsWiggle;

        m_heldItem.Wiggle();
    }
}