using NavisPlugins.Infos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Base.Common;
using System.Text;
using static BIMModelInfo;

[Serializable]
public class Model2TransformResult
{
    [NonSerialized]
    List<ModelItemInfo> models1;
    [NonSerialized]
    ModelItemInfoDictionary modelDict;
    [NonSerialized]
    TransformDictionary TransformDict;
    [NonSerialized]
    float MinDistance;

    [NonSerialized]
    float MaxDistance;

    [NonSerialized]
    float MoreMaxDistance;


    public List<ModelItemInfo> allModels_uid_all = new List<ModelItemInfo>();
    //[NonSerialized]
    public List<ModelItemInfo> allModels_uid_found1 = new List<ModelItemInfo>();
    //[NonSerialized]
    public List<ModelItemInfo> allModels_uid_found2 = new List<ModelItemInfo>();
    //[NonSerialized]
    public List<ModelItemInfo> allModels_uid_nofound1 = new List<ModelItemInfo>();
    //[NonSerialized]
    public List<ModelItemInfo> allModels_uid_nofound2 = new List<ModelItemInfo>();

    public List<ModelItemInfo> allModels_uid_exception = new List<ModelItemInfo>();
    public int notFoundCount;

    [NonSerialized]
    private List<ModelItemInfo> allModels_uid_AllNotFound = new List<ModelItemInfo>();
    //[NonSerialized]

    //[NonSerialized]
    public string DebugErrorLog = "";

    //public bool IsShowLog { get; internal set; }

    public Model2TransformResult()
    {

    }

    public Model2TransformResult(List<ModelItemInfo> models1, ModelItemInfoDictionary modelDict, TransformDictionary TransformDict, float MinDistance)
    {
        this.models1 = models1;
        this.modelDict = modelDict;
        this.TransformDict = TransformDict;
        this.MinDistance = MinDistance;
        this.MaxDistance = MinDistance * 10;
        this.MoreMaxDistance = this.MaxDistance * 20;
    }

    public CheckResultArg CheckArg = new CheckResultArg();

    public bool isCenterPos = true;

