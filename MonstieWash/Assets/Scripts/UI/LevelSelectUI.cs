using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject smallParent;
    [SerializeField] private GameObject bigParent;
    [SerializeField] private GameObject bigElement;
    [SerializeField] private Button backButton;

    private Button m_selectBtn;

    private void Awake()
    {
        m_selectBtn = GetComponent<Button>();
        m_selectBtn.onClick.AddListener(EnableElement);
    }

    private void EnableElement()
    {
        smallParent.SetActive(false);
        bigParent.SetActive(true);
        bigElement.SetActive(true);
        backButton.onClick.AddListener(DisableElement);
    }

    public void DisableElement()
    {
        smallParent.SetActive(true);
        bigParent.SetActive(false);
        bigElement.SetActive(false);
        backButton.onClick.RemoveListener(DisableElement);
    }
}
