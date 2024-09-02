using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolSwitcher : MonoBehaviour
{
    [SerializeField] private List<GameObject> tools = new();
    [SerializeField] private Transform toolHolder;

    private List<GameObject> m_toolInstances = new();

    public event Action OnSwitchTool;  // The player switched the tool in their hand

    /// <summary>
    /// The current index in the m_toolInstances list. -1 represents an empty hand.
    /// </summary>
    private int m_currentToolIndex = -1;

    public int CurrentToolIndex { get { return m_currentToolIndex; } }

    public List<GameObject> ToolInstances { get { return m_toolInstances; } }

    private void Awake()
    {
        foreach (var tool in tools)
        {
            var toolInstance = Instantiate(tool, toolHolder);
            m_toolInstances.Add(toolInstance);
        }
    }

    private void OnEnable()
    {
        InputManager.Instance.OnSwitchTool += Inputs_OnSwitchTool;
        GameSceneManager.Instance.OnMonsterScenesLoaded += RoomSaver_OnScenesLoaded;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnSwitchTool -= Inputs_OnSwitchTool;
        GameSceneManager.Instance.OnMonsterScenesLoaded -= RoomSaver_OnScenesLoaded;
    }

    private void RoomSaver_OnScenesLoaded()
    {
        foreach (var tool in m_toolInstances)
        {
            tool.GetComponent<Eraser>().InitializeTool();
            tool.SetActive(false);
        }
    }
    
    private void Inputs_OnSwitchTool(int dirInput)
    {
        if (dirInput == 0) return;

        RotateCurrentTool(dirInput);
    }

    /// <summary>
    /// Changes the current tool based on the direction input.
    /// </summary>
    /// <param name="dir">The direction to rotate through the tools.</param>
    private void RotateCurrentTool(int dir)
    {
        //Increase by one in the chosen direction, looping from -1 (empty hand) to the last tool index and back to -1
        var nextToolIndex = LoopValue(m_currentToolIndex + dir, -1, tools.Count - 1);
        
        SetActiveTool(nextToolIndex);
        OnSwitchTool?.Invoke();
    }

    /// <summary>
    /// Keeps a given value within the bounds of min and max cyclically.
    /// </summary>
    /// <param name="value">The value to loop.</param>
    /// <param name="min">The minimum value of the loop.</param>
    /// <param name="max">The maximum value of the loop.</param>
    /// <returns>The given value as it fits into the loop.</returns>
    private int LoopValue(int value, int min, int max)
    {
        var loopSize = max - min + 1;
        
        while (value > max)
        {
            value -= loopSize;
        }

        while (value < min)
        {
            value += loopSize;
        }

        return value;
    }

    /// <summary>
    /// Sets the current tool.
    /// </summary>
    /// <param name="toolIndex">The index of the tool to use within the m_toolInstances list.</param>
    private void SetActiveTool(int toolIndex)
    {
        if (m_currentToolIndex >= 0) m_toolInstances[m_currentToolIndex].SetActive(false);

        m_currentToolIndex = toolIndex;

        if (m_currentToolIndex >= 0) m_toolInstances[m_currentToolIndex].SetActive(true);
    }
}
