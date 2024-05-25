using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public event Action OnScenesLoaded;
    public event Action<string> OnSceneChanged;

    [SerializeField] private GameScene levelSelectScene;
    [SerializeField] private GameScene loadingScene;
    [SerializeField] private GameScene scoreSummaryScene;
    [SerializeField] private GameScene deathScene;
    [SerializeField] private List<LevelScenes> allLevelScenes = new();

    private Scene m_currentScene;
    private LevelScenes m_currentLevelScenes;
    private List<string> m_activeScenes = new();

    public List<string> AllLevelScenes { get; private set; } = new();

    public enum Level
    {
        Slime,
        Mimic
    }

    [Serializable]
    private struct LevelScenes
    {
        public Level level;
        public GameScene startingScene;
        public List<GameScene> gameScenes;
    }
    
    private void Awake()
    {
        foreach (var level in allLevelScenes)
        {
            foreach (var gameScene in level.gameScenes)
            {
                if (gameScene == null) Debug.LogError($"Target scene not assigned for {name}!");
                else AllLevelScenes.Add(gameScene.SceneName);
            }
        }
    }

    private void Start()
    {
        LoadMenuScenes();
    }

    #region Private
    /// <summary>
    /// Asynchronously loads all the scenes in the m_allScenes list.
    /// </summary>
    private async void LoadMenuScenes()
    {
        await LoadScene(loadingScene.SceneName);
        await LoadScene(levelSelectScene.SceneName);

        SetSceneActive(loadingScene.SceneName, false);

        m_currentScene = SceneManager.GetSceneByName(levelSelectScene.SceneName);
    }

    private async Task UnloadActiveLevelScenes()
    {
        var tasks = new Task[m_activeScenes.Count];

        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = UnloadScene(m_activeScenes[i]);
        }

        m_activeScenes.Clear();

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Loads a scene.
    /// </summary>
    /// <param name="scene">The scene to load.</param>
    /// <returns></returns>
    private async Task LoadScene(string scene)
    {
        await SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Unloads a scene.
    /// </summary>
    /// <param name="scene">The scene to unload.</param>
    /// <returns></returns>
    private async Task UnloadScene(string scene)
    {
        await SceneManager.UnloadSceneAsync(scene);
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
    #endregion

    #region Public
    private async void LoadMonsterScene(Level level)
    {
        //Load the monster scenes
        m_currentLevelScenes = allLevelScenes.Find(levelScene => levelScene.level == level);
        var monsterScenes = m_currentLevelScenes.gameScenes;

        //Load the starting scene first (to manage dependencies)
        await LoadScene(m_currentLevelScenes.startingScene.SceneName);
        m_activeScenes.Add(m_currentLevelScenes.startingScene.SceneName);

        //Load the rest of the scenes
        var tasks = new Task[monsterScenes.Count];

        for (int i = 0; i < tasks.Length; i++)
        {
            var sceneName = monsterScenes[i].SceneName;
            tasks[i] = LoadScene(sceneName);
            m_activeScenes.Add(sceneName);
        }

        await Task.WhenAll(tasks);

        OnScenesLoaded?.Invoke();

        //Set all as inactive
        for (int i = 0; i < monsterScenes.Count; i++)
        {
            SetSceneActive(monsterScenes[i].SceneName, false);
        }

        //Remove loading screen and activate first level
        MoveToScene(monsterScenes[0].SceneName);
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

    public async void StartNewLevel(Level level)
    {
        MoveToScene(loadingScene.SceneName);

        SetSceneActive(levelSelectScene.SceneName, false);
        await UnloadActiveLevelScenes();

        InputManager.Inputs.SetCursorMode(true);
        LoadMonsterScene(level);
    }

    public async void LevelComplete()
    {
        await Task.Delay(3000);

        MoveToScene(loadingScene.SceneName);

        SetSceneActive(m_currentLevelScenes.startingScene.SceneName, false);
        await LoadScene(scoreSummaryScene.SceneName);
        m_activeScenes.Add(scoreSummaryScene.SceneName);

        InputManager.Inputs.SetCursorMode(false);
        MoveToScene(scoreSummaryScene.SceneName);
    }

    public async void PlayerDied()
    {
        await Task.Delay(3000);

        MoveToScene(loadingScene.SceneName);

        SetSceneActive(m_currentLevelScenes.startingScene.SceneName, false);
        await LoadScene(deathScene.SceneName);
        m_activeScenes.Add(deathScene.SceneName);

        InputManager.Inputs.SetCursorMode(false);
        MoveToScene(deathScene.SceneName);
    }

    public async void GoToMainMenu()
    {
        MoveToScene(loadingScene.SceneName);

        await UnloadActiveLevelScenes();

        GetComponentInChildren<MusicManager>().SetMusic(MusicManager.MusicType.Background);
        InputManager.Inputs.SetCursorMode(false);
        MoveToScene(levelSelectScene.SceneName);
    }

    public async void ContinueLevel()
    {
        await UnloadScene(deathScene.SceneName);
        m_activeScenes.RemoveAt(m_activeScenes.IndexOf(deathScene.SceneName));

        GetComponentInChildren<MusicManager>().SetMusic(MusicManager.MusicType.Background);
        InputManager.Inputs.SetCursorMode(true);
        SetSceneActive(m_currentLevelScenes.startingScene.SceneName, true);
        SetSceneActive(m_activeScenes[1], true);
    }

    public async void RestartLevel()
    {
        MoveToScene(loadingScene.SceneName);

        await UnloadActiveLevelScenes();

        GetComponentInChildren<MusicManager>().SetMusic(MusicManager.MusicType.Background);
        InputManager.Inputs.SetCursorMode(true);
        LoadMonsterScene(m_currentLevelScenes.level);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
    #endregion
}
