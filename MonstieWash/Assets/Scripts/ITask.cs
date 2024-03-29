using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITask
{
    string taskName {get; set;}
    float taskProgress {get; set; }
}
