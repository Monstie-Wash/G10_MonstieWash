using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Inputs;

    public event Action OnMove_Started;
    public event Action<Vector2> OnMove;
    public event Action OnMove_Ended;

    public event Action OnActivate_Started;
    public event Action OnActivate_Held;
    public event Action OnActivate_Ended;

    public event Action OnSwitchTool_Started;
    public event Action<float> OnSwitchTool;
    public event Action OnSwitchTool_Ended;

    public event Action OnNavigate_Started;
    public event Action OnNavigate;
    public event Action OnNavigate_Ended;

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
        m_playerInput.PlayerActions.Move.started += Move_started;
        m_playerInput.PlayerActions.Move.performed += Move_performed;
        m_playerInput.PlayerActions.Move.canceled += Move_canceled;

        m_playerInput.PlayerActions.Activate.started += Activate_started;
        m_playerInput.PlayerActions.Activate.performed += Activate_performed;
        m_playerInput.PlayerActions.Activate.canceled += Activate_canceled;

        m_playerInput.PlayerActions.SwitchTool.started += SwitchTool_started;
        m_playerInput.PlayerActions.SwitchTool.performed += SwitchTool_performed;
        m_playerInput.PlayerActions.SwitchTool.canceled += SwitchTool_canceled;

        m_playerInput.PlayerActions.Navigate.started += Navigate_started;
        m_playerInput.PlayerActions.Navigate.performed += Navigate_performed;
        m_playerInput.PlayerActions.Navigate.canceled += Navigate_canceled;

        m_playerInput.PlayerActions.Enable();
    }

    private void OnDisable()
    {
        m_playerInput.PlayerActions.Move.started -= Move_started;
        m_playerInput.PlayerActions.Move.performed -= Move_performed;
        m_playerInput.PlayerActions.Move.canceled -= Move_canceled;

        m_playerInput.PlayerActions.Activate.started -= Activate_started;
        m_playerInput.PlayerActions.Activate.performed -= Activate_performed;
        m_playerInput.PlayerActions.Activate.canceled -= Activate_canceled;

        m_playerInput.PlayerActions.SwitchTool.started -= SwitchTool_started;
        m_playerInput.PlayerActions.SwitchTool.performed -= SwitchTool_performed;
        m_playerInput.PlayerActions.SwitchTool.canceled -= SwitchTool_canceled;

        m_playerInput.PlayerActions.Navigate.started -= Navigate_started;
        m_playerInput.PlayerActions.Navigate.performed -= Navigate_performed;
        m_playerInput.PlayerActions.Navigate.canceled -= Navigate_canceled;

        m_playerInput.PlayerActions.Disable();
    }

    private void UpdateInputDevice(InputDevice device)
    {
        if (device is Gamepad) m_inputDevice = PlayerInputDevice.Controller;
        if (device is Keyboard || device is Mouse) m_inputDevice = PlayerInputDevice.MKB;
        
    }

    #region Move
    private void Move_started(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnMove_Started?.Invoke();
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
    #endregion

    #region Activate
    private void Activate_started(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnActivate_Started?.Invoke();
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
    #endregion

    #region SwitchTool
    private void SwitchTool_started(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnSwitchTool_Started?.Invoke();
    }

    private void SwitchTool_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnSwitchTool?.Invoke(context.ReadValue<float>());
    }

    private void SwitchTool_canceled(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnSwitchTool_Ended?.Invoke();
    }
    #endregion

    #region Navigate
    private void Navigate_started(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnNavigate_Started?.Invoke();
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
    #endregion
}