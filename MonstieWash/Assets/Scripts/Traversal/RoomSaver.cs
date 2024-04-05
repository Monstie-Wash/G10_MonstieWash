using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSaver : MonoBehaviour
{
    [SerializeField] private Object startingScene;

    private List<SceneAsset> m_roomsLoaded = new();
    private SceneAsset m_startingScene;
    private Scene m_currentScene;

    private void Awake()
    {
        if (startingScene == null) Debug.LogError($"Target scene not assigned for {name}!");
        else if (!(startingScene is SceneAsset)) Debug.LogError($"Target scene is not a scene for {name}!");
        else m_startingScene = startingScene as SceneAsset;
    }

    private void Start()
    {
        LoadNewScene(m_startingScene);
    }

    public void LoadScene(SceneAsset target)
    {
        var sceneLoaded = false;

        //Unload current room.
        var rootObjects = new List<GameObject>();

        m_currentScene.GetRootGameObjects(rootObjects);

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

                m_currentScene = SceneManager.GetSceneByName(target.name);
                m_currentScene.GetRootGameObjects(rotObjects);

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
            LoadNewScene(target);
        }
    }

    private void LoadNewScene(SceneAsset scene)
    {
        m_roomsLoaded.Add(scene);

        SceneManager.LoadScene(scene.name, LoadSceneMode.Additive);

        m_currentScene = SceneManager.GetSceneByName(scene.name);
    }
}
