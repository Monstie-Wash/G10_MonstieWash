using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject smallParent;
    [SerializeField] private GameObject bigParent;
    [SerializeField] private GameObject bigElement;
    [SerializeField] private Button backButton;
    [SerializeField] private LevelSelectManager levelSelectManager;

    private EventSystem m_eventSystem;
    private Button m_selectBtn;

    private void Awake()
    {
        m_selectBtn = GetComponent<Button>();
        m_selectBtn.onClick.AddListener(EnableElement);
        m_eventSystem = FindFirstObjectByType<EventSystem>();
    }

    private void EnableElement()
    {
        smallParent.SetActive(false);
        bigParent.SetActive(true);
        bigElement.SetActive(true);
        backButton.onClick.AddListener(DisableElement);

        m_eventSystem.SetSelectedGameObject(bigElement.GetComponentInChildren<Button>().gameObject);
        levelSelectManager.FindBack();
    }

    public void DisableElement()
    {
        smallParent.SetActive(true);
        bigParent.SetActive(false);
        bigElement.SetActive(false);
        backButton.onClick.RemoveListener(DisableElement);

        m_eventSystem.SetSelectedGameObject(smallParent.GetComponentInChildren<Button>().gameObject);
        levelSelectManager.FindBack();
    }


}
