using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SoundPlayer))]
public class TaskTracker : MonoBehaviour
{
    public event Action<string> OnSceneCompleted;
    public event Action OnLevelCompleted;

    //Private
    [SerializeField] private List<TaskData> m_taskData = new();
    private Dictionary<Scene, bool> m_scenesCompleted = new();

    private UIManager m_uiManager;
    private SoundPlayer m_soundPlayer;

	public List<TaskData> TaskData { get => m_taskData; }
    public Dictionary<Scene, bool> ScenesCompleted { get => m_scenesCompleted; }

	private void Awake()
    {
        m_uiManager = FindFirstObjectByType<UIManager>();
        m_soundPlayer = GetComponent<SoundPlayer>();
    }

    public void Initialize()
    {
        AddTasks(FindObjectsByType<TaskData>(FindObjectsSortMode.None));

        m_uiManager.LoadOverallTasks(m_taskData.ToArray());

        foreach (var task in m_taskData)
        {
            var taskScene = task.gameObject.scene;
            if (!m_scenesCompleted.ContainsKey(taskScene)) m_scenesCompleted.Add(taskScene, false);
        }
    }

    public void AddTasks(TaskData[] tasks)
    {
        foreach (var obj in tasks)
        {
            m_taskData.Add(obj);
        }
    }

    public void RemoveTasks(TaskData[] tasks)
    {
        foreach (var obj in tasks)
        { 
            m_taskData.Remove(obj);
        }
    }

    /// <summary>
    /// Checks if the given task is complete.
    /// </summary>
    /// <param name="task">The task to check.</param>
	public void UpdateTaskTracker(TaskData task)
	{
        if (!m_taskData.Contains(task))
        {
            Debug.LogWarning($"{task.Name} is not being tracked! Something went wrong!");
            return;
        }

        if (!task.Complete) return;

        SceneCompletionCheck(task.gameObject.scene);
        TaskTypeCompletionCheck(task);
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
        OnSceneCompleted?.Invoke(scene.name);

        m_scenesCompleted[scene] = true;
        m_soundPlayer.PlaySound(true, true);

        LevelCompletionCheck();
    }

    private void TaskTypeCompletionCheck(TaskData toCheck)
    {
        foreach (var task in m_taskData.Where(task => task.TaskType == toCheck.TaskType))
        {
            if(!task.Complete)
                return;
        }
	}

    /// <summary>
    /// Return whether a scene exists in the dict of completed scenes, without requiring knowledge of other scenes.
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    public bool IsThisSceneComplete()
    {
        if (m_scenesCompleted.ContainsKey(GameSceneManager.Instance.CurrentScene))   // If the scene exists in dict
        {
            return m_scenesCompleted[GameSceneManager.Instance.CurrentScene];    // Return the value of the scene
        }
        return false;    // Scene is one that has no tasks (title screen, etc.) - return false by default
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

        MusicManager.Instance.SetMusic(MusicManager.MusicType.Victory);
    }

    /// <summary>
    /// Calculates how much of a given scene's tasks have been completed.
    /// </summary>
    /// <param name="scene">The scene in which to check for completion.</param>
    /// <returns>The overall completion percentage.</returns>
    private float CalculateCurrentSceneCompletionPercentage(Scene scene)
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

    private float CalculateOverallCompletionPercentage()
    {
        var numOfTasks = 0;
        var sumOfTasks = 0f;

        foreach (var task in m_taskData)
        {
            numOfTasks++;
            sumOfTasks += task.Progress;
        }

        return sumOfTasks / numOfTasks / 100f;
    }
}
