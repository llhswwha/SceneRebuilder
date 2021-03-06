
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LODGroupDetails:IComparable<LODGroupDetails>
{
    private LODGroupInfo groupInfo;

    private void InitLODGroupInfo()
    {
        if (groupInfo == null)
        {
            groupInfo = group.GetComponent<LODGroupInfo>();

        }
        if (groupInfo == null)
        {
            groupInfo = group.gameObject.AddComponent<LODGroupInfo>();
            groupInfo.GetLODs();
        }
    }

    public string GetCaption()
    {
        InitLODGroupInfo();
        if (this.group.transform.parent != null)
        {
            return $"{this.group.transform.parent.name}->{this.group.name} [{MeshHelper.GetVertexCountS(childs[0].vertexCount)}][{groupInfo.IsSceneCreatable()}]";
        }
        else
        {
            return $"{this.group.name} [{MeshHelper.GetVertexCountS(childs[0].vertexCount)}][{groupInfo.IsSceneCreatable()}]";
        }
        
    }
    public int CompareTo(LODGroupDetails other)
    {
        //return other.group.gameObject.name.CompareTo(this.group.gameObject.name);
        //return this.group.gameObject.name.CompareTo(other.group.gameObject.name);
        return other.childs[0].vertexCount.CompareTo(this.childs[0].vertexCount);
    }

    public LODGroupData group;
    public List<LODChildInfo> childs;

    public LODValueInfo currentInfo;//当前所处level和百分比
    public LODChildInfo currentChild;//当前level对应的LOD[]
    //public GameObject childObj;
    public bool isCullOff;
    public int vertexCount = 0;

    public void UpdatePoint()
    {
        group.UpdatePoint();
    }

    public List<MeshRenderer> GetAllRenderers()
    {
        InitLODGroupInfo();
        return groupInfo.GetLODRenderers();
    }

    public List<MeshRenderer> GetLODRenderers(int i)
    {
        InitLODGroupInfo();
        return groupInfo.GetLODRenderers(i);
    }

    public LODGroupDetails(LODGroup groupT)
    {
        group = new LODGroupData(groupT);
        childs = new List<LODChildInfo>();
        if (group == null) return;
        LOD[] lods = group.GetLODs();
        foreach(var item in lods)
        {
            LODChildInfo child = new LODChildInfo();
            child.meshCount = item.renderers == null ? 0 : item.renderers.Length;
            child.vertexCount = CaculteVertex(item);
            child.renderers = item.renderers;
            child.screenRelativeTransitionHeight = item.screenRelativeTransitionHeight;
            childs.Add(child);
            vertexCount += child.vertexCount;
        }
    }
    /// <summary>
    /// 计算当前LOD所处等级
    /// </summary>
    /// <param name="viewType"></param>
    /// <param name="cam"></param>
    public LODChildInfo CaculateLODValueInfo(LODSceneView viewType,CameraData cam)
    {
        //if(viewType==LODSceneView.GameView)
        //{
        //    if (group == null || cam == null) return currentChild;
        //    currentInfo = LODUtility.GetVisibleLOD(group,cam);                
        //}else
        //{
        //    currentInfo = LODUtility.GetVisibleLODSceneView(group);
        //}

        if (group == null || cam == null) return currentChild;
        currentInfo = LODUtility.GetVisibleLOD(group, cam);

        if (childs != null && currentInfo.currentLevel < childs.Count)
        {
            currentChild = childs[currentInfo.currentLevel]; 
            //if(group!=null&&group.transform.childCount>currentInfo.currentLevel)
            //{
            //    childObj = group.transform.GetChild(currentInfo.currentLevel).gameObject;
            //}
            isCullOff = false;
        }
        else
        {
            currentChild = null;
            //childObj = null;
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
    public static List<LODGroupDetails> GetSceneLodGroupInfo(GameObject root,bool includeinactive=false)
    {
        // if (lodInfos != null) return;
        var lodInfos = new List<LODGroupDetails>();
        //LODGroup[] groups = null;
        //if (root == null)
        //{
        //    groups = GameObject.FindObjectsOfType<LODGroup>(includeinactive);
        //}
        //else
        //{
        //    groups = root.GetComponentsInChildren<LODGroup>(includeinactive);
        //}

        LODGroup[] groups = LODHelper.GetLODGroups(root, includeinactive);
        
        int i = 0;
        foreach(var item in groups)
        {
            i++;
            LODGroupDetails detail = new LODGroupDetails(item);
            lodInfos.Add(detail);
        }

        lodInfos.Sort();

        return lodInfos;
    }
    public static int[] CaculateGroupInfo(List<LODGroupDetails> lodInfos, LODSceneView viewType, LODSortType sortType, Camera cam)
    {
        CameraData camData = new CameraData(cam, viewType);
        return CaculateGroupInfo(lodInfos, viewType, sortType, camData);
    }

        /// <summary>
        /// 计算当前视角下，场景Lod信息
        /// </summary>
        public static int[] CaculateGroupInfo(List<LODGroupDetails> lodInfos,LODSceneView viewType,LODSortType sortType,CameraData cam)
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

public class CameraData
{
    public Camera camera;
    public TransformData transform;
    public bool orthographic;
    public float orthographicSize;
    public float fieldOfView;
    public float lodBias;
    public CameraData(Camera camera, LODSceneView viewType)
    {
        this.camera = camera ?? Camera.current;

        if (viewType == LODSceneView.SceneView)
        {
#if UNITY_EDITOR
            camera = UnityEditor.SceneView.lastActiveSceneView.camera;
#endif
        }

        //this.camera = camera;
        this.transform = new TransformData(camera.transform);

        this.orthographic = camera.orthographic;
        this.orthographicSize = camera.orthographicSize;
        this.fieldOfView = camera.fieldOfView;

        this.lodBias = QualitySettings.lodBias;
    }
}

public class LODGroupData
{
    public string name;

    public LODGroup group;

    public GameObject gameObject;

    public TransformData transform;

    public LOD[] lods;

    public float size = 0;

    public int lodCount;

    public Vector3 localReferencePoint;

    public Vector3 worldReferencePoint;//??

    public LODGroupData(LODGroup group)
    {
        this.name = group.name;
        this.group = group;
        this.gameObject = group.gameObject;
        this.transform = new TransformData(group.transform);

        this.lods = group.GetLODs();
        this.size = group.size;
        this.lodCount = group.lodCount;
        this.localReferencePoint=group.localReferencePoint;
        this.worldReferencePoint = group.transform.TransformPoint(group.localReferencePoint);
    }

    public void UpdatePoint()
    {
        //this.transform = new TransformData(group.transform);

        //this.localReferencePoint = group.localReferencePoint;
        //this.worldReferencePoint = group.transform.TransformPoint(group.localReferencePoint);
    }

    public LOD[] GetLODs()
    {
        return lods;
    }

    public T GetComponent<T>() where T :Component
    {
        return group.GetComponent<T>();
    }
}