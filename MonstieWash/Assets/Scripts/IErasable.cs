using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// I needed an intermediary to pass data from Erasable struct to TaskTracker and in reverse. I couldn't find an alternative so this is a quick ugly fix. 
/// </summary>
public class IErasable : MonoBehaviour, ITask
{
    public string taskName { get; set; }
    public float taskProgress { get; set; }

    public void Awake()
    {
        taskName = "";
        taskProgress = 0f;
    }
}
