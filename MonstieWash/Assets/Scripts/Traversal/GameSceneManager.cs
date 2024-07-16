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
    [SerializeField] private GameScene upgradeScene;
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
        await LoadScene(upgradeScene.SceneName);

        SetSceneActive(upgradeScene.SceneName, false);
        SetSceneActive(loadingScene.SceneName, false);

        InputManager.Instance.SetCursorMode(false);
        InputManager.Instance.SetControlScheme(InputManager.ControlScheme.MenuActions);
        m_currentScene = SceneManager.GetSceneByName(levelSelectScene.SceneName);
    }

    /// <summary>
    /// Unloads all currently active level scenes.
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// Loads all the scenes of the given level and moves there.
    /// </summary>
    /// <param name="level">The level to load.</param>
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

    /// <summary>
    /// Starts a level from the beginning. Intended to be run from the level select scene.
    /// </summary>
    /// <param name="level"></param>
    public async void StartNewLevel(Level level)
    {
        MoveToScene(loadingScene.SceneName);

        SetSceneActive(levelSelectScene.SceneName, false);
        await UnloadActiveLevelScenes();

        GetComponentInChildren<MusicManager>().SetMusic(MusicManager.MusicType.Background);
        InputManager.Instance.SetCursorMode(true);
        InputManager.Instance.SetControlScheme(InputManager.ControlScheme.PlayerActions);
        LoadMonsterScene(level);
    }

    /// <summary>
    /// Loads the score summary scene.
    /// </summary>
    public async void LevelComplete()
    {
        await Task.Delay(3000);

        MoveToScene(loadingScene.SceneName);

        SetSceneActive(m_currentLevelScenes.startingScene.SceneName, false);
        await LoadScene(scoreSummaryScene.SceneName);
        m_activeScenes.Add(scoreSummaryScene.SceneName);

        InputManager.Instance.SetCursorMode(false);
        InputManager.Instance.SetControlScheme(InputManager.ControlScheme.MenuActions);
        MoveToScene(scoreSummaryScene.SceneName);
    }

    /// <summary>
    /// Loads the death screen.
    /// </summary>
    public async void PlayerDied()
    {
        await Task.Delay(7000);

        MoveToScene(loadingScene.SceneName);

        SetSceneActive(m_currentLevelScenes.startingScene.SceneName, false);
        await LoadScene(deathScene.SceneName);
        m_activeScenes.Add(deathScene.SceneName);

        InputManager.Instance.SetCursorMode(false);
        InputManager.Instance.SetControlScheme(InputManager.ControlScheme.MenuActions);
        MoveToScene(deathScene.SceneName);
    }

    public async void GoToUpgradeMenu()
    {
        MoveToScene(loadingScene.SceneName);

        SetSceneActive(scoreSummaryScene.SceneName, false);

        GetComponentInChildren<MusicManager>().SetMusic(MusicManager.MusicType.Evening);
        InputManager.Instance.SetCursorMode(false);
        InputManager.Instance.SetControlScheme(InputManager.ControlScheme.MenuActions);
        MoveToScene(upgradeScene.SceneName);
    }

    /// <summary>
    /// Loads the main menu.
    /// </summary>
    public async void GoToMainMenu()
    {
        MoveToScene(loadingScene.SceneName);

        await UnloadActiveLevelScenes();

        GetComponentInChildren<MusicManager>().SetMusic(MusicManager.MusicType.Morning);
        InputManager.Instance.SetCursorMode(false);
        InputManager.Instance.SetControlScheme(InputManager.ControlScheme.MenuActions);
        MoveToScene(levelSelectScene.SceneName);
    }

    /// <summary>
    /// Sets the current scene back to the first scene of the current level.
    /// </summary>
    public async void ContinueLevel()
    {
        await UnloadScene(deathScene.SceneName);
        m_activeScenes.RemoveAt(m_activeScenes.IndexOf(deathScene.SceneName));

        GetComponentInChildren<MusicManager>().SetMusic(MusicManager.MusicType.Background);
        InputManager.Instance.SetCursorMode(true);
        InputManager.Instance.SetControlScheme(InputManager.ControlScheme.PlayerActions);
        SetSceneActive(m_currentLevelScenes.startingScene.SceneName, true);
        SetSceneActive(m_activeScenes[1], true);
    }

    /// <summary>
    /// Restarts the current level.
    /// </summary>
    public async void RestartLevel()
    {
        MoveToScene(loadingScene.SceneName);

        await UnloadActiveLevelScenes();

        GetComponentInChildren<MusicManager>().SetMusic(MusicManager.MusicType.Background);
        InputManager.Instance.SetCursorMode(true);
        InputManager.Instance.SetControlScheme(InputManager.ControlScheme.PlayerActions);
        LoadMonsterScene(m_currentLevelScenes.level);
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
    #endregion
}
