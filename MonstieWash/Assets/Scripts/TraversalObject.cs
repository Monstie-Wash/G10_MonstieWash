using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TraversalObject : MonoBehaviour
{
    [SerializeField] RoomSaver saveObj;
    [SerializeField] public string targetScene;

    public void Start()
    {
        saveObj = FindFirstObjectByType<RoomSaver>();
    }
    public void OnMouseDown()
    {
        saveObj.LoadNewScene(targetScene);
    }
}