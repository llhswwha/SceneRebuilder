using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class MeshMaterial
{
    public string GetName()
    {
        if (matInfo == null) return "NULL";
        return matInfo.mat.name;
    }

    public Material GetMat()
    {
        if (matInfo == null) return null;
        return matInfo.mat;
    }

    public Color GetColor()
    {
        if (matInfo == null) return new Color(0, 0, 0, 0);
        return matInfo.color;
    }

    public string GetKey()
    {
        if (matInfo == null) return "";
        return matInfo.key;
    }

    public SubMesh subMesh;

    public MatInfo matInfo;

    public MeshMaterial(Material mat, MeshFilter meshFilter, int index)
    {
        matInfo = new MatInfo(mat);
        subMesh = new SubMesh(meshFilter, index);
    }

    public override string ToString()
    {
        return $"mat:{matInfo.mat.name}, {subMesh}, key:{matInfo.key}";

    }

    public string GetInfo()
    {
        return $"color:{matInfo.color} {subMesh}";
    }
}

[Serializable]
public class SharedMeshMaterial
{
    public string GetName()
    {
        if (matInfo == null) return "NULL";
        return matInfo.mat.name;
    }

    public Material GetMat()
    {
        if (matInfo == null) return null;
        return matInfo.mat;
    }

    public Color GetColor()
    {
        if (matInfo == null) return new Color(0, 0, 0, 0);
        return matInfo.color;
    }

    public void SetColor(Color color)
    {
        if (matInfo != null)
        {
            matInfo.SetColor(color);
        }
    }

    public void SetColor(float r,float g,float b)
    {
        if (matInfo != null)
        {
            matInfo.SetColor(r, g, b);
        }
    }

    public string GetKey()
    {
        if (matInfo == null) return "";
        return matInfo.key;
    }

    public List<SubMesh> subMeshs = new List<SubMesh>();

    public MatInfo matInfo;

    public SharedMeshMaterial(Material mat)
    {
        matInfo = new MatInfo(mat);
    }

    public SharedMeshMaterial(Material mat, MeshFilter meshFilter, int index)
    {
        matInfo = new MatInfo(mat);
        AddMesh(meshFilter, index);
    }

    public void AddMesh(MeshFilter meshFilter, int index)
    {
        subMeshs.Add(new SubMesh(meshFilter, index));
    }

    public override string ToString()
    {
        return $"mat:{matInfo.mat.name}, subMeshs:{subMeshs.Count}, key:{matInfo.key}";

    }

    public string GetInfo()
    {
        return $"color:{matInfo.color} {subMeshs.Count}";
    }
}

[Serializable]
public class SharedMeshMaterialList : List<SharedMeshMaterial>
{
    public int RendererCount = 0;

    private Dictionary<string, SharedMeshMaterial> matDict = new Dictionary<string, SharedMeshMaterial>();
    //public List<SharedMeshMaterial> Mats = new List<SharedMeshMaterial>();

    //public List<string> MatKeys = new List<string>();

    public SharedMeshMaterial GetMaterial(Material m)
    {
        string key = MatInfo.GetMatKey(m);
        if (matDict.Count != this.Count)
        {
            matDict.Clear();
            foreach(var mesMat in this)
            {
                matDict.Add(mesMat.GetKey(), mesMat);
            }
        }

        //int id = MatKeys.IndexOf(key);

        if (matDict.ContainsKey(key))
        {
            return matDict[key];
        }
        else
        {
            SharedMeshMaterial newMat = new SharedMeshMaterial(m);
            matDict.Add(newMat.GetKey(), newMat);
            this.Add(newMat);
            return newMat;
        }

    }

    public void SortByMat()
    {
        this.Sort((a, b) =>
        {
            return a.matInfo.mat.name.CompareTo(b.matInfo.mat.name);
        });
    }

    //public void SortByMesh()
    //{
    //    this.Sort((a, b) =>
    //    {
    //        return a.subMesh.meshFilter.name.CompareTo(b.subMesh.meshFilter.name);
    //    });
    //}

    public void SortByKey()
    {
        this.Sort((a, b) =>
        {
            return a.matInfo.key.CompareTo(b.matInfo.key);
        });
    }

