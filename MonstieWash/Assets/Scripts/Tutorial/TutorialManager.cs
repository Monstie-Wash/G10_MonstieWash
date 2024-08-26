using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    [Tooltip("List of tutorial prompts in sequential order")][SerializeField] private List<TutorialPrompt> m_tutorialPrompts;     // List of prompts to iterate through

    private int m_tutorialStep = 0;       // Index for current tutorial prompt in the List
    private bool m_completed = false;     // Event flag for the current tutorial prompt
    private enum CompletionEvent { ChangeScene, EraseStarted, OnMove, SwitchTool, UnstickItem };    // Enumerated list for designers of events to listen for
    private enum CompletionType { Instant, Count, Time };
    // Enumerated list of ways the prompt can be completed:                                      
        // Instant - completes instantly once the event is received         (value = delay after event is received)
        // Count - completes after a number of times the event is received  (value = number of times)
        // Time - completes after receiving the event for a period of time  (value = overall time performed)
    private float m_trackedValue = 0;     // depends on value of CompleteType

    private List<Eraser> m_erasers = new();
    private List<StuckItem> m_stuckItems = new();
    private ToolSwitcher m_toolSwitcher;

    [Serializable] 
    private struct TutorialPrompt
    {
        [Tooltip("The tutorial prompt")][SerializeField] private GameObject prompt;                                  // The prompt to be shown
        [Tooltip("The event to listen for")][SerializeField] private CompletionEvent eventListen;                    
        [Tooltip("The method to detect for completion")][SerializeField] private CompletionType promptCompletion;
        [Tooltip("The value to pair with the completion detection method")][SerializeField] private float value;

        public GameObject Prompt { get { return prompt; } }
        public CompletionEvent CompleteEvent { get { return eventListen; } }
        public CompletionType CompleteType { get { return promptCompletion; } }
        public float Value { get { return value; } }
    }

    private void Awake()
    {
        GameSceneManager.Instance.OnMonsterScenesLoaded += OnMonsterScenesLoaded;
        m_toolSwitcher = FindFirstObjectByType<ToolSwitcher>();
    }

    private void OnEnable()
    {
        GameSceneManager.Instance.OnSceneChanged += OnSceneChanged;
        InputManager.Instance.OnMove += OnMove;
        m_toolSwitcher.OnSwitchTool += OnSwitchTool;
    }

    private void OnMonsterScenesLoaded()
    {
        GameSceneManager.Instance.OnMonsterScenesLoaded -= OnMonsterScenesLoaded;
        foreach (var eraser in FindObjectsByType<Eraser>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            m_erasers.Add(eraser);
            eraser.OnErasing_Started += EraseStart;
        }
    }

    private void OnDisable()
    {
        GameSceneManager.Instance.OnSceneChanged -= OnSceneChanged;
        InputManager.Instance.OnMove -= OnMove;
        foreach (var eraser in m_erasers) eraser.OnErasing_Started -= EraseStart;
        foreach (var stuckItem in m_stuckItems) stuckItem.OnItemUnstuck -= OnItemUnstuck;
        m_toolSwitcher.OnSwitchTool -= OnSwitchTool;
    }

    private void Start()
    {
        // Activate the first prompt
        m_tutorialPrompts[m_tutorialStep].Prompt.SetActive(true);
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
            m_tutorialPrompts[m_tutorialStep].Prompt.SetActive(false);

            if (m_tutorialStep <  m_tutorialPrompts.Count - 1)
            {
                m_tutorialStep++;
                var currentPrompt = m_tutorialPrompts[m_tutorialStep];
                currentPrompt.Prompt.SetActive(true);

                if (currentPrompt.CompleteEvent == CompletionEvent.UnstickItem) SetStuckItemTrackedValue();
            }
        }
    }

    /// <summary>
    /// Sets the tracked value to match the number of bones unstuck in the scenes.
    /// </summary>
    private void SetStuckItemTrackedValue()
    {
        m_stuckItems.Clear();
        var bonesUnstuckCount = 0; // Calculated with an integer to keep things accurate

        // Refresh the linked items
        var linkers = FindObjectsByType<ItemLinker>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var linker in linkers.Where(l => l.gameObject.activeInHierarchy)) linker.SaveItem();
        foreach (var linker in linkers.Where(l => !l.gameObject.activeInHierarchy)) linker.LoadItem();

        // Calculate how many active unstuck items there are, accommodating for linked and unlinked items
        foreach (var stuckItem in FindObjectsByType<StuckItem>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!stuckItem.Stuck)
            {
                if (stuckItem.transform.parent.GetComponent<ItemLinker>() != null) bonesUnstuckCount += 1; // Linked items are only worth 1 (since there are two of each)
                else bonesUnstuckCount += 2; // Non-linked items
            }
            else
            {
                m_stuckItems.Add(stuckItem);
                stuckItem.OnItemUnstuck += OnItemUnstuck;
            }
        }

        bonesUnstuckCount /= 2;

        m_trackedValue = bonesUnstuckCount;

        //Check for premature completion
        if (m_trackedValue >= m_tutorialPrompts[m_tutorialStep].Value)
        {
            m_trackedValue = 0f;
            m_completed = true;
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
            m_trackedValue = 0f;
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
            m_trackedValue = 0f;
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

    private void OnSceneChanged()
    {
        var currentPrompt = m_tutorialPrompts[m_tutorialStep];
        if (currentPrompt.CompleteEvent == CompletionEvent.ChangeScene)
        {
            RunCompletionTests(currentPrompt);
        }
    }

    private void OnSwitchTool()
    {
        var currentPrompt = m_tutorialPrompts[m_tutorialStep];
        if (currentPrompt.CompleteEvent == CompletionEvent.SwitchTool)
        {
            RunCompletionTests(currentPrompt);
        }
    }

    private void OnItemUnstuck()
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
                StartCoroutine(InstantCompletion(prompt.Value));
                break;
            case CompletionType.Count:
                CountCompletion(prompt.Value);
                break;
            case CompletionType.Time:
                TimeCompletion(prompt.Value);
                break;
            default:
                throw new Exception("Invalid completion type for TutorialManager's current prompt.");
        }
    }
}