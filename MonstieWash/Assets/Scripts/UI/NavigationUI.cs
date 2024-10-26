using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generates nav UI. Finds scene camera, sets it to Canvas render camera.
/// </summary>
public class NavigationUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonTile;
    [SerializeField] private GameObject navPanel;
    [SerializeField] private GameObject panelEnd;

	[SerializeField] private float tilePadding = 1.4f; //Seems like a nice number. Means that the UI doesn't overlap with the bag on Mimic.

    private List<GameScene> m_gameScenes;
    private List<MonsterController> m_monsterControllers = new();
    private Button m_currentlyActiveButton;

    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        Camera camera = FindFirstObjectByType<Camera>();

        canvas.worldCamera = camera;

        m_gameScenes = GameSceneManager.Instance.CurrentLevelScenes;

        InstantiateUI();

        GameSceneManager.Instance.OnMonsterScenesLoaded += OnMonsterScenesLoaded;
    }

    private void OnMonsterScenesLoaded()
    {
        GameSceneManager.Instance.OnMonsterScenesLoaded -= OnMonsterScenesLoaded;

        var controllers = FindObjectsByType<MonsterController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var controller in controllers)
        {
            m_monsterControllers.Add(controller);
            controller.OnAttackBegin += MonsterController_OnAttackStart;
            controller.OnAttackEnd += MonsterController_OnAttackEnd;
        }
    }

    private void InstantiateUI ()
    {
        //Resize Nav Panel to fit all tiles.
        var npSize = navPanel.GetComponent<RectTransform>();
        npSize.sizeDelta = new Vector2(tilePadding, m_gameScenes.Count * tilePadding);

        //Spawns a nav tile per scene. Pulls the image from the sciptable object. Probably a much nicer implementation than pulling direct from resources. Who would even do that? Certainly not me. Terrible idea.
        foreach (var scene in m_gameScenes)
        {
            buttonTile.GetComponent<Image>().sprite = scene.SceneThumb;
            buttonTile.GetComponent<TestTravObj>().TargetScene = scene;
            var buttonObj = Instantiate(buttonTile, navPanel.transform);
            buttonObj.GetComponent<TestTravObj>().OnStateChanged += OnButtonStateChanged;
        }

        var peSize = panelEnd.GetComponent<RectTransform>();

        peSize.anchoredPosition = new Vector2(0, npSize.rect.height / 2.0f);
        Instantiate(panelEnd, gameObject.transform);

		peSize.anchoredPosition = new Vector2(0, -npSize.rect.height / 2.0f);
		Instantiate(panelEnd, gameObject.transform);
	}

    private void OnButtonStateChanged(Button button, bool enabled)
    {
        if (enabled) m_currentlyActiveButton = button;
    }

    private void MonsterController_OnAttackStart()
    {
        m_currentlyActiveButton.interactable = false;
    }

    private void MonsterController_OnAttackEnd()
    {
        m_currentlyActiveButton.interactable = true;
    }
}
