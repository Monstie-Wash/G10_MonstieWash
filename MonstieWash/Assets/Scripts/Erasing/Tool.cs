using UnityEngine;

[CreateAssetMenu(fileName = "Tool", menuName = "ScriptableObjects/Tool")]
public class Tool : ScriptableObject
{
    public string toolName = "";
    public Sprite mask;
    [Range(1, 10)] public int size = 1;
    [SerializeField][Range(1f, 100f)] private float inputStrength = 100f;

    public byte[] MaskPixels { get; private set; }
    public float InputStrength { get { return inputStrength; } set { inputStrength = Mathf.Clamp(value, 1f, 100f); } }
    public float Strength { get; private set; }

    public Tool(string toolName, Sprite mask, int size, float inputStrength)
    {
        this.toolName = toolName;
        this.mask = mask;
        this.size = size;
        this.inputStrength = inputStrength;

        Initialize();
    }

    public Tool(Tool otherTool)
    {
        SetValues(otherTool);

        Initialize();
    }

    /// <summary>
    /// Initalize the tool, setting up all required values and variables. (Used in place of an Awake)
    /// </summary>
    public void Initialize()
    {
        //The input strength is what the user will adjust to control the strength of the tool.
        //The input strength is scaled exponentially to provide a smoother strength gradient and improved user experience.
        var exponentBase = 1.0472f; //This is a value that comes from the 100th root of 100. This is to ensure 100% input strength matches 100% output strength.
        Strength = Mathf.Pow(exponentBase, inputStrength);

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
}
