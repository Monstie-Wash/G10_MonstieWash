using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml;

public class ConsumablesManager : MonoBehaviour
{
    private Dictionary<Consumable, int> storedDict; //Consumable is type of item, int is stored amount.

    private List<Image> consumableImages; //Generates images to represent consumables in Ui;
    private GameObject imageHolder; //Empty Gameobject in Ui to hold images;
    private Image refImage; //blankImage for instantiate to reference;

    private UiState state;
    public enum UiState
    {
        Open,
        Closing,
        Closed
    }



    private void Awake()
    {
        state = UiState.Closed;
        //Generate Images
        foreach (KeyValuePair<Consumable, int> consumable in storedDict)
        {
            var newImage = Instantiate<Image>(refImage);
            newImage.transform.SetParent(imageHolder.transform);
            newImage.sprite = consumable.Key.Sprite;
            newImage.gameObject.SetActive(false);
        }
    }

    public void UpdateStorageAmount(Consumable type, int change)
    {
        if (storedDict.ContainsKey(type))
        {
            storedDict[type] = Mathf.Clamp(storedDict[type] + change, 0, type.MaxQuantity);
        }
        else print($"Consumable {type.ConsumableName} doesn't exist. ");
    }
    

    public void SetStorageAmount(Consumable type, int amount)
    {
        if (storedDict.ContainsKey(type))
        {
            storedDict[type] = Mathf.Clamp(amount, 0, type.MaxQuantity);
        }
        else print($"Consumable {type.ConsumableName} doesn't exist. ");
    }

    public void AddNewConsumable(Consumable type, int amount)
    {
        if (!storedDict.ContainsKey(type))
        {
            storedDict.Add(type, amount);
        }
        else print($"Consumable {type.ConsumableName} already exists.");
    }

    public void RemoveConsumable(Consumable type)
    {
        if (storedDict.ContainsKey(type))
        {
            storedDict.Remove(type);
        }
        else print($"Consumable {type.ConsumableName} doesn't exist.");
    }


    public void ToggleUi()
    {
        


    }

    public void OpenUI()
    {

        

    }

    public void CloseUi()
    {



    }

}
