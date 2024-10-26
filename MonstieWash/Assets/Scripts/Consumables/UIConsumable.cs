using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIConsumable : MonoBehaviour
{
    [Tooltip("The consumable type represented by this object")] public Consumable consumable;

    [HideInInspector] public Vector3 extendedPos;
    [HideInInspector] public Vector3 closedPos;

    [SerializeField] public bool clickable;
    [SerializeField] public bool holding;

    private float m_distToPoint;
    private GameObject itemBackground; //Item that will move with the hand when grabbed.

    [HideInInspector] public ConsumablesManager manager;

    [SerializeField] public TextMeshProUGUI quantityText;
    [SerializeField] public Image fadedBackground;
    private PlayerHand m_playerHand;

    [SerializeField] private bool showDebug;

    public void OnEnable()
    {
        InputManager.Instance.OnActivate += CheckClickedOn;
    }

    public void OnDisable()
    {
        InputManager.Instance.OnActivate -= CheckClickedOn;
    }

    public void Awake()
    {
        m_playerHand = FindFirstObjectByType<PlayerHand>();
        itemBackground = transform.parent.gameObject;
    }

    private void Update()
    {
        if (holding) transform.position = Camera.main.WorldToScreenPoint(m_playerHand.transform.position);
    }

    public void ClickedOn()
    {
        if (showDebug) print("Clicked on");
        //Return if bag isnt in an open state.
        if (manager.state != ConsumablesManager.UiState.Open) return;

        if (!manager.holdingConsumable)
        {
            manager.holdingConsumable = true;
            holding = true;
        }
        else
        {
            if (CheckOverMonster())
            {
                if (showDebug) print("Consuming");
                consumable.Consume();
            }
            else
            {
                manager.state = ConsumablesManager.UiState.Opening;
            }
            manager.dropItems();
            manager.holdingConsumable = false;
            clickable = false;
            GetComponent<RectTransform>().SetLocalPositionAndRotation(Vector3.zero,Quaternion.identity);
            manager.RefreshUI();
        }
    }

    public void MoveTowardsExtendedPos(float speed)
    {
        //Get Distance to Pos;
        m_distToPoint = Vector3.Distance(itemBackground.transform.position, extendedPos);

        //Move towards pos if not close enough.
        if (m_distToPoint >= 0.2f)
        {
            itemBackground.transform.position = Vector3.Lerp(itemBackground.transform.position, extendedPos, speed * Time.deltaTime);
        }
        else // Become clickable
        {
            clickable = true;
        }
    }

    public void MoveTowardsClosedPos(float speed)
    {
        //Get Distance to Pos;
        m_distToPoint = Vector3.Distance(itemBackground.transform.position, closedPos);

        //Move towards pos if not close enough.
        if (m_distToPoint >= 0.5f)
        {
            itemBackground.transform.position = Vector3.MoveTowards(itemBackground.transform.position, closedPos, speed * Time.deltaTime * 100f);
        }
        else // Become unclickable
        {
            clickable = false;
            itemBackground.SetActive(false);
        }

    }

    public bool CheckOverMonster()
    {
        if (Physics2D.OverlapCircle(m_playerHand.transform.position, 1f, manager.MonsterLayer)) return true;
            else return false;
    }

    public void CheckClickedOn()
    {
        var col = Physics2D.OverlapCircle(Camera.main.WorldToScreenPoint(m_playerHand.transform.position), 1f, manager.ItemConsumableLayer);
        if (col != null)
        {
            if (col.gameObject == gameObject) ClickedOn();
        }
    }

}
