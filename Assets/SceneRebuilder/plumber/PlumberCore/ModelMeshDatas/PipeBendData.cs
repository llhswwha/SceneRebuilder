using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PipeBendData
{
    //public NativeArray<Vector3> StartPoints;

    //public PipeLineData Lines;

    public PipeLineData Line1;

    public PipeLineData Line2;

    public PipeLineData Line3;

    public PipeLineData Line4;

    public PipeLineData Line5;

    public PipeLineData Line6;

    public PipeLineData Line7;

    public PipeLineData Line8;

    public int Count;

    public void Set(int id, PipeLineData line)
    {
        if (id + 1 > Count)
        {
            Count = id + 1;
        }
        if (id == 0)
        {
            Line1 = line;
        }
        else if (id == 1)
        {
            Line2 = line;
        }
        else if (id == 2)
        {
            Line3 = line;
        }
        else if (id == 3)
        {
            Line4 = line;
        }
        else if (id == 4)
        {
            Line5 = line;
        }
        else if (id == 5)
        {
            Line6 = line;
        }
        else if (id == 6)
        {
            Line7 = line;
        }
        else if (id == 7)
        {
            Line8 = line;
        }
        else
        {
            Debug.LogError($"PipeBendData.Set Error id:{id} data:{line} ");
        }
    }

    public PipeLineData Get(int id)
    {
        if (id == 0)
        {
            return Line1;
        }
        else if (id == 1)
        {
            return Line2;
        }
        else if (id == 2)
        {
            return Line3;
        }
        else if (id == 3)
        {
            return Line4;
        }
        else if (id == 4)
        {
            return Line5;
        }
        else if (id == 5)
        {
            return Line6;
        }
        else if (id == 6)
        {
            return Line7;
        }
        else if (id == 7)
        {
            return Line8;
        }
        else
        {
            Debug.LogError($"PipeBendData.Get Error id:{id}");
            return new PipeLineData();
        }
    }

    //public override string ToString()
    //{
    //    return $"Bend[{Lines.Length}]";
    //}

    public override string ToString()
    {
        string s = $"Bend[{Count}]";
        for (int i = 0; i < Count; i++)
        {
            s += Get(i).ToString() + ";";
        }
        return s;
    }
}

