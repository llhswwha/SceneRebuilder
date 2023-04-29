using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DbModel.Location.Pipes;
using UnityEngine;
using UnityEngine.UI;
using Y_UIFramework;

public class PipeSystemEditUIPanel : PipeSystemEditBasePanel
{
    public static PipeSystemEditUIPanel Instance;

    public Button btnShowSettingWindow;

    public Button btnDelete;

    public Button btnSave;

    public InputField inputPointX;

    public InputField inputPointY;

    public InputField inputPointZ;

    public Button btnSavePoint;

    public Button btnAddPoint;

    public Button btnInsertPoint;

    public Button btnConvertPos;

    public PipePoint currentPoint;

    public Toggle toggleIsShowFollowUI;

    public Toggle toggleIsAutoSave;

    public GameObject pipePointListPanel;

    private int count =0;

    protected override void SelectPoint(PipePoint point){
        Debug.Log($"SelectPoint12 point:{point} Start count:{count}");
        try
        {
            count++;
            currentPoint=point;
            // inputPointX.text=$"ABC{point.X:F3}";
            // inputPointY.text=$"{point.Y:F3}";
            // inputPointZ.text=$"{point.Z:F3}";

            inputPointX.text=$"{point.X:F3}";
            inputPointY.text=$"{point.Y:F3}";
            inputPointZ.text=$"{point.Z:F3}";
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SelectPoint point:{point} Exceptio:{ex}");
        }
        
    }

