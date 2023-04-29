using System;
using System.Collections;
using System.Collections.Generic;
using DbModel.Location.Pipes;
using Mogoson.CameraExtension;
using UnityEngine;
using Y_UIFramework;

public class PipeSystemBuilder : SingletonBehaviour<PipeSystemBuilder>
{
    public Material pipeMat;
    public PipeType[] pipeTypes;
    public float pipeSizePow=1;
    public float pipeRadius=1;
    public int pipeSegments=12;
    private Dictionary<string,GameObject> sysObjDict=new Dictionary<string, GameObject>();
    public List<Material> materials=new List<Material>();
    public GameObject pointFollowUIPrefab;
    public GameObject currentPipeSysObj=null;

    public bool IsShowPointFollowUI=true;

    public void SetIsShowPointFollowUI(bool v){
        IsShowPointFollowUI=v;
        if(v){
            PipeSystemUtils.ShowPointPoints();
            
        }
        else{
            PipeSystemUtils.HidePointPoints();
            
        }
    }

    public bool IsPipeEditMode=false;

    public void SetIsPipeEditMode(bool v){
        IsPipeEditMode=v;
        if(v){
            
            if(PipeSystemComponent.currentPipeSystem!=null){
                //// UIManager.GetInstance().ShowUIPanel("PipeSystemEditUIPanel");
                //// MessageCenter.SendMsg(MsgType.PipeSystemEditPanelMsg.TypeName, MsgType.PipeSystemEditPanelMsg.InitData, PipeSystemComponent.currentPipeSystem.pipeSystem);

                PipeSystemComponent.currentPipeSystem.OnClick();
            }
            
            if(ObjectsEditManage.Instance)ObjectsEditManage.Instance.OpenDevEdit();
            if(RTEManager.Instance)RTEManager.Instance.ShowHandles(false);
        }
        else{
            UIManager.GetInstance().CloseUIPanels("PipeSystemEditUIPanel");
            if(ObjectsEditManage.Instance)ObjectsEditManage.Instance.CloseDevEdit();
            if(RTEManager.Instance)RTEManager.Instance.HideHandles();
        }

        PipePointFollowUI.SetIsEditMode(v);
    }

    void Awake()
    {
        InitPipeTypes(null);

        editListener=this.GetComponent<PipeSystemEditEventListener>();
        if(editListener==null){
            editListener=this.gameObject.AddComponent<PipeSystemEditEventListener>();
        }

        PipePoint.ppOffsetX=this.ppOffsetX;
        PipePoint.ppOffsetY=this.ppOffsetY;
        PipePoint.ppOffsetXMin=this.ppOffsetXMin;
        PipePoint.ppOffsetYMin=this.ppOffsetYMin;
    }

    private PipeSystemEditEventListener editListener;

    void InitPipeTypes(Action<PipeType[]> callBack){
        Debug.LogError("PipeSystemBuilder.InitPipeTypes Start  ");
        CommunicationObject.Instance.GetPipeTypes(types=>{
            if(types!=null){
                this.pipeTypes=types;
                Debug.LogError($"PipeSystemBuilder.InitPipeTypes End types:{types.Length}");
            }
            else{
                Debug.LogError("PipeSystemBuilder.InitPipeTypes End types == null ");
            }
            if(callBack!=null){
                callBack(types);
            }
        });
    }

    public void GetPipeTypes(Action<PipeType[]> callBack){

        if(pipeTypes!=null&&pipeTypes.Length>0)
        {
            Debug.Log("PipeSystemBuilder.GetPipeTypes 1 ");
            if(callBack!=null){
                callBack(pipeTypes);
            }
        }
        else{
            Debug.Log("PipeSystemBuilder.GetPipeTypes 2 ");
            InitPipeTypes(callBack);
        }
    }

