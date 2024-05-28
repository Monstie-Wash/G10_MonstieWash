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

    [HideInInspector] public bool clickable;
    [HideInInspector] public bool holding;

    private float m_distToPoint;

    [HideInInspector] public ConsumablesManager manager;

    [HideInInspector] public TextMeshProUGUI quantityText;
    private PlayerHand m_playerHand;

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
        quantityText = GetComponentInChildren<TextMeshProUGUI>();
        m_playerHand = FindFirstObjectByType<PlayerHand>();
    }

    private void Update()
    {
        if (holding) transform.position = Camera.main.WorldToScreenPoint(m_playerHand.transform.position);
    }

    public void ClickedOn()
    {
        //Return if bag isnt in an open state.
        if (manager.state != ConsumablesManager.UiState.Open) return;

        if (!manager.holdingConsumable)
        {
            manager.holdingConsumable = true;
            holding = true;
        }
        else
        {
            if (CheckOverPlayer())
            {
                print("Consuming");
                consumable.Consume();
            }
            else
            {
                manager.state = ConsumablesManager.UiState.Opening;
            }
            manager.dropItems();
            manager.holdingConsumable = false;
            clickable = false;
            manager.RefreshUI();
        }
    }

    public void MoveTowardsExtendedPos(float speed)
    {
        //Get Distance to Pos;
        m_distToPoint = Vector3.Distance(transform.position, extendedPos);

        //Move towards pos if not close enough.
        if (m_distToPoint >= 0.2f)
        {
           transform.position = Vector3.Lerp(transform.position, extendedPos, speed * Time.deltaTime);
        }
        else // Become clickable
        {
            clickable = true;
        }
    }

    public void MoveTowardsClosedPos(float speed)
    {
        //Get Distance to Pos;
        m_distToPoint = Vector3.Distance(transform.position, closedPos);

        //Move towards pos if not close enough.
        if (m_distToPoint >= 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, closedPos, speed * Time.deltaTime * 100f);
        }
        else // Become unclickable
        {
            clickable = false;
            gameObject.SetActive(false);
        }

    }

    public bool CheckOverPlayer()
    {
        if (Physics2D.OverlapCircle(m_playerHand.transform.position, 1f, manager.MonsterLayer)) return true;
            else return false;
    }

    public void CheckClickedOn()
    {
        var col = Physics2D.OverlapCircle(Camera.main.WorldToScreenPoint(m_playerHand.transform.position), 1f, manager.consumableLayer,-999999,999999);
        if (col != null)
        {
            if (col.gameObject == gameObject) ClickedOn();
        }
    }

}
