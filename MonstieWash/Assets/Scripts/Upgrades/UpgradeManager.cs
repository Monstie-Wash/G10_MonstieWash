using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameObject toolUpgradePrefab;
    [SerializeField] private Transform upgradeContainer;
    [SerializeField] private TextMeshProUGUI scoreUI;

    private ToolManager m_toolManager;
    private int m_currentScore = 0;

    private void Awake()
    {
        m_toolManager = FindFirstObjectByType<ToolManager>();

        m_currentScore += 150;
        UpdateScoreUI();
    }

    private void Start()
    {
        foreach (var tool in m_toolManager.Tools)
        {
            var upgradeObject = Instantiate(toolUpgradePrefab, upgradeContainer);
            var upgradeScript = upgradeObject.GetComponent<ToolUpgrade>();

            upgradeScript.SetTool(tool);
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreUI != null) 
        {
            scoreUI.text = $"${m_currentScore.ToString()}";
        }
    }

    public bool SpendScore(int score)
    {
        var result = false;
        if (score <= m_currentScore)
        {
            m_currentScore -= score;
            UpdateScoreUI();
            result = true;
        }
        return result;
    }
}
