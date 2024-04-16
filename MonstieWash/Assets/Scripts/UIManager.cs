using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI Text;
    [SerializeField] public Image Clipboard;
    [SerializeField] public Animator CBAnimator;

    private Dictionary<string, float> m_Tasks;
    private bool m_UIVisible;
    // Start is called before the first frame update
    private void Awake()
    {
        m_Tasks = gameObject.GetComponent<TaskTracker>().m_taskProgress;
        InputManager.Inputs.OnToggleUI += Inputs_OnToggleUI;
    }

    private void Inputs_OnToggleUI()
    {
        ToggleUIVisibility();
    }

    // Update is called once per frame
    private void Update()
    {
        string output = "";

        foreach (var task in m_Tasks)
        {
            //Task names need to be redone for readability, but at least it's currently functional.
            output += task.Key + ": " + Math.Round(task.Value, 2) + "%\n";
        }

        Text.text = output;
    }

    private void ToggleUIVisibility()
    {
        //Clipboard.gameObject.SetActive(!Clipboard.gameObject.activeSelf);
        m_UIVisible = !m_UIVisible;
        CBAnimator.SetBool("Hide", m_UIVisible);
    }
}