    public GameObject ShowPipeSystem(PipeSystem pipeSystem){
        string key=pipeSystem.GetKey();
        GameObject obj=GetPipeSystemObj(key);
        if(obj==null)
        {
            obj=GeneratePipeSystem(pipeSystem);
            obj.name+=$"({key})";
            //obj.AddComponent<EnableDebug>();
            Debug.Log($"ShowPipeSystem_1 key:{key} pipeSystem:{pipeSystem} obj:{obj}");
        }
        else{
            Debug.Log($"ShowPipeSystem_2 key:{key} pipeSystem:{pipeSystem} ");
        }
        //currentPipeSysObj=obj;
        return obj;
    }
    
    public float GetPipeRadius(float sizeX){
        float radius=sizeX;
        if(radius==0)
        {
            radius=pipeRadius;
        }
        return radius*pipeSizePow;
    }

    public int GetPipeSegments(PipeType pipeType)
    {
        int seg=pipeSegments;
        if(pipeType.Segments!=0){
            seg=pipeType.Segments;
        }
        if(seg<4){
            seg=4;
        }
        return seg;
    }

    public PipeSysGenerateArg GetPipeSysGenerateArg(PipeSystem pipeSystem)
    {
        PipeType pipeType=GetPipeTypeByName(pipeSystem.Type);
        PipeSysGenerateArg arg=new PipeSysGenerateArg();
        arg.mat=GetPipeMaterial(pipeType);
        //systemComponent.pipeMat=GetPipeMaterial(pipeType);
        arg.radius=GetPipeRadius(pipeSystem.SizeX);
        arg.segments=GetPipeSegments(pipeType);
        arg.isSmooth=pipeType.IsSmooth;
        Debug.Log($"GetPipeSysGenerateArg type:{pipeType.Name} mat:{arg.mat} radius:{arg.radius} seg:{arg.segments} IsSmooth:{pipeType.IsSmooth}");
        return arg;
    }

    public GameObject GeneratePipeSystem(PipeSystem pipeSystem){
        
        GameObject obj=new GameObject();
        
        obj.layer=LayerMask.NameToLayer("DepDevice");
        obj.name="PipeSystem_"+pipeSystem.Name;
        PipeSystemComponent systemComponent=obj.AddComponent<PipeSystemComponent>();
        systemComponent.pipeSystem=pipeSystem;

        PipeSysGenerateArg arg=GetPipeSysGenerateArg(pipeSystem);
        systemComponent.GeneratePipe(arg);

        obj.transform.SetParent(this.transform);

        string key=pipeSystem.GetKey();
        if(sysObjDict.ContainsKey(key)){
            sysObjDict[key]=obj;
        }
        else{
            sysObjDict.Add(key,obj);
        }
        
        //obj.transform.position=new Vector3(100,100,100);
        return obj;
    }

    public GameObject GetPipeSystemObj(string id){
        if(sysObjDict.ContainsKey(id)){
            return sysObjDict[id];
        }
        return null;
    }

    public void ClearPipeSystems()
    {
        foreach(var key in sysObjDict.Keys){
            GameObject pipeSysObj=sysObjDict[key];
            if(pipeSysObj==null)continue;
            GameObject.DestroyImmediate(pipeSysObj);
        }
        sysObjDict.Clear();
    }

    List<PipeSystem> pipeSystems=new List<PipeSystem>();

    public void ShowAllPipeSystems(List<PipeSystem> pipeSyss,Action<int,PipeSystem,GameObject> progressCallback=null,Action callback=null)
    {
        pipeSystems=pipeSyss;
        Debug.Log($"PipeSystemBuilder.ShowAllPipeSystems pipeSystems:{pipeSyss.Count}");
        StartCoroutine(ShowAllPipeSystemsAsync(pipeSyss,progressCallback,callback));
    }

    public void ReShowAllPipeSystems()
    {
        ClearPipeSystems();
        PipeSystemUtils.ClearPointPoints();
        ShowAllPipeSystems(pipeSystems);
    }

