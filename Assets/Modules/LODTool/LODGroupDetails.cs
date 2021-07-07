
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODGroupDetails
{
    public LODGroup group;
    public List<LODChildInfo> childs;

    public LODValueInfo currentInfo;//当前所处level和百分比
    public LODChildInfo currentChild;//当前level对应的LOD[]
    public GameObject childObj;
    public bool isCullOff;

    public LODGroupDetails(LODGroup groupT)
    {
        group = groupT;
        childs = new List<LODChildInfo>();
        if (group == null) return;
        LOD[] lods = group.GetLODs();
        foreach(var item in lods)
        {
            LODChildInfo child = new LODChildInfo();
            child.meshCount = item.renderers == null ? 0 : item.renderers.Length;
            child.vertexCount = CaculteVertex(item);
            childs.Add(child);
        }
    }
    /// <summary>
    /// 计算当前LOD所处等级
    /// </summary>
    /// <param name="viewType"></param>
    /// <param name="cam"></param>
    public LODChildInfo CaculateLODValueInfo(LODSceneView viewType,Camera cam=null)
    {
        if(viewType==LODSceneView.GameView)
        {
            if (group == null || cam == null) return currentChild;
            currentInfo = LODUtility.GetVisibleLOD(group,cam);                
        }else
        {
            currentInfo = LODUtility.GetVisibleLODSceneView(group);
        }
        if (childs != null && currentInfo.currentLevel < childs.Count)
        {
            currentChild = childs[currentInfo.currentLevel]; 
            if(group!=null&&group.transform.childCount>currentInfo.currentLevel)
            {
                childObj = group.transform.GetChild(currentInfo.currentLevel).gameObject;
            }
            isCullOff = false;
        }
        else
        {
            currentChild = null;
            childObj = null;
            isCullOff = true;
        }
        return currentChild;
    }
    /// <summary>
    /// 顶点占比
    /// </summary>
    /// <returns></returns>
    public string GetVertexString()
    {
        if (currentChild == null) return "0";
        else
        {
            float percent = (float)Math.Round(currentChild.vertexPercent * 100, 1);
            return string.Format("{0:F2}({1}%)", currentChild.GetVertexCountF(), percent);
        }
    }
    /// <summary>
    /// mesh个数占比
    /// </summary>
    /// <returns></returns>
    public string GetMeshString()
    {
        if (currentChild == null) return "0";
        else
        {
            float percent = (float)Math.Round(currentChild.meshPercent * 100, 1);
            return string.Format("{0}个({1}%)", currentChild.meshCount, percent);
        }
    }
    private int CaculteVertex(LOD lodInfo)
    {
        if (lodInfo.renderers == null) return 0;
        else
        {
            int vertexCount = 0;
            foreach(var item in lodInfo.renderers)
            {
                if(item==null)continue;
                MeshFilter fliter= item.transform.GetComponent<MeshFilter>();
                if (fliter) vertexCount += fliter.sharedMesh == null ? 0 : fliter.sharedMesh.vertexCount;
            }
            return vertexCount;
        }
    }

    /// <summary>
    /// 查找当前场景下，所有的LodGroup信息
    /// </summary>
    public static List<LODGroupDetails> GetSceneLodGroupInfo()
    {
        // if (lodInfos != null) return;
        var lodInfos = new List<LODGroupDetails>();
        LODGroup[] groups = GameObject.FindObjectsOfType<LODGroup>(true);
        int i = 0;
        foreach(var item in groups)
        {
            i++;
            LODGroupDetails detail = new LODGroupDetails(item);
            lodInfos.Add(detail);
        }
        return lodInfos;
    }

    /// <summary>
    /// 计算当前视角下，场景Lod信息
    /// </summary>
    public static int[] CaculateGroupInfo(List<LODGroupDetails> lodInfos,LODSceneView viewType,LODSortType sortType,Camera cam=null)
    {
        DateTime now = DateTime.Now;
        int allVertexCount = 0;
        int allMeshCount = 0;
        if (lodInfos == null) return new int[2]{allVertexCount,allMeshCount};
        // Camera cam = gameCamera.GetComponent<Camera>();
        //1.计算当前所有LODGroup的level层级，并统计顶点和mesh的总数
        foreach (var item in lodInfos)
        {            
            LODChildInfo info = item.CaculateLODValueInfo(viewType, cam);
            if(info!=null)
            {
                allVertexCount += info.vertexCount;
                allMeshCount += info.meshCount;
            }
        }
        //计算子物体所占百分比
        foreach(var item in lodInfos)
        {
            LODChildInfo child = item.currentChild;
            if (child == null) continue;//culloff层级时，是没有lod信息的
            child.vertexPercent = allVertexCount == 0 ? 0 : (float)child.vertexCount / allVertexCount;
            child.meshPercent = allMeshCount == 0 ? 0 : (float)child.meshCount / allMeshCount;
            //Debug.LogError("v:"+child.vertexPercent+" m:"+child.meshPercent+" "+child.vertexCount+" "+allVertexCount);
        }      
        lodInfos.Sort((a,b)=> 
        {
            if(sortType==LODSortType.Mesh)
            {
                int aMesh = a.currentChild == null ? 0 : a.currentChild.meshCount;
                int bMesh = b.currentChild == null ? 0 : b.currentChild.meshCount;
                return bMesh.CompareTo(aMesh);
            }
            else
            {
                int aVetex = a.currentChild == null ? 0 : a.currentChild.vertexCount;
                int bVetex = b.currentChild == null ? 0 : b.currentChild.vertexCount;
                return bVetex.CompareTo(aVetex);
            }
        });
        //isCaculate = false;
        //Debug.Log($"CaculateGroupInfo完成，耗时{(DateTime.Now-now).TotalSeconds.ToString("f1")}s allVertexCount:{allVertexCount/10000f:F1}, allMeshCount:{allMeshCount}");
        return new int[2]{allVertexCount,allMeshCount};
    }
}