    public static SharedMeshMaterialList GetMeshMaterialList(MeshRenderer[] renderers)
    {
        SharedMeshMaterialList meshMaterials = new SharedMeshMaterialList();
        meshMaterials.RendererCount = renderers.Length;
        for (int i = 0; i < renderers.Length; i++)
        {
            MeshRenderer renderer = renderers[i];
            if (renderer == null) continue;
            NoCombine noCombine = renderer.GetComponent<NoCombine>();
            if (noCombine != null)
            {
                continue;
            }

            for (int i1 = 0; i1 < renderer.sharedMaterials.Length; i1++)
            {
                Material mat = renderer.sharedMaterials[i1];
                if (mat == null) continue;
                MeshFilter filter = renderer.GetComponent<MeshFilter>();
                //MeshMaterial meshMaterial = new MeshMaterial(mat, filter, i1);
                //meshMaterials.Add(meshMaterial);
                SharedMeshMaterial meshMat = meshMaterials.GetMaterial(mat);
                meshMat.AddMesh(filter, i1);


            }
        }
        return meshMaterials;
    }
}

[Serializable]
public class SubMesh
{
    public MeshFilter meshFilter;
    public int meshIndex;

    public string GetName()
    {
        if (meshFilter == null) return "NULL";
        //return meshFilter.name+"_"+meshIndex;
        return $"{meshFilter.name}[{meshIndex}]";
    }

    public SubMesh(MeshFilter mf, int id)
    {
        meshFilter = mf;
        meshIndex = id;
    }

    public override string ToString()
    {
        if (meshFilter == null) return "NULL";
        return $"mesh:{meshFilter.name}, index:{meshIndex}";
    }

    public Mesh sharedMesh
    {
        get
        {
            if (meshFilter == null) return null;
            return meshFilter.sharedMesh;
        }
    }
}

public class SubMeshList:List<SubMesh>
{
    public List<MeshFilter> GetMeshFilters()
    {
        List<MeshFilter> list = new List<MeshFilter>();
        foreach(var item in this)
        {
            list.Add(item.meshFilter);
        }
        return list;
    }
}

[Serializable]
public class MeshMaterialList : List<MeshMaterial>
{
    public void SortByMat()
    {
        this.Sort((a, b) =>
        {
            return a.matInfo.mat.name.CompareTo(b.matInfo.mat.name);
        });
    }

    public void SortByMesh()
    {
        this.Sort((a, b) =>
        {
            return a.subMesh.meshFilter.name.CompareTo(b.subMesh.meshFilter.name);
        });
    }

    public void SortByKey()
    {
        this.Sort((a, b) =>
        {
            return a.matInfo.key.CompareTo(b.matInfo.key);
        });
    }

    public static MeshMaterialList GetMeshMaterialList(MeshRenderer[] renderers)
    {
        MeshMaterialList meshMaterials = new MeshMaterialList();
        for (int i = 0; i < renderers.Length; i++)
        {
            MeshRenderer renderer = renderers[i];
            if (renderer == null) continue;
            NoCombine noCombine = renderer.GetComponent<NoCombine>();
            if (noCombine != null)
            {
                continue;
            }

            for (int i1 = 0; i1 < renderer.sharedMaterials.Length; i1++)
            {
                Material mat = renderer.sharedMaterials[i1];
                if (mat == null) continue;
                MeshFilter filter = renderer.GetComponent<MeshFilter>();
                MeshMaterial meshMaterial = new MeshMaterial(mat, filter, i1);
                meshMaterials.Add(meshMaterial);
            }
        }
        return meshMaterials;
    }
}

[Serializable]
public class MatInfo
{
    public Material mat;

    public string key = "";

    public Color color;
    public Texture tex;

    public Shader shader;

    public bool isDoubleSide = false;

    public Texture normal;

    public void SetColor(Color color)
    {
        if (this.color == color) return;
        this.color = color;
        SetColor(mat, color);
    }

    public void SetColor(float r,float g,float b)
    {
        Color newColor = new Color(r, g, b, color.a);
        SetColor(newColor);
    }

