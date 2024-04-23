using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUIManager : MonoBehaviour
{
    [SerializeField] private Canvas m_clipCanvas;
    [SerializeField] private Button m_clipCont;

    [SerializeField] private Canvas m_resultsCanvas;
    [SerializeField] private Button m_resultsCont;




    // Start is called before the first frame update
    private void OnEnable()
    {
        MenuInputManager.Inputs.OnSelect += Inputs_Select;
    }

    private void OnDisable()
    {
        MenuInputManager.Inputs.OnSelect -= Inputs_Select;
    }

    // Update is called once per frame
    private void Inputs_Select()
    {
        UIButtonPressed();
    }

    private void UIButtonPressed()
    {
        if (m_clipCanvas.isActiveAndEnabled)
        {
            m_clipCont.onClick.Invoke();
        }
        else
        {
            m_resultsCont.onClick.Invoke();
        }
           
    }

    public void HideScoreUI()
    {
        Debug.Log("Button activated.");
        m_clipCanvas.gameObject.SetActive(false);
        m_resultsCanvas.gameObject.SetActive(true);
    }

    public void EndResults()
    {
        Debug.Log("Next scene.");
    }    
}
