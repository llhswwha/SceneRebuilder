using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class StaticCullingTestReporter 
{
    public static StaticCullingTestData Current = null;

    public static void StartTest(string txt)
    {
        if (Current != null)
        {
            list.Add(Current);
        }
        Current = new StaticCullingTestData();
    }

    public static void EndTest(string txt)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(txt);
        sb.AppendLine(Current.ToString());
        sb.AppendLine("--------------------------");

        list.Add(Current);
        foreach (var item in list)
        {
            sb.AppendLine(item.ToString());
        }
        Debug.LogError(sb.ToString());
        Current = null;
        list.Clear();
    }

    public static List<StaticCullingTestData> list = new List<StaticCullingTestData>();
}

public class StaticCullingTestData
{
    //    ��� Area��С  CellSize JobsCount   DirectionCount FastBake    OptimizeTree Time    CastersCount TreeNodeCount   OptimizeCount TotalVisible    VisibleLeaf AvgVisible  Depth
    //1	(333.7,289.3,560.8)	10	20	2000	TRUE TRUE		142.6s	60900	112349	54	7657422	56175	136	17
    public Vector3 areaSize;
    public int ObjectsCount;
    public float CellSize;

    public int JobsPerObject;
    public int DirectionCount;
    public bool FastBake;
    public bool OptimizeTree;

    public double Time;

    public int CastersCount;

    public int TreeNodeCount;

    public int OptimizeCount;

    public int TotalVisible;

    public int VisibleLeaf;

    public int AvgVisible;

    public int TreeDepth;

    public int RayPointsCountAll;

    public int RayPointsCount;

    public int AvgRayPointsCount;

    public override string ToString()
    {
        // string line1= "{areaSize}\t{ObjectsCount}\t{CellSize}\t{JobsPerObject}\t{DirectionCount}\t{FastBake}\t{OptimizeTree}\t\t{Time}\t{CastersCount}\t{TreeNodeCount}\t{OptimizeCount}\t{TotalVisible}\t{VisibleLeaf}\t{AvgVisible}\t{TreeDepth}\t{RayPointsCount}\t{RayPointsCountAll}\t{AvgRayPointsCount}";
        // string line2= $"{areaSize}\t{ObjectsCount}\t{CellSize}\t{JobsPerObject}\t{DirectionCount}\t{FastBake}\t{OptimizeTree}\t\t{Time}\t{CastersCount}\t{TreeNodeCount}\t{OptimizeCount}\t{TotalVisible}\t{VisibleLeaf}\t{AvgVisible}\t{TreeDepth}\t{RayPointsCount}\t{RayPointsCountAll}\t{AvgRayPointsCount}";
        // return line1+"\n"+line2;

        string line3= $"areaSize:{areaSize}\tObjectsCount:{ObjectsCount}\tCellSize:{CellSize}\tJobsPerObject:{JobsPerObject}\tDirectionCount:{DirectionCount}\tFastBake:{FastBake}\tOptimizeTree:{OptimizeTree}\nTime:{Time}\tCastersCount:{CastersCount}\tTreeNodeCount:{TreeNodeCount}\tOptimizeCount:{OptimizeCount}\tTotalVisible:{TotalVisible}\tVisibleLeaf:{VisibleLeaf}\tAvgVisible:{AvgVisible}\tTreeDepth:{TreeDepth}\t{RayPointsCount}\tRayPointsCountAll:{RayPointsCountAll}\tAvgRayPointsCount:{AvgRayPointsCount}";
        return line3;
    }
}
