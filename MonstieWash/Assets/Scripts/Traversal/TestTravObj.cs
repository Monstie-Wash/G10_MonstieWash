using System.Runtime.InteropServices;
using UnityEngine;

public class TestTravObj : MonoBehaviour, INavigator
{
    [SerializeField] private GameScene targetScene;
    [SerializeField] private bool targetIsUI;
    [SerializeField] private BoxCollider2D playerHand;

    private string m_targetScene;

    public void Awake()
    {
        if (targetScene == null) Debug.LogError($"Target scene not assigned for {name}!");
        else m_targetScene = targetScene.SceneName;

        playerHand = FindFirstObjectByType<PlayerHand>().gameObject.GetComponent<BoxCollider2D>();
    }

	private void OnEnable()
	{
        InputManager.Instance.OnNavigate += OnNavigate;
	}

	private void OnDisable()
	{
		InputManager.Instance.OnNavigate -= OnNavigate;
	}

	public void OnClicked()
    {
        GameSceneManager.Instance.MoveToScene(m_targetScene, targetIsUI);
    }

    public void OnNavigate()
    {
        BoxCollider2D buttonCol = GetComponent<BoxCollider2D>();
        if (buttonCol.bounds.Intersects(playerHand.bounds))
        {
            OnClicked();
        }
    }
}