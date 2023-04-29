using System;
using System.Collections;
using System.Collections.Generic;
using DbModel.Location.Pipes;
using UnityEngine;
using Y_UIFramework;

public static class PipeSystemClient //: MonoBehaviour
{
    private static string SetPoints(PipeSystem pipeSys,PipeSystem result)
    {
        string pId="";
        if(result.Points!=null)
        {
            for (int i = 0; i < result.Points.Count; i++)
            {
                PipePoint p = result.Points[i];
                pId +=p.Id+",";
                PipePoint p0=pipeSys.Points[i];
                if(p0.Num!=p.Num){
                    Debug.LogError($"AddPipeSystem[{i}] p0.Num!=p.Num p:{p} | p0:{p0} ");
                }
                else{
                     p0.Id=p.Id;
                }
                p0.PId=p.PId;
                //Debug.LogError($"AddPipeSystem[{i}] p0.Num!=p.Num p:{p} | p0:{p0} ");
            }
        }
        return pId;
    }

    public static void AddPipeSystem(PipeSystem pipeSys,Action<PipeSystem> callback)
    {
        Debug.Log($"AddPipeSystem Start pipeSystem:{pipeSys.Name}");
        CommunicationObject.Instance.AddPipeSystem(pipeSys,result=>{
            //Debug.Log($"AddPipeSystem End result:{result}");
            if(result!=null){
                pipeSys.Id=result.Id;
                int pCount=0;
                string pId=SetPoints(pipeSys,result);
                Debug.Log($"AddPipeSystem_End result:{result} pCount:{pCount} pId:{pId} ");

                MessageCenter.SendMsg(MsgType.PipeSystemTreePanelMsg.TypeName, MsgType.PipeSystemTreePanelMsg.AddNode, pipeSys);

                if(callback!=null){
                    callback(pipeSys);
                }
            }
            else{
                Debug.LogError($"AddPipeSystem result == null");
            }
        });
    }

    public static void DeletePipeSystem(PipeSystem pipeSys,bool isShowMessageBox,Action<PipeSystem> callback)
    {
        Debug.Log($"DeletePipeSystem Start1 pipeSystem:{pipeSys.Name}");

        if(isShowMessageBox){
            UGUIMessageBox.Show($"确定删除管线[{pipeSys.Name}]？",()=>{
                DeletePipeSystemInner(pipeSys,callback);
            },()=>{

            });
        }
        else{
            DeletePipeSystemInner(pipeSys,callback);
        }
    }

    public static void DeletePipeSystemInner(PipeSystem pipeSys,Action<PipeSystem> callback)
    {
        Debug.Log($"DeletePipeSystem Start2 pipeSystem:{pipeSys.Name} Id:{pipeSys.Id}");
            CommunicationObject.Instance.DeletePipeSystem(pipeSys.Id,result=>{
                try
                {
                    if(result!=null){
                        //pipeSystem.Id=result.Id;
                        Debug.Log($"DeletePipeSystem End result:{result}");
                        //ShowPipeSystem(pipeSystem);
                        //UGUIMessageBox.Show($"删除管线[{pipeSys.Name}]成功!");
                        MessageCenter.SendMsg(MsgType.PipeSystemTreePanelMsg.TypeName, MsgType.PipeSystemTreePanelMsg.DeleteNode, pipeSys);

                        var pipeSysCom=PipeSystemBuilder.Instance.GetPipeSystemComponent(pipeSys.Id);
                        PipeSystemUtils.ClearPointPoints();
                        if(pipeSysCom)GameObject.DestroyImmediate(pipeSysCom.gameObject);

                        PipeSystemUtils.HidePipeSystemUI();
                    }
                    else{
                        Debug.LogError($"DeletePipeSystem result == null ");
                    }
                    //this.pipeSystem=null;
                    if(callback!=null){
                        callback(result);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"DeletePipeSystem Exception:{ex}");
                }
            });
    }

