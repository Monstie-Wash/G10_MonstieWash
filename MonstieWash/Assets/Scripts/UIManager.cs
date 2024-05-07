using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject taskContainer;
    [SerializeField] private GameObject taskTextPrefab;
    [SerializeField] private Image clipboard;
    [SerializeField] private Animator CBAnimator;
    [SerializeField] private float fontSize = 36f;
    [SerializeField] [Range(0.5f, 1.0f)] private float fontScaling = 0.75f;
    [SerializeField] [Range(0.0f, 2.0f)]private float paddingScaling = 1.2f;
    [SerializeField] private List<string> taskKeys = new();
    
    private Dictionary<string, List<string>> m_sceneTasks = new();
    private RoomSaver m_roomSaver;
    private bool m_UIVisible = true;

    private void Awake()
    {
        m_roomSaver = GetComponent<RoomSaver>();
    }

    private void OnEnable()
    {
        InputManager.Inputs.OnToggleUI += Inputs_OnToggleUI;
        m_roomSaver.OnSceneChanged += UpdateClipboardUI;
    }

    private void OnDisable()
    {
        InputManager.Inputs.OnToggleUI -= Inputs_OnToggleUI;
        m_roomSaver.OnSceneChanged -= UpdateClipboardUI;
    }

    private void Inputs_OnToggleUI()
    {
        ToggleUIVisibility();
    }
    private void ToggleUIVisibility()
    {
        //Clipboard.gameObject.SetActive(!Clipboard.gameObject.activeSelf);
        m_UIVisible = !m_UIVisible;
        CBAnimator.SetBool("Hide", m_UIVisible);
    }

/// <summary>
/// Public method used by TaskTracker to initialise clipboard after all task trackers have been initialised. Creates filters for tasks to show on each scene.
/// </summary>
/// <param name="keys">A list of all the keys used to identify tasks tracked in TaskTracker</param>
    public void LoadKeys(List<string> keys)
    {
        taskKeys = keys;
        foreach (var scene in m_roomSaver.AllScenes)
        {
            if (scene == "Overview")
            {
                m_sceneTasks.Add(scene, taskKeys.FindAll(s => s.Contains("Overall") && s.Count(x => x == '#') <= 1));
            }
            else
            {
                var sceneKeys = taskKeys.FindAll(s => s.Contains(scene));
                sceneKeys.Add("Overall");
                m_sceneTasks.Add(scene, sceneKeys);
            }
        }
        InitialiseClipboard(taskContainer, "", 0);
        UpdateClipboardUI("Overview");
    }

/// <summary>
/// Recursively creates objects inside the clipboard object for each layer of tasks across all monster scenes. 
/// </summary>
/// <param name="currentParent">The parent object of the layer of tasks current being initialised.</param>
/// <param name="currentParentName">The name identifier of the current parent object.</param>
/// <param name="taskLayer">The current depth of the layer of tasks being processed.</param>
    private void InitialiseClipboard(GameObject currentParent, string currentParentName, int taskLayer)
    {
        var currentTaskLayer = taskKeys.FindAll(s => s.Contains(currentParentName) && s.Count(x => x == '#') == taskLayer);
        foreach (var task in currentTaskLayer)
        {
            var newTaskObject = Instantiate(taskTextPrefab, currentParent.transform);

            if (!task.Contains('#'))
            {
                newTaskObject.name = task;
            }
            else
            {
                var tempSplit = task.Split('#');
                newTaskObject.name = tempSplit[taskLayer];
            }
            var newTaskText = newTaskObject.GetComponent<TextMeshProUGUI>();
            newTaskText.fontSize = fontSize * Mathf.Pow(fontScaling, taskLayer);
            newTaskText.text = $"{newTaskObject.name}: 0%";
            InitialiseClipboard(newTaskObject, currentParentName + newTaskObject.name + "#", taskLayer + 1);
        } 
    }

/// <summary>
/// Updates the clipboard to hide irrelevant tasks using a filter for the loaded scene.
/// </summary>
/// <param name="sceneName">Name of the scene currently loaded.</param>
    private void UpdateClipboardUI(string sceneName)
    {
        foreach (var textObject in taskContainer.GetComponentsInChildren<Transform>())
        {
            if(textObject != taskContainer.transform) textObject.gameObject.SetActive(false);
        }

        foreach (var taskName in m_sceneTasks[sceneName])
        {
            var taskObject = FindTaskObject(taskName);

            taskObject.SetActive(true);

            var layoutGroup = taskObject.GetComponent<LayoutGroup>();
            var textUI = taskObject.GetComponent<TextMeshProUGUI>();
            if (layoutGroup != null && textUI != null) layoutGroup.padding.top = (int) (textUI.fontSize * paddingScaling);
        }
    }

/// <summary>
/// Updates the progress value displayed on the clipboard for a given task.
/// </summary>
/// <param name="taskName">String identifier of the task to update.</param>
/// <param name="taskProgress">Progress of the task to be displayed on the clipboard.</param>
    public void UpdateClipboardTask(string taskName, float taskProgress)
    {
        var taskObject = FindTaskObject(taskName);
        var taskText = taskObject.GetComponent<TextMeshProUGUI>();

        taskText.text = $"{taskObject.name}: {Math.Round(taskProgress, 0)}%"; // Rounding up with no decimals makes the display show 100% even with small amounts of dirt remaining
    }

/// <summary>
/// Finds the corresponding task object using the string identifier by diving through the unity hierarchy.
/// </summary>
/// <param name="taskName">String identifier for the task to find</param>
/// <returns>Object corresponding to the task.</returns>
    private GameObject FindTaskObject(string taskName)
    {
        var tempSplit = taskName.Split('#');
        var tempString = String.Join("/", tempSplit);
        return taskContainer.transform.Find(tempString).gameObject;
    }
}
