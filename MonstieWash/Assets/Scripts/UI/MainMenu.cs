using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        VideoController vc = FindFirstObjectByType<VideoController>();

        vc.TransitionFromMainMenu();
        GameSceneManager.Instance.StartGame();
        vc.PlayAnimatic();

        gameObject.SetActive(false);
    }
}