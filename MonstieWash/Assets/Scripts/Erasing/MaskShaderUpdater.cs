using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[ExecuteInEditMode]
public class MaskShaderUpdater : MonoBehaviour
{
    private Material mat;

    private void OnValidate()
    {
        Awake();
    }

    private void Awake()
    {
        mat = GetComponent<Renderer>().sharedMaterial;
    }
    
    private void Update()
    {
        mat.SetFloat("_Main_Rotation", transform.rotation.eulerAngles.z);
    }
}
