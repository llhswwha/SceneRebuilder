using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MaterialManager : MonoBehaviour
{
    public List<Material> Materials=new List<Material>();

    public List<Material> SharedMaterials=new List<Material>();

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

    [ContextMenu("ReplaceShader")]
    public void ReplaceShader()
    {
        foreach(var mat in SharedMaterials)
        {
            mat.shader=NewShader;
        }
    }

    [ContextMenu("InitMaterials")]
    public void InitMaterials()
    {
        LitShader=Shader.Find("HDRP/Lit");

        DateTime start = DateTime.Now;

        Materials.Clear();
        SharedMaterials.Clear();
        Shaders.Clear();

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
                }

                if(!Shaders.Contains(mat.shader))
                {
                    Shaders.Add(mat.shader);
                }
            }

            //if(Application)
            // foreach(var mat in r.materials)
            // {
            //     if(!Materials.Contains(mat))
            //     {
            //         Materials.Add(mat);
            //     }

            //     if(!Shaders.Contains(mat.shader))
            //     {
            //         Shaders.Add(mat.shader);
            //     }
            // }

            float progress = (float)i / count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("InitMaterials", $"Progress1 {i}/{count} {percents:F1}% {r.name}", progress))
            {
                break;
            }
        }
        //Count = allRenderers.Length;
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"InitMaterials count:{allRenderers.Length} time:{(DateTime.Now - start)}");
    }


}
