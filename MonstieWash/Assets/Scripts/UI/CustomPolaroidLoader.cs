using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CustomPolaroidLoader : MonoBehaviour
{
    [SerializeField] private string saveLocation; //Should match exactly with decoration manager set save path.

    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();

        Texture2D spriteTex = LoadTexture(Application.persistentDataPath + saveLocation);
        Sprite NewSprite = Sprite.Create(spriteTex, new Rect(0, 0, spriteTex.width, spriteTex.height), new Vector2(0, 0), 100f, 0, SpriteMeshType.Tight);
        img.sprite = NewSprite;
    }

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
            img.gameObject.SetActive(false);
        }
        return null;
    }
}
