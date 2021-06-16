using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingModelManager : MonoBehaviour
{
    public List<BuildingModelInfo> Buildings = new List<BuildingModelInfo>();

    public float InVertextCount = 0;
    public float Out0VertextCount = 0;
    public float Out1VertextCount = 0;

    public int InRendererCount = 0;
    public int Out0RendererCount = 0;
    public int Out1RendererCount = 0;

    public int AllRendererCount = 0;
    public float AllVertextCount = 0;

    private void ClearCount()
    {
        InVertextCount = 0;
        Out0VertextCount = 0;
        Out1VertextCount = 0;

        InRendererCount = 0;
        Out0RendererCount = 0;
        Out1RendererCount = 0;

        AllRendererCount = 0;
        AllVertextCount = 0;
    }

    [ContextMenu("Init")]
    public void Init()
    {
        Buildings.AddRange(GameObject.FindObjectsOfType<BuildingModelInfo>());

        Buildings.Sort((a, b) =>
        {
            return b.Out0VertextCount.CompareTo(a.Out0VertextCount);
        });

        ClearCount();
        foreach (var b in Buildings)
        {
            InVertextCount += b.InVertextCount;
            Out0VertextCount += b.Out0VertextCount;
            Out1VertextCount += b.Out1VertextCount;

            InRendererCount += b.InRendererCount;
            Out0RendererCount += b.Out0RendererCount;
            Out1RendererCount += b.Out1RendererCount;

            AllRendererCount += b.AllRendererCount;
            AllVertextCount += b.AllVertextCount;
        }
    }

    [ContextMenu("CreatePrefabs")]
    public void CreatePrefabs()
    {

    }
}
