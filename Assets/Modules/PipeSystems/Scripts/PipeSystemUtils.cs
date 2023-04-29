using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Battlehub.RTCommon;
using DbModel.Location.Pipes;
using Mogoson.CameraExtension;
using UnityEngine;
using Y_UIFramework;

public static class PipeSystemUtils 
{
    public static Vector3 GetPointVector3(PipePoint point){
        Vector3 p2=PipeSystemPointConverter.Instance.CadToUnityPosEx(point);
        return p2;
    }

    public static Dictionary<string,GameObject> pointObjDict=new Dictionary<string, GameObject>();

    public static Dictionary<string,GameObject> pointUIDict=new Dictionary<string, GameObject>();

    public static void ClearPointPoints()
    {
        foreach(var obj in pointObjDict.Values){
            GameObject.DestroyImmediate(obj);
        }
        pointObjDict.Clear();
        foreach(var obj in pointUIDict.Values){
            GameObject.DestroyImmediate(obj);
        }
        pointUIDict.Clear();
    }

    public static void HidePointPoints()
    {
        foreach(var obj in pointObjDict.Values){
            if(obj==null)continue;
            obj.SetActive(false);
        }
        foreach(var obj in pointUIDict.Values){
            if(obj==null)continue;
            obj.SetActive(false);
        }
    }

    public static void ShowPointPoints()
    {
        foreach(var obj in pointObjDict.Values){
            if(obj==null)continue;
            obj.SetActive(true);
        }
        foreach(var obj in pointUIDict.Values){
            if(obj==null)continue;
            obj.SetActive(true);
        }
    }

    public static void DeletePointUI(string pointId){
        if(pointObjDict.ContainsKey(pointId)){
            GameObject.DestroyImmediate(pointObjDict[pointId]);
            pointObjDict.Remove(pointId);
        }
        else{
            Debug.LogError($"DeletePoint_Err1 pointObjDict.ContainsKey == false pointId:{pointId}");
        }
        if(pointUIDict.ContainsKey(pointId)){
            GameObject.DestroyImmediate(pointUIDict[pointId]);
            pointUIDict.Remove(pointId);
        }
        else{
            Debug.LogError($"DeletePoint_Err2 pointUIDict.ContainsKey == false pointId:{pointId}");
        }
    }

    public static GameObject GetPipeObject(PipePoint point)
    {
        string key=point.GetKey();
        if(pointObjDict.ContainsKey(key)){
            return pointObjDict[key];
        }
        return null;
    }

    public static GameObject CreatePipePointComponent(PipeSystemComponent pipeSysCom,PipePoint point)
    {
        //Vector3 p=GetPointVector3(point);//CAD?????????
        //GameObject obj=new GameObject(point.ToString());
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var pointComponent=obj.AddComponent<PipePointComponent>();
        float radius=pipeSysCom.generator.pipeRadius;
        pointComponent.InitData(pipeSysCom.pipeSystem,point,radius);
        obj.transform.SetParent(pipeSysCom.transform);
        return obj;
    }

    public static GameObject CreatePipePointFollowUI(PipeSystemComponent pipeSysCom,PipePoint point)
    {
        string key=point.GetKey();
        if(pointObjDict.ContainsKey(key)){
            return pointObjDict[key];
        }

        //GameObject obj=new GameObject(point.ToString());
        GameObject obj = CreatePipePointComponent(pipeSysCom,point);

        GameObject uiObj=PipeSystemBuilder.Instance.CreatePipePointFollowUI(obj,pipeSysCom.pipeSystem,point,null);
        
        if(PipeSystemBuilder.Instance.IsShowPointFollowUI){
            obj.SetActive(true);
            uiObj.SetActive(true);
        }
        else{
            obj.SetActive(false);
            uiObj.SetActive(false);
        }
        
        if(pointUIDict.ContainsKey(key)){
            Debug.LogError($"CreatePipePointFollowUI_Error1 pointUIDict.ContainsKey point:{point}");
        }
        else{
            pointUIDict.Add(key,uiObj);
        }
        
        if(pointObjDict.ContainsKey(key)){
            Debug.LogError($"CreatePipePointFollowUI_Error2 pointObjDict.ContainsKey point:{point}");
        }
        else{
            pointObjDict.Add(key,obj);
        }
        
        return obj;
    }

    
    public static void RefeshPointObjects(PipeSystemComponent pipeSysCom)
    {
        Debug.Log($"PipeSystemComponent.RefeshPointObjects pointObjDict:{pointObjDict.Count}");
        List<PipePoint> points=new List<PipePoint>();
        foreach(var key in pointObjDict.Keys){
            PipePoint point=pipeSysCom.pipeSystem.GetPoint(key);
            if(point == null)continue;
            points.Add(point);
        }

        ClearPointPoints();

        pointObjDict.Clear();
        CreatePipePointsFollowUI(pipeSysCom,points);
    }

