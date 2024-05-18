using UnityEngine;
using UnityEngine.UI;

public class TraversalMenu : MonoBehaviour
{
    [SerializeField] private GameScene targetScene;

    private MenuLoader m_saveObj;
    private string m_targetScene;
    private Button m_button;

    private void Awake()
    {
        if (targetScene == null) Debug.LogError($"Target scene not assigned for {name}!");
        else m_targetScene = targetScene.SceneName;

        m_saveObj = FindFirstObjectByType<MenuLoader>();
        m_button = GetComponent<Button>();

        m_button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        m_saveObj.LoadMonsterScene(m_targetScene);
    }
}