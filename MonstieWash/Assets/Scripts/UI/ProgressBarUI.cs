using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ProgressBarUI : MonoBehaviour
{
    private GameSceneManager m_gameSceneManager;
    private RectMask2D m_progressMask;
    private RectTransform m_maskTransform;
    private List<TaskData> m_taskData;
    private float m_sceneProgress = 0;
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

/// <summary>
/// Recalculates tasks completed and total tasks for the current active scene
/// </summary>
    public void CheckCompletion()
    {
        m_sceneProgress = 0f;
        m_taskCount = 0;
        foreach (var task in m_taskData.Where(t => t.gameObject.scene == m_gameSceneManager.CurrentScene))
        {
            m_taskCount++;
            m_sceneProgress += task.Progress;
        }
        UpdateUI();
    }

/// <summary>
/// Updates progress bar to reflect current progress
/// </summary>
    private void UpdateUI()
    {
        m_progressMask.padding = new Vector4 (0, 0, m_maskTransform.rect.width - Mathf.CeilToInt(m_maskTransform.rect.width * ( m_sceneProgress / 100f / m_taskCount)), 0);
    }
}
