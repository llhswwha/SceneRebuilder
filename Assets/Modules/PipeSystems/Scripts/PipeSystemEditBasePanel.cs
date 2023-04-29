using System;
using System.Collections;
using System.Collections.Generic;
using DbModel.Location.Pipes;
using UnityEngine;
using UnityEngine.UI;
using Y_UIFramework;

public class PipeSystemEditBasePanel : UIBasePanel
{
    public InputField txtName;

    public InputField txtSize;

    //public InputField txtType;

    public Dropdown dropType;

    public Image pipeTypeImage;

    public InputField txtCode;

    protected PipeType[] pipeTypes;

    protected void InitPipeSystemTypes(bool isInit){
        
        PipeSystemBuilder.Instance.GetPipeTypes(types=>{
            if(types!=null){
                this.pipeTypes=types;
                List<string> pipeTypeNames=new List<string>();
                foreach(var pipeType in types){
                    pipeTypeNames.Add(pipeType.Name);
                }
                Debug.Log($"PipeSystemEditBasePanel.InitPipeSystemTypes types:{types.Length} ");
                dropType.ClearOptions();
                dropType.AddOptions(pipeTypeNames);
                dropType.value=0;
                if(isInit){
                    OnPipeTypeChanged(0);
                }
            }
            else{
                Debug.LogError("PipeSystemEditBasePanel.InitPipeSystemTypes types == null 123");
            }
            dropType.onValueChanged.AddListener(OnPipeTypeChanged);
        });
        
    }

    protected int GetPipeTypeIndex(string typeName){
        if(pipeTypes==null){
            Debug.LogError($"GetPipeTypeIndex pipeTypes==null typeName:{typeName}");
            return -1;
        }
        for(int i=0;i<pipeTypes.Length;i++){
            if(pipeTypes[i].Name==typeName){
                return i;
            }
        }
        Debug.LogError($"GetPipeTypeIndex Not Found typeName:{typeName}");
        return -1;
    }

    //public PipeType currentPipeType;

    protected virtual void OnPipeTypeChanged(int n)
    {
        var pipeType=pipeTypes[n];
        Debug.Log($"OnPipeTypeChanged n:{n} currentPipeType:{pipeType}");
        Material mat=PipeSystemBuilder.Instance.GetPipeMaterial(pipeType);
        if(mat==null){
            Debug.LogError($"OnPipeTypeChanged mat==null n:{n} pipe:{pipeType}");
            return;
        }
        if(pipeTypeImage)
            pipeTypeImage.color=mat.GetColor("_Color");
    }

    public string GetCurrentPipeTypeName(){
        if(dropType==null)return "";
        Debug.Log($"GetCurrentPipeTypeName dropType:{dropType.value}");
        if(pipeTypes==null || pipeTypes.Length < dropType.value)return "";
        return pipeTypes[dropType.value].Name;
    }

    //public List<PipePoint> CurrentPipePoints = new List<PipePoint>();

    public List<List<PipePoint>> CurrentPipeGroups=new List<List<PipePoint>>();

    public PipeSystem GetNewPipeSystem(List<PipePoint> points,string type,string tag){
        PipeSystem pipeSys=new PipeSystem();
        pipeSys.Name=txtName.text+tag;
        pipeSys.SizeX=float.Parse(txtSize.text);
        pipeSys.Type=type;
        pipeSys.Code=txtCode.text;
        //var points = ReadCSVFile(CurrentCSVFile);
        //pipeSystem.Points=points;
        pipeSys.Points=points;
        return pipeSys;
    }

    public PipeSystem GetNewPipeSystem(List<PipePoint> points){
        string pipeType=GetCurrentPipeTypeName();
        return GetNewPipeSystem(points,pipeType,"");
    }

    public PipeSystem GetNewPipeSystem(){
        if(CurrentPipeGroups.Count>0){
            return GetNewPipeSystem(CurrentPipeGroups[0]);
        }
        else{
            return null;
        }
    }

