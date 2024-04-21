using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RoomSaver : MonoBehaviour
{
    public event Action OnScenesLoaded;
    public event Action<string> OnSceneChanged;

    [SerializeField] private List<GameScene> allScenes = new();

    public List<string> m_allScenes = new();
    private Scene m_currentScene;


    private void Awake()
    {
        for (int i = 0; i < allScenes.Count; i++)
        {
            if (allScenes[i] == null) Debug.LogError($"Target scene not assigned for {name}!");
            else m_allScenes.Add(allScenes[i].SceneName);
        }
    }

    private void Start()
    {
        LoadScenes();
    }

    /// <summary>
    /// Asynchronously oads all the scenes in the m_allScenes list.
    /// </summary>
    private async void LoadScenes()
    {
        var tasks = new Task[m_allScenes.Count];

        for (int i = 0; i < m_allScenes.Count; i++)
        {
            tasks[i] = LoadScene(m_allScenes[i]);
        }

        await Task.WhenAll(tasks);

        OnScenesLoaded?.Invoke();

        for (int i = 0; i < m_allScenes.Count; i++)
        {
            SetSceneActive(m_allScenes[i], false);
        }

        SetSceneActive(m_allScenes[0], true);
    }

    /// <summary>
    /// Loads a scene and disables it when it is finished loading.
    /// </summary>
    /// <param name="scene">The scene to load.</param>
    /// <returns></returns>
    private async Task LoadScene(string scene)
    {
        await SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Swaps the current active scene to a new scene
    /// </summary>
    /// <param name="target">The scene to move to.</param>
    public void MoveToScene(string target)
    {
        SetSceneActive(m_currentScene.name, false);
        SetSceneActive(target, true);

        OnSceneChanged?.Invoke(target);
    }

    /// <summary>
    /// Disables all gameobjects within a scene.
    /// </summary>
    /// <param name="scene">The scene to disable.</param>
    private void SetSceneActive(string scene, bool active)
    {
        var currentScene = SceneManager.GetSceneByName(scene);
        if (currentScene.name == null) Debug.LogError($"{scene} is not a scene!");

        var gameObjs = currentScene.GetRootGameObjects();

        foreach (var gameObject in gameObjs)
        {
            gameObject.SetActive(active);
        }

        if (active) m_currentScene = currentScene;
    }
}
