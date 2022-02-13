using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PipeWeldModel : PipeModelComponent
{
    public PipeWeldData WeldData;

    public override void InitSaveData(MeshModelSaveData data)
    {
        base.InitSaveData(data);

        data.PipeWelds = null;
    }

    public PipeWeldSaveData GetSaveData()
    {
        PipeWeldSaveData data = new PipeWeldSaveData();
        InitSaveData(data);
        if (ResultGo == null)
        {
            ResultGo = this.gameObject;
        }
        if (ResultGo != null)
        {
            WeldData.start = ResultGo.transform.position;
            WeldData.direction = ResultGo.transform.up;
        }
        else
        {
            Debug.LogError($"PipeWeldModel.GetSaveData ResultGo==null:{this.gameObject} parent:{this.transform.parent}");
        }
        data.Data = WeldData;
        return data;
    }

    internal void SetSaveData(PipeWeldSaveData item)
    {
        WeldData = item.Data;
    }

    //public override void RendererModel()
    //{
    //    //base.RendererModel();
    //    GameObject go = CreateWeldGo(WeldData.start, WeldData.direction);

    //}

    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        ClearGo();
        this.generateArg = arg;
        GameObject go = CreateWeldGo(WeldData.start, WeldData.direction);
        PipeMeshGenerator weldGenerator = go.AddComponent<PipeMeshGenerator>();
        SetPipeMeshGenerator(weldGenerator, generateArg);
        weldGenerator.RenderTorusXZ();
        //weldGenerator.ShowPoints();
        var p = go.transform.parent;
        go.transform.SetParent(null);
        go.transform.up = WeldData.direction;
        go.transform.SetParent(p);
        go.transform.localScale = new Vector3(1, 2, 1);
        //Childrens.Add(go.transform);

        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        //return base.RendererModel(arg, afterName);
        this.ResultGo = go;

        //go.transform.SetParent(this.transform);

        return go;
    }

    private void SetPipeMeshGenerator(PipeMeshGenerator weldGenerator, PipeGenerateArg arg)
    {
        weldGenerator.generateOnStart = false;
        //weldGenerator.points = arg1.vertices;
        weldGenerator.pipeMaterial = arg.weldMaterial;
        weldGenerator.flatShading = false;
        weldGenerator.avoidStrangling = true;
        if (weldGenerator.pipeMaterial == null)
        {
            weldGenerator.pipeMaterial = arg.pipeMaterial;
        }
        weldGenerator.elbowRadius = WeldData.elbowRadius;
        //weldGenerator.pipeRadius = size / 5;
        //weldGenerator.pipeRadius = size / 10;
        weldGenerator.pipeRadius = WeldData.pipeRadius;
        weldGenerator.pipeSegments = arg.weldPipeSegments;
        weldGenerator.elbowSegments = arg.weldElbowSegments;
        weldGenerator.IsLinkEndStart = true;
    }

    private GameObject CreateWeldGo(Vector3 start, Vector3 direction)
    {
        MeshRenderer render = this.GetComponent<MeshRenderer>();
        if (render == null)
        {
            GameObject go = this.gameObject;
            go.name = $"Weld_1 start:{start} direction:{direction}";
            //go.transform.SetParent(this.transform.parent);
            //go.transform.localPosition = Vector3.zero;
            var p = go.transform.parent;
            go.transform.SetParent(null);
            go.transform.position = start;
            go.transform.SetParent(p);
            //Welds.Add(go);
            return go;
        }
        else
        {
            GameObject go = new GameObject();
            go.name = $"Weld_2 start:{start} direction:{direction}";
            go.transform.SetParent(this.transform.parent);
            //go.transform.localPosition = Vector3.zero;
            go.transform.position = start;
            //Welds.Add(go);
            //go.transform.SetParent(this.transform);
            return go;
        }
        
    }


}
