using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        FindFirstObjectByType<VideoController>().TransitionFromMainMenu();

        GameSceneManager.Instance.StartGame();
        gameObject.SetActive(false);
    }
}