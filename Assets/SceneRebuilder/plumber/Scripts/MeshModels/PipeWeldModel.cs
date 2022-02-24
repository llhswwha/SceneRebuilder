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
        if (ResultGo == null)
        {
            ResultGo = this.gameObject;
        }

        PipeWeldSaveData data = new PipeWeldSaveData();
        InitSaveData(data);

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

    public override GameObject CreateModelByPrefab()
    {
        if (ResultGo != null && ResultGo != this.gameObject)
        {
            GameObject.DestroyImmediate(ResultGo);
        }

        GameObject go = GameObject.Instantiate(PipeFactory.Instance.GetPipeModelUnitPrefab_Weld(WeldData));
        go.SetActive(true);

        go.name = this.name + "_New2";
        go.transform.position = WeldData.start;
        go.transform.up = WeldData.direction;
        go.transform.SetParent(this.transform.parent);
        ResultGo = go;

        this.gameObject.SetActive(false);
        return go;
    }

    public override GameObject CreateModelByPrefabMesh()
    {
        if (ResultGo != null && ResultGo != this.gameObject)
        {
            GameObject.DestroyImmediate(ResultGo);
        }

        GameObject prefab = this.gameObject;
        prefab.SetActive(true);
        prefab.name = this.name + "_New3";
        //SetPrefabTransfrom(prefab);

        prefab.transform.position = WeldData.start;
        prefab.transform.up = WeldData.direction;

        MeshHelper.SetNewMesh(prefab, PipeFactory.Instance.GetPipeModelUnitPrefabMesh_Weld(WeldData));

        MeshRenderer renderer = prefab.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            renderer = prefab.AddMissingComponent<MeshRenderer>();
            renderer.sharedMaterial = PipeFactory.Instance.generateArg.weldMaterial;
        }
        ResultGo = prefab;
        return prefab;
    }

    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        ClearGo();
        this.generateArg = arg;

        if(PipeFactory.Instance.IsCreatePipeByUnityPrefab)
        {
            return CreateModelByPrefabMesh();
        }
        else
        {
            GameObject go = CreateWeldGo(this.gameObject, WeldData.start, WeldData.direction);
            PipeMeshGenerator weldGenerator = go.AddComponent<PipeMeshGenerator>();
            SetPipeMeshGenerator(weldGenerator, generateArg, WeldData);
            weldGenerator.RenderTorusXZ();

            //weldGenerator.ShowPoints();

            var p = go.transform.parent;
            go.transform.SetParent(null);
            go.transform.localScale = new Vector3(1, 2, 1);
            go.transform.up = WeldData.direction;
            go.transform.SetParent(p);

            //Childrens.Add(go.transform);

            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            //return base.RendererModel(arg, afterName);
            this.ResultGo = go;

            //go.transform.SetParent(this.transform);
            return go;
        }
    }

    public static void SetPipeMeshGenerator(PipeMeshGenerator weldGenerator, PipeGenerateArg arg,PipeWeldData data)
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
        weldGenerator.elbowRadius = data.elbowRadius;
        //weldGenerator.pipeRadius = size / 5;
        //weldGenerator.pipeRadius = size / 10;
        weldGenerator.pipeRadius = data.pipeRadius;
        weldGenerator.pipeSegments = arg.weldPipeSegments;
        weldGenerator.elbowSegments = arg.weldElbowSegments;
        weldGenerator.IsLinkEndStart = true;
    }

    public static GameObject CreateWeldGo(GameObject target,Vector3 start, Vector3 direction)
    {
        MeshRenderer render = target.GetComponent<MeshRenderer>();
        if (render == null)
        {
            GameObject go = target.gameObject;
            //go.name = $"Weld_1 start:{start} direction:{direction}";
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
            //go.name = $"Weld_2 start:{start} direction:{direction}";
            go.name = target.name+"_New";
            go.transform.SetParent(target.transform.parent);
            //go.transform.localPosition = Vector3.zero;
            go.transform.position = start;
            //Welds.Add(go);
            //go.transform.SetParent(this.transform);
            return go;
        }
        
    }


}
