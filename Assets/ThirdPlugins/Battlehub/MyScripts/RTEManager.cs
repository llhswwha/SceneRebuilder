using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTCommon;
using Battlehub.RTEditor;
using Battlehub.RTEditor.HDRP;
using Mogoson.CameraExtension;
using Battlehub.RTHandles;
using System;
using UnityEngine.UI;
using System.Linq;
// using Unity.RenderStreaming;

public class RTEManager : MonoBehaviour
{
    public static RTEManager Instance;

    public HDRPInit hdrpInit;

    public bool IsToolBarVisible=false;

    public bool IsLockCamera=false;

    public GameObject EditorExtensions;

    public static Action<List<GameObject>> OnSelectionEvent;

    public static Action<List<GameObject>> GizmoDragStartEvent;
    public static Action<List<GameObject>> GizmoDragEvent;
    public static Action<List<GameObject>> GizemoDragEndEvent;

    public bool IsHandleInit;

    private bool useAroundCamera;//使用AroundAlignCamera
    void Awake(){
        Instance=this;
    }

    [UnityEngine.ContextMenu("HideHandles")]
    public void HideHandles(){
        Debug.LogError("RTEManager.HideHandles");
        IsHandlesEnabled=true;
        if(ToolBar)ToolBar.DisableHandles();
        EditButton.SetActive(false);
    }

    [UnityEngine.ContextMenu("ShowHandles")]
    public void ShowHandles(bool isShowUI=true){
        Debug.LogError("RTEManager.ShowHandles");
        IsHandlesEnabled=true;
        if(ToolBar)ToolBar.EnableHandles(isShowUI);
        EditButton.SetActive(true);
    }

    void Start(){
        EditButton=RTEditor.EditButon.gameObject;
        RTEditor.Created+=OnEditorCreated;
        RTEditor.Closed+=OnEditorClosed; 
        RTEditor.BeforeOpen+=OnEditorBeforeOpen;

        //ToggleToolbar();
        hdrpInit.EnableOutline();

        // var volumns=GameObject.FindObjectsOfType<UnityEngine.Rendering.Volumn>();
        // Debug.LogError("RTEManager.Start volumns:"+volumns.Length);

        if(IsLockCamera){
            SetLookFreeEnable(false);
        }

        ShowToolbar();
        ShowHandles();
        Invoke("HideHandlesTest", 1f);//3
    }

    private void HideHandlesTest()
    {
        IsHandleInit = true;
        HideToolbar();
        HideHandles();
        //if (PlayManager.Instance) PlayManager.Instance.Play();

        ConnectRenderStreaming();
    }

    private void ConnectRenderStreaming()
    {
        // if (RenderStreaming.Instance && !RenderStreaming.Instance.StartOnAwake)
        // {
        //     SystemSettingHelper.GetSystemSetting();
        //     var communicationSetting = SystemSettingHelper.communicationSetting;
        //     RenderStreaming.Instance.StartConnect(communicationSetting.RenderStreamingType, communicationSetting.RenderStreamingUrl,
        //         communicationSetting.RenderStreamingStartBirate, communicationSetting.RenderStreamingsMinBirate, communicationSetting.RenderStreamingMaxBirate,communicationSetting.Interval);
        //     RenderStreaming.Instance.SetAutoCloseInfo(SystemSettingHelper.communicationSetting.AutoCloseInUserQuit);
        // }
        // else
        // {
        //     Debug.LogError("RenderStreaming.Instance is null...");
        // }
    }

    public void EnableHDRP(){
        hdrpInit.gameObject.SetActive(true);
    }

    public void DisableHDRP(){
        hdrpInit.gameObject.SetActive(false);
    }

    public bool IsEditorClosed=false;