    public void CheckResult(ModelItemInfo model1, List<Transform> transforms1)
    {
        allModels_uid_all.Add(model1);

        if (model1.Name == InitNavisFileInfoByModelSetting.Instance.DebugFilterModelName)
        {
            Debug.Log(InitNavisFileInfoByModelSetting.Instance.DebugFilterModelName);
        }

        if (transforms1 == null || transforms1.Count == 0)//1.?????????
        {

            //allModels_uid_nofound1.Add(model1);

            var ms = TransformDict.GetTransformsByName(model1.Name);
            if (ms.Count == 0)
            {
                //bool isCenterPos = true;
                var closedT = model1.FindClosedTransform(TransformDict.ToList(), isCenterPos);
                if (closedT == null)
                {
                    DebugLogError($"???{ms.Count}???[Rute1_1_5][closedT == null][{MinDistance}-{MaxDistance}][{closedT}][Name:{model1.Name}][Path:{model1.GetPath()}][{model1.ShowDistance(closedT)})]");
                    return;
                }
                float dis = model1.GetDistance(closedT, isCenterPos);
                //if (model1.GetParent().Name == "???" )
                //{
                //    if(closedT.name.ToLower().Contains("door"))
                //    {
                //        if (dis < 0.3)
                //        {
                //            //AddFounded1(model1, closedT, BIMFoundType.ByClosed);
                //        }
                //        else
                //        {
                //            DebugLogError($"???{ms.Count}???[Rute1_1_4][???????????????????????????][{MinDistance}-{MaxDistance}][{dis},{closedT}][Name:{model1.Name}][Path:{model1.GetPath()}][{model1.ShowDistance(closedT)})]");
                //        }
                //    }
                //    else
                //    {
                //        DebugLogError($"???{ms.Count}???[Rute1_1_5][??????????????????][{MinDistance}-{MaxDistance}][{dis},{closedT}][Name:{model1.Name}][Path:{model1.GetPath()}][{model1.ShowDistance(closedT)})]");
                //    }
                   
                //}

                if (InitNavisFileInfoByModelSetting.Instance.CheckExceptialCases(model1, closedT, dis))
                {
                    AddFounded1(model1, closedT, BIMFoundType.ByClosed);
                    return;
                }

                //??????????????????

                if (CheckArg.IsFindClosed1)
                {
                    
                    if (dis < MinDistance)
                    {
                        AddFounded1(model1, closedT, BIMFoundType.ByClosed);
                    }
                    else if (dis < MaxDistance)
                    {
                        AddFounded1(model1, closedT, BIMFoundType.ByClosed);
                    }
                    else
                    {
                        DebugLogError($"???{ms.Count}???[Rute1_1_3][??????????????????Transform&&Closed????????????][{MinDistance}-{MaxDistance}|{dis}][{dis},{closedT}][Name:{model1.Name}][Path:{model1.GetPath()}][{model1.ShowDistance(closedT)})]");
                        allModels_uid_nofound1.Add(model1);
                    }
                    
                }
                else
                {
                    if(model1.GetParentName()=="???")
                    {
                        //???????????????
                        allModels_uid_exception.Add(model1);//??????????????????????????????
                    }
                    else
                    {
                        DebugLogError($"???{ms.Count}???[Rute1_1_1][??????????????????Transform][{MinDistance}-{MaxDistance}][{dis},{closedT}][Name:{model1.Name}][Path:{model1.GetPath()}][{model1.ShowDistance(closedT)})]");
                        allModels_uid_nofound1.Add(model1);
                    }
                    
                }
            }
            else if (ms.Count == 1)
            {
                var transf = ms[0];
                float dis = model1.GetDistance(transf, isCenterPos);
                if (dis <= MinDistance)
                {
                    AddFounded1(model1, transf, BIMFoundType.ByName);
                }
                else
                {
                    if (CheckArg.IsOnlyName)//??????????????????????????????
                    {
                        AddFounded1(model1, transf, BIMFoundType.ByName);
                    }
                    else if (CheckArg.IsByNameAfterNotFindModel)//????????????111
                    {
                        if(dis<MaxDistance)
                        {
                            AddFounded1(model1, transf, BIMFoundType.ByName);
                        }
                        else if(InitNavisFileInfoByModelSetting.Instance.IsStructrue(transf.name) && dis< MoreMaxDistance)
                        {
                            AddFounded1(model1, transf, BIMFoundType.ByName);
                        }
                        else
                        {
                            if(model1.Name== "????????????B")
                            {

                            }
                            if (CheckArg.IsMoreDistance && dis < MoreMaxDistance)
                            {
                                AddFounded1(model1, transf, BIMFoundType.ByName);
                            }
                            else
                            {
                                DebugLogError($"???{ms.Count}???[Rute1_1_22][?????????Transform][{MinDistance}-{MaxDistance}-{MoreMaxDistance}]({CheckArg.IsMoreDistance})][m:{model1.ShowDistance(transf)}][path:{model1.GetPath()}]");
                                allModels_uid_nofound1.Add(model1);
                            }
                        }
                        
                    }
                    else 
                    { 
                        DebugLogError($"???{ms.Count}???[Rute1_1_21][?????????Transform][{MinDistance}-{MaxDistance}-{MoreMaxDistance}][m:{model1.ShowDistance(transf)}][path:{model1.GetPath()}]");
                        allModels_uid_nofound1.Add(model1);
                    }
                }
            }
            else
            {
                var transf = model1.FindClosedTransform(ms, isCenterPos);
                float dis = model1.GetDistance(transf, isCenterPos);
                if (dis <= MinDistance)
                {
                    AddFounded1(model1, transf,BIMFoundType.ByNameAndClosed);
                }
                else
                {
                    //DebugLogError($"???{ms.Count}???[Rute1_1_3][?????????Transform][{MinDistance}][m:{model1.ShowDistance(transf)}][path:{model1.GetPath()}]");
                    //for (int i1 = 0; i1 < ms.Count; i1++)
                    //{
                    //    var m = ms[i1];
                    //    DebugLogError($"[Rute1_1_3][{i1 + 1}/{ms.Count}] m:{model1.ShowDistance(m)}");
                    //    //??????????????????
                    //}

                    if (CheckArg.IsOnlyName)//??????????????????????????????
                    {
                        AddFounded1(model1, transf, BIMFoundType.ByName);
                    }
                    else if (CheckArg.IsByNameAfterNotFindModel)//????????????111
                    {
                        if (dis < MaxDistance)
                        {
                            AddFounded1(model1, transf, BIMFoundType.ByName);
                        }
                        else if (InitNavisFileInfoByModelSetting.Instance.IsStructrue(transf.name) && dis < MoreMaxDistance)
                        {
                            AddFounded1(model1, transf, BIMFoundType.ByName);
                        }
                        else
                        {
                            if(CheckArg.IsMoreDistance && dis< MoreMaxDistance)
                            {
                                AddFounded1(model1, transf, BIMFoundType.ByName);
                            }
                            else
                            {
                                DebugLogError($"???{ms.Count}???[Rute1_1_32][?????????Transform][{MinDistance}-{MaxDistance}-{MoreMaxDistance}][m:{model1.ShowDistance(transf)}][path:{model1.GetPath()}]");
                                for (int i1 = 0; i1 < ms.Count; i1++)
                                {
                                    var m = ms[i1];
                                    MeshRenderer renderer = m.GetComponent<MeshRenderer>();
                                    bool isRenderer = renderer != null;
                                    DebugLogError($"[Rute1_1_32][{i1 + 1}/{ms.Count}][renderer:{isRenderer}] m:{model1.ShowDistance(m)}");
                                    //??????????????????
                                }
                                allModels_uid_nofound1.Add(model1);
                            }
                        }
                    }
                    else
                    {
                        DebugLogError($"???{ms.Count}???[Rute1_1_31][?????????Transform][{MinDistance}-{MaxDistance}-{MoreMaxDistance}][m:{model1.ShowDistance(transf)}][path:{model1.GetPath()}]");
                        for (int i1 = 0; i1 < ms.Count; i1++)
                        {
                            var m = ms[i1];
                            MeshRenderer renderer = m.GetComponent<MeshRenderer>();
                            bool isRenderer = renderer != null;
                            DebugLogError($"[Rute1_1_31][{i1 + 1}/{ms.Count}][renderer:{isRenderer}] m:{model1.ShowDistance(m)}");
                            //??????????????????

                        }
                        allModels_uid_nofound1.Add(model1);
                    }
                }
            }
        }
        else if (transforms1.Count == 1)//2.???????????? 1-1-N ????????????????????????1-1-1
        {
            var transf = transforms1[0];
            //var model = modelDict.FindModelByPos(transform);
            if (model1.Name.Contains("??????????????????"))
            {

            }
            int r1 = CheckTransform(model1, transf, $"[{model1.Name}]???1-1-N???");
            if (r1 == 1) //2.1.??????1-1-1
            {
                AddFounded1(model1, transf, BIMFoundType.ByPos111);
            }
            else if (r1 == 0)//2.2.??????1-1-N
            {
                if (CheckArg.IsUseFound2)
                {
                    AddFounded1(model1, transf, BIMFoundType.ByPos1NN);
                }
                else
                {
                    allModels_uid_found2.Add(model1);
                }
                
            }
            else //-1 ???2.3.?????? 1-1-0
            {
                allModels_uid_nofound2.Add(model1);
            }
        }
        else//3.????????????    1-N-N???????????????????????????1-1-1
        {
            int result = -1;
            for (int i = 0; i < transforms1.Count; i++)
            {
                Transform transf = transforms1[i];
                int r1 = CheckTransform(model1, transf, $"[{model1.Name}-{transf.name}({i + 1}/{transforms1.Count})]???1 -N-N???");
                if (r1 == 1)
                {
                    result = r1;

                    //3.1.??????1-1-1
                    AddFounded1(model1, transf, BIMFoundType.ByPos1NN);

                    DebugErrorLog = "";
                    break;
                }
                else
                {
                    if (CheckArg.IsUseFound2 && result == 0)
                    {
                        AddFounded1(model1, transf, BIMFoundType.ByPos1NN);
                    }
                    else
                    {
                        if (r1 > result)
                        {
                            result = r1;
                        }
                        //allModels_uid_found2.Add(model1);
                    }


                }
            }
            if (result == 1)
            {
                //?????????????????????
            }
            else if (result == 0)//3.2.??????1-1-N
            {
                if(CheckArg.IsUseFound2==false)
                    allModels_uid_found2.Add(model1);

                //if (CheckArg.IsUseFound2)
                //{
                //    AddFounded1(model1, transf, BIMFoundType.ByPos1NN);
                //}
                //else
                //{
                //    allModels_uid_found2.Add(model1);
                //}
            }
            else //-1 ???3.3.?????? 1-1-0
            {
                allModels_uid_nofound2.Add(model1);
            }
        }

        if (!string.IsNullOrEmpty(DebugErrorLog))
        {
            if (CheckArg.IsShowLog)
            {
                Debug.LogError(DebugErrorLog);
            }
            
            DebugErrorLog = "";
        }
    }

