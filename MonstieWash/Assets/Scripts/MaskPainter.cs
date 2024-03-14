using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MaskPainter : MonoBehaviour
{
    private List<GameObject> erasables = new List<GameObject>();
    private Dictionary<GameObject, Sprite> erasablesMasks = new Dictionary<GameObject, Sprite>();
    private Dictionary<Sprite, byte[]> maskPixels = new Dictionary<Sprite, byte[]>();

    private Sprite brush;
    byte[] brushPixels = new byte[32 * 32];

    private void Awake()
    {
        brush = GetComponent<SpriteRenderer>().sprite;

        Color[] brushTexture = brush.texture.GetPixels();
        for (int i = 0;  i < brushTexture.Length; i++)
        {
            if (brushTexture[i].a > 0f) brushPixels[i] = 255;
        }

        GameObject[] tempErasables =  GameObject.FindGameObjectsWithTag("Erasable");
        erasables.AddRange(tempErasables);

        foreach (GameObject erasable in erasables) 
        {
            Sprite sprite = erasable.GetComponent<SpriteMask>().sprite;
            erasablesMasks.Add(erasable, sprite);
            maskPixels.Add(sprite, new byte[sprite.texture.width * sprite.texture.height]);

            ResetSpriteMask(sprite.texture);
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject erasable in erasables)
        {
            ResetSpriteMask(erasablesMasks[erasable].texture);
        }
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0)) return;
        
        foreach (GameObject erasable in erasables)
        {
            Vector3 erasablePos = Camera.main.WorldToScreenPoint(erasable.transform.position);
            Vector2Int erasableCentrePixel = new Vector2Int(Mathf.FloorToInt(erasablePos.x), Mathf.FloorToInt(erasablePos.y));
            Vector2Int mousePos = new Vector2Int(Mathf.FloorToInt(Input.mousePosition.x), Mathf.FloorToInt(Input.mousePosition.y));
            Vector2Int mouseDistFromErasableCentre = mousePos - erasableCentrePixel;

            Texture2D mask = erasablesMasks[erasable].texture;
            Vector2Int halfBrushSize = new Vector2Int(brush.texture.width / 2, brush.texture.height / 2);
            Vector2Int halfErasableSize = new Vector2Int(mask.width / 2, mask.height / 2);

            //Stop if brush is not on mask
            if (Math.Abs(mouseDistFromErasableCentre.x) - halfBrushSize.x >= halfErasableSize.x || Math.Abs(mouseDistFromErasableCentre.y) - halfBrushSize.y >= halfErasableSize.y) continue;

            byte[] pixels = maskPixels[erasablesMasks[erasable]];

            for (int i = 0; i < brushPixels.Length; i++)
            {
                if (brushPixels[i] != 0)
                {
                    Vector2Int currentPixel = GetPixelCoordinates(i);

                    //Get the position of this pixel on the mask
                    Vector2Int distFromBrushCentre = new Vector2Int(currentPixel.x - halfBrushSize.x, currentPixel.y - halfBrushSize.y);
                    Vector2Int pixelPosFromCentre = mouseDistFromErasableCentre + distFromBrushCentre;

                    //Stop if the pixel is out of bounds
                    if (Math.Abs(pixelPosFromCentre.x) >= halfErasableSize.x || Math.Abs(pixelPosFromCentre.y) >= halfErasableSize.y) continue;

                    int pixel = (halfErasableSize.y + pixelPosFromCentre.y) * mask.width + halfErasableSize.x + pixelPosFromCentre.x;

                    pixels[pixel] = 255;
                }
            }

            mask.SetPixelData(pixels, 0);
            mask.Apply(false);
        }
    }

    private Vector2Int GetPixelCoordinates(int pixel)
    {
        int x = pixel % brush.texture.width;
        int y = Mathf.FloorToInt(pixel / brush.texture.width);

        return new Vector2Int(x, y);
    }

    private void ResetSpriteMask(Texture2D mask)
    {
        mask.SetPixelData(new byte[mask.width * mask.height], 0);
        mask.Apply(false);
    }
}
