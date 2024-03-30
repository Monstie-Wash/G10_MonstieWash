using System;
using System.Collections;
using System.Collections.Generic;
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

#pragma warning restore 67

    private PlayerInput m_playerInput;
    private Coroutine m_activeRoutine;

    private void Awake()
    {
        //Ensure there's only one
        if (Inputs == null) Inputs = this;
        else Destroy(this);

        m_playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        m_playerInput.PlayerActions.Move.performed += Move_performed;
        m_playerInput.PlayerActions.Move.canceled += Move_canceled;
        m_playerInput.PlayerActions.Activate.performed += Activate_performed;
        m_playerInput.PlayerActions.Activate.canceled += Activate_canceled;
        m_playerInput.PlayerActions.Transfer.performed += Transfer_performed;
        m_playerInput.PlayerActions.Enable();
    }

    private void OnDisable()
    {
        m_playerInput.PlayerActions.Move.performed -= Move_performed;
        m_playerInput.PlayerActions.Move.canceled -= Move_canceled;
        m_playerInput.PlayerActions.Activate.performed -= Activate_performed;
        m_playerInput.PlayerActions.Activate.canceled -= Activate_canceled;
        m_playerInput.PlayerActions.Disable();
    }

    private void Move_performed(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>());
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        OnMove_Ended?.Invoke();
    }

    private void Activate_performed(InputAction.CallbackContext context)
    {
        m_activeRoutine = StartCoroutine(Activate());
    }

    private void Activate_canceled(InputAction.CallbackContext context)
    {
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
        OnTransfer?.Invoke();
    }
}


    