    private void AddFounded1(ModelItemInfo model1, Transform transf, BIMFoundType foundType)
    {
        //foundType???1(findByPos) 2.(findbyName) 3.(findbyClosed)
        allModels_uid_found1.Add(model1);
        TransformDict.RemoveTransform(transf);
        BIMModelInfo bim=BIMModelInfo.SetModelInfo(transf, model1);
        bim.FoundType = foundType;
        DebugErrorLog = "";
    }


    private int CheckTransform(ModelItemInfo model1, Transform transf, string logTag)
    {
        var models2 = modelDict.FindModelsByPosAndName(transf); //???Model1??????Transform1,Transform1?????????Model2??????????????????????????????????????????????????????????????????
        if (models2.Count == 0) //2.1 ?????????Model2???????????????????????????????????????????????????
        {
            //float dis = model1.GetDistance(transf);
            bool isSameName = model1.IsSameName(transf);
            var ms = modelDict.FindModelsByPos(transf);
            //allModels_uid_nofound2.Add(model1);

            DebugLogError($"{logTag}[Rute2_1][?????????Model2][{ms.Count}][{isSameName}]({model1.ShowDistance(transf)})");
            //if (ms.Count == 0)
            //{

            //}
            //else if (ms.Count == 1)
            //{

            //}
            //else
            //{
            //    for (int i1 = 0; i1 < ms.Count; i1++)
            //    {
            //        ModelItemInfo m = ms[i1];
            //        DebugLogError($"{logTag}[Rute2_1][{i1 + 1}/{ms.Count}] m:{m.ShowDistance(transf)}");
            //    }
            //}
            return -1;
        }
        else if (models2.Count == 1) //2.2 ????????????
        {
            var model2 = models2[0];
            bool r1 = CheckModel2(model1, transf, model2, $"{logTag}???2.2_One???");
            if (r1 == true)
            {
                //allModels_uid_found1.Add(model1);
                //TransformDict.RemoveTransform(transf);
                //BIMModelInfo.SetModelInfo(transf, model1);
                return 1;
            }
            else
            {
                //allModels_uid_found2.Add(model1);
                return 0;
            }
        }
        else //2.3 ????????????
        {
            bool isFound = false;
            foreach (var model2 in models2)
            {
                bool r1 = CheckModel2(model1, transf, model2, $"{logTag}???2.3_Multi???");
                if (r1 == true)
                {
                    isFound = true;
                    break;
                }
            }

            if (isFound == true)
            {
                //allModels_uid_found1.Add(model1);
                //TransformDict.RemoveTransform(transf);
                //BIMModelInfo.SetModelInfo(transf, model1);

                return 1;
            }
            else
            {
                //allModels_uid_found2.Add(model1);
                return 0;
            }
        }
    }

