using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    private GameSceneManager gameSceneManager;

    private void Start()
    {
        gameSceneManager = FindFirstObjectByType<GameSceneManager>();
        startButton.onClick.AddListener(OnStartGame);
        InputManager.Instance.OnSelect += OnStartGame;
    }

    public void OnStartGame()
    {
        InputManager.Instance.OnSelect -= OnStartGame;
        gameSceneManager.StartGame();
        gameObject.SetActive(false);
    }
}
