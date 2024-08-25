using System;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.U2D;

public class Eraser : MonoBehaviour
{
    [SerializeField] private Tool tool;
    [SerializeField] private Transform drawPosTransform;

    private List<Erasable> m_erasables = new();
    private PlayerHand m_playerHand;
    private TaskTracker m_taskTracker;

    private float m_distFromCentre;   // Distance of the Eraser from the centre of the screen
    [Tooltip("The distance of tool sparkle trail from centre of screen")][SerializeField] private float maxSparkleDist;  // When monstie is clean, sparkle trail detects a 'bubble' rather than the monster itself

    private bool m_isErasing = false;
    private bool m_isErasingClean = false;
    public static event Action<bool> OnErasing_Started;    // True = Started erasing on a complete scene. | False = Started erasing on an incomplete scene. 
    public static event Action<bool> OnErasing_Ended;      // True = Stopped erasing on a complete scene. | False = Stopped erasing on an incomplete scene. 

    /// <summary>
    /// A struct representing any erasable object (dirt, mould etc.) to keep track of all relevant values and apply changes.
    /// </summary>
    private class Erasable
    {
        public GameObject obj { get; private set; }
        public Sprite sprite { get; private set; }
        public byte[] maskPixels;
        public TaskData erasableTask;

        /// <summary>
        /// An erasable object representation.
        /// </summary>
        /// <param name="obj">The GameObject that is an erasable.</param>
        public Erasable(GameObject obj)
        {
            this.obj = obj;
            sprite = obj.GetComponent<SpriteRenderer>().sprite;
            maskPixels = new byte[sprite.texture.width * sprite.texture.height];
            erasableTask = obj.GetComponent<TaskData>();
        }

        /// <summary>
        /// Applies the mask to the sprite.
        /// </summary>
        public void ApplyMask()
        {
            var colors = sprite.texture.GetPixels();
            var newColors = new Color[maskPixels.Length];
            var erasedCount = 0;

            for (int i = 0; i < newColors.Length; i++)
            {
                newColors[i].r = colors[i].r;
                newColors[i].g = colors[i].g;
                newColors[i].b = colors[i].b;
                newColors[i].a = Mathf.Min((255 - maskPixels[i])/255f, colors[i].a);
                if (newColors[i].a < 0.01f) erasedCount++;
            }

            sprite.texture.SetPixels(newColors, 0);
            sprite.texture.Apply(false);

			erasableTask.Progress = ((float)erasedCount / maskPixels.Length) * 100;
		}
    }

    private void Awake()
    {
        m_playerHand = FindFirstObjectByType<PlayerHand>();
        m_taskTracker = FindFirstObjectByType<TaskTracker>();
    }

    private void Start()
    {
        InitializeTool();
    }

    private void Update()
    {
        m_distFromCentre = Vector3.Distance(Vector3.zero, drawPosTransform.position);
    }

    private void OnEnable()
    {
        InputManager.Instance.OnActivate_Held += UseTool;
        InputManager.Instance.OnActivate_Held += UseToolClean;
        InputManager.Instance.OnActivate_Ended += StopUseTool;
        InputManager.Instance.OnActivate_Ended += StopUseToolClean;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnActivate_Held -= UseTool;
        InputManager.Instance.OnActivate_Held -= UseToolClean;
        InputManager.Instance.OnActivate_Ended -= StopUseTool;
        InputManager.Instance.OnActivate_Ended -= StopUseToolClean;

        StopUseTool();
    }

    /// <summary>
    /// Called every frame until the activate button is released.
    /// </summary>
    public void UseTool()
    {
        if (!m_playerHand.IsMoving)
        {
            StopUseTool();
            return;
        }

        var wasErasing = m_isErasing;
        m_isErasing = false;

        foreach (var erasable in m_erasables)
        {
            if (!erasable.obj.activeInHierarchy) continue;
            if (UpdateErasableMask(erasable)) 
            {
                erasable.ApplyMask();
                m_taskTracker.UpdateTaskTracker(erasable.erasableTask);

                m_isErasing = true;
            }
        }

        if (!wasErasing && m_isErasing) OnErasing_Started?.Invoke(false);
        if (wasErasing && !m_isErasing) OnErasing_Ended?.Invoke(false);
    }

    public void UseToolClean()
    {
        if (!m_isErasingClean && (m_distFromCentre < maxSparkleDist && m_taskTracker.IsThisSceneComplete()))
        {
            OnErasing_Started?.Invoke(true);
            m_isErasingClean = true;
        }
        if (m_isErasingClean && m_distFromCentre > maxSparkleDist)
        {
            StopUseToolClean();
        }
    }