    public void AutoInsertPipe(PipePoint point){
        try
        {
            this.currentPoint=point;
            int id=this.pipeSystem.GetPointIndex(point);
            int id2=id;
            if(id==0){
                id2=id+1;
            }
            else if(id==this.pipeSystem.PointCount-1){
                id2=id-1;
            }
            else{
                id2=id+1;
            }
            PipePoint point2=this.pipeSystem.Points[id2];
            PipePoint pipePoint=new PipePoint();
            pipePoint.PId=pipeSystem.Id;
            pipePoint.X=(point.X+point2.X)/2;
            pipePoint.Y=(point.Y+point2.Y)/2;
            pipePoint.Z=(point.Z+point2.Z)/2;
            pipePoint.Num=currentPoint.Num;
            InsertPipePoint(pipePoint);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"AutoInsertPipe Exceptio:{ex}");
        }
    }

    private void InsertPipePoint()
    {
        try
        {
            PipePoint pipePoint=GetPipePoint();
            pipePoint.Num=currentPoint.Num;
            InsertPipePoint(pipePoint);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"AddPipePoint_4 Exceptio:{ex}");
        }
    }

    private void InsertPipePoint(PipePoint pipePoint)
    {
        try
        {
            var pipeSys=this.pipeSystem;
            Debug.Log($"InsertPipePoint_1 currentPoint:{currentPoint}");
            if(pipeSys.Points==null){
                pipeSys.Points=new List<PipePoint>();
            }
            PipeSystemClient.InsertPipePoint(pipeSys,currentPoint,pipePoint,(result)=>{
                GameObject obj=base.NewPipePoint(result);
                txtPoints.text="顶点数："+pipeSys.Points.Count;
                AddPipePoints();
            });

            // Debug.Log($"InsertPipePoint_2 pipePoint:{pipePoint}");
            // CommunicationObject.Instance.AddPipePoint(pipePoint,result=>{
            //     if(result==null){
            //         Debug.LogError($"InsertPipePoint_3 result==null");
            //     }
            //     else{
            //         Debug.Log($"InsertPipePoint_3 pipePoint:{pipePoint}");
            //         GameObject obj=base.NewPipePoint(result);
            //         PipeSystemComponent pipeSysCom=PipeSystemBuilder.Instance.GetPipeSystemComponent(pipeSys.Id);
            //         int id=pipeSysCom.InsertPoint(result,currentPoint);
            //         txtPoints.text="顶点数："+pipeSys.Points.Count;
            //         AddPipePoints();
                    
            //         PipeSystemUtils.RefeshPointObjects(pipeSysCom);

            //         pipeSysCom.FocusPoint(result);
                    
            //         //RefreshPipePoints(obj,id);

            //         Debug.Log($"InsertPipePoint_4 pipePoint:{pipePoint}");
            //         CommunicationObject.Instance.EditPipePoints(pipeSys,r1=>{
            //             Debug.Log($"InsertPipePoint_5 pipePoint:{pipePoint} r1:{r1}");
            //         });
            //     }
            // });


        }
        catch (System.Exception ex)
        {
            Debug.LogError($"AddPipePoint_4 Exceptio:{ex}");
        }
    }

    private PipePoint GetPipePoint()
    {
        PipePoint pipePoint=new PipePoint();
        try
        {
            pipePoint.PId=pipeSystem.Id;
            pipePoint.X=double.Parse(inputPointX.text);
            pipePoint.Y=double.Parse(inputPointY.text);
            pipePoint.Z=double.Parse(inputPointZ.text);
        }
        catch (System.Exception ex)
        {
             Debug.Log($"GetPipePoint Exception:{ex}");
        }
        return pipePoint;
    }
    private void AddPipePoint()
    {
        try
        {
            var pipeSys=this.pipeSystem;
            Debug.Log($"AddPipePoint_1 currentPoint:{currentPoint}");
            if(pipeSys.Points==null){
                pipeSys.Points=new List<PipePoint>();
            }

            PipePoint pipePoint=GetPipePoint();

            int num=1;
            if(pipeSys.Points.Count>0)
            {
                num=pipeSys.Points.Last().Num+1;
            }
            pipePoint.Num=num;
            
            Debug.Log($"AddPipePoint_2 pipePoint:{pipePoint}");
            CommunicationObject.Instance.AddPipePoint(pipePoint,result=>{
                if(result==null){
                    Debug.LogError($"AddPipePoint_3 result==null");
                }
                else{
                    Debug.Log($"AddPipePoint_3 pipePoint:{pipePoint}");
                    base.AddPipePoint(result);
                    PipeSystemComponent pipeSysCom=PipeSystemBuilder.Instance.GetPipeSystemComponent(this.pipeSystem.Id);
                    pipeSysCom.AddPoint(result);
                    pipeSysCom.FocusPoint(result);
                    txtPoints.text="顶点数："+pipeSys.Points.Count;
                }
            });
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"AddPipePoint_4 Exceptio:{ex}");
        }
    }

    private void EditPipePoint()
    {
        try
        {
            Debug.Log($"EditPipePoint_1 currentPoint:{currentPoint}");
            currentPoint.X=double.Parse(inputPointX.text);
            currentPoint.Y=double.Parse(inputPointY.text);
            currentPoint.Z=double.Parse(inputPointZ.text);
            Debug.Log($"EditPipePoint_2 currentPoint:{currentPoint}");

            // CommunicationObject.Instance.EditPipePoint(currentPoint,result=>{
            //     Debug.Log($"EditPipePoint_3 currentPoint:{currentPoint}");
            //     GameObject pointItem=pointItemDict[currentPoint.Id];
            //     PipePointItemUI ui=pointItem.GetComponent<PipePointItemUI>();
            //     ui.InitData(currentPoint);
            //     PipeSystemBuilder.Instance.ModifyPoint(currentPoint);
            // });

            PipeSystemUtils.EditPipePoint(currentPoint,true);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"EditPipePoint_4 Exceptio:{ex}");
        }
    }

    private void Awake()
    {
        Instance=this;
        InitPipeSystemTypes(false);

        //窗体性质
        CurrentUIType.UIPanels_Type = UIPanelType.Normal;  //普通窗体
        //CurrentUIType.UIPanel_LucencyType = UIPanelTransparentType.Pentrate;//透明，可以穿透
        CurrentUIType.UIPanels_ShowMode = UIPanelShowMode.Normal;//普通，可以和其他窗体共存

        RegisterMsgListener(MsgType.PipeSystemEditPanelMsg.TypeName,
            obj =>
            {
                Debug.Log($"PipeSystemEditUIPanel.RegisterMsg obj.Key:{obj.Key} obj.Values:{obj.Values}");
                if (obj.Key == MsgType.PipeSystemEditPanelMsg.InitData)
                {
                    InitData(obj.Values as PipeSystem);
                }
                else if (obj.Key == MsgType.PipeSystemEditPanelMsg.SetTransformPos)
                {
                    SetTransformPos((Vector3)obj.Values);
                }
                else if (obj.Key == MsgType.PipeSystemEditPanelMsg.SaveTransform)
                {
                    SaveTransform(obj.Values as PipeSystem);
                }
                else
                {
                    Debug.LogError($"PipeSystemEditUIPanel.RegisterMsg obj.Key:{obj.Key} obj.Values:{obj.Values}");
                }
            }
       );

        if(btnDelete){
            btnDelete.onClick.AddListener(DeletePipeSystem);
        }

        if(btnSave){
            btnSave.onClick.AddListener(EditPipeSystem);
        }

        if(btnSavePoint){
            btnSavePoint.onClick.AddListener(EditPipePoint);
        }

        if(btnAddPoint){
            btnAddPoint.onClick.AddListener(AddPipePoint);
        }

        if(btnInsertPoint){
            btnInsertPoint.onClick.AddListener(InsertPipePoint);
        }

        if(btnConvertPos){
            btnConvertPos.onClick.AddListener(ConvertPos);
        }

        txtSize.onEndEdit.AddListener(OnSizeChanged);
        txtName.onEndEdit.AddListener(OnTextChanged);
        txtCode.onEndEdit.AddListener(OnTextChanged);

        if(toggleIsShowFollowUI){
            toggleIsShowFollowUI.onValueChanged.AddListener(OnIsShowFollowUIChanged);
        }

        InitTransformInputField();

        if(btnShowSettingWindow){
            btnShowSettingWindow.onClick.AddListener(ShowSettingWindow);
        }
    }

    private void OnTextChanged(string newV)
    {
        AutoSave();
    }

    private void ConvertPos()
    {
        PipePoint pipePoint=GetPipePoint();
        Vector3 pos=PipeSystemUtils.GetPointVector3(pipePoint);
        inputPointX.text=$"{pos.x:F3}";
        inputPointY.text=$"{pos.y:F3}";
        inputPointZ.text=$"{pos.z:F3}";
    }

    private void ShowSettingWindow()
    {
        PipeSystemSettingWindow.Instance.ShowWindow();
    }

    private bool isInitDone=false;

    protected override void InitData(PipeSystem pipeSys)
     {
        isInitDone=false;

        //this.pipeSystem=pipeSys;

        base.InitData(pipeSys);
        SetTransform();

        isInitDone=true;
     }

    private void OnIsShowFollowUIChanged(bool isOn){
        PipeSystemBuilder.Instance.SetIsShowPointFollowUI(isOn);
        if(pipePointListPanel)pipePointListPanel.SetActive(isOn);
    }

 

    private void OnSizeChanged(string newSize){
        float radius=float.Parse(newSize);
        PipeSystemComponent systemComponent=PipeSystemBuilder.Instance.GetPipeSystemComponent(this.pipeSystem.Id);
        systemComponent.ChangePipeRadius(PipeSystemBuilder.Instance.GetPipeRadius(radius));
        AutoSave();
    }

    private void AutoSave()
    {
        if(IsAutoSave()==false)return;
        EditPipeSystemInner(false);
    }

    private bool IsAutoSave()
    {
        if(isInitDone==false)return false;
        if(toggleIsAutoSave==null)return false;
        if(toggleIsAutoSave.isOn==false)return false;
        return true;
    }

    int typeChangeCount=0;

    protected override void OnPipeTypeChanged(int n)
    { 
        typeChangeCount++;
        base.OnPipeTypeChanged(n);

        if(isInitDone==false)return;

        var pipeType=pipeTypes[n];
        var pipeSys=this.pipeSystem;

        Debug.Log($"PipeSystemEditUIPanel.OnPipeTypeChanged_1 [{typeChangeCount}] n:{n},{pipeSys} > {pipeType} ");

        if(pipeSys==null){
            Debug.Log($"PipeSystemEditUIPanel.OnPipeTypeChanged_2 pipeSystem==null ");
            return;
        }
        if(pipeType==null){
            Debug.Log($"PipeSystemEditUIPanel.OnPipeTypeChanged_3 currentPipeType==null  ");
            return;
        }

        //currentPipeType=pipeTypes[n];

        Debug.Log($"PipeSystemEditUIPanel.OnPipeTypeChanged_4 [{typeChangeCount}] n:{n},{pipeSys.Type} > {pipeType.Name} | ({pipeSys.Id}) ({pipeType.Id})");

        pipeSys.Type=pipeType.Name;


        PipeSystemComponent obj=PipeSystemBuilder.Instance.GetPipeSystemComponent(pipeSys.GetKey());
        if(obj){
            obj.ChangePipeType(pipeType);
        }
        else{

        }

        AutoSave();
    }

    private void Start(){
        
    }

    public InputField inputPosX;

    public InputField inputPosY;

    public InputField inputPosZ;

    public InputField inputScaleX;

    public InputField inputScaleY;

    public InputField inputScaleZ;

    public InputField inputRotationX;

    public InputField inputRotationY;

    public InputField inputRotationZ;

    public void InitTransformInputField()
    {
        if(inputPosX)inputPosX.onEndEdit.AddListener(OnTransformChanged);
        if(inputPosY)inputPosY.onEndEdit.AddListener(OnTransformChanged);
        if(inputPosZ)inputPosZ.onEndEdit.AddListener(OnTransformChanged);
        if(inputScaleX)inputScaleX.onEndEdit.AddListener(OnTransformChanged);
        if(inputScaleY)inputScaleY.onEndEdit.AddListener(OnTransformChanged);
        if(inputScaleZ)inputScaleZ.onEndEdit.AddListener(OnTransformChanged);
        if(inputRotationX)inputRotationX.onEndEdit.AddListener(OnTransformChanged);
        if(inputRotationY)inputRotationY.onEndEdit.AddListener(OnTransformChanged);
        if(inputRotationZ)inputRotationZ.onEndEdit.AddListener(OnTransformChanged);
    }

    private float GetValue(InputField input,float defaultV=0){
        if(input==null) return defaultV;
        if(string.IsNullOrEmpty(input.text)){
            return defaultV;
        }
        return float.Parse(input.text);
    }

    [ContextMenu("TestTransformChanged")]
    private void TestTransformChanged()
    {
        OnTransformChanged("");
    }

    private void OnTransformChanged(string newV)
    {
        if(isInitDone==false)return;
        GetTransform();
        PipeSystemComponent systemComponent=PipeSystemBuilder.Instance.GetPipeSystemComponent(this.pipeSystem.Id);
        systemComponent.SetPipeTransform();
        AutoSave();
    }

    private void GetTransform()
    {
        var pipeSys=this.pipeSystem;
        pipeSys.PosX=GetValue(inputPosX);
        pipeSys.PosY=GetValue(inputPosY);
        pipeSys.PosZ=GetValue(inputPosZ);

        pipeSys.RotationX=GetValue(inputRotationX);
        pipeSys.RotationY=GetValue(inputRotationY);
        pipeSys.RotationZ=GetValue(inputRotationZ);

        pipeSys.ScaleX=GetValue(inputScaleX,1);
        pipeSys.ScaleY=GetValue(inputScaleY,1);
        pipeSys.ScaleZ=GetValue(inputScaleZ,1);
    }

    private void SetValue(InputField input,float v)
    {
        if(input){
            input.text=v+"";
        }
    }

    private void SetValue(InputField input,float v,float defaultV)
    {
        if(input){
            if(v==0){
                v=defaultV;
            }
            input.text=v+"";
        }
    }

    private void SaveTransform(PipeSystem pipeSys)
    {
        SetTransform(pipeSys);
        EditPipeSystemInner(false);
    }

    private void SetTransformPos(Vector3 pos)
    {
        SetValue(inputPosX,pos.x);
        SetValue(inputPosY,pos.y);
        SetValue(inputPosZ,pos.z);
    }

    private void SetTransform(PipeSystem pipeSys)
    {
        if(pipeSys==null)return;
        SetValue(inputPosX,pipeSys.PosX);
        SetValue(inputPosY,pipeSys.PosY);
        SetValue(inputPosZ,pipeSys.PosZ);
        SetValue(inputRotationX,pipeSys.RotationX);
        SetValue(inputRotationY,pipeSys.RotationY);
        SetValue(inputRotationZ,pipeSys.RotationZ);
        SetValue(inputScaleX,pipeSys.ScaleX,1);
        SetValue(inputScaleY,pipeSys.ScaleY,1);
        SetValue(inputScaleZ,pipeSys.ScaleZ,1);
    }

    private void SetTransform()
    {
        var pipeSys=this.pipeSystem;
        SetTransform(pipeSys);
    }

}
