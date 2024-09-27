using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Finds scene camera, adds it to canvas. For use with world space UI.
/// </summary>
public class NavigationUI : MonoBehaviour
{
    private List<GameScene> gameScenes;
    [SerializeField] private GameObject buttonTile;
    [SerializeField] private GameObject navPanel;

    [SerializeField] private float sizeBuffer = 1.2f;

    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        Camera camera = FindFirstObjectByType<Camera>();

        canvas.worldCamera = camera;

        gameScenes = GameSceneManager.Instance.GetLevelScenes();

        InstantiateUI();
    }

    private void InstantiateUI ()
    {
        //Resize Nav Panel to fit all tiles.
        RectTransform npSize = navPanel.GetComponent<RectTransform>();
        npSize.sizeDelta = new Vector2(sizeBuffer, gameScenes.Count * sizeBuffer);

        foreach (GameScene scene in gameScenes)
        {
            buttonTile.GetComponent<Image>().sprite = scene.SceneThumb;
            buttonTile.GetComponent<TestTravObj>().TargetScene = scene;
            Instantiate(buttonTile, navPanel.transform);
        }
    }
}
