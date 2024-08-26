using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        GameSceneManager.Instance.StartGame();
        gameObject.SetActive(false);
    }
}