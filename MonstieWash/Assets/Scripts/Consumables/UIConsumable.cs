using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConsumable : MonoBehaviour
{
    public Consumable consumable;

    public Vector3 extendedPos;
    public Vector3 closedPos;

    public bool clickable;

    public float distToPoint;



    public void moveTowardsExtendedPos(float speed)
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

    public void moveTowardsClosedPos(float speed)
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
    
}
