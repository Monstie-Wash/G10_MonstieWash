using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generates nav UI. Finds scene camera, sets it to Canvas render camera.
/// </summary>
public class NavigationUI : MonoBehaviour
{
    private List<GameScene> gameScenes;
    [SerializeField] private GameObject buttonTile;
    [SerializeField] private GameObject navPanel;

    [SerializeField] private float tilePadding = 1.25f; //Seems like a nice number. Means that the UI doesn't overlap with the bag on Mimic.

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
        npSize.sizeDelta = new Vector2(tilePadding, gameScenes.Count * tilePadding);

        //Spawns a nav tile per scene. Pulls the image from the sciptable object. Probably a much nicer implementation than pulling direct from resources. Who would even do that? Certainly not me. Terrible idea.
        foreach (var scene in gameScenes)
        {
            buttonTile.GetComponent<Image>().sprite = scene.SceneThumb;
            buttonTile.GetComponent<TestTravObj>().TargetScene = scene;
            Instantiate(buttonTile, navPanel.transform);
        }
    }
}
