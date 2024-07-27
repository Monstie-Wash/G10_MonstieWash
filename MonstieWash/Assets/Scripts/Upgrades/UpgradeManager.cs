using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameObject toolUpgradePrefab;
    [SerializeField] private Transform upgradeContainer;
    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private Sound upgradeSound;

    private ToolManager m_toolManager;
    private SoundPlayer m_soundPlayer;
    private int m_currentScore = 0;

    private void Awake()
    {
        m_toolManager = FindFirstObjectByType<ToolManager>();
        m_soundPlayer = GetComponent<SoundPlayer>();

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
        m_soundPlayer.SwitchSound(upgradeSound);
    }

    private void UpdateScoreUI()
    {
        if (scoreUI != null) 
        {
            scoreUI.text = $"${m_currentScore.ToString()}";
        }
    }

    public bool TryUpgrade(int score)
    {
        var result = false;
        if (score <= m_currentScore)
        {
            m_soundPlayer.PlaySound();
            m_currentScore -= score;
            UpdateScoreUI();
            result = true;
        }
        return result;
    }
}
