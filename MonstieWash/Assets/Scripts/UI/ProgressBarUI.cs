using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ProgressBarUI : MonoBehaviour
{
    private RectMask2D m_progressMask;
    private RectTransform m_maskTransform;

    private void Awake()
    {
        m_progressMask = GetComponent<RectMask2D>();
        m_maskTransform = GetComponent<RectTransform>();
    }

/// <summary>
/// Updates progress bar to reflect current progress
/// </summary>
    public void UpdateUI(float completion)
    {
        m_progressMask.padding = new Vector4 (0, 0, m_maskTransform.rect.width - Mathf.CeilToInt(m_maskTransform.rect.width * completion), 0);
    }
}
