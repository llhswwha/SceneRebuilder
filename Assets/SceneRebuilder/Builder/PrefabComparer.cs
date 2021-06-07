using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PrefabComparer : MonoBehaviour
{
    public GameObject Prefab;

    public GameObject Target;

    public int TryRotateAngleCount=20;

    #if UNITY_EDITOR

    public bool IsUsePrefab=false;

    public Vector3 centerOffset=Vector3.zero;

    private GameObject GetPrefabObject()
    {
        GameObject newGo=Prefab;

        if(IsUsePrefab==false){ //只计算一次，快很多很多
            newGo=PrefabUtility.InstantiatePrefab(Prefab) as GameObject;
            if (newGo == null)
            {
                newGo = MeshHelper.CopyGO(Prefab);
            }
            MeshNode node1=newGo.GetComponent<MeshNode>();
            if(node1==null){
                node1=newGo.AddComponent<MeshNode>();
            }
            node1.GetVertexCenterInfo(false,false,centerOffset);
        }

        return newGo;
    }

    [ContextMenu("ReplaceToPrefab")]
    public void ReplaceToPrefab()
    {
        GameObject newGo=GetPrefabObject();

        List<GameObject> models=GetTargetModels();
        ReplaceToPrefab_Core(newGo,models);
        if(IsUsePrefab==false){ //只计算一次，快很多很多
            GameObject.DestroyImmediate(newGo);
        }
        ProgressBarHelper.ClearProgressBar();
    }

    private List<GameObject> GetTargetModels()
    {
        List<GameObject> models=new List<GameObject>();
        MeshNode[] nodes=Target.GetComponentsInChildren<MeshNode>();
        foreach(var node in nodes)
        {
            node.ClearChildren();
        }

        MeshFilter[] meshFilters=Target.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < meshFilters.Length; i++)
        {
            // float progress = (float)i / meshFilters.Length;
            // float percents = progress * 100;
            
            // if(ProgressBarHelper.DisplayCancelableProgressBar("ReplaceToPrefab", $"{i}/{meshFilters.Length} {percents}% of 100%", progress))
            // {
            //     //ProgressBarHelper.ClearProgressBar();
            //     break;
            // }
            MeshFilter mf = meshFilters[i];
            if(mf==null)
            {
                continue;
            }
            //ReplaceToPrefab(newGo,mf.gameObject);
            models.Add(mf.gameObject);
        }
        return models;
    }

    public List<GameObject> ReplaceToPrefab_Core(GameObject prefabGo,List<GameObject> models)
    {
        DateTime start=DateTime.Now;
        List<GameObject> newModels=new List<GameObject>();
        for (int i = 0; i < models.Count; i++)
        {
            float progress = (float)i /models.Count;
            float percents = progress * 100;
            
            if(ProgressBarHelper.DisplayCancelableProgressBar("ReplaceToPrefab_Core", $"{i}/{models.Count} {percents}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
            GameObject go = models[i];
            if(go==null)
            {
                continue;
            }

            GameObject newGo=null;
            if(IsUsePrefab){
                newGo=PrefabUtility.InstantiatePrefab(Prefab) as GameObject;
            }
            else //只计算一次，快很多很多
            {
                newGo=MeshHelper.CopyGO(prefabGo);
            }
            var r=MeshHelper.ReplaceToPrefab(newGo,go,TryRotateAngleCount,centerOffset,false);
            if(r==false)
            {
                newModels.Add(go);
            }
        }

        ProgressBarHelper.ClearProgressBar();

        Debug.LogWarning($"ReplaceToPrefab_Core Count:{models.Count},Time:{(DateTime.Now-start).ToString()}");
        return newModels;
    }
    #endif

    // public static bool AlignMeshNode(MeshNode node1,MeshNode node2,int tryCount)
    // {
    //     DateTime start=DateTime.Now;
    //     //Debug.Log("AlignMeshNode Start");
    //     Vector3 longLine1 = node1.GetLongLine();
    //     Vector3 longLine2 = node2.GetLongLine();

    //     Quaternion qua1 = Quaternion.FromToRotation(node2.GetLongLine(), node1.GetLongLine());
    //     node2.transform.rotation = qua1 * node2.transform.rotation;

    //     //Quaternion qua2 = Quaternion.FromToRotation(node2.GetShortLine(), node1.GetShortLine());
    //     //node2.transform.rotation = qua2 * node2.transform.rotation;

    //     //Quaternion qua3 = Quaternion.FromToRotation(node2.GetLongLine(), node1.GetLongLine());
    //     //node2.transform.rotation = qua3 * node2.transform.rotation;

    //     //Quaternion qua4 = Quaternion.FromToRotation(node2.GetShortLine(), node1.GetShortLine());
    //     //node2.transform.rotation = qua4 * node2.transform.rotation;

    //     Vector3 offset = node1.GetCenterP() - node2.GetCenterP();
    //     node2.transform.position += offset;


    //     int j=0;
    //     float angle=Vector3.Angle(node2.GetShortLine(), node1.GetShortLine());
    //     for(;j<tryCount;j++)
    //     {
    //         //Debug.Log($"[{j}]angle:{angle}");
    //         node2.transform.RotateAround(node1.GetCenterP(), node2.GetLongLine(), -angle);
    //         angle = Vector3.Angle(node2.GetShortLine(), node1.GetShortLine());
    //         if (angle <= 0) {
    //             Debug.Log($"[{j}]angle:{angle}");
    //             break;
    //         }
    //     }
    //     if(j>=tryCount)
    //     {
    //         Debug.LogError($"[j>=tryCount]tryCount:{tryCount},angle:{angle},Node1:{node1.name},Node2:{node2.name}");
    //         return false;
    //     }
    //     Debug.Log($"AlignMeshNode Time:{(DateTime.Now-start).TotalMilliseconds}ms");
    //     return true;
    // }
}
