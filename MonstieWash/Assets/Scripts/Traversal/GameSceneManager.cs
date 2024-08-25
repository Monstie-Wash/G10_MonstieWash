using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    /// <summary>Event that fires when all monster scenes have been loaded, but before they are set to inactive.</summary>
    public event Action OnMonsterScenesLoaded;
    /// <summary>Event that fires right before switching to another scene.</summary>
    public event Action OnSceneSwitch;
    /// <summary>Event that fires right after switching to another scene.</summary>
    public event Action OnSceneChanged;
    /// <summary>Event that fires when the level ends, before moving to another scene.</summary>
    public event Action OnLevelEnd;
    /// <summary>Event that fires right before the game is reset.</summary>
    public event Action OnRestartGame;

    [SerializeField] private GameScene gameStartingScene;
    [SerializeField] private GameScene loadingScene;
    [SerializeField] private GameScene initialScene;
    [SerializeField] private List<GameScene> bedroomScenes;
    [SerializeField] private GameScene scoreSummaryScene;
    [SerializeField] private GameScene deathScene;
    [SerializeField] private List<LevelScenes> allLevelScenes = new();

    private Level m_currentLevel;
    private Scene m_currentScene;
    private LevelScenes m_currentLevelScenes;
    private List<string> m_loadedScenes = new();
    private MusicManager m_musicManager;

    [HideInInspector] public List<string> AllLevelScenes { get; private set; } = new();
    [HideInInspector] public Level CurrentLevel { get { return m_currentLevel; } }
    [HideInInspector] public Scene CurrentScene { get { return m_currentScene; } }

    public enum Level
    {
        None,
        Slime,
        Mimic,
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
        m_musicManager = GetComponentInChildren<MusicManager>();

        foreach (var level in allLevelScenes)
        {
            foreach (var gameScene in level.gameScenes)
            {
                if (gameScene == null) Debug.LogError($"Target scene not assigned for {name}!");
                else AllLevelScenes.Add(gameScene.SceneName);
            }
        }
    }

    /// <summary>
    /// A function run from the main menu to start the game.
    /// </summary>
    public async void StartGame()
    {
        await LoadScene(loadingScene.SceneName, false);
        await LoadBedroomScenes();

        SetSceneActive(initialScene.SceneName, true);

        m_currentScene = SceneManager.GetSceneByName(initialScene.SceneName);
        InputManager.Instance.SetCursorMode(false);
        InputManager.Instance.SetControlScheme(InputManager.ControlScheme.MenuActions);
        m_musicManager.SetMusic(MusicManager.MusicType.Morning);
        SetSceneActive(loadingScene.SceneName, false);
    }

    #region Private
    /// <summary>
    /// Loads a scene and adds it to the list of loaded scenes.
    /// </summary>
    /// <param name="scene">The scene to load.</param>
    private async Task LoadScene(string scene, bool addToList = true)
    {
        await SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        if (addToList) m_loadedScenes.Add(scene);
    }

    /// <summary>
    /// Unloads a scene and removes it from the list of loaded scenes.
    /// </summary>
    /// <param name="scene">The scene to unload.</param>
    private async Task UnloadScene(string scene)
    {
        await SceneManager.UnloadSceneAsync(scene);
        m_loadedScenes.Remove(scene);
    }

    /// <summary>
    /// Enables/disables all gameobjects within a scene, setting currentScene to the newly active scene.
    /// </summary>
    /// <param name="scene">The scene to disable.</param>
    private void SetSceneActive(string scene, bool active)
    {
        var currentScene = SceneManager.GetSceneByName(scene);
        if (currentScene.name == null) Debug.LogError($"{scene} is not a loaded scene!");

        var gameObjs = currentScene.GetRootGameObjects();

        foreach (var gameObject in gameObjs)
        {
            gameObject.SetActive(active);
        }

        if (active) m_currentScene = currentScene;
    }

    /// <summary>
    /// Unloads all currently active level scenes.
    /// </summary>
    private async Task UnloadActiveLevelScenes()
    {
        var tasks = new Task[m_currentLevelScenes.gameScenes.Count + 1];

        for (int i = 0; i < tasks.Length - 1; i++)
        {
            tasks[i] = UnloadScene(m_currentLevelScenes.gameScenes[i].SceneName);
        }

        tasks[tasks.Length - 1] = UnloadScene(m_currentLevelScenes.startingScene.SceneName);

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Unloads all currently loaded scenes. (Except for GameStartingScene and LoadingScene)
    /// </summary>
    /// <returns></returns>
    private async Task UnloadAllScenes()
    {
        var tasks = new Task[m_loadedScenes.Count];

        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = UnloadScene(m_loadedScenes[i]);
        }

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Loads all the bedroom scenes.
    /// </summary>
    private async Task LoadBedroomScenes()
    {
        foreach (GameScene scene in bedroomScenes)
        {
            await LoadScene(scene.SceneName);
            SetSceneActive(scene.SceneName, false);
        }
    }
    #endregion

    #region Public
    /// <summary>
    /// Swaps the current active scene to a new scene. Will fail if the target scene is not loaded.
    /// </summary>
    /// <param name="target">The scene to move to.</param>
    /// <param name="targetIsUI">Whether the target scene requires UI interaction.</param>
    public void MoveToScene(string target, bool targetIsUI = false)
    {
        OnSceneSwitch?.Invoke();

        SetSceneActive(m_currentScene.name, false);
        SetSceneActive(target, true);

        if (targetIsUI)
        {
            InputManager.Instance.SetCursorMode(false);
            InputManager.Instance.SetControlScheme(InputManager.ControlScheme.MenuActions);
        }
        else
        {
            InputManager.Instance.SetCursorMode(true);
            InputManager.Instance.SetControlScheme(InputManager.ControlScheme.PlayerActions);
        }

        OnSceneChanged?.Invoke();
    }

    /// <summary>
    /// Loads all the scenes of the given level and moves there.
    /// </summary>
    /// <param name="level">The level to load.</param>
    private async void LoadMonsterScene(Level level)
    {
        m_currentLevel = level;

        //Load the monster scenes
        m_currentLevelScenes = allLevelScenes.Find(levelScene => levelScene.level == level);
        var monsterScenes = m_currentLevelScenes.gameScenes;

        //Load the starting scene first (to manage dependencies)
        await LoadScene(m_currentLevelScenes.startingScene.SceneName);

        //Load the rest of the scenes
        var tasks = new Task[monsterScenes.Count];

        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = LoadScene(monsterScenes[i].SceneName);
        }

        await Task.WhenAll(tasks);

        OnMonsterScenesLoaded?.Invoke();

        //Set all as inactive
        for (int i = 0; i < monsterScenes.Count; i++)
        {
            SetSceneActive(monsterScenes[i].SceneName, false);
        }

        //Remove loading screen and activate first level
        MoveToScene(monsterScenes[0].SceneName);
    }

    /// <summary>
    /// Starts a level from the beginning. Intended to be run from the level select scene.
    /// </summary>
    /// <param name="level"></param>
    public async void StartNewLevel(Level level)
    {
        MoveToScene(loadingScene.SceneName);

        await UnloadAllScenes();

        m_musicManager.SetMusic(MusicManager.MusicType.Background);

        LoadMonsterScene(level);
    }

    /// <summary>
    /// Loads the score summary scene.
    /// </summary>
    public async void FinishLevel()
    {
        OnLevelEnd?.Invoke();

        MoveToScene(loadingScene.SceneName);

        SetSceneActive(m_currentLevelScenes.startingScene.SceneName, false);
        await LoadScene(scoreSummaryScene.SceneName);

        MoveToScene(scoreSummaryScene.SceneName, true);
    }

    /// <summary>
    /// Loads the death screen.
    /// </summary>
    public async void PlayerDied()
    {
        await Task.Delay(7000);
        OnLevelEnd?.Invoke();

        MoveToScene(loadingScene.SceneName);

        SetSceneActive(m_currentLevelScenes.startingScene.SceneName, false);
        await LoadScene(deathScene.SceneName);

        MoveToScene(deathScene.SceneName, true);
    }

    /// <summary>
    /// Moves to a bedroom scene, loading the bedroom if necessary.
    /// </summary>
    /// <param name="target">The bedroom scene to move to.</param>
    /// <param name="targetIsUI">Whether the target scene requires UI interaction.</param>
    public async Task GoToBedroomScene(string target, bool targetIsUI)
    {
        var lastActiveScene = m_currentScene.name;
        m_currentLevel = Level.None;
        MoveToScene(loadingScene.SceneName);

        if (!bedroomScenes.Exists(scene => scene.SceneName.Equals(lastActiveScene)))
        {
            await UnloadActiveLevelScenes();
            await UnloadScene(lastActiveScene);
            await LoadBedroomScenes();
        }

        MoveToScene(target, targetIsUI);
    }

    /// <summary>
    /// Moves to a bedroom scene while also setting the music, loading the bedroom if necessary.
    /// </summary>
    /// <param name="target">The bedroom scene to move to.</param>
    /// <param name="targetIsUI">Whether the target scene requires UI interaction.</param>
    /// <param name="music">The music to begin playing.</param>
    public async Task GoToBedroomScene(string target, bool targetIsUI, MusicManager.MusicType music)
    {
        await GoToBedroomScene(target, targetIsUI);

        m_musicManager.SetMusic(music);
    }

    /// <summary>
    /// Sets the current scene back to the first scene of the current level.
    /// </summary>
    public async void ContinueLevel()
    {
        await UnloadScene(deathScene.SceneName);
        m_loadedScenes.RemoveAt(m_loadedScenes.IndexOf(deathScene.SceneName));

        m_musicManager.SetMusic(MusicManager.MusicType.Background);

        SetSceneActive(m_currentLevelScenes.startingScene.SceneName, true);
        SetSceneActive(m_currentLevelScenes.gameScenes[0].SceneName, true);
    }

    /// <summary>
    /// Restarts the current level.
    /// </summary>
    public async void RestartLevel()
    {
        MoveToScene(loadingScene.SceneName);

        await UnloadActiveLevelScenes();

        m_musicManager.SetMusic(MusicManager.MusicType.Background);

        LoadMonsterScene(m_currentLevelScenes.level);
    }

    /// <summary>
    /// Restarts the entire game.
    /// </summary>
    public async void RestartGame()
    {
        OnRestartGame?.Invoke();

        MoveToScene(loadingScene.SceneName);

        await SceneManager.LoadSceneAsync(gameStartingScene.SceneName);
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
