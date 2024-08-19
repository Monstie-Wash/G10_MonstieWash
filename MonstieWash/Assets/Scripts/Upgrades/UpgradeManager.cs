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

    //Changes from Luca for keeping consistent inventory/gold across scenes;
    private Inventory m_inventory;
    [SerializeField] private InventoryItem goldData; //Need to set the item that will be updated;
    [SerializeField] private int levelReward = 150;


    private void Awake()
    {

        m_toolManager = FindFirstObjectByType<ToolManager>();
        m_soundPlayer = GetComponent<SoundPlayer>();

        //Finds inventory and passes in a type to update by an amount given.
        m_inventory = FindFirstObjectByType<Inventory>();
        levelReward = m_inventory.LastEarnedScore;

        //Add to gold if player has some already otherwise create a new gold item in the invetory;
        if (m_inventory.ContainsItem(goldData)) m_inventory.UpdateStorageAmount(goldData, levelReward);
        else m_inventory.AddNewItem(goldData, levelReward);


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
            scoreUI.text = $"${m_inventory.ReadQuantity(goldData)}";
        }
    }

    public bool TryUpgrade(int score)
    {
        var result = false;
        if (score <= m_inventory.ReadQuantity(goldData))
        {
            m_soundPlayer.PlaySound();
            m_inventory.UpdateStorageAmount(goldData,-score);
            UpdateScoreUI();
            result = true;
        }
        return result;
    }
}
