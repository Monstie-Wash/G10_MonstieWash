using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CustomPolaroidLoader : ImageLoader
{
    private void Awake()
    {
        var image = GetComponent<Image>();

        //Get file location lenght
        var savePath = Application.persistentDataPath + saveLocation;
        var fileCount = Directory.GetFiles(savePath).Length - 1;

        var spriteTex = LoadTexture($"{savePath}/Polaroid_{fileCount}.Png");
        var newSprite = Sprite.Create(spriteTex, new Rect(0, 0, spriteTex.width, spriteTex.height), new Vector2(0, 0), 100f, 0, SpriteMeshType.Tight);
        image.sprite = newSprite;
    }
}