    void Update(){
        if(OnEditorDestroyedFlag){
            OnEditorDestroyedFlag=false;
            ToggleToolbar();
            hdrpInit.EnableOutline();

            LoadPivot();

            Y_UIFramework.UIManager.GetInstance().ShowUIPanel("ModelSystemTreePanel");
            Y_UIFramework.MessageCenter.SendMsg(MsgType.ModelSystemTreePanelMsg.TypeName, MsgType.ModelSystemTreePanelMsg.ShowModelTree, null);
            Y_UIFramework.MessageCenter.SendMsg(MsgType.ModuleToolbarMsg.TypeName, MsgType.ModuleToolbarMsg.ShowWindow, null);
            Y_UIFramework.MessageCenter.SendMsg(MsgType.RTEditorMsg.TypeName, MsgType.RTEditorMsg.OnEditorClosed, null);
            
        }

        if(rteBase==null ){
            rteBase=GameObject.FindObjectOfType<RTEBase>();
            if(IsEditorOpen==false){ //cww 在ToolBar显示，Editor关闭的情况下，要把RTEBase的射线检测去掉，这样子原有的项目的UI才能点击，不然RTE里面的UI(ScreenSpace-Overlay)会遮挡原来的UI(ScreenSpace-Camera)
                DisableRaycast();
                //不知道为什么哦，通过代码这样关闭Raycaster，摄像头的控制还是能用的。但是手动点击的化，摄像头就不能用了。
                //手动点击 DisableRaycast()菜单，也会导致摄像头无法控制了，只有在这里 Update 设置 DisableRaycast()，才能达到我的目的
                //很无语，本来打算放弃了的，把原项目UI从 ScreenSpace-Camera改成ScreenSpace-Overlay，再处理一下原来的漂浮UI的问题，结果这样居然可以...
            }
        }

        if(ToolBar!=null){
            if (IsClickUGUIorNGUI.Instance && IsClickUGUIorNGUI.Instance.isOverUI) 
            {
                // Debug.Log("RuntimeSceneInput.LateUpdate IsClickUGUIorNGUI.Instance.isOverUI");
                // return;
                ToolBar.sceneInput.IsOverUI=true;
            }
            else{
                ToolBar.sceneInput.IsOverUI=false;
            }
        }
        
    }

    private void InstantiateToolBar(){
        if(ToolBar==null){
            Debug.Log("Instantiate ToolBar");
            // Battlehub.RTCommon.IOC.ClearAll();
            // Battlehub.RTCommon.RTEBase.Init();
            GameObject instance=GameObject.Instantiate(ToolBarPrefab);
            instance.SetActive(true);
            ToolBar=instance.GetComponent<RTEToolBar>();
            IsToolBarVisible=false;
        }
    }

    [UnityEngine.ContextMenu("HideToolbar")]
    public void HideToolbar(){
        Debug.Log("HideToolbar");
        IsDraging = false;
        InstantiateToolBar();

        IsToolBarVisible=false;
        ToolBar.HideToolbar();
        ToolBar.gameObject.SetActive(false);
        EditorExtensions.SetActive(false);
        LookFreeToLookAround();
        useAroundCamera = false;
        //DisableRaycast();
    }

    [UnityEngine.ContextMenu("ShowToolbar")]
    public void ShowToolbar(){
        Debug.Log("ShowToolbar");

        useAroundCamera = false;
        InstantiateToolBar();
        IsToolBarVisible =true;
        ToolBar.gameObject.SetActive(true);
        ToolBar.ShowToolbar();
        EditorExtensions.SetActive(true);
        LookAroundToLookFree();
        //LookFreeToLookAround();//不一样

        //EnableRaycast();
    }

    
    /// <summary>
    /// 显示Toolbar，不切换相机
    /// </summary>
    public void ShowToolbarInCurrentCamera()
    {
        useAroundCamera = true;
        InstantiateToolBar();
        IsToolBarVisible = true;
        ToolBar.gameObject.SetActive(true);
        ToolBar.ShowToolbar();
        ToolBar.SetHandleUIState(false);
        EditorExtensions.SetActive(true);
        SetLookFreeEnable(false);
    }
    public void SelectObjects(params GameObject[] objs)
    {
        this.SelectObjs(objs);
    }

