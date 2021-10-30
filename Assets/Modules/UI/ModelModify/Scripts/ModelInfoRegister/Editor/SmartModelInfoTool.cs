using Base.Common;
using NavisPlugins.Infos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Threading;

public class SmartModelInfoTool : MonoBehaviour
{
    [Tooltip("遍历xml，根据位置信息找模型信息")]
    public List<GameObject> modelParent;

    private string ConfigPath = "\\..\\NavisFileInfo.XML";
    private NavisFileInfo treeInfo;

    private List<ModelItemInfo> modelInfoList = new List<ModelItemInfo>();

    //记录UID，有些模型的名称直接就是UID，加快匹配速度
    private Dictionary<string,ModelItemInfo> modelInfoDict = new Dictionary<string,ModelItemInfo>();

    //public int infoCount;

    public int noIdCount;

    public int guIdCount;

    public float posOffset = 0.005f;
    public int newModelCount1=0;

    public int newModelCount2=0;

    public int oldModelCount=0;

    public int errorModelCount=0;

    public int repeatCount=0;

    public bool UpdataAll=false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [ContextMenu("AddModelGuidEx")]
    public void AddModelIdEx()
    {
        // repeatCount=0;
        // errorModelCount=0;
        // DateTime rec = DateTime.Now;
        // string path = Application.dataPath + ConfigPath;
        // if (File.Exists(path))
        // {
        //     EditorUtility.DisplayProgressBar("AddModelId", "加载NavisFileInfo.XML", 0);
        //     treeInfo = SerializeHelper.DeserializeFromFile<NavisFileInfo>(path);
        //     Debug.LogErrorFormat("加载NavisFileInfo.XML,耗时:{0}秒", (DateTime.Now - rec).TotalSeconds);
        //     rec = DateTime.Now;
        //     //EditorUtility.ClearProgressBar();
            
        //     infoCount = 0;
        //     noIdCount = 0;
        //     modelInfoList.Clear();
        //     modelInfoDict.Clear();

        //     if (treeInfo != null && treeInfo.Models != null)
        //     {
        //         EditorUtility.DisplayProgressBar("AddModelId", "收集模型信息", 0.1f);
        //         StoreModelItemInfo(treeInfo.Models);
        //         guIdCount=modelInfoDict.Count;
        //          Debug.LogErrorFormat("modelInfoDict数量：{1}",modelInfoDict.Count);
        //         EditorUtility.DisplayProgressBar("AddModelId", "给模型添加GUID脚本", 1);
        //         AddModelScripts(true);//添加脚本
        //     }
        //     Debug.LogErrorFormat("已添加脚本模型数量：{0} 新增脚本数量：{1}",oldModelCount,(newModelCount1+newModelCount2));
        //     Debug.LogErrorFormat("给模型添加GUID脚本,耗时:{0}秒", (DateTime.Now - rec).TotalSeconds);
        //     EditorUtility.ClearProgressBar();
        // }

        InnerAddModelId(true);//添加脚本
    }

    private void InnerAddModelId(bool isAddScript){
         repeatCount=0;
        errorModelCount=0;
        DateTime rec = DateTime.Now;
        string path = Application.dataPath + ConfigPath;
        if (File.Exists(path))
        {
            EditorUtility.DisplayProgressBar("AddModelId", "加载NavisFileInfo.XML", 0);
            treeInfo = SerializeHelper.DeserializeFromFile<NavisFileInfo>(path);
            Debug.LogErrorFormat("加载NavisFileInfo.XML,耗时:{0}秒", (DateTime.Now - rec).TotalSeconds);
            rec = DateTime.Now;
            //EditorUtility.ClearProgressBar();
            
            infoCount = 0;
            noIdCount=0;
            modelInfoList.Clear();
            modelInfoDict.Clear();
            InfoModels.Clear();
            InfoModelsFiltered.Clear();

            if (treeInfo != null && treeInfo.Models != null)
            {
                EditorUtility.DisplayProgressBar("AddModelId", "收集模型信息", 0.1f);
                StoreModelItemInfo(null,treeInfo.Models,0,null);
                guIdCount=modelInfoDict.Count;
                 Debug.LogErrorFormat("modelInfoDict数量：{0}",modelInfoDict.Count);
                EditorUtility.DisplayProgressBar("AddModelId", "给模型添加GUID脚本", 1);
                AddModelScripts(isAddScript);//不添加脚本，先看看信息
            }
            Debug.LogErrorFormat("已添加脚本模型数量：{0} 新增脚本数量：{1}",oldModelCount,(newModelCount1+newModelCount2));
            Debug.LogErrorFormat("给模型添加GUID脚本,耗时:{0}秒", (DateTime.Now - rec).TotalSeconds);
            EditorUtility.ClearProgressBar();
        }
    }


