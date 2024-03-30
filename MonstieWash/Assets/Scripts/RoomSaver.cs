using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSaver : MonoBehaviour
{
    //public GameObject testSpawner;
    public string firstRoomToLoad;

    private List<string> roomsLoaded;
    private Scene currentscene;
    
    private void Start()
    {
        roomsLoaded = new List<string>();
        roomsLoaded.Add(firstRoomToLoad);
        SceneManager.LoadScene(firstRoomToLoad, LoadSceneMode.Additive);
        currentscene = SceneManager.GetSceneByName(firstRoomToLoad); 
    }

    private void Update()
    {
        if (currentscene.isLoaded)
        {
            SceneManager.SetActiveScene(currentscene);
        }

        /* Section for testing Saving
        if (Input.GetMouseButtonDown(2))
        {
            var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Instantiate(testSpawner, point, Quaternion.identity);
        }
        */
    }

    public void LoadNewScene(string target)
    {
        var sceneLoaded = false;

        //Unload current room.
        var rootObjects = new List<GameObject>();
        var scene = SceneManager.GetActiveScene();

        scene.GetRootGameObjects(rootObjects);

        foreach (var rootObj in rootObjects)
        {
            rootObj.SetActive(false);
        }

        //Check if room has already been loaded before
        foreach (var room in roomsLoaded)
        {
            if (room == target)
            {
                sceneLoaded = true;

                //ReOpen the already loaded room.
                List<GameObject> rotObjects = new List<GameObject>();

                currentscene = SceneManager.GetSceneByName(target);
                currentscene.GetRootGameObjects(rotObjects);

                foreach (GameObject ob in rotObjects)
                {
                    ob.SetActive(true);
                }

                return;
            }
        }

        if (!sceneLoaded)
        {
            //If room has never been loaded, load it and add it to the list of loaded rooms.
            roomsLoaded.Add(target);
            SceneManager.LoadScene(target, LoadSceneMode.Additive);
            currentscene = SceneManager.GetSceneByName(target);
        }
    }
}
