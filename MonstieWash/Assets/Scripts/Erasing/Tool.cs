using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Tool", menuName = "ScriptableObjects/Tool")]
public class Tool : ScriptableObject
{
    public string toolName = "";
    public Sprite mask;
    [Range(1, 10)] public int size = 1;
    [SerializeField][Range(1f, 100f)] private float inputStrength = 100f;
    [SerializeField] private List<ErasableLayerer.ErasableLayer> erasableLayers = new();

    public byte[] MaskPixels { get; private set; }
    public float InputStrength { get { return inputStrength; } set { inputStrength = Mathf.Clamp(value, 1f, 100f); } }
    public float Strength { get; private set; }
    public List<ErasableLayerer.ErasableLayer> ErasableLayers { get { return erasableLayers; } }

    /// <summary>
    /// Initalize the tool, setting up all required values and variables. (Used in place of an Awake)
    /// </summary>
    public void Initialize()
    {
        //The input strength is what the user will adjust to control the strength of the tool.
        //The input strength is scaled exponentially to provide a smoother strength gradient and improved user experience.
        var exponentBase = 1.0472f; //This is a value that comes from the 100th root of 100. This is to ensure 100% input strength matches 100% output strength.
        Strength = Mathf.Clamp(Mathf.Pow(exponentBase, inputStrength), 1f, 100f);

        MaskPixels = new byte[mask.texture.width * mask.texture.height];
        Color[] maskColors = mask.texture.GetPixels();

        for (int i = 0; i < maskColors.Length; i++)
        {
            MaskPixels[i] = (byte)Mathf.FloorToInt(255f * maskColors[i].a);
        }
    }

    public void SetValues(Tool otherTool)
    {
        toolName = otherTool.toolName;
        mask = otherTool.mask;
        size = otherTool.size;
        inputStrength = otherTool.inputStrength;
    }

    private void OnValidate()
    {
        List<ErasableLayerer.ErasableLayer> duplicateLayers = new();

        foreach (var layer in erasableLayers)
        {
            var layerCount = 0;

            foreach (var otherLayer in erasableLayers)
            {
                if (otherLayer == layer) layerCount++;
            }

            if (layerCount > 1 && !duplicateLayers.Contains(layer))
            {
                Debug.LogWarning($"There are duplicate values of {layer} in the ErasableLayers list on tool: {toolName}");
                duplicateLayers.Add(layer);
            }
        }
    }
}