    [UnityEngine.ContextMenu("ToggleToolbar")]
    public void ToggleToolbar()
    {
        Debug.Log("ToggleToolbar");
        Debug.Log("ToolBar isnull:"+(ToolBar==null));
        if(IsToolBarVisible){
            HideToolbar();
        }
        else{
            ShowToolbar();
        }
    }
    [UnityEngine.ContextMenu("SetLockOn")]
    public void SetLockOn()
    {
        SetGlobalLock(true);
    }
    [UnityEngine.ContextMenu("SetLockOff")]
    public void SetLockOff()
    {
        SetGlobalLock(false);
    }
    public void SetGlobalLock(bool isOn)
    {
        LockObject obj = new LockObject();
        obj.PositionX = isOn;
        obj.PositionY = isOn;
        obj.PositionZ = isOn;
        obj.RotationX = isOn;
        obj.RotationY = isOn;
        obj.RotationZ = isOn;
        obj.ScaleX = isOn;
        obj.ScaleY = isOn;
        obj.ScaleZ = isOn;
        if (sceneCompnent != null&&sceneCompnent.PositionHandle!=null) sceneCompnent.PositionHandle.LockObject.SetGlobalLock(obj);
    }



    public bool IsHandlesEnabled=false;

    [UnityEngine.ContextMenu("ToggleHandles")]
    public void ToggleHandles()
    {
        if(IsHandlesEnabled){
            HideHandles();
        }
        else{
            ShowHandles();
        }
    }

    public bool IsEditorOpen=false;
    public GameObject ToolBarPrefab;
    public RTEToolBar ToolBar;
    public CreateEditor RTEditor;

    public GameObject EditButton;
    
    public Vector3 PivotPos;

    public Vector3 CameraPos;

    public void OpenSceneEditor(){
        // IsToolBarVisible=false;
        // IsEditorOpen=true;
        // // HandleRootObj.SetActive(false);
        // ToolBar.Close();
        // ToolBar=null;

        // SavePivot();
   
        RTEditor.OnOpen();
    }

    public RuntimeSelectionComponent GetSelectionComponent()
    {
        if (ToolBar != null && ToolBar.selectionComponent != null) return ToolBar.selectionComponent;
        else return null;
    }

    [UnityEngine.ContextMenu("SavePivot")]
    public void SavePivot(){
        Debug.Log("SavePivot");
        if(sceneCompnent==null){
            Debug.LogError("SavePivot sceneCompnent==null");
        }
        else{
            PivotPos=sceneCompnent.Pivot;
            CameraPos=sceneCompnent.CameraPosition;
        }
    }

    [UnityEngine.ContextMenu("LoadPivot")]
    public void LoadPivot(){
        Debug.Log("LoadPivot");
        if(sceneCompnent==null){
            Debug.LogError("LoadPivot sceneCompnent==null");
        }
        else{
            sceneCompnent.Pivot=PivotPos;
            sceneCompnent.CameraPosition=CameraPos;
        }
    }

    private void OnDestroy()
    {
        if(RTEditor)
        {
            RTEditor.Created-=OnEditorCreated;
            RTEditor.Closed-=OnEditorClosed;  
            RTEditor.BeforeOpen-=OnEditorBeforeOpen;
        }  
    }


    public bool OnEditorDestroyedFlag=false;
    private void OnEditorDestroyed(object sender)
    {
        RuntimeEditor editor=sender as RuntimeEditor;
        Debug.LogError("OnEditorDestroyed:"+sender+"|"+editor.Id);
        //IsEditorOpen=false;
        IsEditorClosed=true;
        OnEditorDestroyedFlag=true;
        //ToggleToolbar();

        // Battlehub.RTCommon.IRTE m_editor = Battlehub.RTCommon.IOC.Resolve<Battlehub.RTCommon.IRTE>();
        // Debug.LogError("m_editor:"+m_editor);
        // Debug.LogError("RTEDeps:"+Battlehub.RTEditor.RTEDeps.Instance);
    }

    
    private void OnEditorCreated(object sender)
    {
        RuntimeEditor editor=sender as RuntimeEditor;
        Debug.LogError("OnEditorCreated:"+sender+"|"+editor.Id+"|"+RTEDeps.Instance);
        // editor.Destroyed+=OnEditorDestroyed;
        RTEDeps.Instance.Destroyed+=OnEditorDestroyed;
        // editor.TestDoDestroyed();

        Y_UIFramework.MessageCenter.SendMsg(MsgType.ModelSystemTreePanelMsg.TypeName, MsgType.ModelSystemTreePanelMsg.CloseWindow, null);
        Y_UIFramework.MessageCenter.SendMsg(MsgType.ModuleToolbarMsg.TypeName, MsgType.ModuleToolbarMsg.CloseWindow, null);

    }

