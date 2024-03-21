using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSaver : MonoBehaviour
{
    List<string> roomsLoaded;
    Scene currentscene;
    public GameObject testSpawner;

    public void loadNewRoom(string target)
    {
        var roomLoaded = false;

        //Unload current room.
        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);
        foreach (GameObject ob in rootObjects)
        {
            ob.SetActive(false);
        }
        //Check if room has already been loaded before
        foreach (string room in roomsLoaded)
        {
            if (room == target) 
            {
                roomLoaded = true;

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
        if (!roomLoaded)
        {

            //If room has never been loaded, load it and add it to the list of loaded rooms.
            roomsLoaded.Add(target);
            SceneManager.LoadScene(target, LoadSceneMode.Additive);
            currentscene = SceneManager.GetSceneByName(target);
        }

    }



    private void Start()
    {
        roomsLoaded = new List<string>();
        roomsLoaded.Add("NavTest");
        SceneManager.LoadScene("NavTest", LoadSceneMode.Additive);
        currentscene = SceneManager.GetSceneByName("NavTest"); 
    }

    private void Update()
    {
        if (currentscene.isLoaded)
        {
            SceneManager.SetActiveScene(currentscene);
        }

        //Section for testing Saving;
        if (Input.GetMouseButtonDown(2))
        {
            var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Instantiate(testSpawner, point, Quaternion.identity);
        }
    }

}
