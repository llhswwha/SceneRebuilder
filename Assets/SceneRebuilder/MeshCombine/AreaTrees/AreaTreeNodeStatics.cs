using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class AreaTreeNodeStatics
{
    public int CellCount = 0;

    public int AvgCellRendererCount = 0;

    public int MaxCellRendererCount = 0;

    public int MinCellRendererCount = 0;

    public int MaxNodeVertexCount = 0;

    public int MaxNodeVertexCount2 = 0;

    public int MinNodeVertexCount = 0;

    public int LevelDepth = 0;

    public void Clear()
    {
        CellCount = 0;
        AvgCellRendererCount = 0;
        MaxCellRendererCount = 0;
        MinCellRendererCount = 0;
        MaxNodeVertexCount = 0;
        LevelDepth = 0;
        MaxNodeVertexCount2 = 0;
    }

    public void SetInfo(AreaTreeNodeStatics other)
    {
        this.AvgCellRendererCount += other.AvgCellRendererCount;
        if (other.MaxCellRendererCount > this.MaxCellRendererCount || this.MaxCellRendererCount == 0)
            this.MaxCellRendererCount = other.MaxCellRendererCount;
        if (other.MinCellRendererCount < this.MinCellRendererCount || this.MinCellRendererCount==0)
            this.MinCellRendererCount = other.MinCellRendererCount;
        if (other.MaxNodeVertexCount > this.MaxNodeVertexCount || this.MaxNodeVertexCount == 0)
            this.MaxNodeVertexCount = other.MaxNodeVertexCount;
        if (other.MaxNodeVertexCount2 > this.MaxNodeVertexCount2 || this.MaxNodeVertexCount2 == 0)
            this.MaxNodeVertexCount2 = other.MaxNodeVertexCount2;
        if (other.MinNodeVertexCount < this.MinNodeVertexCount || this.MinNodeVertexCount == 0)
            this.MinNodeVertexCount = other.MinNodeVertexCount;
        if (other.LevelDepth > this.LevelDepth)
            this.LevelDepth = other.LevelDepth;
    }
}