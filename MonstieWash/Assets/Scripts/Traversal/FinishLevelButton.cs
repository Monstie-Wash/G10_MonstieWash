using UnityEngine;

public class FinishLevelButton : MonoBehaviour, INavigator
{
    private GameSceneManager m_gameSceneManager;

    private void Awake()
    {
        m_gameSceneManager = FindFirstObjectByType<GameSceneManager>();
    }

    public void OnClicked()
    {
        m_gameSceneManager.FinishLevel();
    }
}
