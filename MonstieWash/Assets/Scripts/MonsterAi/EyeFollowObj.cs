using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EyeFollowObj : MonoBehaviour
{
    private PlayerHand m_ph;

    [SerializeField] private float eyeSpeed;
    [SerializeField] private float trackingSpeed;
    [SerializeField] private float bufferFromEdge;
    [SerializeField] private List<Pupil> pupils;
    [SerializeField] private LayerMask eyeLayer;
    private SpriteRenderer m_spriteRenderer;
    private PolygonCollider2D m_polyCollider;

    private Sprite m_spriteLastFrame;

    [Serializable]
    public class Pupil
    {
        public GameObject pupilObj;
        public Vector2 pupilStartingPos;
    }



    // Start is called before the first frame update
    void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_polyCollider = GetComponent<PolygonCollider2D>();
        m_ph = FindFirstObjectByType<PlayerHand>();

        foreach( Pupil p in pupils)
        {
            p.pupilStartingPos = p.pupilObj.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check if sprite has changed due to animation and if so regenerate collider shape.
        if (m_spriteLastFrame != m_spriteRenderer.sprite)
        {
            if (m_polyCollider != null && m_spriteRenderer != null)
            {
                //Get new physics shape count.
                m_polyCollider.pathCount = m_spriteRenderer.sprite.GetPhysicsShapeCount();

                //Recreate the path component of collider.
                List<Vector2> path = new List<Vector2>();

                //Loop through path and generate based on physics shape.
                for (int i = 0; i < m_polyCollider.pathCount; i++)
                {
                    path.Clear();
                    m_spriteRenderer.sprite.GetPhysicsShape(i, path);
                    m_polyCollider.SetPath(i, path.ToArray());
                }
            }
        }

        foreach (Pupil p in pupils)
        {
            //Change playerhand transform to vector 2.
            var playerHandVec = new Vector2(m_ph.transform.position.x, m_ph.transform.position.y);

            //Cast ray from hand towards eye to find the outer edge of the closest eye.
            var cast = Physics2D.RaycastAll(m_ph.transform.position, p.pupilStartingPos - playerHandVec, Mathf.Infinity, eyeLayer);
            if (cast.Length == 0) { print("Cast couldn't find Eye obj"); return; }

            //Find out which cast collision is closest to its starting point.
            var bestCast = new Vector2(-999999, -999999);

            foreach(RaycastHit2D hit in cast)
            {
                //Get distance from hit to pupil starting pos
                var hitDist = Vector2.Distance(hit.point, p.pupilStartingPos);

                //Update bestCast position if distance is less
                if (hitDist < Vector2.Distance(bestCast, p.pupilStartingPos)) bestCast = hit.point;
            }


            //Convert pupil world space and player hand to 2D directional vector.
            var dir = new Vector2(p.pupilStartingPos.x, p.pupilStartingPos.y) - new Vector2(m_ph.transform.position.x, m_ph.transform.position.y);

            //Calculate point back from closest point offset by the buffer.
            var newPupilPoint = Vector2.Lerp(p.pupilObj.transform.position, bestCast + dir.normalized * bufferFromEdge, eyeSpeed * Time.deltaTime);

            //Move the pupil towards the calculated point by the follow speed.
            p.pupilObj.transform.position = Vector2.Lerp(p.pupilObj.transform.position, newPupilPoint, eyeSpeed * Time.deltaTime);
        }

        m_spriteLastFrame = m_spriteRenderer.sprite;
        
    }
}
