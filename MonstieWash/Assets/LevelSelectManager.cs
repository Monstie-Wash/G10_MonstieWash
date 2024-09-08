using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject m_currentPanel;
    [SerializeField] private Button m_firstSelected;

    private EventSystem m_es;
    private Button m_backButton;

    private void Awake()
    {
        m_es = FindFirstObjectByType<EventSystem>();

        m_es.firstSelectedGameObject = m_firstSelected.gameObject;
        m_es.SetSelectedGameObject(m_firstSelected.gameObject);
    }

    private void OnEnable()
    {
        FindBackButton();
        InputManager.Instance.OnCancel += Inputs_Back;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnCancel -= Inputs_Back;
    }

    public void ShowPanel(GameObject panel)
    {
        //Save the last selected button here?
        
        panel.SetActive(true);

        m_currentPanel.SetActive(false);
        m_currentPanel = panel;

        m_backButton = FindBackButton();

        m_currentPanel.transform.SetAsLastSibling();

        m_es.SetSelectedGameObject(FindFirstObjectByType<Button>().gameObject);
    }

    private Button FindBackButton()
    {
        var Buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button b in Buttons)
        {
            if (b.gameObject.name == "Back" && b.gameObject.activeInHierarchy)
                return b;
        }

        Debug.LogError("Back Button not found.");
        return null;
    }

    private void Inputs_Back()
    {
        m_backButton.onClick.Invoke();
    }
}
