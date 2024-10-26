using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventSystemController : MonoBehaviour
{
    [SerializeField] private Button firstSelected;
    private EventSystem m_eventSystem;


    private void Start()
    {
        m_eventSystem = FindFirstObjectByType<EventSystem>();
        m_eventSystem.firstSelectedGameObject = firstSelected.gameObject;
        m_eventSystem.SetSelectedGameObject(firstSelected.gameObject);
    }
}
