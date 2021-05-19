using AdvancedCullingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class CullingSetting
{
    //float _cellSize = 3f;
    //int _jobsPerObject = 20;
    //bool _fastBake;

    //int _directionCount = 2000;//cww_add

    //bool _isOptimaizeTree = true;//cww_add
    //bool _showCells = false;

    [SerializeField]
    public float CellSize = 3f;
    [SerializeField]
    public int JobsPerObject = 20;

    public int GetStep(int rayPointsCount)
    {
        int step = JobsPerObject >= rayPointsCount ? 1 : Mathf.RoundToInt((float)rayPointsCount / JobsPerObject);
        return step;
    }

    [SerializeField]
    public bool FastBake=true;
    [SerializeField]
    public int DirectionCount=2000;//cww_add
    [SerializeField]
    public bool IsOptimaizeTree=true;//cww_add

    [SerializeField]
    public bool _showCells;


    [SerializeField]
    public int CastersCount;
    [SerializeField]
    public int TreeNodeCount;
    [SerializeField]
    public float BakingTime;
    [SerializeField]
    public int layer;

    public CullingSetting()
    {

    }

    public CullingSetting(float cellSize, int jobsPerObject, bool fastBake)
    {
        this.CellSize = cellSize;
        this.JobsPerObject = jobsPerObject;
        this.FastBake = fastBake;
        this.DirectionCount = 2000;
        this.IsOptimaizeTree = true;
        this._showCells = false;
        this.CastersCount = 0;
        this.BakingTime = 0;
        this.layer = ACSInfo.CullingLayer;
    }

    public override string ToString()
    {
        return $"_castersCount:{CastersCount}\t_treeNodeCount:{TreeNodeCount}\t_bakingTime:{BakingTime}\n_cellSize:{CellSize}\t_jobsPerObject:{JobsPerObject}\t_fastBake:{FastBake}\t_sphereCount:{DirectionCount}\t_isOptimaizeTree:{IsOptimaizeTree}";
    }
}
