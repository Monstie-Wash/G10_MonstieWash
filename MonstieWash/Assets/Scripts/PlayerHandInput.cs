using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandInput : MonoBehaviour
{
    [SerializeField]
    private PlayerHand m_playerHand;
    [SerializeField]
    private Eraser m_eraser;

    private PlayerInput m_playerInput;
    private Coroutine m_activeRoutine;

    private void Awake()
    {
        m_playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        m_playerInput.PlayerActions.Move.performed += Move_performed;
        m_playerInput.PlayerActions.Move.canceled += Move_canceled;
        m_playerInput.PlayerActions.Activate.performed += Activate_performed;
        m_playerInput.PlayerActions.Activate.canceled += Activate_canceled;
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
        m_playerHand.MovePerformed(context);
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        m_playerHand.MoveCancelled();
    }

    private void Activate_performed(InputAction.CallbackContext context)
    {
        m_activeRoutine = StartCoroutine(Activate());
    }

    private void Activate_canceled(InputAction.CallbackContext obj)
    {
        StopCoroutine(m_activeRoutine);
    }

    private IEnumerator Activate()
    {
        while (true)
        {
            m_eraser.UseTool();

            yield return null;
        }
    }
}


    
