using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Inputs;

#pragma warning disable 67

    public event Action<Vector2> OnMove;
    public event Action OnMove_Ended;

    public event Action OnActivate_Started;
    public event Action OnActivate_Held;
    public event Action OnActivate_Ended;

    public event Action OnTransfer;
    public event Action OnTransfer_Ended;

    public event Action OnNavigate;
    public event Action OnNavigate_Ended;

#pragma warning restore 67

    private PlayerInput m_playerInput;
    private Coroutine m_activeRoutine;
    private PlayerInputDevice m_inputDevice = PlayerInputDevice.Controller;

    public PlayerInputDevice InputDevice { get { return m_inputDevice; } }

    public enum PlayerInputDevice
    {
        Controller,
        MKB,
    }

    private void Awake()
    {
        //Ensure there's only one
        if (Inputs == null) Inputs = this;
        else Destroy(this);

        m_playerInput = new PlayerInput();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        m_playerInput.PlayerActions.Move.performed += Move_performed;
        m_playerInput.PlayerActions.Move.canceled += Move_canceled;
        m_playerInput.PlayerActions.Activate.performed += Activate_performed;
        m_playerInput.PlayerActions.Activate.canceled += Activate_canceled;
        m_playerInput.PlayerActions.Transfer.performed += Transfer_performed;
        m_playerInput.PlayerActions.Navigate.performed += Navigate_performed;
        m_playerInput.PlayerActions.Navigate.canceled += Navigate_canceled; ;
        m_playerInput.PlayerActions.Enable();
    }

    private void OnDisable()
    {
        m_playerInput.PlayerActions.Move.performed -= Move_performed;
        m_playerInput.PlayerActions.Move.canceled -= Move_canceled;
        m_playerInput.PlayerActions.Activate.performed -= Activate_performed;
        m_playerInput.PlayerActions.Activate.canceled -= Activate_canceled;
        m_playerInput.PlayerActions.Transfer.performed -= Transfer_performed;
        m_playerInput.PlayerActions.Disable();
    }

    private void UpdateInputDevice(InputDevice device)
    {
        if (device is Gamepad) m_inputDevice = PlayerInputDevice.Controller;
        if (device is Keyboard || device is Mouse) m_inputDevice = PlayerInputDevice.MKB;
        
    }

    private void Move_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnMove?.Invoke(context.ReadValue<Vector2>());
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnMove_Ended?.Invoke();
    }

    private void Activate_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnActivate_Started?.Invoke();
        m_activeRoutine = StartCoroutine(Activate());
    }

    private void Activate_canceled(InputAction.CallbackContext context)
    {
        OnActivate_Ended?.Invoke();
        StopCoroutine(m_activeRoutine);
    }

    private IEnumerator Activate()
    {
        while (true)
        {
            OnActivate_Held?.Invoke();

            yield return null;
        }
    }

    private void Transfer_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnTransfer?.Invoke();
    }

    private void Navigate_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnNavigate?.Invoke();
    }

    private void Navigate_canceled(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnNavigate_Ended?.Invoke();
    }
}


    
