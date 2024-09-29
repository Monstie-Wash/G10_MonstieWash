using UnityEngine;

public class TraversalObject : MonoBehaviour, INavigator
{
    [SerializeField] private GameScene targetScene;
    [SerializeField] private bool targetIsUI;

    private string m_targetScene;
    private bool m_traversalEnabled = true;
    private SpriteRenderer m_spriteRenderer;

    public void Awake()
    {
        if (targetScene == null) Debug.LogError($"Target scene not assigned for {name}!");
        else m_targetScene = targetScene.SceneName;

        var monsterController = FindFirstObjectByType<MonsterController>();
        monsterController.OnAttackBegin += MonsterController_OnAttackStart;
        monsterController.OnAttackEnd += MonsterController_OnAttackEnd;

        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void MonsterController_OnAttackEnd()
    {
        m_traversalEnabled = true;
        m_spriteRenderer.color = Color.white;
    }

    private void MonsterController_OnAttackStart()
    {
        m_traversalEnabled = false;
        m_spriteRenderer.color = new Color(0.5f, 0f, 0f);
    }

    public void OnClicked()
    {
        if (m_traversalEnabled) GameSceneManager.Instance.MoveToScene(m_targetScene, targetIsUI);
    }
}