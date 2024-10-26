using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DirtCombiner : MonoBehaviour
{
    [SerializeField] private LayerMask erasableLayer;
    [SerializeField] private Transform Container;
    [SerializeField] private GameObject erasablePrefab;
    [SerializeField] private float scorePerErasable = 5f;
    [SerializeField] private List<GameObject> erasables = new();

    private Vector3 m_combinedDirtPos;
    private const float m_pixelsPerUnit = 108f;
    private GameObject combinedDirt;

    public async Task Combine()
    {
        foreach (var erasable in erasables) erasable.GetComponent<SpriteRenderer>().enabled = true;

        CreateCombinedDirt();
        await CleanupCombinedDirt();

        combinedDirt.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void CreateCombinedDirt()
    {
        var tex = GetCameraTexture();
        combinedDirt = Instantiate(erasablePrefab, m_combinedDirtPos, Quaternion.identity, Container);
        combinedDirt.name = "Combined Erasable";
        var taskData = combinedDirt.GetComponent<TaskData>();
        taskData.Score = erasables.Count * scorePerErasable;
        taskData.Threshold = Mathf.Lerp(99f, 99.999f, erasables.Count / 25f);
        var spriteRenderer = combinedDirt.GetComponent<SpriteRenderer>();
        var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, m_pixelsPerUnit);
        sprite.name = "Combined Sprite Duplicate";
        spriteRenderer.sprite = sprite;
    }

    private Texture2D GetCameraTexture()
    {
        var topLeft = Vector2.zero;
        var botRight = Vector2.zero;

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
        var textureWidth = Mathf.CeilToInt(width * m_pixelsPerUnit);
        var textureHeight = Mathf.CeilToInt(height * m_pixelsPerUnit);

        m_combinedDirtPos = center;

        var rt = RenderTexture.GetTemporary(textureWidth, textureHeight);
        var screenshot = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);

        var tempCamera = new GameObject().AddComponent<Camera>();
        tempCamera.cullingMask = erasableLayer;
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

    private async Task CleanupCombinedDirt()
    {
        foreach (var erasable in erasables)
        {
            Destroy(erasable);
        }
        erasables.Clear();

        await Task.Yield();
    }
}
