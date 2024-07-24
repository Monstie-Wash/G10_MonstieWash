using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;

[ExecuteInEditMode]
public class MaskShaderUpdater : MonoBehaviour
{
    [SerializeField] private GameObject mask;
    private Renderer renderer;
    private Material mat;
    private Transform maskTransform;
    private Texture maskTexture;

    private void OnValidate()
    {

    }

    private void Awake()
    {
        
    }

    private void Update()
    {
        
    }

    public void Refresh()
    {
        renderer = GetComponent<Renderer>();
        mat = renderer.sharedMaterial;


        if (mask == null) return;
        maskTransform = mask.transform;
        Sprite maskSprite = mask.GetComponent<SpriteRenderer>().sprite;
        Sprite mainSprite = GetComponent<SpriteRenderer>().sprite;
        maskTexture = maskSprite.texture;

        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        propertyBlock.SetTexture("_MaskTex", maskTexture);
        propertyBlock.SetTexture("_MainTex", mainSprite.texture);
        propertyBlock.SetFloat("_Main_PPU", mainSprite.pixelsPerUnit);
        propertyBlock.SetFloat("_Mask_PPU", maskSprite.pixelsPerUnit);
        propertyBlock.SetFloat("_Main_Rotation", transform.rotation.eulerAngles.z);
        propertyBlock.SetVector("_Mask_Position", maskTransform.position);
        propertyBlock.SetFloat("_Mask_Rotation", maskTransform.rotation.eulerAngles.z);
        propertyBlock.SetVector("_Mask_Scale", maskTransform.localScale);

        renderer.SetPropertyBlock(propertyBlock);

        /*
        Texture2D tex = new Texture2D(300, 300);
        RenderTexture rt = RenderTexture.GetTemporary(300, 300, 0);
        Graphics.Blit(null, rt, mat, 0);
        tex.ReadPixels(new Rect(0, 0, 300, 300), 0, 0);
        tex.Apply();

        propertyBlock.SetTexture("_MainTex", tex);

        renderer.SetPropertyBlock(propertyBlock);
        */
        int mainppu = (int)mat.GetFloat("_Main_PPU");
        int maskppu = (int)mat.GetFloat("_Mask_PPU");
        if (mainppu != maskppu) Debug.LogWarning($"{name} pixels per unit does not match its mask's pixels per unit. {mainppu} | {maskppu}");
    }
}

#region Custom Editor
#if UNITY_EDITOR
[CustomEditor(typeof(MaskShaderUpdater))]
public class MaskShaderUpdaterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();

        MaskShaderUpdater msu = (MaskShaderUpdater)target;
        if (GUILayout.Button("Refresh Shader")) msu.Refresh();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
#endregion