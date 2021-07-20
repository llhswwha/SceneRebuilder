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

    public SubMesh(MeshFilter mf,int id)
    {
        meshFilter = mf;
        meshIndex = id;
    }

    public override string ToString()
    {
        if (meshFilter == null) return "NULL";
        return $"mesh:{meshFilter.name}, index:{meshIndex}";
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
        key = GetMatKey(m);
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

    public List<MeshFilter> MeshFilters = new List<MeshFilter>();

    public void AddList(List<MeshFilter> list)
    {
        MeshFilters.AddRange(list);
    }
}
