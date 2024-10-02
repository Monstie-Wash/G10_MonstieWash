using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CustomPolaroidLoader : ImageLoader
{
    private void Awake()
    {
        var image = GetComponent<Image>();

        //Get file location lenght
        var fileCount = Directory.GetFiles(Application.persistentDataPath).Length - 1;


        var spriteTex = LoadTexture($"{Application.persistentDataPath}{saveLocation}/Polaroid_{fileCount}.Png");
        var newSprite = Sprite.Create(spriteTex, new Rect(0, 0, spriteTex.width, spriteTex.height), new Vector2(0, 0), 100f, 0, SpriteMeshType.Tight);
        image.sprite = newSprite;
    }
}
