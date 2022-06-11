//using MeshJobs;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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

     public int CombineVertexCountOffset=0;

    public float CombineVertexPercentOffset = 0.05f;

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
                MeshFilterList a =list[i];
                for(int j = i+1; j < list.Count; j++)
                {
                    MeshFilterList b = list[j];
                    int offVertex = Mathf.Abs(b.vertexCount - a.vertexCount);
                    float percent1 = (float)offVertex / (float)b.vertexCount;
                    float percent2 = (float)offVertex / (float)a.vertexCount;
                    if (DistanceSetting.IsByMat)
                    {
                        if (b.MatId == a.MatId)
                        {
                            if (offVertex <= CombineVertexCountOffset || percent1 < CombineVertexPercentOffset)//合并vertexCount相近的列表
                            {
                                if (DistanceSetting.IsShowLog)
                                {
                                    Debug.LogWarning($"GetMeshFiltersList[{i},{j}][list:{list.Count}] a:[mat={a.MatId} vertexCount={a.vertexCount}] b:[mat={b.MatId} vertexCount={b.vertexCount}] d:{b.vertexCount - a.vertexCount} percent1:{percent1} percent2:{percent2} [DisOffset:{CombineVertexCountOffset} PercentOffset:{CombineVertexPercentOffset}]");
                                }
                                a.AddRang(b);
                                a.vertexCount = (b.vertexCount * b.Count + a.vertexCount * a.Count) / (a.Count + b.Count);
                                list.RemoveAt(j);
                                j--;
                                //break;
                            }
                        }
                    }
                    else
                    {
                        if (offVertex <= CombineVertexCountOffset || percent1 < CombineVertexPercentOffset)//合并vertexCount相近的列表
                        {
                            if (DistanceSetting.IsShowLog)
                            {
                                Debug.LogWarning($"GetMeshFiltersList[{i},{j}] a:[mat={a.MatId} vertexCount={a.vertexCount}] b:[mat={b.MatId} vertexCount={b.vertexCount}] d:{b.vertexCount - a.vertexCount} percent1:{percent1} percent2:{percent2} [DisOffset:{CombineVertexCountOffset} PercentOffset:{CombineVertexPercentOffset}]");
                            }
                            a.AddRang(b);
                            a.vertexCount = (b.vertexCount * b.Count + a.vertexCount * a.Count) / (a.Count + b.Count);
                            list.RemoveAt(j);
                            j--;
                        }
                    }

                    if (DistanceSetting.IsShowLog)
                    {
                        Debug.Log($"GetMeshFiltersList[{i},{j}] a:[mat={a.MatId} vertexCount={a.vertexCount}] b:[mat={b.MatId} vertexCount={b.vertexCount}] d:{b.vertexCount - a.vertexCount} percent1:{percent1} percent2:{percent2} [DisOffset:{CombineVertexCountOffset} PercentOffset:{CombineVertexPercentOffset}]");
                    }
                }
                
            }
            int count2 = list.Count;
            if (count1 != count2)
            {
                Debug.Log($"GetMeshFiltersList Start: {count1} > End: {count2} VertexCountOffset:{CombineVertexCountOffset} CombineVertexPercentOffset:{CombineVertexPercentOffset}  |Detail:{GetGroupCountDetails()} | CombineDistance:{CombineVertexCountOffset}");
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

    public string GetDictKey(MeshPoints mf,string defaultKey)
    {
        PipeModelComponent pipeModel = mf.GetComponent<PipeModelComponent>();
        if (pipeModel == null)
        {
            PipeMeshGeneratorBase generator = mf.GetComponent<PipeMeshGeneratorBase>();
            if (generator != null)
            {
                if (generator.Target == null)
                {
                    Debug.LogError($"GetDictKey name:{mf.name} generator.Target == null");
                }
                else
                {
                    pipeModel = generator.Target.GetComponent<PipeModelComponent>();
                }
            }
        }

        if (pipeModel != null)
        {
            string key2 = pipeModel.GetDictKey();
            if (!string.IsNullOrEmpty(key2))
            {
                defaultKey = key2;
            }
        }
        else
        {
            //MeshRendererInfo meshRendererInfo = MeshRendererInfo.GetMinMax(this.gameObject);
        }

        return defaultKey;
    }

    public MeshFilterListDict(MeshPoints[] meshPoints,int vertexCountOffset)
    {
        int[] meshCounts = new int[meshPoints.Length];
        StringBuilder sb = new StringBuilder();
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
            if (DistanceSetting.IsByMat)
            {
                key = vCount + "_" + matId;
            }
            string key2= GetDictKey(mf,key);
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
                sb.Append(key + ";");
            }
            var mfl = dict[key];
            mfl.Add(mf);
        }

        Debug.Log($"MeshFilterListDict meshPoints:{meshPoints.Length} dict:{dict.Count} count:{dict.Count} vertexCountOffset:{vertexCountOffset} keys:{sb.ToString()}");

        // for (int i = 0; i < meshCounts.Length; i++)
        // {
        //     AddMeshFilter(meshCounts[i], meshFilters[i]);
        // }
        this.CombineVertexCountOffset = vertexCountOffset;
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

    public int GetMaxJobCount()
    {
        int sum = 0;
        for (int i1 = 0; i1 < list.Count; i1++)
        {
            MeshFilterList item = list[i1];
            sum += item.Count*item.Count;
        }
        return sum;
    }

    public void RemoveMeshFilter(MeshPoints meshFilter)
  {
    for (int i1 = 0; i1 < list.Count; i1++)
        {
            MeshFilterList item = list[i1];
            item.Remove(meshFilter);
        }
  }
}