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

    [ContextMenu("GetPipeParts")]
    public void GetPipeParts()
    {
        ClearList();

        PipeLineModel[] pipeLines = Target.GetComponentsInChildren<PipeLineModel>(true);
        foreach(var pipe in pipeLines)
        {
            pipe.ClearChildren();
        }

        PipeElbowModel[] pipeElbows = Target.GetComponentsInChildren<PipeElbowModel>(true);
        foreach (var pipe in pipeElbows)
        {
            pipe.ClearChildren();
        }

        ModelClassDict<Transform> modelClassList =ModelMeshManager.Instance.GetPrefixNames<Transform>(Target);
        var keys = modelClassList.GetKeys();
        foreach(var key in keys)
        {
            var list = modelClassList.GetList(key);
            if (key.Contains("Pipe"))
            {
                PipeLines.AddRange(list);
            }
            else if (key.Contains("Degree_Direction_Change"))
            {
                PipeElbows.AddRange(list);
            }
            else if (key.Contains("Tee"))
            {
                PipeTees.AddRange(list);
            }
            else if (key.Contains("Reducer"))
            {
                PipeReducers.AddRange(list);
            }
            else if (key.Contains("Flange"))
            {
                PipeFlanges.AddRange(list);
            }
            else
            {
                PipeOthers.AddRange(list);
            }
        }
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

    [ContextMenu("GeneratePipe")]
    public void GeneratePipe()
    {
        GameObject builder = new GameObject("Builder");
        builder.transform.position = this.transform.position;
        builder.transform.SetParent(this.transform);
        PipeBuilder newBuilder=builder.AddComponent<PipeBuilder>();
        newBuilder.PipeModelGos = PipeLines;
        newBuilder.PipeElbowsGos = PipeElbows;
        newBuilder.CreateEachPipes();

    }
}