    public int MaxPipeSystemCount=0;

    private IEnumerator ShowAllPipeSystemsAsync(List<PipeSystem> pipeSyss,Action<int,PipeSystem,GameObject> progressCallback,Action finishCallback)
    {
        DateTime startT=DateTime.Now;

        PipeSystemBuilder builder=PipeSystemBuilder.Instance;
        int maxCount=pipeSyss.Count;
        if(MaxPipeSystemCount>0 && MaxPipeSystemCount<maxCount){
            maxCount=MaxPipeSystemCount;
        }
        Debug.Log($"PipeSystemBuilder.ShowAllPipeSystemsAsync Start pipeSystems:{pipeSyss.Count} maxCount:{maxCount}");
        for (int i = 0; i < maxCount; i++)
        {
            PipeSystem pipeSys = pipeSyss[i];
            if (pipeSys==null)continue;
            try
            {
                float p=i/(maxCount+0.0f);
                //pipeSys.Name="_"+(i+1);
                ProgressbarLoad.Instance.Show(p,$"创建管道[{pipeSys.Name}]");
                GameObject pipeObj=builder.ShowPipeSystem(pipeSys);
                if(pipeObj!=null){
                    pipeObj.SetActive(true);
                }
                if(progressCallback!=null){
                    progressCallback(i,pipeSys,pipeObj);
                }
            }
            catch(Exception ex){
                Debug.LogError($"PipeSystemTreePanel.ShowAllPipeSystemsAsync pipeSys:{pipeSys} Exception:{ex}");
            }

            Debug.Log($"PipeSystemBuilder.ShowAllPipeSystemsAsync[{i}] count:{pipeSys.PointCount} pipeSys:{pipeSys} {(DateTime.Now-startT).TotalSeconds:F2}s");
            yield return null;
        }
        ProgressbarLoad.Instance.Hide();
        Debug.Log($"PipeSystemBuilder.ShowAllPipeSystemsAsync End {(DateTime.Now-startT).TotalSeconds:F2}s ");
        if(finishCallback!=null){
            finishCallback();
        }
    }

    [ContextMenu("DeleteAllPipeSystems")]
    public void DeleteAllPipeSystems()
    {
        DeleteAllPipeSystems(pipeSystems);
    }

    public void DeleteAllPipeSystems(List<PipeSystem> pipeSystems)
    {
        List<PipeSystem> list=new List<PipeSystem>();
        for (int i = 0; i < pipeSystems.Count; i++)
        {
            var pipeSysObj=GetPipeSystemObj(pipeSystems[i].GetKey());
            if(pipeSysObj!=null){
                list.Add(pipeSystems[i]);
            }
        }
        StartCoroutine(DeleteAllPipeSystemsAsync(list));
    }

    public static IEnumerator DeleteAllPipeSystemsAsync(List<PipeSystem> pipeSystems)
    {
        Debug.Log($"PipeSystemTreePanel.DeleteAllPipeSystems Start pipeSystems:{pipeSystems.Count} 1");
        PipeSystemBuilder builder=PipeSystemBuilder.Instance;
        bool isAdded=false;
            for (int i = 0; i < pipeSystems.Count; i++)
            {
                PipeSystem pipeSys = pipeSystems[i];
                if (pipeSys==null)continue;
                float p=i/(pipeSystems.Count+0.0f);
                //pipeSys.Name="_"+(i+1);
                ProgressbarLoad.Instance.Show(p,$"添加管道[{pipeSys.Name}]");

                isAdded=false;
                PipeSystemClient.DeletePipeSystem(pipeSys,false,(result)=>{
                    isAdded=true;
                });
                while(isAdded==false){
                    yield return null;
                }
                yield return null;
            }
        ProgressbarLoad.Instance.Hide();
        Debug.Log($"PipeSystemTreePanel.DeleteAllPipeSystems End");
    }

