using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(OnStartGame);
        InputManager.Instance.OnSelect += OnStartGame;
    }

    public void OnStartGame()
    {
        InputManager.Instance.OnSelect -= OnStartGame;
        GameSceneManager.Instance.StartGame();
        gameObject.SetActive(false);
    }
}
