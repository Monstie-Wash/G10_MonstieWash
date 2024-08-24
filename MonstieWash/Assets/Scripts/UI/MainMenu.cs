using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private GameSceneManager m_gameSceneManager;
    [SerializeField] private Canvas m_menuCanvas;

    // Start is called before the first frame update
    void Start()
    {
        m_gameSceneManager = FindFirstObjectByType<GameSceneManager>();
    }

    public void StartGame()
    {
        m_gameSceneManager.StartGame();
        gameObject.SetActive(false);
    }


}
