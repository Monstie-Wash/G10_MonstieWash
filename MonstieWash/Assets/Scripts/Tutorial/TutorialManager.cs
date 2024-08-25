using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    private int m_tutorialStep = 0;       // Index for current tutorial prompt in the List
    private bool m_completed = false;     // Event flag for the current tutorial prompt
    private enum CompletionEvent { ChangeScene, EraseStarted, OnMove, SwitchTool, UnstickItem };    // Enumerated list for designers of events to listen for
    private enum CompletionType { Instant, Count, Time };
    // Enumerated list of ways the prompt can be completed:                                      
        // Instant - completes instantly once the event is received         (value = delay after event is received)
        // Count - completes after a number of times the event is received  (value = number of times)
        // Time - completes after receiving the event for a period of time  (value = overall time performed)
    private float m_trackedValue = 0;     // depends on value of CompleteType

    [Serializable] 
    private struct TutorialPrompt
    {
        [Tooltip("The tutorial prompt")] public GameObject prompt;                                  // The prompt to be shown
        [Tooltip("The event to listen for")] public CompletionEvent eventListen;                    
        [Tooltip("The method to detect for completion")] public CompletionType promptCompletion;
        [Tooltip("The value to pair with the completion detection method")] public float value;

        public CompletionEvent CompleteEvent { get { return eventListen; } }
        public CompletionType CompleteType { get { return promptCompletion; } }

        public void SetActive(bool value)
        {
            prompt.SetActive(value);
        }
    }

    [Tooltip("List of tutorial prompts in sequential order")][SerializeField] private List<TutorialPrompt> m_tutorialPrompts;     // List of prompts to iterate through

    private void OnEnable()
    {
        Eraser.OnErasing_Started += EraseStart;
        GameSceneManager.OnSceneChanged += SceneChange;
        InputManager.Instance.OnMove += OnMove;
        StuckItem.UnstickItem += UnstickItem;
        ToolSwitcher.SwitchTool += SwitchTool;
    }

    private void OnDisable()
    {
        Eraser.OnErasing_Started -= EraseStart;
        GameSceneManager.OnSceneChanged -= SceneChange;
        InputManager.Instance.OnMove -= OnMove;
        StuckItem.UnstickItem -= UnstickItem;
        ToolSwitcher.SwitchTool -= SwitchTool;
    }

    private void Start()
    {
        // Activate the first prompt
        m_tutorialPrompts[m_tutorialStep].prompt.SetActive(true);
        // Begin
        StartCoroutine(RunTutorial());
    }

    /// <summary>
    /// Runs the tutorial.
    /// </summary>
    private IEnumerator RunTutorial()
    {
        foreach (var prompt in m_tutorialPrompts)
        {
            yield return new WaitUntil(() => m_completed);
            m_completed = false;
            m_tutorialPrompts[m_tutorialStep].prompt.SetActive(false);
            if (m_tutorialStep <  m_tutorialPrompts.Count - 1)
            {
                m_tutorialStep++;
                m_tutorialPrompts[m_tutorialStep].prompt.SetActive(true);
            }
        }
    }

    #region Completion Type Checks
    /// <summary>
    /// Waits value amount of time, then sets tutorial task as complete.
    /// </summary>
    /// <param name="value">The time (in seconds) to wait before completing.</param>
    private IEnumerator InstantCompletion(float value)
    {
        if (m_trackedValue != -1f)  // Don't allow multiple instant completions to run simultaneously
        {
            m_trackedValue = -1f;
            yield return new WaitForSeconds(value);
            m_trackedValue = 0;
            m_completed = true;
        }
    }

    /// <summary>
    /// Iterates task success count, then completes if success count exceeds given count.
    /// </summary>
    /// <param name="targetValue">The number of successes required.</param>
    private void CountCompletion(float targetValue)
    {
        m_trackedValue++;
        if (m_trackedValue >= targetValue)
        {
            m_trackedValue = 0;
            m_completed = true;
        }
    }

    /// <summary>
    /// Updates time spent on the task, then completes if spent time exceeds given time.
    /// </summary>
    /// <param name="targetValue">The time required to complete the task.</param>
    private void TimeCompletion(float targetValue)
    {
        m_trackedValue += 0.002f;  // Approximate - can replace with how long the call actually takes
        if (m_trackedValue >= targetValue)
        {
            m_trackedValue = 0;
            m_completed = true;
        }
    }
    #endregion

    #region Event Listeners
    private void OnMove(Vector2 movement)
    {
        var currentPrompt = m_tutorialPrompts[m_tutorialStep];
        if (currentPrompt.CompleteEvent == CompletionEvent.OnMove)
        {
            RunCompletionTests(currentPrompt);
        }
    }

    private void EraseStart(bool value)
    {
        var currentPrompt = m_tutorialPrompts[m_tutorialStep];
        if (currentPrompt.CompleteEvent == CompletionEvent.EraseStarted)
        {
            RunCompletionTests(currentPrompt);
        }
    }    

    private void SceneChange()
    {
        var currentPrompt = m_tutorialPrompts[m_tutorialStep];
        if (currentPrompt.CompleteEvent == CompletionEvent.ChangeScene)
        {
            RunCompletionTests(currentPrompt);
        }
    }

    private void SwitchTool()
    {
        var currentPrompt = m_tutorialPrompts[m_tutorialStep];
        if (currentPrompt.CompleteEvent == CompletionEvent.SwitchTool)
        {
            RunCompletionTests(currentPrompt);
        }
    }

    private void UnstickItem()
    {
        var currentPrompt = m_tutorialPrompts[m_tutorialStep];
        if (currentPrompt.CompleteEvent == CompletionEvent.UnstickItem)
        {
            RunCompletionTests(currentPrompt);
        }
    }
    #endregion

    /// <summary>
    /// Determines which type of completion check to run based on enumerated value.
    /// </summary>
    /// <param name="prompt">The prompt to evaluate.</param>
    private void RunCompletionTests(TutorialPrompt prompt)
    {
        switch (prompt.CompleteType)
        {
            case CompletionType.Instant:
                StartCoroutine(InstantCompletion(prompt.value));
                break;
            case CompletionType.Count:
                CountCompletion(prompt.value);
                break;
            case CompletionType.Time:
                TimeCompletion(prompt.value);
                break;
            default:
                throw new Exception("Invalid completion type for TutorialManager's current prompt.");
        }
    }
}
