using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[RequireComponent(typeof(LODGroup))]
public class LODGroupInfo : MonoBehaviour
{
    public LODGroup LODGroup;

    public List<LODInfo> LodInfos = new List<LODInfo>();

    public List<Renderer> LOD0Renderers = new List<Renderer>();

    public int lodCount;


    //public List<int> lodVertexCount=new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        LODManager.Instance.AddLODGroup(LODGroup);
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
    public void GetLODs(bool isClear = false)
    {
        var allVertexCount=GetLODsInner();

        if (allVertexCount == 0)
        {
            Debug.LogError($"GetLODs Error 1 allVertexCount == 0 go:{this.name} path:{this.transform.GetPath()}");
            LODHelper.CreateLODs(this.gameObject,isClear);
            this.GetLODsInner();

            if (allVertexCount == 0)
            {
                Debug.LogError($"GetLODs Error 2 allVertexCount == 0 go:{this.name} path:{this.transform.GetPath()}");
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
                Debug.LogError($"GetLODsInner lodInfo.vertextCount == 0 i:{i} go:{this.name} path:{this.transform.GetPath()}");
            }
            allVertexCount += vc;
        }
        lodCount = LODGroup.lodCount;
        return allVertexCount;
    }

    [ContextMenu("NewLODs")]
    public void NewLODs(int lodCount)
    {
        LODGroup group = LODHelper.CreateEmptyLODs(gameObject, GetLevels(lodCount));
        LOD[] lods = group.GetLODs();
        lods[0].renderers = this.GetRenderers();
        group.SetLODs(lods);

        GetLODs();
    }

    [ContextMenu("NewLODs")]
    public void NewLODs(string key)
    {
        LODGroup group = LODHelper.CreateEmptyLODs(gameObject, GetLevels(1));
        LOD[] lods = group.GetLODs();
        lods[0].renderers = this.GetRenderers();
        List<MeshRenderer> lod0Renderers = this.GetRenderers().ToList();
        for (int i = 0; i < lod0Renderers.Count; i++)
        {
            MeshRenderer renderer = lod0Renderers[i];
            if (renderer.name.Contains(key))
            {
                lod0Renderers.RemoveAt(i);
                i--;
            }
        }
        lods[1].renderers = lod0Renderers.ToArray();
        group.SetLODs(lods);
        GetLODs();
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

    public MeshRenderer[] GetRenderers()
    {
        return this.GetComponentsInChildren<MeshRenderer>(true);
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

    public float[] GetLevels(int lodCount)
    {
        if (lodCount == 0)
        {
            return new float[] { ls[ls.Length - 1] };
        }
        else if(lodCount == 1)
        {
            return new float[] { ls[0], ls[ls.Length - 1] };
        }
        else if (lodCount == 2)
        {
            return new float[] { ls[0], ls[1], ls[ls.Length - 1] };
        }
        else
        {
            return ls;
        }
    }

    [ContextMenu("SetDefulatLOD")]
    public void SetDefulatLOD()
    {
        LODGroup.SetLODs(LODHelper.CreateLODs(ls));
    }

    [ContextMenu("CreateLOD")]
    public LODGroup CreateLOD()
    {
        return LODHelper.CreateEmptyLODs(gameObject, ls);
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

        SetRenderersLODInfo();

        RendererId rid = RendererId.GetRId(this.gameObject);
        rid.RefreshParentId();
        rid.UpdateChildrenId(false);
        scene =LODHelper.SaveLOD0(null, this);
    }

    [ContextMenu("SetRenderersLODInfo")]
    public void SetRenderersLODInfo()
    {
        LODHelper.SetRenderersLODInfo(LODGroup, null);
    }


    public void EditorLoadScene()
    {
        IdDictionary.InitInfos();
        GetScene();
        if (scene == null)
        {
            return;
        }
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
        Material mat = null;
        MeshRenderer[] mrs = this.GetComponentsInChildren<MeshRenderer>(true);
        foreach(var mr in mrs)
        {
            if (mr.sharedMaterial != null)
            {
                mat = mr.sharedMaterial;
                break;
            }
        }
        var lods = LODGroup.GetLODs();
        var sceneRenderers = scene.GetSceneRenderers().ToArray();
        if (sceneRenderers.Length == 0)
        {
            Debug.LogError($"SetLOD0FromScene[{this.name}] Length == 0 sceneRenderers:{sceneRenderers.Length} scene:{scene.sceneName} [arg:{scene.GetSceneArg()}]");
            return;
        }
        bool isVisible = true;
        foreach(var renderer in sceneRenderers)
        {
            if (renderer.gameObject.activeInHierarchy == false)
            {
                isVisible = false;
            }
        }

        //Debug.LogError($"SetLOD0FromScene[{this.name}] isVisible=={isVisible} sceneRenderers:{sceneRenderers.Length} scene:{scene.sceneName} [arg:{scene.GetSceneArg()}]");
        //Debug.Log($"SetLOD0FromScene sceneRenderers:{sceneRenderers.Length} scene:{scene.sceneName} [arg:{scene.GetSceneArg()}]");

        List<Renderer> lod0Renderers = new List<Renderer>(sceneRenderers);
        foreach(var render0 in this.LOD0Renderers)
        {
            if (render0 == null) continue;
            lod0Renderers.Add(render0);
        }

        lods[0].renderers = lod0Renderers.ToArray(); ;
        foreach(var r in sceneRenderers)
        {
            if (r.sharedMaterial == null)
            {
                r.sharedMaterial = mat;
                Debug.LogError($"SetLOD0FromScene r.sharedMaterial == null renderer:{r}");
            }
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

#if UNITY_EDITOR
    public void OnDestroy()
    {
        Debug.Log($"LODGroupInfo.OnDestroy path:{transform.GetPath()}");
    }
#endif

}
