using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInputManager : MonoBehaviour
{
    public static MenuInputManager Inputs;

    public event Action OnSelect;
    public event Action OnAltSelect;
    public event Action OnBack;
    public event Action OnSwitch;
	
    private PlayerInput m_playerInput;
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
        m_playerInput.MenuActions.Select.performed += Select_performed;
        m_playerInput.MenuActions.AltSelect.performed += AltSelect_performed;
        m_playerInput.MenuActions.Back.performed += Back_performed;
        m_playerInput.MenuActions.Switch.performed += Switch_performed;

        m_playerInput.MenuActions.Enable();
    }

    private void OnDisable()
    {
        m_playerInput.MenuActions.Select.performed -= Select_performed;
        m_playerInput.MenuActions.AltSelect.performed -= AltSelect_performed;
        m_playerInput.MenuActions.Back.performed -= Back_performed;
        m_playerInput.MenuActions.Switch.performed -= Switch_performed;

        m_playerInput.PlayerActions.Disable();
    }

    private void UpdateInputDevice(InputDevice device)
    {
        if (device is Gamepad) m_inputDevice = PlayerInputDevice.Controller;
        if (device is Keyboard || device is Mouse) m_inputDevice = PlayerInputDevice.MKB;
        
    }

    #region Select
    private void Select_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnSelect?.Invoke();
    }
    #endregion

    #region AltSelect
    private void AltSelect_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnAltSelect?.Invoke();
    }
    #endregion

    #region Back
    private void Back_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnBack?.Invoke();
    }
    #endregion

    #region Switch
    private void Switch_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnSwitch?.Invoke();
    }
    #endregion
}