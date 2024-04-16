using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] [Range(0,100)] public int MoveOffset;
    
    private GameObject m_Cursor;
    private Vector2 m_InitialPos;

    // Start is called before the first frame update
    private void Start()
    {
        m_InitialPos = transform.position;
        m_Cursor = GameObject.Find("PlayerHand");
    }

    /// <summary>
    /// Parallax-ish effect. Background image follows hand position. Bigger MoveOffset, smaller amount of movement.
    /// </summary>
    private void Update()
    {
        Vector2 cursorPos = m_Cursor.transform.position;

        transform.position = new Vector3(m_InitialPos.x - (cursorPos.x / MoveOffset), m_InitialPos.y - (cursorPos.y / MoveOffset), transform.position.z);
    }
}
