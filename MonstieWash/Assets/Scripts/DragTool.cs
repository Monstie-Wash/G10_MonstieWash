using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
Notes for Refactoring:
- Fix discourse between worldspace and screenspace
- FindObjectOfType is deprecated
- Find better method for detecting collision (if possible); more tolerance for hand (not just middle point of hand)
*/

public class DragTool : MonoBehaviour
{
    private bool beingHeld = false;
    private bool tempInactive = false;  // allows picking up object without using it
    private Playerhand uiHand;
    private SpriteRenderer mySprite;
    private float myWidth;
    private float myHeight;

    // Start is called before the first frame update
    void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
        myWidth = mySprite.bounds.size.x;
        myHeight = mySprite.bounds.size.y;

        uiHand = (Playerhand)FindObjectOfType(typeof(Playerhand));      // fix this later :)
    }

    // Update is called once per frame
    void Update()
    {

        // pickup item
        if (Input.GetButtonDown("Fire1") & (CollidingWithHand())) { 
            if (!beingHeld) {
                tempInactive = true;    
                beingHeld = true;   // hold the item
            }
        }

        // allow item to be used once pickup is complete
        if (Input.GetButtonUp("Fire1") & beingHeld) {
                tempInactive = false;
        }

        // while tool is held
        if (beingHeld) {
            transform.position = Camera.main.ScreenToWorldPoint(uiHand.m_handPosition); // follow hand
            if (Input.GetButton("Fire1") & !tempInactive) {     // press fire1 to use the item
                Debug.Log("Tool used"); 
            }
            else if (Input.GetButton("Fire2")) {    // press fire2 to drop the item
                beingHeld = false;  
            }
        }
    }

    bool CollidingWithHand() {
        Vector3 myPos = transform.position;
        if (Camera.main.ScreenToWorldPoint(uiHand.m_handPosition).x > myPos.x - (myWidth/2) & Camera.main.ScreenToWorldPoint(uiHand.m_handPosition).x < myPos.x + (myWidth/2)) {
            if (Camera.main.ScreenToWorldPoint(uiHand.m_handPosition).y > myPos.y - (myHeight/2) & Camera.main.ScreenToWorldPoint(uiHand.m_handPosition).y < myPos.y + (myHeight/2)) {
                return true;
            }
        }
        return false;
    }
}