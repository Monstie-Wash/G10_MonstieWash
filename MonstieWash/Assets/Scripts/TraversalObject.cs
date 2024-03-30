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
            saveObj = FindObjectOfType<RoomSaver>();
        }
        public void OnMouseDown()
        {
                saveObj.loadNewRoom(targetScene);
        }

    }



