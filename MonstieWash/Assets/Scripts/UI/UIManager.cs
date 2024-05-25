using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image m_clipboard;
    [SerializeField] private Animator m_CBAnimator;
    [SerializeField] private GameObject m_taskContainer;
    [SerializeField] private GameObject m_taskTextPrefab;
    [SerializeField] private float m_fontSize = 36f;

    private Dictionary<string, bool> m_taskList = new();

    private RoomSaver m_roomSaver;
    private bool m_UIVisible = true;

    private void Awake()
    {
        m_roomSaver = GetComponent<RoomSaver>();
    }

    private void OnEnable()
    {
        InputManager.Inputs.OnToggleUI += Inputs_OnToggleUI;
    }

    private void OnDisable()
    {
        InputManager.Inputs.OnToggleUI -= Inputs_OnToggleUI;
    }

    private void Inputs_OnToggleUI()
    {
        ToggleUIVisibility();
    }
    private void ToggleUIVisibility()
    {
        m_UIVisible = !m_UIVisible;
        m_CBAnimator.SetBool("Hide", m_UIVisible);
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
            var newTaskObject = Instantiate(m_taskTextPrefab, m_taskContainer.transform);
            newTaskObject.name = pair.Key;

            var newTaskText = newTaskObject.GetComponent<TextMeshProUGUI>();
            newTaskText.fontSize = m_fontSize;
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
        m_taskContainer.transform.Find(scene).GetComponent<TextMeshProUGUI>().text = $"<s>{scene}</s>";
    }
}
