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

    [HideInInspector] public float distToPoint;

    [HideInInspector] public ConsumablesManager manager;

    [HideInInspector] public TextMeshProUGUI quantityText;

    public void Awake()
    {
        quantityText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (holding) this.transform.position = Input.mousePosition;
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
        distToPoint = Vector3.Distance(this.transform.position, extendedPos);

        //Move towards pos if not close enough.
        if (distToPoint >= 0.2f)
        {
           this.transform.position = Vector3.Lerp(this.transform.position, extendedPos, speed * Time.deltaTime);
        }
        else // Become clickable
        {
            clickable = true;
        }
    }

    public void MoveTowardsClosedPos(float speed)
    {
        //Get Distance to Pos;
        distToPoint = Vector3.Distance(this.transform.position, closedPos);

        //Move towards pos if not close enough.
        if (distToPoint >= 0.5f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, closedPos, speed * Time.deltaTime * 100);
        }
        else // Become unclickable
        {
            clickable = false;
            gameObject.SetActive(false);
        }

    }

    public bool CheckOverPlayer()
    {
        if (Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1, manager.monsterLayer,-999999,999999)) return true;
        else return false;
    }
    
}
