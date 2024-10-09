using UnityEngine;
using UnityEngine.Video;

public class DebugManager : MonoBehaviour
{
    private void OnEnable()
    {
        InputManager.Instance.OnDebugReset += ResetGame;
        InputManager.Instance.OnFinishLevel += FinishLevel;
        InputManager.Instance.OnSkipAnimatic += SkipAnimatic;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnDebugReset -= ResetGame;
        InputManager.Instance.OnFinishLevel -= FinishLevel;
        InputManager.Instance.OnSkipAnimatic -= SkipAnimatic;
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

    private void SkipAnimatic()
    {
        if (!GameSceneManager.Instance.CurrentScene.name.Equals("LevelSelectScene")) return;

        var vc = FindFirstObjectByType<VideoController>();
        if (vc.GetComponent<VideoPlayer>().enabled) vc.FinishAnimatic();
    }
}
