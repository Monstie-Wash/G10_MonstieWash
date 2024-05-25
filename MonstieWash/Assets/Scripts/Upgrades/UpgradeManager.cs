using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameObject toolUpgradePrefab;
    [SerializeField] private Transform upgradeContainer;
    [SerializeField] private Tool[] upgradableTools;
    [SerializeField] private TextMeshProUGUI scoreUI;

    private int m_currentScore = 0;
    
    // Start is called before the first frame update
    private void Start()
    {
        foreach (var tool in upgradableTools)
        {
            var upgradeObject = Instantiate(toolUpgradePrefab, upgradeContainer);
            var upgradeScript = upgradeObject.GetComponent<ToolUpgrade>();

            Debug.Log(upgradeObject.name);
            Debug.Log(tool.name);

            upgradeScript.SetTool(tool);
        }
    }

    private void OnEnable()
    {
        m_currentScore += 150;
        UpdateScoreUI();
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
