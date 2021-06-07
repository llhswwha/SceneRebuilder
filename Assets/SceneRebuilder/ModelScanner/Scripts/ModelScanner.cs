using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
//using Y_UIFramework;

public class ModelScanner : MonoBehaviour
{
    public static ModelScanner Instance;

    public ModelManagerSettings settings;

    public Camera camera;
    public Transform cameraT;
    
    public Vector3 cameraPos;

    public Vector3 cameraForward;

    public List<ModelSubScanner> SubScanners=new List<ModelSubScanner>();
    public int ScanLevel=0;

 //   public void RegisterMsgListener(string messagType,MessageCenter.DelMessenger handler)
	//{
 //       MessageCenter.AddMsgListener(messagType, handler);
	//}

    void Awake(){
        
        if (camera == null)
        {
            camera = Camera.main;
        }
        Instance = this;
        InitScanLevel();

        //RegisterMsgListener(MsgType.LoadModelMsg.TypeName,obj =>
        //{
        //    if (obj.Key == MsgType.LoadModelMsg.OnBeforeLoadModel)
        //    {
        //        //StartSubScanners();
        //    }
        //    else if(obj.Key==MsgType.LoadModelMsg.OnAfterLoadModel)
        //    {
        //        StartSubScanners();
        //    }
        //});

        //RegisterMsgListener(MsgType.ModelScaneMsg.TypeName,obj =>
        //{
        //    if (obj.Key == MsgType.ModelScaneMsg.StartSubScanners)
        //    {
        //        StartSubScanners();
        //    }
        //});
    }

    public void InitScanLevel(int lv)
    {
        Debug.LogError("ModelScanner.InitScanLevel lv:"+lv);
        if (this.ScanLevel == lv) return;//避免后面的操作重复做
        this.ScanLevel = lv;
        InitScanLevel();

        //SetLastEnableHidden();
        RestartScan();
    }
    public void InitScanLevel()
    {
        if (ScanLevel > 0)
        {
            for (int i = 0; i < SubScanners.Count; i++)
            {
                if (i < ScanLevel)
                {
                    SubScanners[i].gameObject.SetActive(true);
                }
                else
                {
                    SubScanners[i].gameObject.SetActive(false);
                }
            }

            InitScans(); 
            SetLastEnableHidden();

           
        }

        SetCamera(camera);
    }

    void Start(){


        if (SubScanners.Count==0){
            SubScanners=this.GetComponentsInChildren<ModelSubScanner>().ToList();
        }


        InitScans();

        StartScan();

        //if(FPSMode.Instance){
        //    Debug.LogError("ModelScanner.Regist FPSStateChanged:");
        //    FPSMode.Instance.FPSStateChanged+= OnFPSStateChanged;
        //}
    }

    private void OnFPSStateChanged(bool isOn){
        Debug.LogError("ModelScanner.OnFPSStateChanged:"+isOn+"|"+camera);
        camera=Camera.main;
        cameraT=camera.transform;
        cameraPos=camera.transform.position;
        StartSubScanners();

        Debug.LogError("camera:"+camera);
    }

    private void OnDestroyed(){
        //if(FPSMode.Instance){
        //    FPSMode.Instance.FPSStateChanged-= OnFPSStateChanged;
        //}
    }

    private void InitScans()
    {
        ModelSubScanner last = null;

        foreach (ModelSubScanner item in SubScanners)
        {
            if (item.gameObject.activeSelf == false) continue;
            item.settings = this.settings;
            item.Manager = this;
            // item.EnableHiddenModel=false;
            last = item;
        }
        // if(last!=null)
        //     last.EnableHiddenModel=true;
    }

    public void SetLastEnableHidden()
    {
        ModelSubScanner last = null;

        foreach (ModelSubScanner item in SubScanners)
        {
            if (item.gameObject.activeSelf == false) continue;
            item.settings = this.settings;
            item.Manager = this;
            item.EnableHiddenModel=false;
            last = item;
        }
        if(last!=null)
            last.EnableHiddenModel=true;
    }


    public float UpdateInterval = 0.2f;

    public bool HiddenDelay = true;//延迟隐藏

    public bool HiddenCheck = true;

    public float CastDistance = 0;

    public bool IsStop = false;

    public bool IsPause = false;

    public int UpdateSize2 = 300;

    public bool EnableHidden=true;


    [ContextMenu("CheckHit_Hidden")]
    public IEnumerator CheckHit_Hidden()
    {
        while (IsStop == false)
        {
            if (IsAllFinished() && EnableHidden)
            {
                // yield return HideModelsOfBuffer();

                yield return new WaitForSeconds(UpdateInterval);
            }
            else
            {
                 yield return new WaitForSeconds(UpdateInterval);
            }
        }
    }

