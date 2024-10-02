using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class GalleryManager : ImageLoader
{
    [SerializeField] private Bounds bounds;

    private void Start()
    {
        var fileCount = Directory.GetFiles(Application.persistentDataPath + saveLocation).Length;

        for (int i = 0; i < fileCount; i++)
        {
            var spriteTex = LoadTexture($"{Application.persistentDataPath}{saveLocation}/Polaroid_{i}.Png");
            var newSprite = Sprite.Create(spriteTex, new Rect(0, 0, spriteTex.width, spriteTex.height), new Vector2(0.5f, 0.5f), 500f, 0, SpriteMeshType.Tight);
            var polaroid = new GameObject($"Polaroid {i}");
            var sr = polaroid.AddComponent<SpriteRenderer>();
            sr.sprite = newSprite;
            sr.sortingLayerName = "Tools";
            sr.sortingOrder = i;

            if (i == fileCount - 1)
            {
                polaroid.transform.parent = FindFirstObjectByType<PlayerHand>().transform;
            }
            else
            {
                polaroid.transform.position = new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x), UnityEngine.Random.Range(bounds.min.y, bounds.max.y), 0);
                polaroid.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-30f, 30f));
                polaroid.transform.localScale = Vector3.one * Mathf.Lerp(0.5f, 1f, i / (fileCount - 1f));
                polaroid.transform.parent = transform;
            }
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        var topLeft = new Vector3(bounds.min.x, bounds.max.y, 0);
        var topRight = new Vector3(bounds.max.x, bounds.max.y, 0);
        var botLeft = new Vector3(bounds.min.x, bounds.min.y, 0);
        var botRight = new Vector3(bounds.max.x, bounds.min.y, 0);
        Debug.DrawLine(topLeft, topRight, Color.green);
        Debug.DrawLine(topRight, botRight, Color.green);
        Debug.DrawLine(botRight, botLeft, Color.green);
        Debug.DrawLine(botLeft, topLeft, Color.green);
    }
}
