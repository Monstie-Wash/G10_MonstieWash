using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TaskTracker : MonoBehaviour
{
    //Unity Inspector
    //Container object for all the the different areas to clean 
    [SerializeField] private Transform taskContainer;
    //These are temporary replacement for UI progress bar
    //[SerializeField] private List<string> subtasks = new List<string>();
    //[SerializeField] private string[] tempSplit;
    //[SerializeField] private string taskString;
    [SerializeField] private SerializableDictionary<string, float> m_taskProgress = new SerializableDictionary<string, float>();

    //Private
    private List<string> m_taskDictKeys = new List<string>();
    
    private void Awake()
    {
        if(taskContainer)
        {
            InitialiseTasks(taskContainer, taskContainer.name);
        }
        else
        {
            Debug.Log("WARNING: Please assign taskContainer to TaskTracker script!!!");
        }
    }

    private void InitialiseTasks(Transform taskContainer, string taskName)
    {
        if (taskContainer.tag == "Untagged")
        {
            AddTaskGroupTracker(taskName);
            foreach (Transform child in taskContainer)
            {
                InitialiseTasks(child, taskName + "#" + child.name);
            }
        }
        else
        {
            ITask iTask = taskContainer.GetComponent<ITask>();
            if(iTask != null) 
            {
                AddTaskTracker(taskName, iTask);
            }
            else
            {
                Debug.Log($"{taskContainer.name} should implmement the ITask interface but doesn't!");
            }
        }
    }

    private void AddTaskTracker(string taskName, ITask iTask)
    {
        AddTaskGroupTracker(taskName);
        if(iTask != null) iTask.TaskName = taskName;
    }

    private void AddTaskGroupTracker(string taskName)
    {
        if(!m_taskDictKeys.Contains(taskName))
        {
            m_taskDictKeys.Add(taskName);
            m_taskProgress.Add(taskName,0f);
        }
        else
        {
            Debug.Log($"{taskName} is already being tracked.");
        }
    }

    public void UpdateTaskTracker(string taskName, float progress)
    {
        if(m_taskDictKeys.Contains(taskName))
        {
            m_taskProgress[taskName] += progress;
            UpdateTaskGroupTracker(taskName, progress);
        }
        else
        {
            Debug.Log($"{taskName} is not being tracked! Something went wrong!");
        }
    }

    private void UpdateTaskGroupTracker(string taskName, float subtaskProgress)
    {
        var tempSplit = taskName.Split('#');

        if(tempSplit.Length > 1)
        {
            var taskGroup = String.Join("#", tempSplit.Take(tempSplit.Length - 1));
            var subtasks = m_taskDictKeys.FindAll(s => s.Contains(taskGroup) && s.Count(x => x == '#') == taskGroup.Count(x => x == '#') + 1);
            UpdateTaskTracker(taskGroup, subtaskProgress / subtasks.Count);
        }
    }
}
