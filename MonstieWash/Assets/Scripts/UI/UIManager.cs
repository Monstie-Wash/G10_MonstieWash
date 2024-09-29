using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image clipboard;
    [SerializeField] private bool autoHideClipboard = true;
    [SerializeField] private float clipboardAutoHideDelay = 5f;
    [SerializeField] private Animator CBAnimator;
    [SerializeField] private GameObject taskContainer;
    [SerializeField] private GameObject taskTextPrefab;
    [SerializeField] private TextMeshProUGUI completionText;
    [SerializeField] private float fontSize = 36f;
    [SerializeField] private float fontBuffer = 1.25f;
	[Space(20), SerializeField] private GameObject finishLevelButton;

    [Space(20), SerializeField] private GameObject startList;
    [SerializeField] private Animator taskListAnim;
    [SerializeField] private Button hideTL;

    private Dictionary<TaskType, bool> m_taskList = new();

    private TaskTracker m_taskTracker;
    private ProgressBarUI[] m_progressBars; // Array is overhead if we have multiple progress bars to track scene vs total completion
    private Coroutine m_clipboardAutoHide;

    private void Awake()
    {
        m_taskTracker = FindFirstObjectByType<TaskTracker>();
        m_progressBars = FindObjectsByType<ProgressBarUI>(FindObjectsSortMode.None);

        Time.timeScale = 0f;
    }

    private void OnEnable()
    {
        InputManager.Instance.OnToggleUI += Inputs_OnToggleUI;
        m_taskTracker.OnLevelCompleted += OnLevelCompleted;
        m_taskTracker.OnSceneCompleted += OnSceneCompleted;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnToggleUI -= Inputs_OnToggleUI;
        m_taskTracker.OnLevelCompleted -= OnLevelCompleted;
        m_taskTracker.OnSceneCompleted -= OnSceneCompleted;
    }

    private void Inputs_OnToggleUI()
    {
        ToggleUIVisibility(CBAnimator);
    }
    private void ToggleUIVisibility(Animator animator)
    {
        var clipboardHidden = animator.GetBool("Hide");
        animator.SetBool("Hide", !clipboardHidden);

        if (!autoHideClipboard) return;
        if (clipboardHidden) m_clipboardAutoHide = StartCoroutine(ClipboardHideTimer());
        else StopCoroutine(m_clipboardAutoHide);
    }

    private IEnumerator ClipboardHideTimer()
    {
        yield return new WaitForSeconds(clipboardAutoHideDelay);
        ToggleUIVisibility(CBAnimator);
    }

/// <summary>
/// Public method used by TaskTracker to initialise clipboard after all task trackers have been initialised. Creates filters for tasks to show on each scene.
/// </summary>
/// <param name="keys">A list of all the keys used to identify tasks tracked in TaskTracker</param>
    public void LoadOverallTasks(TaskData[] tasks)
    {
        foreach (var task in tasks)
        {
            if (!m_taskList.ContainsKey(task.TaskType))
            {
                m_taskList.Add(task.TaskType, false);
            }
        }

        InitialiseClipboard();
        InitialiseStartList();
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
            
            newTaskObject.name = pair.Key.ToString();

            var newTaskText = newTaskObject.GetComponent<TextMeshProUGUI>();
            newTaskText.fontSize = fontSize;
            newTaskText.text = Resources.Load<TaskDesc>(newTaskObject.name).description;
        } 
    }

    private void InitialiseStartList()
    {
		foreach (var pair in m_taskList)
		{
            var newTaskObject = Instantiate(taskTextPrefab, startList.transform);

			newTaskObject.name = pair.Key.ToString();

			var newTaskText = newTaskObject.GetComponent<TextMeshProUGUI>();
			newTaskText.fontSize = fontSize * fontBuffer;
			newTaskText.text = Resources.Load<TaskDesc>(newTaskObject.name).description;
		}
	}

/// <summary>
/// Updates the progress value displayed on the clipboard for a given task.
/// </summary>
/// <param name="scene">String identifier of the task to update.</param>
    public void UpdateClipboardTask(TaskType type)
    {
		TextMeshProUGUI taskText = taskContainer.transform.Find(type.ToString()).GetComponent<TextMeshProUGUI>();
		taskText.text = $"<s>{taskText.text}</s>";
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
        var uncleanButtons = FindObjectsByType<NextUncleanButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var button in uncleanButtons)
        {
            GameSceneManager.Instance.SetObjectActiveState(button.gameObject.GetHashCode(), false);
            button.gameObject.SetActive(false);
        }

        finishLevelButton.SetActive(true);
    }

    private void OnSceneCompleted(string scene)
    {
        if (GameSceneManager.Instance.CurrentScene.name.Equals(scene))
        {
            var uncleanButtons = FindObjectsByType<NextUncleanButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            var button = Array.Find(uncleanButtons, button => button.gameObject.scene.name.Equals(scene));
            button.gameObject.SetActive(true);
        }
    }

    public void OnTaskListHide()
    {
		ToggleUIVisibility(taskListAnim);
		ToggleUIVisibility(CBAnimator);
        Time.timeScale = 1.0f;
    }
}
