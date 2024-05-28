using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class ConsumablesManager : MonoBehaviour
{

    [Tooltip("How far apart the consumables are in the UI")] [SerializeField] private int uiBorderDistance; //Distance between objects in the Ui;
    [Tooltip("How quickly Consumables move towards their open position")] [SerializeField] private float uiMoveSpeed; //How fast Ui Elements close and open;
    [Tooltip("Sprite to represent open bag.")] [SerializeField] private Sprite openSprite; //Sprite used to represent open bag.
    [Tooltip("Sprite to represent closed bag.")] [SerializeField] private Sprite closedSprite; //Sprite used to represent closed bag.
    [SerializeField] private List<UIConsumable> activeUiElements; //UI Elements already generated in the scene;
    [Tooltip("Gameobject new Ui consumable items will be parented under")] [SerializeField] private GameObject imageHolder; //Empty Gameobject in Ui to hold images; (I tried changing to transform as per feedback but doesn't seem to work with rect transform in ui)
    [Tooltip("A template obj requiring, image, UIconsumableScript, button linked to the scripts onclick")] [SerializeField] private GameObject refObject; //blank object for instantiate to reference; Needs a button, UIConsumableScript, Image.
    [Tooltip("Layer that checks player mouse is over monster collider")] [SerializeField] private LayerMask monsterLayer; //Layer of monster hitbox;
    [Tooltip("Layer that checks player mouse is over monster collider")] [SerializeField] public LayerMask consumableLayer; //Layer of consumables hitbox;

    [Header("All consumables go here")]
    [Tooltip("List all starting consumables")] [SerializeField] private List<consumableData> consumableList;

    private Image m_managerimage; //Reference to the image object for this manager.
    private PlayerHand m_playerHand;
    public bool holdingConsumable;

    public UiState state;

    public LayerMask MonsterLayer {  get { return monsterLayer; } }

    public enum UiState
    {
        Open,
        Opening,
        Closing,
        Closed
    }

    [Serializable]
    private class consumableData
    {
        [SerializeField] private Consumable consumable;
        [SerializeField] private int quantity;

        public Consumable Consumable { get { return consumable; } }
        public int Quantity 
        { 
            get { return quantity;  }
            set { quantity = value; }        
        }  

        public consumableData(Consumable type, int quant)
        {
            consumable = type;
            quantity = quant;
        }
    }

    public void OnEnable()
    {
        InputManager.Instance.OnActivate += CheckClickedOn;
    }

    public void OnDisable()
    {
        InputManager.Instance.OnActivate -= CheckClickedOn;
    }

    private void Awake()
    {
        holdingConsumable = false;
        m_playerHand = FindFirstObjectByType<PlayerHand>();
        m_managerimage = gameObject.GetComponent<Image>();
        state = UiState.Closed;
        RefreshUI();
    }

    private void Update()
    {
        //State Transitions
        switch (state)
        {
            case UiState.Opening:
                var stillMoving = false;
                //Move all Ui Objects towards their intended pos and check if all are finished moving;

                foreach (var consumableUI in activeUiElements)
                {
                    if (!consumableUI.isActiveAndEnabled) consumableUI.gameObject.SetActive(true); //Set objects to be active.
                    if (!consumableUI.clickable)
                    {
                        consumableUI.MoveTowardsExtendedPos(uiMoveSpeed);
                        stillMoving = true; 
                    }
                }

                if (stillMoving == false)
                {
                    state = UiState.Open;
                }

                break;

            case UiState.Closing:
                //Move all Ui Objects towards their intended pos and check if all are finished moving;
                var stillClosing = false;
                foreach (var consumableUI in activeUiElements)
                {
                    if (consumableUI.clickable)
                    {
                        consumableUI.MoveTowardsClosedPos(uiMoveSpeed);
                        stillClosing = true; 
                    }
                }

                if (stillClosing == false)
                {
                    m_managerimage.sprite = closedSprite;
                    state = UiState.Closed;
                }
                break;
        }

    }

    public void toggleBag()
    {
        if (state == UiState.Closed) OpenUI();
        if (state == UiState.Open) CloseUI();
    }



    /// <summary>
    /// Checks if any new consumables need UI objects and creates them, removes UI objects for used items, moves objects towards their new position.
    /// </summary>
    public void RefreshUI()
    {
        //Generate new Ui objects
        foreach (consumableData consumeStruct in consumableList)
        {
            //Check if consumable already has Ui object and skip
            var matched = false; 
            foreach (UIConsumable uiConsumable in activeUiElements)
            {
                if (consumeStruct.Consumable.ConsumableName == uiConsumable.consumable.ConsumableName) matched = true;
            }
            if (matched) continue;

            CreateUiElement(consumeStruct);
        }

        //Assign Ui positions.
        for (int i = 0; i < activeUiElements.Count; i++)
        {
            activeUiElements[i].extendedPos = new Vector3(this.transform.position.x + ((i+1) * uiBorderDistance), this.transform.position.y, this.transform.position.z);
            activeUiElements[i].closedPos = this.transform.position;
            activeUiElements[i].clickable = false;
        }

        //Reset state to opening so objects move to new positions.
        if (state == UiState.Open) state = UiState.Opening;
    }

    private void CreateUiElement(consumableData data)
    {
        //If no object exists create a new one and insert data from its scriptable obj.
        var newImage = Instantiate(refObject, refObject.transform.position, Quaternion.identity);
        newImage.transform.SetParent(imageHolder.transform);
        newImage.transform.position = this.transform.position;
        newImage.GetComponent<Image>().sprite = data.Consumable.Sprite;
        newImage.gameObject.SetActive(false);

        var newImageUi = newImage.GetComponent<UIConsumable>();
        newImageUi.consumable = data.Consumable;
        newImageUi.manager = this;
        newImageUi.quantityText.text = data.Quantity.ToString();
        activeUiElements.Add(newImageUi);
    }


    /// <summary>
    /// Takes a type of consumable and changes its value in storage by the given change amount.
    /// </summary>
    /// <param name="type">The type of consumable to change.</param> 
    /// <param name="change">The change to apply to that consumable can be pos or neg</param>
    public void UpdateStorageAmount(Consumable type, int change)
    {
        var index = RetrieveConsumableIndex(type);
        if (index != -1)
        {
            consumableList[index].Quantity = Mathf.Clamp(consumableList[index].Quantity + change, 0, type.MaxQuantity);
            if (consumableList[index].Quantity == 0) RemoveConsumable(type);
            else RetrieveActiveUi(type.ConsumableName).quantityText.text = consumableList[index].Quantity.ToString();
        }
        else Debug.LogWarning($"Consumable {type.ConsumableName} doesn't exist."); ;
    }
    
    /// <summary>
    /// Sets the amount of a certain consumable in storage to a given value.
    /// </summary>
    /// <param name="type"> Type of consumable to set storage of</param>
    /// <param name="amount">Amount to set consumable to</param>
    public void SetStorageAmount(Consumable type, int amount)
    {
        var index = RetrieveConsumableIndex(type);
        if (index != -1)
        {
            consumableList[index].Quantity = Mathf.Clamp(amount, 0, type.MaxQuantity);
            if (consumableList[index].Quantity == 0) RemoveConsumable(type);
            else RetrieveActiveUi(type.ConsumableName).quantityText.text = consumableList[index].Quantity.ToString();
        }
        else Debug.LogWarning($"Consumable {type.ConsumableName} doesn't exist."); ;
    }

    /// <summary>
    /// Adds a new consumable of the given type to storage.
    /// </summary>
    /// <param name="type">Type of consumable to add</param>
    /// <param name="amount">Amount starting in storage</param>
    public void AddNewConsumable(Consumable type, int amount)
    {
        var index = RetrieveConsumableIndex(type);
        if (index != -1)
        {
            Debug.LogWarning($"Consumable {type.ConsumableName} already exists."); 
        }
        else consumableList.Add( new consumableData(type, amount));

    }

    /// <summary>
    /// Removes a given consumable from storage and deletes its relevant UI object.
    /// </summary>
    /// <param name="type">Type of consumable to remove</param>
    public void RemoveConsumable(Consumable type)
    {
        var consumeIndex = RetrieveConsumableIndex(type);
        if (consumeIndex != -1)
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
            consumableList.RemoveAt(consumeIndex);
        }
        else Debug.LogWarning($"Consumable {type.ConsumableName} doesn't exist.") ;
        RefreshUI();
    }


    /// <summary>
    /// Takes a consumable and returns the index of it within this managers consumablelist.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public int RetrieveConsumableIndex(Consumable c)
    {
        for (int i = 0; i < consumableList.Count; i++)
        {
            if (consumableList[i].Consumable.ConsumableName == c.ConsumableName)
            {
                return i;
            }
        }
        return -1;
    }


    /// <summary>
    /// Retrieves an active UI objects by its given name.
    /// </summary>
    /// <param name="name">Name of the consumables UI to retrieve</param>
    /// <returns></returns>
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

    public void dropItems()
    {
        foreach (UIConsumable ui in activeUiElements)
        {
            ui.holding = false;
            ui.clickable = false;
        }
    }    


    /// <summary>
    /// Opens bag or UI equivalent and begins moving items to their assigned UI positions.
    /// </summary>
    public void OpenUI()
    {
        //Only open when closed;
        if (state != UiState.Closed) return;

        state = UiState.Opening;
        m_managerimage.sprite = openSprite;
    }

    /// <summary>
    /// Closes bag or UI equivalent and begins moving items to closed UI position.
    /// </summary>
    public void CloseUI()
    {
        //Only close when opened
        if (state != UiState.Open) return;

        foreach(UIConsumable c in activeUiElements)
        {
            c.holding = false;     
        }

        state = UiState.Closing;
    }

    public void CheckClickedOn()
    {
        var col = Physics2D.OverlapCircle(Camera.main.WorldToScreenPoint(m_playerHand.transform.position), 1f, consumableLayer, -999999, 999999);
        if (col != null)
        {
            if (col.gameObject == gameObject) toggleBag();
        }
    }


}
