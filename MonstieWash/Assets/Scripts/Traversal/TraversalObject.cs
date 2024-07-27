using UnityEngine;

public class TraversalObject : MonoBehaviour
{
    [SerializeField] private GameScene targetScene;
    [SerializeField] private bool targetIsUI;

    private GameSceneManager m_saveObj;
    private string m_targetScene;

    public void Awake()
    {
        if (targetScene == null) Debug.LogError($"Target scene not assigned for {name}!");
        else m_targetScene = targetScene.SceneName;

        m_saveObj = FindFirstObjectByType<GameSceneManager>();
    }

    public void OnClicked()
    {
        m_saveObj.MoveToScene(m_targetScene, targetIsUI);
    }
}