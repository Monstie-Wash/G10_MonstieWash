using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerhand : MonoBehaviour
{
    //Public
    public Vector3 handPosition { get; private set; }

    //Unity Inspector
    [SerializeField]
    [Range(1.0f, 50.0f)]
    private float cursorSpeed = 20f;

    //Private
    private float m_moveHorizontal;
    private float m_moveVertical;

    // Update is called once per frame
    private void Update()
    {
        m_moveHorizontal = Input.GetAxis("Horizontal");
        m_moveVertical = Input.GetAxis("Vertical");

        transform.position = CalculateMovement();
        handPosition = Camera.main.WorldToScreenPoint(transform.position);
    }

    /// <summary>
    /// Calculates the movement of the hand, restricted to screen space.
    /// </summary>
    private Vector3 CalculateMovement()
    {
        var newPosition = transform.position + new Vector3(m_moveHorizontal * cursorSpeed * Time.deltaTime, m_moveVertical * cursorSpeed * Time.deltaTime, 0f);
        var aspectRatio = 16f / 9f;
        var cameraWidthInWorldUnits = Camera.main.orthographicSize * aspectRatio;
        var cameraHeightInWorldUnits = Camera.main.orthographicSize;

        //Keep within screen bounds
        newPosition.x = Mathf.Clamp(newPosition.x, -cameraWidthInWorldUnits, cameraWidthInWorldUnits);
        newPosition.y = Mathf.Clamp(newPosition.y, -cameraHeightInWorldUnits, cameraHeightInWorldUnits);

        return newPosition;
    }
}
