using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// I needed an intermediary to pass data from Erasable struct to TaskTracker and in reverse. I couldn't find an alternative so this is a quick ugly fix. 
/// </summary>
public class ErasableTaskWrapper : MonoBehaviour, ITask
{
    private string m_taskName;
    private float m_taskProgress;
    private float m_newProgress;

    public string TaskName 
    { 
        get { return m_taskName; } 
        set { if (m_taskName == null) m_taskName = value; } 
    }

    public float TaskProgress
    {
        get { return m_taskProgress; }
        set
        {
            m_newProgress = value - m_taskProgress;
            m_taskProgress = value;
        }
    }
    public float NewProgress
    {
        get { return m_newProgress; }
        set { NewProgress = value; }
    }

    private void Awake()
    {
        //m_taskName = GetParentName(transform);
    }

    private string GetParentName(Transform parentTransform)
    {
        var taskName = "";

        if (parentTransform.parent != null)
        {
            taskName += GetParentName(parentTransform.parent);
        }
        else
        {
            taskName += parentTransform.gameObject.scene.name + "Container";
            return taskName;
        }

        taskName += $"#{parentTransform.name}";
        return taskName;
    }
}
