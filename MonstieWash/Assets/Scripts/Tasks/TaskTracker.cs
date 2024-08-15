using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SoundPlayer))]
public class TaskTracker : MonoBehaviour
{
    public event Action OnSceneCompleted;
    public event Action OnLevelCompleted;

    //Private
    [SerializeField] private List<TaskData> m_taskData = new();
    private Dictionary<Scene, bool> m_scenesCompleted = new();

    private GameSceneManager m_roomSaver;
    private UIManager m_uiManager;
    private SoundPlayer m_soundPlayer;
    private MusicManager m_musicManager;

	public List<TaskData> TaskData { get => m_taskData; }

	private void Awake()
    {
        m_roomSaver = FindFirstObjectByType<GameSceneManager>();
        m_uiManager = FindFirstObjectByType<UIManager>();
        m_soundPlayer = GetComponent<SoundPlayer>();
        m_musicManager = FindFirstObjectByType<MusicManager>();
    }

    private void OnEnable()
    {
        m_roomSaver.OnMonsterScenesLoaded += RoomSaver_OnScenesLoaded;
        m_roomSaver.OnSceneChanged += UpdateUI;
    }

    private void OnDisable()
    {
        m_roomSaver.OnMonsterScenesLoaded -= RoomSaver_OnScenesLoaded;
        m_roomSaver.OnSceneChanged += UpdateUI;
    }

    private void RoomSaver_OnScenesLoaded()
    {
        foreach (var obj in FindObjectsByType<TaskData>(FindObjectsSortMode.None))
        {
            m_taskData.Add(obj);
        }

        m_uiManager.LoadOverallTasks(m_taskData);

        foreach (var task in m_taskData)
        {
            var taskScene = task.gameObject.scene;
            if (!m_scenesCompleted.ContainsKey(taskScene)) m_scenesCompleted.Add(taskScene, false);
        }

        m_roomSaver.OnMonsterScenesLoaded -= RoomSaver_OnScenesLoaded;
    }

    /// <summary>
    /// Checks if the given task is complete.
    /// </summary>
    /// <param name="task">The task to check.</param>
	public void UpdateTaskTracker(TaskData task)
	{
        if (!m_taskData.Contains(task))
        {
            Debug.LogWarning($"{task.Id} is not being tracked! Something went wrong!");
            return;
        }

        UpdateUI();

        if (task.Progress < task.Threshold) return;

        // Task over threshold; complete!
        task.Complete = true;

        // Needed to comment out the below line to keep bone picking tasks working. Will figure out a way to make dirt disappear later.
        //task.gameObject.SetActive(false); // Remove task here?

        SceneCompletionCheck(task.gameObject.scene);
	}

    /// <summary>
    /// Check if the current scene (task) has been completed.
    /// </summary>
    /// <param name="scene"></param>
    private void SceneCompletionCheck(Scene scene)
    {
        if (m_scenesCompleted[scene]) return; // Exit early if scene is already noted as complete

        foreach (var task in m_taskData.Where(task => task.gameObject.scene == scene))
        {
            if (!task.Complete)
                return;
        }

        //Scene Complete!
        OnSceneCompleted?.Invoke();

        m_scenesCompleted[scene] = true;
        m_uiManager.UpdateClipboardTask(scene.name);
        m_soundPlayer.PlaySound(true, true);

        LevelCompletionCheck();
    }

    /// <summary>
    /// Check if the entire level has been completed.
    /// </summary>
	private void LevelCompletionCheck()
    {
        foreach (var task in m_taskData)
        {
            if (!task.Complete)
                return;
        }

        //Level Complete!
        OnLevelCompleted?.Invoke();

        m_musicManager.SetMusic(MusicManager.MusicType.Victory);
    }

    /// <summary>
    /// Updates the completion amounts in the UI
    /// </summary>
    private void UpdateUI()
    {
        var overallCompletion = CalculateCompletionPercentage(m_roomSaver.CurrentScene);
        m_uiManager.UpdateCompletion(overallCompletion);
        m_uiManager.UpdateProgressBar(overallCompletion);
    }

    /// <summary>
    /// Calculates how much of a given scene's tasks have been completed.
    /// </summary>
    /// <param name="scene">The scene in which to check for completion.</param>
    /// <returns>The overall completion percentage.</returns>
    private float CalculateCompletionPercentage(Scene scene)
    {
        var numOfTasks = 0;
        var sumOfTasks = 0f;

        foreach (var sceneTask in m_taskData.Where(t => t.gameObject.scene == scene))
        {
            numOfTasks++;
            sumOfTasks += sceneTask.Progress;
        }

        return sumOfTasks / numOfTasks / 100f;
    }
}
