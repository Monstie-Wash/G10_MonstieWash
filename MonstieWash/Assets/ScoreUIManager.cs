using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public struct PHTask
{
    public string name;
    public bool complete;
    public int score;

    public PHTask(string _name, bool _complete, int _score)
    {  name = _name; complete = _complete; score = _score; }
}

public class ScoreUIManager : MonoBehaviour
{
    [SerializeField] private Image m_clipboard;
    [SerializeField] private GameObject m_lineItem;
    [SerializeField] private TextMeshProUGUI m_totalScore;

    [SerializeField][Range(0.0f, 5.0f)] private float m_spacing;

    /// <summary>
    /// Placeholder. Suggest Json to player prefs.
    /// </summary>
    [SerializeField] private List<PHTask> m_tasks;
    [SerializeField] private List<GameObject> m_LIList;

    // Start is called before the first frame update
    private void OnEnable()
    {
        m_tasks = PlaceholderTaskFill();

        Transform clipTransform = m_clipboard.transform;
        RectTransform LITransform;

        TextMeshProUGUI name = null;
        TextMeshProUGUI score = null;

        int totalScore = 0;

        for (int i = 0; i < m_tasks.Count; i++)
        {
            m_LIList.Add(Instantiate(m_lineItem, clipTransform));
            LITransform = m_LIList[i].GetComponent<RectTransform>();
            LITransform.anchoredPosition = new Vector2(0, LITransform.rect.height / m_spacing * -i);

            name = m_LIList[i].transform.Find("TaskName").GetComponent<TextMeshProUGUI>();
            

            name.text = m_tasks[i].name;
            

            //Put the score in the score.
            if (m_tasks[i].complete)
            {
                score = m_LIList[i].transform.Find("TaskScore").GetComponent<TextMeshProUGUI>();
                score.text = $"${m_tasks[i].score.ToString()}";

                //Add to score total
                totalScore += m_tasks[i].score;
            }
        }



        //Animate clipboard

        //Animate crossing out

        //Setactive Total Score
        m_totalScore.text = $"${totalScore.ToString()}";
    }

    private void OnDisable()
    {
        m_tasks.Clear();
    }

    /// <summary>
    /// Fills the task list and returns it.
    /// </summary>
    /// <returns>List of tasks with score multiplier</returns>
    private List<PHTask> PlaceholderTaskFill()
    {
        List<PHTask> ph_tasks = new List<PHTask>();

        for (int i = 0; i < 8; i++)
        {
            ph_tasks.Add(new PHTask("Task Name Here", true, 10));
        }

        return ph_tasks;
    }

    public void OnLevelExit()
    {
        Debug.Log("LEVEL EXIT");
    }    
}
