using System;
using System.Collections;
using System.Collections.Generic;
using DbModel.Location.Pipes;
using UnityEngine;
using Y_UIFramework;

public class PipeSystemComponent : MonoBehaviour
{
    public Material pipeMat;

    public PipeSystem pipeSystem;

    //public Vector3 minOff;

    //public Vector3 maxOff;

    double minX=double.MaxValue;
    double minY=double.MaxValue;
    double minZ=double.MaxValue;
    double maxX=double.MinValue;
    double maxY=double.MinValue;
    double maxZ=double.MinValue;

    public PipeMeshGenerator generator;

    public void ChangePipeRadius(float radius){
        if(generator==null){
            Debug.LogError($"ChangePipe generator==null");
            return;
        }
        generator.pipeRadius=radius;
        //generator.elbowRadius=radius*2;
        generator.RenderPipe();

        foreach(var p in pipeSystem.Points){
            var pointObj = PipeSystemUtils.GetPipeObject(p);
            if(pointObj != null){
                pointObj.transform.localScale=new Vector3(radius,radius,radius);
            }
        }
    }

    [ContextMenu("SetPipeTransform")]
    public void SetPipeTransform()
    {
        //this.transform.localPosition=GetPipeSysPos(pipeSystem)+posOff;
        //this.transform.localPosition= pipeSystem.GetPos() + posOff;
        this.transform.position= pipeSystem.GetPos() + posOff;
        this.transform.localScale=GetPipeSysScale(pipeSystem);
        this.transform.localRotation=Quaternion.Euler(GetPipeSysRotation(pipeSystem));
    }

    public Vector3 GetPipeSysPos()
    {
        //return GetPipeSysPos(pipeSystem);
        return pipeSystem.GetPos();
    }

    // private Vector3 GetPipeSysPos(PipeSystem pipeSys){
    //     //return new Vector3(pipeSys.PosX,pipeSys.PosY,pipeSys.PosZ);
    //     return pipeSys.GetPos();
    // }

    private Vector3 GetPipeSysRotation(PipeSystem pipeSys){
        return new Vector3(pipeSys.RotationX,pipeSys.RotationY,pipeSys.RotationZ);
    }

    private Vector3 GetPipeSysScale(PipeSystem pipeSys){
        float scaleX=1;
        if(pipeSys.ScaleX!=0)
        {
            scaleX=pipeSys.ScaleX;
        }
        float scaleY=1;
        if(pipeSys.ScaleY!=0)
        {
            scaleY=pipeSys.ScaleY;
        }
        float scaleZ=1;
        if(pipeSys.ScaleZ!=0)
        {
            scaleZ=pipeSys.ScaleZ;
        }
        return new Vector3(scaleX,scaleY,scaleZ);
    }

    private void GetMinMax(){
        foreach(var point in pipeSystem.Points){
            if(point.X<minX){
                minX=point.X;
            }
            if(point.X>maxX){
                maxX=point.X;
            }
            if(point.Y<minY){
                minY=point.Y;
            }
            if(point.Y>maxY){
                maxY=point.Y;
            }
            if(point.Z<minZ){
                minZ=point.Z;
            }
            if(point.Z>maxZ){
                maxZ=point.Z;
            }
        }
        //minOff=new Vector3(minX,minY,minZ);
        //maxOff=new Vector3(maxX,maxY,maxZ);
    }

    public void GeneratePipe(PipeSysGenerateArg arg){
        this.pipeMat=arg.mat;
        GeneratePipe(arg.radius,arg.segments,arg.isSmooth);
    }

    [ContextMenu("GeneratePipe")]
    public void GeneratePipe(float radius,int segments,bool isSmooth){
        if(generator==null)
        {
            generator=this.gameObject.AddComponent<PipeMeshGenerator>();
        }

        generator.pipeMaterial=pipeMat;
        generator.avoidStrangling=true;
        generator.pipeRadius=radius;
        //generator.elbowRadius=radius*2;
        generator.pipeSegments=segments;
        generator.flatShading=!isSmooth;
        generator.generateEndCaps=true;

        if(pipeSystem.Points==null)return;
        
        RenderPipe();
        //Debug.Log("GeneratePipe 4 ");

        this.gameObject.AddMissingComponent<MeshCollider>();

        //this.transform.position=new Vector3(50,0,50);
    }

    [ContextMenu("RenderPipe")]
    public void RenderPipe()
    {
        //GetMinMax();
        generator.points=new List<Vector3>();

        pipeSystem.SortPoints();
        for (int i = 0; i < pipeSystem.Points.Count; i++)
        {
            PipePoint point = pipeSystem.Points[i];
            var pos=GetPointVector3(point);
            generator.points.Add(pos);//坐标系转换
        }
        //Debug.Log("GeneratePipe 3 ");
        RenderPipeInner();

        this.SetPipeTransform();
    }

    public Vector3 posOff=Vector3.zero;

