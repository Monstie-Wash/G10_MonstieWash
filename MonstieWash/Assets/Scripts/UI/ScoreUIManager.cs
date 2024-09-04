using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUIManager : MonoBehaviour
{
	[SerializeField] private Transform m_clipboard;
    [SerializeField] private GameObject m_lineItem;
    [SerializeField] private TextMeshProUGUI m_totalScore;
    [SerializeField] private Image m_polaroid;
    [SerializeField] private List<Sprite> m_sprites;
    
    private TaskTracker m_tracker;

	//Probably temporary, may change to calc scale with number of tasks later.
	[SerializeField][Range(0.0f, 5.0f)] private float m_spacing;

    private Dictionary<string, float> m_tasks = new();
    [SerializeField] private List<GameObject> m_LIList = new();

    [SerializeField][Range(0.0f, 2.0f)] private float m_secondsWait = 1.0f;
    private bool m_skip = false;

    private void Awake()
    {
		m_tracker = FindFirstObjectByType<TaskTracker>(FindObjectsInactive.Include);

        if (m_sprites.Count >= (int)GameSceneManager.Instance.CurrentLevel - 1)
            m_polaroid.sprite = m_sprites[((int)GameSceneManager.Instance.CurrentLevel - 1)];
        else
            Debug.LogError("Invalid level. No polaroid sprite available.");

		m_skip = false;
        LoadOverallTasks();

        StartCoroutine(LineItemSetActive());
	}

	/// <summary>
	/// Gathers the task list from the game manager.
	/// </summary>
	/// <returns>A list of TaskData objects</returns>
	public void LoadOverallTasks()
	{
		foreach (var task in m_tracker.TaskData)
		{
			if (!m_tasks.ContainsKey(task.Type.ToString()))
			{
				m_tasks.Add(task.Type.ToString(), task.Score);
			}
			else
			{
				m_tasks[task.Type.ToString()] += task.Score;
			}
		}
	}

	private IEnumerator LineItemSetActive()
    {
        RectTransform LITransform;

        TextMeshProUGUI name = null;
        TextMeshProUGUI score = null;
        GameObject strikeOut = null;

        float totalScore = 0f;

        for (var i = 0; i < m_tasks.Count; i++)
        {
            m_LIList.Add(Instantiate(m_lineItem, m_clipboard));
            m_LIList[i].SetActive(false);
            LITransform = m_LIList[i].GetComponent<RectTransform>();
            LITransform.anchoredPosition = new Vector2(0, LITransform.rect.height / m_spacing * -i);

            name = m_LIList[i].transform.Find("TaskName").GetComponent<TextMeshProUGUI>();
            strikeOut = m_LIList[i].transform.Find("StrikeOut").gameObject;

            name.text = Resources.Load<TaskDesc>(m_tasks.ElementAt(i).Key).description;

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
            score = m_LIList[i].transform.Find("TaskScore").GetComponent<TextMeshProUGUI>();
            //score.text = $"${m_tasks[i].ToString()}";

            strikeOut = m_LIList[i].transform.Find("StrikeOut").gameObject;
            strikeOut.SetActive(true);
            
            //Add to score total
            totalScore += m_tasks.ElementAt(i).Value;
        }

        //totalScore = MathF.Round(totalScore, 2);

        m_totalScore.text = $"${Mathf.Round(totalScore).ToString()}";
        FindFirstObjectByType<Inventory>().LastEarnedScore = (int)Mathf.Round(totalScore);
        
    }

	private void Inputs_OnAlt_Select()
    {
        m_skip = true;
    }
}
