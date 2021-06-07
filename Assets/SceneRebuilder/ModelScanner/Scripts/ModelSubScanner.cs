using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ModelSubScanner : MonoBehaviour
{
    public ModelScanner Manager;
    public ModelManagerSettings settings;

    // public Camera camera;

    // public Vector3 cameraPos;

    // void Start(){
    //     if(camera==null){
    //         camera=Camera.main;
    //     }

    //     StartCoroutine(CheckHit_Coroutine());
    // }

    public int ScreenWidth=1;

    public int ScreenHeight=1;

    public int StepX=16;

    public int StepY=16;

    public int Count=0;

    public double UpdateTimed_CheckHit;

    public double UpdateTimed_SetVisible;

    public float UpdateInterval=0.2f;

    public int UpdateSize=1000;

    public int UpdateSize2=300;

    public int UpdateCount=0;

    public bool EnableCast=true;

    public void CreateTargetPoint(){

    }

    public bool IsDebug=true;

    public bool IsStop=false;

    public bool IsPause=false;

    public bool IsBreak=false;

    public GetScanePoints points;

    public List<GameObject> HitObjects=new List<GameObject>();

    private List<Vector2> GetPointsSimple(){
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;
        List<Vector2> list=new List<Vector2>();
        for(int i=0;i<ScreenWidth;i+=StepX){
            for(int j=0;j<ScreenHeight;j+=StepY){
                Vector2 pos=new Vector2(i,j);
                list.Add(pos);
            }
        }
        return list;
    }

    //public List<RendererInfo> ChangedModels = new List<RendererInfo>();

    public List<RendererInfo> HiddenModels = new List<RendererInfo>();

    public List<RendererInfo> VisibleModels = new List<RendererInfo>();

    public int VisibleBufferCount=5;
    public int CurrentBufferIndex=0;

    public List<List<RendererInfo>> VisibleModelsBuffer = new List<List<RendererInfo>>();
    public List<List<RendererInfo>> HiddenModelsBuffer = new List<List<RendererInfo>>();

    public List<int> VisibleCountBuffer = new List<int>();
    public List<int> HiddenCountBuffer = new List<int>();

    public List<RendererInfo> HiddenModelsAll = new List<RendererInfo>();

    public List<RendererInfo> VisibleModelsAll = new List<RendererInfo>();

    public void ClearModelBufferInfos()
    {
        HiddenModels.Clear();
        VisibleModels.Clear();
        VisibleModelsBuffer.Clear();
        HiddenModelsBuffer.Clear();
        VisibleCountBuffer.Clear();
        HiddenCountBuffer.Clear();
        HiddenModelsAll.Clear();
        VisibleModelsAll.Clear();
    }

    public bool EnableScanPoints=true;

    public int ScanTime=0;

    public int HitCount=0;

    private void InitModelBuffers()
    {
        if (VisibleModelsBuffer.Count != VisibleBufferCount)
        {
            VisibleModelsBuffer.Clear();
            HiddenModelsBuffer.Clear();
            for (int i = 0; i < VisibleBufferCount; i++)
            {
                VisibleModelsBuffer.Add(new List<RendererInfo>());
                HiddenModelsBuffer.Add(new List<RendererInfo>());
                VisibleCountBuffer.Add(0);
                HiddenCountBuffer.Add(0);
            }
            CurrentBufferIndex = 0;
        }
    }

    private IEnumerator ScanPoints(List<Vector2> posList)
    {
        InitModelBuffers();

        ChangedCount =0;
        var objs = new List<GameObject>();
        int total = 0;
        int c = 0;
        int c2 = 0;
        HitCount=0;
        VisibleModels = new List<RendererInfo>();

        if(EnableScanPoints){
            var start=DateTime.Now;
            for (int i = 0; i < posList.Count; i++)
            {
                if(IsBreak){
                    IsBreak=false;
                    //Debug.LogWarning("ScanPoints Break");   
                }
                else{
                
                }
                Vector2 pos = posList[i];
                HitTest(pos,objs);
                c++;
                total++;
                if (UpdateSize > 0 && c >= UpdateSize)
                {
                    c = 0;
                    c2++;
                    yield return null;
                }

            }
            ScanTime=(int)((DateTime.Now-start).TotalMilliseconds);
        }

        foreach(var item in HiddenModels)
        {
            if(!HiddenModelsAll.Contains(item))
            {
                HiddenModelsAll.Add(item);
            }
        }

        HiddenModelsBuffer[CurrentBufferIndex]=HiddenModels;
        
        VisibleModelsBuffer[CurrentBufferIndex]=VisibleModels;
        VisibleCountBuffer[CurrentBufferIndex]=VisibleModels.Count;
        HiddenCountBuffer[CurrentBufferIndex]=HiddenModels.Count;

        CurrentBufferIndex++;
        if(CurrentBufferIndex>=VisibleBufferCount){
            CurrentBufferIndex=0;
        }

        UpdateCount = c2;
        Count = total;
        HitObjects = objs;

        // print("HiddenModels:"+ HiddenModels.Count);
        if (HiddenModels.Count > 0)
        {
             for (int i = 0; i < HiddenModels.Count; i++)
            {
                var info = HiddenModels[i];
                if(Manager.HiddenDelay == false)
                    info.SetVisible(info.visibleNew);
                ChangedCount++;
            }
        }
        HiddenModels = VisibleModels;

        // HideModelsOfBuffer();
    }

    public void CollectVisibleModels(List<RendererInfo> vAll){
        for(int i=0;i<VisibleBufferCount;i++){
            var vList=VisibleModelsBuffer[i];
            foreach(var item in vList){
                if(!vAll.Contains(item)){
                    vAll.Add(item);
                }
            }
        }
    }

    public void CollectHiddenModels(List<RendererInfo> hAll){
        for (int j = 0; j < HiddenModelsAll.Count; j++)
        {
            var info = HiddenModelsAll[j];
            if (!hAll.Contains(info))
            {
                hAll.Add(info);
            }
        }
    }


    public List<RendererInfo> CollectVisibleModels(){
        var vAll = new List<RendererInfo>();
        CollectVisibleModels(vAll);
        return vAll;
    }

    private IEnumerator HideModelsOfBuffer(){
        var start=DateTime.Now;
        var vAll=CollectVisibleModels();
        Debug.Log(string.Format("HiddenModelsAll={0}",HiddenModelsAll.Count));
        var hAll = new List<RendererInfo>();
        for (int j = 0; j < HiddenModelsAll.Count; j++)
        {
            var info = HiddenModelsAll[j];
            if(!vAll.Contains(info))
            {
                hAll.Add(info);
            }
        }
        HiddenModelsAll.Clear();

        int cc=0;
        // hAll.Clear();
        // foreach(var info in modelsDict.Values){
        //     if(!vAll.Contains(info)){
        //         info.SetVisible(false);
        //         hAll.Add(info);
        //     }
        // }

        VisibleModelsAll=vAll;
        // HiddenModelsAll=hAll;

        VisibleCount=vAll.Count;
        HiddenCount=hAll.Count;
        if (hAll.Count > 0)
        {
            int j=0;
             for (int i = 0; i < hAll.Count; i++)
            {
                var info = hAll[i];
                if(Manager.HiddenCheck){
                    Ray ray = new Ray(Manager.cameraPos, info.position - Manager.cameraPos);
                    RaycastHit hitInfo;
                    bool isHit = Manager.Raycast(ray, out hitInfo);
                    if (isHit==false || hitInfo.collider.gameObject!=info.gameObject)
                    {
                        info.SetVisible(false);
                        j++;
                        if (j >= UpdateSize2)
                        {
                            j = 0;
                            yield return null;
                        }
                    }
                }
                else
                {
                    info.SetVisible(false);
                    j++;
                    if (j >= UpdateSize2)
                    {
                        j = 0;
                        yield return null;
                    }
                }
            }
        }

        var time=(DateTime.Now-start).TotalMilliseconds;
        Debug.Log(string.Format("VisibleCount={0},HiddenCount={1},Time={2}",VisibleCount,HiddenCount,time));

        VisibleCount = Manager.GetStateCount(true);
        HiddenCount = Manager.GetStateCount(false);
        yield return null;
    }


    private Ray GetRay(Vector2 pos){
        Ray ray = Manager.camera.ScreenPointToRay(pos);//从摄像机发出到点击坐标的射线 
        return ray;
    }

    public bool Debug_OnlyHit=false;

    public bool Debug_DrawLine=false;

    private GameObject HitTest(Vector2 pos,List<GameObject> objs){
        try
        {
            DateTime start=DateTime.Now;
            GameObject gameObj=null;
                if(EnableCast){
                // if(!Manager.camera.rect.Contains(pos)){
                //     Debug.LogWarning("ModelSubScanner !Manager.camera.screenRect.Contains(pos):"+Manager.camera.rect+"|"+pos+"|"+this);
                //     return null;
                // }
                Ray ray = Manager.camera.ScreenPointToRay(pos);//从摄像机发出到点击坐标的射线 
                var t1=DateTime.Now-start;
                RaycastHit hitInfo;
                bool isHit = Manager.Raycast(ray, out hitInfo);

                if (Debug_OnlyHit){
                    return gameObj;
                }

                var t2=DateTime.Now-start;
                if (isHit)
                {
                    gameObj = hitInfo.collider.gameObject;
                    if (gameObj.layer != LayerMask.NameToLayer("Default")) return gameObj;//只处理没右设置Layer的
                    if(!objs.Contains(gameObj)){
                        if(Debug_DrawLine)
                            Debug.DrawLine(ray.origin, hitInfo.point);//划出射线，只有在scene视图中才能看到  
                        // Debug.Log("click object name is " + gameObj.name);
                        objs.Add(gameObj);

                        //MeshRenderer renderer=gameObj.GetComponent<MeshRenderer>();

                        //if(IsDebug){
                        //    renderer.enabled=true;
                        //    renderer.material=settings.GreenMaterial;
                        //}
                        //else{
                        //    if(renderer.enabled==false)
                        //        renderer.enabled=true;
                        //}

                        RendererInfo info = Manager.GetRendererInfo(gameObj);
                        info.visibleNew = true;
                        if (info.visibleNew != info.visible)
                        {
                            ChangedCount++;
                        }
                        info.SetVisible(true);

                        SetVisible(info,true);

                        VisibleModels.Add(info);
                        HiddenModels.Remove(info);
                    }
                    HitCount++;
                    // var t3=DateTime.Now-start;
                    // print(string.Format("t1={0},t2={1},t3={2},count={3}",t1.TotalMilliseconds,t2.TotalMilliseconds,t3.TotalMilliseconds,HitCount));
                
                }
                else{
                    // if(t2.TotalMilliseconds>0){
                    //     print(string.Format("t1={0},t2={1}",t1.TotalMilliseconds,t2.TotalMilliseconds));
                    // }
                    
                }
            }
            return gameObj;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("ModelSubScanner.HitTest Exception:"+ex);
            return null;
        }
        
    }

    public bool IsUpdatePoints=false;

    private List<Vector2> posList1=null;

    public Text LogText;

    public int ChangedCount = 0;

    public bool IsUpdating = true;

    public bool IsUpdateFinished=false;

    public void SetVisible(RendererInfo info,bool isV)
    {

    }

    public bool EnableHiddenModel=true;

    public bool IsHiddenGlobal=true;

    public Coroutine CheckHitC;


    [ContextMenu("CheckHit")]
    public IEnumerator CheckHit_Coroutine(){
        while(IsStop==false){
            if(IsPause){
                if(Manager.HiddenDelay==true && HiddenModelsAll.Count>0 && EnableHiddenModel){
                    
                    if(IsHiddenGlobal)
                        {
                            yield return Manager.HideModelsOfBuffer();//停止移动后隐藏模型
                        }
                    else{
                             yield return HideModelsOfBuffer();//停止移动后隐藏模型
                        }
                }
                
                yield return new WaitForSeconds(UpdateInterval);
            }
            else{

                IsUpdateFinished=false;

                IsPause=true;//需要外部处理
                var start = DateTime.Now;

                //if(IsDebug){
                //    settings.SetMaterial(settings.RedMaterial);
                //}
                //else{
                //    //settings.HideAll();
                //    settings.SetAllHiddenNew();
                //}

                // var sum=settings.SetAllHiddenNew();

                Manager.SetNewState(false);

                if (points!=null){
                    posList1=points.points;
                }
                else{
                    if(posList1==null||posList1.Count==0 || IsUpdatePoints==true)
                    {
                        posList1=GetPointsSimple();
                        IsUpdatePoints=false;
                    }
                }

                yield return ScanPoints(posList1);

                var t = (DateTime.Now - start).TotalMilliseconds;
                UpdateTimed_CheckHit=t;
                // Debug.Log("CheckHit 用时:"+t);
                // VisibleCount=VisibleModels.Count;
                // var vCount=GetStateCount(true);
                // HiddenCount= Manager.GetStateCount(false);
                // var log=string.Format("Count={0}\nVisible={1}\tHidden={2}\nChanged={3}\tHit={4}\nTime={5}\nUpdating={6}"
                // , AllHitCount, VisibleCount, HiddenCount, ChangedCount,HitCount, UpdateTimed_CheckHit, this.IsUpdating);
                // if (LogText != null)
                // {
                //     LogText.text = log;
                // }
                // else{
                //     print(log);
                // }

                

                yield return new WaitForSeconds(UpdateInterval);

                IsUpdateFinished=true;
            }
            
        }
        
    }

    public int VisibleCount=0;

    public int HiddenCount=0;

    //[ContextMenu("CheckHit")]
    //public void CheckHit(){

    //    int c=0;
    //    var start = DateTime.Now;

    //    HitObjects=new List<GameObject>();
    //    ScreenWidth=Screen.width;
    //    ScreenHeight=Screen.height;

    //    if(IsDebug){
    //        settings.SetMaterial(settings.RedMaterial);
    //    }
    //    else{
    //        settings.HideAll();
    //    }
        

        
    //    for(int i=0;i<ScreenWidth;i+=StepX){
    //        for(int j=0;j<ScreenHeight;j+=StepY){
    //            Vector2 pos=new Vector2(i,j);
    //            if(EnableCast){
    //                Ray ray = Camera.main.ScreenPointToRay(pos);//从摄像机发出到点击坐标的射线 
    //                RaycastHit hitInfo;
    //                if (Physics.Raycast(ray, out hitInfo))
    //                {
    //                    GameObject gameObj = hitInfo.collider.gameObject;
    //                    if(!HitObjects.Contains(gameObj)){
    //                        Debug.DrawLine(ray.origin, hitInfo.point);//划出射线，只有在scene视图中才能看到  
    //                        Debug.Log("click object name is " + gameObj.name);
    //                        HitObjects.Add(gameObj);

    //                        MeshRenderer renderer=gameObj.GetComponent<MeshRenderer>();
    //                        renderer.material=settings.GreenMaterial;
    //                    }
                        
    //                }
    //            }
    //            c++;
    //        }
    //    }
    //    Count=c;
    //    var t = (DateTime.Now - start).TotalMilliseconds;
    //    UpdateTimed_CheckHit=t;
    //    Debug.Log("CheckHit 用时:"+t);
    //}

    public Material LastClickMaterial;

    public MeshRenderer LastClickRenderer;

    public bool CheckClick = true;

    void Update()
    {
        // if (CheckClick&&Input.GetMouseButtonUp(0))
        // {
        //     // this.screenRect = new Rect(0, 0, Screen.width, Screen.height);
            
        //     print("Input.mousePosition:"+Input.mousePosition);
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线  
        //     RaycastHit hitInfo;
        //     if (Physics.Raycast(ray, out hitInfo))
        //     {
        //         Debug.DrawLine(ray.origin, hitInfo.point);//划出射线，只有在scene视图中才能看到  
        //         GameObject gameObj = hitInfo.collider.gameObject;
        //         Debug.Log("click object name is " + gameObj.name);
        //         //if (gameObj.tag == "boot")//当射线碰撞目标为boot类型的物品 ，执行拾取操作  
        //         //{
        //         //    Debug.Log("pick up!");
        //         //}

        //         MeshRenderer renderer=gameObj.GetComponent<MeshRenderer>();
        //         LastClickMaterial = renderer.material;
        //         renderer.material = settings.RedMaterial;

        //         if(LastClickRenderer!= renderer && LastClickRenderer!=null)
        //         {
        //             LastClickRenderer.material = LastClickMaterial;
        //             LastClickRenderer = renderer;
        //         }
        //     }
        // }
    }
}