using MeshJobs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFilterListDict
{
    public int Count
    {
        get
        {
            return list.Count;
        }
    }
    private List<MeshFilterList> list = new List<MeshFilterList>();

     //private List<MeshFilterList> listEx = new List<MeshFilterList>();//合并后的

     public bool IsCombinedByVertex=false;

     public int VertexCountOffset=0;

    public List<MeshFilterList> GetMeshFiltersList()
    {
        if(IsCombinedByVertex==false)
        {
            IsCombinedByVertex=true;
            int count1 = list.Count;
            //Debug.Log($"GetMeshFiltersList Start: {count1} VertexCountOffset:{VertexCountOffset}");
            list.Sort();
            for(int i=0;i<list.Count-1;i++)
            {
                var a=list[i];
                var b=list[i+1];
                if (b.vertexCount - a.vertexCount <= VertexCountOffset)//合并vertexCount相近的列表
                {
                    a.AddRang(b);
                    list.RemoveAt(i + 1);
                    i--;
                    Debug.Log($"GetMeshFiltersList mat:{a.MatId} a:{a.vertexCount} b:{b.vertexCount} d:{b.vertexCount - a.vertexCount} dis:{VertexCountOffset}");
                }
            }
            int count2 = list.Count;
            if (count1 != count2)
            {
                Debug.Log($"GetMeshFiltersList Start: {count1} VertexCountOffset:{VertexCountOffset} End: {list.Count} |Detail:{GetGroupCountDetails()} | CombineDistance:{VertexCountOffset}");
            }
            
        }
        return list;
    }
    
    public Dictionary<string, MeshFilterList> dict = new Dictionary<string, MeshFilterList>();
    // public void AddMeshFilter(int count,int matId,MeshFilter mf)
    // {
    //     if (!dict.ContainsKey(count))
    //     {
    //         MeshFilterList mflNew = new MeshFilterList();
    //         mflNew.vertexCount=count;

    //         // MeshRenderer renderer=mf.GetComponent<MeshRenderer>();
    //         // var matId=renderer.sharedMaterial.GetInstanceID();

    //         list.Add(mflNew);
    //         dict.Add(count, mflNew);
    //     }
    //     var mfl = dict[count];
    //     mfl.Add(mf);
    // }

    int vertexCountPower = 1;

    public MeshFilterListDict(MeshPoints[] meshPoints,int vertexCountOffset)
    {
        int[] meshCounts = new int[meshPoints.Length];
        for(int i=0;i<meshPoints.Length;i++)
        {
            var mf=meshPoints[i];
            //var vCount = mf.vertexCount;
            var vCount = Mathf.RoundToInt(mf.vertexCount/ vertexCountPower);
            // if(vCount>AcRTAlignJobContainer.MaxVertexCount)//排除点数特别多的，不然会卡住
            // {
            //     continue;
            // }
            var matId = mf.GetMatId();
            //string key=vCount+"_"+matId;
            string key = vCount + "_";
            string key2= mf.GetDictKey(key);
            if (key2 != key)
            {
                IsCombinedByVertex = true;//自定义Key的情况下就不需要考虑合并相同的顶点数了。
            }
            key = key2;
            if (!dict.ContainsKey(key)){
                //Debug.LogError("MeshFilterListDict key:"+key);

                MeshFilterList mflNew = new MeshFilterList();
                mflNew.vertexCount=vCount;
                mflNew.MatId=matId;
                mflNew.Key=key;

                // MeshRenderer renderer=mf.GetComponent<MeshRenderer>();
                // var matId=renderer.sharedMaterial.GetInstanceID();

                list.Add(mflNew);
                dict.Add(key, mflNew);
            }
            var mfl = dict[key];
            mfl.Add(mf);
        }

        Debug.Log($"MeshFilterListDict meshPoints:{meshPoints.Length} dict:{dict.Count} count:{dict.Count} vertexCountOffset:{vertexCountOffset}");

        // for (int i = 0; i < meshCounts.Length; i++)
        // {
        //     AddMeshFilter(meshCounts[i], meshFilters[i]);
        // }
        this.VertexCountOffset = vertexCountOffset;
        GetMeshFiltersList();
    }

    public void RemoveEmpty()
    {
        for (int i1 = 0; i1 < list.Count; i1++)
        {
            MeshFilterList item = list[i1];
            if (item.Count == 0)
            {
                list.RemoveAt(i1);
                i1--;
                dict.Remove(item.Key);
            }
        }
    }

    public int GetMeshFilterCount()
    {
        int sum = 0;
        for (int i1 = 0; i1 < list.Count; i1++)
        {
            MeshFilterList item = list[i1];
            sum += item.Count;
        }
        return sum;
    }

    public string GetGroupCountDetails()
    {
        string txt="";
        for (int i1 = 0; i1 < list.Count; i1++)
        {
            MeshFilterList item = list[i1];
            txt += item.vertexCount+"="+item.Count+";";
        }
        return txt;
    }

    internal int GetMaxJobCount()
    {
        int sum = 0;
        for (int i1 = 0; i1 < list.Count; i1++)
        {
            MeshFilterList item = list[i1];
            sum += item.Count*item.Count;
        }
        return sum;
    }

  internal void RemoveMeshFilter(MeshPoints meshFilter)
  {
    for (int i1 = 0; i1 < list.Count; i1++)
        {
            MeshFilterList item = list[i1];
            item.Remove(meshFilter);
        }
  }
}