    public List<PipeSystem> GetNewPipeSystemList(){
        List<PipeSystem> list=new List<PipeSystem>();
        string pipeType=GetCurrentPipeTypeName();
        var pipeGroup=this.CurrentPipeGroups;
        if(pipeGroup.Count==1){
            PipeSystem pipeSystem=GetNewPipeSystem(pipeGroup[0],pipeType,"");
            list.Add(pipeSystem);
        }
        else{
            for (int i = 0; i < pipeGroup.Count; i++)
            {
                List<PipePoint> ps = pipeGroup[i];
                PipeSystem pipeSystem=GetNewPipeSystem(ps,pipeType,$"_{i+1}");
                list.Add(pipeSystem);
            }
        }
        return list;
    }

    public PipeSystem ChangedPipeSystem(){
        PipeSystem pipeSys=this.pipeSystem;
        pipeSys.Name=txtName.text;
        pipeSys.SizeX=float.Parse(txtSize.text);
        pipeSys.Type=GetCurrentPipeTypeName();
        pipeSys.Code=txtCode.text;
        return pipeSys;
    }

    protected PipeSystem pipeSystem;

    public List<PipeSystem> pipeSystems=new List<PipeSystem>();

    public GameObject currentPipeSystemObj;

    //public List<GameObject> currentPipeSystemObjList=new List<GameObject>();

    public Dictionary<PipeSystem,GameObject> currentPipeSystemObjDict=new Dictionary<PipeSystem,GameObject>();

    protected void ShowPipeSystem(PipeSystem pipeSys){
        var pipeSystemObj = PipeSystemBuilder.Instance.ShowPipeSystem(pipeSys);
        //currentPipeSystemObjList.Add(pipeSystemObj);
        currentPipeSystemObjDict.Add(pipeSys,pipeSystemObj);
        PipeSystemBuilder.Instance.FocusPipeSystemObject(pipeSystemObj);
        this.transform.localPosition=new Vector3(700,0,0);

        currentPipeSystemObj=pipeSystemObj;
    }

    protected void DestroyPipeSystem(){
        if(currentPipeSystemObj){
            GameObject.DestroyImmediate(currentPipeSystemObj);
        }
    }

    protected void DestroyPipeSystems(){
        Debug.Log($"DestroyPipeSystems count:{currentPipeSystemObjDict.Count}");
        foreach(var item in currentPipeSystemObjDict.Values){
            GameObject.DestroyImmediate(item);
        }
        currentPipeSystemObjDict.Clear();

        DestroyPipeSystem();
    }

    public void AddPipeSystem(){

        DestroyPipeSystem();

        var pipeSysList=GetNewPipeSystemList();

        // PipeSystem pipeSys=null;
        // if(pipeSysList.Count>0){
        //     pipeSys=pipeSysList[0];
        //     this.pipeSystem=pipeSys;
        // }
        // if(pipeSys==null){
        //     Debug.LogError($"AddPipeSystem pipeSys==null");
        //     return;
        // }

        //pipeSystem = GetNewPipeSystem();
        
        if(pipeSysList.Count==1){
            var pipeSys=pipeSysList[0];
            PipeSystemClient.AddPipeSystem(pipeSys,result=>{
                DestroyPipeSystem();
                PipeSystemBuilder.Instance.ShowPipeSystem(pipeSys);
                //ShowPipeSystem(pipeSys);
                UGUIMessageBox.Show($"添加管线[{pipeSys.Name}]成功!");
                Hide();
            });
        }
        else{
            StartCoroutine(AddPipeSystems(pipeSysList));
        }
    }

