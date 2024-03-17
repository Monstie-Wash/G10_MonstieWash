using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MaskPainter : MonoBehaviour
{
    [SerializeField] private Tool brush;

    private List<Erasable> erasables = new List<Erasable>();
    private Vector2Int mouseScreenPos;
    private Vector2Int prevMouseScreenPos = Vector2Int.zero;
    private Vector2Int halfBrushSize;

    private struct Erasable
    {
        public Erasable(GameObject obj, Sprite sprite, byte[] maskPixels)
        {
            this.obj = obj;
            this.sprite = sprite;
            this.maskPixels = maskPixels;
        }

        public GameObject obj;
        public Sprite sprite;
        public byte[] maskPixels;

        public void ApplyMask()
        {
            Color[] colors = sprite.texture.GetPixels();
            Color[] newColors = new Color[maskPixels.Length];

            for (int i = 0; i < newColors.Length; i++)
            {
                newColors[i].r = colors[i].r;
                newColors[i].g = colors[i].g;
                newColors[i].b = colors[i].b;
                newColors[i].a = Mathf.Min((255 - maskPixels[i])/255f, colors[i].a);
            }
            sprite.texture.SetPixels(newColors, 0);
            sprite.texture.Apply(false);
        }
    }

    private void Awake()
    {
        brush.Initialize();
        GetComponent<SpriteRenderer>().sprite = brush.mask;
        GameObject[] tempErasables =  GameObject.FindGameObjectsWithTag("Erasable");

        foreach (GameObject erasable in tempErasables) 
        {
            SpriteRenderer spriteRenderer = erasable.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = DuplicateSprite(spriteRenderer.sprite);
            Sprite sprite = spriteRenderer.sprite;

            Erasable newErasable = new Erasable(erasable, sprite, new byte[sprite.texture.width * sprite.texture.height]);
            erasables.Add(newErasable);
        }
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0)) return;

        mouseScreenPos = new Vector2Int(Mathf.FloorToInt(Input.mousePosition.x), Mathf.FloorToInt(Input.mousePosition.y));

        //Stop if mouse hasnt moved since last frame
        if (prevMouseScreenPos == mouseScreenPos) return;
        else prevMouseScreenPos = mouseScreenPos;

        halfBrushSize = new Vector2Int(brush.mask.texture.width / 2, brush.mask.texture.height / 2);

        foreach (Erasable erasable in erasables)
        {
            if (UpdateErasableMask(erasable)) erasable.ApplyMask();
        }
    }

    private bool UpdateErasableMask(Erasable erasable)
    {
        Vector3 erasableScreenPos = Camera.main.WorldToScreenPoint(erasable.obj.transform.position);
        Vector2Int erasableCentrePixel = new Vector2Int(Mathf.FloorToInt(erasableScreenPos.x), Mathf.FloorToInt(erasableScreenPos.y));
        Vector2Int mouseDistFromErasableCentre = mouseScreenPos - erasableCentrePixel;

        Vector2Int halfErasableSize = new Vector2Int(erasable.sprite.texture.width / 2, erasable.sprite.texture.height / 2);

        //Stop if brush is not on mask
        if (Math.Abs(mouseDistFromErasableCentre.x) - halfBrushSize.x >= halfErasableSize.x || Math.Abs(mouseDistFromErasableCentre.y) - halfBrushSize.y >= halfErasableSize.y) return false;

        for (int i = 0; i < brush.maskPixels.Length; i++)
        {
            if (brush.maskPixels[i] != 0)
            {
                Vector2Int currentPixelOnBrush = GetPixelCoordinatesOnTexture(brush.mask.texture, i);

                //Get the position of this pixel on the mask
                Vector2Int distFromBrushCentre = new Vector2Int(currentPixelOnBrush.x - halfBrushSize.x, currentPixelOnBrush.y - halfBrushSize.y);
                Vector2Int pixelPosFromCentre = mouseDistFromErasableCentre + distFromBrushCentre;

                //Stop if the pixel is out of bounds
                if (Math.Abs(pixelPosFromCentre.x) >= halfErasableSize.x || Math.Abs(pixelPosFromCentre.y) >= halfErasableSize.y) continue;

                int x = halfErasableSize.x + pixelPosFromCentre.x;
                int y = (halfErasableSize.y + pixelPosFromCentre.y) * erasable.sprite.texture.width;
                int pixel = x + y;

                float newAlpha = Mathf.Clamp(erasable.maskPixels[pixel] + brush.maskPixels[i] * (brush.strength / 100), 0f, 255f);
                erasable.maskPixels[pixel] = (byte)Mathf.FloorToInt(newAlpha);
            }
        }

        return true;
    }

    private Vector2Int GetPixelCoordinatesOnTexture(Texture2D texture, int pixel)
    {
        int x = pixel % texture.width;
        int y = Mathf.FloorToInt(pixel / texture.width);

        return new Vector2Int(x, y);
    }

    private Sprite DuplicateSprite(Sprite sprite)
    {
        Texture2D textureCopy = new Texture2D(sprite.texture.width, sprite.texture.height, sprite.texture.format, false);
        textureCopy.alphaIsTransparency = true;
        textureCopy.name = $"{sprite.texture.name} Texture Duplicate";

        textureCopy.SetPixels(sprite.texture.GetPixels());
        textureCopy.Apply();

        Sprite spriteCopy = Sprite.Create(textureCopy, sprite.rect, new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit, 0, SpriteMeshType.FullRect);
        spriteCopy.name = $"{sprite.name} Sprite Duplicate";

        return spriteCopy;
    }
}
