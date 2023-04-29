using System.Collections;
using System.Collections.Generic;
using DbModel.Location.Pipes;
using UnityEngine;
using UnityEngine.UI;
using Y_UIFramework;

public class PipeSystemInfoPanel : UIBasePanel
{
    public GameObject PointItemPrefab;

    public Text txtName;
    public Text txtType;
    public Text txtCode;
    public Text txtPoints;

    public GameObject pointListPanel;

    void Awake()
    {
        this.CurrentUIType.UIPanels_Type=UIPanelType.Normal;
        Debug.Log($"PipeSystemInfoPanel.Awake CurrentUIType:{this.CurrentUIType}");

        RegisterMsgListener(MsgType.PipeSystemInfoPanelMsg.TypeName,
            obj =>
            {
                Debug.Log($"PipeSystemInfoPanel.RegisterMsg obj.Key:{obj.Key} obj.Values:{obj.Values}");
                if (obj.Key == MsgType.PipeSystemInfoPanelMsg.InitData)
                {
                    InitData(obj.Values as PipeSystem);
                }
                else
                {
                    Debug.LogError($"PipeSystemInfoPanel.RegisterMsg obj.Key:{obj.Key} obj.Values:{obj.Values}");
                }
            }
       );
    }

    public void InitData(PipeSystem pipeSys){
        if(pipeSys==null) return;
        Debug.Log($"PipeSystemInfoPanel.InitData pipeSys:{pipeSys}");
        txtName.text="名称："+pipeSys.Name;
        txtType.text="类型："+pipeSys.Type;
        txtCode.text="编码："+pipeSys.Code;
        // if(txtDetail)
        //     txtDetail.text=pipeSys.Name;
        if(pipeSys.Points!=null)
        {
            txtPoints.text="顶点数："+pipeSys.Points.Count;
            if(PointItemPrefab){
                foreach(var point in pipeSys.Points)
                {
                    AddPipePoint(point);
                }
            }
        }
        else{
            txtPoints.text="顶点数：0";
        }
    }

    private void AddPipePoint(PipePoint point){
        if(PointItemPrefab==null)return;
        GameObject pointItem=GameObject.Instantiate(PointItemPrefab);
        pointItem.SetActive(true);
        pointItem.transform.SetParent(pointListPanel.transform);
        //pointItem.transform.rotation=Quaternion.identity;
        pointItem.transform.rotation=Quaternion.Euler(10,20,30);
        pointItem.transform.localScale=Vector3.one;
        // RectTransform rect=pointItem.GetComponent<RectTransform>();
        // rect.rotation
        PipePointItemUI pointItemUI=pointItem.GetComponent<PipePointItemUI>();
        pointItemUI.InitData(point);
    }

    [ContextMenu("TestAddPoint1")]
    public void TestAddPoint1(){
         GameObject pointItem=GameObject.Instantiate(PointItemPrefab);
         pointItem.transform.rotation=Quaternion.identity;
         pointItem.transform.localScale=Vector3.one;
        pointItem.transform.SetParent(pointListPanel.transform);
    }

    [ContextMenu("TestAddPoint2")]
    public void TestAddPoint2(){
         GameObject pointItem=GameObject.Instantiate(PointItemPrefab);
        pointItem.transform.SetParent(pointListPanel.transform);
        pointItem.transform.rotation=Quaternion.identity;
         pointItem.transform.localScale=Vector3.one;
    }
}
