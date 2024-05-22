using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeFollowObj : MonoBehaviour
{
    private PlayerHand m_ph;
    private Vector3 pupilStartingPos;

    public float eyeSpeed;
    public float trackingSpeed;
    public float bufferFromEdge;
    public GameObject pupil;
    public LayerMask eyeLayer;
    public SpriteRenderer spriteRenderer;
    public PolygonCollider2D polyCollider;

    private Sprite m_spriteLastFrame;

    // Start is called before the first frame update
    void Start()
    {
        m_ph = FindFirstObjectByType<PlayerHand>();
        pupilStartingPos = pupil.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Have to check pupil exists within animation as some eyes don't have one.
        pupil = this.transform.GetChild(0).gameObject;
        if (pupil == null) return;

        //Check if sprite has changed due to animation and if so regenerate collider shape.
        if (m_spriteLastFrame != spriteRenderer.sprite)
        {
            if (polyCollider != null && spriteRenderer != null)
            {
                //Get new physics shape count.
                polyCollider.pathCount = spriteRenderer.sprite.GetPhysicsShapeCount();

                //Recreate the path component of collider.
                List<Vector2> path = new List<Vector2>();

                //Loop through path and generate based on physics shape.
                for (int i = 0; i < polyCollider.pathCount; i++)
                {
                    path.Clear();
                    spriteRenderer.sprite.GetPhysicsShape(i, path);
                    polyCollider.SetPath(i, path.ToArray());
                }
            }
        }

        //Cast ray from hand towards eye to find the outer edge of the eye.
        var cast = Physics2D.Raycast(m_ph.transform.position, pupilStartingPos - m_ph.transform.position, Mathf.Infinity, eyeLayer);
        if (!cast) { print("Cast couldn't find Eye obj"); return; }

        
        //Convert pupil world space and player hand to 2D directional vector.
        var dir = new Vector2(pupilStartingPos.x, pupilStartingPos.y) - new Vector2(m_ph.transform.position.x, m_ph.transform.position.y);
        //Calculate point back from closest point offset by the buffer.
        var newPupilPoint = Vector2.Lerp(pupil.transform.position, cast.point + dir.normalized * bufferFromEdge, eyeSpeed * Time.deltaTime);

        //Move the pupil towards the calculated point by the follow speed.
        pupil.transform.position = Vector2.Lerp(pupil.transform.position, newPupilPoint, eyeSpeed * Time.deltaTime);


        m_spriteLastFrame = spriteRenderer.sprite;
        
    }
}