    private void DebugLogError(string log)
    {
        DebugErrorLog += log+"\n";

        //Debug.LogError(log);
    }

    //public bool IsFindByName1 = false;

    private bool CheckModel2(ModelItemInfo model1, Transform transf, ModelItemInfo model2, string logTag)
    {
        float dis = model1.GetDistance(transf, isCenterPos);
        bool isSameName = model1.IsSameName(transf);

        if (model2 == model1)   //2.2.1 ???????????? Model1??????Transform1,Transform1?????????Model2??????Model1
        {
            if (dis > MinDistance) //2.2.1.1 ????????????_????????????
            {
                if (isSameName) //2.2.1.1.1 ??????-
                {
                    if (dis < MinDistance * 2) // [2.2.1.1.1_1] 2??????????????????????????????????????????
                    {
                        //0.0002
                        //allModels_uid_found1.Add(model1);
                        //TransformDict.RemoveTransform(transf);
                        //BIMModelInfo.SetModelInfo(transf, model1);
                        return true;
                    }
                    else //[2.2.1.1.1_2]
                    {
                        if (CheckArg.IsByNameAfterFindModel) //???????????????
                        {
                            var ms = TransformDict.GetTransformsByName(model1.Name);
                            if (ms.Count == 1)//
                            {
                                if (dis < MaxDistance)
                                {
                                    return true; //??????1???
                                }
                                else if (InitNavisFileInfoByModelSetting.Instance.IsStructrue(transf.name) && dis < MaxDistance * 20)
                                {
                                    return true; //??????1???
                                }
                                else
                                {
                                    if(CheckArg.IsMoreDistance && dis< MaxDistance * 20)
                                    {
                                        return true; //??????1???
                                    }
                                    else
                                    {
                                        DebugLogError($"{logTag}[Rute1_2.2.1.1.1_21][????????????][{MinDistance}-{MaxDistance}][{isSameName}]{model1.ShowDistance(transf)}");
                                        return false;
                                    }
                                }
                            }
                            else if (ms.Count == 0)//?????????
                            {
                                DebugLogError($"{logTag}[Rute1_2.2.1.1.1_20][????????????][{MinDistance}-{MaxDistance}][{isSameName}]{model1.ShowDistance(transf)}");
                                return false;
                            }
                            else //
                            {
                                DebugLogError($"{logTag}[Rute1_2.2.1.1.1_22][????????????][{MinDistance}-{MaxDistance}][{isSameName}]{model1.ShowDistance(transf)}");
                                return false;
                            }
                        }
                        else
                        {

                            DebugLogError($"{logTag}[Rute1_2.2.1.1.1_3][????????????][{MinDistance}][{isSameName}]{model1.ShowDistance(transf)}");
                            return false;
                        }
                    }
                }
                else //2.2.1.1.2 ?????????????????????
                {

                    //allModels_uid_found2.Add(model1);
                    DebugLogError($"{logTag}[Rute1_2_1][????????????][{MinDistance}][{isSameName}]{model1.ShowDistance(transf)}");
                    return false;
                }
            }
            else if (isSameName == false) //2.2.1.2 ????????????????????????_??????????????????????????????????????????????????????
            {
                if (CheckArg.IsFindClosed2)
                {

                    return true;
                }
                else
                {
//???????????????????????????????????????????????????
                //allModels_uid_found2.Add(model1);
                DebugLogError($"{logTag}[Rute12][?????????-??????????????????][{MinDistance}][{isSameName}:{model1.Name} <> {transf.name}]{model1.ShowDistance(transf)}");
                return false;
                }
                
            }
            else //2.2.1.3 ???????????????
            {
                //allModels_uid_found1.Add(model1);
                //TransformDict.RemoveTransform(transf);
                //BIMModelInfo.SetModelInfo(transf, model1);
                return true;
            }
        }
        else  //2.2.2 Model1??????Transform1,Transform1?????????Model2??????Model1
        {
            var ms = modelDict.FindModelsByPos(transf);
            //allModels_uid_found2.Add(model1);
            DebugLogError($"{logTag}[Rute2_2_2][Model2??????Model1][{ms.Count}][{isSameName}]???{model1.ShowDistance(transf)}??? - ???{model2.ShowDistance(transf)}???");
            for (int i1 = 0; i1 < ms.Count; i1++)
            {
                ModelItemInfo m = ms[i1];
                bool isSameName2 = model1.IsSameName(transf);
                DebugLogError($"{logTag}[Rute2_2_2][{i1}/{ms.Count}][{isSameName2}]{m.ShowDistance(transf)}");
            }
            return false;
        }
    }

