using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemLinker : MonoBehaviour
{
    [SerializeField] private LinkMethod link;
    [SerializeField] private LinkedItem linkObject;
    
    private TaskTracker m_taskTracker;
    private List<ITaskScript> m_taskScript = new();

    private enum LinkMethod
    {
        Group,
        Single
    }

    private void Awake()
    {
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
        GameSceneManager.Instance.OnSceneSwitch += SaveItem;
        GameSceneManager.Instance.OnSceneChanged += LoadItem;
        GameSceneManager.Instance.OnLevelEnd += CleanUp;
        GameSceneManager.Instance.OnRestartGame += CleanUp;
    }

    private void OnDisable()
    {
        GameSceneManager.Instance.OnSceneSwitch -= SaveItem;
        GameSceneManager.Instance.OnSceneChanged -= LoadItem;
        GameSceneManager.Instance.OnLevelEnd -= CleanUp;
        GameSceneManager.Instance.OnRestartGame -= CleanUp;
    }

    private void OnApplicationQuit()
    {
        CleanUp();
    }

/// <summary>
/// Saves data from linked item in scene to link object
/// </summary>
    public void SaveItem()
    {
		linkObject.Reset();
		if (link == LinkMethod.Group)
        {
            foreach (Transform child in transform)
            {
                linkObject.positions.Add(FlipItemPos(child));
                linkObject.rotations.Add(FlipItemRot(child));
                linkObject.velocity.Add(FlipItemVel(child.GetComponent<Rigidbody2D>().velocity));
                linkObject.angularVelocity.Add(FlipItemAng(child.GetComponent<Rigidbody2D>().angularVelocity));

                linkObject.itemData.Add(new List<object>(child.GetComponent<ITaskScript>().SaveData()));
            }
        }
        else
        {
            linkObject.positions.Add(FlipItemPos(transform));
            linkObject.rotations.Add(FlipItemRot(transform));
            linkObject.velocity.Add(FlipItemVel(GetComponent<Rigidbody2D>().velocity));
            linkObject.angularVelocity.Add(GetComponent<Rigidbody2D>().angularVelocity);

            linkObject.itemData.Add(new List<object>(m_taskScript[0].SaveData()));
        }
        linkObject.loadOnEnable = true;
    }

/// <summary>
/// Loads data from link object to linked item in scene
/// </summary>
    public void LoadItem()
    {
        if (linkObject.loadOnEnable == true) 
        {
            if (link == LinkMethod.Group)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).position = linkObject.positions[i];
                    transform.GetChild(i).rotation = linkObject.rotations[i];
					transform.GetChild(i).GetComponent<Rigidbody2D>().velocity = linkObject.velocity[i];
					transform.GetChild(i).GetComponent<Rigidbody2D>().angularVelocity = linkObject.angularVelocity[i];

					transform.GetChild(i).GetComponent<ITaskScript>().LoadData(linkObject.itemData[i]);
                }
            }
            else
            {
                transform.position = linkObject.positions[0];
                transform.rotation = linkObject.rotations[0];
				transform.GetComponent<Rigidbody2D>().velocity = linkObject.velocity[0];
				transform.GetComponent<Rigidbody2D>().angularVelocity = linkObject.angularVelocity[0];

				m_taskScript[0].LoadData(linkObject.itemData[0]);
            }
            linkObject.loadOnEnable = false;
        }
    }

    //If a scene with linked objects loads incorrectly it's probably because a previous scene was exited/closed without CleanUp being run, leaving bad data in the link scriptableObj
    private void CleanUp()
    {
        GameSceneManager.Instance.OnSceneSwitch -= SaveItem;
        GameSceneManager.Instance.OnSceneChanged -= LoadItem;
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

    private Vector2 FlipItemVel(Vector2 vel)
    {
        return new Vector2(-vel.x, vel.y);
    }
    
    private float FlipItemAng(float ang) 
    {
        return -ang;
    }
}
