using System.Collections.Generic;
using UnityEngine;

public class ToolSwitcher : MonoBehaviour
{
    [SerializeField] private List<GameObject> tools = new();
    [SerializeField] private Transform toolHolder;

    private List<GameObject> m_toolInstances = new();
    /// <summary>
    /// The current index in the m_toolInstances list. -1 represents an empty hand.
    /// </summary>
    private int m_currentToolIndex = -1;

    private void Awake()
    {
        foreach (var tool in tools)
        {
            var toolInstance = Instantiate(tool, toolHolder);
            toolInstance.SetActive(false);
            m_toolInstances.Add(toolInstance);
        }
    }

    private void OnEnable()
    {
        InputManager.Inputs.OnSwitchTool += Inputs_OnSwitchTool;
    }

    private void OnDisable()
    {
        InputManager.Inputs.OnSwitchTool -= Inputs_OnSwitchTool;
    }

    private void Inputs_OnSwitchTool()
    {
        RotateCurrentTool();
    }

    private void RotateCurrentTool()
    {
        //Increase by one, looping from -1 (empty hand) to the last tool index and back to -1
        var nextToolIndex = ((m_currentToolIndex + 2) % (tools.Count + 1)) - 1;

        SetActiveTool(nextToolIndex);
    }

    private void SetActiveTool(int toolIndex)
    {
        if (m_currentToolIndex >= 0) m_toolInstances[m_currentToolIndex].SetActive(false);

        m_currentToolIndex = toolIndex;

        if (m_currentToolIndex >= 0) m_toolInstances[m_currentToolIndex].SetActive(true);
    }
}
