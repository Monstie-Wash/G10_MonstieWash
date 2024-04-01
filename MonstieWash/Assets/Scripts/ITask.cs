using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITask
{
    public string TaskName { get; set; }
    public float TaskProgress { get; set; }
    public float NewProgress { get; set; }
}
