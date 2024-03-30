using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// I needed an intermediary to pass data from Erasable struct to TaskTracker and in reverse. I couldn't find an alternative so this is a quick ugly fix. 
/// </summary>
public class IErasable : MonoBehaviour, ITask
{
    [SerializeField] private float _taskProgress;
    [SerializeField] private float _newProgress;
    public string TaskName { get; set; }
    public float TaskProgress
    {
        get { return _taskProgress; }
        set
        {
            _newProgress = value - _taskProgress;
            _taskProgress = value;
        }
    }
    public float NewProgress
    {
        get { return _newProgress; }
        set {;}
    }

    public void Awake()
    {
        TaskName = "";
        TaskProgress = 0f;
    }
}
