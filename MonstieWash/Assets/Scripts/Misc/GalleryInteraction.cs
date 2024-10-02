using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryInteraction : MonoBehaviour
{
    private float m_rotatingDirection;
    private bool holding = true;
    private Transform currentPolaroid;
    private float m_pickupDistance = 1.5f;
    private float m_rotation;
    private GalleryManager m_galleryManager;

    private void Start()
    {
        m_galleryManager = FindFirstObjectByType<GalleryManager>();
        currentPolaroid = transform.GetChild(3).transform;
    }

    private void OnEnable()
    {
        InputManager.Instance.OnSwitchTool += CycleOptions;
        InputManager.Instance.OnSwitchTool_Ended += ResetRotation;
        InputManager.Instance.OnActivate += Interact;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnSwitchTool -= CycleOptions;
        InputManager.Instance.OnSwitchTool_Ended -= ResetRotation;
        InputManager.Instance.OnActivate -= Interact;
    }

    private void Update()
    {
        //Rotate held object if desired.
        if (holding && m_rotatingDirection != 0)
        {
            var rotation = 100 * m_rotatingDirection * Time.deltaTime;
            m_rotation = Mathf.Clamp(m_rotation + rotation, -30f, 30f);
            currentPolaroid.rotation = Quaternion.Euler(0, 0, m_rotation);
        }
    }

    private void Interact()
    {
        if (holding)
        {
            if (m_galleryManager.Bounds.Contains(currentPolaroid.transform.position))
            {
                currentPolaroid.parent = m_galleryManager.transform;
                holding = false;
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, currentPolaroid.position) <= m_pickupDistance)
            {
                currentPolaroid.parent = transform;
                holding = true;
            }
        }
    }

    /// <summary>
    /// Takes an input direction from input manager and calls correct cycle method.
    /// </summary>
    /// <param name="dir">-1 for left 1 for right.</param>
    private void CycleOptions(int dir)
    {
        m_rotatingDirection = -dir;
    }

    /// <summary>
    /// Called by releasing rotation input.
    /// </summary>
    private void ResetRotation()
    {
        m_rotatingDirection = 0f;
    }
}