    [ContextMenu("RenderPipe")]
    private void RenderPipeInner()
    {
        generator.RenderPipe();
        this.posOff=generator.posOff;

        if(objCenterDict.ContainsKey(this.gameObject))
        {
            objCenterDict.Remove(this.gameObject);
        }
    }

    public static Dictionary<GameObject,Vector3> objCenterDict=new Dictionary<GameObject, Vector3>();
    
    public static Vector3 GetCenter(GameObject model)
    {
        if(objCenterDict.ContainsKey(model)){
            return objCenterDict[model];
        }
        Vector3[] minMax = MeshHelper.GetMinMax(model);
        Vector3 size = minMax[2];
        Vector3 center = minMax[3];
        Bounds bounds = new Bounds(center, size);
        Vector3 v= bounds.center;
        objCenterDict.Add(model,v);
        return v;
    }

    public void RefreshPipe(PipePointComponent pipeCom)
    {
        DateTime startT=DateTime.Now;
        pipeCom.GetCurrentPos();//更新坐标

        double t1=(DateTime.Now-startT).TotalMilliseconds;
        //GetMinMax();
        //generator.points=new List<Vector3>();
        //pipeSystem.SortPoints();
        var point=pipeCom.point;
        int id=pipeSystem.Points.IndexOf(point);
        if(id!=-1){
            // Vector3 pos1=GetPointVector3(point);
            // Vector3 pos2=pipeCom.transform.position;
            // Debug.Log($"RefreshPipe 1 id:{id} point:{point} pos1:({pos1.x:F5},{pos1.y:F5},{pos1.z:F5}) pos2:({pos2.x:F5},{pos2.y:F5},{pos2.z:F5})");
            // generator.points[id]=pos1;
            generator.points[id]=pipeCom.GetPos();
        }
        else{
            Debug.Log($"RefreshPipe 2 point:{point}");
        }

        double t2=(DateTime.Now-startT).TotalMilliseconds;
        
        RenderPipeInner();

        double t3=(DateTime.Now-startT).TotalMilliseconds;
        PipeSystemUtils.UpdateUIInfo(pipeCom.point);

        double t4=(DateTime.Now-startT).TotalMilliseconds;

        Debug.Log($"RefreshPipe End :{t1:F1}ms,{t2:F1}ms,{t3:F1}ms,{t4:F1}ms");
    }

    internal void ModifyPoint(PipePoint currentPoint)
    {
        Debug.Log($"PipeSystemComponent.ModifyPoint currentPoint:{currentPoint}");
        RenderPipe();
        PipeSystemUtils.RefeshPointObjects(this);
    }

    internal void AddPoint(PipePoint newPoint)
    {
        if(pipeSystem.Points==null){
            pipeSystem.Points=new List<PipePoint>();
        }
        UnityEngine.Debug.Log($"PipeSystemComponent.AddPoint_1 Points:{pipeSystem.Points.Count}");
        pipeSystem.Points.Add(newPoint);
        UnityEngine.Debug.Log($"PipeSystemComponent.AddPoint_2 Points:{pipeSystem.Points.Count}");
        RenderPipe();
    }

    internal int InsertPoint(PipePoint newPoint,PipePoint currentPoint)
    {
        if(pipeSystem.Points==null){
            pipeSystem.Points=new List<PipePoint>();
        }
        UnityEngine.Debug.Log($"PipeSystemComponent.InsertPoint_1 Points:{pipeSystem.Points.Count}");
        //var p=pipeSystem.
        int id=pipeSystem.InsertPoint(newPoint,currentPoint);
        UnityEngine.Debug.Log($"PipeSystemComponent.InsertPoint_2 Points:{pipeSystem.Points.Count}");
        RenderPipe();
        return id;
    }

    internal int DeletePoint(PipePoint point)
    {
        if(pipeSystem.Points==null){
            pipeSystem.Points=new List<PipePoint>();
        }
        int id=pipeSystem.DeletePoint(point);
        PipeSystemUtils.DeletePointUI(point.GetKey());
        RenderPipe();
        return id;
    }


    void Start()
    {
        DoubleClickEventTrigger_u3d trigger = DoubleClickEventTrigger_u3d.Get(gameObject);
        trigger.onClick = OnClick;
    }

    public static PipeSystemComponent currentPipeSystem;

    public static void ClearLastPipeSystem()
    {
        if(currentPipeSystem!=null){
            PipeSystemUtils.HidePointPoints();
            HightlightModuleBase.HighlightOff(currentPipeSystem.gameObject);
        }
    }