    [ContextMenu("AddModelGuid")]
    public void AddModelId()
    {
        // repeatCount=0;
        // errorModelCount=0;
        // DateTime rec = DateTime.Now;
        // string path = Application.dataPath + ConfigPath;
        // if (File.Exists(path))
        // {
        //     EditorUtility.DisplayProgressBar("AddModelId", "加载NavisFileInfo.XML", 0);
        //     treeInfo = SerializeHelper.DeserializeFromFile<NavisFileInfo>(path);
        //     Debug.LogErrorFormat("加载NavisFileInfo.XML,耗时:{0}秒", (DateTime.Now - rec).TotalSeconds);
        //     rec = DateTime.Now;
        //     //EditorUtility.ClearProgressBar();
            
        //     infoCount = 0;
        //     noIdCount=0;
        //     modelInfoList.Clear();
        //     modelInfoDict.Clear();

        //     if (treeInfo != null && treeInfo.Models != null)
        //     {
        //         EditorUtility.DisplayProgressBar("AddModelId", "收集模型信息", 0.1f);
        //         StoreModelItemInfo(treeInfo.Models);
        //         guIdCount=modelInfoDict.Count;
        //          Debug.LogErrorFormat("modelInfoDict数量：{0}",modelInfoDict.Count);
        //         EditorUtility.DisplayProgressBar("AddModelId", "给模型添加GUID脚本", 1);
        //         AddModelScripts(false);//不添加脚本，先看看信息
        //     }
        //     Debug.LogErrorFormat("已添加脚本模型数量：{0} 新增脚本数量：{1}",oldModelCount,(newModelCount1+newModelCount2));
        //     Debug.LogErrorFormat("给模型添加GUID脚本,耗时:{0}秒", (DateTime.Now - rec).TotalSeconds);
        //     EditorUtility.ClearProgressBar();
        // }
        InnerAddModelId(false);//不添加脚本，先看看信息
    }

    public List<string> InfoModels=new List<string>();

    public List<string> InfoModelsFiltered=new List<string>();

    public int FilterMode=0;//0:不过滤 1:按照FilterModels过滤 2:按照ModelParent过滤
    public List<string> FilterModels=new List<string>();

    public List<GameObject> InfoModelsRoot=new List<GameObject>();

    public List<string> NoUseInfoModels=new List<string>();