    public void LocatePoint(PipePoint point){
        Debug.Log($"LocatePoint point:{point}");
        PipeSystemComponent pipeSystemComponent=GetPipeSystemComponent(point.PId);
        if(pipeSystemComponent){
            //pipeSystemComponent.ShowPipePointsFollowUI();
            pipeSystemComponent.FocusPoint(point);
        }
        else{
            Debug.LogError($"LocatePoint pipeSystemComponent==null PId:{point.PId}");
        }
    }

    internal void ModifyPoint(PipePoint currentPoint)
    {
         var systemComponent=GetPipeSystemComponent(currentPoint.PId);
         systemComponent.ModifyPoint(currentPoint);
    }

    private Dictionary<PipePointComponent,PipeSystemComponent> pointToPipeDict=new Dictionary<PipePointComponent, PipeSystemComponent>();

    internal void RefreshPipe(PipePointComponent pipeCom)
    {
        PipeSystemComponent systemComponent=null;
        if(pointToPipeDict.ContainsKey(pipeCom)){
            systemComponent=pointToPipeDict[pipeCom];
        }
        else{
            systemComponent=GetPipeSystemComponent(pipeCom.pipeSys.Id);
            pointToPipeDict.Add(pipeCom,systemComponent);
        }
        
        systemComponent.RefreshPipe(pipeCom);
    }

    public PipeSystemComponent GetPipeSystemComponent(string id){
        GameObject obj=GetPipeSystemObj(id);
        if(obj==null)return null;
        PipeSystemComponent systemComponent=obj.GetComponent<PipeSystemComponent>();
        return systemComponent;
    }

    public PipeSystemComponent GetPipeSystemComponent(int id){
        return GetPipeSystemComponent(id+"");
    }

    public PipeType DefaultPipeType=new PipeType("电缆", "黄色", 4,false);

    public PipeType GetPipeTypeByName(string typeName){
        foreach(PipeType pipeType in pipeTypes)
        {
            if(pipeType.Name==typeName){
                return pipeType;
            }
        }
        return DefaultPipeType;
    }

    public Material GetPipeMaterial(string typeName){

        Material mat0=null;
        foreach(var mat in materials){
            if(mat.name==typeName)
            {
                mat0=mat;
                break;
            }
        }
        if(mat0==null){
            PipeType pipeType=GetPipeTypeByName(typeName);
            foreach(var mat in materials){
                if(mat.name==pipeType.Name)
                {
                    mat0=mat;
                    break;
                }
            }
        }

        if(mat0==null){
            Material mat=pipeMat;
            if(mat0==null){
                mat0=new Material(Shader.Find("HDRP/Lit"));
            }
        }
        Debug.Log($"GetPipeMaterial typeName:{typeName} mat0:{mat0}");
        return mat0;
    }

    public Material GetPipeMaterial(PipeType pipeType){
        Material mat0=null;
        if(mat0==null){
            foreach(var mat in materials){
                if(mat.name==pipeType.Name)
                {
                    mat0=mat;
                    break;
                }
            }
        }
        if(mat0==null){
            Material mat=pipeMat;
            if(mat0==null){
                mat0=new Material(Shader.Find("HDRP/Lit"));
            }
        }
        Debug.Log($"PipeSystemBuilder.GetPipeMaterial pipeType:{pipeType} mat0:{mat0}");
        return mat0;
    }

    private GameObject pipeSysCenter;

