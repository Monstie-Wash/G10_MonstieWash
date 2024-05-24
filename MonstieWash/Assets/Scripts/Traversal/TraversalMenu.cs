using UnityEngine;
using UnityEngine.UI;

public class TraversalMenu : MonoBehaviour
{
    [SerializeField] private GameScene targetScene;
    [SerializeField] private LoadBehaviour loadBehaviour;
    private enum LoadBehaviour
    {
        Add,
        Single,
        Move
    }



    private MenuLoader m_menuObj;
    private RoomSaver m_saveObj;
    private string m_targetScene;
    private Button m_button;

    private void Awake()
    {
        if (targetScene == null) Debug.LogError($"Target scene not assigned for {name}!");
        else m_targetScene = targetScene.SceneName;

        m_menuObj = FindFirstObjectByType<MenuLoader>();
        m_saveObj = FindFirstObjectByType<RoomSaver>();
        m_button = GetComponent<Button>();

        m_button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        switch(loadBehaviour)
        {
            case LoadBehaviour.Add:
                m_menuObj.LoadSceneAdditive(m_targetScene);
                break;
            case LoadBehaviour.Single:
                m_menuObj.LoadSceneSingle(m_targetScene);
                break;
            case LoadBehaviour.Move:
                m_saveObj.MoveToScene(m_targetScene);
                break;
        }
        
    }
}