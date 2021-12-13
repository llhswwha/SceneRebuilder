using Mogoson.CameraExtension;
//using RTEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Y_UIFramework;
using NavisPlugins.Infos;
using System;

public class BIMModelInfo : MonoBehaviour,IComparable<BIMModelInfo>
{
    // private float InnerGlowStrength = 1.3f;
    // private float InnerGlowWidth = 1.3f;

    private GameObject FollowUI;



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

    public static List<BIMModelInfo> currentFocusModels = new List<BIMModelInfo>();

    public string Guid;
    public string RenderId;
    public string MName;
    public string MId;

    private string areaName = "";

    [ContextMenu("TestGetArea")]
    public void TestGetArea()
    {
        areaName = "";
        //GetArea();

        if (string.IsNullOrEmpty(areaName))
        {
            var depNode = this.GetComponentInParent<DepNode>();
            Debug.LogError($"depNode:{depNode}");
            if (depNode != null)
            {
                areaName = depNode.name;
            }
            else
            {
                areaName = "NULL";
            }
        }

        Debug.LogError($"areaName:{areaName}");
    }

    public string GetArea()
    {
        if (string.IsNullOrEmpty(areaName) || areaName=="NULL")
        {
            var depNode = this.GetComponentInParent<DepNode>();
            if (depNode != null)
            {
                if (string.IsNullOrEmpty(depNode.NodeName))
                {
                    areaName = depNode.name;
                }
                else
                {
                    areaName = depNode.NodeName;
                }
            }
            else
            {
                areaName = "NULL";
            }
        }

        return areaName;
    }

    public Vector3 Position1;

    public Vector3 Position2;

    public float Distance=0;

    public float GetDistance()
    {
        this.Distance = Vector3.Distance(this.Position1, this.Position2);
        return this.Distance;
    }

    public GameObject RepeatObj;

    [NonSerialized]
    public ModelItemInfo Model;

    public static BIMModelInfo SetModelInfo(Transform transform, ModelItemInfo model)
    {
        BIMModelInfo bim = transform.gameObject.GetComponent<BIMModelInfo>();
        if(bim==null)
            bim = transform.gameObject.AddComponent<BIMModelInfo>();
        bim.SetModelInfo(model);
        return bim;
    }

    public void SetModelInfo(ModelItemInfo model)
    {
        this.Model = model;
        this.MName = model.Name;
        this.MId = model.Id;
        this.Guid = model.UId;
        this.Position1 = transform.position;
        this.Position2 = model.GetPositon();
        this.GetDistance();

        this.RenderId = RendererId.GetId(this.gameObject,0);
        model.RenderId = this.RenderId;
        model.RenderName = this.name;
    }

    public ModelItemInfo GetModelInfo()
    {
        return Model;
    }

    public bool IsFound = false;

    public BIMFoundType FoundType = 0;

    public enum BIMFoundType
    {
        None,
        ByUID,
        ByPos111,
        ByPos1NN,
        ByName,
        ByNameAndClosed,
        ByClosed
    }

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

    public static void FocusOnDevs(List<BIMModelInfo> bims)
    {

    }

    public static void BeginMultiFocus()
    {
        foreach(var dev in currentFocusModels)
        {
            dev.FocusOff();
        }
        currentFocusModels.Clear();
    }

    public void MultiFocusOn()
    {
        currentFocusModels.Add(this);
        FocusOnInner(true);
    }

    private void FocusOnInner(bool isFoucsBimModel)
    {
        //if (RTEManager.Instance.IsToolBarVisible)
        //{
        //    RTEManager.Instance.FocusGO(currentFocusModel.gameObject);
        //}
        //else
        //{
        //    CameraSceneManager manager = CameraSceneManager.Instance;
        //    if (manager)
        //    {
        //        if (isFoucsBimModel) SetFocusAlignArg(manager.GetCurrentAlign());
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
        //    else
        //    {
        //        Debug.LogError("manager==null");
        //    }
        //}
    }


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


        FocusOnInner(isFoucsBimModel);


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
        //Debug.LogError("HighlightOn:" + this);

        ////if (gameObject == null||transform.GetComponent<Renderer>()==null) return;

        ////if(transform.getcomp)

        //var renderers = transform.GetComponentsInChildren<Renderer>(true);
        //foreach (var renderer in renderers)
        //{
        //    var h = renderer.gameObject.AddMissingComponent<HightlightModuleBase>();
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

    private void SetFocusAlignArg(AlignTarget targetT)
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
    protected AlignTarget GetTargetInfo(GameObject obj)
    {
        MeshRendererInfo info = MeshRendererInfo.GetInfo(this.gameObject,false);

        camDistance = info.diam * 2.2f;
        angleFocus = new Vector2(45, 45);

        //angleFocus = new Vector2(15, transform.eulerAngles.y);
        AlignTarget alignTargetTemp = new AlignTarget(obj.transform, angleFocus,
                               camDistance, angleRange, disRange);



        Debug.LogError($"SetFocusAlignArg bim:{this.name} (targetT {alignTargetTemp}) diam:{info.diam} size:{info.size}");

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

    public ModelItemInfo FindClosedModel(List<ModelItemInfo> models)
    {
        return FindClosedModel(models, this.transform.position);
    }

    public static ModelItemInfo FindClosedModel(List<ModelItemInfo> models,Vector3 pos)
    {
        float minDis = float.MaxValue;
        ModelItemInfo minModel = null;
        foreach(var model in models)
        {
            float dis = Vector3.Distance(model.GetPositon(), pos);
            if (minDis > dis)
            {
                minDis = dis;
                minModel = model;
            }
        }
        return minModel;
    }
}
