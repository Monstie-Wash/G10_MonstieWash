using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteMask))]
public class MaskUpdater : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;
    private SpriteMask m_mask;
    private Sprite m_currentSprite;

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_mask = GetComponent<SpriteMask>();
        m_currentSprite = m_spriteRenderer.sprite;
        m_mask.sprite = m_currentSprite;
    }

    private void LateUpdate()
    {
        if (m_currentSprite.GetHashCode() != m_spriteRenderer.sprite.GetHashCode())
        {
            m_currentSprite = m_spriteRenderer.sprite;
            m_mask.sprite = m_currentSprite;
        }
    }
}
