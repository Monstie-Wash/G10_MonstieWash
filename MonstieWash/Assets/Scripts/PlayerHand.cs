using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerhand : MonoBehaviour
{
    //Unity Inspector
    [SerializeField]
    [Range(1.0f, 50.0f)]
    private float cursorSpeed = 20f;

    //Private
    private Transform m_transform;
    private Vector3 m_handPosition;
    private float m_moveHorizontal;
    private float m_moveVertical;
	
    // Start is called before the first frame update
    void Start()
    {
        m_transform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        m_handPosition = Camera.main.WorldToScreenPoint(m_transform.position);
        m_moveHorizontal = Input.GetAxis("Horizontal");
        m_moveVertical = Input.GetAxis("Vertical");

        //Boundary Check X-axis
        if(m_handPosition.x > Screen.width && m_moveHorizontal > 0)
        {
            m_moveHorizontal = 0;
        }
        else if(m_handPosition.x < 0 && m_moveHorizontal < 0)
        {
            m_moveHorizontal = 0;
        }
        //Boundary Check Y-axis
        if(m_handPosition.y > Screen.height && m_moveVertical > 0)
        {
            m_moveVertical = 0;
        }
        else if(m_handPosition.y < 0 && m_moveVertical < 0)
        {
            m_moveVertical = 0;
        }
        
        m_transform.position = transform.position + new Vector3(m_moveHorizontal * cursorSpeed * Time.deltaTime, m_moveVertical * cursorSpeed * Time.deltaTime, 0);
    }
}
