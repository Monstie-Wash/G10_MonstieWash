using UnityEngine;

public class TraversalObject : MonoBehaviour, INavigator
{
    [SerializeField] private GameScene targetScene;
    [SerializeField] private bool targetIsUI;
    [SerializeField] private Color disabledColour = new Color(0.5f, 0f, 0f);

    private string m_targetScene;
    private bool m_traversalEnabled = true;
    private SpriteRenderer m_spriteRenderer;
    private MonsterController m_monsterController;

    public void Awake()
    {
        if (targetScene == null) Debug.LogError($"Target scene not assigned for {name}!");
        else m_targetScene = targetScene.SceneName;        

        m_monsterController = FindFirstObjectByType<MonsterController>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (m_monsterController == null) return;
        m_monsterController.OnAttackBegin += MonsterController_OnAttackStart;
        m_monsterController.OnAttackEnd += MonsterController_OnAttackEnd;
    }

    private void OnDisable()
    {
        if (m_monsterController == null) return;
        m_monsterController.OnAttackBegin -= MonsterController_OnAttackStart;
        m_monsterController.OnAttackEnd -= MonsterController_OnAttackEnd;
    }

    private void MonsterController_OnAttackEnd()
    {
        m_traversalEnabled = true;
        m_spriteRenderer.color = Color.white;
    }

    private void MonsterController_OnAttackStart()
    {
        m_traversalEnabled = false;
        m_spriteRenderer.color = disabledColour;
    }

    public void OnClicked()
    {
        if (m_traversalEnabled) GameSceneManager.Instance.MoveToScene(m_targetScene, targetIsUI);
    }
}