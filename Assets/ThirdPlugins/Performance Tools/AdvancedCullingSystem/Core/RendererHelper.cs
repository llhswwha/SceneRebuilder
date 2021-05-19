using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RendererHelper
{

    public static void SetRendererVisible(Renderer render, bool isVisible)
    {
        if (isVisible)
        {
            ShowRenderer(render);
        }
        else
        {
            HideRenderer(render);
        }
    }

    public static bool IsTest = true;

    public static void ShowRenderer(this Renderer render)
    {
        if (render == null)
        {
           Debug.LogError("ShowRenderer render == null");
           return;
        }
        render.enabled = true;

        // if (IsTest)
        // {
        //    foreach (var item in render.materials)
        //    {
        //        item.SetColor("_BaseColor", Color.red);
        //    }
        //    //render.material.SetColor("_BaseColor",Color.red);
        // }
        // else
        // {
        //    render.enabled = true;
        // }

        //render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;//显示物体
    }

    public static void HideRenderer(this Renderer render)
    {
        if (render == null)
        {
           Debug.LogError("HideRenderer render == null");
           return;
        }

        render.enabled = false;//

        // if (IsTest)
        // {
        //    foreach (var item in render.materials)
        //    {
        //        item.SetColor("_BaseColor", Color.blue);
        //    }
        //    //render.material.SetColor("_BaseColor", Color.blue);
        // }
        // else
        // {
        //    render.enabled = false;//
        // }


        //render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;//隐藏物体,留下阴影
        //3.3w个有阴影的物体
        //shadowCastingMode：50ms,20FPS，Set Pass 70
        //enabled：100FPS，Set Pass 78
    }

    public static Dictionary<Renderer, string> Renderer2Id = new Dictionary<Renderer, string>();
    public static Dictionary<string, Renderer> Id2Renderer = new Dictionary<string, Renderer>();

    public static string GetRendererID(this Renderer renderer)
    {
        if (renderer == null) return "";
        string id = "";
        if (!Renderer2Id.ContainsKey(renderer))
        {
            string name = renderer.name;
            GameObject go = renderer.gameObject;
            Transform t = go.transform;
            Transform p = t.parent;
            string pName = "p";
            if (p != null)
            {
                pName = p.name;
            }

            Vector3 pos = t.position;

            id = $"{name}_{pName}_{pos.x}_{pos.y}_{pos.z}";
            Renderer2Id.Add(renderer, id);
            if (Id2Renderer.ContainsKey(id))
            {
                Debug.LogError("Id2Renderer.ContainsKey(id):"+ id);
            }
            else
            {
                Id2Renderer.Add(id, renderer);
            }
            
        }
        else
        {
            id = Renderer2Id[renderer];
        }
        return id;
    }

    public static Renderer GetRendererById(string id)
    {
        InitRenderDict();
        if(Id2Renderer.ContainsKey(id))
        {
            return Id2Renderer[id];
        }
        else
        {
            return null;
        }
    }

    public static MeshRenderer[] InitRenderDict()
    {
        if (Renderer2Id.Count == 0)
        {
            DateTime start=DateTime.Now;
            var renderers=GameObject.FindObjectsOfType<MeshRenderer>();
            foreach (var render in renderers)
            {
                render.GetRendererID();
            }
            Debug.LogError("RendererHelper.InitRenderDict:"+(DateTime.Now-start).ToString());
            return renderers;
        }
        else
        {
            return null;
        }
    }

}
