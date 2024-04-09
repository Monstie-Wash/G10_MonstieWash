using UnityEngine;

[CreateAssetMenu(fileName = "Tool", menuName = "ScriptableObjects/Tool")]
public class Tool : ScriptableObject
{
    public Sprite mask;
    [Range(1, 10)] public int size = 1;
    [SerializeField][Range(1f, 100f)] private float inputStrength = 100f;

    public byte[] maskPixels { get; private set; }
    [HideInInspector]
    public float strength;

    /// <summary>
    /// Initalize the tool, setting up all required values and variables. (Used in place of an Awake)
    /// </summary>
    public void Initialize()
    {
        //The input strength is what the user will adjust to control the strength of the tool.
        //The input strength is scaled exponentially to provide a smoother strength gradient and improved user experience.
        var exponentBase = 1.0472f; //This is a value that comes from the 100th root of 100. This is to ensure 100% input strength matches 100% output strength.
        strength = Mathf.Pow(exponentBase, inputStrength);

        maskPixels = new byte[mask.texture.width * mask.texture.height];
        Color[] maskColors = mask.texture.GetPixels();

        for (int i = 0; i < maskColors.Length; i++)
        {
            maskPixels[i] = (byte)Mathf.FloorToInt(255f * maskColors[i].a);
        }
    }
}