    public void SetModelList(ModelItemInfoListEx ModelList)
    {
        notFoundCount = allModels_uid_nofound1.Count + allModels_uid_nofound2.Count + allModels_uid_found2.Count;

        //allModels_uid = allModels_uid_nofound;
        allModels_uid_AllNotFound.AddRange(allModels_uid_nofound1);
        allModels_uid_AllNotFound.AddRange(allModels_uid_found2);
        allModels_uid_AllNotFound.AddRange(allModels_uid_nofound2);


        allModels_uid_AllNotFound.AddRange(ModelList.allModels_noDrawable_nozero);

        allModels_uid_AllNotFound.AddRange(ModelList.allModels_drawable_zero);
        allModels_uid_AllNotFound.AddRange(ModelList.allModels_noDrawable_zero);

        ModelList.SetList(allModels_uid_AllNotFound);
    }

    public override string ToString()
    {
        //return $"found1:{allModels_uid_found1.Count} ,found2:{allModels_uid_found2.Count} ,nofound1:{allModels_uid_nofound1.Count}, nofound2:{allModels_uid_nofound2.Count} allNoFound:{allModels_uid_AllNotFound.Count}";
        return $"??????:{allModels_uid_found1.Count} ,????????????:{allModels_uid_found2.Count} ,?????????:{allModels_uid_nofound1.Count}, ???????????????:{allModels_uid_nofound2.Count} ???????????????:{notFoundCount} ???????????????:{allModels_uid_AllNotFound.Count}";
    }
}

