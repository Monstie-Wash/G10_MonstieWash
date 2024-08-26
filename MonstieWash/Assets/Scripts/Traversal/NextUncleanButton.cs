using UnityEngine;

public class NextUncleanButton : MonoBehaviour, INavigator
{
    private GameSceneManager m_gameSceneManager;
    private TaskTracker m_taskTracker;

    private void Awake()
    {
        m_gameSceneManager = FindFirstObjectByType<GameSceneManager>();
        m_taskTracker = FindFirstObjectByType<TaskTracker>();
    }

    public void OnClicked()
    {
        string targetScene = "";

        foreach (var kvp in m_taskTracker.ScenesCompleted)
        {
            if (!kvp.Value)
            {
                targetScene = kvp.Key.name;
                break;
            }
        }

        if (targetScene != "") m_gameSceneManager.MoveToScene(targetScene);
        else Debug.LogError("No unclean scenes found!");
    }
}