    private IEnumerator AddPipeSystems(List<PipeSystem> pipeSysList)
    {
        Debug.Log($"AddPipeSystems pipeSysList:{pipeSysList.Count}");
        int maxCount=pipeSysList.Count;
        if(PipeSystemBuilder.Instance.MaxPipeSystemCount>0 && PipeSystemBuilder.Instance.MaxPipeSystemCount<maxCount){
            maxCount=PipeSystemBuilder.Instance.MaxPipeSystemCount;
        }
        bool isAdded=false;
        for (int i = 0; i < maxCount; i++)
        {
            PipeSystem pipeSys = pipeSysList[i];
            float p=i/(maxCount+0.0f);
            //pipeSys.Name="_"+(i+1);
            ProgressbarLoad.Instance.Show(p,$"添加管道[{pipeSys.Name}]");
            isAdded=false;
            PipeSystemClient.AddPipeSystem(pipeSys,result=>{
                isAdded=true;
            });
            while(isAdded==false){
                yield return null;
            }
            yield return null;
        }

        DestroyPipeSystems();

        ShowAllPipeSystems(pipeSysList,()=>{
            UGUIMessageBox.Show($"添加多段管线成功!数量:{pipeSysList.Count}");
            Hide();
        });
    }

    protected void ShowAllPipeSystems(List<PipeSystem> list,Action callback=null)
    {
        PipeSystemBuilder.Instance.ShowAllPipeSystems(list,(i,pipeSys,obj)=>{
            currentPipeSystemObjDict.Add(pipeSys,obj);
        },callback);
    }

    protected virtual void InitData(PipeSystem pipeSys)
    {
        this.pipeSystem=pipeSys;

        txtName.text=pipeSys.Name;
        txtSize.text=pipeSys.SizeX+"";

        int typeId=GetPipeTypeIndex(pipeSys.Type);
        if(dropType.value==typeId)
        {
            OnPipeTypeChanged(typeId);
        }
        else{
            dropType.value=typeId;
        }
        
        txtCode.text=pipeSys.Code;

        AddPipePoints();
    }

    [ContextMenu("RefreshPipePoints")]
    protected void RefreshPipePoints(GameObject obj,int id){
        Debug.Log($"RefreshPipePoints id:{id} obj:{obj}");
        List<Transform> list=new List<Transform>();
        for(int i=0; i< pointListPanel.transform.childCount;i++){
            Transform t1=pointListPanel.transform.GetChild(i);
            list.Add(t1);
            t1.SetParent(null);
        }
        for(int i=0; i< list.Count;i++){
            Transform t1=list[i];
            t1.SetParent(pointListPanel.transform);
            if(i==id){
                 obj.transform.SetParent(pointListPanel.transform);
            }
        }
    }

    private void ClearPipePointItems()
    {
        PipeSystemUtils.ClearPipePointItems();
        
        if(pointListPanel){
            List<Transform> ts=new List<Transform>();
            for(int i=0;i<pointListPanel.transform.childCount;i++){
                ts.Add(pointListPanel.transform.GetChild(i));
            }
            foreach(var t in ts){
                GameObject.DestroyImmediate(t.gameObject);
            }
        }
    }

    [ContextMenu("AddPipePoints")]
    protected void AddPipePoints(){
        var pipeSys=pipeSystem;
        if(pipeSys.Points!=null)
        {
            if(txtPoints)txtPoints.text="顶点数："+pipeSys.Points.Count;

            // foreach(var pipeObj in pointItemDict.Values)
            // {
            //     GameObject.DestroyImmediate(pipeObj);
            // }
            // pointItemDict.Clear();

            //pointListPanel.transform

            ClearPipePointItems();

            pipeSys.SortPoints();

            if(PointItemPrefab)
            {
                Debug.Log($"AddPipePoints count:{pipeSys.Points.Count} children:{pointListPanel.transform.childCount}");
                for (int i = 0; i < pipeSys.Points.Count; i++)
                {
                    PipePoint point = pipeSys.Points[i];
                    //Debug.Log($"AddPipePoints[{i}] point:{point} children:{pointListPanel.transform.childCount}");
                    AddPipePoint(point);
                }
            }
            else{
                Debug.LogError($"AddPipePoints PointItemPrefab==null this:{this}");
            }
        }
        else{
            if(txtPoints)txtPoints.text="顶点数：0";
        }
    }

