using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tool", menuName = "ScriptableObjects/Tool")]
public class Tool : ScriptableObject
{
    public Sprite mask;
    [Range(1, 100)] public float strength = 100;
    [HideInInspector] public byte[] maskPixels;

    public void Initialize()
    {
        maskPixels = new byte[mask.texture.width * mask.texture.height];
        Color[] maskColors = mask.texture.GetPixels();
        for (int i = 0; i < maskColors.Length; i++)
        {
            maskPixels[i] = (byte)Mathf.FloorToInt(255f * maskColors[i].a);
        }
    }
}
