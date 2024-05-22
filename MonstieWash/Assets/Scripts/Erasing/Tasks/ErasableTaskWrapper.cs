using UnityEngine;

/// <summary>
/// I needed an intermediary to pass data from Erasable struct to TaskTracker and in reverse. I couldn't find an alternative so this is a quick ugly fix. 
/// </summary>
public class ErasableTaskWrapper : TaskData
{
	protected float m_newProgress;
	public override float Progress
    {
        get { return m_progress; }
        set
        {
            m_newProgress = value - m_progress;
            m_progress = value;
        }
    }
    public float NewProgress
    {
        get { return m_newProgress; }
        set { m_newProgress = value; }
    }
}
