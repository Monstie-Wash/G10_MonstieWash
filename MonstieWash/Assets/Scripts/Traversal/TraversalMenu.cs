using UnityEngine;

public class TraversalMenu : MonoBehaviour
{
    [SerializeField] private GameScene targetScene;
    [SerializeField] private Color highlightColour;

    private MenuLoader m_saveObj;
    private string m_targetScene;
    private SpriteRenderer m_sprite;

    private void Awake()
    {
        if (targetScene == null) Debug.LogError($"Target scene not assigned for {name}!");
        else m_targetScene = targetScene.SceneName;

        m_saveObj = FindFirstObjectByType<MenuLoader>();
        m_sprite = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        m_saveObj.LoadMonsterScene(m_targetScene);
    }

    private void OnMouseOver()
    {
        m_sprite.color = highlightColour;
    }

    private void OnMouseExit()
    {
        m_sprite.color = Color.white;
    }
}