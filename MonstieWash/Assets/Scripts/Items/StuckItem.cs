using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class StuckItem : MonoBehaviour, ITaskScript
{
    [SerializeField] private float startMaxRotation = 20f;
    [SerializeField] private float endMaxRotation = 90f;
    [SerializeField] private int wiggleCount = 6;
    [SerializeField] private float grabDistanceMultiplier = 1.5f;
    [SerializeField] private AnimationCurve greenFadeCurve;
    [SerializeField] private float greenFadeTime = 1f;
    [SerializeField] private float boneBuffer = 10f;

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
        if (transform.lossyScale.x < 0f) Debug.LogWarning($"{name} has a negative X-scale! This will break it! Make sure it is positive and use the 'Mirror' button to flip it.");
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
            m_spriteRenderer.sortingLayerName = "Tools";
            m_spriteRenderer.sortingOrder = 0;
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

        while (!(transform.position.y > cameraHeightInWorldUnits + boneBuffer || transform.position.y < -cameraHeightInWorldUnits - boneBuffer))
        {
            yield return null;
        }

        Debug.Log("Do the bone go?");
        gameObject.SetActive(false);
    }

    private IEnumerator FadeColour()
    {
        var t = greenFadeTime;
        var currentColor = m_spriteRenderer.color;

        while (t > 0f)
        {
            currentColor.a = greenFadeCurve.Evaluate(t);

            m_spriteRenderer.color = currentColor;

            t -= Time.deltaTime;

            yield return null;
        }
    }

    public List<object> SaveData()
    {
        List<object> data = new();

        data.Add(Stuck);
        data.Add(wiggleCount);

        return data;
    }

    public void LoadData(List<object> data)
    {
        Stuck = (bool)data[0];
        wiggleCount = (int)data[1];

        m_pickingTask.Progress = 100f*(m_initialWiggleCount - wiggleCount)/m_initialWiggleCount;
        m_taskTracker.UpdateTaskTracker(m_pickingTask);
        return;
    }
}

#region Custom Editor
#if UNITY_EDITOR
[CustomEditor(typeof(StuckItem))]
public class StuckItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();

        EditorGUILayout.Space(10f);

        StuckItem item = (StuckItem)target;
        if (GUILayout.Button("Mirror"))
        {
            item.transform.rotation = Quaternion.Euler(0f, 0f, GetFlippedDir(item.transform.rotation.eulerAngles.z));
            item.transform.position = new Vector3(-item.transform.position.x, item.transform.position.y, item.transform.position.z);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private float GetFlippedDir(float dir)
    {
        return 90f - (dir - 90f);
    }
}
#endif
#endregion
