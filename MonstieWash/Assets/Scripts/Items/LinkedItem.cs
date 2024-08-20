using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LinkedItem", menuName = "ScriptableObjects/LinkedItem")]

public class LinkedItem : ScriptableObject
{
    public bool loadOnEnable = false;
    public List<Vector3> positions = new();
    public List<Quaternion> rotations = new();
    public List<Vector2> velocity = new();
    public List<float> angularVelocity = new();
	public List<List<object>> itemData = new();

    public void Reset()
    {
        loadOnEnable = false;
        positions.Clear();
        rotations.Clear();
        itemData.Clear();
    }
}
