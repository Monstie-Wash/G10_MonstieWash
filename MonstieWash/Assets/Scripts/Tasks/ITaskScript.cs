using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITaskScript
{
    public List<object> SaveData();

    public void LoadData(List<object> data);
}