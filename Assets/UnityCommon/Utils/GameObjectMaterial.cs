using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 保存模型材质，用于修改材质后还原原来的材质
/// </summary>
public class GameObjectMaterial : MonoBehaviour
{

    private Material[] mmaterial;//材质

    public Material[] Mmaterial
    {
        get { return mmaterial; }
        set { mmaterial = value; }
    }

    public static void Recover(Transform t)
    {
        Recover(t.gameObject);
    }

    /// <summary>
    /// 恢复原来Render
    /// </summary>
    /// <param name="objs"></param>
    public static void RecoverObjs(List<Transform> objs)
    {
        foreach (Transform obj in objs)
        {
            if (obj.GetComponent<Renderer>() == null) continue;
            Recover(obj);
        }
    }

    /// <summary>
    /// 恢复原来Render
    /// </summary>
    /// <param name="objs"></param>
    public static void RecoverObjs(List<GameObject> objs)
    {
        foreach (GameObject obj in objs)
        {
            if (obj.GetComponent<Renderer>() == null) continue;
            Recover(obj);
        }
    }

    /// <summary>
    /// 还原
    /// </summary>
    /// <param name="obj"></param>
    public static void Recover(GameObject obj)
    {
        //GameObjectMaterial objMaterial = obj.GetComponent<GameObjectMaterial>();
        //if (objMaterial == null) return;
        //Material[] m = objMaterial.Mmaterial;
        //obj.GetComponent<Renderer>().materials = m;
        GameObjectMaterial[] objMaterial = obj.GetComponentsInChildren<GameObjectMaterial>();
        foreach (GameObjectMaterial material in objMaterial)
        {
            material.gameObject.GetComponent<Renderer>().materials = material.Mmaterial;
        }
    }

    public static void Init(List<Transform> transforms)
    {
        foreach (Transform transform in transforms)
        {
            Init(transform.gameObject);
        }
    }

    /// <summary>
    /// 初始化（绑定脚本）
    /// </summary>
    /// <param name="go"></param>
    public static void Init(GameObject go)
    {
        if (go.GetComponent<Renderer>() != null)
        {
            InitOP(go);
        }
    }

    /// <summary>
    /// 初始化（绑定脚本）
    /// </summary>
    /// <param name="go"></param>
    public static void InitOP(GameObject go)
    {
        if (go.GetComponent<GameObjectMaterial>() == null)
        {
            GameObjectMaterial m = go.AddComponentEx<GameObjectMaterial>();
            m.Mmaterial = go.GetMaterials();
        }
    }


    public static void Destroy(GameObject go)
    {
        GameObjectMaterial m = go.GetComponent<GameObjectMaterial>();
        if (m != null)
        {
            DestroyImmediate(m);
        }
    }

    public static void SetColor(GameObject obj, Color color)
    {
        Material[] ms = obj.GetComponent<Renderer>().materials;
        for (int i = 0; i < ms.Length; i++)
        {
            ms[i].color = color;
        }
    }

    /// <summary>
    /// 设置物体及其子物体的透明度
    /// </summary>
    public static void SetAllTransparent(GameObject go, float percent)
    {
        go.SetAllTransparentEx(percent);
    }

    public static void SetTransparentOn(GameObject obj, float percent)
    {
        Init(obj);//保存材质
        obj.SetTransparent(percent);
    }

    public static void SetTransparentOn(Renderer render, float percent)
    {
        Init(render.gameObject);//保存材质
        render.SetTransparent(percent);
    }

    public static void SetTransparentOff(GameObject obj)
    {
        Recover(obj);
    }

    public static void SetTransparentOff(Renderer render)
    {
        Recover(render.gameObject);
    }
}
