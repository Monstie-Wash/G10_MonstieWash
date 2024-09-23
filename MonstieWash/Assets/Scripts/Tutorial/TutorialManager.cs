using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class TutorialManager : MonoBehaviour
{
    [Tooltip("List of tutorial prompts in sequential order")][SerializeField] private List<TutorialPrompt> m_tutorialPrompts;     // List of prompts to iterate through

    private int m_tutorialStep = 0;       // Index for current tutorial prompt in the List
    private bool m_completed = false;     // Event flag for the current tutorial prompt
    private enum CompletionEvent { ChangeScene, OnMove, Scan, SwitchTool, ToggleUI, UnstickItem, UseBrush, UseSponge, UseWaterWand, UseTreat };    // Enumerated list for designers of events to listen for
    private enum CompletionType { Instant, Count, Time };
    // Enumerated list of ways the prompt can be completed:                                      
        // Instant - completes instantly once the event is received         (value = delay after event is received)
        // Count - completes after a number of times the event is received  (value = number of times)
        // Time - completes after receiving the event for a period of time  (value = overall time performed)
    private float m_trackedValue = 0;     // depends on value of CompleteType

    private List<Eraser> m_erasers = new();
    private List<StuckItem> m_stuckItems = new();
    private ToolSwitcher m_toolSwitcher;
    private TaskTracker m_taskTracker;

    // scripted events - include scripted event enumtypes to be used here (tutoriial should probably have all types)
    private ScriptedEventsManager m_scriptedEventsManager;

    [Serializable] 
    private struct TutorialPrompt
    {
        [Tooltip("The tutorial prompt")][SerializeField] private GameObject prompt;                                  // The prompt to be shown
        [Tooltip("The event to listen for")][SerializeField] private CompletionEvent eventListen;                    
        [Tooltip("The method to detect for completion")][SerializeField] private CompletionType promptCompletion;
        [Tooltip("The value to pair with the completion detection method")][SerializeField] private float value;
        [Tooltip("The scripted event to occur")][SerializeField] private ScriptedEventsManager.ScriptedEventType scriptedEvent;

        public GameObject Prompt { get { return prompt; } }
        public CompletionEvent CompleteEvent { get { return eventListen; } }
        public CompletionType CompleteType { get { return promptCompletion; } }
        public float Value { get { return value; } }
        public ScriptedEventsManager.ScriptedEventType ScriptedEventType { get { return scriptedEvent; } }
    }

    private void Awake()
    {
        GameSceneManager.Instance.OnMonsterScenesLoaded += OnMonsterScenesLoaded;
        m_toolSwitcher = FindFirstObjectByType<ToolSwitcher>();
        m_taskTracker = FindFirstObjectByType<TaskTracker>();
        m_scriptedEventsManager = FindFirstObjectByType<ScriptedEventsManager>();

        // subscribe to all Scripted Events Manager events
        m_scriptedEventsManager.Subscribe(gameObject,
            ScriptedEventsManager.ScriptedEventType.SetAngry);
    }

    private void OnEnable()
    {
        GameSceneManager.Instance.OnSceneChanged += OnSceneChanged;
        InputManager.Instance.OnMove += OnMove;
        InputManager.Instance.OnScan += OnScan;
        m_toolSwitcher.OnSwitchTool += OnSwitchTool;
        InputManager.Instance.OnToggleUI += OnToggleUI;
        Treat.UseTreat += UseTreat;
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
        InputManager.Instance.OnScan -= OnScan;
        foreach (var eraser in m_erasers) eraser.OnErasing_Started -= EraseStart;
        foreach (var stuckItem in m_stuckItems) stuckItem.OnItemUnstuck -= OnItemUnstuck;
        m_toolSwitcher.OnSwitchTool -= OnSwitchTool;
        InputManager.Instance.OnToggleUI -= OnToggleUI;
        Treat.UseTreat -= UseTreat;
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
            // run the scripted event for the new prompt if there is one
            var currentPrompt = m_tutorialPrompts[m_tutorialStep];
            if (currentPrompt.ScriptedEventType != ScriptedEventsManager.ScriptedEventType.None) m_scriptedEventsManager.RunScriptedEvent(gameObject, currentPrompt.ScriptedEventType);

            // wait for the prompt to be completed
            yield return new WaitUntil(() => m_completed);

            // reset complete flag and hide old prompt
            m_completed = false;
            m_tutorialPrompts[m_tutorialStep].Prompt.SetActive(false);

            // dont exceed bounds of prompt list
            if (m_tutorialStep <  m_tutorialPrompts.Count - 1)
            {
                // show new prompt
                m_tutorialStep++;
                currentPrompt = m_tutorialPrompts[m_tutorialStep];
                currentPrompt.Prompt.SetActive(true);
                // run safety checks
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

    /// <summary>
    /// Runs a completion check if the triggered event type matches the current prompt event listener. 
    /// </summary>
    /// <param name="eventType">The event to evaluate.</param>
    private void EventTriggered(CompletionEvent eventType)
    {
        var currentPrompt = m_tutorialPrompts[m_tutorialStep];
        if (currentPrompt.CompleteEvent == eventType)
        {
            RunCompletionTests(currentPrompt);
        }
    }

    private void OnMove(Vector2 movement)
    {
        EventTriggered(CompletionEvent.OnMove);
    }

    private void EraseStart(bool value, Tool tool)
    {
        switch (tool.TypeOfTool)
        {
            case Tool.ToolType.Brush:
                EventTriggered(CompletionEvent.UseBrush);
                break;
            case Tool.ToolType.Sponge:
                EventTriggered(CompletionEvent.UseSponge);
                break;
            case Tool.ToolType.WaterWand:
                EventTriggered(CompletionEvent.UseWaterWand);
                break;
            default:
                break;  //ToolType.None
        }

        TaskSafetyCheck(tool); // Prevents the player from being softlocked
    }

    private void OnScan()
    {
        EventTriggered(CompletionEvent.Scan);
    }

    private void OnSceneChanged()
    {
        EventTriggered(CompletionEvent.ChangeScene);
    }

    private void OnSwitchTool()
    {
        EventTriggered(CompletionEvent.SwitchTool);
    }

    private void OnItemUnstuck()
    {
        EventTriggered(CompletionEvent.UnstickItem);
    }

    private void OnToggleUI()
    {
        EventTriggered(CompletionEvent.ToggleUI);
    }

    private void UseTreat()
    {
        EventTriggered(CompletionEvent.UseTreat);
    }
    #endregion

    #region Scripted Events

    private void RunScriptedEvent(ScriptedEventsManager.ScriptedEventType type) { 
        switch (type)
        {
            case ScriptedEventsManager.ScriptedEventType.SetAngry:
                m_scriptedEventsManager.SetAngry(gameObject);
                break;
            default:
                break;
        }
    }

    #endregion

    #region Safety Checks
    /// <summary>
    /// Runs every frame and will skip a task if it is uncompletable by the player
    /// </summary>
    private void TaskSafetyCheck(Tool tool)
    {
        switch (m_tutorialPrompts[m_tutorialStep].CompleteEvent) {
            case CompletionEvent.UseBrush:
                {
                    if (!DirtRemainsForTool(tool)) m_completed = true;

                } break;
            default:
                break;
        }
    }

    /// <summary>
    /// Returns TRUE if there is an incomplete Dirt task in TaskTracker, otherwise returns FALSE.
    /// </summary>
    ///
    private bool DirtRemains()
    {
        foreach (var task in m_taskTracker.TaskData)
        {
            if ((task.TaskType == TaskType.Dirt) && !task.Complete)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns TRUE if there is an incomplete Dirt task in TaskTracker that can be erased by the given tool, otherwise returns FALSE.
    /// </summary>
    /// <param name="tool">The tool to check for remaining erasables.</param>
    private bool DirtRemainsForTool(Tool tool)
    {
        foreach (var task in m_taskTracker.TaskData)
        {
            if ((task.TaskType == TaskType.Dirt) && !task.Complete)
            {
                var dirt = task.Container.gameObject;
                var dirtLayerInToolLayer = tool.ErasableLayers.Contains(dirt.GetComponent<ErasableLayerer>().Layer);

                if (dirtLayerInToolLayer) return true;
            }
        }
        return false;
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
