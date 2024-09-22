using UnityEngine;

public class TaskData : MonoBehaviour
{
    public enum TypeOfTask { Dirt, StuckItem };

	[SerializeField] protected string m_id;

    [SerializeField] protected float m_progress;

    [SerializeField] protected bool m_complete;
    [SerializeField] protected float m_threshold;

	[SerializeField] protected Transform m_container;

	[SerializeField] protected float m_score = 9.85f;

    [SerializeField] protected TypeOfTask m_taskType;

	public string Id { get => m_id; set => m_id = value; }
    public float Progress { get => m_progress; set => m_progress = value; }
    public bool Complete { get => m_complete; set => m_complete = value; }
    public float Threshold { get => m_threshold; set => m_threshold = value; }
    public Transform Container { get => m_container; }
	public float Score { get => m_score; }
    public TypeOfTask TaskType { get => m_taskType; }

	private void OnAwake()
	{
        m_container = gameObject.transform;
        m_id = m_container.name;
        m_progress = 0f;
        m_complete = false;
	}
}
