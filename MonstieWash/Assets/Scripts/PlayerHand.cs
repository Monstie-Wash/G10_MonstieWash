using UnityEngine;
using UnityEngine.InputSystem;

public class Playerhand : MonoBehaviour
{
    //Unity Inspector
    [SerializeField]
    [Range(1.0f, 50.0f)]
    private float cursorSpeed = 20f;

    //Private
    private float m_moveHorizontal;
    private float m_moveVertical;

    private PlayerInput m_playerInput;

    private void Awake()
    {
        m_playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        m_playerInput.PlayerActions.Move.performed += Move_performed;
        m_playerInput.PlayerActions.Move.canceled += Move_canceled;
        m_playerInput.PlayerActions.Enable();
    }

    private void OnDisable()
    {
        m_playerInput.PlayerActions.Move.performed -= Move_performed;
        m_playerInput.PlayerActions.Move.canceled -= Move_canceled;
        m_playerInput.PlayerActions.Disable();
    }

    private void Move_performed(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();
        m_moveHorizontal = movementInput.x;
        m_moveVertical = movementInput.y;
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        m_moveHorizontal = 0f;
        m_moveVertical = 0f;
    }

    private void Update()
    {
        transform.position = CalculateMovement();
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
