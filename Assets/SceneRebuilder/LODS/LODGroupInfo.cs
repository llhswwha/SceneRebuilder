using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(LODGroup))]
public class LODGroupInfo : MonoBehaviour
{
    public LODGroup LODGroup;

    public List<LODInfo> LodInfos = new List<LODInfo>();

    public int lodCount;


    //public List<int> lodVertexCount=new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CheckLOD0Scenes()
    {
        if(this.scene!=null)
        {
            var scenes = gameObject.GetComponents<SubScene_Base>();
            foreach (var s in scenes)
            {
                if (this.scene != s)
                {
                    GameObject.DestroyImmediate(s);
                }
            }
        }
        else
        {
            var scenes = gameObject.GetComponents<SubScene_Base>();
            if (scenes.Length == 1)
            {
                this.scene = scenes[0];
            }
            else
            {
                Debug.LogWarning($"CheckLOD0Scenes {this.name} scenes:{scenes.Length}");
            }
        }
    }

    [ContextMenu("GetLODs")]
    public void GetLODs()
    {
        var allVertexCount=GetLODsInner();

        if (allVertexCount == 0)
        {
            Debug.LogError($"GetLODs Error 1 allVertexCount == 0");
            LODHelper.CreateLODs(this.gameObject);
            this.GetLODsInner();

            if (allVertexCount == 0)
            {
                Debug.LogError($"GetLODs Error 2 allVertexCount == 0");
            }
        }
    }

    private int GetLODsInner()
    {
        int allVertexCount = 0;

        LodInfos.Clear();
        //lodVertexCount.Clear();
        LODGroup = gameObject.GetComponent<LODGroup>();
        if (LODGroup == null)
        {
            Debug.LogError("LODGroupInfo.GetLODs LODGroup == null:" + this.name);
            return allVertexCount;
        }
        LOD[] lods = LODGroup.GetLODs();
        
        for (int i = 0; i < lods.Length; i++)
        {
            LOD lod = lods[i];
            LODInfo lodInfo = new LODInfo(lod, i);
            LodInfos.Add(lodInfo);
            int vc = 0;
            foreach (var r in lod.renderers)
            {
                if (r == null) continue;
                MeshFilter meshFilter = r.GetComponent<MeshFilter>();
                if (meshFilter == null)
                {
                    continue;
                }
                if (meshFilter.sharedMesh == null)
                {
                    continue;
                }
                vc += meshFilter.sharedMesh.vertexCount;
            }
            //lodVertexCount.Add(vc);
            lodInfo.vertextCount = vc;
            if (lodInfo.vertextCount == 0)
            {
                Debug.LogError($"lodInfo.vertextCount == 0 i:{i}");
            }
            allVertexCount += vc;
        }
        lodCount = LODGroup.lodCount;
        return allVertexCount;
    }

    [ContextMenu("SetLODs")]
    public void SetLODs()
    {
        LODGroup = gameObject.GetComponent<LODGroup>();
        List<LOD> lods = new List<LOD>();
        for (int i = 0; i < LodInfos.Count; i++)
        {
            LODInfo lodInfo = LodInfos[i];
            //lodInfo.screenRelativeTransitionHeight = ls[i];
            LOD lod = lodInfo.GetLOD();
            lods.Add(lod);
        }
        LODGroup.SetLODs(lods.ToArray());
    }

    public List<MeshRenderer> GetLODRenderers()
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        LODGroup = gameObject.GetComponent<LODGroup>();
        LOD[] lods = LODGroup.GetLODs();
        for (int i = 0; i < lods.Length; i++)
        {
            LOD lod = lods[i];
            foreach (MeshRenderer r in lod.renderers)
            {
                if (r == null) continue;
                if (!renderers.Contains(r))
                {
                    renderers.Add(r);
                }
            }
        }
        return renderers;
    }

    public List<MeshRenderer> GetLODRenderers(int i)
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        LODGroup = gameObject.GetComponent<LODGroup>();
        LOD[] lods = LODGroup.GetLODs();
        //for (int i = 0; i < lods.Length; i++)
        {
            LOD lod = lods[i];
            foreach (MeshRenderer r in lod.renderers)
            {
                if (r == null) continue;
                if (!renderers.Contains(r))
                {
                    renderers.Add(r);
                }
            }
        }
        return renderers;
    }

    public Renderer[] GetLastRenderers()
    {
        LODGroup = gameObject.GetComponent<LODGroup>();
        LOD[] lods = LODGroup.GetLODs();
        return lods[lods.Length - 1].renderers;
    }

    public List<MeshRenderer> GetNewRenderers()
    {
        List<MeshRenderer> newRenderers = new List<MeshRenderer>();
        MeshRenderer[] renderers = this.gameObject.GetComponentsInChildren<MeshRenderer>(true);
        List<MeshRenderer> lodRenderers = GetLODRenderers();
        foreach(var r in renderers)
        {
            if(!lodRenderers.Contains(r))
            {
                newRenderers.Add(r);
            }
        }
        return newRenderers;
    }

    [ContextMenu("AddLODN")]
    public void AddLODN()
    {
        List<MeshRenderer> newRenderers = GetNewRenderers();
        Debug.Log($"AddLODN newRenderers:{newRenderers.Count} lodCount:{lodCount}");
        if (newRenderers.Count > 0)
        {
            if(lodCount==2)
            {
                LODManager.Instance.AddLOD2(LODGroup, newRenderers, new float[] { 0.7f, 0.1f, 0.01f });
            }
            else if (lodCount == 3)
            {
                LODManager.Instance.AddLOD3(LODGroup, newRenderers, new float[] { 0.7f, 0.3f, 0.1f, 0.01f });
            }
        }

        GetLODs();
        SetMaterial();
    }

    [ContextMenu("AddBox")]
    public void AddBox()
    {
        var lastRenderers=GetLastRenderers();
        MeshRendererInfoList rendererInfos = new MeshRendererInfoList(lastRenderers);
        List<Renderer> boxRenderers = rendererInfos.GetBoundsRenderers();
        Debug.Log($"AddBox lastRenderers:{lastRenderers.Length} boxRenderers:{boxRenderers.Count} lodCount:{lodCount}");
        if (lastRenderers.Length > 0)
        {
            if (lodCount == 2)
            {
                LODManager.Instance.AddLOD2(LODGroup, boxRenderers, new float[] { 0.7f, 0.1f, 0.01f });
            }
            else if (lodCount == 3)
            {
                LODManager.Instance.AddLOD3(LODGroup, boxRenderers, new float[] { 0.7f, 0.3f,0.1f, 0.01f });
            }
        }

        GetLODs();
        SetMaterial();
    }

    public float[] ls = new float[] { 0.6f, 0.2f,0.1f,0.01f};

    [ContextMenu("SetDefulatLOD")]
    public void SetDefulatLOD()
    {
        LODGroup.SetLODs(LODHelper.CreateLODs(ls));
    }

    [ContextMenu("CreateLOD")]
    public void CreateLOD()
    {
        LODHelper.CreateLODs(gameObject, ls);
    }

    //public static LOD[] CreateLODs(float[] ls)
    //{
    //    LOD[] lods = new LOD[ls.Length];
    //    for(int i=0;i<ls.Length;i++)
    //    {
    //        LOD lod = new LOD();
    //        lod.screenRelativeTransitionHeight = ls[i];
    //        lod.fadeTransitionWidth = 0;
    //        lods[i] = lod;
    //    }
    //    return lods;
    //}

    //public static LODGroup CreateLODs(GameObject obj,float[] ls)
    //{
    //    LODGroup lodGroup = obj.GetComponent<LODGroup>();
    //    if (lodGroup == null)
    //    {
    //        lodGroup = obj.AddComponent<LODGroup>();
    //    }
    //    lodGroup.SetLODs(CreateLODs(ls));
    //    return lodGroup;
    //}

    public SubScene_Base scene;