    public static void CreatePipePointsFollowUI(PipeSystemComponent pipeSysCom,List<PipePoint> points)
    {
        if(points == null) return;
        foreach(var point in points){
            CreatePipePointFollowUI(pipeSysCom,point);
        }
    }

    public static Dictionary<string,GameObject> pointItemDict=new Dictionary<string, GameObject>();

    public static void DestoryPipePointItem(PipePoint point)
    {
        if(pointItemDict.ContainsKey(point.GetKey()))
        {
            GameObject pointItem=pointItemDict[point.GetKey()];
            GameObject.DestroyImmediate(pointItem);
        }
        else{
            Debug.LogError($"DestoryPipePointItem pointItemDict.ContainsKey==false point:{point}");
        }
    }

    public static void ClearPipePointItems()
    {
        Debug.Log($"PipeSystemUtils.ClearPipePointItems count:{pointItemDict.Count}");
        foreach(var pipeObj in pointItemDict.Values)
        {
            GameObject.DestroyImmediate(pipeObj);
        }
        pointItemDict.Clear();
    }

    public static void AddPipePointItemUI(PipePoint point,GameObject pointItem)
    {
        string key=point.GetKey();
        if(pointItemDict.ContainsKey(key)){
            pointItemDict[key]=pointItem;
        }
        else{
            pointItemDict.Add(key,pointItem);
        }
    }

    public static void LocatePipePoint(PipePoint point){
        if(pointItemDict.ContainsKey(point.GetKey()))
        {
            GameObject pointItem=pointItemDict[point.GetKey()];
            PipePointItemUI pointItemUI=pointItem.GetComponent<PipePointItemUI>();
            if(pointItemUI)pointItemUI.SetIsLocated();
        }
        else{
            Debug.LogError($"LocatePipePoint pointItemDict.ContainsKey==false point:{point}");
        }
    }