[Serializable]
public class CheckResultArg
{
    public bool IsShowLog = true;

    public bool IsByNameAfterFindModel = false;//1-1-N???Position?????????????????????????????????????????????????????????????????????????????????

    public bool IsMoreDistance = false;//1-1-N???Position?????????????????????????????????????????????????????????????????????????????????

    public bool IsByNameAfterNotFindModel = false;//Position??????????????????????????????????????????111?????????????????????????????????

    public bool IsOnlyName = false;//??????????????????

    public bool IsFindClosed1 = false;

    public bool IsFindClosed2 = false;

    public bool IsUseFound2 = false;

    public CheckResultArg()
    {
        Init(false, false, false,false, false,false, false, false);
    }

    public CheckResultArg(bool isShowLog, bool isFindByName1, bool isFindByName2, bool isMoreDistance, bool isOnlyName, bool isFindClosed1,bool isFindClosed2, bool isUseFound2)
    {
        Init(isShowLog, isFindByName1, isFindByName2, isMoreDistance, isOnlyName, isFindClosed1,isFindClosed2, isUseFound2);
    }

    private void Init(bool isShowLog, bool isFindByName1, bool isFindByName2, bool isMoreDistance, bool isOnlyName, bool isFindClosed1,bool isFindClosed2, bool isUseFound2)
    {
        this.IsShowLog = isShowLog;
        this.IsByNameAfterFindModel = isFindByName1;
        this.IsByNameAfterNotFindModel = isFindByName2;
        this.IsMoreDistance = isMoreDistance;
        this.IsOnlyName = isOnlyName;
        this.IsFindClosed1 = isFindClosed1;this.IsFindClosed2 = isFindClosed2;
        this.IsUseFound2 = isUseFound2;
    }

    public CheckResultArg(bool isShowLog, CheckResultArg arg)
    {
        this.IsShowLog = isShowLog;
        this.IsByNameAfterFindModel = arg.IsByNameAfterFindModel;
        this.IsByNameAfterNotFindModel = arg.IsByNameAfterNotFindModel;
        this.IsOnlyName = arg.IsOnlyName;
        this.IsMoreDistance = arg.IsMoreDistance;
        this.IsFindClosed1 = arg.IsFindClosed1;this.IsFindClosed2 = arg.IsFindClosed2;
        this.IsUseFound2 = arg.IsUseFound2;
    }
}