#if UNITY_EDITOR
    public void EditorCreateScene()
    {
        if (LODGroup == null)
        {
            LODGroup = gameObject.GetComponent<LODGroup>();
        }
        if (LODGroup == null)
        {
            Debug.LogError("LODGroupInfo.EditorCreateScene LODGroup == null:"+this);
            return;
        }
        if (scene != null)
        {
            GameObject.DestroyImmediate(scene);
        }
        scene =LODHelper.SaveLOD0(null, this.LODGroup);
    }



    public void EditorLoadScene()
    {
        IdDictionary.InitInfos();
        GetScene();
        if (scene == null) return;
        scene.EditorLoadScene();
        SetLOD0FromScene();

        GetLODs();
    }
#endif

    public void ClearOtherScenes()
    {
        var scenes = this.GetComponents<SubScene_Base>();
        foreach (var s in scenes)
        {
            if (s != this.scene)
            {
                GameObject.DestroyImmediate(s);
            }
        }
    }

    public void UnloadScene()
    {
        LODHelper.LOD1ToLOD0(this.LODGroup);
        GetScene();
        scene.UnLoadGosM();
    }

    public bool IsSceneCreatable()
    {
        if (this.gameObject == false) return false;
        GetScene();
        if (scene == null) return true;
        return scene.IsLoaded==true;
    }

    public void GetScene()
    {
        if (scene == null)
        {
            scene = gameObject.GetComponent<SubScene_Base>();
        }
    }

    public void SetLOD0FromScene()
    {
        var lods = LODGroup.GetLODs();
        var sceneRenderers = scene.GetSceneRenderers().ToArray();
        Debug.Log($"SetLOD0FromScene sceneRenderers:{sceneRenderers.Length} scene:{scene.sceneName} [arg:{scene.GetSceneArg()}]");
        lods[0].renderers = sceneRenderers;
        foreach(var r in sceneRenderers)
        {
            if(r.transform.parent==this.transform.parent)
            {
                r.transform.SetParent(this.transform);
            }
        }
        LODGroup.SetLODs(lods);
    }

    public void LoadScene()
    {
        //GetScene();

        if (scene == null) return;
        if (scene.IsLoaded) return;

        IdDictionary.InitInfos();

        scene.LoadSceneAsync((b, s) =>
        {
            SetLOD0FromScene();
        });
    }

    public void SetMaterial()
    {
        GetLODs();
        var mats = LodInfos[0].GetMats();
        for(int i = 1; i < LodInfos.Count; i++)
        {
            LodInfos[i].SetMats(mats);
        }
    }

    public void Rename()
    {
        GetLODs();
        for (int i = 0; i < LodInfos.Count; i++)
        {
            LodInfos[i].Rename($"{this.name}_LOD{i}");
        }
    }

    public static LODGroupInfo Init(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("LODGroupInfo.Init go == null");
            return null;
        }
        LODGroupInfo info = go.GetComponent<LODGroupInfo>();
        if (info == null)
        {
            info=go.AddComponent<LODGroupInfo>();
            info.GetLODs();
        }
        info.GetScene();
        return info;
    }

    internal void AddRenderers(int v, List<Transform> pipeWelds)
    {
        var lods = LODGroup.GetLODs();
        List<Renderer> renderers = new List<Renderer>(lods[v].renderers);
        foreach(var item in pipeWelds)
        {
            Renderer renderer = item.GetComponent<Renderer>();
            renderers.Add(renderer);
        }
        lods[v].renderers = renderers.ToArray();
        LODGroup.SetLODs(lods);
    }
}
