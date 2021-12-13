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

    //[NonSerialized]
    public List<ModelItemInfo> allModels_uid_found1 = new List<ModelItemInfo>();
    //[NonSerialized]
    public List<ModelItemInfo> allModels_uid_found2 = new List<ModelItemInfo>();
    //[NonSerialized]
    public List<ModelItemInfo> allModels_uid_nofound1 = new List<ModelItemInfo>();
    //[NonSerialized]
    public List<ModelItemInfo> allModels_uid_nofound2 = new List<ModelItemInfo>();
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
    }

    public CheckResultArg CheckArg = new CheckResultArg();

    public void CheckResult(ModelItemInfo model1, List<Transform> transforms1)
    {
        if (transforms1 == null || transforms1.Count == 0)//1.没找到
        {
            if(model1.Name== "HorPumpBB1Asm-1-0002")
            {
                Debug.Log("HorPumpBB1Asm-1-0002");
            }
            allModels_uid_nofound1.Add(model1);

            var ms = TransformDict.GetTransformsByName(model1.Name);
            if (ms.Count == 0)
            {
                var closedT = model1.FindClosedTransform(TransformDict.ToList());
                
                //遍历全部模型

                if (CheckArg.IsFindClosed)
                {
                    AddFounded1(model1, closedT, BIMFoundType.ByClosed);
                }
                else
                {
                    DebugLogError($"【{ms.Count}】[Rute1_1_1][没找到同名的Transform][Name:{model1.Name}][{MinDistance}][{model1.ShowDistance(closedT)})");
                }
            }
            else if (ms.Count == 1)
            {
                var transf = ms[0];
                float dis = model1.GetDistance(transf);
                if (dis <= MinDistance)
                {
                    AddFounded1(model1, transf, BIMFoundType.ByName);
                }
                else
                {
                    
                    //减少难度111
                    if (CheckArg.IsFindByName2)
                    {
                        AddFounded1(model1, transf, BIMFoundType.ByName);
                    }
                    else 
                    { 
                        DebugLogError($"【{ms.Count}】[Rute1_1_2][没找到Transform][{MinDistance}] m:{model1.ShowDistance(transf)}");
                    }
                }
            }
            else
            {
                var transf = model1.FindClosedTransform(ms);
                float dis = model1.GetDistance(transf);
                if (dis <= MinDistance)
                {
                    AddFounded1(model1, transf,BIMFoundType.ByNameAndClosed);
                }
                else
                {
                    DebugLogError($"【{ms.Count}】[Rute1_1_3][没找到Transform][{MinDistance}] m:{model1.ShowDistance(transf)}");
                    for (int i1 = 0; i1 < ms.Count; i1++)
                    {
                        var m = ms[i1];
                        DebugLogError($"[Rute1_1_3][{i1 + 1}/{ms.Count}] m:{model1.ShowDistance(m)}");
                        //遍历这些模型
                    }
                }
            }
        }
        else if (transforms1.Count == 1)//2.找到一个 1-1-N 找到匹配的那一个1-1-1
        {
            var transf = transforms1[0];
            //var model = modelDict.FindModelByPos(transform);
            if (model1.Name.Contains("污油侧进油口"))
            {

            }
            int r1 = CheckTransform(model1, transf, $"[{model1.Name}]【1-1-N】");
            if (r1 == 1) //2.1.找到1-1-1
            {
                AddFounded1(model1, transf, BIMFoundType.ByPos111);
            }
            else if (r1 == 0)//2.2.找到1-1-N
            {
                allModels_uid_found2.Add(model1);
            }
            else //-1 ，2.3.找到 1-1-0
            {
                allModels_uid_nofound2.Add(model1);
            }
        }
        else//3.找到多个    1-N-N，找到匹配的那一个1-1-1
        {
            int result = -1;
            for (int i = 0; i < transforms1.Count; i++)
            {
                Transform transf = transforms1[i];
                int r1 = CheckTransform(model1, transf, $"[{model1.Name}-{transf.name}({i + 1}/{transforms1.Count})]【1 -N-N】");
                if (r1 == 1)
                {
                    result = r1;

                    //3.1.找到1-1-1
                    AddFounded1(model1, transf, BIMFoundType.ByPos1NN);

                    DebugErrorLog = "";
                    break;
                }
                else
                {
                    if (r1 > result)
                    {
                        result = r1;
                    }
                }
            }
            if (result == 1)
            {
                //在上面的循环内
            }
            else if (result == 0)//3.2.找到1-1-N
            {
                allModels_uid_found2.Add(model1);
            }
            else //-1 ，3.3.找到 1-1-0
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
        //foundType：1(findByPos) 2.(findbyName) 3.(findbyClosed)
        allModels_uid_found1.Add(model1);
        TransformDict.RemoveTransform(transf);
        BIMModelInfo bim=BIMModelInfo.SetModelInfo(transf, model1);
        bim.FoundType = foundType;
        DebugErrorLog = "";
    }


    private int CheckTransform(ModelItemInfo model1, Transform transf, string logTag)
    {
        var models2 = modelDict.FindModelsByPosAndName(transf); //【Model1找到Transform1,Transform1找到的Model2，进一步判断确认，避免重复和找到其他的对象】
        if (models2.Count == 0) //2.1 没找到Model2，奇怪，不应该，至少应该找到一个吧
        {
            //float dis = model1.GetDistance(transf);
            bool isSameName = model1.IsSameName(transf);
            var ms = modelDict.FindModelsByPos(transf);
            //allModels_uid_nofound2.Add(model1);

            DebugLogError($"{logTag}[Rute2_1][没找到Model2][{ms.Count}][{isSameName}]({model1.ShowDistance(transf)})");
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
        else if (models2.Count == 1) //2.2 找到一个
        {
            var model2 = models2[0];
            bool r1 = CheckModel2(model1, transf, model2, $"{logTag}【2.2_One】");
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
        else //2.3 找到多个
        {
            bool isFound = false;
            foreach (var model2 in models2)
            {
                bool r1 = CheckModel2(model1, transf, model2, $"{logTag}【2.3_Multi】");
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
        float dis = model1.GetDistance(transf);
        bool isSameName = model1.IsSameName(transf);

        if (model2 == model1)   //2.2.1 最理想的 Model1找到Transform1,Transform1找到的Model2就是Model1
        {
            if (dis > MinDistance) //2.2.1.1 判断距离_距离太远
            {
                if (isSameName) //2.2.1.1.1 同名-
                {
                    if (dis < MinDistance * 2) // [2.2.1.1.1_1] 2倍的距离也凑合吧，【找到了】
                    {
                        //0.0002
                        //allModels_uid_found1.Add(model1);
                        //TransformDict.RemoveTransform(transf);
                        //BIMModelInfo.SetModelInfo(transf, model1);
                        return true;
                    }
                    else //[2.2.1.1.1_2]
                    {
                        if (CheckArg.IsFindByName1) //考虑名称了
                        {
                            var ms = TransformDict.GetTransformsByName(model1.Name);
                            if (ms.Count == 1)//
                            {
                                return true; //只有1个
                            }
                            else if (ms.Count == 0)//不可能
                            {
                                DebugLogError($"{logTag}[Rute1_2.2.1.1.1_21][距离太远][{MinDistance}][{isSameName}]{model1.ShowDistance(transf)}");
                                return false;
                            }
                            else //
                            {
                                DebugLogError($"{logTag}[Rute1_2.2.1.1.1_22][距离太远][{MinDistance}][{isSameName}]{model1.ShowDistance(transf)}");
                                return false;
                            }
                        }
                        else
                        {

                            DebugLogError($"{logTag}[Rute1_2.2.1.1.1_3][距离太远][{MinDistance}][{isSameName}]{model1.ShowDistance(transf)}");
                            return false;
                        }
                    }
                }
                else //2.2.1.1.2 距离远又不同名
                {

                    //allModels_uid_found2.Add(model1);
                    DebugLogError($"{logTag}[Rute1_2_1][距离太远][{MinDistance}][{isSameName}]{model1.ShowDistance(transf)}");
                    return false;
                }
            }
            else if (isSameName == false) //2.2.1.2 判断名称是否相同_不同名，不同名也可以考虑作为找到了的
            {
                //最终可以考虑下不同名但是距离很近的
                //allModels_uid_found2.Add(model1);
                DebugLogError($"{logTag}[Rute12][不同名-但是距离很近][{MinDistance}][{isSameName}:{model1.Name} <> {transf.name}]{model1.ShowDistance(transf)}");
                return false;
            }
            else //2.2.1.3 【找到了】
            {
                //allModels_uid_found1.Add(model1);
                //TransformDict.RemoveTransform(transf);
                //BIMModelInfo.SetModelInfo(transf, model1);
                return true;
            }
        }
        else  //2.2.2 Model1找到Transform1,Transform1找到的Model2不是Model1
        {
            var ms = modelDict.FindModelsByPos(transf);
            //allModels_uid_found2.Add(model1);
            DebugLogError($"{logTag}[Rute2_2_2][Model2不是Model1][{ms.Count}][{isSameName}]【{model1.ShowDistance(transf)}】 - 【{model2.ShowDistance(transf)}】");
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
        return $"found1:{allModels_uid_found1.Count} ,found2:{allModels_uid_found2.Count} ,nofound1:{allModels_uid_nofound1.Count}, nofound2:{allModels_uid_nofound2.Count} allNoFound:{allModels_uid_AllNotFound.Count}";
    }
}

public class CheckResultArg
{
    public bool IsShowLog = true;

    public bool IsFindByName1 = false;//1-1-N

    public bool IsFindByName2 = false;

    public bool IsFindClosed = false;

    public CheckResultArg()
    {

    }

    public CheckResultArg(bool isShowLog,bool isFindByName1, bool isFindByName2, bool isFindClosed)
    {
        this.IsShowLog = isShowLog;
        this.IsFindByName1 = isFindByName1;
        this.IsFindByName2 = isFindByName2;
        this.IsFindClosed = isFindClosed;
}
}
