using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Canvas m_menuCanvas;

    public void StartGame()
    {
        GameSceneManager.Instance.StartGame();
        gameObject.SetActive(false);
    }
}