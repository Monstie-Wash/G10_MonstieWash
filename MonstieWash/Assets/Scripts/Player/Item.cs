using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Item : MonoBehaviour
{
    [SerializeField] private bool startStuck = true;
    [SerializeField] private float maxRotation = 90f;
    [SerializeField] private float grabDistanceMultiplier = 1.5f;
    [SerializeField] private int wiggleCount = 6;

    private Rigidbody2D m_rb;

    public bool Stuck { get; private set; }
    public float GrabDistance { get; private set; }
    public float MaxRotation { get { return maxRotation; } }
    public Rigidbody2D Rb { get { return m_rb; } }
    public int WiggleCount { get {  return wiggleCount; } }


    private void Awake()
    {
        Stuck = startStuck;
        m_rb = GetComponent<Rigidbody2D>();
        GrabDistance = grabDistanceMultiplier * transform.lossyScale.x;
    }

    public void SetStuck(bool stuck)
    {
        Stuck = stuck;

        if (Stuck)
        {
            m_rb.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            m_rb.bodyType = RigidbodyType2D.Dynamic;
            var spriteTransform = transform.GetChild(0);
            spriteTransform.localPosition *= -1f;
        }
    }

    /// <summary>
    /// Applies a wiggle.
    /// </summary>
    /// <returns>Whether this wiggle unstuck the item.</returns>
    public bool Wiggle()
    {
        wiggleCount--;

        if (wiggleCount <= 0)
        {
            SetStuck(false);
            return true;
        }

        return false;
    }
}
