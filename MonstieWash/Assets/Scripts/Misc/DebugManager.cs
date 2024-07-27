using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private GameSceneManager m_gameSceneManager;

    private void Awake()
    {
        m_gameSceneManager = GetComponent<GameSceneManager>();
    }

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
        m_gameSceneManager.RestartGame();
    }
}
