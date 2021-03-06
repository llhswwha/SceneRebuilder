using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class SubScene_Base : SubSceneArgComponent
{
    //public void OnEnable()
    //{
        
    //}

    //public void OnDisable()
    //{
        
    //}

    public virtual void DestroyScene()
    {
        GameObject.DestroyImmediate(this);
    }

    public SubScene_Base LinkedScene;

    //public SubSceneArg sceneArg;

    public Transform sceneParent;

    public Transform GetSceneParent()
    {
        if (sceneParent != null)
        {
            return sceneParent;
        }
        return this.transform;
    }

    public bool IsLoaded = false;

    public virtual bool CanUnload()
    {
        return IsLoaded == true && IsLoading == false && this.gameObject.isStatic==false;
    }

    public virtual bool CanLoad()
    {
        return IsLoaded == false && IsLoading == false;
    }

    public bool GetIsLoaded()
    {
        if (scene != null)
        {
            return scene.isLoaded;
        }
        return IsLoaded;
    }

    public List<GameObject> gos
    {
        get
        {
            if (sceneArg == null)
            {
                return new List<GameObject>();
            }
            else
            {
                return sceneArg.objs;
            }
        }
    }

    public List<GameObject> GetObjects()
    {
        if (this.IsLoaded == false)
        {
            return new List<GameObject>();
        }
        if (sceneArg == null)
        {
            return new List<GameObject>();
        }
        else
        {
            return sceneArg.objs;
        }
    }

    //public List<MeshRenderer> GetRenderers()
    //{
    //    List<MeshRenderer> renderers = new List<MeshRenderer>();
    //    foreach(var go in gos)
    //    {
    //        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
    //        if (renderer != null)
    //        {
    //            renderers.Add(renderer);
    //        }
    //    }
    //    return renderers;
    //}

    public bool HaveGos()
    {
        bool r = false;
        if(gos==null)return false;
        foreach(var g in gos)
        {
            if (g != null)
            {
                r = true;
            }
        }
        return r;
    }

    public bool IsLoading = false;

    //public SubSceneType sceneType;

    //public SceneContentType contentType;

    //public string sceneName = "";

    //public string scenePath;

    public void SetPath(string path)
    {
        if (sceneArg == null)
        {
            sceneArg = new SubSceneArg();
        }
        sceneArg.path = path;
        GetSceneName();
    }

    public Scene scene;

    public Bounds bounds;

    public Vector3 center;

    public float radius;

    //public List<GameObject> gos = new List<GameObject>();

    public virtual void SetObjects(List<GameObject> goList)
    {
        //gos = goList;
        if (sceneArg == null)
        {
            sceneArg = new SubSceneArg();
        }

        if (sceneArg != null)
        {
            sceneArg.objs = goList;
        }

        //SetRendererParent();
    }

    public int childCount;

    public int rendererCount;

    public float vertexCount = 0;


    public bool IsSetParent = true;

    public bool IsAutoLoad = false;

    public event Action<float,SubScene_Base> ProgressChanged;

    protected void OnProgressChanged(float progress)
    {
        this.loadProgress = progress;
        if (ProgressChanged != null)
        {
            ProgressChanged(progress,this);
        }
    }

    public event Action<SubScene_Base> LoadFinished;

    protected void OnLoadedFinished()
    {

        OnProgressChanged(1);

        if (LoadFinished != null)
        {
            LoadFinished(this);
        }
    }

    public event Action<SubScene_Base> UnLoadFinished;

    protected void OnUnLoadedFinished()
    {

        //OnProgressChanged(1);

        if (UnLoadFinished != null)
        {
            UnLoadFinished(this);
        }
    }

    private float boundsVolume = 0;

    public float GetBoundsVolume()
    {
        if (boundsVolume == 0)
        {
            Vector3 size = bounds.size;
            boundsVolume = size.x * size.y * size.z;
        }
        return boundsVolume;
    }

    internal string GetSceneInfo()
    {
        //return $"r:{rendererCount}\tv:{vertexCount:F1}w\t[{GetSceneName()}]  ";
        string scenePath = "";
        BuildingController bc= this.GetComponentInParent<BuildingController>();
        BuildingModelInfo buildingModelInfo = this.GetComponentInParent<BuildingModelInfo>();
        ModelAreaTree tree = this.GetComponentInParent<ModelAreaTree>();
        //Vector3 size = bounds.size;
        if(bc==null||buildingModelInfo==null)
        {
            
        }
        if(bc!=null&& buildingModelInfo != null)
        {
            string buildingName = bc.name;
            if (buildingModelInfo.gameObject != bc.gameObject)
            {
                buildingName += ">" + buildingModelInfo.name;
            }
            scenePath = buildingName;
        }
        else
        {
            Debug.LogError($"Exception.SubScene_Base.GetSceneInfo.Name:{transform.name} bc:{bc} buildingModelInfo:{buildingModelInfo} path:{this.transform.GetPath()}");
            //return "";
        }

        if (tree != null)
        {
            string treeName = tree.name.Replace(buildingModelInfo.name, "");
            scenePath = $"({scenePath}>{treeName})";
        }

        return $"({scenePath})[{AngleToCam:F0},{DisToCam:F0}][({GetBoundsVolume():F0}){bounds.size}]{GetSceneName()} r:{rendererCount} v:{vertexCount:F0}w ";
    }

    internal string GetSceneNameEx()
    {
        return $"[{contentType}]{GetSceneName()} r:{rendererCount} v:{vertexCount:F0}w ";
    }

    //public SceneLoadArg GetSceneArg()
    //{
    //    SceneLoadArg arg=new SceneLoadArg();
    //    arg.name=GetSceneName();
    //    arg.path=sceneArg.path;
    //    arg.index=sceneArg.index;
    //    return arg;
    //}

    //public string GetSceneName()
    //{
    //    if (string.IsNullOrEmpty(sceneName))
    //    {
    //        try
    //        {
    //            if (sceneArg == null) return "";
    //            if (string.IsNullOrEmpty(sceneArg.path)) return "";
    //            string[] parts = sceneArg.path.Split(new char[] { '.', '\\', '/' });
    //            if (parts.Length < 2) return "";
    //            sceneName = parts[parts.Length - 2];
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.LogError($"GetSceneName  obj:{this},path:{sceneArg.path},Exception:{ex}");
    //        }
            
    //    }
    //    return sceneName;
    //}


    void Start()
    {
        if (IsAutoLoad)
        {
            LoadSceneAsync(null);
        }
    }

    public GameObject boundsGo;

    protected virtual string BoundsName
    {
        get
        {
            return $"{this.name}_Bounds_{contentType}";
            //return "123";
        }
    }

    public int cubePrefabId = 0;

    internal void ShowBounds(Transform boxP)
    {
#if UNITY_EDITOR
        //Debug.Log("ShowBounds:"+BoundsName+"|"+ contentType);
        //DestroyBoundsBox();
        //boundsGo = AreaTreeHelper.CreateBoundsCube(bounds, BoundsName, transform);

        if (boundsGo == null)
        {
            boundsGo = AreaTreeHelper.CreateBoundsCube(bounds, BoundsName, boxP, cubePrefabId);
        }
        else
        {
            boundsGo.SetActive(true);
        }
#endif
    }

    public Vector3[] GetBoundPoints()
    {
        var min = bounds.min;
        var max = bounds.max;
        Vector3 p0 = new Vector3(min.x, min.y, min.z);
        Vector3 p1 = new Vector3(min.x, min.y, max.z);
        Vector3 p2 = new Vector3(min.x, max.y, min.z);
        Vector3 p3 = new Vector3(max.x, min.y, min.z);
        Vector3 p4 = new Vector3(min.x, max.y, max.z);
        Vector3 p5 = new Vector3(max.x, min.y, max.z);
        Vector3 p6 = new Vector3(max.x, max.y, min.z);
        Vector3 p7 = new Vector3(max.x, max.y, max.z);
        return new Vector3[9] { bounds.center,p0, p1, p2, p3, p4, p5, p6, p7 };
    }

    public float GetCameraAngle(Transform camera)
    {
        
        Vector3[] ps = GetBoundPoints();
        float minAngle = 360;
        float maxAngle = 0;
        for (int i = 0; i < ps.Length; i++)
        {
            Vector3 p = ps[i];
            Vector3 dir = p - camera.position;
            float angle = Vector3.Angle(camera.forward, dir);
            if (angle < minAngle)
            {
                minAngle = angle;
            }
            if (angle > maxAngle)
            {
                maxAngle = angle;
            }
            //Debug.Log($"GetCameraAngle[{i}] p:{p} dir:{dir} angle:{angle} minAngle:{minAngle} maxAngle:{maxAngle}  ");
        }
        //return maxAngle; 
        return minAngle;
    }

    [ContextMenu("ShowCameraAngle")]
    internal void ShowCameraAngle()
    {
        GetCameraAngle(Camera.main.transform);
    }

    [ContextMenu("ShowBoundsPoints")]
    internal void ShowBoundsPoints()
    {
        ShowBounds();
        Transform boxP = boundsGo.transform;

        TransformHelper.ShowPoint(bounds.center, 0.05f, boxP).name="center";
        var min = bounds.min;
        var max = bounds.max;
        Vector3 p0 = new Vector3(min.x, min.y, min.z);
        Vector3 p1 = new Vector3(min.x, min.y, max.z);
        Vector3 p2 = new Vector3(min.x, max.y, min.z);
        Vector3 p3 = new Vector3(max.x, min.y, min.z);
        Vector3 p4 = new Vector3(min.x, max.y, max.z);
        Vector3 p5 = new Vector3(max.x, min.y, max.z);
        Vector3 p6 = new Vector3(max.x, max.y, min.z);
        Vector3 p7 = new Vector3(max.x, max.y, max.z);
        TransformHelper.ShowPoint(p0, 0.05f, boxP).name = "p0";
        TransformHelper.ShowPoint(p1, 0.05f, boxP).name = "p1";
        TransformHelper.ShowPoint(p2, 0.05f, boxP).name = "p2";
        TransformHelper.ShowPoint(p3, 0.05f, boxP).name = "p3";
        TransformHelper.ShowPoint(p4, 0.05f, boxP).name = "p4";
        TransformHelper.ShowPoint(p5, 0.05f, boxP).name = "p5";
        TransformHelper.ShowPoint(p6, 0.05f, boxP).name = "p6";
        TransformHelper.ShowPoint(p7, 0.05f, boxP).name = "p7";
    }

    [ContextMenu("ShowBounds")]
    public void ShowBounds()
    {
        ShowBounds(this.transform);
    }

    [ContextMenu("HideBoundsBox")]
    public void HideBoundsBox()
    {
        if (boundsGo)
        {
            boundsGo.SetActive(false);
        }
    }

    [ContextMenu("DestroyBoundsBox")]
    public virtual void DestroyBoundsBox()
    {
        if (boundsGo)
        {
            GameObject.DestroyImmediate(boundsGo);
        }
    }

    [ContextMenu("DestoryChildren")]
    public void DestoryChildren()
    {
        var children = SubSceneHelper.GetChildrenGos(GetSceneParent());
        foreach (var go in children)
        {
            if (go == null) continue;
            GameObject.DestroyImmediate(go);
        }
        gos.Clear();

        IsLoaded = false;
        IsLoading = false;
    }

    internal List<ModelAreaTree> GetTrees()
    {
        List<ModelAreaTree> ts = new List<ModelAreaTree>();
        foreach(var go in gos)
        {
            ts.AddRange(go.GetComponentsInChildren<ModelAreaTree>());
        }
        return ts;
    }

    public float DisToCam;

    public float AngleToCam;

    public int LifeTimeMode = -1;//0:Load,1:Unload;

    public float LifeStartTime = 0;

    public float LifeTimeSpane = 0;

    public void ClearLifeTime()
    {
        //LifeTimeMode = -1;
        LifeStartTime = 0;
        LifeTimeSpane = 0;
    }

    public void SetLifeTime(int mode)
    {
        if (LifeTimeMode != mode)
        {
            ClearLifeTime();
        }
        LifeTimeMode = mode;
        if (LifeStartTime == 0)
        {
            LifeStartTime = Time.time;
            LifeTimeSpane = 0;
        }
        else
        {
            LifeTimeSpane = Time.time - LifeStartTime;
        }
    }

    public virtual void HideObjects()
    {
        if (IsVisible==false) return;
        IsVisible = false;
        if (gos != null)
            foreach (var go in gos)
            {
                if (go == null) continue;
                SubScene_Base scene = go.GetComponent<SubScene_Base>();
                if (scene != null)
                {
                    continue;
                }
                go.SetActive(false);
            }
    }

    //public bool IsFirst

    public bool IsVisible = true;
    public virtual void ShowObjects()
    {
        if (IsVisible) return;
        IsVisible = true;
        if(gos!=null)
            foreach (var go in gos)
            {
                if (go == null) continue;
                SubScene_Base scene = go.GetComponent<SubScene_Base>();
                if (scene != null)
                {
                    continue;
                }
                go.SetActive(true);
            }
    }

    [ContextMenu("UnLoadGosM")]
    public virtual int UnLoadGosM()
    {
        unloadCount++;
        unloadTime = DateTime.Now;
        //if (gos != null)
        //{
        //    foreach (var go in gos)
        //    {
        //        if (go == null) continue;
        //        GameObject.DestroyImmediate(go);
        //    }
        //    gos.Clear();
        //}

        int count = 0;

        if (sceneArg != null && sceneArg.objs != null)
        {
            foreach (var obj in sceneArg.objs)
            {
                if (obj == null) continue;
                GameObject.DestroyImmediate(obj);
                count++;
            }
            //sceneArg.objs = null;
            sceneArg.objs.Clear();
        }

        IsLoaded = false;
        IsLoading = false;

        OnUnLoadedFinished();
        return count;
    }

    private DateTime unloadTime;
    private int unloadCount = 0;

    [ContextMenu("UnLoadGos")]
    public virtual void UnLoadGos()
    {
        unloadCount++;
        unloadTime = DateTime.Now;
        foreach (var go in gos)
        {
            if (go == null) continue;
            GameObject.Destroy(go);
        }
        gos.Clear();

        if(sceneArg!=null&& sceneArg.objs != null)
        {
            foreach (var obj in sceneArg.objs)
            {
                if (obj == null) continue;
                GameObject.Destroy(obj);
            }
            sceneArg.objs = null;
        }


        IsLoaded = false;
        IsLoading = false;

        OnUnLoadedFinished();
    }

    //[ContextMenu("ReLoadScene")]
    //public void ReLoadScene()
    //{
    //    UnLoadGosM();
    //    LoadScene();
    //}

    //[ContextMenu("LoadScene")]
    //public void LoadScene()
    //{
    //    var gs= EditorHelper.LoadScene(GetSceneArg(), IsSetParent ? GetSceneParent() : null).ToList();
    //    SetObjects(gs);
    //    SetRendererParent();
    //    IsLoaded = true;
    //}

    //[ContextMenu("TestLoadSceneAsync")]
    //public void TestLoadSceneAsync()
    //{
    //    LoadSceneAsync();
    //    LoadSceneAsync();
    //}

    public bool IsLoadedOrLoading()
    {
        return IsLoading || IsLoaded;
    }

    public IEnumerator LoadSceneAsyncCoroutine(Action<bool,SubScene_Base> callback)
    {
        //Debug.Log($"LoadSceneAsyncCoroutine scene:{TransformHelper.GetPath(this.transform)} ");
        if (IsLoadedOrLoading())
        {
            Debug.LogWarning($"[SubScene_Base.LoadSceneAsyncCoroutine] scene:{GetSceneName()}, IsLoading:{IsLoading} || IsLoaded:{IsLoaded} path:{sceneArg.path}");
            if (callback != null)
            {
                callback(false,this);
            }
            yield return null;
        }
        else
        {
            DateTime start = DateTime.Now;

            IsLoading = true;
            OnProgressChanged(0);
            yield return EditorHelper.LoadSceneAsync(this,GetSceneArg(), progress =>
            {
                OnProgressChanged(progress);
                //Debug.Log("progress:" + progress);
            }, s =>
            {
                if (IsSetParent)
                    GetSceneObjects();
                IsLoaded = true;
                IsLoading = false;

                if (callback != null)
                {
                    callback(true,this);
                }


                //WriteLog($"Load name:{GetSceneName()},time:{(DateTime.Now - start).ToString()},progress:{loadProgress}");
                //WriteLog($"Load[ {GetSceneName()} ]: {(DateTime.Now - start).ToString()}");
                WriteLog($"Load name:{GetSceneName()}",$"{(DateTime.Now - start).ToString()}|r:{rendererCount}|v:{GetVertexCountText()}w");
                OnLoadedFinished();
            }, IsSetParent);
        }
    }

    public string GetVertexCountText()
    {
        if(vertexCount<0.1){
            return vertexCount.ToString("F3");
        }
        else if(vertexCount<1){
            return vertexCount.ToString("F2");
        }
        else if(vertexCount<10){
            return vertexCount.ToString("F1");
        }
        else
        {
            return vertexCount.ToString("F0");
        }
    }

    public string Log = "";

    private void WriteLog(string tag,string log)
    {
        Log = log;
        //Debug.Log($"[{tag}]{log}");
    }

    [ContextMenu("LoadSceneAsync")]
    public void TestLoadSceneAsync()
    {
        SubSceneManager.Instance.StartCoroutine(LoadSceneAsyncCoroutine(null));
    }

    //[ContextMenu("LoadSceneAsync")]
    public void LoadSceneAsync(Action<bool,SubScene_Base> callback)
    {
        if (IsLoading || IsLoaded)
        {
            Debug.LogWarning($"[SubScene_Base.LoadSceneAsync] scene:{GetSceneName()}, IsLoading:{IsLoading} || IsLoaded:{IsLoaded} index:{sceneArg.index} path:{sceneArg.path} ");
            if (callback != null)
            {
                callback(false,this);
            }
            return;
        }

        //SubSceneManager.Instance.StartCoroutine(EditorHelper.LoadSceneAsync(GetSceneName(), progress=>
        //{
        //    loadProgress = progress;
        //    Debug.Log("progress:"+ progress);
        //},s=>
        //{
        //    if(IsSetParent)
        //        GetSceneObjects();
        //    IsLoaded = true;
        //    IsLoading = false;
        //}, IsSetParent)
        //);

        if (this.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning("[SubScene_Base.LoadSceneAsync]this.gameObject.activeInHierarchy == false :" + name);
            //if (callback != null)
            //{
            //    callback(false);
            //}
            //return;
            HideBoundsBox();
            this.gameObject.SetActive(true);
        }
        //if (this.gameObject.activeInHierarchy == true)
        //{
            SubSceneManager.Instance.StartCoroutine(LoadSceneAsyncCoroutine(callback));
        //}
        //else{
        //    if(callback!=null){
        //        callback(false,this);
        //    }
        //     Debug.LogError("[SubScene_Base.LoadSceneAsync]this.gameObject.activeInHierarchy == false :" + name);
        //}
    }
    [ContextMenu("UnLoadSceneAsync2")]
    public void TestUnLoadSceneAsync()
    {
        UnLoadSceneAsync();
        UnLoadSceneAsync();
    }

    [ContextMenu("UnLoadSceneAsync")]
    public void UnLoadSceneAsync(bool isUnload=false)
    {
        if (IsLoaded == false)
        {
            //Debug.LogWarning("IsLoaded==false :" + GetSceneName());
            return;
        }
        //DestoryGosImmediate();
        UnLoadGos();
        SceneLoadArg loadArg = GetSceneArg();
        loadArg.isUnload = isUnload;
        SubSceneManager.Instance.StartCoroutine(EditorHelper.UnLoadSceneAsync(loadArg, progress =>
        {
            loadProgress = progress;
            Debug.Log("progress:" + progress);
        }, () =>
        {
            //GetSceneObjects();
            IsLoaded = false;
        }));
    }

    public IEnumerator UnLoadSceneAsync_Coroutine(bool isUnload = false)
    {
        if (IsLoaded == true)
        {
            //DestoryGosImmediate();
            UnLoadGos();
            SceneLoadArg loadArg = GetSceneArg();
            loadArg.isUnload = isUnload;
            yield return EditorHelper.UnLoadSceneAsync(loadArg, progress =>
            {
                loadProgress = progress;
                Debug.Log("progress:" + progress);
            }, () =>
            {
                //GetSceneObjects();
                IsLoaded = false;
            });
        }
    }

    public float loadProgress;



    [ContextMenu("GetSceneObjects")]
    public virtual void GetSceneObjects()
    {
        //DestroyBoundsBox();
        HideBoundsBox();

        var gs = EditorHelper.GetSceneObjects(GetSceneArg(), GetSceneParent()).ToList();
        SetObjects(gs);
        SetRendererParent();
        InitVisible();

        AreaTreeNode node=this.transform.parent.GetComponent<AreaTreeNode>();
        if(node!=null)
            node.LoadRenderers(this.gameObject);
    }

    [ContextMenu("UpdateRidParent")]
    public void UpdateRidParent()
    {
        RendererId thisRid = RendererId.GetRId(this);
        thisRid.UpdateParent();
        thisRid.UpdateChildrenId(true);

        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    var child = transform.GetChild(i);
        //    RendererId rid = RendererId.GetRId(child);
        //    rid.UpdateParent();
        //}
    }

     [ContextMenu("SetRendererParent")]
    public virtual void SetRendererParent()
    {
        RendererId[] rIds=this.GetComponentsInChildren<RendererId>(true);
        foreach(var rI in rIds){
            rI.SetParent();
            IdDictionary.SetId(rI);
            AreaTreeNodeShowManager.Instance.MoveRenderer(rI);
        }


    }

     [ContextMenu("SetRendererParent_Ids")]
    private void SetRendererParentEx()
    {
        IdDictionary.InitInfos();
        SetRendererParent();
    }

    public int GetSceneObjectCount()
    {
        int i = 0;
        foreach(var g in gos)
        {
            if (g != null) i++;
        }
        return i;
    }

    //public void Update()
    //{
    //    //if (IsLoaded == true && gos.Count == 0 && IsSetParent)
    //    //{
    //    //    GetSceneObjects();
    //    //}
    //}

    public void CheckGetSceneObjects()
    {
        //Debug.LogError($"CheckGetSceneObjects {this.name} IsLoaded:{IsLoaded},count:{gos.Count},IsSetParent:{IsSetParent}");
        if (IsLoaded == true && gos.Count == 0 && IsSetParent)
        {
            GetSceneObjects();
            Debug.LogError($"CheckGetSceneObjects {this.name} UnLoadSceneAsync1!!!");

            //if (this.gameObject.activeSelf == false)
            //{
            //    HideBoundsBox();
            //    this.gameObject.SetActive(true);
            //}

            StartCoroutine(EditorHelper.UnLoadSceneAsync(GetSceneArg(), null,null));
        }
        else
        {
            //Debug.LogError($"CheckGetSceneObjects {this.name} UnLoadSceneAsync2!!!");
            //this.UnLoadSceneAsync();
        }
    }

#if UNITY_EDITOR

    [ContextMenu("EditorCreateScene")]
    public virtual void EditorCreateScene(bool isOnlyChildren, GameObject dirGo=null, SceneContentType contentType= SceneContentType.Single)
    {

        SubSceneManager subSceneManager = SubSceneManager.Instance;
        string path = subSceneManager.GetScenePath($"({transform.parent.name}){gameObject.name}[{RendererId.GetInsId(gameObject)}]", contentType, dirGo);
        if (IsLoaded == false)
        {
            Debug.LogWarning($"EditorCreateScene IsLoaded==false name:{this.name} path:{path}");
            return;
        }
        //if (sceneArg == null)
        //{
        //    sceneArg = new SubSceneArg();
        //}
        //if (sceneArg.objs.Length == 0)
        //{

        //}
        SubSceneHelper.EditorCreateScene(this.gameObject, path, subSceneManager.IsOverride, true, isOnlyChildren,this);
    }

    [ContextMenu("EditorReLoadScene")]
    public void EditorReLoadScene()
    {
        UnLoadGosM();
        EditorLoadSceneEx();
    }

    [ContextMenu("EditorSelectObjects")]
    public void EditorSelectObjects()
    {
        EditorHelper.SelectObjects(sceneArg.objs);
    }

    [ContextMenu("EditorLoadScene")]
    public virtual void EditorLoadScene()
    {
        EditorHelper.UnpackPrefab(this.gameObject);

        IsLoaded = true;
        if (boundsGo)
        {
            GameObject.DestroyImmediate(boundsGo);
        }
        var gs = EditorHelper.EditorLoadScene(scene, sceneArg.path, IsSetParent ? GetSceneParent() : null).ToList();
        SetObjects(gs);
        InitIdDict();
        SetRendererParent();
    }

    [ContextMenu("EditorLoadSceneEx")]
    public void EditorLoadSceneEx()
    {
        if (IsLoaded == true)
        {
            Debug.LogWarning("EditorLoadScene IsLoaded==true :" + GetSceneName());
            return;
        }
        EditorLoadScene();
    }

    [ContextMenu("EditorLoadLinkedScene")]
    public void EditorLoadLinkedScene()
    {
        if (LinkedScene != null)
        {
            LinkedScene.EditorLoadSceneEx();
        }
        else
        {
            Debug.LogError($"SubScene_Base.EditorLoadLinkedScene LinkedScene==null scene:{this.name}");
        }
    }


    private void LoadTreeRenderers()
    {
        var trees = GetTrees();
        foreach(var tree in trees)
        {
            tree.LoadRenderers();
        }
    }


    internal void InitIdDict()
    {
        //Debug.Log($"SubScene_Base.InitIdDict name:{this.name} type:{contentType} linked:{LinkedScene}");
        IdDictionary.InitGos(gos, sceneName);
        if (LinkedScene != null)
        {
            if (this.contentType == SceneContentType.Tree)
            {
                if (LinkedScene.IsLoaded)
                {
                    this.LoadTreeRenderers();
                }
                else
                {
                    Debug.LogError($"SubScene_Base.InitIdDict LinkedScene.IsLoaded==false scene:{this.name}");
                }
            }
            else if (this.contentType == SceneContentType.Part)
            {
                if (LinkedScene.IsLoaded)
                {
                    LinkedScene.LoadTreeRenderers();
                }
                else
                {
                    Debug.LogError($"SubScene_Base.InitIdDict LinkedScene.IsLoaded==false scene:{this.name}");
                }
            }
            else if (this.contentType == SceneContentType.TreeAndPart)
            {
                LoadTreeRenderers();
            }
            else //TreeNode
            {
               
                // if (LinkedScene.IsLoaded)
                // {
                //     LinkedScene.LoadTreeRenderers();
                //     this.LoadTreeRenderers();
                // }
                // else
                // {
                //     Debug.LogError($"SubScene_Base.InitIdDict LinkedScene.IsLoaded==false scene:{this.name} linkedScene:{LinkedScene.name}");
                // }
            }
        }
        else
        {
            //Debug.Log($"SubScene_Base.InitIdDict LinkedScene==null scene:{this.name}");
        }
    }

    [ContextMenu("EditorSaveScene")]
    public void EditorSaveScene()
    {
        if (IsLoaded == false)
        {
            Debug.LogWarning("EditorLoadScene IsLoaded==false :" + GetSceneName());
            return;
        }
        IsLoaded = false;

        if (this is SubScene_Single)
        {
            var gs = SubSceneHelper.GetChildrenGos(GetSceneParent());
            SetObjects(gs);
        }

        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();

        //Scene scene=EditorHelper.CreateScene(scenePath, true, subSceneManager.IsOpenSubScene, gos.ToArray());
        Scene scene = SubSceneHelper.CreateScene(sceneArg);
        gos.Clear();
        IsLoaded = false;

        AreaTreeHelper.InitCubePrefab();
        ShowBounds();
        bool r1 = UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
        Debug.Log("r1:" + r1);
    }

    //public void SaveScene(string path, bool isOverride)
    //{
    //    SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
    //    sceneArg = new SubSceneArg(path, isOverride, subSceneManager.IsOpenSubScene, gos.ToArray());
    //    scene = SubSceneHelper.CreateScene(sceneArg);
    //    //SetPath(path);
    //    GetSceneName();
    //}

    public void SaveScene(SubSceneArg arg)
    {
        scene = SubSceneHelper.CreateScene(arg);
        this.IsLoaded=false;
    }

    public void SaveScene()
    {
        scene = SubSceneHelper.CreateScene(sceneArg);
        this.IsLoaded=false;
    }

    public void SetArg(string path, bool isOverride, List<GameObject> gs)
    {
        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        if (sceneArg == null)
        {
            sceneArg = new SubSceneArg();
        }
        sceneArg.path = path;
        sceneArg.isOveride = isOverride;
        sceneArg.isOpen= subSceneManager.IsOpenSubScene;
        sceneArg.objs = gs;
        GetSceneName();
    }

#endif



    internal void Init()
    {
        if (gos.Count == 0)
        {
            Debug.LogError($"SubScene.Init gos.Count == 0 subScene:{this.name} contentType:{contentType}");
        }
        //if (gos.Count == 0)
        //{
        //    var renderers = this.GetComponentsInChildren<MeshRenderer>(true);
        //    InitRenderersInfo(renderers);
        //}
        //else
        {
            List<MeshRenderer> renderers = GetSceneRenderers();
            InitRenderersInfo(renderers.ToArray());
        }

        InitVisible();

        this.sceneParent = this.transform;
    }

    public List<GameObject> GetLoadedSceneGos()
    {
        List<GameObject> objs = new List<GameObject>();
        foreach (var go in gos)
        {
            if (go == null) continue;
            if (!objs.Contains(go))
            {
                objs.Add(go);
            }
        }
        if(sceneArg!=null)
            foreach (var go in sceneArg.objs)
            {
                if (go == null) continue;
                if (!objs.Contains(go))
                {
                    objs.Add(go);
                }
            }
        if (objs.Count == 0 && sceneParent!=null)
        {
            for(int i=0;i<sceneParent.childCount;i++)
            {
                var child = sceneParent.GetChild(i);
                objs.Add(child.gameObject);
            }
        }
        return objs;
    }

    public List<MeshRenderer> GetSceneRenderers()
    {
        List<GameObject> objs = GetLoadedSceneGos();
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        foreach (var go in objs)
        {
            if (go == null) continue;
            renderers.AddRange(go.GetComponentsInChildren<MeshRenderer>(true));
        }
        //Debug.Log($"SubScene_Base.GetSceneRenderers name:{this.name} gos:{objs.Count},renderers:{renderers.Count}");
        return renderers;
    }

    private void InitVisible()
    {
        IsVisible = true;
        foreach (var go in gos)
        {
            if (go.activeInHierarchy == false)
            {
                IsVisible = false;
                break;
            }
        }
    }

    private void InitRenderersInfo(MeshRenderer[] renderers)
    {
        rendererCount = renderers.Length;
        bounds = ColliderHelper.CaculateBounds(renderers);
        center = bounds.center;
        //radius=bounds.
        vertexCount = GetVertexCount(renderers);
        //Debug.Log($"SubScene_Base.Init name:{this.name} renderers:{renderers.Length} bounds:{bounds} center:{center}");
    }

    public float GetVertexCount(MeshRenderer[] renderers)
    {
        int count = 0;
        foreach (var renderer in renderers)
        {
            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;
            if (meshFilter.sharedMesh == null) continue;
            count += meshFilter.sharedMesh.vertexCount;
        }
        float vertexCount = count / 10000.0f;
        return vertexCount;
    }

    [ContextMenu("GetClostPoint")]
    public void GetClostPoint()
    {
        Camera cam = GameObject.FindObjectOfType<Camera>();
        var pos = cam.transform.position;
        var clo = this.bounds.ClosestPoint(pos);
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.position = clo;

        var dis = Vector3.Distance(clo, pos);
        Debug.Log("dis:" + dis+"|"+(dis*dis));
        Debug.DrawLine(clo, pos, Color.red, 10);
    }

    [ContextMenu("CreateLOD")]
    public void CreateLOD()
    {
        var renderers = GetSceneRenderers();
        foreach(var renderer in renderers)
        {
            //AutomaticLODHelper.CreateLOD(renderer.gameObject, null, null, null, true, true);
            LODManager.Instance.CreateAutoLOD(renderer.gameObject);
        }
    }

    public void OnDestroy()
    {
        //Debug.Log("SubScene.OnDestroy:"+this.name);
    }
}

public class SubSceneBag:List<SubScene_Base>
{
    public SubSceneBag(SubScene_Base[] scenes)
    {
        var list = scenes.ToList().Where(s => s != null).ToList();
        this.AddRange(list);
    }

    public SubSceneBag()
    {

    }

    //public SubSceneList ToList()
    //{
    //    var list=this.ToList().Where(s => s != null).ToList();
    //}
}

public class SubSceneBagList:List<SubSceneBag>
{
    public SubSceneBag GetAllScenes()
    {
        SubSceneBag bag = new SubSceneBag();
        foreach(var item in this)
        {
            bag.AddRange(item);
        }
        return bag;
    }

    public SubScene_Base[] GetAllScenesArray()
    {
        return GetAllScenes().ToArray();
    }
}



public class SubSceneList<T> : List<T> where T : SubScene_Base
{

}