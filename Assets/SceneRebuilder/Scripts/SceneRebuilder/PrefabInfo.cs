using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrefabInfo:IComparable<PrefabInfo>
{
    public PrefabInfo(GameObject prefab){
        this.Prefab=prefab;
    }

    public PrefabInfo(MeshFilter mf){
        this.MeshFilter=mf;
        this.Prefab=mf.gameObject;
        this.VertexCount=mf.sharedMesh.vertexCount;
        this.Size=mf.sharedMesh.bounds.size;
    }

    public GameObject Prefab;

    public GameObject PrefAsset;

    public MeshFilter MeshFilter;

    public List<GameObject> Instances=new List<GameObject>();

    public List<GameObject> GetInstances()
    {
        return Instances;
    }

    public void DestroyInstances()
    {
      foreach(var ins in Instances){
        GameObject.DestroyImmediate(ins);
      }
    }

    public int VertexCount=0;

    public float SizeVolumn
    {
      get{
        return Size.x*Size.y*Size.z;
      }
      
    }

    public Vector3 Size=Vector3.zero;

    public int _InstanceCount;

    public int InstanceCount
    {
        get
        {
            if(Instances==null)return 0;
            _InstanceCount=Instances.Count;
            return Instances.Count;
        }
    }

    public void Add(GameObject instance){
        Instances.Add(instance);
        //InstanceCount++;
    }

  public int CompareTo(PrefabInfo other)
  {
    return other.InstanceCount.CompareTo(this.InstanceCount);
  }
}

[Serializable]
public static class PrefabInfoListHelper
{
    public static int GetInstanceCount(this List<PrefabInfo> list)
    {
        int count = 0;
        for(int i=0;i< list.Count;i++)
        {
            count += list[i].InstanceCount+1;
        }
        return count;
    }

    public static List<MeshRenderer> GetRenderers(this List<PrefabInfo> list)
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        for (int i = 0; i < list.Count; i++)
        {
            renderers.Add(list[i].Prefab.GetComponent<MeshRenderer>());
            var insList = list[i].GetInstances();
            if(insList==null)continue;
            for (int j=0;j< insList.Count; j++)
            {
                renderers.Add(insList[j].GetComponent<MeshRenderer>());
            }
        }
        return renderers;
    }

    public static int RemoveInstances(this List<PrefabInfo> list)
    {
        int count = 0;
        for (int i = 0; i < list.Count; i++)
        {
            var insList = list[i].GetInstances();
            for (int j=0;j< insList.Count; j++)
            {
                GameObject.DestroyImmediate(insList[j]);
            }
            GameObject.DestroyImmediate(list[i].Prefab);
        }
        return count;
    }


}