    public List<RendererInfo> CollectVisibleModels()
    {
        var vAll = new List<RendererInfo>();
        for (int i = 0; i < SubScanners.Count; i++)
        {
            var scanner = SubScanners[i];
            if(scanner.gameObject.activeSelf==false)continue;
            scanner.CollectVisibleModels(vAll);
        }
        return vAll;
    }

    public List<RendererInfo> CollectHiddenModels()
    {
        var hAll = new List<RendererInfo>();
        for (int i = 0; i < SubScanners.Count; i++)
        {
            var scanner = SubScanners[i];
            if(scanner.gameObject.activeSelf==false)continue;
            scanner.CollectHiddenModels(hAll);
        }
        return hAll;
    }

    private void ClearHiddenModels()
    {
        for (int i = 0; i < SubScanners.Count; i++)
        {
            var scanner = SubScanners[i];
            if(scanner.gameObject.activeSelf==false)continue;
            scanner.HiddenModelsAll.Clear();
        }
    }
    
    public List<RendererInfo> VisibleModelsAll = new List<RendererInfo>();

    public bool Raycast(Ray ray, out RaycastHit hitInfo)
    {
        bool isHit = false;
        if (settings.ScanLayer > 0)
        {
            if (this.CastDistance > 0)
            {
                isHit = Physics.Raycast(ray, out hitInfo, this.CastDistance, settings.ScanLayerMask.value);
            }
            else
            {
                isHit = Physics.Raycast(ray, out hitInfo, 3000, settings.ScanLayerMask.value);
            }
        }
        else
        {
            if (this.CastDistance > 0)
            {
                isHit = Physics.Raycast(ray, out hitInfo, this.CastDistance);
            }
            else
            {
                isHit = Physics.Raycast(ray, out hitInfo);
            }
        }

        return isHit;
    }

    public float MaxAngle=60;

