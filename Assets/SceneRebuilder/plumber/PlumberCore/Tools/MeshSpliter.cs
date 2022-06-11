using CommonExtension;
using CommonUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSpliter : MonoBehaviour
{
    public static GameObject SplitGo(GameObject go, ProgressArg p0=null)
    {
        return SplitInner(go, p0);
    }

    [ContextMenu("Split")]
    public void Split()
    {
        SplitInner(this.gameObject);
    }

    private static GameObject SplitInner(GameObject go, ProgressArg p0 = null)
    {
        GameObject root = new GameObject(go.name + "_Splited");
        root.transform.position = Vector3.zero;

        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        Material mat = mr.sharedMaterial;
        List<MeshTriangleList> parts = MeshTriangles.GetSplitedParts(go.transform, 0, p0);
        root.name = go.name + "_Splited_" + parts.Count;
        for (int i = 0; i < parts.Count; i++)
        {
            MeshTriangleList part = parts[i];
            GameObject obj = part.CreateMeshObject(mat, $"{go.name}_P{i + 1:000}");
            obj.transform.SetParent(root.transform);
        }

        root.transform.position = go.transform.position;
        root.transform.SetParent(go.transform.parent);
        return root;
    }

    public float pScale = 0.1f;

    [ContextMenu("ShowVertexes")]
    public void ShowVertexes()
    {
        ClearChildren();
        VertexHelper.ShowVertexes(transform, pScale);
    }

    [ContextMenu("ClearChildren")]
    public void ClearChildren()
    {
        gameObject.ClearChildren();
    }

    [ContextMenu("ClearDebugInfoGos")]
    public void ClearDebugInfoGos()
    {
        DebugInfoRoot.ClearDebugInfoGos(transform);
    }


    [ContextMenu("DebugShowTriangles")]
    public void DebugShowTriangles()
    {
        ClearChildren();
        ClearDebugInfoGos();

        MeshTriangles.DebugShowTriangles(gameObject, pScale);
    }

    [ContextMenu("DebugShowSharedPoints")]
    public void DebugShowSharedPoints()
    {
        //ClearChildren();
        ClearDebugInfoGos();

        MeshTriangles.DebugShowSharedPointsByPoint(transform, pScale);
    }

    [ContextMenu("DebugShowSplitedParts")]
    public void DebugShowSplitedParts()
    {
        //ClearChildren();
        ClearDebugInfoGos();

        MeshTriangles.DebugShowSplitedParts(transform, pScale, maxPartCount);
    }

    public int maxPartCount = 0;
}
