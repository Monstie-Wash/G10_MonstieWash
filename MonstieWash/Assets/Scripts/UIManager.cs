using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject m_TaskContainer;
    public GameObject m_TaskTextPrefab;

    [SerializeField] private Image Clipboard;
    [SerializeField] private Animator CBAnimator;
    [SerializeField] private float fontSize = 36f;
    [SerializeField] private float fontScaling = 0.75f;
    [SerializeField] private float paddingScaling = 1.2f;
    [SerializeField] private List<string> m_TaskKeys = new();
    
    private Dictionary<string, List<string>> m_SceneTasks = new();
    private RoomSaver m_roomSaver;
    private bool m_UIVisible;
    // Start is called before the first frame update
    private void Awake()
    {
        m_roomSaver = GetComponent<RoomSaver>();
        InputManager.Inputs.OnToggleUI += Inputs_OnToggleUI;
    }

    private void OnEnable()
    {
        m_roomSaver.OnSceneChanged += UpdateClipboardUI;
    }

    private void OnDisable()
    {
        m_roomSaver.OnSceneChanged -= UpdateClipboardUI;
    }

    private void Inputs_OnToggleUI()
    {
        ToggleUIVisibility();
    }

/// <summary>
/// Public method used by TaskTracker to initialise clipboard after all task trackers have been initialised. Creates filters for tasks to show on each scene.
/// </summary>
/// <param name="keys">A list of all the keys used to identify tasks tracked in TaskTracker</param>
    public void LoadKeys(List<string> keys)
    {
        m_TaskKeys = keys;
        foreach (var scene in m_roomSaver.m_allScenes)
        {
            switch (scene)
            {
                case "Overview":
                    m_SceneTasks.Add(scene, m_TaskKeys.FindAll(s => s.Contains("Overall") && s.Count(x => x == '#') <= 1));
                    break;
                default:
                    var sceneKeys = m_TaskKeys.FindAll(s => s.Contains(scene));
                    sceneKeys.Add("Overall");
                    m_SceneTasks.Add(scene, sceneKeys);
                    break;
            } 
        }

        InitialiseClipboard(m_TaskContainer, "", 0);
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
        var currentTaskLayer = m_TaskKeys.FindAll(s => s.Contains(currentParentName) && s.Count(x => x == '#') == taskLayer);
        foreach (var task in currentTaskLayer)
        {
            var newTaskObject = GameObject.Instantiate(m_TaskTextPrefab, currentParent.transform);

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

    private void ToggleUIVisibility()
    {
        //Clipboard.gameObject.SetActive(!Clipboard.gameObject.activeSelf);
        m_UIVisible = !m_UIVisible;
        CBAnimator.SetBool("Hide", m_UIVisible);
    }

/// <summary>
/// Updates the clipboard to hide irrelevant tasks using a filter for the loaded scene.
/// </summary>
/// <param name="sceneName">Name of the scene currently loaded.</param>
    private void UpdateClipboardUI(string sceneName)
    {
        foreach (var textObject in m_TaskContainer.GetComponentsInChildren<Transform>())
        {
            if(textObject != m_TaskContainer.transform) textObject.gameObject.SetActive(false);
        }

        foreach (var taskName in m_SceneTasks[sceneName])
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
        return m_TaskContainer.transform.Find(tempString).gameObject;
    }
}
