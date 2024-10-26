using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] [Range(0,100)] private int MoveOffset;
    
    private Transform m_cursor;
    private Vector2 m_initialPos;

    // Start is called before the first frame update
    private void Start()
    {
        m_initialPos = transform.position;
        m_cursor = GameObject.FindFirstObjectByType<PlayerHand>().transform;
    }

    /// <summary>
    /// Parallax-ish effect. Background image follows hand position. Bigger MoveOffset, smaller amount of movement.
    /// </summary>
    private void Update()
    {
        Vector2 cursorPos = m_cursor.transform.position;

        transform.position = new Vector3(m_initialPos.x - (cursorPos.x / MoveOffset), m_initialPos.y - (cursorPos.y / MoveOffset), transform.position.z);
    }
}
