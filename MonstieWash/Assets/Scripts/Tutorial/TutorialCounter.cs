using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCounter : MonoBehaviour
{
    private TextMeshProUGUI m_Text;
    private TutorialManager m_Manager;

    // Start is called before the first frame update
    void Awake()
    {
        m_Text = GetComponent<TextMeshProUGUI>();
        m_Manager = FindFirstObjectByType<TutorialManager>();
    }

    private void OnEnable()
    {
        m_Manager.ProgressUpdate += UpdateText;
    }

    private void OnDisable()
    {
        m_Manager.ProgressUpdate -= UpdateText;
    }

    // Update is called once per frame
    void UpdateText()
    {
        if (m_Manager == null || m_Text == null) return;
        else m_Text.text = m_Manager.Completion;
    }
}