    public void OnClick()
    {
        if(PipeSystemEditEventListener.Instance.IsGraging){
            Debug.Log($"PipeSystemComponent.OnClick[{this}] IsGraging ");
            return;
        }
        if(currentPipeSystem==this){
            Debug.Log($"PipeSystemComponent.OnClick[{this}] currentPipeSystem==this ");
            //return;
        }

        if(currentPipeSystem!=this){
            ClearLastPipeSystem();

            DateTime startT = DateTime.Now;
            Debug.Log($"PipeSystemComponent.OnClick[{this}]");
            //HighlighterHelper.HighlightOn(this.gameObject);
            HightlightModuleBase.HighlightOn(this.gameObject);

            UIManager.GetInstance().ShowUIPanel("PipeSystemInfoPanel");
            MessageCenter.SendMsg(MsgType.PipeSystemInfoPanelMsg.TypeName, MsgType.PipeSystemInfoPanelMsg.InitData, pipeSystem);

            // MessageCenter.SendMsg(MsgType.PipeSystemAddPanelMsg.TypeName, MsgType.PipeSystemAddPanelMsg.InitData, pipeSystem);

            Debug.Log($"PipeSystemComponent.OnClick[{this}] 1 time1:{DateTime.Now-startT}");
            ShowPipePointsFollowUI();
            Debug.Log($"PipeSystemComponent.OnClick[{this}] 2 time1:{DateTime.Now-startT}");

            if(PipeSystemBuilder.Instance.IsPipeEditMode)
            {
                UIManager.GetInstance().ShowUIPanel("PipeSystemEditUIPanel");
                MessageCenter.SendMsg(MsgType.PipeSystemEditPanelMsg.TypeName, MsgType.PipeSystemEditPanelMsg.InitData, pipeSystem);
            }
            else{

            }

            //PipePointFollowUI.SetIsEditMode(PipeSystemBuilder.Instance.IsPipeEditMode);

            currentPipeSystem=this;

            MessageCenter.SendMsg(MsgType.PipeSystemTreePanelMsg.TypeName, MsgType.PipeSystemTreePanelMsg.SelectNode, pipeSystem);

            PipeSystemUtils.RTESelectObject(this.gameObject);
        }
        else{
            if(PipeSystemBuilder.Instance.IsPipeEditMode)
            {
                UIManager.GetInstance().ShowUIPanel("PipeSystemEditUIPanel");
                MessageCenter.SendMsg(MsgType.PipeSystemEditPanelMsg.TypeName, MsgType.PipeSystemEditPanelMsg.InitData, pipeSystem);
            }
            else{

            }
            //PipePointFollowUI.SetIsEditMode(PipeSystemBuilder.Instance.IsPipeEditMode);
        }
    }

    void OnDisable()
    {
        PipeSystemUtils.HidePointPoints();
    }

    void OnEnable()
    {
        //ShowPointPoints();
    }

    void OnDestroy()
    {
        PipeSystemUtils.ClearPointPoints();
    }

    public Vector3 GetPointVector3(PipePoint point){
        // Vector3 p=new Vector3(point.X,point.Y,point.Z)-minOff;
        // return new Vector3(p.x,p.z,p.y);//坐标系转换

        Vector3 p2=PipeSystemPointConverter.Instance.CadToUnityPosEx(point);
        return p2;
    }

    [ContextMenu("ShowPipePointsFollowUI")]
    public void ShowPipePointsFollowUI(){
        //FollowTargetManage.Instance.CreateDevFollowUI
        PipeSystemUtils.ClearPointPoints();
        PipeSystemUtils.CreatePipePointsFollowUI(this,this.pipeSystem.Points);
    }



    public void FocusPoint(PipePoint point)
    {
        GameObject pipeObj=PipeSystemUtils.CreatePipePointFollowUI(this,point);
        // HightlightModuleBase.HighlightOff(this.gameObject);
        PipeSystemUtils.FocusPipePointObj(pipeObj);
    }

    [ContextMenu("RefreshPipe")]
    public void RefreshPipe()
    {
        var arg=PipeSystemBuilder.Instance.GetPipeSysGenerateArg(this.pipeSystem);
        GeneratePipe(arg);
    }

    internal void ChangePipeType(PipeType pipeType)
    {
        // //RenderPipe();
        // var arg=PipeSystemBuilder.Instance.GetPipeSysGenerateArg(this.pipeSystem);
        // GeneratePipe(arg);
        RefreshPipe();

        MeshRenderer renderer=this.GetComponent<MeshRenderer>();
        if(renderer){
            renderer.material=PipeSystemBuilder.Instance.GetPipeMaterial(pipeType);
        }
    }

    // internal void EditPos(Vector3 vector3)
    // {
    //     pipeSystem.PosX+=vector3.x;
    //     pipeSystem.PosY+=vector3.y;
    //     pipeSystem.PosZ+=vector3.z;
    //     //PipeSystemClient.EditPipeSystem(pipeSystem,false);
    // }

    internal void EditPos(Vector3 posOld,Vector3 posNew)
    {
        var posOff=posNew-posOld;
        pipeSystem.PosX+=posOff.x;
        pipeSystem.PosY+=posOff.y;
        pipeSystem.PosZ+=posOff.z;
        //PipeSystemClient.EditPipeSystem(pipeSystem,false);
    }

    public Vector3 GetNewPos(Vector3 posOld,Vector3 posNew)
    {
        var posOff=posNew-posOld;
        Vector3 oriPos=pipeSystem.GetPos();
        return oriPos+posOff;
    }
}