    private void OnEditorBeforeOpen(object sender)
    {
        SavePivot();

        IsToolBarVisible=false;
        IsEditorOpen=true;
        // HandleRootObj.SetActive(false);
        if (ToolBar != null)
        {
            ToolBar.Close();
            ToolBar = null;
        }
        
    }

    private void OnEditorClosed(object sender)
    {
        RuntimeEditor editor=sender as RuntimeEditor;
        Debug.LogError("OnEditorClosed:"+sender+"|"+editor.Id);
        IsEditorOpen=false;
        // Battlehub.RTCommon.IOC.ClearAll();
        // ToggleToolbar();
    }

    public void CloseSceneEditor(){
        IsEditorOpen=false;

        RTEditor.Editor.Close();
    }

    public void ToggleEditor()
    {
        if(IsEditorOpen){
            CloseSceneEditor();
        }
        else{
            OpenSceneEditor();
        }
    }

    public void ShowProBuilder(){

    }

    public void ShowProperies(){

    }

    public void ShowMaterialProperties(){

    }

    public AroundAlignCamera aroundAlignCamera;

    public MouseTranslatePro mouseTranslate;
    public RuntimeSceneComponent sceneCompnent;

    [UnityEngine.ContextMenu("LookFreeToLookAround")]
    public void LookFreeToLookAround()
    {
        if (useAroundCamera) return;
        SetLookAroundEnable(true);
        SetLookFreeEnable(false);
    }

    private void SetLookAroundEnable(bool enable)
    {
        if (aroundAlignCamera == null)
        {
            aroundAlignCamera = GameObject.FindObjectOfType<AroundAlignCamera>();
        }

        if(mouseTranslate==null){
            mouseTranslate = GameObject.FindObjectOfType<MouseTranslatePro>();
        }

        if (aroundAlignCamera != null)
        {
            aroundAlignCamera.enabled = enable;
        }

        if (enable)
        {
            SetRotateCenter();
        }
    }

    private void SetRotateCenter(){
        //if(aroundAlignCamera!=null && sceneCompnent!=null)
        //{
        //    //aroundAlignCamera.GetTarget().position = sceneCompnent.Pivot;
        //    aroundAlignCamera.SetTargetEx(sceneCompnent.Pivot
        //    ,sceneCompnent.CameraTransform.rotation.eulerAngles,
        //    sceneCompnent.OrbitDistance);

        //    mouseTranslate.SetTranslatePosition(sceneCompnent.Pivot,true);
        //}
    }

    public bool IsHighLightSelection = true;

    public GameObject lastSelectionGo = null;

    public bool IsEnableSelection=true;

    private void OnSelectionChanged(object sender, EventArgs e)
    {
#if UNITY_EDITOR
        Debug.Log("RTEManager.OnSelectionChanged :"+sceneCompnent.Selection.activeObject+"|"+e+"|"+sender+"|"+IsEnableSelection);
#endif
        if(IsEnableSelection==false){
            return;
        }
        if (IsHighLightSelection)
        {
            if (lastSelectionGo)
            {
                HightlightModuleBase.HighlightOff(lastSelectionGo);
            }
            HightlightModuleBase.HighlightOn(sceneCompnent.Selection.activeObject as GameObject);
            lastSelectionGo = sceneCompnent.Selection.activeObject as GameObject;
        }

        Y_UIFramework.MessageCenter.SendMsg(MsgType.RTEditorMsg.TypeName, MsgType.RTEditorMsg.OnSelectionChanged, sceneCompnent.Selection.activeObject);
        //可能多选||单选
        if (OnSelectionEvent != null)
        {
            if (sceneCompnent.Selection.gameObjects != null) OnSelectionEvent(sceneCompnent.Selection.gameObjects.ToList());
            else OnSelectionEvent(new List<GameObject>());
        }
    }

    private void Subscribe(RuntimeSceneComponent m_scene)
    {
        m_scene.PositionHandle.BeforeDrag.AddListener(OnBeginDrag);
        m_scene.PositionHandle.Drag.AddListener(OnDrag);
        m_scene.PositionHandle.Drop.AddListener(OnEndDrag);

        m_scene.RotationHandle.BeforeDrag.AddListener(OnBeginDrag);
        m_scene.RotationHandle.Drag.AddListener(OnDrag);
        m_scene.RotationHandle.Drop.AddListener(OnEndDrag);

        m_scene.ScaleHandle.BeforeDrag.AddListener(OnBeginDrag);
        m_scene.ScaleHandle.Drag.AddListener(OnDrag);
        m_scene.ScaleHandle.Drop.AddListener(OnEndDrag);
    }

