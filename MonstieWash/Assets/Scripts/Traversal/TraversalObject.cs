using UnityEditor;
using UnityEngine;

public class TraversalObject : MonoBehaviour
{
    [SerializeField] private Object targetScene;

    private RoomSaver m_saveObj;
    private SceneAsset m_targetScene;

    public void Awake()
    {
        if (targetScene == null) Debug.LogError($"Target scene not assigned for {name}!");
        else if (!(targetScene is SceneAsset)) Debug.LogError($"Target scene is not a scene for {name}!");
        else m_targetScene = targetScene as SceneAsset;

        m_saveObj = FindFirstObjectByType<RoomSaver>();
    }

    public void OnClicked()
    {
        m_saveObj.MoveToScene(m_targetScene);
    }
}