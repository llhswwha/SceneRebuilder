using Mogoson.CameraExtension;
//using RTEditor;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using Y_UIFramework;
using NavisPlugins.Infos;
using System;

public class BIMModelInfo : MonoBehaviour,IComparable<BIMModelInfo>
{
    // private float InnerGlowStrength = 1.3f;
    // private float InnerGlowWidth = 1.3f;

    public string GetItemText()
    {
        if (this.name==this.MName)
        {
            return $"{this.Distance:F5}|{this.MId}|====";
        }
        else if(this.name.StartsWith(this.MName))
        {
            return $"{this.Distance:F5}|{this.MId}|==";
        }
        else
        {
            return $"{this.Distance:F5}|{this.MId}|{this.MName}";
        }
        
    }

    public static BIMModelInfo currentFocusModel;

    public string Guid;
    public string RenderId;
    public string MName;
    public string MId;

    public Vector3 Position1;

    public Vector3 Position2;

    public float Distance=0;

    public GameObject RepeatObj;

    [NonSerialized]
    private ModelItemInfo Model;

    public void SetModelInfo(ModelItemInfo model)
    {
        this.Model = model;
        this.MName = model.Name;
        this.MId = model.Id;
    }

    public ModelItemInfo GetModelInfo()
    {
        return Model;
    }

    public bool IsFound = false;

    private void Start()
    {
        if (transform.GetComponent<Collider>() != null)
        {
            DoubleClickEventTrigger_u3d trigger = DoubleClickEventTrigger_u3d.Get(gameObject);
            trigger.onClick += OnClick;
            trigger.onDoubleClick += OnDoubleClick;
        }
        SaveToRoomFactory();
    }
    public void OnClick()
    {       
        //if (!RTEManager.Instance.IsToolBarVisible)
        //{
        //    FocusOn();
        //    MessageCenter.SendMsg(MsgType.ModelSystemTreePanelMsg.TypeName, MsgType.ModelSystemTreePanelMsg.SelectNodeWithoutFocus, gameObject);
        //}           
    }
    public void OnDoubleClick()
    {

    }
    [ContextMenu("GetRenderId")]
    public void GetRenderId()
    {
        RendererId renderIdT = transform.GetComponent<RendererId>();
        if (renderIdT)
        {
            RenderId = renderIdT.Id;
        }
    }
    void SaveToRoomFactory()
    {
        //if(RoomFactory.Instance)
        //{
        //    RoomFactory.Instance.AddBIMModel(this);
        //}
    }
    #region 模型聚焦
    private Vector2 moveRange=new Vector2(10,10);
    
    public void FocusOn()
    {
        Debug.Log("BIMModelInfo.FocusOn:"+this);
        bool isFoucsBimModel = false ;
        if(currentFocusModel!=null&&currentFocusModel!=this)
        {
            currentFocusModel.FocusOff();
            isFoucsBimModel = true;
        }

        currentFocusModel = this;

        //if(RTEManager.Instance.IsToolBarVisible)
        //{
        //    RTEManager.Instance.FocusGO(currentFocusModel.gameObject);
        //}
        //else{
        //    CameraSceneManager manager = CameraSceneManager.Instance;
        //    if (manager)
        //    {
        //        if (isFoucsBimModel) SetAngleFoucs(manager.GetCurrentAlign());
        //        AlignTarget target = GetTargetInfo(gameObject);

        //        CameraSceneManager.Instance.zoomCoefficient = 0.05f;
        //        CameraSceneManager.Instance.rotateCoefficient = 0.005f;

        //        HighlightOn();
        //        manager.FocusTargetWithTranslate(target, moveRange, () =>
        //        {
        //            //HighlightOn();
        //            SelectObject(gameObject);
        //            MessageCenter.SendMsg(MsgType.ModelSystemTreePanelMsg.TypeName, MsgType.ModelSystemTreePanelMsg.SetBackButtonState, true);
        //        });
        //    }
        //    else{
        //        Debug.LogError("manager==null");
        //    }
        //}

        
    }
    /// <summary>
    /// 选中当前物体
    /// </summary>
    /// <param name="obj"></param>
    private void SelectObject(GameObject obj)
    {
        //EditorObjectSelection.Instance?.AddObjectToSelection(obj, false);
    }

    /// <summary>
    /// 取消聚焦和高亮
    /// </summary>
    public void FocusOff()
    {
        currentFocusModel = null;
        HighlightOff();
    }
    
    /// <summary>
    /// 高亮设备
    /// </summary>
    public void HighlightOn()
    {
        ////if (gameObject == null||transform.GetComponent<Renderer>()==null) return;
        ////var renderers = transform.GetComponentsInChildren<Renderer>(true);
        ////foreach(var renderer in renderers)
        //{
        //    var h = gameObject.AddMissingComponent<HightlightModuleBase>();
        //    Color colorConstant = Color.green;
        //    gameObject.SetActive(true);
        //    h.ConstantOnImmediate(colorConstant);
        //}
        
    }

    public void HighlightOff()
    {
        ////if (gameObject == null || transform.GetComponent<Renderer>() == null) return;
        ////var h = gameObject.AddMissingComponent<HightlightModuleBase>();
        ////h.ConstantOffImmediate();
        //var renderers = transform.GetComponentsInChildren<Renderer>(true);
        //foreach (var renderer in renderers)
        //{
        //    var h = renderer.gameObject.AddMissingComponent<HightlightModuleBase>();
        //    //Color colorConstant = Color.green;
        //    h.ConstantOffImmediate();
        //}
    }

    protected Vector2 angleFocus = new Vector2(51, 220);
    protected float camDistance = 2f;
    protected Range angleRange = new Range(5, 90);
    protected Range disRange = new Range(1f, 200);

    private void SetAngleFoucs(AlignTarget targetT)
    {
        angleFocus = targetT.angles;
        camDistance = targetT.distance;
        angleRange = targetT.angleRange;
        disRange = targetT.distanceRange;
    }
    /// <summary>
    /// 获取相机聚焦物体的信息
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    protected virtual AlignTarget GetTargetInfo(GameObject obj)
    {
        //angleFocus = new Vector2(15, transform.eulerAngles.y);
        AlignTarget alignTargetTemp = new AlignTarget(obj.transform, angleFocus,
                               camDistance, angleRange, disRange);
        return alignTargetTemp;
    }
    #endregion

    private void OnDestroy()
    {
        //if (RoomFactory.Instance)
        //{
        //    RoomFactory.Instance.RemoveBIMmodel(Guid);
        //}
    }

    public int CompareTo(BIMModelInfo other)
    {
        return other.Distance.CompareTo(this.Distance);
    }
}
