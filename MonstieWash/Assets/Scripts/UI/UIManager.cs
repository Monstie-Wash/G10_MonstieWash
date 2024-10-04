using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject taskContainer;
    [SerializeField] private GameObject taskTextPrefab;
    [SerializeField] private float fontSize = 36f;
    [SerializeField] private float fontBuffer = 1.25f;
	[Space(20), SerializeField] private GameObject finishLevelButton;

    [Space(20), SerializeField] private GameObject startList;
    [SerializeField] private Animator taskListAnim;
    [SerializeField] private Button hideTL;

    private Dictionary<TaskType, bool> m_taskList = new();

    private TaskTracker m_taskTracker;

    public static event Action OnTaskHide;

    private void Awake()
    {
        m_taskTracker = FindFirstObjectByType<TaskTracker>();

        Time.timeScale = 0f;
    }

    private void OnEnable()
    {
        //InputManager.Instance.OnToggleUI += Inputs_OnToggleUI;
        m_taskTracker.OnLevelCompleted += OnLevelCompleted;
        m_taskTracker.OnSceneCompleted += OnSceneCompleted;
    }

    private void OnDisable()
    {
        //InputManager.Instance.OnToggleUI -= Inputs_OnToggleUI;
        m_taskTracker.OnLevelCompleted -= OnLevelCompleted;
        m_taskTracker.OnSceneCompleted -= OnSceneCompleted;
    }

    private void ToggleUIVisibility(Animator animator)
    {
        var clipboardHidden = animator.GetBool("Hide");
        animator.SetBool("Hide", !clipboardHidden);
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

        InitialiseStartList();
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
        Time.timeScale = 1.0f;
        OnTaskHide?.Invoke();
    }
}
