using UnityEngine;
using System.Collections;

public class PlayerHand : MonoBehaviour
{
    //Unity Inspector
    [SerializeField][Range(1.0f, 50.0f)] private float cursorSpeed = 20f;

    //Private
    private float m_moveHorizontal;
    private float m_moveVertical;
    private float m_moveThreshold = 0.01f;
    private const float k_mouseSpeedMultiplier = 0.001f;

    private Collider2D m_collider;
    private ContactFilter2D m_contactFilter;

    public bool IsMoving
    {
        get { return Mathf.Abs(m_moveHorizontal) > m_moveThreshold || Mathf.Abs(m_moveVertical) > m_moveThreshold; }
    }

    private void OnEnable()
    {
        InputManager.Inputs.OnMove += Inputs_MovePerformed;
        InputManager.Inputs.OnMove_Ended += Inputs_MoveEnded;
        InputManager.Inputs.OnNavigate += Inputs_OnNavigate;
    }

    private void OnDisable()
    {
        InputManager.Inputs.OnMove -= Inputs_MovePerformed;
        InputManager.Inputs.OnMove_Ended -= Inputs_MoveEnded;
        InputManager.Inputs.OnNavigate -= Inputs_OnNavigate;
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
        m_moveHorizontal = movementInput.x;
        m_moveVertical = movementInput.y;
    }

    public void Inputs_MoveEnded()
    {
        m_moveHorizontal = 0f;
        m_moveVertical = 0f;
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

        switch (InputManager.Inputs.InputDevice)
        {
            case InputManager.PlayerInputDevice.MKB:
                {
                    velocityModifer = cursorSpeed * k_mouseSpeedMultiplier;
                } break;
            case InputManager.PlayerInputDevice.Controller:
                {
                    velocityModifer = cursorSpeed * Time.deltaTime;
                } break;
        }

        var newVelocity = new Vector3(m_moveHorizontal * velocityModifer, m_moveVertical * velocityModifer, 0f);
        var newPosition = transform.position + newVelocity;
        var cameraWidthInWorldUnits = Camera.main.orthographicSize * Camera.main.aspect;
        var cameraHeightInWorldUnits = Camera.main.orthographicSize;

        //Keep within screen bounds
        newPosition.x = Mathf.Clamp(newPosition.x, -cameraWidthInWorldUnits, cameraWidthInWorldUnits);
        newPosition.y = Mathf.Clamp(newPosition.y, -cameraHeightInWorldUnits, cameraHeightInWorldUnits);

        return newPosition;
    }

    private void TryNavigate()
    {
        var results = new Collider2D[1];
        Physics2D.OverlapCollider(m_collider, m_contactFilter, results);

        if (results[0] == null) return;
        
        //Navigate
        TraversalObject navArrow = results[0].GetComponent<TraversalObject>();
        navArrow.OnClicked();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Touch");
    }

}
