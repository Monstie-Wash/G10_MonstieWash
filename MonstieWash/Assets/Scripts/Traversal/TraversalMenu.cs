using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TraversalMenu : MonoBehaviour
{
    [SerializeField] private MenuAction action;
    [SerializeField] private GameSceneManager.Level level;

    private GameSceneManager m_sceneManager;
    private Button m_button;

    public enum MenuAction
    {
        StartNewLevel,
        Continue,
        Restart,
        MainMenu,
        UpgradeMenu,
        Quit
    }

    private void Awake()
    {
        m_sceneManager = FindFirstObjectByType<GameSceneManager>();
        m_button = GetComponent<Button>();

        m_button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        switch (action)
        {
            case MenuAction.StartNewLevel:
                {
                    m_sceneManager.StartNewLevel(level);
                }
                break;
            case MenuAction.Continue:
                {
                    m_sceneManager.ContinueLevel();
                }
                break;
            case MenuAction.Restart:
                {
                    m_sceneManager.RestartLevel();
                }
                break;
            case MenuAction.MainMenu:
                {
                    m_sceneManager.GoToMainMenu();
                }
                break;
            case MenuAction.UpgradeMenu:
                {
                    m_sceneManager.GoToUpgradeMenu();
                }
                break;
            case MenuAction.Quit:
                {
                    m_sceneManager.QuitGame();
                }
                break;
        }
    }
}

#region Custom Editor
#if UNITY_EDITOR
[CustomEditor(typeof(TraversalMenu))]
public class TraversalMenuInspector : Editor
{
    SerializedProperty action;
    SerializedProperty level;

    private void OnEnable()
    {
        action = serializedObject.FindProperty("action");
        level = serializedObject.FindProperty("level");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(action);

        if (action.enumValueIndex == (int)TraversalMenu.MenuAction.StartNewLevel)
        {
            EditorGUILayout.PropertyField(level);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
#endregion