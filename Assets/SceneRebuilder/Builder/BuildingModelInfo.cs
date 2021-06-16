using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingModelInfo : MonoBehaviour
{
    public GameObject InPart;

    public GameObject OutPart0;
    public GameObject OutPart1;

    public float InVertextCount = 0;
    public float Out0VertextCount = 0;
    public float Out1VertextCount = 0;

    public int InRendererCount = 0;
    public int Out0RendererCount = 0;
    public int Out1RendererCount = 0;

    public int AllRendererCount = 0;
    public float AllVertextCount = 0;

    [ContextMenu("InitInOut")]
    public void InitInOut()
    {
        List<Transform> children = new List<Transform>();
        for(int i=0;i<transform.childCount;i++)
        {
            var child = transform.GetChild(i);
            if(child.name=="In")
            {
                InPart = child.gameObject;
            }
            if (child.name == "Out0"|| child.name == "Out")
            {
                OutPart0 = child.gameObject;
            }
            if (child.name == "Out1")
            {
                OutPart1 = child.gameObject;
            }
            children.Add(child);
        }

        if(OutPart1==null&& OutPart0==null&& InPart==null)
        {
            GameObject outRoot = new GameObject("Out0");
            outRoot.transform.position = this.transform.position;
            foreach(var child in children)
            {
                child.SetParent(outRoot.transform);
            }
            OutPart0 = outRoot;
            outRoot.transform.SetParent(this.transform);
        }

        GetInOutVertextCount();

        HideDetail();
    }

    private void GetInOutVertextCount()
    {
        InVertextCount = GetChildrenVertextCount(InPart, out InRendererCount,false);
        Out0VertextCount = GetChildrenVertextCount(OutPart0,out Out0RendererCount, true);
        Out1VertextCount = GetChildrenVertextCount(OutPart1,out Out1RendererCount, false);
        AllVertextCount = InVertextCount + Out0VertextCount + Out1VertextCount;
        AllRendererCount = InRendererCount + Out0RendererCount + Out1RendererCount;
    }

    private float GetChildrenVertextCount(GameObject go, out int renderCount, bool showLog)
    {
        renderCount = 0;
        if (go == null)
        {
            return 0;
        }
        float count = 0;
        for (int i = 0; i < go.transform.childCount; i++)
        {
            var child = go.transform.GetChild(i);
            int childRendererCount = 0;
            float childVertexCount = GetVertextCount(child.gameObject, out childRendererCount, showLog);
            count += childVertexCount;
            renderCount += childRendererCount;
            if(childVertexCount>1)
                Debug.LogError($"GetChildrenVertextCount[{i}]    {childVertexCount:F3}    {go.name}->{child.name}");
        }
        return count;
    }

    private float GetVertextCount(GameObject go,out int renderCount,bool showLog)
    {
        renderCount = 0;
        if (go == null)
        {
            return 0;
        }
        float count = 0;
        var filters = go.GetComponentsInChildren<MeshFilter>(true).ToList().FindAll(i=>i!=null&&i.sharedMesh!=null);
        //filters.Sort((a, b) => b.sharedMesh.vertexCount.CompareTo(a.sharedMesh.vertexCount));
        filters.Sort((a, b) => a.sharedMesh.vertexCount.CompareTo(b.sharedMesh.vertexCount));

        for (int i = 0; i < filters.Count; i++)
        {
            MeshFilter filter = filters[i];
            if (filter == null) continue;
            if (filter.sharedMesh == null) continue;
            float vertextCount = filter.sharedMesh.vertexCount / 10000.0f;
            count += vertextCount;
            if (showLog)
            {
                if (vertextCount > 5f)
                {
                    Debug.LogWarning($"GetVertextCount[{i}]    {vertextCount:F3}    {go.name}->{filter.name}|{filter.transform.parent}");
                }
                else if (vertextCount > 1f)
                {
                    Debug.Log($"GetVertextCount[{i}]    {vertextCount:F3}    {go.name}->{filter.name}|{filter.transform.parent}");
                }
            }

        }
        renderCount = filters.Count;
        return count;
    }

    private void Awake()
    {
        InitInOut();
    }

    [ContextMenu("FindInGos")]
    public void FindInGos()
    {

    }

    [ContextMenu("HideIn")]
    public void HideIn()
    {
        if (InPart)
            InPart.SetActive(false);
    }

    [ContextMenu("ShowIn")]
    public void ShowIn()
    {
        if (InPart)
            InPart.SetActive(true);
    }

    public bool IsDetailVisible = false;

    [ContextMenu("HideDetail")]
    public void HideDetail()
    {
        IsDetailVisible = false;
        if(InPart)
            InPart.SetActive(false);
        if(OutPart1)
            OutPart1.SetActive(false);
    }

    [ContextMenu("ShowDetail")]
    public void ShowDetail()
    {
        if(InPart)
            InPart.SetActive(true);
        if(OutPart1)
            OutPart1.SetActive(true);
    }

    [ContextMenu("CombinedInOut")]
    public void CombinedInOut()
    {
        //var inCombined=
    }

    [ContextMenu("CombinedInOut_Tree")]
    public void CombinedInOut_Tree()
    {
        //var inCombined=
    }
}
