using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    private GameSceneManager m_gameSceneManager;
    private RectMask2D m_progressMask;
    private RectTransform m_maskTransform;
    private List<TaskData> m_taskData;
    private int m_taskCompletedCount = 0;
    private int m_taskCount = 0;

    private void Awake()
    {
        m_gameSceneManager = FindFirstObjectByType<GameSceneManager>();
        m_progressMask = GetComponent<RectMask2D>();
        m_maskTransform = GetComponent<RectTransform>();
        m_taskData = FindFirstObjectByType<TaskTracker>().TaskData;
    }

    private void OnEnable()
    {
        m_gameSceneManager.OnSceneChanged += CheckCompletion;
    }

    private void OnDisable()
    {
        m_gameSceneManager.OnSceneChanged -= CheckCompletion;
    }

    public void AddCompletion()
    {
        m_taskCompletedCount++;
        UpdateUI();
    }

/// <summary>
/// Recalculates tasks completed and total tasks for the current active scene
/// </summary>
    private void CheckCompletion()
    {
        m_taskCompletedCount = 0;
        m_taskCount = 0;
        foreach (var task in m_taskData)
        {
            if (task.gameObject.scene == m_gameSceneManager.CurrentScene)
            {
                m_taskCount++;
                if (task.Complete) m_taskCompletedCount++;
            }
        }
        UpdateUI();
    }

/// <summary>
/// Updates progress bar to reflect current progress
/// </summary>
    private void UpdateUI()
    {
        m_progressMask.padding = new Vector4 (0, 0, m_maskTransform.rect.width - (m_maskTransform.rect.width * ((float) m_taskCompletedCount / (float) m_taskCount)), 0);
    }
}
