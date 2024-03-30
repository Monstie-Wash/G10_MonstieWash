using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITask
{
    string TaskName { get; set; }
    float TaskProgress { get; set; }
    float NewProgress { get; set; }
}
