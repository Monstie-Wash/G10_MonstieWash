using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private void OnEnable()
    {
        InputManager.Instance.OnDebugReset += ResetGame;
        InputManager.Instance.OnFinishLevel += FinishLevel;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnDebugReset -= ResetGame;
        InputManager.Instance.OnFinishLevel -= FinishLevel;
    }

    private void ResetGame()
    {
        GameSceneManager.Instance.RestartGame();
    }

    private void FinishLevel()
    {
        GameSceneManager.Instance.FinishLevel();
    }
}