    private void Unsubscribe(RuntimeSceneComponent m_scene)
    {
        m_scene.PositionHandle.BeforeDrag.RemoveListener(OnBeginDrag);
        m_scene.PositionHandle.Drag.RemoveListener(OnDrag);
        m_scene.PositionHandle.Drop.RemoveListener(OnEndDrag);

        m_scene.RotationHandle.BeforeDrag.RemoveListener(OnBeginDrag);
        m_scene.RotationHandle.Drag.RemoveListener(OnDrag);
        m_scene.RotationHandle.Drop.RemoveListener(OnEndDrag);

        m_scene.ScaleHandle.BeforeDrag.RemoveListener(OnBeginDrag);
        m_scene.ScaleHandle.Drag.RemoveListener(OnDrag);
        m_scene.ScaleHandle.Drop.RemoveListener(OnEndDrag);
    }
    public static bool IsDraging;
    private void OnBeginDrag(BaseHandle handle)
    {
        IsDraging = true;
        if (GizmoDragStartEvent != null) GizmoDragStartEvent(ToObjs(handle.Targets));
    }

    private void OnDrag(BaseHandle handle)
    {
        IsDraging = true;
        if (GizmoDragEvent != null) GizmoDragEvent(ToObjs(handle.Targets));
    }

    private void OnEndDrag(BaseHandle handle)
    {
        IsDraging = false;
        if (GizemoDragEndEvent != null) GizemoDragEndEvent(ToObjs(handle.Targets));
    }
    private List<GameObject>ToObjs(Transform[]targetGroup)
    {
        List<GameObject> objs = new List<GameObject>();
        if(targetGroup!=null)
        {
            foreach(var item in targetGroup)
            {
                objs.Add(item.gameObject);
            }
        }
        return objs;
    }
    public List<GameObject> GetSelectObjs()
    {
        if (sceneCompnent != null&& sceneCompnent.Selection.gameObjects!=null)
        {
            return sceneCompnent.Selection.gameObjects.ToList();
        }
        else
        {
            return new List<GameObject>();
        }
    }
    public void TryToClearSelection()
    {
        if(sceneCompnent!=null)sceneCompnent.TryToClearSelection(true);
    }

    private void SetLookFreeEnable(bool enable)
    {
        BindingEvent(true);
        if (sceneCompnent!=null&&!useAroundCamera)
        {
            sceneCompnent.CanZoom = enable;
            sceneCompnent.CanRotate = enable;
            sceneCompnent.CanFreeMove = enable;
            sceneCompnent.CanOrbit = enable;
            sceneCompnent.CanPan = enable;
        }

        if (enable&&!useAroundCamera)
        {
            SetPivot();
        }
    }

    [UnityEngine.ContextMenu("SetPivot")]
    public void SetPivot(){
        if (aroundAlignCamera != null && sceneCompnent != null)
        {
            Vector3 pos0=sceneCompnent.Pivot;
            Vector3 pos= aroundAlignCamera.GetTargetPosition();
            Debug.Log("SetPivot:" + pos0+"->"+pos);
            sceneCompnent.SetPivotEx(pos);
        } 
    }

    [UnityEngine.ContextMenu("SetCamPos")]
    public void SetCamPos(){
        if (aroundAlignCamera != null && sceneCompnent != null)
        {
            Debug.Log("camTransform.rotation.eulerAngles 1:"+sceneCompnent.CameraTransform.rotation.eulerAngles);
            sceneCompnent.CameraPosition=sceneCompnent.CameraPosition;
            Debug.Log("camTransform.rotation.eulerAngles 2:"+sceneCompnent.CameraTransform.rotation.eulerAngles);
        } 
    }

    [UnityEngine.ContextMenu("LookAroundToLookFree")]
    public void LookAroundToLookFree()
    {
        if (useAroundCamera) return;
        SetLookAroundEnable(false);
        SetLookFreeEnable(true);
    }

    public GameObject TestFocusObj;

    [UnityEngine.ContextMenu("TestFocus")]
    public void TestFocus(){
        this.FocusGO(TestFocusObj);
    }

