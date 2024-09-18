using UnityEngine;

public enum TaskType
{
	Dirt,
    StuckItem,
}

public class TaskData : MonoBehaviour
{
	[SerializeField] private string id;
    [SerializeField] private float progress;
    [SerializeField] private float threshold;
	[SerializeField] private Transform container;
	[SerializeField] private float score = 9.85f;
    [SerializeField] private TaskType taskType;

	public string Id { get => id; set => id = value; }
    public float Progress { get => progress; set => progress = value; }
    public bool Complete { get => m_progress >= m_threshold; }
    public float Threshold { get => threshold; set => threshold = value; }
    public Transform Container { get => container; }
	public float Score { get => score; }
	public TaskType TaskType { get => taskType; set => taskType = value; }

	private void Awake()
	{
        container = gameObject.transform;
        id = container.name;
        progress = 0f;
	}
}
