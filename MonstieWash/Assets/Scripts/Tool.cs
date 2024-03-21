using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tool", menuName = "ScriptableObjects/Tool")]
public class Tool : ScriptableObject
{
    public Sprite mask;
    [HideInInspector]
    public float strength;
    [Range(1, 10)] 
    public int size = 1;
    [HideInInspector] 
    public byte[] maskPixels;

    [SerializeField]
    [Range(1f, 100f)] 
    private float inputStrength = 100f;

    public void Initialize()
    {
        strength = Mathf.Pow(1.0472f, inputStrength);
        maskPixels = new byte[mask.texture.width * mask.texture.height];
        Color[] maskColors = mask.texture.GetPixels();
        for (int i = 0; i < maskColors.Length; i++)
        {
            maskPixels[i] = (byte)Mathf.FloorToInt(255f * maskColors[i].a);
        }
    }
}
