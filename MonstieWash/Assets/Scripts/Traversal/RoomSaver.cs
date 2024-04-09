using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RoomSaver : MonoBehaviour
{
    public event Action OnScenesLoaded;
    public event Action OnSceneChanged;

    [SerializeField] private List<UnityEngine.Object> allScenes = new();

    private List<SceneAsset> m_allScenes = new();
    private SceneAsset m_currentScene;


    private void Awake()
    {
        for (int i = 0; i < allScenes.Count; i++)
        {
            if (allScenes[i] == null) Debug.LogError($"Target scene not assigned for {name}!");
            else if (!(allScenes[i] is SceneAsset)) Debug.LogError($"Target scene is not a scene for {name}!");
            else m_allScenes.Add(allScenes[i] as SceneAsset);
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
    private async Task LoadScene(SceneAsset scene)
    {
        await SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Swaps the current active scene to a new scene
    /// </summary>
    /// <param name="target">The scene to move to.</param>
    public void MoveToScene(SceneAsset target)
    {
        SetSceneActive(m_currentScene, false);
        SetSceneActive(target, true);

        OnSceneChanged?.Invoke();
    }

    /// <summary>
    /// Disables all gameobjects within a scene.
    /// </summary>
    /// <param name="scene">The scene to disable.</param>
    private void SetSceneActive(SceneAsset scene, bool active)
    {
        var currentScene = SceneManager.GetSceneByName(scene.name);
        var gameObjs = currentScene.GetRootGameObjects();

        foreach (var gameObject in gameObjs)
        {
            gameObject.SetActive(active);
        }

        if (active) m_currentScene = scene;
    }
}
