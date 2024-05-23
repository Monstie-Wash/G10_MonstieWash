using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TaskTracker : MonoBehaviour
{
    //Private
    [SerializeField] private List<TaskData> m_taskData = new();
    private Dictionary<string, float> m_areaProgress = new();

    private RoomSaver m_roomSaver;
    private UIManager m_uiManager;

	private void Awake()
    {
        m_roomSaver = GetComponent<RoomSaver>();
        m_uiManager = GetComponent<UIManager>();
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
        m_roomSaver.OnScenesLoaded -= RoomSaver_OnScenesLoaded;
    }

	public void UpdateTaskTracker(TaskData task)
	{
        if (m_taskData.Contains(task))
		{


			if (task.Progress >= task.Threshold)
			{
				task.Complete = true;
				task.gameObject.SetActive(false);//Remove task here?
				m_uiManager.UpdateClipboardTask(m_taskData, task.gameObject.scene.name);
				CompletionCheck();//Check for complete here?
			}
		}
		else
		{
			Debug.Log($"{task.Id} is not being tracked! Something went wrong!");
		}
	}

	private void CompletionCheck()
    {
        foreach (var task in m_taskData)
        {
            if (!task.Complete)
                return;
        }
        Debug.Log("Finished");
    }
}
