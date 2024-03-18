using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerhand : MonoBehaviour
{
    //Unity Inspector
	[SerializeField]
    [Range(1.0f, 50.0f)]
	private float cursorSpeed = 20;

    //Private
    private Transform m_Transform;
    private Vector3 handPosition;
	private float moveHorizontal;
	private float moveVertical;
	
    // Start is called before the first frame update
    void Start()
    {
        m_Transform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        handPosition = Camera.main.WorldToScreenPoint(m_Transform.position);
        moveHorizontal = Input.GetAxis("Horizontal");
		moveVertical = Input.GetAxis("Vertical");

        //Boundary Check X-axis
        if(handPosition.x > Screen.width && moveHorizontal > 0)
        {
            moveHorizontal = 0;
        }
        else if(handPosition.x < 0 && moveHorizontal < 0)
        {
            moveHorizontal = 0;
        }
        //Boundary Check Y-axis
        if(handPosition.y > Screen.height && moveVertical > 0)
        {
            moveVertical = 0;
        }
        else if(handPosition.y < 0 && moveVertical < 0)
        {
            moveVertical = 0;
        }
		
		m_Transform.position = transform.position + new Vector3(moveHorizontal * cursorSpeed * Time.deltaTime, moveVertical * cursorSpeed * Time.deltaTime, 0);
    }
}
