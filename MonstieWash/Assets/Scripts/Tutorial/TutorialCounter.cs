using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCounter : MonoBehaviour
{
    private TextMeshProUGUI m_text;
    private TutorialManager m_manager;

    // Start is called before the first frame update
    void Awake()
    {
        m_text = GetComponent<TextMeshProUGUI>();
        m_manager = FindFirstObjectByType<TutorialManager>();
    }

    private void OnEnable()
    {
        m_manager.ProgressUpdate += UpdateText;
    }

    private void OnDisable()
    {
        m_manager.ProgressUpdate -= UpdateText;
    }

    // Update is called once per frame
    void UpdateText()
    {
        if (m_manager == null || m_text == null) return;
        else m_text.text = m_manager.Completion;
    }
}
