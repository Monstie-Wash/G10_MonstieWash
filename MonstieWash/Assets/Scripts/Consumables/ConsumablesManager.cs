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
    [Tooltip("Layer that checks player mouse is over bag to open its contents")] [SerializeField] public LayerMask bagLayer; //Layer of consumables hitbox;
    [Tooltip("Layer that checks player mouse is over consumable collider")] [SerializeField] public LayerMask itemConsumableLayer; //Layer of consumables hitbox;

    [SerializeField] private Inventory m_inventory; //Reference to Inventory;

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



    public void OnEnable()
    {
        //InputManager.Instance.OnActivate += CheckClickedOn;
    }

    public void OnDisable()
    {
        //InputManager.Instance.OnActivate -= CheckClickedOn;
    }

    private void Awake()
    {
        holdingConsumable = false;
        m_playerHand = FindFirstObjectByType<PlayerHand>();
        m_inventory = FindFirstObjectByType<Inventory>();
        m_managerimage = gameObject.GetComponent<Image>();
        state = UiState.Closed;
        RefreshUI();
    }

    private void Update()
    {
        //Open or close bag when hovered over;
        CheckHovered();


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
        foreach (Inventory.InventoryEntry itemData in m_inventory.StoredItemsList)
        {
            if (itemData.ItemData.ContainsTag(Inventory.ItemTags.Consumable))
            {
                //Check if consumable already has Ui object and skip
                var matched = false;
                int consumableToRemoveIndex = -1;

                foreach (UIConsumable uiConsumable in activeUiElements)
                {
                    if (itemData.ItemData.ItemName == uiConsumable.consumable.ConsumableName) matched = true;
                    if (matched) //If an item already exists then update its number and remove it if it has been reduced to 0;
                    {
                        if (itemData.Quantity <= 0)
                        {
                            consumableToRemoveIndex = activeUiElements.IndexOf(uiConsumable);
                            Destroy(uiConsumable.gameObject);
                        }
                        else uiConsumable.quantityText.text = itemData.Quantity.ToString();
                        break;
                    }
                }
                if (consumableToRemoveIndex != -1) activeUiElements.RemoveAt(consumableToRemoveIndex);
                if (matched) continue;
                else CreateUiElement(itemData);
            }
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

    private void CreateUiElement(Inventory.InventoryEntry data)
    {
        //If no object exists create a new one and insert data from its scriptable obj.
        var newImage = Instantiate(refObject, refObject.transform.position, Quaternion.identity);
        newImage.transform.SetParent(imageHolder.transform);
        newImage.transform.position = this.transform.position;
        newImage.GetComponent<Image>().sprite = data.ItemData.Sprite;
        newImage.gameObject.SetActive(false);

        var newImageUi = newImage.GetComponent<UIConsumable>();
        Consumable c = (data.ItemData as Consumable);
        if (c != null)
        {
            newImageUi.consumable = c;
        }
        else print("Somehow a non consumable has slipped into the Consumable manager code");
        newImageUi.manager = this;
        newImageUi.quantityText.text = data.Quantity.ToString();
        activeUiElements.Add(newImageUi);
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
        //Only open when closed and dont interupt it already opening;
        if (state != UiState.Closed) return;
        if (state == UiState.Opening) return;

        state = UiState.Opening;
        m_managerimage.sprite = openSprite;
    }

    /// <summary>
    /// Closes bag or UI equivalent and begins moving items to closed UI position.
    /// </summary>
    public void CloseUI()
    {
        //Only close when opened and dont interupt it already closing;
        if (state != UiState.Open) return;
        if (state == UiState.Closing) return;

        foreach (UIConsumable c in activeUiElements)
        {
            c.holding = false;     
        }

        state = UiState.Closing;
    }

    public void CheckHovered()
    {
        var col = Physics2D.OverlapCircle(Camera.main.WorldToScreenPoint(m_playerHand.transform.position), 1f, bagLayer, -999999, 999999);
        if (col != null)
        {
            if ((col.gameObject == gameObject) && state != UiState.Open) OpenUI();
        }
        else if (state != UiState.Closed && !holdingConsumable) CloseUI();
    }


}
