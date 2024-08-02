using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TraversalMenu : MonoBehaviour
{
    [SerializeField] private MenuAction action;
    [SerializeField] private GameSceneManager.Level level;
    [SerializeField] private GameScene targetScene;
    [SerializeField] private bool targetIsUI;
    [SerializeField] private bool setMusic;
    [SerializeField] private MusicManager.MusicType music;

    private GameSceneManager m_sceneManager;
    private Button m_button;

    public enum MenuAction
    {
        StartNewLevel,
        Continue,
        Restart,
        GoToBedroomScene,
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
            case MenuAction.GoToBedroomScene:
                {
                    if (setMusic) _ = m_sceneManager.GoToBedroomScene(targetScene.SceneName, targetIsUI, music);
                    else _ = m_sceneManager.GoToBedroomScene(targetScene.SceneName, targetIsUI);
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
    SerializedProperty action, level, targetScene, targetIsUI, setMusic, music;

    private void OnEnable()
    {
        action = serializedObject.FindProperty("action");
        level = serializedObject.FindProperty("level");
        targetScene = serializedObject.FindProperty("targetScene");
        targetIsUI = serializedObject.FindProperty("targetIsUI");
        setMusic = serializedObject.FindProperty("setMusic");
        music = serializedObject.FindProperty("music");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(action);

        if (action.enumValueIndex == (int)TraversalMenu.MenuAction.StartNewLevel)
        {
            EditorGUILayout.PropertyField(level);
        }

        if (action.enumValueIndex == (int)TraversalMenu.MenuAction.GoToBedroomScene)
        {
            EditorGUILayout.PropertyField(targetScene);
            EditorGUILayout.PropertyField(targetIsUI);
            EditorGUILayout.PropertyField(setMusic);
            if (setMusic.boolValue) EditorGUILayout.PropertyField(music);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
#endregion