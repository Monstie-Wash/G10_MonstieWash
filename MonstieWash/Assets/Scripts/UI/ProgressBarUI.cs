using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    private RectMask2D m_progressMask;
    private RectTransform m_maskTransform;
    private List<TaskData> m_taskData;
    private int m_taskCompletedCount = 0;

    // Start is called before the first frame update
    private void Start()
    {
        m_taskData = FindFirstObjectByType<TaskTracker>().TaskData;
        m_progressMask = GetComponent<RectMask2D>();
        m_maskTransform = GetComponent<RectTransform>();
    }

    public void AddCompletion()
    {
        m_taskCompletedCount++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        m_progressMask.padding = new Vector4 (0, 0, m_maskTransform.rect.width - (m_maskTransform.rect.width * ((float) m_taskCompletedCount / (float) m_taskData.Count)), 0);
    }
}