    /// <summary>
    /// Called when the activate button is released. 
    /// </summary>
    public void StopUseTool() 
    {
        if (m_isErasing)
        {
            OnErasing_Ended?.Invoke(false);
            m_isErasing = false;
        }
    }

    public void StopUseToolClean()
    {
        if (m_isErasingClean)
        {
            OnErasing_Ended?.Invoke(true);
            m_isErasingClean = false;
        }
    }

    /// <summary>
    /// Sets up the tool and related variables ready for use.
    /// </summary>
    public void InitializeTool()
    {
        tool.Initialize();
        PopulateErasables();
    }

    /// <summary>
    /// Populates the erasables list, setting up the sprites ready for drawing.
    /// </summary>
    private void PopulateErasables()
    {
        var tempErasables = GameObject.FindGameObjectsWithTag("Erasable");

        foreach (var erasable in tempErasables)
        {
            var spriteRenderer = erasable.GetComponent<SpriteRenderer>();
            if (!spriteRenderer.sprite.name.Contains("Sprite Duplicate")) spriteRenderer.sprite = DuplicateSprite(spriteRenderer.sprite);

            var newErasable = new Erasable(erasable);
            if (!m_erasables.Contains(newErasable)) m_erasables.Add(newErasable);
        }
    }

    /// <summary>
    /// Duplicates the given sprite.
    /// </summary>
    /// <param name="sprite">The sprite to duplicate.</param>
    /// <returns>The duplicated sprite.</returns>
    private Sprite DuplicateSprite(Sprite sprite)
    {
        var textureCopy = new Texture2D(sprite.texture.width, sprite.texture.height, sprite.texture.format, false);
        textureCopy.name = $"{sprite.texture.name} Texture Duplicate";

        textureCopy.SetPixels(sprite.texture.GetPixels());
        textureCopy.Apply();

        var spriteCopy = Sprite.Create(textureCopy, sprite.rect, new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit, 0, SpriteMeshType.FullRect);
        spriteCopy.name = $"{sprite.name} Sprite Duplicate";

        return spriteCopy;
    }      

    /// <summary>
    /// Attempts to apply tool erasure.
    /// </summary>
    /// <param name="erasable">The erasable to update.</param>
    /// <returns>Whether the erasable mask was modified.</returns>
    private bool UpdateErasableMask(Erasable erasable)
    {
        //Cache reused values and setup variables
        var toolMaskTexture = tool.mask.texture;
        var toolMaskPpu = tool.mask.pixelsPerUnit;
        var toolMaskTextureSize = new Vector2Int(toolMaskTexture.width, toolMaskTexture.height);
        var halfToolMaskTextureSize = new Vector2Int(toolMaskTextureSize.x / 2, toolMaskTextureSize.y / 2);

        var erasableTexture = erasable.sprite.texture;
        var erasablePpu = erasable.sprite.pixelsPerUnit;
        var erasableTextureSize = new Vector2Int(erasableTexture.width, erasableTexture.height);
        var erasableTransform = erasable.obj.transform;

        var scalePixels = new List<int>();
        var erased = false;

        // Check if the tool is touching the erasable
        {
            var toolBoundsSize = new Vector2(toolMaskTextureSize.x / toolMaskPpu * tool.size, toolMaskTextureSize.y / toolMaskPpu * tool.size);
            var erasableBoundsSize = new Vector2(erasableTextureSize.x / erasablePpu * erasableTransform.localScale.x, erasableTextureSize.y / erasablePpu * erasableTransform.localScale.y);
            Bounds toolBounds = new Bounds(drawPosTransform.position, toolBoundsSize);
            Bounds erasableBounds = new Bounds(erasableTransform.position, erasableBoundsSize);

            // Exit early if not touching
            var closestPoint = (Vector2)toolBounds.ClosestPoint(erasableTransform.position);
            var relativeClosestPointVector = RotateVector(closestPoint - (Vector2)erasableTransform.position, -erasableTransform.rotation.eulerAngles.z);
            var outOfBoundsX = erasableBounds.extents.x - Mathf.Abs(relativeClosestPointVector.x) < 0f;
            var outOfBoundsY = erasableBounds.extents.y - Mathf.Abs(relativeClosestPointVector.y) < 0f;
            if (outOfBoundsX || outOfBoundsY) return false;
        }

        //Apply each pixel of the tool mask to the erasable texture mask
        for (var i = 0; i < tool.MaskPixels.Length; i++)
        {
            if (tool.MaskPixels[i] == 0) continue;

            // Translate the current pixel position to the erasable's local position
            var currentPixelOnBrush = GetPixelCoordinatesOnTexture(toolMaskTexture, i);
            var scaledCurrentPixelOnBrush = new Vector2((currentPixelOnBrush.x - halfToolMaskTextureSize.x) * tool.size, (currentPixelOnBrush.y - halfToolMaskTextureSize.y) * tool.size);
            var currentPixelInWorld = drawPosTransform.TransformPoint(scaledCurrentPixelOnBrush / toolMaskPpu);
            var localDrawPosFloat = ((Vector2)erasableTransform.InverseTransformPoint(currentPixelInWorld) * erasablePpu) + erasable.sprite.pivot;
            var localDrawPos = new Vector2Int(Mathf.RoundToInt(localDrawPosFloat.x), Mathf.RoundToInt(localDrawPosFloat.y));

            // Get all pixels affected due to brush size scaling
            scalePixels.Clear();
            int xOffset, yOffset;
            bool outOfBoundsX, outOfBoundsY;
            var centrePixelX = localDrawPos.x;
            var centrePixelY = localDrawPos.y * erasableTextureSize.x;
            var centrePixel = centrePixelX + centrePixelY;
            var expansionAmountX = Mathf.CeilToInt(tool.size / erasableTransform.localScale.x);
            var expansionAmountY = Mathf.CeilToInt(tool.size / erasableTransform.localScale.y);

            for (var xCount = 1; xCount <= expansionAmountX; xCount++)
            {
                for (var yCount = 1; yCount <= expansionAmountY; yCount++)
                {
                    xOffset = xCount - 1;
                    yOffset = erasableTextureSize.x * (yCount - 1);

                    //Check if the current pixel is not on the sprite
                    outOfBoundsX = centrePixelX + xOffset < 0 || centrePixelX + xOffset >= erasableTextureSize.x;
                    outOfBoundsY = centrePixelY + yOffset < 0 || localDrawPos.y + yCount - 1 >= erasableTextureSize.y;
                    if (outOfBoundsX || outOfBoundsY) continue;

                    scalePixels.Add(centrePixel + xOffset + yOffset);
                }
            }

            erased |= ApplyPixels(tool.MaskPixels[i], erasable.maskPixels, scalePixels.ToArray());
        }

        return erased;
    }

