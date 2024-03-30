using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHand : MonoBehaviour
{
    //Unity Inspector
    [SerializeField][Range(1.0f, 50.0f)] private float cursorSpeed = 20f;

    //Private
    private float m_moveHorizontal;
    private float m_moveVertical;
    private float m_moveThreshold = 0.01f;

    public bool IsMoving
    {
        get { return Mathf.Abs(m_moveHorizontal) > m_moveThreshold || Mathf.Abs(m_moveVertical) > m_moveThreshold; }
    }

    private void OnEnable()
    {
        InputManager.Inputs.OnMove += Inputs_MovePerformed;
        InputManager.Inputs.OnMove_Ended += Inputs_MoveEnded;
    }

    private void OnDisable()
    {
        InputManager.Inputs.OnMove -= Inputs_MovePerformed;
        InputManager.Inputs.OnMove_Ended -= Inputs_MoveEnded;
    }

    private void Update()
    {
        transform.position = CalculateMovement();
    }

    public void Inputs_MovePerformed(Vector2 movementInput)
    {
        m_moveHorizontal = movementInput.x;
        m_moveVertical = movementInput.y;
    }

    public void Inputs_MoveEnded()
    {
        m_moveHorizontal = 0f;
        m_moveVertical = 0f;
    }

    /// <summary>
    /// Calculates the movement of the hand, restricted to screen space.
    /// </summary>
    private Vector3 CalculateMovement()
    {
        var newPosition = transform.position + new Vector3(m_moveHorizontal * cursorSpeed * Time.deltaTime, m_moveVertical * cursorSpeed * Time.deltaTime, 0f);
        var cameraWidthInWorldUnits = Camera.main.orthographicSize * Camera.main.aspect;
        var cameraHeightInWorldUnits = Camera.main.orthographicSize;

        //Keep within screen bounds
        newPosition.x = Mathf.Clamp(newPosition.x, -cameraWidthInWorldUnits, cameraWidthInWorldUnits);
        newPosition.y = Mathf.Clamp(newPosition.y, -cameraHeightInWorldUnits, cameraHeightInWorldUnits);

        return newPosition;
    }
}
