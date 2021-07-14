using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingModelInfoList : MonoBehaviour
{
    public int AllRendererCount = 0;
    public float AllVertextCount = 0;
    public float ShowVertextCount = 0;
    public BuildingModelInfo[] Buildings;

    [ContextMenu("UpdateBuildings")]
    public void UpdateBuildings()
    {
        Buildings = gameObject.GetComponentsInChildren<BuildingModelInfo>();
        AllRendererCount = 0;
        AllVertextCount = 0;
        ShowVertextCount = 0;
        foreach (var b in Buildings)
        {
            AllRendererCount += b.AllRendererCount;
            AllVertextCount += b.AllVertextCount;

            ShowVertextCount += b.Out0BigVertextCount;
        }
    }
}
