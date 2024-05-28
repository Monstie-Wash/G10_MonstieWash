using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEyeFollow : MonoBehaviour
{
    [SerializeField] private Vector2 pupilStartingPos;
    [SerializeField] private float trackingSpeed;
    [SerializeField] private float bufferFromEdge;
    private PlayerHand m_ph;

    private void Start()
    {
        pupilStartingPos = gameObject.transform.position;
        m_ph = FindFirstObjectByType<PlayerHand>();
    }


    private void Update()
    {
        //Set point in direction of hand based on buffer from starting position
        var newPupilPos = pupilStartingPos + (new Vector2(m_ph.transform.position.x, m_ph.transform.position.y) - pupilStartingPos).normalized * bufferFromEdge;

        //Move Pupil Towards calculated point
        gameObject.transform.position = Vector2.Lerp(gameObject.transform.position, newPupilPos, trackingSpeed*Time.deltaTime);
    }
}
