using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] 
    private float m_pixelsPerUnit = 108f;
    private float m_snapValue;

    private void Awake()
    {
        Cursor.visible = false;

        m_snapValue = 1f / m_pixelsPerUnit;
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = m_snapValue * Mathf.Ceil(mousePos.x / m_snapValue);
        mousePos.y = m_snapValue * Mathf.Floor(mousePos.y / m_snapValue);
        mousePos.z = 0f;
        transform.position = mousePos;
    }
}
