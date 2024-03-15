using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerhand : MonoBehaviour
{
	[SerializeField]
	private float speed = 20;
	private float moveHorizontal;
	private float moveVertical;
	private Transform m_Transform;
    // Start is called before the first frame update
    void Start()
    {
        m_Transform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
		moveVertical = Input.GetAxis("Vertical");
		
		m_Transform.position = transform.position + new Vector3(moveHorizontal * speed * Time.deltaTime, moveVertical * speed * Time.deltaTime, 0);
    }
}
