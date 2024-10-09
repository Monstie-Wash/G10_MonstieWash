using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TraversalMenu : MonoBehaviour, INavigator
{
    [SerializeField] private MenuAction action;
    [SerializeField] private GameSceneManager.Level level;
    [SerializeField] private GameScene targetScene;
    [SerializeField] private bool targetIsUI;
    [SerializeField] private bool setMusic;
    [SerializeField] private MusicManager.MusicType music;

    private Button m_button;

    //Accesor used by Video Controller to start level after finishing;
    public GameSceneManager.Level Level { get { return level; } }

    public enum MenuAction
    {
        StartNewLevel,
        Restart,
        GoToBedroomScene,
        GoToAnimatic,
        Quit
    }

    private void Awake()
    {
        m_button = GetComponent<Button>();

        m_button.onClick.AddListener(OnClicked);
    }

    public void OnClicked()
    {
        switch (action)
        {
            case MenuAction.StartNewLevel:
                {
                    GameSceneManager.Instance.StartNewLevel(level);
                }
                break;
            case MenuAction.Restart:
                {
                    GameSceneManager.Instance.RestartLevel();
                }
                break;
            case MenuAction.GoToBedroomScene:
                {
                    if (setMusic) _ = GameSceneManager.Instance.GoToBedroomScene(targetScene.SceneName, targetIsUI, music);
                    else _ = GameSceneManager.Instance.GoToBedroomScene(targetScene.SceneName, targetIsUI);
                }
                break;
            case MenuAction.GoToAnimatic:
                {
                    FindFirstObjectByType<VideoController>().PlayAnimatic();
                }
                break;
            case MenuAction.Quit:
                {
                    GameSceneManager.Instance.QuitGame();
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