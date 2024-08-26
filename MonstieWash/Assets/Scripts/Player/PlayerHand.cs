using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    //Unity Inspector
    [SerializeField, Range(1.0f, 50.0f)] private float handSpeed = 20f;
    [SerializeField, Range(0f, 5f)] private float extendBoundsX, extendBoundsY;
    [SerializeField] private bool showBounds = false;

    //Private
    private Vector2 m_movement;
    private float m_moveThreshold = 0.01f;
    private const float k_mouseSpeedMultiplier = 0.001f;

    private Collider2D m_collider;
    private ContactFilter2D m_contactFilter;

    public bool IsMoving
    {
        get { return m_movement.magnitude > m_moveThreshold; }
    }
    public Vector2 Velocity { get { return m_movement; } }

    private void OnEnable()
    {
        InputManager.Instance.OnMove += Inputs_MovePerformed;
        InputManager.Instance.OnMove_Ended += Inputs_MoveEnded;
        InputManager.Instance.OnActivate += Inputs_OnNavigate;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnMove -= Inputs_MovePerformed;
        InputManager.Instance.OnMove_Ended -= Inputs_MoveEnded;
        InputManager.Instance.OnActivate -= Inputs_OnNavigate;
    }

    private void Awake()
    {
        m_collider = GetComponent<Collider2D>();
        m_contactFilter = new ContactFilter2D();
        m_contactFilter.SetLayerMask(LayerMask.GetMask("Navigation"));
    }

    private void Update()
    {
        transform.position = CalculateMovement();
    }

    public void Inputs_MovePerformed(Vector2 movementInput)
    {
        m_movement = movementInput;
    }

    public void Inputs_MoveEnded()
    {
        m_movement = Vector2.zero;
    }

    private void Inputs_OnNavigate()
    {
        TryNavigate();
    }

    /// <summary>
    /// Calculates the movement of the hand, restricted to screen space.
    /// </summary>
    private Vector3 CalculateMovement()
    {
        var velocityModifer = 1f;

        switch (InputManager.Instance.InputDevice)
        {
            case InputManager.PlayerInputDevice.MKB:
                {
                    velocityModifer = handSpeed * k_mouseSpeedMultiplier;
                } break;
            case InputManager.PlayerInputDevice.Controller:
                {
                    velocityModifer = handSpeed * Time.deltaTime;
                } break;
        }

        var newVelocity = new Vector3(m_movement.x * velocityModifer, m_movement.y * velocityModifer, 0f);
        var newPosition = transform.position + newVelocity;
        var cameraWidthInWorldUnits = Camera.main.orthographicSize * Camera.main.aspect;
        var cameraHeightInWorldUnits = Camera.main.orthographicSize;

        //Keep within screen bounds
        newPosition.x = Mathf.Clamp(newPosition.x, -cameraWidthInWorldUnits - extendBoundsX, cameraWidthInWorldUnits + extendBoundsX);
        newPosition.y = Mathf.Clamp(newPosition.y, -cameraHeightInWorldUnits - extendBoundsY, cameraHeightInWorldUnits + extendBoundsY);

        return newPosition;
    }

    private void TryNavigate()
    {
        var results = new Collider2D[1];
        Physics2D.OverlapCollider(m_collider, m_contactFilter, results);

        if (results[0] == null) return;
        
        //Navigate
        INavigator navArrow = results[0].GetComponent<INavigator>();
        navArrow.OnClicked();
    }

    private void OnDrawGizmos()
    {
        if (!showBounds) return;

        var cameraWidthInWorldUnits = 5f * 16f / 9f;
        var cameraHeightInWorldUnits = 5f;

        var viewTopLeft = new Vector2(-cameraWidthInWorldUnits, cameraHeightInWorldUnits);
        var viewTopRight = new Vector2(cameraWidthInWorldUnits, cameraHeightInWorldUnits);
        var viewBottomLeft = new Vector2(-cameraWidthInWorldUnits, -cameraHeightInWorldUnits);
        var viewBottomRight = new Vector2(cameraWidthInWorldUnits, -cameraHeightInWorldUnits);

        Debug.DrawLine(viewTopLeft, viewTopRight, Color.white);
        Debug.DrawLine(viewTopRight, viewBottomRight, Color.white);
        Debug.DrawLine(viewBottomRight, viewBottomLeft, Color.white);
        Debug.DrawLine(viewBottomLeft, viewTopLeft, Color.white);

        var boundsTopLeft = new Vector2(-cameraWidthInWorldUnits - extendBoundsX, cameraHeightInWorldUnits + extendBoundsY);
        var boundsTopRight = new Vector2(cameraWidthInWorldUnits + extendBoundsX, cameraHeightInWorldUnits + extendBoundsY);
        var boundsBottomLeft = new Vector2(-cameraWidthInWorldUnits - extendBoundsX, -cameraHeightInWorldUnits - extendBoundsY);
        var boundsBottomRight = new Vector2(cameraWidthInWorldUnits + extendBoundsX, -cameraHeightInWorldUnits - extendBoundsY);

        Debug.DrawLine(boundsTopLeft, boundsTopRight, Color.green);
        Debug.DrawLine(boundsTopRight, boundsBottomRight, Color.green);
        Debug.DrawLine(boundsBottomRight, boundsBottomLeft, Color.green);
        Debug.DrawLine(boundsBottomLeft, boundsTopLeft, Color.green);
    }
}
