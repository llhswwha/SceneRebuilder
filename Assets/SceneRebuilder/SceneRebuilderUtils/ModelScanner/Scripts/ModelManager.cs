using CommonUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ModelManager : MonoBehaviour
{
    public Transform cameraT;
    public Vector3 cameraPos;
    public Vector3 cameraForward;
    public List<Transform> models = new List<Transform>();
    public DistanceMode DisMode;
    public List<ModelLevelSettingInfo> Levels = new List<ModelLevelSettingInfo>();
    public List<SubModelController> Controllers = new List<SubModelController>();
    public bool IsAddScript = true;
    public int GroupSize = 500;
    public int GroupCount = 0;
    public float DisPower = 50;
    public float MinDis = 100f;
    public bool CheckAngle = true;
    // public bool CheckRaycast = false;
    public bool SetParent = true;
    // Start is called before the first frame update


    public int AllCount = 0;
    //动态
    public int VisibleCount = 0;
    public int HiddenCount = 0;
    public int ChangedCount = 0;
    public GameObject LevelsRoot = null;

    public int TestLastCount = 0;

    public bool InitVisible = false;

    public bool Debug_1_DoNone=false;

    public bool Debug_2_Distance=false;

    public bool Debug_3_KeepHidden=false;

    public int MaxVertexCount=200;

    public bool IsClearStatic=true;

    [ContextMenu("DistroyControllers")]
    public void DistroyControllers()
    {
        // foreach (var item in Controllers)
        // {
        //     GameObject.DestroyImmediate(item);
        // }
        if(LevelsRoot){
            LevelsRoot.name+="_Old";
            GameObject.DestroyImmediate(LevelsRoot);
        }
        Controllers.Clear();
    }

    [ContextMenu("ClearInit")]
    public void ClearInit()
    {
        Levels.Clear();
        DistroyControllers();
        Init();
    }

    internal void SetLayer(int l,int tl)
    {
        for (int i = 0; i < Controllers.Count; i++)
        {
            var controller = Controllers[i];
            controller.SetLayer(l,tl);
        }
    }

    internal void ShowNotInLayer(int l)
    {
        for (int i = 0; i < Controllers.Count; i++)
        {
            var controller = Controllers[i];
            controller.ShowNotInLayer(l);
        }
    }

    

    [ContextMenu("ResetInit")]
    public void ResetInit()
    {
        DateTime start = DateTime.Now;
        DistroyControllers();
        Init();
        TimeSpan t = DateTime.Now - start;
        Debug.Log(string.Format("ModelManager.ResetInit 用时{0}", t));
    }

    private List<MeshRenderer> GetRenderers()
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        if (models.Count > 0)
        {
            foreach (var item in models)
            {
                var list = item.GetComponentsInChildren<MeshRenderer>(true);
                renderers.AddRange(list);
                Debug.Log(string.Format("GetRenderers item:{0},count:{1}",item,list.Length));
            }
        }
        else
        {
            var list = gameObject.GetComponentsInChildren<MeshRenderer>(true);
            renderers.AddRange(list);
        }
        AllCount = renderers.Count;
        Debug.Log("GetRenderers:"+AllCount);
        return renderers;
    }

    [ContextMenu("SetLevels_Pipe")]
    public void SetLevels_Pipe(){
        Levels.Clear();
        //厂区管道
        Levels.Add(new ModelLevelSettingInfo(0.25f, 0.3f, 30));//1
        Levels.Add(new ModelLevelSettingInfo(0.5f, 0.3f, 35));//2
        Levels.Add(new ModelLevelSettingInfo(0.75f, 0.3f, 35));//2
        Levels.Add(new ModelLevelSettingInfo(1f, 0.3f, 40));

        Levels.Add(new ModelLevelSettingInfo(2f, 0.2f, 45));
        Levels.Add(new ModelLevelSettingInfo(4f, 0.2f, 50));
        Levels.Add(new ModelLevelSettingInfo(8f, 0.2f, 50));
        Levels.Add(new ModelLevelSettingInfo(12f, 0.1f, 60)); 
        Levels.Add(new ModelLevelSettingInfo(16f, 0.1f, 60));
        Levels.Add(new ModelLevelSettingInfo(20f, 0.1f, 60,true));
        Levels.Add(new ModelLevelSettingInfo(24f, 0.1f, 60,true));
        Levels.Add(new ModelLevelSettingInfo(32f, 0.1f, 60,true));
        Levels.Add(new ModelLevelSettingInfo(64f, 0.1f, 70,true));
        Levels.Add(new ModelLevelSettingInfo(128, 0.1f, 80,true));//9
        Levels.Add(new ModelLevelSettingInfo(256, 0.1f, 120,true));//9
        Levels.Add(new ModelLevelSettingInfo(float.MaxValue, 0.1f, 150,true));//10
    }

    [ContextMenu("SetLevels_Bar")]
    public void SetLevels_Bar(){
        Levels.Clear();

        Levels.Add(new ModelLevelSettingInfo(0.1f, 0.3f, 20));//1
        Levels.Add(new ModelLevelSettingInfo(0.2f, 0.3f, 20));//1
        Levels.Add(new ModelLevelSettingInfo(0.3f, 0.3f, 25));//2
        Levels.Add(new ModelLevelSettingInfo(0.4f, 0.3f, 25));//2
        Levels.Add(new ModelLevelSettingInfo(0.5f, 0.3f, 25));//2
        Levels.Add(new ModelLevelSettingInfo(0.6f, 0.3f, 30));//2
        Levels.Add(new ModelLevelSettingInfo(0.7f, 0.3f, 30));//2
        Levels.Add(new ModelLevelSettingInfo(0.8f, 0.3f, 30));//2
        Levels.Add(new ModelLevelSettingInfo(0.9f, 0.3f, 30));//2
        Levels.Add(new ModelLevelSettingInfo(1f, 0.3f, 35));
        Levels.Add(new ModelLevelSettingInfo(1.5f, 0.3f, 40));
        Levels.Add(new ModelLevelSettingInfo(2f, 0.2f, 45));
        Levels.Add(new ModelLevelSettingInfo(5f, 0.2f, 50));
        Levels.Add(new ModelLevelSettingInfo(10f, 0.2f, 55));
        Levels.Add(new ModelLevelSettingInfo(20f, 0.1f, 60));
        Levels.Add(new ModelLevelSettingInfo(float.MaxValue, 0.1f, 150,true));//10
    }

    [ContextMenu("Init")]
    public void Init()
    {
        DateTime start = DateTime.Now;
        AllCount = 0;

        if (cameraT == null)
        {
            cameraT = Camera.main.transform;
        }

        if (Levels.Count == 0)
        {
            //越小的物体，越多，刷新的频率大一些，可视角度小一些，不然，比较消耗资源。
            SetLevels_Pipe();
        }
        if (Controllers.Count == 0)
        {
            //var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            //AllCount = renderers.Length;
            List<MeshRenderer> renderers = GetRenderers();

            GameObject lvsObj = new GameObject("Levels");
            lvsObj.transform.parent = this.transform;
            LevelsRoot = lvsObj;
            for (int i = 0; i < Levels.Count; i++)
            {
                ModelLevelSettingInfo levelInfo = Levels[i];
                GameObject lvObj = new GameObject();
                lvObj.name = string.Format("[{0}]Level_{1}", (i+1),levelInfo.maxDiameter);
                lvObj.transform.parent = lvsObj.transform;


                var controller = lvObj.AddComponent<SubModelController>();
                controller.levelInfo = levelInfo;
                controller.Manager = this;
                controller.cameraT = cameraT;

                if (TestLastCount > 0)
                {
                    if (i >= Levels.Count - TestLastCount)
                    {
                        controller.UpdateEnbale = true;
                    }
                    else
                    {
                        controller.UpdateEnbale = false;
                    }
                }
                
                
                Controllers.Add(controller);
            }

            foreach (var renderer in renderers)
            {
                RendererInfo rendererInfo = new RendererInfo(renderer);
                rendererInfo.fullDiameter = (rendererInfo.diameter * this.DisPower);

                ModelDisInfo modelInfo=null;
                if (IsAddScript)
                {
                    var oldInfos=renderer.gameObject.GetComponents<ModelDisInfo>();
                    foreach(var item in oldInfos){
                        GameObject.DestroyImmediate(item);
                    }
                    modelInfo = renderer.gameObject.AddComponentEx<ModelDisInfo>();
                    modelInfo.info = rendererInfo;
                }
                else{
                    var oldInfos=renderer.gameObject.GetComponents<ModelDisInfo>();
                    foreach(var item in oldInfos){
                        GameObject.DestroyImmediate(item);
                    }
                }
                rendererInfo.SetVisible(InitVisible);

                for (int i = 0; i < Controllers.Count; i++)
                {
                    var controller = Controllers[i];
                    ModelLevelSettingInfo levelInfo = controller.levelInfo;
                    if (rendererInfo.diameter < levelInfo.maxDiameter)
                    {
                        controller.rendererInfos.Add(rendererInfo);
                        rendererInfo.maxAngle=levelInfo.CameraMaxAngle;
                        if (SetParent)
                        {
                            renderer.transform.parent = controller.transform;
                        }
                        if (IsAddScript)
                        {
                            controller.models.Add(modelInfo);
                        }
                        break;
                    }
                }
            }

            for (int i = 0; i < Controllers.Count; i++)
            {
                var controller = Controllers[i];
                controller.Init();
            }
        }

        TimeSpan t = DateTime.Now - start;
        Debug.Log(string.Format("ModelManager.Init 用时{0}", t));
    }

    [ContextMenu("EnableUpdate")]   
    public void EnableUpdate()
    {
        for (int i = 0; i < Controllers.Count; i++)
        {
            var controller = Controllers[i];
            controller.UpdateEnbale=true;
        }
    }

    [ContextMenu("DisableUpdate")]   
    public void DisableUpdate()
    {
        for (int i = 0; i < Controllers.Count; i++)
        {
            var controller = Controllers[i];
            controller.UpdateEnbale=false;
        }
    }

    public void SetMaterial(Material mat){
        for (int i = 0; i < Controllers.Count; i++)
        {
            var controller = Controllers[i];
            controller.SetMaterial(mat);
        }
    }

    public bool IsUpdating()
    {
        for (int i = 0; i < Controllers.Count; i++)
        {
            var controller = Controllers[i];
            if(controller.UpdateEnbale==true)return true;
        }
        return false;
    }

    [ContextMenu("InitDistanceInfoList")]
    public void InitDistanceInfoList()
    {
        var start = DateTime.Now;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            item.InitDistanceInfoList();
        }
        var t = (DateTime.Now - start).TotalMilliseconds;
        Debug.Log("UpdateModelPos 用时:"+t);
    }

    [ContextMenu("UpdateModelPos")]
    public void UpdateModelPos()
    {
        var start = DateTime.Now;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            item.UpdateModelPos();
        }
        var t = (DateTime.Now - start).TotalMilliseconds;
        Debug.Log("UpdateModelPos 用时:"+t);
    }

    void Start()
    {
        Init();
        StartCoroutine(ShowLogInfo());
    }

    public Text LogText;

    public bool ShowLog=true;

    private IEnumerator ShowLogInfo(){
        while(ShowLog){
            VisibleCount = 0;
            HiddenCount = 0;
            ChangedCount = 0;
            double t = 0;
            for (int i = 0; i < Controllers.Count; i++)
            {
                var controller = Controllers[i];
                VisibleCount += controller.VisibleCount;
                HiddenCount+= controller.HiddenCount;
                ChangedCount+= controller.ChangedCount;
                t += controller.UpdateTime;
            }

            if (LogText != null)
            {
                LogText.text = string.Format("Count={0}\nVisible={1}\nHidden={2}\nChanged={3}\nTime={4}\nUpdating={5}"
            , AllCount, VisibleCount, HiddenCount,ChangedCount, t,this.IsUpdating());
            }
            yield return new WaitForSeconds(0.5f);
        } 
    }

    [ContextMenu("UpdateCameraPos")]
    public void UpdateCameraPos()
    {
        var start = DateTime.Now;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            item.cameraPos=this.cameraPos;
            item.cameraForward=this.cameraForward;
        }
    }

    [ContextMenu("ShowAllRenders")]
    public void ShowAllRenders()
    {
        ResetInit();
        ShowAll();
    }

    [ContextMenu("ShowAll")]
    public void ShowAll()
    {
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            item.ShowAll();
        }
    }

    [ContextMenu("InitState")]
    public void InitState()
    {
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            item.InitState();
        }
    }


    public void InitState(int l)
    {
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            item.InitState(l);
        }
    }

    public int StaticCount=0;

    public List<GameObject> StaticModels=new List<GameObject>();

    public int GetStaticCount(){
        StaticModels.Clear();
        StaticCount=0;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            StaticCount+=item.GetStaticCount(StaticModels);
        }
        return StaticCount;
    }

    public List<GameObject> CheckedStaticObjects=new List<GameObject>();

    public bool IsChangCheckedObjs=false;

    public string checkKey="Slab";

    [ContextMenu("CheckStatic")]
    public void CheckStatic(){
        CheckedStaticObjects=new List<GameObject>();
        int count=0;
        foreach(var item in StaticModels){
            if(item.activeInHierarchy==false){
                continue;
            }
            if(item.name.Contains(checkKey)==false)
            {
                continue;
            }
            var pos1=item.transform.position;
            var pos2=pos1+new Vector3(0,500,0);
            Ray ray=new Ray(pos1,pos2-pos1);
            // RaycastHit hit;
            // if(Physics.Raycast(ray,out hit,1000))
            if(Physics.Raycast(ray,1000))
            {
                //if(hit.collider.gameObject.name.Contains(checkKey))
                {
                    if(IsChangCheckedObjs){
                        MeshRenderer renderer=item.GetComponent<MeshRenderer>();
                        renderer.enabled=false;
                        item.tag="Untagged";
                    
                    }
                    count++;
                    CheckedStaticObjects.Add(item);
                }
                // else{
                //     Debug.Log(item.name+" -->> "+hit.collider.gameObject.name);
                // }
            }
            else{

            }
        }
        Debug.LogError("CheckStatic count:"+count);
    }

    [ContextMenu("RecoverStatic")]
    public void RecoverStatic(){
        foreach(var item in StaticModels){
            MeshRenderer renderer=item.GetComponent<MeshRenderer>();
            renderer.enabled=true;
            item.tag="ScanStatic";
        }
    }

    [ContextMenu("HideAll")]
    public void HideAll()
    {
        var start = DateTime.Now;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            item.HideAll();
        }
    }

    [ContextMenu("SetAllHiddenNew")]
    public int SetAllHiddenNew()
    {
        int sum=0;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            sum+=item.SetAllHiddenNew();
        }
        return sum;
    }

    public int GetStateCount(bool state)
    {
        int sum=0;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            sum+=item.GetStateCount(state);
        }
        return sum;
    }

    public int GetCount(){
        int sum=0;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            sum+=item.GetCount();
        }
        return sum;
    }

    [ContextMenu("AddMeshCollider")]
    public void AddMeshCollider()
    {
        var start = DateTime.Now;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            item.AddMeshCollider();
        }
        var t = (DateTime.Now - start).TotalMilliseconds;
        Debug.Log("AddMeshCollider 用时:"+t);
    }

    [ContextMenu("RemoveScripts")]
    public void RemoveScripts()
    {
        var start = DateTime.Now;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            item.RemoveScripts();
        }
        var t = (DateTime.Now - start).TotalMilliseconds;
        Debug.Log("RemoveScripts 用时:"+t);
    }

    [ContextMenu("RemoveCollider")]
    public void RemoveCollider()
    {
        var start = DateTime.Now;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            item.RemoveCollider();
        }
        var t = (DateTime.Now - start).TotalMilliseconds;
        Debug.Log("RemoveCollider 用时:"+t);
    }

    [ContextMenu("DisableCollider")]
    public void DisableCollider()
    {
        var start = DateTime.Now;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            item.DisableCollider();
        }
        var t = (DateTime.Now - start).TotalMilliseconds;
        Debug.Log("DisableCollider 用时:"+t);
    }

    [ContextMenu("EnableCollider")]
    public void EnableCollider()
    {
        var start = DateTime.Now;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            item.EnableCollider();
        }
        var t = (DateTime.Now - start).TotalMilliseconds;
        Debug.Log("EnableCollider 用时:"+t);
    }

    [ContextMenu("DisactiveObjects")]
    public void DisactiveObjects()
    {
        var start = DateTime.Now;
        for (int i = 0; i < Controllers.Count; i++)
        {
            var item = Controllers[i];
            item.DisactiveObjects();
        }
        var t = (DateTime.Now - start).TotalMilliseconds;
        Debug.Log("DisactiveObjects 用时:"+t);
    }

    // public bool IsCameraChanged=false;

    void Update()
    {
        if(cameraT.position!=cameraPos || cameraT.forward!=cameraForward)
        {
            cameraPos=cameraT.position;
            cameraForward=cameraT.forward;
            // IsCameraChanged=true;
            EnableUpdate();
            UpdateCameraPos();
        }
        
    }
}