    public void FocusPipeSystemObject(GameObject pipeObj){
        Bounds bounds = new Bounds(pipeObj.transform.position, Vector3.zero);
        Renderer[] renderers = pipeObj.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            //UnityEngine.Debug.Log(System.String.Format(" X: {0} Y:{1} Z:{2}", r.bounds.size.x, r.bounds.size.y, r.bounds.size.z));
            bounds.Encapsulate(r.bounds);
        }
            if (AroundAlignCamera.Instance)
            {
                var size = bounds.size;
                var scale = size * 3;
                var dis = Vector3.Distance(scale, Vector3.zero);
                
                //var center=MeshRendererInfo.GetCenterPos(model.gameObject);
                //AroundAlignCamera.Instance.AlignVeiwToTarget(model.transform, dis);
                dis=dis/2f;
                Vector3 center=PipeSystemComponent.GetCenter(pipeObj);

                Debug.Log($"PipeSystemBuilder.FocusPipeSystemObject AlignVeiwToTarget size:{size} dis:{dis} center:{center}");
                if(pipeSysCenter!=null){
                    GameObject.DestroyImmediate(pipeSysCenter);
                }

                if(dis<10){
                    dis=10;
                }
                if(dis>10000 ) {
                    dis=1000;
                }

                //if(pipeSysCenter==null)
                {
                    pipeSysCenter = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    pipeSysCenter.name="PipeSysCenter";
                    pipeSysCenter.SetActive(false);
                }

                pipeSysCenter.transform.position = center;

                // AroundAlignCamera.Instance.distanceRange=new Range(0,dis*2);
                // AroundAlignCamera.Instance.AlignVeiwToTarget(go.transform, dis);

                AlignTarget alignTarget=new AlignTarget();
                alignTarget.SetCenter(pipeSysCenter.transform);
                alignTarget.distanceRange=new Range(0,dis*2);
                alignTarget.distance=dis*0.75f;
                alignTarget.angleRange=new Range(0,180);
                alignTarget.angles=AroundAlignCamera.Instance.targetAngles;
                CameraSceneManager.Instance.FocusTargetWithTranslate(alignTarget,new Vector2(1000,1000));
            }
    }



    private Camera GetMainCamera()
    {
        CameraSceneManager manager = CameraSceneManager.Instance;
        if (manager && manager.MainCamera != null)
        {
            return manager.MainCamera;
        }
        else
        {
            Debug.LogError("CameraSceneManager.MainCamera is null!");
            return null;
        }
    }

    public GameObject CreatePipePointFollowUI(GameObject pointObj,PipeSystem pipeSystem,PipePoint point, Action<PipePointFollowUI> onCreateFinished = null)
    {
        //Debug.Log($"CreatePipePointFollowUI PipeSystem:{pipeSystem} point:{point} pipeObj:{pointObj}");
        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(pointObj, Vector3.zero);
        if (UGUIFollowManage.Instance == null)
        {
            Debug.LogError("UGUIFollowManage.Instance==null");
            return null;
        }
        Camera mainCamera = GetMainCamera();
        if (mainCamera == null) {
            return null;
        }
        string devDepName = pipeSystem.Id+"_"+pipeSystem.Name;
        DisposeFollowTarget dispostTarget = targetTagObj.AddMissingComponent<DisposeFollowTarget>();
        dispostTarget.SetInfo(devDepName);
        //dispostTarget.followUI=
        //if (!DevDepNameList.Contains(devDepName)) DevDepNameList.Add(devDepName);
        GameObject name = UGUIFollowManage.Instance.CreateItem(pointFollowUIPrefab, targetTagObj, devDepName, mainCamera, false, true);
        UGUIFollowTarget followTarget = name.GetComponent<UGUIFollowTarget>();
        //followTarget.SetEnableDistace(true,60);
        PipePointFollowUI cameraInfo = name.GetComponent<PipePointFollowUI>();
        if (cameraInfo != null)
        {
            cameraInfo.InitData(point,pointObj);
            if (onCreateFinished != null) onCreateFinished(cameraInfo);
        }
        return name;
    }
    public double ppOffsetX=4960000;
    public double ppOffsetY=430000;

    public double ppOffsetXMin=4900000;
    public double ppOffsetYMin=400000;
}


    public class PipeSysGenerateArg
    {
        public float radius;
        public int segments;

        public bool isSmooth;

        public Material mat;
    }