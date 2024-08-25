using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLinker : MonoBehaviour
{
    [SerializeField] private LinkMethod link;
    [SerializeField] private LinkedItem linkObject;
    
    private GameSceneManager m_gameSceneManager;
    private TaskTracker m_taskTracker;
    private List<ITaskScript> m_taskScript = new();

    private enum LinkMethod
    {
        Group,
        Single
    }

    private void Awake()
    {
        m_gameSceneManager = FindFirstObjectByType<GameSceneManager>();
        m_taskTracker = FindFirstObjectByType<TaskTracker>();

        ITaskScript taskScript;
        if (link == LinkMethod.Group)
        {
            foreach (Transform child in transform)
            {
                taskScript = child.GetComponent<ITaskScript>();
                if (taskScript != null)
                {
                    m_taskScript.Add(taskScript);
                }
                else
                {
                    Debug.LogError($"ItemLinker on {name} is setup incorrectly. Could not find a task script on child {child.name}!");
                }
            }
        }
        else
        {
            taskScript = GetComponent<ITaskScript>();
            if (taskScript != null)
            {
                m_taskScript.Add(taskScript);
            }
            else
            {
                Debug.LogError($"ItemLinker on {name} is setup incorrectly. Could not find a task script on {name}!");
            }
        }
    }

        private void OnEnable()
    {
        m_gameSceneManager.OnSceneSwitch += SaveItem;
        GameSceneManager.OnSceneChanged += LoadItem;
        m_gameSceneManager.OnLevelEnd += CleanUp;
        m_gameSceneManager.OnRestartGame += CleanUp;
    }

    private void OnDisable()
    {
        m_gameSceneManager.OnSceneSwitch -= SaveItem;
        GameSceneManager.OnSceneChanged -= LoadItem;
        m_gameSceneManager.OnLevelEnd -= CleanUp;
        m_gameSceneManager.OnRestartGame -= CleanUp;
    }

    private void OnApplicationQuit()
    {
        CleanUp();
    }

/// <summary>
/// Saves data from linked item in scene to link object
/// </summary>
    private void SaveItem()
    {
        linkObject.Reset();
        if (link == LinkMethod.Group)
        {
            foreach (Transform child in transform)
            {
                linkObject.positions.Add(FlipItemPos(child));
                linkObject.rotations.Add(FlipItemRot(child));
                linkObject.itemData.Add(new List<object>(child.GetComponent<ITaskScript>().SaveData()));
            }
        }
        else
        {
            linkObject.positions.Add(FlipItemPos(transform));
            linkObject.rotations.Add(FlipItemRot(transform));
            linkObject.itemData.Add(new List<object>(m_taskScript[0].SaveData()));
        }
        linkObject.loadOnEnable = true;
    }

/// <summary>
/// Loads data from link object to linked item in scene
/// </summary>
    private void LoadItem()
    {
        if (linkObject.loadOnEnable == true) 
        {
            if (link == LinkMethod.Group)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).position = linkObject.positions[i];
                    transform.GetChild(i).rotation = linkObject.rotations[i];
                    transform.GetChild(i).GetComponent<ITaskScript>().LoadData(linkObject.itemData[i]);
                }
            }
            else
            {
                transform.position = linkObject.positions[0];
                transform.rotation = linkObject.rotations[0];
                m_taskScript[0].LoadData(linkObject.itemData[0]);
            }
            linkObject.loadOnEnable = false;
        }
    }

    //If a scene with linked objects loads incorrectly it's probably because a previous scene was exited/closed without CleanUp being run, leaving bad data in the link scriptableObj
    private void CleanUp()
    {
        m_gameSceneManager.OnSceneSwitch -= SaveItem;
        GameSceneManager.OnSceneChanged -= LoadItem;
        linkObject.Reset();
    }

    private Vector3 FlipItemPos(Transform item)
    {
        return new Vector3(-item.position.x, item.position.y, item.position.z);
    }

    private Quaternion FlipItemRot(Transform item)
    {
        return Quaternion.Euler(0f, 0f, 90f - (item.rotation.eulerAngles.z - 90f));
    }
    
}
