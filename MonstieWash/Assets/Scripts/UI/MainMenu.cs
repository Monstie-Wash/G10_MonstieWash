using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    private GameSceneManager m_gameSceneManager;

    private void Start()
    {
        m_gameSceneManager = FindFirstObjectByType<GameSceneManager>();
        startButton.onClick.AddListener(OnStartGame);
        InputManager.Instance.OnSelect += OnStartGame;
    }

    public void OnStartGame()
    {
        InputManager.Instance.OnSelect -= OnStartGame;
        m_gameSceneManager.StartGame();
        gameObject.SetActive(false);
    }
}
