using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSaver : MonoBehaviour
{
    //public GameObject testSpawner;
    public string firstRoomToLoad;

    private List<string> roomsLoaded = new();
    private Scene currentscene;

    private void Start()
    {
        roomsLoaded.Add(firstRoomToLoad);
        SceneManager.LoadScene(firstRoomToLoad, LoadSceneMode.Additive);
        currentscene = SceneManager.GetSceneByName(firstRoomToLoad); 
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene() != currentscene && currentscene.isLoaded)
        {
            SceneManager.SetActiveScene(currentscene);
        }
    }

    public void LoadNewScene(string target)
    {
        var sceneLoaded = false;

        //Unload current room.
        var rootObjects = new List<GameObject>();
        var scene = SceneManager.GetActiveScene();

        scene.GetRootGameObjects(rootObjects);

        foreach (var ob in rootObjects)
        {
            ob.SetActive(false);
        }

        //Check if room has already been loaded before
        foreach (var room in roomsLoaded)
        {
            if (room == target)
            {
                sceneLoaded = true;

                //ReOpen the already loaded room.
                var rotObjects = new List<GameObject>();

                currentscene = SceneManager.GetSceneByName(target);
                currentscene.GetRootGameObjects(rotObjects);

                foreach (var ob in rotObjects)
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
