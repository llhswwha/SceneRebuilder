using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GlobalMaterialManager : MonoBehaviour
{
    public List<Material> Materials=new List<Material>();

    public List<Material> SharedMaterials=new List<Material>();

    public List<Texture> Textures=new List<Texture>();

    public List<Material> CombinedMaterials=new List<Material>();

    public List<Shader> Shaders=new List<Shader>();

    public Shader LitShader;

    public Shader NewShader;

    [ContextMenu("ReplaceShaderDefault")]
    public void ReplaceShaderDefault()
    {
        foreach(var mat in SharedMaterials)
        {
            mat.shader=Shader.Find("HDRP/Lit");
        }
    }

    public bool IsReplaceAll=true;

    [ContextMenu("ReplaceShader")]
    public void ReplaceShader()
    {
        ReplaceShader(IsReplaceAll);
    }

    public void ReplaceShader(bool isAll)
    {
        Debug.Log("ReplaceShader:"+isAll);
        foreach(var mat in SharedMaterials)
        {
            if(isAll){
                mat.shader=NewShader;
            }
            else{
                var txt=mat.GetTexture("_BaseColorMap");
                if(txt==null){
                    mat.shader=NewShader;
                }
            }
            
        }
    }

    [ContextMenu("InitMaterials")]
    public string InitMaterials()
    {
        LitShader=Shader.Find("HDRP/Lit");

        DateTime start = DateTime.Now;

        Materials.Clear();
        SharedMaterials.Clear();
        Shaders.Clear();
        Textures.Clear();

        ProgressBarHelper.DisplayProgressBar("ClearIds", "Start", 0);
        var allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        int count = allRenderers.Length;
        for (int i = 0; i < count; i++)
        {
            MeshRenderer r = allRenderers[i];

            foreach(var mat in r.sharedMaterials)
            {
                if(!SharedMaterials.Contains(mat))
                {
                    SharedMaterials.Add(mat);
                    var txt=mat.GetTexture("_BaseColorMap");
                    Textures.Add(txt);
                }

                if(!Shaders.Contains(mat.shader))
                {
                    Shaders.Add(mat.shader);
                }
            }

            float progress = (float)i / count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("InitMaterials", $"Progress1 {i}/{count} {percents:F1}% {r.name}", progress))
            {
                break;
            }
        }
        //Count = allRenderers.Length;
        ProgressBarHelper.ClearProgressBar();
        string log=$"Mats Rcount:{allRenderers.Length} matCount:{SharedMaterials.Count} time:{(DateTime.Now - start)}";
        Debug.LogError(log);

        DateTime start2 = DateTime.Now;
        var mats = MeshCombineHelper.GetMatFilters(allRenderers, out count, false);
        CombinedMaterials=mats.Keys.ToList();
        string log2=$"CombineMat mats:{mats.Count},{(DateTime.Now - start).ToString()}";
        Debug.LogError(log2);

        return log+"\n"+log2;
    }

    [ContextMenu("GetCombineMaterial")]
    public void GetCombineMaterial()
    {
        DateTime start = DateTime.Now;
        var allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        int count = 0;
        var mats = MeshCombineHelper.GetMatFilters(allRenderers, out count, false);
        CombinedMaterials=mats.Keys.ToList();
        Debug.LogError($"GetCombineMaterial {(DateTime.Now - start).ToString()},mats:{mats.Count},count:{count}");
    }


    [ContextMenu("SetCombineMaterial")]
    public void SetCombineMaterial()
    {
        DateTime start = DateTime.Now;
        var allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        int count = 0;
        var mats = MeshCombineHelper.GetMatFilters(allRenderers, out count, true);
        CombinedMaterials=mats.Keys.ToList();
        Debug.LogError($"GetCombineMaterial {(DateTime.Now - start).ToString()},mats:{mats.Count},count:{count}");
    }
}