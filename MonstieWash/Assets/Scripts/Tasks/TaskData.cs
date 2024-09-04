using UnityEngine;

public enum TaskType
{
	Default,
	Dirt,
    Bone,
}

public class TaskData : MonoBehaviour
{
	[SerializeField] private string m_id;

    [SerializeField] private float m_progress;

    [SerializeField] private bool m_complete;
    [SerializeField] protected float m_threshold;

	[SerializeField] private Transform m_container;

	[SerializeField] private float m_score = 9.85f;

    [SerializeField] private TaskType type;

	public string Id { get => m_id; set => m_id = value; }
    public float Progress { get => m_progress; set => m_progress = value; }
    public bool Complete { get => m_complete; set => m_complete = value; }
    public float Threshold { get => m_threshold; set => m_threshold = value; }
    public Transform Container { get => m_container; }
	public float Score { get => m_score; }
	public TaskType Type { get => type; set => type = value; }

	private void Awake()
	{
        m_container = gameObject.transform;
        m_id = m_container.name;
        m_progress = 0f;
        m_complete = false;
	}
}
