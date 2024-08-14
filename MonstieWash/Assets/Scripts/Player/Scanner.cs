using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [SerializeField] private AnimationCurve greenFadeCurve;
    [SerializeField] private float greenFadeTime = 1f;

    private TaskTracker m_taskTracker;
    private bool m_scannerReady = true;

    private void Awake()
    {
        m_taskTracker = FindFirstObjectByType<TaskTracker>();
    }

    private void OnEnable()
    {
        InputManager.Instance.OnScan += Instance_OnScan;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnScan -= Instance_OnScan;
    }

    private async void Instance_OnScan()
    {
        if (!m_scannerReady) return;
        m_scannerReady = false;

        List<SpriteRenderer> spriteRenderers = new();
        foreach (var task in m_taskTracker.TaskData)
        {
            if (task.gameObject.activeInHierarchy && !task.Complete) spriteRenderers.Add(task.GetComponentInChildren<SpriteRenderer>());
        }

        Task[] fadeTasks = new Task[spriteRenderers.Count];
        for (int i = 0; i < fadeTasks.Length; i++)
        {
            fadeTasks[i] = FadeColour(spriteRenderers[i]);
        }

        await Task.WhenAll(fadeTasks);
        m_scannerReady = true;
    }

    private async Task FadeColour(SpriteRenderer sr)
    {
        var t = greenFadeTime;
        var currentColor = sr.color;

        while (t > 0f)
        {
            currentColor.a = greenFadeCurve.Evaluate(t);

            sr.color = currentColor;

            t -= Time.deltaTime;

            await Awaitable.NextFrameAsync();
        }
    }
}