    public static void EditPipePoint(PipePoint currentPoint,bool isUpdateModel)
    {
        try
        {
            // Debug.Log($"EditPipePoint_1 currentPoint:{currentPoint}");
            // currentPoint.X=float.Parse(inputPointX.text);
            // currentPoint.Y=float.Parse(inputPointY.text);
            // currentPoint.Z=float.Parse(inputPointZ.text);
            Debug.Log($"EditPipePoint_2 currentPoint:{currentPoint}");
            CommunicationObject.Instance.EditPipePoint(currentPoint,result=>{
                Debug.Log($"EditPipePoint_3 currentPoint:{currentPoint}");
                if(pointItemDict.ContainsKey(currentPoint.GetKey()))
                {
                    GameObject pointItem=pointItemDict[currentPoint.GetKey()];
                    PipePointItemUI ui=pointItem.GetComponent<PipePointItemUI>();
                    ui.InitData(currentPoint);
                }
                if(isUpdateModel)
                    PipeSystemBuilder.Instance.ModifyPoint(currentPoint);
            });
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"EditPipePoint_4 Exceptio:{ex}");
        }
    }

    public static void UpdateUIInfo(PipePoint point)
    {
        //Debug.Log($"UpdateUIInfo point:{point}");

        if(pointItemDict.ContainsKey(point.GetKey()))
        {
            GameObject pointItem=pointItemDict[point.GetKey()];
            if(pointItem){
                PipePointItemUI ui=pointItem.GetComponent<PipePointItemUI>();
                ui.InitData(point);
            }
            else{
                
            }
        }

        string key=point.GetKey();
        if(pointUIDict.ContainsKey(key)){
            var ui=pointUIDict[key];
            PipePointFollowUI followUI=ui.GetComponent<PipePointFollowUI>();
            followUI.InitData(point);
        }
        else{
            
        }
    }

    public static void EditPipePoint(PipePointComponent pipeCom)
    {
        try
        {
            pipeCom.GetCurrentPos();
            PipePoint currentPoint=pipeCom.point;
            // Debug.Log($"EditPipePoint_1 currentPoint:{currentPoint}");
            // currentPoint.X=float.Parse(inputPointX.text);
            // currentPoint.Y=float.Parse(inputPointY.text);
            // currentPoint.Z=float.Parse(inputPointZ.text);
            Debug.Log($"EditPipePoint_2 currentPoint:{currentPoint}");
            CommunicationObject.Instance.EditPipePoint(currentPoint,result=>{
                Debug.Log($"EditPipePoint_3 currentPoint:{currentPoint}");
                // if(pointItemDict.ContainsKey(currentPoint.Id))
                // {
                //     GameObject pointItem=pointItemDict[currentPoint.Id];
                //     PipePointItemUI ui=pointItem.GetComponent<PipePointItemUI>();
                //     ui.InitData(currentPoint);
                // }
                UpdateUIInfo(currentPoint);
                PipeSystemBuilder.Instance.RefreshPipe(pipeCom);
            });
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"EditPipePoint_4 Exceptio:{ex}");
        }
    }

    public static void HidePipeSystemUI()
    {
        UIManager.GetInstance().CloseUIPanels("PipeSystemEditUIPanel");
        UIManager.GetInstance().CloseUIPanels("PipeSystemInfoPanel");
    }

    private static int FocusCount=0;

    public static void FocusPipePointObj(GameObject pipeObj,float distance=130)
    {
        if(pipeObj==null){
            Debug.LogError($"FocusPipePointObj pipeObj:{pipeObj} distance:{distance} 1");
            return;
        }
        if(lastPointObj==pipeObj){
            Debug.Log($"FocusPipePointObj[{FocusCount}] pipeObj:{pipeObj} lastPipeObj==pipeObj");
            return;
        }

        lastPointObj=pipeObj;
        Debug.Log($"FocusPipePointObj[{++FocusCount}] pipeObj:{pipeObj}");

        //UGUIFollowTarget target=this.gameObject.GetComponent<UGUIFollowTarget>();
        AroundAlignCamera.Instance.AlignVeiwToTarget(pipeObj.transform, distance);

        HightlightModuleBase.ClearHighlightOff();
        HightlightModuleBase.HighlightOff(pipeObj.transform.parent.gameObject);
        HightlightModuleBase.HighlightOn(pipeObj,Color.blue);

        //RTESelectObject(pipeObj);
        RTESelectObjectInner(pipeObj);
    }

    private static GameObject editObj;

    private static GameObject lastPointObj;

    private static GameObject lastPipeObj;

    private static int SelectCount=0;

    public static void RTESelectObject(GameObject pipeObj){
        if(pipeObj==null){
            return;
        }

        if(lastPipeObj==pipeObj){
            Debug.Log($"RTESelectObject[{SelectCount}] pipeObj:{pipeObj} lastPipeObj==pipeObj");
            return;
        }

        if(lastPipeObj!=null){
            lastPipeObj.transform.SetParent(PipeSystemBuilder.Instance.transform);
        }

        lastPipeObj=pipeObj;

        if(editObj){
            //GameObject.DestroyImmediate(editObj);
        }
        else{
            editObj=new GameObject();
        }
        //editObj=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        editObj.name="EditObject_"+pipeObj.name;
        //editObj.SetActive(false);

        //editObj.transform.position=pipeObj.transform.position;
        var pos=PipeSystemComponent.GetCenter(pipeObj);
        editObj.transform.position=pos;
        pipeObj.transform.SetParent(editObj.transform);

        Debug.Log($"RTESelectObject[{++SelectCount}] pipeObj:{pipeObj} pos:{pos}");
        //RTESelectObjectInner(pipeObj);
        RTESelectObjectInner(editObj);
    }

    public static void RTESelectObjectInner(GameObject pipeObj){
        if (RTEManager.Instance)
        {
            pipeObj.AddMissingComponent<ExposeToEditor>();
            pipeObj.layer=LayerMask.NameToLayer("DepDevice");
            //RTEManager.Instance.ShowHandles();
            RTEManager.Instance.SelectObjs(new GameObject[] { pipeObj });            
        }
    }

    public static List<PipePoint> ReadCSVFile(string path){
        List<PipePoint> points=new List<PipePoint>(); 
        if(File.Exists(path)==false) return points;
        string[] lines=File.ReadAllLines(path);
        for(int i=0;i<lines.Length;i++){
            string line=lines[i].Trim();
            if(string.IsNullOrEmpty(line))continue;
            if(line.StartsWith("X"))continue;
            if(line==",,,,,"){
                continue;
            }
            string[] parts=line.Split(',');
            if(string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]) || string.IsNullOrEmpty(parts[2])){
                continue;
            }
            PipePoint point=new PipePoint();
            point.Num=i;
            point.X=float.Parse(parts[0]);
            point.Y=float.Parse(parts[1]);
            point.Z=float.Parse(parts[2]);
            if(point.X<4900000){
                double t=point.X;
                point.X=point.Y;
                point.Y=t;
            }
            points.Add(point);
        }
        return points;
    }

    public static List<List<PipePoint>> ReadMultiPipesCSVFile(string path){
        List<List<PipePoint>> multiPipes=new List<List<PipePoint>>();
        List<PipePoint> points=new List<PipePoint>(); 
        if(File.Exists(path)==false) return multiPipes;
        string[] lines=File.ReadAllLines(path);
        Debug.Log($"ReadMultiPipesCSVFile lines:{lines.Length}");

        for(int i=0;i<lines.Length;i++){
            string line=lines[i].Trim();
            if(string.IsNullOrEmpty(line))continue;
            if(line.StartsWith("X"))continue;
            if(line==",,,,,"){
                if(points.Count>0)multiPipes.Add(points);
                Debug.Log($"ReadMultiPipesCSVFile NewList1 points:{points.Count} count:{multiPipes.Count} i:{i} line:{line} ");
                points=new List<PipePoint>();
                continue;
            }
            string[] parts=line.Split(',');
            
            PipePoint point=new PipePoint();
            point.Num=i;
            point.X=ToValue(parts[0]);
            point.Y=ToValue(parts[1]);
            point.Z=ToValue(parts[2]);
            if(point.X == 0 && point.Y == 0)
            {
                if(points.Count>0)multiPipes.Add(points);
                Debug.Log($"ReadMultiPipesCSVFile NewList2 points:{points.Count} count:{multiPipes.Count} i:{i} line:{line} ");
                points=new List<PipePoint>();
                continue;
            }
            if(point.Z==0){
                if(points.Count>0){
                    double z=points.Last().Z;
                    if(z!=0){
                        point.Z=z;
                    }
                    else{
                        point.Z=PipeSystemPointConverter.Instance.CADOffset.z;
                    }
                }
                else{
                    point.Z=PipeSystemPointConverter.Instance.CADOffset.z;
                }
            }
            if(point.X<4900000){
                double t=point.X;
                point.X=point.Y;
                point.Y=t;
            }
            points.Add(point);

            Debug.Log($"ReadMultiPipesCSVFile[{i}]:{point} | {line} ");
        }
        multiPipes.Add(points);points=new List<PipePoint>();
        return multiPipes;
    }

    public static double ToValue(string txt){
        if(string.IsNullOrEmpty(txt)){
            return 0;
        }
        //float v= float.Parse(txt);
        double v2=double.Parse(txt);//437686.353 > 437686.3 > 437686.353
        //Debug.Log($"ToFloat {txt} > {v} > {v2}");
        return v2;
    }
}
