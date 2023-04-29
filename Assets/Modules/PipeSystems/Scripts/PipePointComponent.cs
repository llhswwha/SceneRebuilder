using System.Collections;
using System.Collections.Generic;
using DbModel.Location.Pipes;
using UnityEngine;
using Mathd;

public class PipePointComponent : MonoBehaviour
{
    public PipeSystem pipeSys;

    public PipePoint point;

    [ContextMenu("RefreshSelf")]
    public void RefreshSelf()
    {
        PipeSystemBuilder.Instance.RefreshPipe(this);
    }

    private bool isRefresh=false;

    [ContextMenu("StartRefreshTest")]
    public void StartRefreshTest()
    {
        isRefresh=true;
    }

    [ContextMenu("StopRefreshTest")]
    public void StopRefreshTest()
    {
        isRefresh=false;
    }

    void Update()
    {
        if(isRefresh)
            RefreshSelf();
    }

    // [ContextMenu("ShowInfo")]
    // private void ShowInfo()
    // {
    //     Vector3 pos01 = GetPos();
    //     Vector3 pos02 = PipeSystemPointConverter.Instance.CadToUnityPosEx(point);
    //     Vector3d pos11 = PipeSystemPointConverter.Instance.UnityPosToCadEx(pos01);
    //     Vector3d pos12 = PipeSystemPointConverter.Instance.UnityPosToCadEx(pos02);
    //     Debug.Log($"ShowInfo point:{point} pos01:{pos01} pos02:{pos02} pos11:{pos11} pos12:{pos12}");
    // }

    private void TestSetPoint()
    {
        
    }

    public static Vector3 GetPointVector3(PipePoint point){
        Vector3 p2=PipeSystemPointConverter.Instance.CadToUnityPosEx(point);
        return p2;
    }

    public void InitData(PipeSystem pipeSys,PipePoint point,float radius)
    {
        this.pipeSys=pipeSys;
        this.point=point;

        this.name += point.ToString();
        this.gameObject.layer = LayerMask.NameToLayer("DepDevice");

        Vector3 p=GetPointVector3(point);//CAD坐标对应的三维坐标
        //float radius=pipeSysCom.generator.pipeRadius;
        this.transform.localScale = new Vector3(radius,radius,radius);
        //obj.transform.position = p + pipeSysCom.GetPipeSysPos();//顶点坐标加上管道坐标
        this.transform.position = p + pipeSys.GetPos();//顶点坐标加上管道坐标
        //this.transform.SetParent(pipeSysCom.transform);
    }

    public Vector3 GetPos()
    {
        //return this.transform.position-pipeSys.GetPos();
        return this.transform.localPosition;
    }

    public Vector3 GetCurrentPos()
    {
        //Debug.Log($"PipePointComponent.GetCurrentPos 1 point:{point}");
        //Vector3 pos01 = this.transform.position;
        Vector3 pos01 = GetPos();
        //Vector3 pos02 = PipeSystemPointConverter.Instance.CadToUnityPosEx(point);
        Vector3d pos11 = PipeSystemPointConverter.Instance.UnityPosToCadEx(pos01);
        //Vector3 pos12 = PipeSystemPointConverter.Instance.UnityPosToCadEx(pos02);
        //Debug.Log($"PipePointComponent.GetCurrentPos 2 point:{point} pos01:{pos01} pos02:{pos02} pos11:{pos11} pos12:{pos12}");
        //Debug.Log($"PipePointComponent.GetCurrentPos 2 point:{point} pos01:{pos01} pos02 pos11:{pos11}");
        point.X=pos11.x;
        point.Y=pos11.y;
        point.Z=pos11.z;
        return pos11;
    }

    internal void RefreshPos(Vector3 posOld,Vector3 posNew)
    {
        PipeSystemBuilder.Instance.RefreshPipe(this);
    }

    internal void EditPos(Vector3 posOld,Vector3 posNew)
    {
        // var posOff=posNew-posOld;
        // point.X+=posOff.x;
        // point.Y+=posOff.y;
        // point.Z+=posOff.z;
        //PipeSystemClient.EditPipeSystem(pipeSystem,false);

        PipeSystemUtils.EditPipePoint(this);
    }

    // public Vector3 GetNewPos(Vector3 posOld,Vector3 posNew)
    // {
    //     var posOff=posNew-posOld;
    //     Vector3 oriPos=GetPos();
    //     return oriPos+posOff;
    // }
}
