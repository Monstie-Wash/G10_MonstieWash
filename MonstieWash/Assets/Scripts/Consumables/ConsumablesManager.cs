using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ConsumablesManager : MonoBehaviour
{

    [Tooltip("How far apart the consumables are in the UI")] [SerializeField] private int uiBorderDistance; //Distance between objects in the Ui;
    [Tooltip("How quickly Consumables move towards their open position")] [SerializeField] private float uiMoveSpeed; //How fast Ui Elements close and open;
    [Tooltip("Sprite to represent open bag.")] [SerializeField] private Sprite openSprite; //Sprite used to represent open bag.
    [Tooltip("Sprite to represent closed bag.")] [SerializeField] private Sprite closedSprite; //Sprite used to represent closed bag.
    [SerializeField] private List<UIConsumable> activeUiElements; //UI Elements already generated in the scene;
    [Tooltip("Gameobject new Ui consumable items will be parented under")] [SerializeField] private GameObject imageHolder; //Empty Gameobject in Ui to hold images;
    [Tooltip("A template obj requiring, image, UIconsumableScript, button linked to the scripts onclick")] [SerializeField] private GameObject refObject; //blank object for instantiate to reference; Needs a button, UIConsumableScript, Image.
    [Tooltip("Layer that checks player mouse is over monster collider")] [SerializeField] public LayerMask monsterLayer; //Layer of monster hitbox;
    [Tooltip("Layer that checks player mouse is over monster collider")] [SerializeField] public LayerMask consumableLayer; //Layer of consumables hitbox;

    [Header("All consumables go here")]
    [Tooltip("List all starting consumables")] [SerializeField] private List<Consumable> consumableData;

    private Image m_managerimage; //Reference to the image object for this manager.
    private Dictionary<string, int> m_storedDict; //string is name of consumable type, int is stored amount.

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
        m_storedDict = new Dictionary<string, int>();
        m_managerimage = gameObject.GetComponent<Image>();
        state = UiState.Closed;

        //For testing purposes randomises amount of consumables, will remove later.
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
                    m_managerimage.sprite = closedSprite;
                    state = UiState.Closed;
                }
                break;
        }

    }

    /// <summary>
    /// Checks if any new consumables need UI objects and creates them, removes UI objects for used items, moves objects towards their new position.
    /// </summary>
    public void RefreshUI()
    {
        //Generate new Ui objects
        foreach (KeyValuePair<string, int> consumable in m_storedDict)
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

        //Reset state to opening so objects move to new positions.
        if (state == UiState.Open) state = UiState.Opening;
    }


    /// <summary>
    /// Takes a type of consumable and changes its value in storage by the given change amount.
    /// </summary>
    /// <param name="type">The type of consumable to change.</param> 
    /// <param name="change">The change to apply to that consumable can be pos or neg</param>
    public void UpdateStorageAmount(Consumable type, int change)
    {
        if (m_storedDict.ContainsKey(type.ConsumableName))
        {
            m_storedDict[type.ConsumableName] = Mathf.Clamp(m_storedDict[type.ConsumableName] + change, 0, 999999);
            if (m_storedDict[type.ConsumableName] == 0) RemoveConsumable(type);
            else RetrieveActiveUi(type.ConsumableName).quantityText.text = m_storedDict[type.ConsumableName].ToString();

            return;        
        }
        else
        print($"Consumable {type.ConsumableName} doesn't exist. ");
    }
    
    /// <summary>
    /// Sets the amount of a certain consumable in storage to a given value.
    /// </summary>
    /// <param name="type"> Type of consumable to set storage of</param>
    /// <param name="amount">Amount to set consumable to</param>
    public void SetStorageAmount(Consumable type, int amount)
    {
        if (m_storedDict.ContainsKey(type.ConsumableName))
        {
            m_storedDict[type.ConsumableName] = Mathf.Clamp(amount, 0, type.MaxQuantity);
            if (m_storedDict[type.ConsumableName] == 0) RemoveConsumable(type);
            else RetrieveActiveUi(type.ConsumableName).quantityText.text = m_storedDict[type.ConsumableName].ToString();
        }
        else print($"Consumable {type.ConsumableName} doesn't exist. ");
    }

    /// <summary>
    /// Adds a new consumable of the given type to storage.
    /// </summary>
    /// <param name="type">Type of consumable to add</param>
    /// <param name="amount">Amount starting in storage</param>
    public void AddNewConsumable(Consumable type, int amount)
    {
        if (!m_storedDict.ContainsKey(type.ConsumableName))
        {
            m_storedDict.Add(type.ConsumableName, amount);
            RefreshUI();
        }
        else print($"Consumable {type.ConsumableName} already exists.");
    }

    /// <summary>
    /// Removes a given consumable from storage and deletes its relevant UI object.
    /// </summary>
    /// <param name="type">Type of consumable to remove</param>
    public void RemoveConsumable(Consumable type)
    {
        if (m_storedDict.ContainsKey(type.ConsumableName))
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
            m_storedDict.Remove(type.ConsumableName);
        }
        else print($"Consumable {type.ConsumableName} doesn't exist.");
        RefreshUI();
    }

    /// <summary>
    /// Retrieves a consumable type from the data pool by its name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
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
