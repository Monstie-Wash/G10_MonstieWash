using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIConsumable : MonoBehaviour
{
    [Tooltip("The consumable type represented by this object")] [SerializeField]  public Consumable consumable;

    [HideInInspector] public Vector3 extendedPos;
    [HideInInspector] public Vector3 closedPos;

    [HideInInspector] public bool clickable;
    [HideInInspector] public bool holding;

    [HideInInspector] public float m_distToPoint;

    [HideInInspector] public ConsumablesManager manager;

    [HideInInspector] public TextMeshProUGUI quantityText;
    public PlayerHand m_playerHand;


    public void Awake()
    {
        InputManager.Inputs.OnActivate += CheckClickedOn;
        quantityText = GetComponentInChildren<TextMeshProUGUI>();
        m_playerHand = FindFirstObjectByType<PlayerHand>();
    }

    private void Update()
    {
        if (holding) transform.position = Input.mousePosition;
    }

    public void ClickedOn()
    {
        //Return if bag isnt in an open state.
        if (manager.state != ConsumablesManager.UiState.Open) return;
        
        if (!holding) holding = true;
        else
        {
            if (CheckOverPlayer())
            {
                consumable.Consume();
            }
            else
            {
                manager.state = ConsumablesManager.UiState.Opening;
            }
            holding = false;
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
        if (Physics2D.OverlapCircle(m_playerHand.transform.position, 1, manager.monsterLayer)) return true;
            else return false;
    }

    public void CheckClickedOn()
    {
        print("Checked clicked on called");
        var col = Physics2D.OverlapCircle(m_playerHand.transform.position, 1, manager.consumableLayer);
        if (col != null)
        {
            if (col.gameObject == gameObject) ClickedOn();
        }
    }

}
