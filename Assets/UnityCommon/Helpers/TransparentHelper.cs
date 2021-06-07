using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 设置物体透明效果
/// </summary>
public static class TransparentHelper
{
    /// <summary>
    /// 设置物体及其子物体的透明度
    /// </summary>
    public static void SetAllTransparentEx(this GameObject go, float percent)
    {
        //Init(obj);//保存材质
        //obj.SetAllTransparent(percent);

        List<Transform> childs = go.GetChildrenTransform(true);
        foreach (Transform t in childs)
        {
            GameObjectMaterial.Init(t.gameObject);//保存材质
            t.SetTransparent(percent);
        }
    }

    /// <summary>
    /// 还原
    /// </summary>
    /// <param name="obj"></param>
    public static void RecoverTransparent(this GameObject obj)
    {
        GameObjectMaterial[] objMaterial = obj.GetComponentsInChildren<GameObjectMaterial>();
        foreach (GameObjectMaterial material in objMaterial)
        {
            material.gameObject.GetComponent<Renderer>().materials = material.Mmaterial;
        }
    }

    /// <summary>
    /// 将游戏物体设置成透明的（包括子物体也都设置成透明的）
    /// </summary>
    /// <param name="t"></param>
    /// <param name="transparent"></param>
    /// <returns></returns>
    public static bool SetTransparent(this Transform t, float transparent)
    {
        return SetTransparent(t.gameObject, transparent);
        //color
        //color = new Color(color.r, color.g, color.b, transparent);
        //return SetTransparent(t.gameObject, color, true, true);
    }

    /// <summary>
    /// 将游戏物体设置成透明的（包括子物体也都设置成透明的）
    /// </summary>
    /// <param name="go"></param>
    /// <param name="transparent"></param>
    /// <returns></returns>
    public static bool SetTransparent(this GameObject go, float transparent)
    {
        Renderer render = go.GetComponent<Renderer>();
        if (render == null) return false;
        return SetTransparent(render, transparent);
    }

    /// <summary>
    /// 将游戏物体设置成透明的（包括子物体也都设置成透明的）
    /// </summary>
    /// <param name="render"></param>
    /// <param name="transparent"></param>
    /// <returns></returns>
    public static bool SetTransparent(this Renderer render, float transparent)
    {
        Shader shader = ShaderHelper.GetTransparentDiffuseShader();
        //Shader shader = ShaderHelper.GetTransparentAlphaBlendShader();

        for (int i = 0; i < render.materials.Length; i++)
        {
            Color color = render.materials[i].color;
            Color transparentColor = new Color(color.r, color.g, color.b, transparent);
            SetTransparentMaterial(render, i, shader, transparentColor, true);
            //StardardShaderSet.SetMaterialRenderingMode(mm, RenderingMode.Transparent);
        }
        return true;
    }


    ///// <summary>
    ///// 将游戏物体设置成透明的
    ///// </summary>
    //public static bool SetTransparent(this GameObject go, float transparent, bool isSpecular, bool removeTexture = false)
    //{
    //    Color colorNew = new Color(color.r, color.g, color.b, transparent);
    //    return SetTransparent(go, colorNew, isSpecular, removeTexture);
    //}

    /// <summary>
    /// 将游戏物体设置成透明的（包括子物体也都设置成透明的）
    /// </summary>
    /// <param name="go"></param>
    /// <param name="transparent"></param>
    /// <param name="color"></param>
    /// <param name="isSpecular"></param>
    /// <param name="removeTexture"></param>
    /// <returns></returns>
    public static bool SetTransparent(this GameObject go, Color color, bool isSpecular, bool removeTexture = false)
    {
        Renderer render = go.GetComponent<Renderer>();
        if (render == null) return false;

        Shader shader = ShaderHelper.GetTransparentShader(isSpecular);

        for (int i = 0; i < render.materials.Length; i++)
        {
            SetTransparentMaterial(render, i, shader, color, removeTexture);
            //StardardShaderSet.SetMaterialRenderingMode(mm, RenderingMode.Transparent);
        }
        return true;
    }

    /// <summary>
    /// 修改材质，达到透明的效果
    /// </summary>
    /// <param name="render"></param>
    /// <param name="i"></param>
    /// <param name="shader"></param>
    /// <param name="color"></param>
    /// <param name="removeTexture"></param>
    private static void SetTransparentMaterial(Renderer render, int i, Shader shader, Color color, bool removeTexture)
    {
        //Material mm = new Material(render.materials[i]);
        //Material mm = render.materials[i];
        //render.materials[i] = mm;
        render.materials[i].shader = shader;
        render.materials[i].color = color;
        if (removeTexture)
        {
            render.materials[i].mainTexture = null;
        }
    }

    /// <summary>
    /// 修改颜色
    /// </summary>
    /// <param name="go"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static bool SetColor(this GameObject go, Color color)
    {
        Renderer render = go.GetComponent<Renderer>();
        if (render == null) return false;
        for (int i = 0; i < render.materials.Length; i++)
        {
            Material m = render.materials[i];
            if (!m.HasProperty("_Color")) continue;
            float transparent1 = m.color.a; //原来的透明度
            float transparent2 = color.a; //新的透明度
            if (transparent2 < 1) //color有透明度的话取该透明度，没有透明度的话取原来的透明度
            {
                transparent1 = transparent2;
            }
            render.materials[i].color = new Color(color.r, color.g, color.b, transparent1);
        }
        return true;
    }

    /// <summary>
    /// 设置物体及其子物体的透明度
    /// </summary>
    /// <param name="go"></param>
    /// <param name="transparent"></param>
    public static void SetAllTransparent(this GameObject go, float transparent)
    {
        SetTransparent(go, transparent);
        List<Transform> childs = go.GetChildrenTransform();
        foreach (Transform t in childs)
        {
            SetTransparent(t, transparent);
        }
    }

}
