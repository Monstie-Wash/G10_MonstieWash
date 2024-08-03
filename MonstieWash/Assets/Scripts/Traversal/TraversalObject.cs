using UnityEngine;

public class TraversalObject : MonoBehaviour, INavigator
{
    [SerializeField] private GameScene targetScene;
    [SerializeField] private bool targetIsUI;

    private GameSceneManager m_gameSceneManager;
    private string m_targetScene;

    public void Awake()
    {
        if (targetScene == null) Debug.LogError($"Target scene not assigned for {name}!");
        else m_targetScene = targetScene.SceneName;

        m_gameSceneManager = FindFirstObjectByType<GameSceneManager>();
    }

    public void OnClicked()
    {
        m_gameSceneManager.MoveToScene(m_targetScene, targetIsUI);
    }
}