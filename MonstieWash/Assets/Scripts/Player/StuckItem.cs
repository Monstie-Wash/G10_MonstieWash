using System.Collections;
using UnityEngine;

public class StuckItem : MonoBehaviour
{
    [SerializeField] private float startMaxRotation = 20f;
    [SerializeField] private float endMaxRotation = 90f;
    [SerializeField] private int wiggleCount = 6;
    [SerializeField] private float grabDistanceMultiplier = 1.5f;

    private Rigidbody2D m_rb;
    private int m_initialWiggleCount;
    private Coroutine m_OOBCheckRoutine;
    private float m_initialRotation;
    private Transform m_initialParent;

    public bool Stuck { get; private set; } = true;
    public float GrabDistance { get; private set; }
    public float InitialRotation { get { return m_initialRotation; } }
    public float MaxRotation
    {
        get
        {
            return Mathf.Lerp(startMaxRotation, endMaxRotation, (m_initialWiggleCount - wiggleCount + 1) / (float)m_initialWiggleCount);
        }
    }
    public Rigidbody2D Rb { get { return m_rb; } }
    public int WiggleCount { get {  return wiggleCount; } }
    public Transform InitialParent {  get { return m_initialParent; } }


    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        GrabDistance = grabDistanceMultiplier * transform.lossyScale.x;
        m_initialWiggleCount = wiggleCount;
        m_initialRotation = transform.rotation.eulerAngles.z;
        m_initialParent = transform.parent;
    }

    /// <summary>
    /// Set the stuck field, triggering the appropriate actions.
    /// </summary>
    /// <param name="stuck">The value t set the stuck field to.</param>
    public void SetStuck(bool stuck)
    {
        Stuck = stuck;

        if (Stuck)
        {
            m_rb.bodyType = RigidbodyType2D.Kinematic;
            StopCoroutine(m_OOBCheckRoutine);
        }
        else
        {
            m_rb.bodyType = RigidbodyType2D.Dynamic;
            transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
            StartCoroutine(CheckOOB());
        }
    }

    /// <summary>
    /// Applies a wiggle.
    /// </summary>
    /// <returns>Whether this wiggle unstuck the item.</returns>
    public bool Wiggle()
    {
        wiggleCount--;

        if (wiggleCount <= 0)
        {
            SetStuck(false);
            return true;
        }

        return false;
    }

    private IEnumerator CheckOOB()
    {
        var cameraWidthInWorldUnits = Camera.main.orthographicSize * Camera.main.aspect;
        var cameraHeightInWorldUnits = Camera.main.orthographicSize;

        while (Mathf.Abs(transform.position.x) < cameraWidthInWorldUnits + 10f && Mathf.Abs(transform.position.y) < cameraHeightInWorldUnits + 10f)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}