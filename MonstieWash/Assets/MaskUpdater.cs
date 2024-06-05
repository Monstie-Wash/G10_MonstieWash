using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteMask))]
public class MaskUpdater : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SpriteMask mask;
    private Sprite currentSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mask = GetComponent<SpriteMask>();
        currentSprite = spriteRenderer.sprite;
        mask.sprite = currentSprite;
    }

    private void LateUpdate()
    {
        if (currentSprite.GetHashCode() != spriteRenderer.sprite.GetHashCode())
        {
            currentSprite = spriteRenderer.sprite;
            mask.sprite = currentSprite;
        }
    }
}
