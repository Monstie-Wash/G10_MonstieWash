using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushAnimationController : MonoBehaviour
{
    private Animator m_Animator;
    [SerializeField] private int m_currentDirection; //-1 for moving left, 0 for stationary, 1 for moving right.
    private float brushReleaseDelay; //Small buffer where brush will not release animation after player stops moving.
    private float m_timeToRelease; //Timer for above setting.
    private Vector3 m_lastPosition;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_lastPosition = transform.position;
        brushReleaseDelay = 0.3f;
    }


    private void Update()
    {
        CalculateDirection();
        UpdateAnimatorFlags();
    }

    private void CalculateDirection()
    {
        //Check first if player is actually holding down to clean.

        //Determine moving direction.
        if (m_lastPosition == transform.position)
        {

        }
        else
        {
            m_currentDirection = (int)Mathf.Sign((m_lastPosition - gameObject.transform.position).y);
            m_timeToRelease = brushReleaseDelay;
        }

        //Update last position
        m_lastPosition = gameObject.transform.position;

        //Update release timer
        m_timeToRelease = Mathf.Max(m_timeToRelease - Time.deltaTime, 0);

        //Release if timer has reached 0
        if (m_timeToRelease == 0) m_currentDirection = 0;
    }

    private void UpdateAnimatorFlags()
    {
        switch (m_currentDirection)
        {
            case 0:
                m_Animator.SetBool("Finish", true);
                m_Animator.SetBool("Enter", false);
                m_Animator.SetBool("Right", false);
                m_Animator.SetBool("Left", false);
                break;

            case -1:
                m_Animator.SetBool("Finish", false);
                m_Animator.SetBool("Enter", true);
                m_Animator.SetBool("Right", true);
                m_Animator.SetBool("Left", false);
                break;

            case 1:
                m_Animator.SetBool("Finish", false);
                m_Animator.SetBool("Enter", true);
                m_Animator.SetBool("Right", false);
                m_Animator.SetBool("Left", true);
                break;                  
        }
    }
}
