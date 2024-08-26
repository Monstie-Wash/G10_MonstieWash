using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private void OnEnable()
    {
        InputManager.Instance.OnDebugReset += ResetGame;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnDebugReset -= ResetGame;
    }

    private void ResetGame()
    {
        GameSceneManager.Instance.RestartGame();
    }
}
