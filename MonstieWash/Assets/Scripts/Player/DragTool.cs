using UnityEngine;

public class DragTool : MonoBehaviour
{
    [SerializeField] private bool alwaysCenterTool = false;

    private bool m_holding = false;
    private Collider2D m_collider;
    private ContactFilter2D m_contactFilter;
    private Transform toolTransform;

    private void Awake()
    {
        m_collider = GetComponent<Collider2D>();
        m_contactFilter = new ContactFilter2D();
        m_contactFilter.SetLayerMask(LayerMask.GetMask("Tool"));
    }
    /*
    private void OnEnable()
    {
        InputManager.Inputs.OnSwitchTool += Inputs_OnTransfer;
    }

    private void OnDisable()
    {
        InputManager.Inputs.OnSwitchTool -= Inputs_OnTransfer;
    }
    */

    private void Inputs_OnTransfer()
    {
        // pickup item
        if (!m_holding)
        {
            if (CollidingWithHand())
            {
                m_holding = true; // hold the item
                toolTransform.parent = transform;
                if (alwaysCenterTool) toolTransform.localPosition = Vector3.zero; // snap to centre of hand
                toolTransform.GetComponent<Eraser>().enabled = true;
            }
        }
        else // drop item
        {
            m_holding = false; // release the item
            toolTransform.parent = null;
            toolTransform.GetComponent<Eraser>().enabled = false;
        }
    }

    /// <summary>
    /// Check for a tool underneath the hand.
    /// </summary>
    /// <returns>Whether or not the hand is colliding with a tool.</returns>
    private bool CollidingWithHand() 
    {
        var results = new Collider2D[1];
        Physics2D.OverlapCollider(m_collider, m_contactFilter, results);

        if (results[0] == null) return false;

        toolTransform = results[0].transform;
        return true;
    }
}