using CommonUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshNode : InnerMeshNode
{
    [ContextMenu("CreateTempGo")]
    public GameObject CreateTempGo(string log)
    {
        GameObject tempCenter = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //tempCenter.transform.SetParent(node2.transform.parent);
        tempCenter.transform.up = this.GetLongShortNormalNew();
        tempCenter.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        tempCenter.name = this.name + "_TempCenter_" + log;

        tempCenter.transform.position = this.GetCenterP();
        //tempCenter.transform.position=CenterGo.transform.position;

        if (CenterGo)
        {
            GameObject centerP2 = MeshHelper.CopyGO(CenterGo);
            centerP2.transform.SetParent(tempCenter.transform);
        }
        this.transform.SetParent(tempCenter.transform);

        return tempCenter;
    }

    public static MeshNode CreateNew(GameObject go)
    {
        MeshNode node = go.GetComponent<MeshNode>();
        if (node != null)
        {
            node.ClearVertexes();
            GameObject.DestroyImmediate(node);
        }
        node = go.AddComponent<MeshNode>();
        return node;
    }

    public static MeshNode InitNodes(GameObject go)
    {
        DateTime start = DateTime.Now;
        MeshNode meshNode = go.AddMissingComponent<MeshNode>();
        meshNode.Init(0, true, p =>
        {
            ProgressBarHelper.DisplayProgressBar(p);
        });
        meshNode.GetSharedMeshList();

        var meshNodes = meshNode.GetComponentsInChildren<MeshNode>(meshNode.isIncludeInactive);
        //ProgressBarHelper.ClearProgressBar();

        //MeshRendererInfo.InitRenderers(go);

        var subms = meshNode.GetMeshNodes();
        for (int i = 0; i < subms.Count; i++)
        {
            var node = subms[i];
            ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("UpdateSharedMesh", i, subms.Count, node));
            node.GetSharedMeshList();
        }
        ProgressBarHelper.ClearProgressBar();

        Debug.Log($"MeshNode.Init count:{meshNodes.Length} time:{(DateTime.Now - start)}");
        return meshNode;
    }

    public MeshRendererAssetInfoDict assetPaths = new MeshRendererAssetInfoDict();

    public MeshRendererAssetInfoDict GetAssetPaths()
    {
        MeshRendererInfoList list = new MeshRendererInfoList(gameObject);
        MeshRendererAssetInfoDict paths = list.GetAssetPaths();
        assetPaths = paths;
        return paths;
    }
}


