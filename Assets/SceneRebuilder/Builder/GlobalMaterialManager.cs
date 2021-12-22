using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static MeshCombineHelper;

public class GlobalMaterialManager : SingletonBehaviour<GlobalMaterialManager>
{
    // private static GlobalMaterialManager _instance;

    // public static GlobalMaterialManager Instance
    // {
    //     get
    //     {
    //         if(_instance==null)
    //         {
    //             _instance = GameObject.FindObjectOfType<GlobalMaterialManager>();
    //         }
    //         if(_instance==null)
    //         {
    //             GameObject insGo=new GameObject(typeof(GlobalMaterialManager).Name);
    //             _instance = insGo.AddComponent<GlobalMaterialManager>();
    //         }
    //         return _instance;
    //     }
    // }

    public SharedMeshMaterialList meshMaterialList = new SharedMeshMaterialList();

    public List<Material> Materials=new List<Material>();

    public List<Material> SharedMaterials=new List<Material>();

    public List<MeshRenderer> MatRenderers = new List<MeshRenderer>();

    public List<Texture> Textures=new List<Texture>();

    public List<Material> CombinedMaterials=new List<Material>();

    public List<Shader> Shaders=new List<Shader>();

    public Shader LitShader;

    public Shader NewShader;

    public Material NewMaterial;

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

    [ContextMenu("SetAllOneMaterial")]
    internal void SetAllOneMaterial()
    {
        var allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        int count = allRenderers.Length;
        for (int i = 0; i < count; i++)
        {
            MeshRenderer r = allRenderers[i];
            //r.material = NewMaterial;
            r.sharedMaterial = NewMaterial;
        }
    }

    public void ReplaceShader(bool isAll)
    {
        Debug.Log("ReplaceShader:"+isAll);
        foreach(var mat in SharedMaterials)
        {
            if(mat.shader.name=="HDRP/Lit")
            {
                if (isAll)
                {
                    mat.shader = NewShader;
                }
                else
                {
                    var txt = mat.GetTexture("_BaseColorMap");//HDRP/Lit
                    if (txt == null)
                    {
                        mat.shader = NewShader;
                    }
                }
            }
            else
            {
                Debug.LogError("Not HDRP/Lit :"+mat+"|"+mat.shader);
            }
        }
    }

    public Color DefaultColor = new Color(1, 1, 1, 1);

    public GameObject LocalTarget = null;

    public void ResetColor()
    {
        foreach(var mat in meshMaterialList)
        {
            mat.SetColor(DefaultColor.r,DefaultColor.g,DefaultColor.b);
        }
    }

    public void GetSharedMaterials()
    {
        MeshRenderer[] allRenderers=null;
        if (LocalTarget == null)
        {
            allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);

            meshMaterialList = SharedMeshMaterialList.GetMeshMaterialList(allRenderers);
        }
        else
        {
            allRenderers = LocalTarget.GetComponentsInChildren<MeshRenderer>(true);

            meshMaterialList = SharedMeshMaterialList.GetMeshMaterialList(allRenderers);
        }

        Debug.LogError($"GetSharedMaterials LocalTarget:{LocalTarget} allRenderers:{allRenderers.Length} meshMaterialList:{meshMaterialList.Count}");
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
        MatRenderers.Clear();

        ProgressBarHelper.DisplayProgressBar("ClearIds", "Start", 0);
        var allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);

        //meshMaterialList = SharedMeshMaterialList.GetMeshMaterialList(allRenderers);

        int count = allRenderers.Length;
        for (int i = 0; i < count; i++)
        {
            MeshRenderer r = allRenderers[i];

            BoundsBox bb = r.GetComponent<BoundsBox>();
            if (bb != null)
            {
                continue;
            }

            foreach(var mat in r.sharedMaterials)
            {
                if(mat==null)
                {
                    Debug.Log($" mat==null renderer:{r}");
                    continue;
                }
                if(!SharedMaterials.Contains(mat))
                {
                    SharedMaterials.Add(mat);
                    if(mat.HasProperty("_BaseColorMap"))
                    {
                        var txt = mat.GetTexture("_BaseColorMap");
                        Textures.Add(txt);
                    }
                    

                    if (!MatRenderers.Contains(r))
                    {
                        MatRenderers.Add(r);
                    }
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
        var mats = MeshCombineHelper.GetMatFilters(allRenderers, false);
        CombinedMaterials=mats.Keys.ToList();
        string log2=$"CombineMat mats:{mats.Count},{(DateTime.Now - start).ToString()}";
        Debug.LogError(log2);

        return log+"\n"+log2;
    }

    public bool includeInactive = false;

    [ContextMenu("GetCombineMaterial")]
    public void GetCombineMaterial()
    {
        DateTime start = DateTime.Now;
        var allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(includeInactive);
        var mats = MeshCombineHelper.GetMatFilters(allRenderers, false);
        CombinedMaterials=mats.Keys.ToList();
        Debug.LogError($"GetCombineMaterial {(DateTime.Now - start).ToString()},mats:{mats.Count},count:{allRenderers.Length}");
    }


    [ContextMenu("SetCombineMaterial")]
    public void SetCombineMaterial()
    {
        DateTime start = DateTime.Now;
        var allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(includeInactive);
        var mats = MeshCombineHelper.GetMatFilters(allRenderers, true);
        CombinedMaterials=mats.Keys.ToList();
        Debug.LogError($"GetCombineMaterial {(DateTime.Now - start).ToString()},mats:{mats.Count},count:{allRenderers.Length}");
    }

    public GameObject SetTargetRoot;

    public Material SetTargetMaterial;

    public void DoChangeMaterial()
    {

    }

    public string RendererNames="";

    public void SetChildrenMaterial()
    {
        var allRenderers=GameObject.FindObjectsOfType<MeshRenderer>().ToList();
        var dic=new Dictionary<string,MeshRenderer>();
        RendererNames="";
        foreach(var r in allRenderers){
            if(dic.ContainsKey(r.name)){

            }
            else{
                dic.Add(r.name,r);
                RendererNames+=r.name+"\n";
            }
        }
        Debug.Log("Names:\n"+RendererNames);
        for(int i=0;i<SetTargetRoot.transform.childCount;i++)
        {
            var child=SetTargetRoot.transform.GetChild(i);
            var rRoot=allRenderers.Find(i=>i.name==child.name);
            Debug.Log($"SetChildrenMaterial[{i}] child:{child.name} rRoot:{rRoot}");
            if(rRoot==null){
                Debug.LogError("rRoot==null :"+child.name);
            }
            else{
                var chidrenRenderer=child.GetComponentsInChildren<MeshRenderer>(true);
                foreach(var r in chidrenRenderer)
                {
                    r.sharedMaterial=rRoot.sharedMaterial;
                }
            }
        }

        Debug.LogError($"SetChildrenMaterial allRenderers:{allRenderers.Count}");
    }

    public List<Material> MaterialLib=new List<Material>();
}
