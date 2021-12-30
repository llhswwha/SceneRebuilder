using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeFactory : MonoBehaviour
{
    public GameObject Target;

    public List<Transform> PipeLines = new List<Transform>();

    public List<Transform> PipeElbows = new List<Transform>();

    public List<Transform> PipeTees = new List<Transform>();

    public List<Transform> PipeReducers = new List<Transform>();

    public List<Transform> PipeFlanges = new List<Transform>();

    public List<Transform> PipeOthers = new List<Transform>();

    private void ClearList()
    {
        PipeLines = new List<Transform>();

        PipeElbows = new List<Transform>();

        PipeTees = new List<Transform>();

        PipeReducers = new List<Transform>();

        PipeFlanges = new List<Transform>();

        PipeOthers = new List<Transform>();
    }

    public void ClearDebugObjs()
    {
        ClearList();

        PipeLineModel[] pipeLines = Target.GetComponentsInChildren<PipeLineModel>(true);
        foreach (var pipe in pipeLines)
        {
            if (pipe == null) continue;
            pipe.ClearChildren();
        }

        PipeElbowModel[] pipeElbows = Target.GetComponentsInChildren<PipeElbowModel>(true);
        foreach (var pipe in pipeElbows)
        {
            if (pipe == null) continue;
            pipe.ClearChildren();
        }

        PipeMeshGenerator[] pipes = Target.GetComponentsInChildren<PipeMeshGenerator>(true);
        foreach (var pipe in pipes)
        {
            if (pipe == null) continue;
            GameObject.DestroyImmediate(pipe.gameObject);
        }
    }

    public void ClearGeneratedObjs()
    {
        if (newBuilder != null)
        {
            newBuilder.ClearGeneratedObjs();
        }
    }

    [ContextMenu("GetPipeParts")]
    public void GetPipeParts()
    {
        ClearDebugObjs();

        ClearGeneratedObjs();

        GetModelClass();

        ShowAll();
    }

    public void AddList(List<Transform> list,List<Transform> newList)
    {
        foreach (var item in newList)
        {
            if (item.GetComponent<MeshRenderer>() == null) continue;
            list.Add(item);
        }
        //list.AddRange(newList);
    }

    private void GetModelClass()
    {
        ModelClassDict<Transform> modelClassList = ModelMeshManager.Instance.GetPrefixNames<Transform>(Target);
        var keys = modelClassList.GetKeys();
        foreach (var key in keys)
        {
            var list = modelClassList.GetList(key);
            if (key.Contains("Pipe"))
            {
                //foreach(var item in list)
                //{
                //    if (item.GetComponent<MeshRenderer>() == null) continue;
                //    PipeLines.Add(item);
                //}
                //PipeLines.AddRange(list);

                AddList(PipeLines, list);
            }
            else if (key.Contains("Degree_Direction_Change"))
            {
                //PipeElbows.AddRange(list);
                AddList(PipeElbows, list);
            }
            else if (key.Contains("Tee"))
            {
                //PipeTees.AddRange(list);
                AddList(PipeTees, list);
            }
            else if (key.Contains("Reducer"))
            {
                //PipeReducers.AddRange(list);
                AddList(PipeReducers, list);
            }
            else if (key.Contains("Flange"))
            {
                //PipeFlanges.AddRange(list);
                AddList(PipeFlanges, list);
            }
            else
            {
                //PipeOthers.AddRange(list);
                AddList(PipeOthers, list);
            }
        }
    }

    public string ResultInfo = "";

    public string GetResultInfo()
    {
        return ResultInfo;
    }

    [ContextMenu("ShowAll")]
    public void ShowAll()
    {
        SetAllVisible(true);
    }

    public void SetAllVisible(bool isVisible)
    {
        SetListVisible(PipeLines, isVisible);
        SetListVisible(PipeElbows, isVisible);
        SetListVisible(PipeReducers, isVisible);
        SetListVisible(PipeFlanges, isVisible);
        SetListVisible(PipeOthers, isVisible);
    }

    public void SetListVisible(List<Transform> renderers,bool isVisible)
    {
        foreach(var item in renderers)
        {
            item.gameObject.SetActive(isVisible);
        }
    }

    [ContextMenu("HidAll")]
    public void HidAll()
    {
        SetAllVisible(false);
    }

    [ContextMenu("OnlyShowPipe")]
    public void OnlyShowPipe()
    {
        HidAll();
        SetListVisible(PipeLines, true);
    }

    public PipeBuilder newBuilder;

    public List<PipeModelBase> GetPipeModels()
    {
        if (newBuilder == null) return new List<PipeModelBase>();
        return newBuilder.PipeModels;
    }

    [ContextMenu("GetInfoAndCreateEachPipes")]
    public void GetInfoAndCreateEachPipes()
    {
        InitPipeBuilder();
        newBuilder.GetInfoAndCreateEachPipes();
    }

    [ContextMenu("GetPipeInfos")]
    public void GetPipeInfos()
    {
        InitPipeBuilder();

        newBuilder.ClearGeneratedObjs();
        newBuilder.GetPipeInfosEx();

        ProgressBarHelper.ClearProgressBar();
    }

    [ContextMenu("RendererEachPipes")]
    public void RendererEachPipes()
    {
        //InitPipeBuilder();
        newBuilder.NewObjName = "_New";
        newBuilder.generateArg = generateArg;
        newBuilder.RendererPipesEx();

        ProgressBarHelper.ClearProgressBar();
    }

    private void InitPipeBuilder()
    {
        if (newBuilder == null)
        {
            GameObject builder = new GameObject("Builder");
            builder.transform.position = this.transform.position;
            builder.transform.SetParent(this.transform);
            newBuilder = builder.AddComponent<PipeBuilder>();
        }
        newBuilder.NewObjName = "_New";
        newBuilder.generateArg = generateArg;
        newBuilder.PipeLineGos = PipeLines;
        newBuilder.PipeElbowsGos = PipeElbows;
    }

    GameObject targetNew;

    public void MovePipes()
    {
        var newPipes = newBuilder.NewPipeList;

        targetNew = new GameObject();
        targetNew.name = Target.name + "_New";
        targetNew.transform.position = Target.transform.position;
        targetNew.transform.SetParent(Target.transform.parent);
        foreach (var pipe in newPipes)
        {
            if (pipe == null) continue;
            pipe.SetParent(targetNew.transform);
        }
        targetNew.transform.SetParent(newBuilder.transform);

        MeshNode meshNode= MeshNode.InitNodes(targetNew);
        ResultInfo = meshNode.GetTitle();
    }

    public PipeGenerateArg generateArg = new PipeGenerateArg();
}