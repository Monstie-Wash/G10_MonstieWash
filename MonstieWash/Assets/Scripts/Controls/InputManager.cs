using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    #region Input Events
    #region PlayerActions
    public event Action OnMove_Started;
    public event Action<Vector2> OnMove;
    public event Action OnMove_Ended;

    public event Action OnActivate_Started;
    public event Action OnActivate;
    public event Action OnActivate_Held;
    public event Action OnActivate_Ended;

    public event Action OnSwitchTool_Started;
    public event Action<int> OnSwitchTool;
    public event Action OnSwitchTool_Ended;

    public event Action OnNavigate_Started;
    public event Action OnNavigate;
    public event Action OnNavigate_Ended;

    public event Action OnToggleUI_Started;
    public event Action OnToggleUI;
    public event Action OnToggleUI_Ended;

    public event Action OnScan_Started;
    public event Action OnScan;
    public event Action OnScan_Ended;
    #endregion

    #region DebugActions
    public event Action OnDebugReset_Started;
    public event Action OnDebugReset;
    public event Action OnDebugReset_Ended;
    #endregion
    #endregion

    private PlayerInput m_playerInput;
    private bool m_runningActiveRoutine;
    private PlayerInputDevice m_inputDevice = PlayerInputDevice.Controller;

    public PlayerInputDevice InputDevice { get { return m_inputDevice; } }

    public enum PlayerInputDevice
    {
        Controller,
        MKB,
    }

    public enum ControlScheme
    {
        None,
        PlayerActions,
        MenuActions,
        DebugActions
    }

    private void Awake()
    {
        //Ensure there's only one
        if (Instance == null) Instance = this;
        else Destroy(this);

        m_playerInput = new PlayerInput();

        SetCursorMode(false);
        SetControlScheme(ControlScheme.MenuActions);
    }

    private void OnEnable()
    {
        #region PlayerActions Subscription
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

        m_playerInput.PlayerActions.ToggleUI.started += ToggleUI_started;
        m_playerInput.PlayerActions.ToggleUI.performed += ToggleUI_performed;
        m_playerInput.PlayerActions.ToggleUI.canceled += ToggleUI_canceled;

        m_playerInput.PlayerActions.Scan.started += Scan_started;
        m_playerInput.PlayerActions.Scan.performed += Scan_performed;
        m_playerInput.PlayerActions.Scan.canceled += Scan_canceled;
        #endregion

        #region DebugActions Subscription
        m_playerInput.DebugActions.DebugReset.started += DebugReset_started;
        m_playerInput.DebugActions.DebugReset.performed += DebugReset_performed;
        m_playerInput.DebugActions.DebugReset.canceled += DebugReset_canceled;
        #endregion
    }

    private void OnDisable()
    {
        #region PlayerActions Subscription
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

        m_playerInput.PlayerActions.ToggleUI.started -= ToggleUI_started;
        m_playerInput.PlayerActions.ToggleUI.performed -= ToggleUI_performed;
        m_playerInput.PlayerActions.ToggleUI.canceled -= ToggleUI_canceled;

        m_playerInput.PlayerActions.Scan.started -= Scan_started;
        m_playerInput.PlayerActions.Scan.performed -= Scan_performed;
        m_playerInput.PlayerActions.Scan.canceled -= Scan_canceled;
        #endregion

        #region DebugActions Subscription
        m_playerInput.DebugActions.DebugReset.started -= DebugReset_started;
        m_playerInput.DebugActions.DebugReset.performed -= DebugReset_performed;
        m_playerInput.DebugActions.DebugReset.canceled -= DebugReset_canceled;
        #endregion
    }

    private void UpdateInputDevice(InputDevice device)
    {
        if (device is Gamepad) m_inputDevice = PlayerInputDevice.Controller;
        if (device is Keyboard || device is Mouse) m_inputDevice = PlayerInputDevice.MKB;
        
    }

    #region Input Methods
    #region PlayerInput
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
        OnActivate?.Invoke();
        m_runningActiveRoutine = true;
        StartCoroutine(Activate());
    }

    private void Activate_canceled(InputAction.CallbackContext context)
    {
        OnActivate_Ended?.Invoke();
        m_runningActiveRoutine = false;
    }

    private IEnumerator Activate()
    {
        while (m_runningActiveRoutine)
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
        OnSwitchTool?.Invoke(Math.Sign(context.ReadValue<float>()));
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

    #region ToggleUI
    private void ToggleUI_started(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnToggleUI_Started?.Invoke();
    }

    private void ToggleUI_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnToggleUI?.Invoke();
    }

    private void ToggleUI_canceled(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnToggleUI_Ended?.Invoke();
    }
    #endregion

    #region Scan
    private void Scan_started(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnScan_Started?.Invoke();
    }

    private void Scan_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnScan?.Invoke();
    }

    private void Scan_canceled(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnScan_Ended?.Invoke();
    }
    #endregion
    #endregion

    #region DebugInput
    #region DebugReset
    private void DebugReset_started(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnDebugReset_Started?.Invoke();
    }

    private void DebugReset_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnDebugReset?.Invoke();
    }

    private void DebugReset_canceled(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnDebugReset_Ended?.Invoke();
    }
    #endregion
    #endregion
    #endregion

    public void SetCursorMode(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SetControlScheme(ControlScheme scheme)
    {
        m_runningActiveRoutine = false;

        switch (scheme)
        {
            case ControlScheme.None:
                {
                    m_playerInput.PlayerActions.Disable();
                    m_playerInput.MenuActions.Disable();
                    m_playerInput.DebugActions.Enable();
                }
                break;
            case ControlScheme.PlayerActions:
                {
                    m_playerInput.MenuActions.Disable();
                    m_playerInput.PlayerActions.Enable();
                    m_playerInput.DebugActions.Enable();
                }
                break;
            case ControlScheme.MenuActions:
                {
                    m_playerInput.PlayerActions.Disable();
                    m_playerInput.MenuActions.Enable();
                    m_playerInput.DebugActions.Enable();
                }
                break;
        }
    }
}