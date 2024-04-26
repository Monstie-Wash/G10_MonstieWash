using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml;

public class ConsumablesManager : MonoBehaviour
{

    [SerializeField] private int uiBorderDistance; //Distance between objects in the Ui;
    [SerializeField] private float uiMoveSpeed; //How fast Ui Elements close and open;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private List<UIConsumable> activeUiElements; //UI Elements already generated in the scene;
    [SerializeField] private GameObject imageHolder; //Empty Gameobject in Ui to hold images;
    [SerializeField] private GameObject refObject; //blank object for instantiate to reference;
    [SerializeField] public LayerMask monsterLayer; //Layer of monster hitbox;

    [Header("All consumables go here")]
    [SerializeField] private List<Consumable> consumableData;

    private Image managerimage; //Reference to the image object for this manager.
    private Dictionary<string, int> storedDict; //string is name of consumable type, int is stored amount.

    public UiState state;
    public enum UiState
    {
        Open,
        Opening,
        Closing,
        Closed
    }



    private void Awake()
    {
        storedDict = new Dictionary<string, int>();
        managerimage = gameObject.GetComponent<Image>();
        state = UiState.Closed;

        foreach (Consumable c in consumableData)
        {
            var randInt = Random.Range(1, 6);
            AddNewConsumable(c,randInt);
        }
        RefreshUI();
    }

    private void Update()
    {
        //Testing Buttons
        if (Input.GetKeyDown(KeyCode.O))
        {
            OpenUI();         
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            CloseUi();
        }



        //State Transitions
        switch (state)
        {
            case UiState.Opening:
                var stillMoving = 0;
                //Move all Ui Objects towards their intended pos and check if all are finished moving;

                foreach (UIConsumable u in activeUiElements)
                {
                    if (!u.isActiveAndEnabled) u.gameObject.SetActive(true); //Set objects to be active.
                    if (!u.clickable)
                    {
                        u.MoveTowardsExtendedPos(uiMoveSpeed);
                        stillMoving += 1; //Increments value to check if any clickables are still moving towards their pos.
                    }
                }

                if (stillMoving == 0)
                {
                    state = UiState.Open;
                }

                break;

            case UiState.Closing:
                //Move all Ui Objects towards their intended pos and check if all are finished moving;
                var stillClosing = 0;
                foreach (UIConsumable u in activeUiElements)
                {
                    if (u.clickable)
                    {
                        u.MoveTowardsClosedPos(uiMoveSpeed);
                        stillClosing += 1; //Increments value to check if any clickables are still moving towards their pos.
                    }
                }

                if (stillClosing == 0)
                {
                    managerimage.sprite = closedSprite;
                    state = UiState.Closed;
                }
                break;
        }

    }


    public void RefreshUI()
    {
        //Generate new Ui objects
        foreach (KeyValuePair<string, int> consumable in storedDict)
        {
            //Check if consumable already has Ui object and skip
            var matches = 0;
            foreach (UIConsumable u in activeUiElements)
            {
                if (consumable.Key == u.consumable.ConsumableName) matches += 1;
            }
            if (matches > 0) continue;

            //If no object exists create a new one and insert data from its scriptable obj.
            var newImage = Instantiate(refObject,refObject.transform.position,Quaternion.identity);
            newImage.transform.SetParent(imageHolder.transform);
            newImage.transform.position = this.transform.position;
            newImage.GetComponent<Image>().sprite = RetrieveConsumableData(consumable.Key).Sprite;
            newImage.gameObject.SetActive(false);

            var newImageUi = newImage.GetComponent<UIConsumable>();
            newImageUi.consumable = RetrieveConsumableData(consumable.Key);
            newImageUi.manager = this;
            newImageUi.quantityText.text = consumable.Value.ToString();
            activeUiElements.Add(newImageUi); 
        }

        //Assign Ui positions.
        for (int i = 0; i < activeUiElements.Count; i++)
        {
            activeUiElements[i].extendedPos = new Vector3(this.transform.position.x + ((i+1) * uiBorderDistance), this.transform.position.y, this.transform.position.z);
            activeUiElements[i].closedPos = this.transform.position;
            activeUiElements[i].clickable = false;
        }

        if (state == UiState.Open) state = UiState.Opening;
    }


    public void UpdateStorageAmount(Consumable type, int change)
    {
        if (storedDict.ContainsKey(type.ConsumableName))
        {
            storedDict[type.ConsumableName] = Mathf.Clamp(storedDict[type.ConsumableName] + change, 0, 999999);
            if (storedDict[type.ConsumableName] == 0) RemoveConsumable(type);
            else RetrieveActiveUi(type.ConsumableName).quantityText.text = storedDict[type.ConsumableName].ToString();

            return;        
        }
        else
        print($"Consumable {type.ConsumableName} doesn't exist. ");
    }
    

    public void SetStorageAmount(Consumable type, int amount)
    {
        if (storedDict.ContainsKey(type.ConsumableName))
        {
            storedDict[type.ConsumableName] = Mathf.Clamp(amount, 0, type.MaxQuantity);
            if (storedDict[type.ConsumableName] == 0) RemoveConsumable(type);
            else RetrieveActiveUi(type.ConsumableName).quantityText.text = storedDict[type.ConsumableName].ToString();
        }
        else print($"Consumable {type.ConsumableName} doesn't exist. ");
    }

    public void AddNewConsumable(Consumable type, int amount)
    {
        if (!storedDict.ContainsKey(type.ConsumableName))
        {
            storedDict.Add(type.ConsumableName, amount);
            RefreshUI();
        }
        else print($"Consumable {type.ConsumableName} already exists.");
    }

    public void RemoveConsumable(Consumable type)
    {
        if (storedDict.ContainsKey(type.ConsumableName))
        {
            //Find active Ui Element Index and remove         
            var index = -1;
            for(var i = 0; i < activeUiElements.Count; i++)
            {
                 if (type == activeUiElements[i].consumable) {
                    index = i;
                    break;
                 } 
            }
            GameObject objectToDestroy = null;
            if (index != -1)
            {
                objectToDestroy = activeUiElements[index].gameObject;
                activeUiElements.RemoveAt(index);
            }
            Destroy(objectToDestroy);

            //Remove dictionary stored type.
            storedDict.Remove(type.ConsumableName);
        }
        else print($"Consumable {type.ConsumableName} doesn't exist.");
        RefreshUI();
    }

    public Consumable RetrieveConsumableData(string name)
    {
        foreach (Consumable c in consumableData)
        {
            if (c.ConsumableName == name)
            {
                return c;
            }
        }
        throw new KeyNotFoundException();
    }

    public UIConsumable RetrieveActiveUi(string name)
    {
        foreach (UIConsumable c in activeUiElements)
        {
            if (c.consumable.ConsumableName == name)
            {
                return c;
            }
        }
        throw new KeyNotFoundException();
    }

    public void OpenUI()
    {
        //Only open when closed;
        if (state != UiState.Closed) return;

        state = UiState.Opening;
        managerimage.sprite = openSprite;
    }

    public void CloseUi()
    {
        //Only close when opened
        if (state != UiState.Open) return;

        foreach(UIConsumable c in activeUiElements)
        {
            c.holding = false;     
        }

        state = UiState.Closing;
    }

}
