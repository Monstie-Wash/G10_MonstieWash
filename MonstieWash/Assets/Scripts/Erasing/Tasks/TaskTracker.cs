using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TaskTracker : MonoBehaviour
{
    //Private
    private SerializableDictionary<string, float> m_taskProgress = new();
    private List<string> m_taskDictKeys = new();
    //Container object for all the cleaning activities
    private List<Transform> m_taskContainers = new();
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
        m_taskContainers.Add(transform);

        foreach (var obj in GameObject.FindGameObjectsWithTag("TaskContainer"))
        {
            m_taskContainers.Add(obj.transform);
        }

        foreach (var taskContainer in m_taskContainers)
        {
            InitialiseTasks(taskContainer, taskContainer.name);
        }
        m_uiManager.LoadKeys(m_taskDictKeys);
        m_roomSaver.OnScenesLoaded -= RoomSaver_OnScenesLoaded;
    }

    /// <summary>
    /// Recursive loop will take every child of the taskContainer object and initialize a tracker for its progress. Objects tagged with "Erasable, etc." are assumed to be end node tasks instead of task containers(groups).
    /// </summary>
    /// <param name="taskContainer">The parent object that contains all interactable tasks for the monster (mimic).</param>
    /// <param name="taskName">A compound string made up of all parent objects in the hierarchy seperated by '#' eg. "Mimic#Front#Teeth#Dirt1". Used to identify each task and the associated locations.</param>
    private void InitialiseTasks(Transform taskContainer, string taskName)
    {
        if (taskContainer.tag == "Erasable")
        {
            ITask iTask = taskContainer.GetComponent<ITask>();
            if (iTask != null)
            {
                AddTaskTracker(taskName, iTask);
            }
            else
            {
                Debug.LogWarning($"{taskContainer.name} should implmement the ITask interface but doesn't!");
            }
        }
        else
        {
            var newTaskName = "";
            if (taskContainer == transform) newTaskName = "Overall";
            else if (taskContainer.parent == null) newTaskName = $"Overall#{taskContainer.gameObject.scene.name}";
            else newTaskName = taskName;

            AddTaskGroupTracker(newTaskName);
            foreach (Transform child in taskContainer)
            {
                InitialiseTasks(child, newTaskName + "#" + child.name);
            }
        }
    }

    /// <summary>
    /// Adds a dictionary entry to track the task progress and sets TaskName for ITask scripts.
    /// </summary>
    /// <param name="taskName">A compound string that uniquely identifies the task and all parent objects in the hierachy.</param>
    /// <param name="iTask">The script that implements the ITask interface. Not required for Untagged objects.</param>
    private void AddTaskTracker(string taskName, ITask iTask)
    {
        AddTaskGroupTracker(taskName);
        if (iTask != null) iTask.TaskName = taskName;
        else Debug.LogWarning($"Warning: {taskName} does not have an ITask script!");
    }

    /// <summary>
    /// Adds a dictionary entry to track task progress of task groups(containers).
    /// </summary>
    /// <param name="taskName">A compound string that uniquely identifies the task and all parent objects in the hierachy.</param>
    private void AddTaskGroupTracker(string taskName)
    {
        if (!m_taskDictKeys.Contains(taskName))
        {
            m_taskDictKeys.Add(taskName);
            m_taskProgress.Add(taskName,0f);
        }
        else
        {
            Debug.LogWarning($"{taskName} is already being tracked.");
        }
    }

    /// <summary>
    /// Updates progress on a task given the taskName identifier and new progress.
    /// </summary>
    /// <param name="taskName">A compound string that uniquely identifies the task and all parent objects in the hierachy.</param>
    /// <param name="progress">Float for newest changes to progress since the last update.</param>
    public void UpdateTaskTracker(string taskName, float progress)
    {
        if (progress == 0f) return;
        if (m_taskDictKeys.Contains(taskName))
        {
            m_taskProgress[taskName] += progress;
            UpdateTaskGroupTracker(taskName, progress);
        }
        else
        {
            Debug.Log($"{taskName} is not being tracked! Something went wrong!");
        }
        m_uiManager.UpdateClipboardTask(taskName, m_taskProgress[taskName]);
    }

    /// <summary>
    /// Recursively updates parent task trackers using taskName identifier with relative weighting to the number of subtasks for that parent group.
    /// </summary>
    /// <param name="taskName">A compound string that uniquely identifies the task and all parent objects in the hierachy.</param>
    /// <param name="subtaskProgress">Float for new changes to progress to add to group tracker</param>
    private void UpdateTaskGroupTracker(string taskName, float subtaskProgress)
    {
        var tempSplit = taskName.Split('#');

        if (tempSplit.Length > 1)
        {
            var taskGroup = String.Join("#", tempSplit.Take(tempSplit.Length - 1));
            var subtasks = m_taskDictKeys.FindAll(s => s.Contains(taskGroup) && s.Count(x => x == '#') == taskGroup.Count(x => x == '#') + 1);
            UpdateTaskTracker(taskGroup, subtaskProgress / subtasks.Count);
        }
    }
}
