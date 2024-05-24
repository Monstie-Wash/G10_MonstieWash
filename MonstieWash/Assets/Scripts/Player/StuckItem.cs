using System.Collections;
using UnityEngine;

public class StuckItem : MonoBehaviour
{
    [SerializeField] private float startMaxRotation = 20f;
    [SerializeField] private float endMaxRotation = 90f;
    [SerializeField] private int wiggleCount = 6;
    [SerializeField] private float grabDistanceMultiplier = 1.5f;
    [SerializeField] private AnimationCurve greenFadeCurve;
    [SerializeField] private float greenFadeTime = 1f;

    private Rigidbody2D m_rb;
    private int m_initialWiggleCount;
    private Coroutine m_OOBCheckRoutine;
    private float m_initialRotation;
    private Transform m_initialParent;
    private TaskTracker m_taskTracker;
    private TaskData m_pickingTask;
    private SpriteRenderer m_spriteRenderer;
    private Coroutine m_colourFadeRoutine;
    private SoundPlayer m_soundPlayer;

    public bool Stuck { get; private set; } = true;
    public float GrabDistance { get; private set; }
    public float InitialRotation { get { return m_initialRotation; } }
    public float MaxRotation
    {
        get
        {
            return Mathf.Lerp(startMaxRotation, endMaxRotation, (m_initialWiggleCount - wiggleCount) / (float)m_initialWiggleCount);
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
        m_taskTracker = FindFirstObjectByType<TaskTracker>();
        m_pickingTask = GetComponent<TaskData>();
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_soundPlayer = GetComponent<SoundPlayer>();
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
            m_OOBCheckRoutine = StartCoroutine(CheckOOB());
            m_soundPlayer.PlaySound(true);
        }
    }

    /// <summary>
    /// Applies a wiggle.
    /// </summary>
    /// <returns>Whether this wiggle unstuck the item.</returns>
    public bool Wiggle()
    {
        wiggleCount--;
        m_pickingTask.Progress = 100f*(m_initialWiggleCount - wiggleCount)/m_initialWiggleCount;
        m_taskTracker.UpdateTaskTracker(m_pickingTask);

        if (m_colourFadeRoutine != null) StopCoroutine(m_colourFadeRoutine);
        m_colourFadeRoutine = StartCoroutine(FadeColour());

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

        gameObject.SetActive(false);
    }

    private IEnumerator FadeColour()
    {
        var t = greenFadeTime;
        var currentColor = Color.green;

        while (t > 0f)
        {
            currentColor.r = greenFadeCurve.Evaluate(t);
            currentColor.b = greenFadeCurve.Evaluate(t);

            m_spriteRenderer.color = currentColor;

            t -= Time.deltaTime;

            yield return null;
        }
    }
}