    private bool IsFiltered(string modelName){
         bool result=true;
         if(FilterMode==1){
            if(FilterModels.Count>0){
                for(int j=0;j<FilterModels.Count;j++){
                    //Debug.Log("StoreModelItemInfo filter1:"+modelName+"|"+FilterModels[j]+"|"+modelName.Contains(FilterModels[j]));
                    if(modelName.Contains(FilterModels[j])){
                        result=false;//过滤掉
                    }
                }
            }
        }

        if(FilterMode==2){
            if(modelParent.Count>0){
                for(int j=0;j<modelParent.Count;j++){
                    if(!modelName.Contains(modelParent[j].name)){
                        result=false;//过滤掉
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 收集模型信息
    /// </summary>
    /// <param name="infoList"></param>
    private void StoreModelItemInfo(ModelItemInfo parent,List<ModelItemInfo> infoList,int layer, ModelItemInfo root)
    {
        if(infoList != null)
        {
            List<ModelItemInfo> infoList2=new List<ModelItemInfo>();
            foreach(var info in infoList)
            {
                if(layer==0){
                    info.SetParent(parent, info);


                    InfoModels.Add(info.Name);//Debug.Log("StoreModelItemInfo info.Name:"+info.Name);
                   if(IsFiltered(info.Name)){
                       continue;
                   }
                   InfoModelsFiltered.Add(info.Name);
                }
                else
                {
                    info.SetParent(parent, root);
                }
                
                if(NoUseInfoModels.Contains(info.Name)){
                    continue;
                }
                modelInfoList.Add(info);
                infoList2.Add(info);
            }
            //modelInfoList.AddRange(infoList);
            infoCount += infoList2.Count;
            foreach(var item in infoList2)
            {
                if(!string.IsNullOrEmpty(item.UId))
                {
                    if(modelInfoDict.ContainsKey(item.UId)){
                        Debug.LogError("StoreModelItemInfo UID 重复:"+item.UId);
                    }
                    else{
                        modelInfoDict.Add(item.UId,item);//记录UID，有些模型的名称直接就是UID，加快匹配速度
                    }
                }
                else{
                    noIdCount++;

                }
                StoreModelItemInfo(item,item.Children,layer+1, root);//递归
            }
        }
        else{
            Debug.LogError("StoreModelItemInfo infoList==null");
        }
    }

        [ContextMenu("AddModelScriptsEx")]
    private void AddModelScriptsEx()
    {
        AddModelScripts(true);
    }

    /// <summary>
    /// 添加模型脚本(按父子结构比对)
    /// </summary>
    [ContextMenu("AddModelScripts")]
    private void AddModelScripts(bool isAddScript)
    {
        DateTime start=DateTime.Now;
        newModelCount1 = 0;
        newModelCount2 = 0;
        oldModelCount = 0;
        errorGos1.Clear();
        foundGos2.Clear();
        if (modelParent==null||modelParent.Count==0)
        {
            Debug.LogError("请添加模型父物体至ModelParent中");
            return;
        }
        if(modelInfoList!=null)
        {
            //infoCount=modelInfoList.Count;
            for (int i = 0; i < modelParent.Count; i++)
            {
                GameObject item = modelParent[i];
                if (item == null) continue;
                var children=item.transform.GetChildrenTransform();
                allModelCount=children.Count;
                currentModelCount=0;
                if(isAddScript)
                {
                    AddChildModelInfo(item.transform);//核心

                    //AddChildModelInfoEx(modelInfoList, children);

                    // for(int j=0;j<children.Count;j++)    
                    // {
                    //     Transform child=children[j].transform;
                    //     AddChidModelInfo_Item(modelInfoList,child);
                    // }
                }
            }           
        }
        TimeSpan time=DateTime.Now-start;
        Debug.Log("AddModelScripts 完成 用时:"+time);
    }

    private void SetBIMModelInfo(Transform child,ModelItemInfo modelInfo)
    {
        newModelCount2++;//根据距离找到的
        if(modelInfo==null){
            Debug.LogError("SetBIMModelInfo modelInfo==null:"+child);
        }
        modelInfo.CurrentModel=child.gameObject;
        BIMModelInfo infoT = child.gameObject.AddMissingComponent<BIMModelInfo>();
        infoT.Guid = modelInfo.Id;
        infoT.MName = modelInfo.Name;
        infoT.Position1=child.position;
        infoT.Position2=new Vector3(modelInfo.X,modelInfo.Z,modelInfo.Y);
        infoT.Distance=Vector3.Distance(infoT.Position1,infoT.Position2);
        if(infoT.Distance>posOffset*4){
            Debug.LogError(string.Format("SetBIMModelInfo 距离过大 选择最新:Id:{0}| Name:{1}| Obj:{2}| Distance:{3}",
                                        modelInfo.Id,modelInfo.Name,child.name,infoT.Distance));
        }
    }

    // private void AddChidModelInfo_Item(List<ModelItemInfo> modelInfos,Transform child){
        
    //             // if(child.GetComponent<Renderer>()==null)
    //             // {
    //             //     if (child.GetComponent<BIMModelInfo>() != null)
    //             //     {
    //             //         BIMModelInfo info=child.GetComponent<BIMModelInfo>();
    //             //         GameObject.DestroyImmediate(info);
    //             //     }
    //             //     Debug.LogWarning("没有 Renderer:"+child);
    //             //     AddChildModelInfo(modelInfoList, child.gameObject);//递归
    //             //     continue;
    //             // }
    //             if(child.position==Vector3.zero)
    //             {
    //                 //AddChildModelInfo(modelInfoList, child.gameObject);//递归
    //                 return;
    //             }
    //             if(child.gameObject.activeSelf==false) return;
    //             if (child.GetComponent<BIMModelInfo>() != null && UpdataAll==false)
    //             {
    //                 oldModelCount++;
    //             }
    //             else
    //             {
    //                 if(modelInfoDict.ContainsKey(child.name)){
    //                     //Debug.Log("modelInfoDict.ContainsKey(child.name):"+child.name);
    //                     ModelItemInfo info=modelInfoDict[child.name];
    //                     SetBIMModelInfo(child,info);
    //                     newModelCount1++;
    //                 }
    //                 else{
    //                     var infos = modelInfoList.FindAll(i => IsSamePosition(i, child,posOffset));
    //                     if(infos.Count==0){
    //                         Debug.LogError("未找到模型信息:"+child);
    //                         errorModelCount++;
    //                     }
    //                     else if(infos.Count==1)
    //                     {
    //                         var modelInfo=infos[0];
    //                         newModelCount2++;
    //                         SetBIMModelInfo(child,modelInfo);
                            
    //                         // if(modelInfo.CurrentModel==child.gameObject){
    //                         //      Debug.LogError(string.Format("重复计算模型:Id:{0}| Name:{1}| Obj:{2}",
    //                         //         modelInfo.Id,modelInfo.Name,child.name));
    //                         // }
    //                         // else{
    //                         //     infoT.RepeatObj=modelInfo.CurrentModel;
    //                         //     modelInfo.CurrentModel=child.gameObject;
    //                         //     modelInfo.Models.Add(child.name);
    //                         //     if(modelInfo.Models.Count>1){
    //                         //         string txt="";
    //                         //         for(int i=0;i<modelInfo.Models.Count;i++)
    //                         //         {
    //                         //             txt+=modelInfo.Models[i]+";";
    //                         //         }
    //                         //         Debug.LogError(string.Format("模型位置信息重叠:Id:{0}| Name:{1}| Obj:{2}| Models:{3}",
    //                         //         modelInfo.Id,modelInfo.Name,child.name,txt));
    //                         //         repeatCount++;
    //                         //     }
    //                         // }
    //                     }
    //                     else //if(infos.Count>1)
    //                     {
    //                         float minDis=10000;
    //                         List<ModelItemInfo> minModels=new List<ModelItemInfo>();
    //                         string txt="";
    //                         for(int i=0;i<infos.Count;i++)
    //                         {
    //                             var modelInfo=infos[i];
    //                             var dis=Vector3.Distance(child.position,new Vector3(modelInfo.X,modelInfo.Z,modelInfo.Y));
    //                             if(dis==minDis)
    //                             {
    //                                 minModels.Add(modelInfo);
    //                             }
    //                             if(dis<minDis){
    //                                 minDis=dis;
    //                                 minModels=new List<ModelItemInfo>();
    //                                 minModels.Add(modelInfo);
    //                             }
    //                             txt+=modelInfo.Id+"_"+modelInfo.Name+"_"+dis+";";
    //                         }

    //                         if(minModels.Count==1){
    //                             ModelItemInfo minModel=minModels[0];

    //                             if(!child.name.Contains(minModel.Name))
    //                             {
    //                                 Debug.LogError(string.Format("模型位置信息重叠1 选择最新:Id:{0}| Name:{1}| Obj:{2}| Models:{3}",
    //                                     minModel.Id,minModel.Name,child.name,txt));
    //                                 repeatCount++;

    //                                 SetBIMModelInfo(child,minModel);//还是要设置的,真的出现了，需要进一步判断
    //                             }
    //                             else{
    //                                 SetBIMModelInfo(child,minModel);
    //                             }
    //                         }
    //                         else{
    //                             ModelItemInfo minModel=null;
    //                             for(int i=0;i<minModels.Count;i++){
    //                                 ModelItemInfo model=minModels[i];
    //                                 if(child.name.Contains(model.Name))
    //                                 {
    //                                     minModel=model;
    //                                     break;
    //                                 }
    //                             }
    //                             if(minModel!=null){
    //                                 SetBIMModelInfo(child,minModel);
    //                             }
    //                             else{

    //                                 minModel=minModels[0];
    //                                 SetBIMModelInfo(child,minModel);//还是要设置的,真的出现了，需要进一步判断

    //                                 Debug.LogError(string.Format("模型位置信息重叠2 选择最新:Id:{0}| Name:{1}| Obj:{2}| Models:{3}",
    //                                     minModel.Id,minModel.Name,child.name,txt));
    //                                 repeatCount++;
    //                             }
    //                         }
                            
    //                         //if(minModel.Name!=child.name)
                            
                            
    //                     }
    //                 }
    //             }
    // }

    // /// <summary>
    // /// 添加子物体模型信息(有进度条)
    // /// </summary>
    // /// <param name="modelInfo"></param>
    // /// <param name="parentObj"></param>
    // private void AddChildModelInfoEx(List<ModelItemInfo> modelInfos,List<Transform> objs)
    // {      
    //     if(modelInfos != null)
    //     {        
    //         for(int j=0;j<objs.Count;j++)    
    //         //foreach (Transform child in parentObj.transform)
    //         {
    //             Transform child=objs[j].transform;
    //             AddChidModelInfo_Item(modelInfos,child);
    //         }
    //     }
    // }

    public float allModelCount=0;
    private float currentModelCount=0;

    public int infoCount=0;

    //private List<ModelItemInfo> modelInfos=new List<ModelItemInfo>();

    public List<string> FilterModelObjs=new List<string>();

    public List<Transform> errorGos1=new List<Transform>();

    public List<Transform> foundGos2=new List<Transform>();

    private bool IsModelObjFiltered(Transform t){
        bool result=false;
        foreach(var str in FilterModelObjs){
            if(t.name.Contains(str)){
                result=true;
            }
        }
        return result;
    }

    /// <summary>
    /// 添加子物体模型信息
    /// </summary>
    /// <param name="modelInfo"></param>
    /// <param name="parentObj"></param>
    private void AddChildModelInfo(Transform parentT)
    {      
        if(modelInfoList != null)
        {        
            for(int j=0;j<parentT.childCount;j++)    
            //foreach (Transform child in parentObj.transform)
            {
                currentModelCount++;
                
                EditorUtility.DisplayProgressBar("AddModelId"
                    , string.Format("AddChildModelInfo {0}/{1} (*{2})",currentModelCount,allModelCount,infoCount)
                    , (float)(currentModelCount/(allModelCount)));
                Transform child=parentT.GetChild(j);
                // if(child.GetComponent<Renderer>()==null)
                // {
                //     if (child.GetComponent<BIMModelInfo>() != null)
                //     {
                //         BIMModelInfo info=child.GetComponent<BIMModelInfo>();
                //         GameObject.DestroyImmediate(info);
                //     }
                //     Debug.LogWarning("没有 Renderer:"+child);
                //     AddChildModelInfo(modelInfoList, child.gameObject);//递归
                //     continue;
                // }
                if(child.position==Vector3.zero )
                {
                    AddChildModelInfo(child);//递归
                    continue;
                }
                if(FilterModelObjs.Count>0){
                    if(IsModelObjFiltered(child)){ //“_LOD",
                        AddChildModelInfo(child);//递归
                        continue;
                    }
                }
                if(child.gameObject.activeSelf==false) continue;
                if (child.GetComponent<BIMModelInfo>() != null && UpdataAll==false)
                {
                    oldModelCount++;
                }
                else
                {
                    if(modelInfoDict.ContainsKey(child.name)){
                        //Debug.Log("modelInfoDict.ContainsKey(child.name):"+child.name);
                        ModelItemInfo info=modelInfoDict[child.name];
                        SetBIMModelInfo(child,info);
                        newModelCount1++;//直接通过名称找到的
                    }
                    else{
                        float offset=posOffset;//0.005，最精确的
                        var infos = modelInfoList.FindAll(i => IsSamePosition(i, child , offset));//核心
                        if(infos.Count==0)
                        {
                            //Debug.LogWarning(string.Format("未找到模型信息[1] 距离:{0},模型:{1}",offset,child));
                            // errorModelCount++;
                            // errorGos.Add(child);

                            //0.01-0.02-0.04-0.08
                            for(int k=0;k<2;k++){
                                offset*=2;//*2;

                                infos = modelInfoList.FindAll(i => IsSamePosition(i, child , offset));//核心
                                if(infos.Count>0){
                                    break;
                                }
                            }
                            //0.02

                            if(infos.Count==0){
                                float md=0;
                                var info=GetClosedItem(child,out md);

                                if(child.name.Contains("设备类型"))//集控楼里面的某些设备被我拆分并改名了，在原vue文件中是一个模型
                                {
                                    var parent=info.GetParent();
                                    if(parent!=null){
                                        foundGos2.Add(child);
                                        SetBIMModelInfo(child,parent);
                                        return;
                                    }
                                }

                                if(md<1){
                                    if(md<0.5){
                                        if(info.CurrentModel==null){
                                            SetBIMModelInfo(child,info);
                                            Debug.Log(string.Format("-->找到距离最近模型1 距离:{0},模型:{1},信息:{2}",md,child,info.Name));
                                            foundGos2.Add(child);
                                        }
                                        else{
                                             Debug.LogError(string.Format("-->-->该信息已经绑定到模型上1 距离:{0},模型:{1},信息:{2},绑定模型:{3}",md,child,info.Name,info.CurrentModel));
                                            errorModelCount++;
                                            errorGos1.Add(child);
                                            
                                        }
                                    }
                                    else
                                    {
                                        if(info.CurrentModel==null){
                                            SetBIMModelInfo(child,info);
                                            Debug.Log(string.Format("-->找到距离最近模型2 距离:{0},模型:{1},信息:{2}",md,child,info.Name));
                                            foundGos2.Add(child);
                                        }
                                        else{
                                            Debug.LogError(string.Format("-->-->该信息已经绑定到模型上2 距离:{0},模型:{1},信息:{2},绑定模型:{3}",md,child,info.Name,info.CurrentModel));
                                            errorModelCount++;
                                            errorGos1.Add(child);
                                        }
                                    }
                                }
                                else{
                                    BIMModelInfo infoT = child.gameObject.GetComponent<BIMModelInfo>();
                                    if(infoT!=null){
                                        GameObject.DestroyImmediate(infoT);
                                    }
                                    Debug.LogError(string.Format("未找到模型信息[2] 距离:{0},模型:{1},信息:{2}",md,child,info.Name));
                                    errorModelCount++;
                                    errorGos1.Add(child);
                                }
                                
                            }
                            else{
                                //Debug.Log(string.Format("  找到模型信息[2] 距离:{0},模型:{1}",offset,child));
                                SelectedCorrectModelInfo(infos,child);//距离远一些的
                            }
                        }
                        else 
                        {
                            SelectedCorrectModelInfo(infos,child);//距离最近的
                        }
                    }
                    AddChildModelInfo(child);//递归
                }
            }
        }
    }

    private void SelectedCorrectModelInfo(List<ModelItemInfo> infos,Transform child){
        if(infos.Count==1)
        {
            var modelInfo=infos[0];
            //newModelCount2++;
            SetBIMModelInfo(child,modelInfo);
            
            // if(modelInfo.CurrentModel==child.gameObject){
            //      Debug.LogError(string.Format("重复计算模型:Id:{0}| Name:{1}| Obj:{2}",
            //         modelInfo.Id,modelInfo.Name,child.name));
            // }
            // else{
            //     infoT.RepeatObj=modelInfo.CurrentModel;
            //     modelInfo.CurrentModel=child.gameObject;
            //     modelInfo.Models.Add(child.name);
            //     if(modelInfo.Models.Count>1){
            //         string txt="";
            //         for(int i=0;i<modelInfo.Models.Count;i++)
            //         {
            //             txt+=modelInfo.Models[i]+";";
            //         }
            //         Debug.LogError(string.Format("模型位置信息重叠:Id:{0}| Name:{1}| Obj:{2}| Models:{3}",
            //         modelInfo.Id,modelInfo.Name,child.name,txt));
            //         repeatCount++;
            //     }
            // }
        }
        else //if(infos.Count>1)
        {
            float minDis=10000;
            List<ModelItemInfo> minModels=new List<ModelItemInfo>();
            string txt="";
            for(int i=0;i<infos.Count;i++)
            {
                var modelInfo=infos[i];
                var dis=Vector3.Distance(child.position,new Vector3(modelInfo.X,modelInfo.Z,modelInfo.Y));
                if(dis==minDis)
                {
                    minModels.Add(modelInfo);
                }
                if(dis<minDis){
                    minDis=dis;
                    minModels=new List<ModelItemInfo>();
                    minModels.Add(modelInfo);
                }
                txt+=modelInfo.Id+"_"+modelInfo.Name+"_"+dis+";";
            }

            if(minModels.Count==1){
                ModelItemInfo minModel=minModels[0];

                if(!child.name.Contains(minModel.Name))
                {
                    Debug.LogError(string.Format("模型位置信息重叠1 选择最新:Id:{0}| Name:{1}| Obj:{2}| Models:{3}",
                        minModel.Id,minModel.Name,child.name,txt));
                    repeatCount++;

                    SetBIMModelInfo(child,minModel);//还是要设置的,真的出现了，需要进一步判断
                }
                else{
                    SetBIMModelInfo(child,minModel);
                }
            }
            else{
                ModelItemInfo minModel=null;
                for(int i=0;i<minModels.Count;i++){
                    ModelItemInfo model=minModels[i];
                    if(child.name.Contains(model.Name))
                    {
                        minModel=model;
                        break;
                    }
                }
                if(minModel!=null){
                    SetBIMModelInfo(child,minModel);
                }
                else{

                    minModel=minModels[0];
                    SetBIMModelInfo(child,minModel);//还是要设置的,真的出现了，需要进一步判断

                    Debug.LogError(string.Format("模型位置信息重叠2 选择最新:Id:{0}| Name:{1}| Obj:{2}| Models:{3}",
                        minModel.Id,minModel.Name,child.name,txt));
                    repeatCount++;
                }
            }
            
            //if(minModel.Name!=child.name)
            
            
            }
    }

    /// <summary>
    /// 位置是否相同
    /// </summary>
    /// <param name="infoT"></param>
    /// <param name="parentT"></param>
    /// <returns></returns>
    private bool IsSamePosition(ModelItemInfo infoT,Transform t,float offset)
    {
        Vector3 pos = t.position;
        bool isXInRange = Mathf.Abs(infoT.X - pos.x) < offset;
        if(isXInRange==false)return false;
        bool isYInRange= Mathf.Abs(infoT.Z - pos.y) < offset;
         if(isYInRange==false)return false;
        bool isZInRange = Mathf.Abs(infoT.Y - pos.z) < offset;
         if(isZInRange==false)return false;
        //if (parentT.name == "0028-140039-302404565767943477" && infoT.DisplayName == "0028-140039-302404565767943477")
        //{
        //    Vector3 v = new Vector3(infoT.X,infoT.Z,infoT.Y);
        //    Debug.LogErrorFormat("Pos:{3} infoPos:{4} X:{0} Y:{1} Z:{2}", Mathf.Abs(infoT.X - pos.x), Mathf.Abs(infoT.Z - pos.y), Mathf.Abs(infoT.Y - pos.z)
        //        ,pos,v);
        //}
        if (isXInRange&&isYInRange&&isZInRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private ModelItemInfo GetClosedItem(Transform t,out float md){
        float minDis=float.MaxValue;
        ModelItemInfo minItem=null;
        foreach(var info in modelInfoList)
        {
            Vector3 pos = t.position;
            Vector3 pos2=new Vector3(info.X,info.Z,info.Y);
            float dis=Vector3.Distance(pos,pos2);
            if(dis<minDis){
                minItem=info;
                minDis=dis;
            }
        }
        md=minDis;
        return minItem;
    }


}
