using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct PHTask
{
    public string name;
    public bool complete;
    public int score;

    public PHTask(string name, bool complete, int score)
    {  this.name = name; this.complete = complete; this.score = score; }
}

public class ScoreUIManager : MonoBehaviour
{
    [SerializeField] private Image m_clipboard;
    [SerializeField] private GameObject m_lineItem;
    [SerializeField] private TextMeshProUGUI m_totalScore;

    //Probably temporary, may change to calc scale with number of tasks later.
    [SerializeField][Range(0.0f, 5.0f)] private float m_spacing;

    /// <summary>
    /// Placeholder. Suggest Json to player prefs.
    /// </summary>
    [SerializeField] private List<PHTask> m_tasks = new();
    [SerializeField] private List<GameObject> m_LIList = new();

    [SerializeField][Range(0.0f, 2.0f)] private float m_secondsWait = 1.0f;
    private bool m_skip = false;

    // Start is called before the first frame update
    private void OnEnable()
    {
        MenuInputManager.Inputs.OnAltSelect += Inputs_OnAlt_Select;

        m_tasks = PlaceholderTaskFill();

        StartCoroutine(LineItemSetActive());
    }

    private void OnDisable()
    {
        MenuInputManager.Inputs.OnAltSelect -= Inputs_OnAlt_Select;
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

    private IEnumerator LineItemSetActive()
    {
        Transform clipTransform = m_clipboard.transform;
        RectTransform LITransform;

        TextMeshProUGUI name = null;
        TextMeshProUGUI score = null;
        GameObject strikeOut = null;

        int totalScore = 0;

        for (var i = 0; i < m_tasks.Count; i++)
        {
            m_LIList.Add(Instantiate(m_lineItem, clipTransform));
            m_LIList[i].SetActive(false);
            LITransform = m_LIList[i].GetComponent<RectTransform>();
            LITransform.anchoredPosition = new Vector2(0, LITransform.rect.height / m_spacing * -i);

            name = m_LIList[i].transform.Find("TaskName").GetComponent<TextMeshProUGUI>();
            strikeOut = m_LIList[i].transform.Find("StrikeOut").gameObject;

            name.text = m_tasks[i].name;

            m_LIList[i].SetActive(true);
            strikeOut.SetActive(false);
        }

        for (var i = 0; i < m_LIList.Count; i++)
        {
            for (float timer = m_secondsWait; timer >= 0f; timer -= Time.deltaTime)
            {
                if (m_skip)
                {
                    break;
                }
                yield return null;
            }

            //Put the score in the score.
            if (m_tasks[i].complete)
            {
                score = m_LIList[i].transform.Find("TaskScore").GetComponent<TextMeshProUGUI>();
                score.text = $"${m_tasks[i].score.ToString()}";

                strikeOut = m_LIList[i].transform.Find("StrikeOut").gameObject;
                strikeOut.SetActive(true);

                //Add to score total
                totalScore += m_tasks[i].score;
            }
        }

        m_totalScore.text = $"${totalScore.ToString()}";
    }

    private void Inputs_OnAlt_Select()
    {
        m_skip = true;
    }

    private void OnLevelExit()
    {
        Debug.Log("LEVEL EXIT");
    }    
}
