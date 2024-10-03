using UnityEngine;
using System.IO;

public abstract class ImageLoader : MonoBehaviour
{
    [SerializeField] protected string saveLocation; //Should match exactly with decoration manager set save path.

    /// <summary>
    /// Loads the texture from the file at the given path.
    /// </summary>
    /// <param name="FilePath">The path of the file.</param>
    /// <returns>The texture of the file.</returns>
    public Texture2D LoadTexture(string FilePath)
    {
        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);
            if (Tex2D.LoadImage(FileData))
                return Tex2D;
        }
        else
        {
            gameObject.SetActive(false);
        }
        return null;
    }
}
