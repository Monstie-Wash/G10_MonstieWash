using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSaver : MonoBehaviour
{
    public string firstRoomToLoad;

    private List<string> m_roomsLoaded = new();
    private Scene m_currentscene;

    private void Start()
    {
        m_roomsLoaded.Add(firstRoomToLoad);
        SceneManager.LoadScene(firstRoomToLoad, LoadSceneMode.Additive);
        m_currentscene = SceneManager.GetSceneByName(firstRoomToLoad); 
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene() != m_currentscene && m_currentscene.isLoaded)
        {
            SceneManager.SetActiveScene(m_currentscene);
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
        foreach (var room in m_roomsLoaded)
        {
            if (room == target)
            {
                sceneLoaded = true;

                //ReOpen the already loaded room.
                var rotObjects = new List<GameObject>();

                m_currentscene = SceneManager.GetSceneByName(target);
                m_currentscene.GetRootGameObjects(rotObjects);

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
            m_roomsLoaded.Add(target);
            SceneManager.LoadScene(target, LoadSceneMode.Additive);
            m_currentscene = SceneManager.GetSceneByName(target);
        }
    }
}