    public static void EditPipeSystem(PipeSystem pipeSys,bool isShowMessageBox){
        // var pipeSys=this.pipeSystem;
        // ChangedPipeSystem();
        Debug.Log($"EditPipeSystem Start pipeSystem:{pipeSys.Name}");
        CommunicationObject.Instance.EditPipeSystem(pipeSys,result=>{
            //pipeSystem=result;

            if(result!=null){
                pipeSys.Id=result.Id;
                Debug.Log($"EditPipeSystem End result:{result}");
                //ShowPipeSystem(pipeSystem);
                
                MessageCenter.SendMsg(MsgType.PipeSystemTreePanelMsg.TypeName, MsgType.PipeSystemTreePanelMsg.EditNode, pipeSys);
                MessageCenter.SendMsg(MsgType.PipeSystemInfoPanelMsg.TypeName, MsgType.PipeSystemInfoPanelMsg.InitData, pipeSys);

                if(isShowMessageBox){
                    UGUIMessageBox.Show($"修改管线[{pipeSys.Name}]成功!");
                }
            }
            else{
                Debug.LogError($"EditPipeSystem result == null");
            }
        });
    }

    public static void DeletePoint(PipeSystem pipeSys, PipePoint point,Action callback){
        //var pipeSys=this.pipeSystem;
        Debug.Log($"DeletePoint Start0 point:{point}");
        UGUIMessageBox.Show($"确定删除顶点[{point.ToString()}]？",()=>{
            Debug.Log($"DeletePoint point:{point} Start  ");
            CommunicationObject.Instance.DeletePipePoint(point.Id,result=>{
            Debug.Log($"DeletePoint point:{point} End result:{result}");

            PipeSystemUtils.DestoryPipePointItem(point);

            var pipeSysCom=PipeSystemBuilder.Instance.GetPipeSystemComponent(pipeSys.Id);
            int id=pipeSysCom.DeletePoint(point);

            if(id==-1 || id > pipeSys.Points.Count-1 )
            {
                id=pipeSys.Points.Count-1;
            }
            var pointNew=pipeSys.Points[id];
            pipeSysCom.FocusPoint(pointNew);

            //txtPoints.text="顶点数："+pipeSys.Points.Count;
            if(callback!=null){
                callback();
            }
        });
        },()=>{

        });
        
    }

    public static void InsertPipePoint(PipeSystem pipeSys, PipePoint currentPoint,PipePoint pipePoint,Action<PipePoint> callback)
    {
        try
        {
            // var pipeSys=this.pipeSystem;
            Debug.Log($"InsertPipePoint_1 currentPoint:{currentPoint}");
            if(pipeSys.Points==null){
                pipeSys.Points=new List<PipePoint>();
            }
            // PipePoint pipePoint=GetPipePoint();
            // pipePoint.Num=currentPoint.Num;
            Debug.Log($"InsertPipePoint_2 pipePoint:{pipePoint}");
            CommunicationObject.Instance.AddPipePoint(pipePoint,result=>{
                if(result==null){
                    Debug.LogError($"InsertPipePoint_3 result==null");
                }
                else{
                    Debug.Log($"InsertPipePoint_3 pipePoint:{pipePoint}");
                    
                    PipeSystemComponent pipeSysCom=PipeSystemBuilder.Instance.GetPipeSystemComponent(pipeSys.Id);
                    int id=pipeSysCom.InsertPoint(result,currentPoint);

                    // GameObject obj=base.NewPipePoint(result);
                    // txtPoints.text="顶点数："+pipeSys.Points.Count;
                    // AddPipePoints();
                    
                    PipeSystemUtils.RefeshPointObjects(pipeSysCom);

                    pipeSysCom.FocusPoint(result);
                    
                    //RefreshPipePoints(obj,id);
                    if(callback!=null){
                        callback(result);
                    }
                    Debug.Log($"InsertPipePoint_4 pipePoint:{pipePoint}");
                    CommunicationObject.Instance.EditPipePoints(pipeSys,r1=>{
                        Debug.Log($"InsertPipePoint_5 pipePoint:{pipePoint} r1:{r1}");
                    });
                }
            });
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"AddPipePoint_4 Exceptio:{ex}");
        }
    }
}