    public GameObject activeObject = null;

    public void FocusGO(GameObject go){

        if (activeObject != null)
        {
            HightlightModuleBase.HighlightOff(activeObject);
        }
        activeObject = go;

        var size = go.GetSize();
        //AroundAlignCamera.Instance.AlignVeiwToTarget(go.transform,2);
        AroundAlignCamera.Instance.AlignVeiwToTarget(new AlignTarget(go.transform,new Vector2(45,0),3.5f,new Mogoson.CameraExtension.Range(-90,90),new Mogoson.CameraExtension.Range(0.5f,300)));
        //CameraSceneManager.Instance.zoomCoefficient = 0.05f;
        //CameraSceneManager.Instance.rotateCoefficient = 0.005f;
        HightlightModuleBase.HighlightOn(go);
        //下面不知道为什么没有效果啊。

        Debug.Log($"RTEManager.FocusGO go:{go} size:{size} sceneCompnent:{sceneCompnent}");

        if (sceneCompnent!=null){

            if (sceneCompnent.Selection.activeObject==go){
                Debug.LogWarning("sceneCompnent.Selection.activeObject==go");
                return;
            }
            sceneCompnent.FocusGO(go);
            Y_UIFramework.MessageCenter.SendMsg(MsgType.ModelScaneMsg.TypeName, MsgType.ModelScaneMsg.StartSubScanners, go);
        }
        else{
            Debug.LogError("FocusGO sceneCompnent == null !!");
        }
        
    }

    public RTEBase rteBase;

    public bool IsRaycastEnabled=true;

    public GraphicRaycaster raycaster;

    [UnityEngine.ContextMenu("EnableRaycast")]
    public void EnableRaycast(){
        SetRaycastEnableState(true);
    }

    [UnityEngine.ContextMenu("DisableRaycast")]
    public void DisableRaycast(){
        SetRaycastEnableState(false);
    }

    public void SetRaycastEnableState(bool v){
        IsRaycastEnabled=v;
        if(rteBase==null ){
            rteBase=GameObject.FindObjectOfType<RTEBase>();
        }
        if(raycaster==null && rteBase!=null){
            raycaster=rteBase.gameObject.GetComponentInChildren<GraphicRaycaster>();
        }
        if(raycaster!=null)
            raycaster.enabled=v;
    }

    public void CanUnSelect(bool isOn)
    {
        if(sceneCompnent!=null)
        {
            sceneCompnent.SetAllowUnSelect(isOn);
        }
    }

    public void SelectObjs(params GameObject[] objs){
        if(objs==null){
            Debug.LogError("RTEManager.SelectObjs objs==null");
            return;
        }
        Debug.LogError("RTEManager.SelectObjs:"+objs.Length);
        if (IsToolBarVisible == false)
        {
            //ShowToolbar();

            Debug.Log("ShowToolbar");

            InstantiateToolBar();

            IsToolBarVisible = true;
            ToolBar.gameObject.SetActive(true);
            ToolBar.EnableHighlight();//不一样
            EditorExtensions.SetActive(true);
            
            LookAroundToLookFree();
            LookFreeToLookAround();//不一样
        }
        BindingEvent(true);
        sceneCompnent.TryToClearSelection(true);
        foreach (var item in objs)
        {
            sceneCompnent.SelectGO(true, true, item);
        }
        // sceneCompnent.SelectList(objs);//不行啊
        //sceneCompnent.Focus(FocusMode.Default);
    }
    private bool isSubScribe;
    private void BindingEvent(bool isOn)
    {
        if (sceneCompnent == null)
        {
            sceneCompnent = GameObject.FindObjectOfType<RuntimeSceneComponent>();            
        }
        if (sceneCompnent == null) return;
        if (isOn)
        {
            if(!isSubScribe)
            {
                sceneCompnent.SelectionChanged += OnSelectionChanged;
                Subscribe(sceneCompnent);
                isSubScribe = true;
            }            
        }
        else
        {
            if(isSubScribe)
            {
                sceneCompnent.SelectionChanged -= OnSelectionChanged;
                Unsubscribe(sceneCompnent);
                isSubScribe = false;
            }           
        }
    }

    void OnDestory()
    {
        BindingEvent(false);
    }
}