    /// <summary>
    /// Gets the x/y coordinates of the given pixel on the given texture.
    /// </summary>
    /// <param name="texture">The texture to get the pixel coordinates on.</param>
    /// <param name="pixel">The pixel to get the coordinates of.</param>
    /// <returns>The coordinates of the pixel on the texture.</returns>
    private Vector2Int GetPixelCoordinatesOnTexture(Texture2D texture, int pixel)
    {
        var x = pixel % texture.width;
        var y = Mathf.FloorToInt(pixel / texture.width);

        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Set the given erasable mask.
    /// </summary>
    /// <param name="toolMaskPixelAlpha">The alpha of the tool pixel to use.</param>
    /// <param name="erasableMaskPixels">The erasable mask.</param>
    /// <param name="drawingPixels">The array of pixels to apply.</param>
    /// <returns>Whether a change was made to the erasable mask.</returns>
    private bool ApplyPixels(byte toolMaskPixelAlpha, byte[] erasableMaskPixels, int[] drawingPixels)
    {
        var pixelStrength = toolMaskPixelAlpha * (tool.Strength / 100f);
        var erased = false;

        foreach (var pixel in drawingPixels)
        {
            var newAlpha = Mathf.Clamp(erasableMaskPixels[pixel] + (pixelStrength * 255f), 0f, 255f);
            var newValue = (byte)Mathf.FloorToInt(newAlpha);

            if (erasableMaskPixels[pixel] != newValue)
            {
                erasableMaskPixels[pixel] = newValue;
                erased = true;
            }
        }

        return erased;
    }

    /// <summary>
    /// Rotates a vector.
    /// </summary>
    /// <param name="vector">The vector to rotate.</param>
    /// <param name="angle">The angle in degrees to rotate by.</param>
    /// <returns></returns>
    private Vector2 RotateVector(Vector2 vector, float angle)
    {
        var angleRadians = angle * Mathf.Deg2Rad;
        var cos = Mathf.Cos(angleRadians);
        var sin = Mathf.Sin(angleRadians);
        var a = new Vector2(cos, sin);
        var b = new Vector2(-sin, cos);

        return vector.x * a + vector.y * b;
    }
}
