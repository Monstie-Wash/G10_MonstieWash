using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeFollowObj : MonoBehaviour
{
    private PlayerHand m_ph;
    private Vector3 pupilStartingPos;

    public float followSpeed;
    public float bufferFromEdge;
    public GameObject pupil;



    // Start is called before the first frame update
    void Start()
    {
        m_ph = FindFirstObjectByType<PlayerHand>();
        pupilStartingPos = pupil.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Get the closest point on the eye to the playerhand.

            
        //Calculate point back from closest point offset by the buffer.


        //Move the pupil towards the calculated point by the follow speed.


    }
}