    public IEnumerator HideModelsOfBuffer()
    {
        var start = DateTime.Now;

        var vAll = CollectVisibleModels();

        var HiddenModelsAll = CollectHiddenModels();

        //Debug.Log(string.Format("HiddenModelsAll={0};vAll={1}", HiddenModelsAll.Count,vAll.Count));

        if(HiddenModelsAll.Count>0){
            var hAll = new List<RendererInfo>();
            for (int j = 0; j < HiddenModelsAll.Count; j++)
            {
                var info = HiddenModelsAll[j];
                if (!vAll.Contains(info))
                {
                    hAll.Add(info);
                }
            }
            //Debug.Log(string.Format("hAll={0}", hAll.Count));
            ClearHiddenModels();

            int cc = 0;
            hAll.Clear();
            foreach (var info in modelsDict.Values)
            {
                if (info.visible && !vAll.Contains(info))
                {
                    //info.SetVisible(false);
                    hAll.Add(info);
                }
            }

            VisibleModelsAll = vAll;

            VisibleCount = vAll.Count;
            HiddenCount = hAll.Count;
            if (hAll.Count > 0)
            {
                int j = 0;
                for (int i = 0; i < hAll.Count; i++)
                {
                    bool isHidden = false;
                    var info = hAll[i];
                    if (info.renderer == ModelSelection.LastClickRenderer) continue;//当前点击的那个不能隐藏

                    if (HiddenCheck)
                    {
                        //var dis = Vector3.Distance(cameraPos, info.position);
                        var angle = Vector3.Angle(cameraForward, (info.position - cameraPos));

                        //if (dis < info.fullDiameter && angle< 90)
                        if (angle < MaxAngle)
                        {
                            Ray ray = new Ray(cameraPos, info.position - cameraPos);
                            RaycastHit hitInfo;
                            bool isHit = Raycast(ray, out hitInfo);
                            if (isHit == false || hitInfo.collider.gameObject != info.gameObject)
                            {
                                isHidden = true;
                            }
                        }
                        else
                        {
                            isHidden = true;
                        }
                    }
                    else
                    {
                        isHidden = true;
                    }
                    if (isHidden)
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

            var time = (DateTime.Now - start).TotalMilliseconds;
            Debug.Log(string.Format("VisibleCount={0},HiddenCount={1},Time={2}", VisibleCount, HiddenCount, time));

            VisibleCount = GetStateCount(true);
            HiddenCount = GetStateCount(false);
        }
        
        yield return null;
    }

    private void SetCamera(Camera c){
        camera=c;
        cameraT=c.transform;
        foreach(ModelSubScanner item in SubScanners)
        {
            if(item.gameObject.activeSelf==false)continue;
            //item.camera=c;
        }
    }

    public Coroutine CheckHitC;

    public void StartScan(){
        foreach(ModelSubScanner item in SubScanners)
        {
            if(item.gameObject.activeSelf==false)continue;
            item.CheckHitC=StartCoroutine(item.CheckHit_Coroutine());
        }

        CheckHitC=StartCoroutine(CheckHit_Hidden());
    }

    public void StopScan()
    {
        try
        {
            foreach (ModelSubScanner item in SubScanners)
            {
                if (item.gameObject.activeSelf == false) continue;
                if(item.CheckHitC!=null)
                    StopCoroutine(item.CheckHitC);
            }
            if (CheckHitC != null)
            {
                StopCoroutine(CheckHitC);
            }
        }catch(Exception ex){
            Debug.LogError("ModelScanner.StopScan Exception:"+ex);
        }
        
    }

    public void ClearModelBufferInfos()
    {
        foreach (ModelSubScanner item in SubScanners)
        {
            if (item.gameObject.activeSelf == false) continue;
            item.ClearModelBufferInfos();
        }
        
    }

    public void RestartScan()
    {
        StopScan();

        ClearModelBufferInfos();
        SetVisible(false);

        StartSubScanners();

        StartScan();
    }

    public bool EnableScanPoints=true;

    public int ScanTime=0;

    public int HitCount=0;

    public bool Debug_OnlyHit=false;

    public bool IsUpdatePoints=false;

    private List<Vector2> posList1=null;

    public Text LogText;

    public int ChangedCount = 0;

    public bool IsUpdating = true;

    public void SetVisible(RendererInfo info,bool isV)
    {

    }



    //public bool CheckClick = true;

    void Update()
    {
        //MouseClick();

        UpdateCameraInfo();

        ShowLog();

        if (IsAllFinished())
        {

        }
    }

    public bool IsAllFinished()
    {
        for (int i = 0; i < SubScanners.Count; i++)
        {
            var scanner = SubScanners[i];
            if(scanner.gameObject.activeSelf==false)continue;
            if (scanner.IsUpdateFinished == false) return false;
        }
        return true;
    }

    private void UpdateCameraInfo(){
        if(cameraT.position!=cameraPos || cameraT.forward!=cameraForward)
        {

            // IsCameraChanged=true;
            StartSubScanners();
            // UpdateCameraPos();
        }
        else{
            
        }
    }

    // public string TransparentLayer="TransparentFX";

    public int VisibleCount=0;

    public int HiddenCount=0;

    public double UpdateTimed_CheckHit;

    public int AllHitCount = 0;

    private Dictionary<GameObject, RendererInfo> modelsDict = new Dictionary<GameObject, RendererInfo>();

    public RendererInfo GetRendererInfo(GameObject obj)
    {
        if (modelsDict.ContainsKey(obj))
        {
            return modelsDict[obj];
        }
        else
        {
            RendererInfo info = new RendererInfo(obj);
            modelsDict.Add(obj, info);
            AllHitCount = modelsDict.Count();
            return info;
        }
    }

    public int GetStateCount(bool isV)
    {
        int sum = 0;
        foreach (var info in modelsDict.Values)
        {
            if (info.visible == isV)
            {
                sum++;
            }
        }
        return sum;
    }

    public void SetNewState(bool isV)
    {
        foreach (var info in modelsDict.Values)
        {
            info.visibleNew = isV;
        }
    }

    public void SetVisible(bool isV)
    {
        foreach (var info in modelsDict.Values)
        {
            info.SetVisible(isV);
        }
    }

    private void ShowLog(){

        AllHitCount=modelsDict.Count;
        VisibleCount=0;
        HiddenCount=0;
        ChangedCount=0;
        HitCount=0;
        UpdateTimed_CheckHit=0;
        IsUpdating=false;

        VisibleCount = this.GetStateCount(true);
        HiddenCount = this.GetStateCount(false);

        for (int i = 0; i < SubScanners.Count; i++)
        {
            var scanner = SubScanners[i];
            if(scanner.gameObject.activeSelf==false)continue;
            //AllHitCount+=scanner.AllHitCount;
            //VisibleCount+=scanner.VisibleCount;
            //HiddenCount+=scanner.HiddenCount;
            ChangedCount+=scanner.ChangedCount;
            HitCount+=scanner.HitCount;
            UpdateTimed_CheckHit+=scanner.UpdateTimed_CheckHit;
        }
        IsUpdating = !IsAllFinished();
        var log=string.Format("Count={0}\nVisible={1}\tHidden={2}\nChanged={3}\tHit={4}\nTime={5}\nUpdating={6}"
                , AllHitCount, VisibleCount, HiddenCount, ChangedCount,HitCount, UpdateTimed_CheckHit, IsUpdating);
        if(LogText){
            LogText.text=log;
        }
    }


    [ContextMenu("StartSubScanners")]   
    public void StartSubScanners()
    {
        cameraPos = cameraT.position;
        cameraForward = cameraT.forward;

        for (int i = 0; i < SubScanners.Count; i++)
        {
            var scanner = SubScanners[i];
            if(scanner.gameObject.activeSelf==false)continue;
            scanner.IsPause=false;
            scanner.IsBreak=true;
            scanner.IsUpdateFinished=false;
            //scanner.cameraPos=this.cameraPos;
        }
    }
}