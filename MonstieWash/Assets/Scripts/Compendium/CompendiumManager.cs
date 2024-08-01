using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompendiumManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textTitle;
    [SerializeField] private Image imageOriginal;
    [SerializeField] private Image imageCleared;
    [SerializeField] private TextMeshProUGUI textTemperament;
    [SerializeField] private TextMeshProUGUI textDescription;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void DisplayEntry(CompendiumEntry entry)
    {
        if (entry == null) return;

        if(!transform.GetChild(0).gameObject.activeSelf)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        textTitle.text = entry.Name;
        imageOriginal.sprite = entry.ImageOriginal;
        imageCleared.sprite = entry.ImageCleared;
        textTemperament.text = entry.Temperament;
        textDescription.text = entry.Description;

        imageCleared.enabled = entry.Completed;
    }
}
