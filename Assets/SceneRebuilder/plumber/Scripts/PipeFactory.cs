using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeFactory : MonoBehaviour
{
    public GameObject Target;

    public List<MeshRenderer> PipeLines = new List<MeshRenderer>();

    public List<MeshRenderer> PipeElbows = new List<MeshRenderer>();

    public List<MeshRenderer> PipeTees = new List<MeshRenderer>();

    public List<MeshRenderer> PipeReducers = new List<MeshRenderer>();

    public List<MeshRenderer> PipeFlanges = new List<MeshRenderer>();

    public List<MeshRenderer> PipeOthers = new List<MeshRenderer>();

    private void ClearList()
    {
        PipeLines = new List<MeshRenderer>();

        PipeElbows = new List<MeshRenderer>();

        PipeTees = new List<MeshRenderer>();

        PipeReducers = new List<MeshRenderer>();

        PipeFlanges = new List<MeshRenderer>();

        PipeOthers = new List<MeshRenderer>();
    }

    [ContextMenu("GetPipeParts")]
    public void GetPipeParts()
    {
        ClearList();

        ModelClassDict<MeshRenderer> modelClassList =ModelMeshManager.Instance.GetPrefixNames(Target);
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
}