    public MatInfo(Material m)
    {
        this.mat = m;
        color = GetColor(m);
        //Key = color.ToString();
        shader = m.shader;
        key = GetMatKey(m);
        tex = GetTexture(m);
        normal = GetNormal(m);
        isDoubleSide = GetIsDoubleSide(m);
    }

    public static Texture GetTexture(Material m)
    {
        if(m.HasProperty("_BaseColorMap"))
        {
            return m.GetTexture("_BaseColorMap");
        }
        else
        {
            return null;
        }
    }

    public static Texture GetNormal(Material m)
    {
        if (m.HasProperty("_NormalMap"))
        {
            return m.GetTexture("_NormalMap");
        }
        else
        {
            return null;
        }
    }

    public static bool GetIsDoubleSide(Material m)
    {
        if (m.HasProperty("_DoubleSidedEnable"))
        {
            return m.GetFloat("_DoubleSidedEnable")!=0;
        }
        else
        {
            return false;
        }
    }

    public static Color GetColor(Material mat)
    {
        if (mat.HasProperty("_BaseColor"))
        {
            return mat.GetColor("_BaseColor");
        }
        else if(mat.HasProperty("_MainColor"))
        {
            return mat.GetColor("_MainColor");
        }
        else if (mat.HasProperty("_Color"))
        {
            return mat.GetColor("_Color");
        }
        else
        {
            //return mat.color;
            return new Color(0, 0, 0, 0);
        }
    }

    public static void SetColor(Material mat, Color color)
    {
        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor("_BaseColor",color);
        }
        else if (mat.HasProperty("_MainColor"))
        {
            mat.SetColor("_MainColor", color);
        }
        else if (mat.HasProperty("_Color"))
        {
            mat.SetColor("_Color", color);
        }
        else
        {
            //mat.color = color;
        }
    }

    public static string GetMatKey(Material mat)
    {
        string key = "";
        key += GetColor(mat);
        if (mat.HasProperty("_Metallic"))
        {
            key += mat.GetFloat("_Metallic");
        }
        if (mat.HasProperty("_Smoothness"))
        {
            key += mat.GetFloat("_Smoothness");
        }
        if (mat.HasProperty("_BaseColorMap"))
        {
            key += mat.GetTexture("_BaseColorMap");
        }
        if (mat.HasProperty("_NormalMap"))
        {
            key += mat.GetTexture("_NormalMap");
        }
        return key;
        //return $"{mat.GetColor("_BaseColor")},{mat.GetFloat("_Metallic")},{mat.GetFloat("_Smoothness")},{mat.GetTexture("_BaseColorMap")},{mat.GetTexture("_NormalMap")}";
    }

    public SubMeshList MeshFilters = new SubMeshList();

    public void AddList(List<SubMesh> list)
    {
        MeshFilters.AddRange(list);
    }

    public void SetIsDoubleSide(bool isD)
    {
        if (isDoubleSide == isD) return;
        SetIsDoubleSide(mat, isD);
        isDoubleSide = isD;
    }

    public void SetTexture(Texture t)
    {
        if (tex == t) return;
        SetTexture(mat, t);
        tex = t;
    }

    public void SetNormal(Texture t)
    {
        if (normal == t) return;
        SetTexture(mat, t);
        normal = t;
    }

    public static void SetIsDoubleSide(Material m, bool isD)
    {
        Debug.LogError($"SetIsDoubleSide {m} {isD} {m.HasProperty("_DoubleSidedEnable")}");
        if (m.HasProperty("_DoubleSidedEnable"))
        {
            m.SetFloat("_DoubleSidedEnable", isD ? 1 : 0);
        }
        else
        {
            //return false;
        }
    }

    public static void SetTexture(Material m, Texture tex)
    {
        if (m.HasProperty("_BaseColorMap"))
        {
            m.SetTexture("_BaseColorMap",tex);
        }
        else
        {
            //return null;
        }
    }

    public static void SetNormal(Material m, Texture tex)
    {
        if (m.HasProperty("_NormalMap"))
        {
            m.SetTexture("_NormalMap", tex);
        }
        else
        {
            //return null;
        }
    }
}
