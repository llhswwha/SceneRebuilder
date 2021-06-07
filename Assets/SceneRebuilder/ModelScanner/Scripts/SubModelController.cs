using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SubModelController : MonoBehaviour
{
    //静态
    public ModelManager Manager;
    public Transform cameraT;
    public ModelLevelSettingInfo levelInfo;
    public bool CheckRaycast = false;
    public List<RendererInfo> rendererInfos = new List<RendererInfo>();
    public List<ModelDisInfo> models=new List<ModelDisInfo>();
    public int AllCount = 0;

    //动态
    public int VisibleCount = 0;
    public int HiddenCount = 0;
    public int ChangedCount = 0;

    public int CurrentId = 0;
    public double UpdateTime = 0;
    public double ThreadUpdateTime = 0;
    public int GroupCount = 0;

    public List<RendererInfo> VisibleRenderers = new List<RendererInfo>();

    // Start is called before the first frame update

    public void Init()
    {
        AllCount = rendererInfos.Count;
        gameObject.name += "_" + AllCount;
    }
    void Start()
    {
        StartUpdate();
    }

    public bool IsMultiThread = false;

    [ContextMenu("StartUpdate")]
    public void StartUpdate(){
        Init();

        if (settings == null)
            {
                settings = GameObject.FindObjectOfType<ModelManagerSettings>();
            }

        if (IsMultiThread == false)
        {
            StartCoroutine(UpdateRenderer());
            //StartCoroutine(UpdateRaycast());
        }
        else
        {
            StartCoroutine(UpdateRenderersVisibleNew());
            thread = new Thread(UpdateRenderersThreadCore);
            thread.Start();

            
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        try
        {
            StopCoroutine("UpdateRenderer");
            if (thread!=null)
            {
                thread.Abort();
                thread = null;
            }
            
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    internal void SetLayer(int l, int tl)
    {
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            if (item == null)
            {
                Debug.LogError("item == null:"+i);
                continue;
            }
            if (item.renderer == null)
            {
                Debug.LogError("item.renderer == null:" + item.gameObject);
                continue;
            }
            if (item.renderer.sharedMaterial == null)
            {
                Debug.LogError("item.renderer.sharedMaterial == null:" + item.gameObject);
                continue;
            }
            try{
                Color color=Color.black;
                if(item.renderer.sharedMaterial.HasProperty("_Color"))
                {
                    color = item.renderer.sharedMaterial.color;
                }
                else if(item.renderer.sharedMaterial.HasProperty("_BaseColor"))
                {
                    color = item.renderer.sharedMaterial.GetColor("_BaseColor");
                }
                else{
                    Debug.LogError("item.renderer.sharedMaterial.color ==null " + item.gameObject);
                    item.gameObject.layer = l;
                    continue;
                }

                if(color.a < 1)
                {
                    item.gameObject.layer = tl;
                }
                else
                {
                    item.gameObject.layer = l;
                }
            }catch(Exception ex){
                 Debug.LogError("item.renderer.sharedMaterial Exception:" + item.gameObject+"|"+ex);
            }
            
        }
    }

    internal void ShowNotInLayer(int l)
    {
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            if (item.gameObject.layer != l)
            {
                item.renderer.enabled = true;
            }
        }
    }

    public bool StopUpdate = false;

    public bool UpdateEnbale = true;


    IEnumerator UpdateRenderer()
    {
        Debug.Log("UpdateRenderer Start:" + this);
        while (StopUpdate == false)
        {
            if (UpdateEnbale == false)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return UpdateRenderersEx();
                UpdateEnbale=false;//需要ModelManager配合使用，不然就不用
            }

            //Debug.Log("UpdateRenderer Update:" + this);
        }
    }

    public void SetLevelItemsVisible(List<RendererInfo> items, bool v)
    {
        foreach (var item in items)
        {
            item.SetVisible(v);
        }
    }

    private IEnumerator UpdateRenderersEx()
    {
        //VisibleRenderers.Clear();
        yield return UpdateRenderers(rendererInfos, true);
        //SetLevelItemsVisible(rendererInfos, false);
        //if (MaxNodeCount > 0 && VisibleRenderers.Count > MaxNodeCount)
        //{
        //    VisibleRenderers.Sort();
        //    for (int i = 0; i < MaxNodeCount; i++)
        //    {
        //        VisibleRenderers[i].enabled = true;
        //    }
        //}
        //else
        //{
        //    SetLevelItemsVisible(VisibleRenderers, true);
        //}
        yield return new WaitForSeconds(levelInfo.updateInterval);
    }

    Thread thread=null;
    public List<DistanceInfo> disInfoList=new List<DistanceInfo>();

    [ContextMenu("InitDistanceInfoList")]
    public void InitDistanceInfoList()
    {
        disInfoList.Clear();
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            DistanceInfo disInfo = new DistanceInfo();
            disInfo.info = item;
            disInfo.objPos=item.transform.position;
            // disInfo.camPos=cameraT.position;
            disInfoList.Add(disInfo);
        }
    }


    // [ContextMenu("UpdateRenderersInEditor")]
    // public void UpdateRenderersInEditor(){
    //     UpdateRenderersInner(rendererInfos, true);
    // }

    
    // public void UpdateRenderersInner(List<RendererInfo> rs, bool setEnabled)
    // {
    //     //Debug.Log("UpdateRenderers rs:" + rs.Count+"|"+this);
    //     var start = DateTime.Now;
    //     //int c1 = 0;
    //     //int c2 = 0;
    //     float sumDis = 0;
    //     VisibleRenderers.Clear();
    //     GroupCount = 0;
    //     var changedCount = 0;
    //     var hiddenCount = 0;
    //     var visibleCount = 0;
    //     CurrentId = 0;
    //     // if(disInfoList.Count!=rs.Count)
    //     // {
    //     //     InitDistanceInfoList();
    //     // }
        
    //     for(int i=0 ;i<rs.Count;i++)
    //     {
    //         var item=rs[i];
    //         if(Manager.IsAddScript){
    //             var model=models[i];
    //             item=model.info;
    //         }
    //         if (Manager.Debug_1_DoNone)continue;
    //         // if(disInfoList.Count==rs.Count)
    //         // {
    //         //     //DistanceInfo disInfo=disInfoList[i];
    //         //     disInfoList[i].objPos=item.transform.position;
    //         //     disInfoList[i].camPos=cameraT.position;
    //         // }

    //         // Debug.Log(string.Format("objPos :{0}| camPos:{1}", item.transform.position, cameraT.position));

    //         //disInfoList[i].SetPos(item.transform.position, cameraT.position);

    //         if (Manager.Debug_2_Distance)continue;

    //         float dis = Vector3.Distance(item.transform.position, cameraT.position);
    //         if(Manager.Debug_3_KeepHidden)continue;
    //         item.distance = dis;
    //         item.fullDistance = dis + item.diameter;
    //         ////
    //         bool isVisible = false;
    //         if (Manager.DisMode == DistanceMode.Absolute)
    //         {
    //             isVisible = item.fullDistance < Manager.MinDis;
    //         }
    //         if (Manager.DisMode == DistanceMode.Relative)
    //         {
    //             isVisible = dis < item.fullDiameter;
    //         }
    //         //Debug.Log(string.Format("isVisible1 {0}| {1}", isVisible, item));

    //         if (Manager.CheckAngle)
    //         {
    //             var angle = Vector3.Angle(cameraForward, (item.transform.position - cameraT.position));
    //             //if (screenRect.Contains(camera.WorldToScreenPoint(item.transform.position)))
    //             if (angle >= levelInfo.CameraMaxAngle)
    //             {
    //                 isVisible = false;
    //             }
    //             item.angle = angle;
    //         }
    //         //Debug.Log(string.Format("isVisible2 {0}| {1}", isVisible, item));
    //         if (isVisible)
    //         {
    //             visibleCount++;
    //             VisibleRenderers.Add(item);
    //         }
    //         else
    //         {
    //             hiddenCount++;
    //         }
    //         if (setEnabled) item.SetVisible(isVisible);
    //         sumDis += dis;
    //     }
    //     this.ChangedCount=changedCount;
    //     this.HiddenCount=hiddenCount;
    //     this.VisibleCount=visibleCount;
    //     UpdateTime = (DateTime.Now - start).TotalMilliseconds;
    // }

    [ContextMenu("UpdateModelPos")] //场景中的模型位置变了动画必须手动更新
    public void UpdateModelPos()
    {
        // var start = DateTime.Now;
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            if(Manager.IsAddScript){
                var model=models[i];
                item=model.info;
            }
            item.position=item.transform.position;
        }
        // var t = (DateTime.Now - start).TotalMilliseconds;
        // Debug.Log("UpdateModelPos 用时:"+t);
    }

    [ContextMenu("ShowAll")] //场景中的模型位置变了动画必须手动更新
    public void ShowAll()
    {
        // var start = DateTime.Now;
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            if(Manager.IsAddScript){
                var model=models[i];
                item=model.info;
            }
            item.SetVisible(true);
        }
        // var t = (DateTime.Now - start).TotalMilliseconds;
        // Debug.Log("UpdateModelPos 用时:"+t);
    }

    [ContextMenu("InitState")] //场景中的模型位置变了动画必须手动更新
    public void InitState()
    {
        // var start = DateTime.Now;
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            if(Manager.IsAddScript){
                var model=models[i];
                item=model.info;
            }
            if(levelInfo.IsStatic){
                item.gameObject.tag="ScanStatic";
            }
            item.InitState();
        }
        // var t = (DateTime.Now - start).TotalMilliseconds;
        // Debug.Log("UpdateModelPos 用时:"+t);
    }

    public void InitState(int l)
    {
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            if(Manager.IsAddScript){
                var model=models[i];
                item=model.info;
            }
            if(Manager.IsClearStatic){
                item.gameObject.tag="Default";
            }
            if(levelInfo.IsStatic && item.VertexCount < Manager.MaxVertexCount){
                item.gameObject.tag="ScanStatic";
            }
            else{

            }
            item.InitState(l);
        }
    }

    public List<RendererInfo> StaticRenderInfos=new List<RendererInfo>();

    public int StaticCount=0;

    public int GetStaticCount(List<GameObject> staticModels){
        StaticRenderInfos=new List<RendererInfo>();
        StaticCount=0;
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            if(Manager.IsAddScript){
                var model=models[i];
                item=model.info;
            }
            if(item.gameObject.tag=="ScanStatic")
            {
                StaticCount++;
                StaticRenderInfos.Add(item);
                staticModels.Add(item.gameObject);
            }
        }
        return StaticCount;
    }

    [ContextMenu("HideAll")] //场景中的模型位置变了动画必须手动更新
    public void HideAll()
    {
        // var start = DateTime.Now;
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            if(Manager.IsAddScript){
                var model=models[i];
                item=model.info;
            }
            item.SetVisible(false);
        }
        // var t = (DateTime.Now - start).TotalMilliseconds;
        // Debug.Log("UpdateModelPos 用时:"+t);
    }

    [ContextMenu("SetAllHiddenNew")] //场景中的模型位置变了动画必须手动更新
    public int SetAllHiddenNew()
    {
        // var start = DateTime.Now;
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            if (Manager.IsAddScript)
            {
                var model = models[i];
                item = model.info;
            }
            item.visibleNew = false;
        }
        // var t = (DateTime.Now - start).TotalMilliseconds;
        // Debug.Log("UpdateModelPos 用时:"+t);
        return rendererInfos.Count;
    }

    public int GetStateCount(bool state)
    {
        int sum=0;
        // var start = DateTime.Now;
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            if (Manager.IsAddScript)
            {
                var model = models[i];
                item = model.info;
            }
            if(item.visible == state){
                sum++;
            }
        }
        // var t = (DateTime.Now - start).TotalMilliseconds;
        // Debug.Log("UpdateModelPos 用时:"+t);
        return sum;
    }

    public int GetCount(){
        return rendererInfos.Count;
    }

    [ContextMenu("AddMeshCollider")] //场景中的模型位置变了动画必须手动更新
    public void AddMeshCollider()
    {
        // var start = DateTime.Now;
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            item.AddMeshCollider();
        }
        // var t = (DateTime.Now - start).TotalMilliseconds;
        // Debug.Log("UpdateModelPos 用时:"+t);
    }

    [ContextMenu("RemoveScripts")]
    public void RemoveScripts()
    {
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            item.RemoveScripts();
        }
    }

    [ContextMenu("RemoveCollider")]
    public void RemoveCollider()
    {
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            item.RemoveCollider();
        }
    }

    [ContextMenu("DisableCollider")]
    public void DisableCollider()
    {
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            item.DisableCollider();
        }
    }

     [ContextMenu("EnableCollider")]
    public void EnableCollider()
    {
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            item.EnableCollider();
        }
    }

     [ContextMenu("DisactiveObjects")]
    public void DisactiveObjects()
    {
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            item.gameObject.SetActive(false);
        }
    }
    
    
    [ContextMenu("SetColorRed")] //场景中的模型位置变了动画必须手动更新
    public void SetColorRed()
    {
        if (settings == null)
        {
            settings = GameObject.FindObjectOfType<ModelManagerSettings>();
        }
        // var start = DateTime.Now;
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            if(settings){ //使用颜色区分是否显示
               item.renderer.material=settings.RedMaterial;
            }
        }
        // var t = (DateTime.Now - start).TotalMilliseconds;
        // Debug.Log("UpdateModelPos 用时:"+t);
    }

    public void SetMaterial(Material mat)
    {
        if (settings == null)
        {
            settings = GameObject.FindObjectOfType<ModelManagerSettings>();
        }
        // var start = DateTime.Now;
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item = rendererInfos[i];
            if(settings){ //使用颜色区分是否显示
               item.renderer.material=mat;
            }
        }
        // var t = (DateTime.Now - start).TotalMilliseconds;
        // Debug.Log("UpdateModelPos 用时:"+t);
    }

    public GameObject DistanceSphereRoot;
    public float TestPower=10;

    [ContextMenu("SetPower")] //场景中的模型位置变了动画必须手动更新
    public void SetPower()
    {
        var start = DateTime.Now;
        for (int i = 0; i < rendererInfos.Count; i++)
        {
            var item1 = rendererInfos[i];
            item1.SetPower(TestPower);
            if(Manager.IsAddScript){
                var model=models[i];
                var item2=model.info;
                item2.SetPower(TestPower);
            }
        }
        var t = (DateTime.Now - start).TotalMilliseconds;
        Debug.Log("CreateSphere 用时:"+t);
    }

    [ContextMenu("CreateSphere")] //场景中的模型位置变了动画必须手动更新
    public void CreateSphere()
    {
        if(DistanceSphereRoot==null){
            DistanceSphereRoot=new GameObject("DistanceSphereRoot");
            DistanceSphereRoot.transform.SetParent(this.transform);
        }
        var start = DateTime.Now;
        for (int i = 0; i < models.Count; i++)
        {
            var model=models[i];
            GameObject sphere=model.CreateSphere();
            sphere.transform.SetParent(DistanceSphereRoot.transform);
        }
        var t = (DateTime.Now - start).TotalMilliseconds;
        Debug.Log("CreateSphere 用时:"+t);
    }
    

    [ContextMenu("CreateSphereEx")] //场景中的模型位置变了动画必须手动更新
    public void CreateSphereEx()
    {
        if(DistanceSphereRoot==null){
            DistanceSphereRoot=new GameObject("DistanceSphereRoot");
            DistanceSphereRoot.transform.SetParent(this.transform);
        }
        var start = DateTime.Now;
        for (int i = 0; i < models.Count; i++)
        {
            var model=models[i];
            model.TestPower=TestPower;
            GameObject sphere=model.CreateSphereEx();
            sphere.transform.SetParent(DistanceSphereRoot.transform);
        }
        var t = (DateTime.Now - start).TotalMilliseconds;
        Debug.Log("CreateSphere 用时:"+t);
    }

    [ContextMenu("CreateSphereFull")] //场景中的模型位置变了动画必须手动更新
    public void CreateSphereFull()
    {
        if(DistanceSphereRoot==null){
            DistanceSphereRoot=new GameObject("DistanceSphereRoot");
            DistanceSphereRoot.transform.SetParent(this.transform);
        }
        var start = DateTime.Now;
        for (int i = 0; i < models.Count; i++)
        {
            var model=models[i];
            GameObject sphere=model.CreateSphereFull();
            sphere.transform.SetParent(DistanceSphereRoot.transform);
        }
        var t = (DateTime.Now - start).TotalMilliseconds;
        Debug.Log("CreateSphereFull 用时:"+t);
    }

    public Vector3 cameraPos;

    public Vector3 cameraForward;
    
    public ModelManagerSettings settings;

    IEnumerator UpdateRenderersVisibleNew()
    {
        Debug.Log("UpdateRenderer Start:" + this);
        while (StopUpdate == false)
        {
            if (UpdateEnbale == false)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                var start = DateTime.Now;

                var rs =new List<RendererInfo>(ChangedItems);
                CurrentId = 0;
                for (int i = 0; i < rs.Count; i++)
                {
                    var item = rs[i];
                    if (Manager.IsAddScript)
                    {
                        var model = models[i];
                        item = model.info;
                    }
                    item.SetVisible(item.visibleNew);

                    CurrentId++;
                    if (Manager.GroupSize > 0 && CurrentId > Manager.GroupSize)
                    {
                        GroupCount++;
                        CurrentId = 0;
                        yield return null;
                    }
                }
                UpdateEnbale = false;//需要ModelManager配合使用，不然就不用

                this.UpdateTime = (DateTime.Now - start).TotalMilliseconds;
            }

            //Debug.Log("UpdateRenderer Update:" + this);

            yield return new WaitForSeconds(0.5f);
        }
    }

    public bool IsThreadUpdating = false;

    private List<RendererInfo> ChangedItems = new List<RendererInfo>();

    public void UpdateRenderersThreadCore()
    {
        while (true)
        {
            try
            {
                if (UpdateEnbale == false)
                {
                    Thread.Sleep(500);
                }
                else
                {
                     List<RendererInfo> rs = rendererInfos;
                    // Debug.Log("UpdateRenderers rs:" + rs.Count + "|" + this);
                    var start = DateTime.Now;
                    //int c1 = 0;
                    //int c2 = 0;
                    float sumDis = 0;
                    VisibleRenderers.Clear();
                    GroupCount = 0;
                    var changedCount = 0;
                    List<RendererInfo> itemsNew = new List<RendererInfo>();
                    var hiddenCount = 0;
                    var visibleCount = 0;
                    CurrentId = 0;
                    // if(disInfoList.Count!=rs.Count)
                    // {
                    //     InitDistanceInfoList();
                    // }

                    for (int i = 0; i < rs.Count; i++)
                    {
                        var item = rs[i];
                        if (Manager.IsAddScript)
                        {
                            var model = models[i];
                            item = model.info;
                        }
                        float dis = Vector3.Distance(item.position, cameraPos);
                        item.distance = dis;
                        bool isVisible = false;
                        if (Manager.DisMode == DistanceMode.Absolute)
                        {
                            item.fullDistance = dis + item.diameter;
                            isVisible = item.fullDistance < Manager.MinDis;
                        }
                        if (Manager.DisMode == DistanceMode.Relative)
                        {
                            isVisible = dis < item.fullDiameter;
                        }
                        //Debug.Log(string.Format("isVisible1 {0}| {1}", isVisible, item));

                        if (isVisible != item.visible) //发生变化的部分
                        {
                            changedCount++;
                            itemsNew.Add(item);
                        }
                        if (isVisible && Manager.CheckAngle)
                        {
                            var angle = Vector3.Angle(cameraForward, (item.position - cameraPos));
                            //if (screenRect.Contains(camera.WorldToScreenPoint(item.transform.position)))
                            if (angle >= levelInfo.CameraMaxAngle)
                            {
                                isVisible = false;
                            }
                            item.angle = angle;

                            // if (isVisible && this.CheckRaycast)
                            // {
                            //     item.isHideByCollider=false;
                            //     item.hitObj=null;
                            //     Ray ray = new Ray(cameraPos, (item.position - cameraPos));
                            //     RaycastHit hit;
                            //     if(Physics.Raycast(ray, out hit))
                            //     {
                            //         if (hit.transform != item.transform)
                            //         {
                            //             isVisible = false;
                            //             item.isHideByCollider = true;
                            //         }
                            //         else{

                            //         }
                            //         item.hitObj=hit.transform;

                            //         print("hit:"+item.gameObject);
                            //     }
                            //     else{
                            //         print("no hit:"+item.gameObject);
                            //     }
                            // }
                        }
                        //Debug.Log(string.Format("isVisible2 {0}| {1}", isVisible, item));
                        if (isVisible)
                        {
                            visibleCount++;
                            VisibleRenderers.Add(item);
                        }
                        else
                        {
                            hiddenCount++;
                        }
                        item.visibleNew = isVisible;
                        sumDis += dis;
                    }
                    this.ChangedCount = changedCount;
                    this.HiddenCount = hiddenCount;
                    this.VisibleCount = visibleCount;
                    

                    if (Manager.IsAddScript)
                    {
                        models.Sort();
                    }
                    else
                    {
                        rs.Sort();
                    }
                    ChangedItems = itemsNew;

                    this.ThreadUpdateTime = (DateTime.Now - start).TotalMilliseconds;
                    IsThreadUpdating = false;
                    Thread.Sleep(100);
                }


            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                Thread.Sleep(1000);
            }
        }
    }

    public IEnumerator UpdateRenderers(List<RendererInfo> rs, bool setEnabled)
    {
        if(settings==null){
            settings=GameObject.FindObjectOfType<ModelManagerSettings>();
        }
        
        //Debug.Log("UpdateRenderers rs:" + rs.Count+"|"+this);
        var start = DateTime.Now;
        //int c1 = 0;
        //int c2 = 0;
        float sumDis = 0;
        VisibleRenderers.Clear();
        GroupCount = 0;
        var changedCount = 0;
        var hiddenCount = 0;
        var visibleCount = 0;
        CurrentId = 0;
        // if(disInfoList.Count!=rs.Count)
        // {
        //     InitDistanceInfoList();
        // }
        
        for(int i=0 ;i<rs.Count;i++)
        {
            var item=rs[i];
            if(Manager.IsAddScript){
                var model=models[i];
                item=model.info;
            }

            // if (Manager.Debug_1_DoNone)continue;
            // if(disInfoList.Count==rs.Count)
            // {
            //     //DistanceInfo disInfo=disInfoList[i];
            //     disInfoList[i].objPos=item.transform.position;
            //     // disInfoList[i].camPos=cameraT.position;
            // }

            // Debug.Log(string.Format("objPos :{0}| camPos:{1}", item.transform.position, cameraT.position));

            //disInfoList[i].SetPos(item.transform.position, cameraT.position);

            // if (Manager.Debug_2_Distance)continue;

            float dis = Vector3.Distance(item.position, cameraPos);
            // if(Manager.Debug_3_KeepHidden)continue;

            item.distance = dis;
            bool isVisible = false;
            if (Manager.DisMode == DistanceMode.Absolute)
            {
                item.fullDistance = dis + item.diameter;
                isVisible = item.fullDistance < Manager.MinDis;
            }
            if (Manager.DisMode == DistanceMode.Relative)
            {
                isVisible = dis < item.fullDiameter;
            }
            //Debug.Log(string.Format("isVisible1 {0}| {1}", isVisible, item));

            if (isVisible != item.visible) //发生变化的部分
            {
                changedCount++;
            }

            CurrentId++;
            if (Manager.GroupSize>0 && CurrentId > Manager.GroupSize)
            {
                GroupCount++;
                CurrentId = 0;
                yield return null;
            }

            if (isVisible && Manager.CheckAngle)
            {
                Vector3 direction = item.position - cameraPos;
                var angle = Vector3.Angle(cameraForward, direction);
                //if (screenRect.Contains(camera.WorldToScreenPoint(item.transform.position)))
                if (angle >= levelInfo.CameraMaxAngle)
                {
                    isVisible = false;
                }
                item.angle = angle;

                if (isVisible && this.CheckRaycast)
                {
                    item.isHideByCollider=false;
                    item.hitObj=null;
                    Ray ray = new Ray(cameraPos, direction);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform != item.transform)
                        {
                            isVisible = false;
                            item.isHideByCollider = true;
                        }
                        else{

                        }
                        item.hitObj=hit.transform;

                        print("hit:"+item.gameObject);
                    }
                    else{
                        print("no hit:"+item.gameObject);
                    }
                }
            }
            //Debug.Log(string.Format("isVisible2 {0}| {1}", isVisible, item));
            if (isVisible)
            {
                visibleCount++;
                VisibleRenderers.Add(item);
            }
            else
            {
                hiddenCount++;
            }
            if (setEnabled) {
                if(settings&&settings.TestDistance){ //使用颜色区分是否显示
                    
                    if(isVisible){
                        item.renderer.material=settings.TransparentMaterial;
                    }
                    else
                    {
                        if (item.isHideByCollider)
                        {
                            item.renderer.material = settings.GreenMaterial;
                        }
                        else
                        {
                            item.renderer.material = settings.BlueMaterial;
                        }
                        
                    }
                    item.visible=isVisible;
                    item.Show();
                }
                else
                {
                    item.SetVisible(isVisible);
                }
                
                // item.SetVisible(isVisible);
            }
            sumDis += dis;
        }
        this.ChangedCount=changedCount;
        this.HiddenCount=hiddenCount;
        this.VisibleCount=visibleCount;
        UpdateTime = (DateTime.Now - start).TotalMilliseconds;
        yield return null;
    }
}
