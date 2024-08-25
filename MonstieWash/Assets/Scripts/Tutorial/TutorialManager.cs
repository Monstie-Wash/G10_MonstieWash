using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{

    private float waitTime = 3f;

    private int tutorialStep = 0;
    private bool completed = false;
    private enum CompletionEvent { Erase, Navigate, OnMove, PickBone, SwitchTool };
    private enum CompletionType { Instant, Count, Time };

    [Serializable] 
    private struct TutorialPrompt
    {
        [Tooltip("The tutorial prompt")] public GameObject prompt;
        [Tooltip("The event to listen for")] public CompletionEvent eventListen;
        [Tooltip("The method to detect for completion")] public CompletionType promptCompletion;

        public CompletionEvent CompleteEvent { get { return eventListen; } }
        public CompletionType CompleteType { get { return promptCompletion; } }
        public void SetActive(bool value)
        {
            prompt.SetActive(value);
        }
    }

    [Tooltip("List of tutorial prompts in sequential order")][SerializeField] private List<TutorialPrompt> tutorialPrompts;

    private void OnEnable()
    {
        InputManager.Instance.OnMove += OnMove;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnMove -= OnMove;
    }

    private void Start()
    {
        tutorialPrompts[tutorialStep].prompt.SetActive(true);
        Debug.Log($"{tutorialPrompts[tutorialStep]} enabled!");
    }

    // Update is called once per frame
    void Update()
    {
        waitTime -= Time.deltaTime;
        if (waitTime < 0 )
        {
            tutorialStep++;
            tutorialPrompts[tutorialStep - 1].prompt.SetActive(false);
            Debug.Log($"{tutorialPrompts[tutorialStep-1]} disabled!");
            tutorialPrompts[tutorialStep].SetActive(true);
            Debug.Log($"{tutorialPrompts[tutorialStep]} enabled!");
            waitTime = 3f;
        }
    }

    // Wait for an event
    private IEnumerator WaitForEvent(UnityEvent waitfor) {
        var trigger = false;
        Action action = () => trigger = true;
        waitfor.AddListener(action.Invoke);
        yield return new WaitUntil(()=>trigger);
        waitfor.RemoveListener(action.Invoke);
    }

    // Move Event received (from InputManager)
    private void OnMove(Vector2 movement)
    {
        var currentPrompt = tutorialPrompts[tutorialStep];
        if (currentPrompt.CompleteEvent == CompletionEvent.OnMove)
        {
            Debug.Log("TutorialManager saw you move");
        }
    }



}
