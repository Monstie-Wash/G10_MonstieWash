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
        GameSceneManager.Instance.BeginDecoration();
        var decoNav = FindFirstObjectByType<DecorationNavigate>(FindObjectsInactive.Include);
        decoNav.InDecorationScene = true;
        decoNav.gameObject.SetActive(true);
    }
}
