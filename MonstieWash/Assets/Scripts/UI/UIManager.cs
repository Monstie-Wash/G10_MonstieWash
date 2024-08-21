using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image clipboard;
    [SerializeField] private Animator CBAnimator;
    [SerializeField] private GameObject taskContainer;
    [SerializeField] private GameObject taskTextPrefab;
    [SerializeField] private TextMeshProUGUI completionText;
    [SerializeField] private float fontSize = 36f;
    [Space(20), SerializeField] private GameObject finishLevelButton;

    private Dictionary<string, bool> m_taskList = new();

    private GameSceneManager m_roomSaver;
    private TaskTracker m_taskTracker;
    private ProgressBarUI[] m_progressBars; // Array is overhead if we have multiple progress bars to track scene vs total completion
    private bool m_UIInvisible = false;

    private void Awake()
    {
        m_roomSaver = FindFirstObjectByType<GameSceneManager>();
        m_taskTracker = FindFirstObjectByType<TaskTracker>();
        m_progressBars = FindObjectsByType<ProgressBarUI>(FindObjectsSortMode.None);
    }

    private void OnEnable()
    {
        InputManager.Instance.OnToggleUI += Inputs_OnToggleUI;
        m_taskTracker.OnLevelCompleted += OnLevelCompleted;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnToggleUI -= Inputs_OnToggleUI;
        m_taskTracker.OnLevelCompleted -= OnLevelCompleted;
    }

    private void Inputs_OnToggleUI()
    {
        ToggleUIVisibility();
    }
    private void ToggleUIVisibility()
    {
        m_UIInvisible = !m_UIInvisible;
        CBAnimator.SetBool("Hide", m_UIInvisible);
    }

/// <summary>
/// Public method used by TaskTracker to initialise clipboard after all task trackers have been initialised. Creates filters for tasks to show on each scene.
/// </summary>
/// <param name="keys">A list of all the keys used to identify tasks tracked in TaskTracker</param>
    public void LoadOverallTasks(List<TaskData> tasks)
    {
        foreach (var task in tasks)
        {
            if (!m_taskList.ContainsKey(task.gameObject.scene.name))
            {
                m_taskList.Add(task.gameObject.scene.name, false);
            }
        }

        InitialiseClipboard();
    }

/// <summary>
/// Recursively creates objects inside the clipboard object for each layer of tasks across all monster scenes. 
/// </summary>
/// <param name=""></param>
    private void InitialiseClipboard()
    {
        foreach (var pair in m_taskList)
        {
            var newTaskObject = Instantiate(taskTextPrefab, taskContainer.transform);
            newTaskObject.name = pair.Key;

            var newTaskText = newTaskObject.GetComponent<TextMeshProUGUI>();
            newTaskText.fontSize = fontSize;
            newTaskText.text = $"{newTaskObject.name}";
        } 
    }

/// <summary>
/// Updates the progress value displayed on the clipboard for a given task.
/// </summary>
/// <param name="taskName">String identifier of the task to update.</param>
/// <param name="taskProgress">Progress of the task to be displayed on the clipboard.</param>
    public void UpdateClipboardTask(string scene)
    {
        taskContainer.transform.Find(scene).GetComponent<TextMeshProUGUI>().text = $"<s>{scene}</s>";
    }

	/// <summary>
	/// Updates the progress bar based on completion percentage.
	/// </summary>
    public void UpdateProgressBar(float overallCompletion)
    {
        foreach(var progressBar in m_progressBars)
        {
            progressBar.UpdateUI(overallCompletion);
        }
	}
	
    /// <summary>
    /// Updates the completion percentage on the clipboard for the current scene.
    /// </summary>
    /// <param name="overallCompletion">The overall completion to display.</param>
    public void UpdateCompletion(float overallCompletion)
    {
        completionText.text = $"{Mathf.CeilToInt(overallCompletion * 100f)}%";
    }

    private void OnLevelCompleted()
    {
        finishLevelButton.SetActive(true);
    }
}
