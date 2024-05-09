using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image clipboard;
    [SerializeField] private Animator CBAnimator;

    private Dictionary<string, float> m_tasks;
    private bool m_UIVisible;
    // Start is called before the first frame update
    private void OnEnable()
    {
        m_tasks = gameObject.GetComponent<TaskTracker>().m_taskProgress;
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
        //Clipboard.gameObject.SetActive(!Clipboard.gameObject.activeSelf);
        m_UIVisible = !m_UIVisible;
        CBAnimator.SetBool("Hide", m_UIVisible);
    }

    // Update is called once per frame
    private void Update()
    {
        string output = "";

        foreach (var task in m_tasks)
        {
            //Task names need to be redone for readability, but at least it's currently functional.
            output += $"{task.Key}: {Math.Round(task.Value, 2)}%\n";
        }

        text.text = output;
    }

    
}
