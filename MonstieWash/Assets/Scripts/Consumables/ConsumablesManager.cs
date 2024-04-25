using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml;

public class ConsumablesManager : MonoBehaviour
{

    public int uiBorderDistance; //Distance between objects in the Ui;
    public float uiMoveSpeed; //How fast Ui Elements close and open;
    public Sprite openSprite;
    public Sprite closedSprite;  
    public List<UIConsumable> activeUiElements; //UI Elements already generated in the scene.
    public GameObject imageHolder; //Empty Gameobject in Ui to hold images;
    public GameObject refObject; //blank object for instantiate to reference;


    [Header("Testing List")]
    public List<Consumable> testingObjs;

    private Image managerimage; //Reference to the image object for this manager.
    private Dictionary<Consumable, int> storedDict; //Consumable is type of item, int is stored amount.

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
        storedDict = new Dictionary<Consumable, int>();
        managerimage = gameObject.GetComponent<Image>();
        state = UiState.Closed;
        RefreshUI();

        foreach (Consumable c in testingObjs)
        {
            AddNewConsumable(c,1);
        }
    }

    private void Update()
    {
        //Testing Buttons
        if (Input.GetKeyDown(KeyCode.O))
        {
            print("Opening Ui");
            OpenUI();         
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            print("Closing Ui");
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
                        u.moveTowardsExtendedPos(uiMoveSpeed);
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
                        u.moveTowardsClosedPos(uiMoveSpeed);
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
        //Delete current ui elements
        for (int i = activeUiElements.Count - 1; i > -1; i-- )
        {
            Destroy(activeUiElements[i].gameObject);
        }
        activeUiElements.Clear();

        //Generate new Ui objects
        foreach (KeyValuePair<Consumable, int> consumable in storedDict)
        {
            var newImage = Instantiate(refObject,refObject.transform.position,Quaternion.identity);
            newImage.transform.SetParent(imageHolder.transform);
            newImage.transform.position = this.transform.position;
            newImage.GetComponent<Image>().sprite = consumable.Key.Sprite;
            newImage.gameObject.SetActive(false);
            activeUiElements.Add(newImage.GetComponent<UIConsumable>()); 
        }

        //Assign Ui positions.
        for (int i = 0; i < activeUiElements.Count; i++)
        {
            activeUiElements[i].extendedPos = new Vector3(this.transform.position.x + ((i+1) * uiBorderDistance), this.transform.position.y, this.transform.position.z);
            activeUiElements[i].closedPos = this.transform.position;
        }

        if (state == UiState.Open) state = UiState.Opening;
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


    public void OpenUI()
    {
        //Only open when closed;
        if (state != UiState.Closed) return;

        RefreshUI();
        state = UiState.Opening;
        managerimage.sprite = openSprite;
    }

    public void CloseUi()
    {
        //Only close when opened
        if (state != UiState.Open) return;
        state = UiState.Closing;
    }

}
