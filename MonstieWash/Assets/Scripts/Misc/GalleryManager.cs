using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GalleryManager : ImageLoader
{
    [SerializeField] private Bounds bounds;
    [SerializeField] private Sprite polaroidBorder;

    private GameObject m_currentPolaroid;

    public Bounds Bounds { get { return bounds; } }
    public Transform CurrentPolaroid { get { return m_currentPolaroid.transform; } }

    private struct PolaroidTransform
    {
        public float posX;
        public float posY;
        public float rot;

        public PolaroidTransform(float posX, float posY, float rot)
        {
            this.posX = posX;
            this.posY = posY;
            this.rot = rot;
        }
    }

    private void Awake()
    {
        var playerHand = FindFirstObjectByType<PlayerHand>().gameObject;
        playerHand.AddComponent<GalleryInteraction>();

        var savePath = Path.Combine(Application.persistentDataPath, saveLocation);
        var fileCount = Directory.GetFiles(savePath).Length;

        PolaroidTransform[] polaroidTransforms = { }; // Only used when there is more than one polaroid saved
        if (fileCount > 1) polaroidTransforms = ReadPolaroidTransforms();

        // Oldest polaroids to newest polaroids
        for (int i = 0; i < fileCount; i++)
        {
            // Get polaroid sprite
            var spriteTex = LoadTexture($"{savePath}/Polaroid_{i}.Png");
            var sizePercentage = spriteTex.width / 1920f;
            var newSprite = Sprite.Create(spriteTex, new Rect(0, 0, spriteTex.width, spriteTex.height), new Vector2(0.5f, 0.5f), sizePercentage * 750f, 0, SpriteMeshType.Tight);

            // Create polaroid gameobject
            var polaroid = new GameObject($"Polaroid {i}");
            var sr = polaroid.AddComponent<SpriteRenderer>();
            sr.sprite = newSprite;
            sr.sortingLayerName = "Tools";
            sr.sortingOrder = i * 2; // Order from oldest to newest

            // Create polaroid border
            var border = new GameObject($"Polaroid {i} Border");
            border.transform.parent = polaroid.transform;
            var psr = border.AddComponent<SpriteRenderer>();
            psr.sprite = polaroidBorder;
            psr.sortingLayerName = "Tools";
            psr.sortingOrder = i * 2 + 1;

            if (i == fileCount - 1) // Last file is the one just captured
            {
                polaroid.transform.position = playerHand.transform.position; 
                polaroid.transform.parent = playerHand.transform;
                m_currentPolaroid = polaroid;
            }
            else
            {
                var polaroidTransform = polaroidTransforms[i];
                polaroid.transform.position = new Vector3(polaroidTransform.posX, polaroidTransform.posY, 0);
                polaroid.transform.rotation = Quaternion.Euler(0, 0, polaroidTransform.rot);
                polaroid.transform.localScale = Vector3.one * Mathf.Lerp(0.75f, 1f, i / (fileCount - 1f));
                polaroid.transform.parent = transform;
            }
            
        }
    }

    /// <summary>
    /// Reads positional and rotational data from PolaroidPositions.txt.
    /// </summary>
    /// <returns>Array of polaroid positions ordered from oldest to newest.</returns>
    private PolaroidTransform[] ReadPolaroidTransforms()
    {
        List<PolaroidTransform> polaroidTransforms = new();
        var saveLocation = Path.Combine(Application.persistentDataPath, "PolaroidPositions.txt");

        if (File.Exists(saveLocation))
        {
            using (var inputFile = new StreamReader(saveLocation))
            {
                while (!inputFile.EndOfStream)
                {
                    var line = inputFile.ReadLine();
                    var values = line.Split(',');
                    var posX = float.Parse(values[0]);
                    var posY = float.Parse(values[1]);
                    var rot = float.Parse(values[2]);

                    var polaroidTransform = new PolaroidTransform(posX, posY, rot);
                    polaroidTransforms.Add(polaroidTransform);
                }
            }
        }

        return polaroidTransforms.ToArray();
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
