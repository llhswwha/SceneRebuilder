using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialKey : MonoBehaviour
{
    public string Key = "";
    [ContextMenu("GetKey")]
    public void GetKey()
    {
        MeshRenderer render = gameObject.GetComponent<MeshRenderer>();
        Material mat = render.sharedMaterial;
        Key = $"{mat.GetColor("_BaseColor")},{mat.GetFloat("_Metallic")},{mat.GetFloat("_Smoothness")},{mat.GetTexture("_BaseColorMap")},{mat.GetTexture("_NormalMap")}";
    }

    public static string GetMatKey(Material mat)
    {
        return $"{mat.GetColor("_BaseColor")},{mat.GetFloat("_Metallic")},{mat.GetFloat("_Smoothness")},{mat.GetTexture("_BaseColorMap")},{mat.GetTexture("_NormalMap")}";
    }
}
