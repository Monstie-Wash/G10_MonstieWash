using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TaskTracker : MonoBehaviour
{
    public event Action OnSceneCompleted;
    public event Action OnLevelCompleted;

    //Private
    [SerializeField] private List<TaskData> m_taskData = new();
    private Dictionary<string, float> m_areaProgress = new();
    private Dictionary<string, bool> m_scenesCompleted = new();

    private RoomSaver m_roomSaver;
    private UIManager m_uiManager;
    private SoundPlayer m_soundPlayer;
    private MusicManager m_musicManager;

	private void Awake()
    {
        m_roomSaver = GetComponent<RoomSaver>();
        m_uiManager = GetComponent<UIManager>();
        m_soundPlayer = GetComponent<SoundPlayer>();
        m_musicManager = GetComponentInChildren<MusicManager>();
    }

    private void OnEnable()
    {
        m_roomSaver.OnScenesLoaded += RoomSaver_OnScenesLoaded;
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
            var taskScene = task.gameObject.scene.name;
            if (!m_scenesCompleted.ContainsKey(taskScene)) m_scenesCompleted.Add(taskScene, false);
        }

        m_roomSaver.OnScenesLoaded -= RoomSaver_OnScenesLoaded;
    }

	public void UpdateTaskTracker(TaskData task)
	{
        if (!m_taskData.Contains(task))
        {
            Debug.LogWarning($"{task.Id} is not being tracked! Something went wrong!");
            return;
        }

        if (task.Progress < task.Threshold) return;

        // Task over threshold; complete!
		task.Complete = true;

        // Needed to comment out the below line to keep bone picking tasks working. Will figure out a way to make dirt disappear later.
        //task.gameObject.SetActive(false); // Remove task here?

        SceneCompletionCheck(task.gameObject.scene.name);
	}

    private void SceneCompletionCheck(string scene)
    {
        if (m_scenesCompleted[scene]) return; // Exit early if scene is already noted as complete

        foreach (var task in m_taskData.Where(task => task.gameObject.scene.name == scene))
        {
            if (!task.Complete)
                return;
        }

        //Scene Complete!
        OnSceneCompleted?.Invoke();

        m_scenesCompleted[scene] = true;
        m_uiManager.UpdateClipboardTask(scene);
        m_soundPlayer.PlaySound(true, true);

        LevelCompletionCheck();
    }

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
}
