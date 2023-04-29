using System.Collections;
using System.Collections.Generic;
using Battlehub.RTHandles;
using UnityEngine;

public class RTEToolBar : MonoBehaviour
{
    public GameObject HandlesUI;

    public GameObject Gizmo;

    public GameObject HdrpObj;

    public RuntimeToolsInput toolsInput;
    
    public RuntimeSceneInput sceneInput;

    

    public RuntimeSelectionComponent selectionComponent;

    //public Battlehub.RTCommon.RTEBase RTE;

    public static RTEToolBar Instance;
    // public GameObject EditorHandlesPrefab;
    // public GameObject EditorHandles;

    void Awake()
    {
        Instance=this;

        //InitState();

        DisableHandles();//没有轴
    }

    public bool IsBoxSelectionEnabled=false;

    void Start(){
        selectionComponent.BoxSelection.enabled=IsBoxSelectionEnabled;
    }

    [ContextMenu("EnableBoxSelection")]
    public void EnableBoxSelection()
    {
        IsBoxSelectionEnabled=true;
        selectionComponent.BoxSelection.enabled=true;
    }

    [ContextMenu("DisableBoxSelection")]
    public void DisableBoxSelection()
    {
        IsBoxSelectionEnabled=false;
        selectionComponent.BoxSelection.enabled=false;
    }

    [ContextMenu("InitState")]
    public void InitState(){
        ShowToolbar();
        DisableHandles();//没有轴
        HandlesUI.SetActive(false); //UI
        //if(Gizmo!=null)  Gizmo.SetActive(false); //轴显示
    }

    // void Update(){
    //     

    // }

    [ContextMenu("ShowToolbar")]
    public void ShowToolbar(){
        //HandlesUI.SetActive(true); 
        if(Gizmo!=null) Gizmo.SetActive(true); 
        if(HdrpObj!=null){
            HdrpObj.SetActive(true);
        }
        
        toolsInput.gameObject.SetActive(true);
        sceneInput.EnableSelection=true;

        if (SceneGridObj != null)
        {
            SceneGridObj.SetActive(true);
        }
    }
    public void SetHandleUIState(bool isOn)
    {
        if(HandlesUI!=null)HandlesUI.gameObject.SetActive(isOn);
    }
    [ContextMenu("HideToolbar")]
    public void HideToolbar(){
        HandlesUI.SetActive(false);
        if (Gizmo != null) Gizmo.SetActive(false); 
        if(HdrpObj!=null){
            HdrpObj.SetActive(false);
        }
        toolsInput.gameObject.SetActive(false);
        sceneInput.EnableSelection=false;
    }

    [ContextMenu("DisableHandles")]
    public void DisableHandles(){
        HandlesUI.SetActive(false); 
        selectionComponent.IsPositionHandleEnabled=false;
        selectionComponent.IsRotationHandleEnabled=false;
        selectionComponent.IsScaleHandleEnabled=false;
        selectionComponent.IsRectToolEnabled=false;

        
    }

    [ContextMenu("EnableHandles")]
    public void EnableHandles(bool showUI){
        HandlesUI.SetActive(showUI); 
        selectionComponent.IsPositionHandleEnabled=true;
        selectionComponent.IsRotationHandleEnabled=true;
        selectionComponent.IsScaleHandleEnabled=true;
        selectionComponent.IsRectToolEnabled=true;

        if (SceneGridObj != null)
        {
            SceneGridObj.SetActive(true);
        }
    }

    public Battlehub.RTCommon.RTEBase HandleRTE;
    public void Close(){

        if(HandleRTE==null)
        {
            HandleRTE=GameObject.FindObjectOfType<Battlehub.RTCommon.RTEBase>();
        }

        if(HandleRTE!=null){
            //HandleRTE.gameObject.SetActive(false);
            GameObject.DestroyImmediate(HandleRTE.gameObject);
        }

        GameObject.DestroyImmediate(this.gameObject);
    }

    public GameObject SceneGridObj;

    public void EnableHighlight()
    {
        //if(Gizmo!=null) Gizmo.SetActive(true);
        if (HdrpObj != null)
        {
            HdrpObj.SetActive(true);
        }
        toolsInput.gameObject.SetActive(false);
        sceneInput.EnableSelection = false;

        if (SceneGridObj != null)
        {
            SceneGridObj.SetActive(false);
        }
    }
}
