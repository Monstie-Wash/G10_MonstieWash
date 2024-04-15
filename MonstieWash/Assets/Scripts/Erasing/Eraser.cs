using System;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour
{
    [SerializeField] private Tool tool;

    private List<Erasable> m_erasables = new();
    private PlayerHand m_playerHand;
    private Vector2Int m_drawPos;
    private Transform m_drawPosTransform;
    private TaskTracker m_taskTracker;
    private RoomSaver m_roomSaver;

    private bool IsErasing = false;
    public event Action OnErasing_Started;
    public event Action OnErasing_Ended;

    /// <summary>
    /// A struct representing any erasable object (dirt, mould etc.) to keep track of all relevant values and apply changes.
    /// </summary>
    private struct Erasable
    {
        public GameObject obj { get; private set; }
        public Sprite sprite { get; private set; }
        public byte[] maskPixels;
        public ErasableTaskWrapper erasableTask;

        /// <summary>
        /// An erasable object representation.
        /// </summary>
        /// <param name="obj">The GameObject that is an erasable.</param>
        public Erasable(GameObject obj)
        {
            this.obj = obj;
            sprite = obj.GetComponent<SpriteRenderer>().sprite;
            maskPixels = new byte[sprite.texture.width * sprite.texture.height];
            erasableTask = obj.GetComponent<ErasableTaskWrapper>();
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

            erasableTask.TaskProgress = erasedCount/(maskPixels.Length/100f);
        }
    }

    private void Awake()
    {
        m_playerHand = FindFirstObjectByType<PlayerHand>();
        m_drawPosTransform = transform.GetChild(0);
        m_taskTracker = FindFirstObjectByType<TaskTracker>();
        m_roomSaver = FindFirstObjectByType<RoomSaver>();
        m_roomSaver.OnScenesLoaded += RoomSaver_OnScenesLoaded;
    }

    private void Start()
    {
        InitializeTool();
    }

    private void OnEnable()
    {
        InputManager.Inputs.OnActivate_Held += UseTool;
        InputManager.Inputs.OnActivate_Ended += StopUseTool;
    }

    private void OnDisable()
    {
        InputManager.Inputs.OnActivate_Held -= UseTool;
        InputManager.Inputs.OnActivate_Ended -= StopUseTool;

        OnErasing_Ended?.Invoke();
        IsErasing = false;
    }

    private void RoomSaver_OnScenesLoaded()
    {
        PopulateErasables();
        m_roomSaver.OnScenesLoaded -= RoomSaver_OnScenesLoaded;
    }

    /// <summary>
    /// Called every frame until the activate button is released.
    /// </summary>
    public void UseTool()
    {
        if (!HandMoved()) return;

        var wasErasing = IsErasing;
        IsErasing = false;

        foreach (var erasable in m_erasables)
        {
            if (!erasable.obj.activeInHierarchy) continue;
            if (UpdateErasableMask(erasable)) 
            {
                erasable.ApplyMask();
                m_taskTracker.UpdateTaskTracker(erasable.erasableTask.TaskName, erasable.erasableTask.NewProgress);

                IsErasing = true;
            }
        }

        if (!wasErasing && IsErasing) OnErasing_Started?.Invoke();
        if (wasErasing && !IsErasing) OnErasing_Ended?.Invoke();
    }

    /// <summary>
    /// Called when the activate button is released. 
    /// </summary>
    public void StopUseTool() 
    {
        if (IsErasing)
        {
            OnErasing_Ended?.Invoke();
            IsErasing = false;
        }
    }

    /// <summary>
    /// Sets up the tool and related variables ready for use.
    /// </summary>
    private void InitializeTool()
    {
        tool.Initialize();
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
    /// Checks if the hand moved since last frame, updating the last position if it did.
    /// </summary>
    /// <returns>Whether or not the hand moved since last frame.</returns>
    private bool HandMoved()
    {
        var drawPointScreenPos = Camera.main.WorldToScreenPoint(m_drawPosTransform.position);
        m_drawPos = new Vector2Int(Mathf.RoundToInt(drawPointScreenPos.x), Mathf.RoundToInt(drawPointScreenPos.y));

        if (m_playerHand.IsMoving) return true;
        return false;
    }

    /// <summary>
    /// Updates the mask for the given erasable.
    /// </summary>
    /// <param name="erasable">The erasable whose mask to update.</param>
    /// <returns>Whether or not the mask was changed.</returns>
    private bool UpdateErasableMask(Erasable erasable)
    {
        var erasableScreenPos = Camera.main.WorldToScreenPoint(erasable.obj.transform.position);
        var erasableCentrePixel = new Vector2Int(Mathf.RoundToInt(erasableScreenPos.x), Mathf.RoundToInt(erasableScreenPos.y));
        var mouseDistFromErasableCentre = m_drawPos - erasableCentrePixel;
        var halfErasableSize = new Vector2Int(erasable.sprite.texture.width / 2, erasable.sprite.texture.height / 2);
        var halfToolSize = new Vector2Int(tool.mask.texture.width / 2, tool.mask.texture.height / 2);

        //Stop if brush is not on mask
        var outOfBoundsX = Math.Abs(mouseDistFromErasableCentre.x) - (halfToolSize.x * tool.size) >= halfErasableSize.x;
        var outOfBoundsY = Math.Abs(mouseDistFromErasableCentre.y) - (halfToolSize.y * tool.size) >= halfErasableSize.y;
        if (outOfBoundsX || outOfBoundsY) return false;

        var erased = false;

        for (var i = 0; i < tool.maskPixels.Length; i++)
        {
            if (tool.maskPixels[i] == 0) continue;

            var currentPixelOnBrush = GetPixelCoordinatesOnTexture(tool.mask.texture, i);
            var pixels = GetPixelsOnTexture(currentPixelOnBrush, mouseDistFromErasableCentre, halfErasableSize, halfToolSize);

            erased |= ApplyPixels(tool.maskPixels[i], erasable.maskPixels, pixels);
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
    /// Gets the pixels affected due to scaling of the brush based on the given pixel.
    /// </summary>
    /// <param name="currentPixelOnBrush">The pixel to start from, in x/y coordinates</param>
    /// <param name="mouseDistFromErasableCentre">Vector representing the mouse distance from the centre of the erasable in pixels.</param>
    /// <param name="halfErasableSize">Vector representing half the size of the erasable in pixels.</param>
    /// <param name="halfToolSize">Vector representing half the size of the tool in pixels.</param>
    /// <returns>The array of pixels affected by the given pixel.</returns>
    private int[] GetPixelsOnTexture(Vector2Int currentPixelOnBrush, Vector2Int mouseDistFromErasableCentre, Vector2Int halfErasableSize, Vector2Int halfToolSize)
    {
        //Get the position of this pixel on the erasable texture
        var xDist = (currentPixelOnBrush.x - halfToolSize.x) * tool.size;
        var yDist = (currentPixelOnBrush.y - halfToolSize.y) * tool.size;
        var distFromToolCentre = new Vector2Int(xDist, yDist);
        var pixelPosFromCentre = mouseDistFromErasableCentre + distFromToolCentre;

        //Convert coordinates to pixel in erasable texture
        var erasableTextureWidth = halfErasableSize.x * 2;
        var erasableTextureHeight = halfErasableSize.y * 2;
        var centrePixelX = halfErasableSize.x + pixelPosFromCentre.x;
        var centrePixelY = (halfErasableSize.y + pixelPosFromCentre.y) * erasableTextureWidth;
        var centrePixel = centrePixelX + centrePixelY;

        //Get all pixels affected due to brush size scaling
        var scalePixels = new List<int>();
        int xOffset, yOffset;
        bool outOfBoundsX, outOfBoundsY;

        for (var xCount = 1; xCount <= tool.size; xCount++)
        {
            for (var yCount = 1; yCount <= tool.size; yCount++)
            {
                xOffset = xCount - 1;
                yOffset = erasableTextureWidth * (yCount - 1);

                outOfBoundsX = centrePixelX + xOffset < 0 || centrePixelX + xOffset >= erasableTextureWidth;
                outOfBoundsY = centrePixelY + yOffset < 0 || halfErasableSize.y + pixelPosFromCentre.y + yCount - 1 >= erasableTextureHeight;
                if (outOfBoundsX || outOfBoundsY) continue;

                scalePixels.Add(centrePixel + xOffset + yOffset);
            }
        }

        return scalePixels.ToArray();
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
        var pixelStrength = toolMaskPixelAlpha * (tool.strength / 100f);
        var erased = false;

        foreach (var pixel in drawingPixels)
        {
            var newAlpha = Mathf.Clamp(erasableMaskPixels[pixel] + pixelStrength, 0f, 255f);
            var newValue = (byte)Mathf.FloorToInt(newAlpha);

            if (erasableMaskPixels[pixel] != newValue)
            {
                erasableMaskPixels[pixel] = newValue;
                erased = true;
            }
        }

        return erased;
    }
}
