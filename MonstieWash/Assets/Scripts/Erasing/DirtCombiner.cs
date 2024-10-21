using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class DirtCombiner : MonoBehaviour
{
    [SerializeField] GameObject dirtPrefab;

    private void Start()
    {
        var erasables = GameObject.FindGameObjectsWithTag("Erasable");
        var taskContainer = GameObject.FindGameObjectWithTag("TaskContainer");
        var combinedDirt = Instantiate(dirtPrefab, taskContainer.transform);
        var spriteRenderer = combinedDirt.GetComponent<SpriteRenderer>();
        var tex = GetCameraTexture();
        var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        spriteRenderer.sprite = sprite;

        foreach (var erasable in erasables)
        {
            Destroy(erasable);
        }

        var erasers = FindObjectsByType<Eraser>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var eraser in erasers)
        {
            eraser.PopulateErasables();
        }
    }

    private Texture2D GetCameraTexture()
    {
        var erasables = GameObject.FindGameObjectsWithTag("Erasable");
        Vector2 topLeft = Vector2.zero;
        Vector2 botRight = Vector2.zero;

        foreach (var erasable in erasables)
        {
            var erasableBounds = erasable.GetComponent<BoxCollider2D>().bounds;
            
            if (erasableBounds.min.x < topLeft.x) topLeft.x = erasableBounds.min.x;
            if (erasableBounds.min.y > topLeft.y) topLeft.y = erasableBounds.min.y;
            if (erasableBounds.max.x > botRight.x) botRight.x = erasableBounds.min.x;
            if (erasableBounds.max.y < botRight.y) botRight.y = erasableBounds.min.y;
        }
        var topLeftScreen = Camera.main.WorldToScreenPoint(topLeft);
        var botRightScreen = Camera.main.WorldToScreenPoint(botRight);

        Rect rect = new Rect(topLeftScreen.x, topLeftScreen.y, botRightScreen.x - topLeftScreen.x, topLeftScreen.y - botRightScreen.y);
        RenderTexture rt = RenderTexture.GetTemporary((int)rect.width, (int)rect.height);
        Texture2D screenshot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);

        Camera tempCamera = new GameObject().AddComponent<Camera>();

        int erasableLayerMask = 1 << LayerMask.NameToLayer("Erasable");
        tempCamera.cullingMask = erasableLayerMask;

        tempCamera.targetTexture = rt;
        tempCamera.transform.position -= Vector3.forward * 10f;
        tempCamera.orthographic = true;
        tempCamera.orthographicSize = Camera.main.orthographicSize;
        tempCamera.backgroundColor = Color.clear;

        tempCamera.Render();

        RenderTexture.active = rt;
        screenshot.ReadPixels(rect, 0, 0);
        screenshot.Apply();

        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        Destroy(tempCamera.gameObject);

        return screenshot;
    }
}
