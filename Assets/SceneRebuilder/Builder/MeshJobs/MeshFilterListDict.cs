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

     public bool IsConbined=false;

     public static int CombineDistance=3;

    public List<MeshFilterList> GetMeshFiltersList()
    {
        if(IsConbined==false)
        {
            IsConbined=true;
            Debug.LogError("GetMeshFiltersList1:"+list.Count);
            list.Sort();
            for(int i=0;i<list.Count-1;i++)
            {
                var a=list[i];
                var b=list[i+1];
                if(a.MatId==b.MatId && b.vertexCount-a.vertexCount<=CombineDistance)//合并vertexCount相近的列表
                {
                    a.AddRang(b);
                    list.RemoveAt(i+1);
                    i--;
                }
            }
            Debug.LogError("GetMeshFiltersList2:"+list.Count);

            Debug.Log("Detail:"+GetGroupCountDetails());
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

    public MeshFilterListDict(MeshFilter[] meshFilters)
    {
        int[] meshCounts = new int[meshFilters.Length];
        for(int i=0;i<meshFilters.Length;i++)
        {
            var mf=meshFilters[i];
            var vCount = mf.sharedMesh.vertexCount;

            // if(vCount>AcRTAlignJobContainer.MaxVertexCount)//排除点数特别多的，不然会卡住
            // {
            //     continue;
            // }

            MeshRenderer renderer=mf.GetComponent<MeshRenderer>();
            Color color=renderer.sharedMaterial.color;
            //var matId=renderer.sharedMaterial.GetInstanceID();
            var matId=color.ToString();

            string key=vCount+"_"+matId;
            if(!dict.ContainsKey(key)){
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

        Debug.LogError("MeshFilterListDict:"+dict.Count);

        // for (int i = 0; i < meshCounts.Length; i++)
        // {
        //     AddMeshFilter(meshCounts[i], meshFilters[i]);
        // }
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

  internal void RemoveMeshFilter(MeshFilter meshFilter)
  {
    for (int i1 = 0; i1 < list.Count; i1++)
        {
            MeshFilterList item = list[i1];
            item.Remove(meshFilter);
        }
  }
}