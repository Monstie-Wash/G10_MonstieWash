using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class ScriptedEventsManager : MonoBehaviour
{
    public static ScriptedEventsManager Instance;

    private MonsterBrain m_monsterBrain;

    [SerializeField] bool debugMode;    // whether to print debug messages

    [SerializeField] MoodType monsterAngryMood; // annoying to have to do this, but we don't have a better way to detect a monster's angry mood at the moment

    public enum ScriptedEventType { None, SetAngry }; // Which scripted events exist
    private Dictionary<ScriptedEventType, bool> m_eventPermission = new Dictionary<ScriptedEventType, bool>(); // Used to reference whether an event is available to a subscriber
    private Dictionary<GameObject, Dictionary<ScriptedEventType, bool>> m_subscribers = new Dictionary<GameObject, Dictionary<ScriptedEventType, bool>>(); // Used to reference subscribers and permissions

    private void Awake()
    {
        //Ensure there's only one
        if (Instance == null) Instance = this;
        else Destroy(this);

        m_monsterBrain = FindFirstObjectByType<MonsterBrain>();
    }

    public void Subscribe(GameObject subscriber, params ScriptedEventType[] permissions)
    {
        if (!m_subscribers.ContainsKey(subscriber))     // subscriber can only be subscribed once
        {
            // create and add new subscriber
            Dictionary<ScriptedEventType, bool> myPerms = new Dictionary<ScriptedEventType, bool>();
            m_subscribers.Add(subscriber, myPerms);

            // create and add perms for new subscriber
            foreach (ScriptedEventType eventType in Enum.GetValues(typeof(ScriptedEventType)))
            {
                if (permissions.Contains(eventType)) m_subscribers[subscriber].Add(eventType, true);
                else m_subscribers[subscriber].Add(eventType, false);
            }
        }
    }

    public void Unsubscribe(GameObject subscriber)
    {
        if (m_subscribers.ContainsKey(subscriber)) m_subscribers.Remove(subscriber);
    }

    public void ModifyPermission(GameObject subscriber, ScriptedEventType type, bool newVal)
    {
        if (m_subscribers.ContainsKey(subscriber))
        {
            m_subscribers[subscriber][type] = newVal;
        }
    }

    public void RunScriptedEvent(GameObject caller, ScriptedEventType type)
    {
        if (m_subscribers.ContainsKey(caller))  // if caller is subscribed
        {
            switch (type)
            {
                case ScriptedEventType.SetAngry:
                    if (m_subscribers[caller][ScriptedEventType.SetAngry])  // if caller has permission to execute this scripted event
                    {
                        SetAngry(caller);
                    }
                    else // caller doesn't have permission
                    {
                        if (debugMode) Debug.LogWarning($"{caller.name} tried to call scripted event SetAngry but is missing permissions to do so!");
                    }
                    break;
                default:
                    break;
            }
        }
        else // caller isn't subscribed
        {
            if (debugMode) Debug.LogWarning($"{caller.name} tried to call a scripted event but isn't subscribed!");
        }
    }

    private void SetAngry(GameObject caller)
    {
        // currently sets the monster to its maximum anger
        float maxAnger = monsterAngryMood.MoodUpperLimit;
        m_monsterBrain.UpdateMood(maxAnger, monsterAngryMood);
    }
}