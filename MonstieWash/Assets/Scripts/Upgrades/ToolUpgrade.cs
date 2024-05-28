using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolUpgrade : MonoBehaviour
{
    private Tool m_tool;
    private Image m_toolIcon;
    private TextMeshProUGUI m_toolName;
    private Button m_toolUpgrade;
    private Image m_toolMask;
    private TextMeshProUGUI m_toolPrice;
    private int m_upgradeCost = 25;
    private UpgradeManager m_upgradeManager;
    
    void Awake()
    {
        m_upgradeManager = FindFirstObjectByType<UpgradeManager>();
        m_toolIcon = GetComponentsInChildren<Image>()[1];
        m_toolName = GetComponentsInChildren<TextMeshProUGUI>()[0];
        m_toolPrice = GetComponentsInChildren<TextMeshProUGUI>()[1];
        m_toolUpgrade = GetComponentInChildren<Button>();
        m_toolMask = GetComponentsInChildren<Image>()[2];

        m_toolUpgrade.onClick.AddListener(RequestUpgrade);
    }

    public void SetTool(Tool tool)
    {
        m_tool = tool;
        m_toolName.text = m_tool.toolName;
        m_toolMask.sprite = m_tool.mask;
        m_toolPrice.text = $"${m_upgradeCost.ToString()}";
    }

    private void RequestUpgrade()
    {
        if (m_upgradeManager.SpendScore(m_upgradeCost))
        {
            UpgradeTool();
        }
    }

    private void UpgradeTool()
    {
        if (m_tool != null)
        {
            m_tool.size += 1;
            m_tool.InputStrength += 5f;
            m_upgradeCost += 25;
            m_toolPrice.text = $"${m_upgradeCost.ToString()}";
        }
        else
        {
            Debug.Log($"{name} has not been assigned a tool to upgrade by UpgradeManager.");
        }
    }
}