    public Text txtPoints;

    public GameObject PointItemPrefab;

    public GameObject pointListPanel;

    [ContextMenu("TestAddPipePoint")]
    private void TestAddPipePoint()
    {
        AddPipePoint(new PipePoint());
    }

    protected GameObject AddPipePoint(PipePoint point){
        GameObject pointItem=NewPipePoint(point);
        if(pointItem==null)return null;
        pointItem.transform.SetParent(pointListPanel.transform);
        pointItem.transform.localRotation=Quaternion.Euler(0,0,0);
        pointItem.transform.localScale=Vector3.one;
        return pointItem;
    }

    protected GameObject NewPipePoint(PipePoint point){
        if(PointItemPrefab==null)return null;
        GameObject pointItem=GameObject.Instantiate(PointItemPrefab);
        pointItem.SetActive(true);
        // pointItem.transform.SetParent(pointListPanel.transform);
        // pointItem.transform.localRotation=Quaternion.Euler(0,0,0);
        // pointItem.transform.localScale=Vector3.one;
        PipePointItemUI pointItemUI=pointItem.GetComponent<PipePointItemUI>();
        pointItemUI.InitData(point);
        pointItemUI.OnDeleteEvent+=DeletePoint;
        pointItemUI.OnLocateEvent+=LocatePoint;
        pointItemUI.OnSelectEvent+=SelectPoint;

        // if(pointItemDict.ContainsKey(point.Id)){
        //     pointItemDict[point.Id]=pointItem;
        // }
        // else{
        //     pointItemDict.Add(point.Id,pointItem);
        // }

        PipeSystemUtils.AddPipePointItemUI(point,pointItem);
        return pointItem;
    }

    protected virtual void SelectPoint(PipePoint point){
        Debug.Log($"SelectPoint point:{point} Start");
        
    }

    public void DeletePoint(PipePoint point){
        var pipeSys=this.pipeSystem;
        PipeSystemClient.DeletePoint(pipeSys,point,()=>{
            txtPoints.text="顶点数："+pipeSys.Points.Count;
        });
    }

    private void LocatePoint(PipePoint point){
        PipeSystemBuilder.Instance.LocatePoint(point);
    }

    public void EditPipeSystem()
    {
        EditPipeSystemInner(true);
    }

    public void EditPipeSystemInner(bool isShowMessageBox){
        var pipeSys=this.pipeSystem;
        ChangedPipeSystem();
        // Debug.Log($"EditPipeSystem Start pipeSystem:{pipeSys.Name}");
        // CommunicationObject.Instance.EditPipeSystem(pipeSys,result=>{
        //     //pipeSystem=result;

        //     if(result!=null){
        //         pipeSys.Id=result.Id;
        //         Debug.Log($"EditPipeSystem End result:{result}");
        //         //ShowPipeSystem(pipeSystem);
                
        //         MessageCenter.SendMsg(MsgType.PipeSystemTreePanelMsg.TypeName, MsgType.PipeSystemTreePanelMsg.EditNode, pipeSys);
        //         MessageCenter.SendMsg(MsgType.PipeSystemInfoPanelMsg.TypeName, MsgType.PipeSystemInfoPanelMsg.InitData, pipeSys);

        //         if(isShowMessageBox){
        //             UGUIMessageBox.Show($"修改管线[{pipeSys.Name}]成功!");
        //         }
        //     }
        //     else{
        //         Debug.LogError($"EditPipeSystem result == null");
        //     }
        // });

        PipeSystemClient.EditPipeSystem(pipeSys,isShowMessageBox);
    }

    public void DeletePipeSystem(){
        var pipeSys=this.pipeSystem;
        PipeSystemClient.DeletePipeSystem(pipeSys,true,(result)=>{
            this.pipeSystem=null;
            UGUIMessageBox.Show($"删除管线[{pipeSys.Name}]成功!");
        });
    }
}
