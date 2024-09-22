using UnityEngine;

/// <summary>
/// Finds scene camera, adds it to canvas. For use with world space UI.
/// </summary>
public class CanvasController : MonoBehaviour
{
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        Camera camera = FindFirstObjectByType<Camera>();

        canvas.worldCamera = camera;
    }
}
