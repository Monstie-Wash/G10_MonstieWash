using System;
using UnityEngine;
using UnityEngine.UI;

public class TestTravObj : MonoBehaviour, INavigator
{
    [SerializeField] private GameScene targetScene;
    [SerializeField] private bool targetIsUI;
    [SerializeField] private Button button;

    private string m_targetScene;

	public event Action<Button, bool> OnStateChanged;

    public GameScene TargetScene { get => targetScene; set => targetScene = value; }

    public void Awake()
    {
        if (targetScene == null) Debug.LogError($"Target scene not assigned for {name}!");
        else m_targetScene = targetScene.SceneName;
    }

    private void OnEnable()
	{
		GameSceneManager.Instance.OnSceneChanged += OnSceneChanged;
    }

	private void OnDisable()
	{
		GameSceneManager.Instance.OnSceneChanged -= OnSceneChanged;
    }

	private void OnSceneChanged()
	{
		if (GameSceneManager.Instance.CurrentScene.name == targetScene.name)
		{
			button.interactable = false;
			OnStateChanged?.Invoke(button, false);
		}
		else
		{
			button.interactable = true;
			OnStateChanged?.Invoke(button, true);
		}
	}

	public void OnClicked()
    {
		if (button.interactable)
		{
			GameSceneManager.Instance.MoveToScene(m_targetScene, targetIsUI);
		}
    }
}