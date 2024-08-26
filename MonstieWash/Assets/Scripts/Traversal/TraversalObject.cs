using UnityEngine;

public class TraversalObject : MonoBehaviour, INavigator
{
    [SerializeField] private GameScene targetScene;
    [SerializeField] private bool targetIsUI;

    private string m_targetScene;

    public void Awake()
    {
        if (targetScene == null) Debug.LogError($"Target scene not assigned for {name}!");
        else m_targetScene = targetScene.SceneName;
    }

    public void OnClicked()
    {
        GameSceneManager.Instance.MoveToScene(m_targetScene, targetIsUI);
    }
}