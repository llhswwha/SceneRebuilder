using Base.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Mat2GosAsset : ScriptableObject
{
    public List<Mat2Gos> Items = new List<Mat2Gos>();

    //public Mat2GosList Items = new Mat2GosList();

    public static Dictionary<Material,List<GameObject>> GetMat2GosDict(MeshRenderer[] renderers)
    {
        
        StringBuilder sb = new StringBuilder();
        int missingCount = 0;
        Dictionary<Material, List<GameObject>> mat2Gos = new Dictionary<Material, List<GameObject>>();
        for (int i = 0; i < renderers.Length; i++)
        {
            MeshRenderer renderer = renderers[i];
            Material mat = renderer.sharedMaterial;
            if (mat == null)
            {

                MeshFilter mf = renderer.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh!=null)
                {
                    missingCount++;
                    sb.AppendLine($"[{missingCount}] name:{renderer.name} path:{renderer.transform.GetPath()} MeshFilter:{mf != null}");
                }
                continue;
            }
            if (!mat2Gos.ContainsKey(mat))
            {
                mat2Gos.Add(mat, new List<GameObject>());
            }
            mat2Gos[mat].Add(renderer.gameObject);
        }
        if (sb.Length > 0)
        {
            Debug.LogError($"missing Mats[{missingCount}]:\n{sb.ToString()}");
        }
        else
        {
            Debug.Log("no missing");
        }
        return mat2Gos;
    }

    public static void Save(Dictionary<Material, List<GameObject>> mat2Gos)
    {
#if UNITY_EDITOR
        //生成 SkillInfo实例
        Mat2GosAsset asset = ScriptableObject.CreateInstance<Mat2GosAsset>();
        //生成文件
        foreach (var mat in mat2Gos.Keys)
        {
            var list = mat2Gos[mat];
            Mat2Gos item = new Mat2Gos();
            item.name = mat.name;
            item.file = AssetDatabase.GetAssetPath(mat);
            item.mat = mat;
            foreach (var go in list)
            {
                item.pathList.Add(go.transform.GetPath());
            }
            asset.Items.Add(item);
        }
        AssetDatabase.CreateAsset(asset, Path);
        //AssetDatabase.CreateAsset(asset, "Assets/Mat2Gos.asset");

        Mat2GosList matList = new Mat2GosList();
        matList.AddRange(asset.Items);
        string path = Application.dataPath + XmlPath;
        try
        {
            SerializeHelper.Save(matList, path);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }

        Debug.Log($"Save mat2Gos:{mat2Gos.Count} Path:{Path} path:{path}");
#endif
    }

    public static string XmlPath = "\\..\\Mat2GosList.XML";

    public static string Path = "Assets/Models/闵行模型/Materials/Mat2Gos.asset";

    public static Mat2GosAsset Load()
    {
#if UNITY_EDITOR
        //ScriptableObject.Instantiate()

        Mat2GosAsset cubeAsset = AssetDatabase.LoadAssetAtPath<Mat2GosAsset>(Path);
        //do something
        return cubeAsset;
#else
        return null;
#endif
    }
}

[XmlType("Mat2GosList")]
[Serializable]
public class Mat2GosList :List<Mat2Gos>
{

}

[Serializable]
public class Mat2Gos //: ScriptableObject
{
    [XmlAttribute]
    public string name = "";
    [XmlIgnore]
    public Material mat;
    public Material GetMat()
    {
#if UNITY_EDITOR
        if (mat == null)
        {
            mat = AssetDatabase.LoadAssetAtPath<Material>(file);
        }
#endif
        return mat;
    }
    public Material GetMat2()
    {
#if UNITY_EDITOR
        //Assets/Models/闵行模型/闵行色卡材质球/Materials/
        string path = file;
        path = path.Replace("Assets/Scenes/SceneModels/闵行色卡材质球/Materials", "Assets/Scenes/SceneModels/闵行色卡材质球黑蓝/Materials");
        Material mat2 = AssetDatabase.LoadAssetAtPath<Material>(path);
        return mat2;
#else
        return mat;
#endif
    }

    public Material GetMat3()
    {
#if UNITY_EDITOR
        //Assets/Models/闵行模型/闵行色卡材质球/Materials/黑色.mat
        //Assets\Scenes\SceneModels\闵行色卡材质球黑蓝\Materials
        string path = file;
        path = path.Replace("Assets/Models/闵行模型/闵行色卡材质球/Materials", "Assets/Scenes/SceneModels/闵行色卡材质球黑蓝/Materials");
        Material mat2 = AssetDatabase.LoadAssetAtPath<Material>(path);
        return mat2;
#else
        return mat;
#endif
    }

    //Assets\Models\闵行模型\闵行色卡材质球\Materials

    public string file = "";

    public List<string> pathList = new List<string>();
}
