using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class DirtCombiner : MonoBehaviour
{
    [SerializeField] GameObject dirtPrefab;
    [SerializeField] float scorePerDirt = 5f;

    private Vector3 combinedDirtPos;

    private void Start()
    {
        var erasables = GameObject.FindGameObjectsWithTag("Erasable");
        var taskContainer = GameObject.FindGameObjectWithTag("TaskContainer");
        var tex = GetCameraTexture();
        var combinedDirt = Instantiate(dirtPrefab, combinedDirtPos, Quaternion.identity, taskContainer.transform);
        var taskData = combinedDirt.GetComponent<TaskData>();
        taskData.Score = erasables.Length * scorePerDirt;
        var spriteRenderer = combinedDirt.GetComponent<SpriteRenderer>();
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
            if (erasableBounds.max.y > topLeft.y) topLeft.y = erasableBounds.max.y;
            if (erasableBounds.max.x > botRight.x) botRight.x = erasableBounds.max.x;
            if (erasableBounds.min.y < botRight.y) botRight.y = erasableBounds.min.y;
        }

        var width = botRight.x - topLeft.x;
        var height = topLeft.y - botRight.y;
        var center = new Vector2(topLeft.x + (width / 2f), botRight.y + (height / 2f));
        var textureWidth = Mathf.CeilToInt(width * 100);
        var textureHeight = Mathf.CeilToInt(height * 100);

        combinedDirtPos = center;

        RenderTexture rt = RenderTexture.GetTemporary(textureWidth, textureHeight);
        Texture2D screenshot = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);

        Camera tempCamera = new GameObject().AddComponent<Camera>();

        int erasableLayerMask = 1 << LayerMask.NameToLayer("Erasable");
        tempCamera.cullingMask = erasableLayerMask;
        tempCamera.targetTexture = rt;
        tempCamera.transform.position = new Vector3(center.x, center.y, -10f);
        tempCamera.orthographic = true;
        tempCamera.orthographicSize = height / 2f;
        tempCamera.backgroundColor = Color.clear;
        tempCamera.clearFlags = CameraClearFlags.SolidColor;

        tempCamera.Render();

        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        screenshot.Apply();

        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        Destroy(tempCamera.gameObject);

        return screenshot;
    }
}
