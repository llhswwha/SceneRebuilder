using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeFactory : SingletonBehaviour<PipeFactory>
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

        PipeModelBase[] pipeModels = Target.GetComponentsInChildren<PipeModelBase>(true);
        foreach (var pipe in pipeModels)
        {
            if (pipe == null) continue;
            pipe.ClearChildren();
            GameObject.DestroyImmediate(pipe);
        }

        PipeMeshGeneratorBase[] pipes = Target.GetComponentsInChildren<PipeMeshGeneratorBase>(true);
        foreach (var pipe in pipes)
        {
            if (pipe == null) continue;
            GameObject.DestroyImmediate(pipe.gameObject);
        }

        OBBCollider[] obbs = Target.GetComponentsInChildren<OBBCollider>(true);
        foreach (var obb in obbs)
        {
            if (obb == null) continue;
            GameObject.DestroyImmediate(obb);
        }
    }

    public void ClearGeneratedObjs()
    {
        if (newBuilder != null)
        {
            newBuilder.ClearGeneratedObjs();
        }
    }



    public void OneKey(bool isJob)
    {
        DateTime start = DateTime.Now;

        MeshNode meshNode = MeshNode.InitNodes(Target);
        meshNode.GetSharedMeshList();
        TargetInfo = meshNode.GetVertexInfo();
        TargetVertexCount = meshNode.VertexCount;

        this.GetPipeParts();

        if (isJob)
        {
            this.GetPipeInfosJob();
        }
        else
        {
            this.GetPipeInfos();
        }

        this.ClearGeneratedObjs();
        this.RendererEachPipes();

        if (IsCheckResult)
        {
            if (isJob)
            {
                this.CheckResultsJob();
            }
            else
            {
                this.CheckResults();
            }
        }


        this.MovePipes();

        if(IsReplaceOld)
        {
            this.ReplaceOld();
        }
        
        var pres1=this.PrefabPipes();
        var pres2 = this.PrefabOthers();

        //ResultInfo=ShowTargetInfo(Target);

        MeshNode meshNode2 = MeshNode.InitNodes(Target);
        SharedMeshInfoList sInfo=meshNode2.GetSharedMeshList();
        ResultInfo = meshNode2.GetVertexInfo();
        ResultVertexCount = meshNode2.VertexCount;
        SharedResultVertexCountCount = sInfo.sharedVertexCount;

        Debug.LogError($"OneKey time:{DateTime.Now-start} Models:{newBuilder.PipeModels.Count+this.PipeOthers.Count}({newBuilder.PipeModels.Count}+{this.PipeOthers.Count}) Prefabs:{pres1.Count+pres2.Count}({pres1.Count}+{pres2.Count}) [TargetInfo:{TargetInfo} -> ResultInfo:{ResultInfo}]({ResultVertexCount/TargetVertexCount:P2},{SharedResultVertexCountCount / TargetVertexCount:P2})");
    }

    [ContextMenu("GetPipeParts")]
    public void GetPipeParts()
    {
        ClearDebugObjs();

        ClearGeneratedObjs();

        GetModelClass();

        ShowAll();

        InitPipeBuilder();
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

    public bool isUniformRaidus = false;

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

        Debug.Log($"GetModelClass keys:{keys.Count}");
    }

    public float ResultVertexCount = 0;

    public float SharedResultVertexCountCount = 0;

    public string ResultInfo = "";

    public float TargetVertexCount = 0;

    public string TargetInfo = "";

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
        SetListVisible(PipeTees, isVisible);
        SetListVisible(PipeOthers, isVisible);
    }

    public void SetListVisible(List<Transform> renderers,bool isVisible)
    {
        foreach(var item in renderers)
        {
            item.gameObject.SetActive(isVisible);
        }
    }

    public Transform OthersRoot = null;

    public void MoveOthersParent()
    {
        OthersRoot = RendererId.MoveTargetsParent(PipeOthers, OthersRoot, "PipeOther");
        OthersRoot.transform.SetParent(this.transform);
    }

    public void RecoverOthersParent()
    {
        RendererId.RecoverTargetsParent(PipeOthers, null);
    }

    public PrefabInfoList PrefabPipes()
    {
        DateTime start = DateTime.Now;
        //MoveOthersParent();
        ////this.PipeOthers;
        ////MeshRendererInfoList list=new MeshRendererInfoList(this.PipeOthers)
        //ModelClassDict<Transform> modelClassList = ModelMeshManager.Instance.GetPrefixNames<Transform>(OthersRoot.gameObject);
        //var keys = modelClassList.GetKeys();

        PrefabInfoList prefabs = PrefabInstanceBuilder.Instance.GetPrefabsOfList(newBuilder.PipeGenerators, true);

        Debug.Log($"PrefabPipes time:{DateTime.Now - start} Pipes:{newBuilder.PipeGenerators.Count} prefabs:{prefabs.Count}");

        //RecoverOthersParent();
        return prefabs;
    }

    public PrefabInfoList PrefabOthers()
    {
        DateTime start = DateTime.Now;
        //MoveOthersParent();
        ////this.PipeOthers;
        ////MeshRendererInfoList list=new MeshRendererInfoList(this.PipeOthers)
        //ModelClassDict<Transform> modelClassList = ModelMeshManager.Instance.GetPrefixNames<Transform>(OthersRoot.gameObject);
        //var keys = modelClassList.GetKeys();



        PrefabInfoList prefabs =PrefabInstanceBuilder.Instance.GetPrefabsOfList(PipeOthers, true);
        PipeOthers.Clear();
        PipeOthers.AddRange(prefabs.GetComponents<Transform>());

        Debug.Log($"PrefabOthers time:{DateTime.Now-start} others:{this.PipeOthers.Count} prefabs:{prefabs.Count}");

        //RecoverOthersParent();
        return prefabs;
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

    public PipeRunList GetPipeRunList()
    {
        if (newBuilder == null) return new PipeRunList();
        return newBuilder.pipeRunList;
    }

    [ContextMenu("GetInfoAndCreateEachPipes")]
    public void GetInfoAndCreateEachPipes()
    {
        InitPipeBuilder();
        newBuilder.GetInfoAndCreateEachPipes();
    }

    public void ReplaceOld()
    {
        newBuilder.ReplaceOld();
        
        if (targetNew)
        {
            GameObject.DestroyImmediate(targetNew);
        }
    }

    [ContextMenu("GetPipeInfos")]
    public void GetPipeInfos()
    {
        InitPipeBuilder();

        newBuilder.ClearGeneratedObjs();

        newBuilder.isUniformRaidus = this.isUniformRaidus;
        newBuilder.GetPipeInfosEx();

        ProgressBarHelper.ClearProgressBar();
    }

    [ContextMenu("GetObbInfJobs")]
    public void GetObbInfosJob()
    {
        InitPipeBuilder();
        newBuilder.GetObbInfosJob();
    }

    [ContextMenu("GetObbInfos")]
    public void GetObbInfos()
    {
        InitPipeBuilder();
        newBuilder.GetObbInfos();
    }

    [ContextMenu("GetPipeInfosJob")]
    public void GetPipeInfosJob()
    {
        InitPipeBuilder();

        newBuilder.ClearGeneratedObjs();

        newBuilder.isUniformRaidus = this.isUniformRaidus;
        newBuilder.GetPipeInfosJob();

        ProgressBarHelper.ClearProgressBar();
    }

    [ContextMenu("CreatePipeRunList")]
    public void CreateRunList()
    {
        newBuilder.IsCreatePipeRuns = true;
        newBuilder.CreatePipeRunList();
    }

    [ContextMenu("RendererEachPipes")]
    public void RendererEachPipes()
    {
        //InitPipeBuilder();
        newBuilder.isUniformRaidus = this.isUniformRaidus;
        newBuilder.IsCreatePipeRuns = this.IsCreatePipeRuns;
        newBuilder.IsSaveMaterials = this.IsSaveMaterials;
        newBuilder.IsCopyComponents = this.IsCopyComponents;
        //newBuilder.CreatePipeRunList();

        newBuilder.NewObjName = "_New";
        newBuilder.generateArg = generateArg;

        newBuilder.RendererPipesEx();

        ProgressBarHelper.ClearProgressBar();
    }

    [ContextMenu("CheckResults")]
    public void CheckResults()
    {
        newBuilder.CheckResults();
        ProgressBarHelper.ClearProgressBar();
    }

    [ContextMenu("CheckResultsJob")]
    public void CheckResultsJob()
    {
        newBuilder.CheckResultsJob();
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
        newBuilder.IsCreatePipeRuns = this.IsCreatePipeRuns;

        if (EnablePipeLine)
        {
            newBuilder.PipeLineGos = PipeLines;
        }
        else
        {
            newBuilder.PipeLineGos = new List<Transform>();
        }
        if(EnablePipeElbow)
        {
            newBuilder.PipeElbowGos = PipeElbows;
        }
        else
        {
            newBuilder.PipeElbowGos = new List<Transform>();
        }

        if (EnablePipeReducer)
        {
            newBuilder.PipeReducerGos = PipeReducers;
        }
        else
        {
            newBuilder.PipeReducerGos = new List<Transform>();
        }
        if (EnablePipeFlange)
        {
            newBuilder.PipeFlangeGos = PipeFlanges;
        }
        else
        {
            newBuilder.PipeFlangeGos = new List<Transform>();
        }
        if (EnablePipeTee)
        {
            newBuilder.PipeTeeGos = PipeTees;
        }
        else
        {
            newBuilder.PipeTeeGos = new List<Transform>();
        }
    }

    public bool EnablePipeLine = true;

    public bool EnablePipeElbow = true;

    public bool EnablePipeReducer = true;

    public bool EnablePipeTee = true;

    public bool EnablePipeFlange = true;

    GameObject targetNew;

    public bool IsMoveResultToFactory = true;

    public bool IsSaveMaterials = true;

    public bool IsCopyComponents = true;

    public bool IsCreatePipeRuns = false;

    public bool IsReplaceOld = true;

    public bool IsCheckResult = false;

    public void MovePipes()
    {
        if (IsMoveResultToFactory == false) return;

        if (targetNew != null)
        {
            if (targetNew.transform.childCount == 0)
            {
                GameObject.DestroyImmediate(targetNew);
            }
        }

        targetNew = new GameObject();
        targetNew.name = Target.name + "_New";
        targetNew.transform.position = Target.transform.position;
        targetNew.transform.SetParent(Target.transform.parent);

        if (IsCreatePipeRuns)
        {
            if (newBuilder.pipeRunList != null)
            {
                var runs = newBuilder.pipeRunList.PipeRunGos;
                foreach (var run in runs)
                {
                    if (run == null) continue;
                    run.transform.SetParent(targetNew.transform);
                }
            }
            else
            {
                var newPipes = newBuilder.NewPipeList;
                foreach (var pipe in newPipes)
                {
                    if (pipe == null) continue;
                    pipe.SetParent(targetNew.transform);
                }
            }
        }
        else
        {
            var newPipes = newBuilder.NewPipeList;
            foreach (var pipe in newPipes)
            {
                if (pipe == null) continue;
                pipe.SetParent(targetNew.transform);
            }
        }


        targetNew.transform.SetParent(newBuilder.transform);

        ResultInfo=ShowTargetInfo(targetNew);
    }

    private string ShowTargetInfo(GameObject t)
    {
        MeshNode meshNode = MeshNode.InitNodes(t);
        meshNode.GetSharedMeshList();
        return meshNode.GetVertexInfo();
    }

    public Material pipeMaterial;

    public Material weldMaterial;

    public PipeGenerateArg generateArg = new PipeGenerateArg();

    public List<PipeModelBase> RunTestModels = new List<PipeModelBase>();

    public PipeRunList TestRunList = new PipeRunList();

    public void TestModelIsConnected()
    {
        TestRunList=newBuilder.TestModelIsConnected(RunTestModels);
    }
}
