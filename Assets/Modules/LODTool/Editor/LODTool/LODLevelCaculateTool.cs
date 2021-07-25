using LODExtended;
using LODLevelCaculate;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LODLevelCaculate
{
    // public class LODGroupDetails
    // {
    //     public LODGroup group;
    //     public List<LODChildInfo> childs;

    //     public LODValueInfo currentInfo;//当前所处level和百分比
    //     public LODChildInfo currentChild;//当前level对应的LOD[]
    //     public GameObject childObj;
    //     public bool isCullOff;

    //     public LODGroupDetails(LODGroup groupT)
    //     {
    //         group = groupT;
    //         childs = new List<LODChildInfo>();
    //         if (group == null) return;
    //         LOD[] lods = group.GetLODs();
    //         foreach(var item in lods)
    //         {
    //             LODChildInfo child = new LODChildInfo();
    //             child.meshCount = item.renderers == null ? 0 : item.renderers.Length;
    //             child.vertexCount = CaculteVertex(item);
    //             childs.Add(child);
    //         }
    //     }
    //     /// <summary>
    //     /// 计算当前LOD所处等级
    //     /// </summary>
    //     /// <param name="viewType"></param>
    //     /// <param name="cam"></param>
    //     public LODChildInfo CaculateLODValueInfo(LODSceneView viewType,Camera cam=null)
    //     {
    //         if(viewType==LODSceneView.GameView)
    //         {
    //             if (group == null || cam == null) return currentChild;
    //             currentInfo = LODExtendedUtility.GetVisibleLOD(group,cam);                
    //         }else
    //         {
    //             currentInfo = LODExtendedUtility.GetVisibleLODSceneView(group);
    //         }
    //         if (childs != null && currentInfo.currentLevel < childs.Count)
    //         {
    //             currentChild = childs[currentInfo.currentLevel]; 
    //             if(group!=null&&group.transform.childCount>currentInfo.currentLevel)
    //             {
    //                 childObj = group.transform.GetChild(currentInfo.currentLevel).gameObject;
    //             }
    //             isCullOff = false;
    //         }
    //         else
    //         {
    //             currentChild = null;
    //             childObj = null;
    //             isCullOff = true;
    //         }
    //         return currentChild;
    //     }
    //     /// <summary>
    //     /// 顶点占比
    //     /// </summary>
    //     /// <returns></returns>
    //     public string GetVertexString()
    //     {
    //         if (currentChild == null) return "0";
    //         else
    //         {
    //             float percent = (float)Math.Round(currentChild.vertexPercent * 100, 1);
    //             return string.Format("{0:F2}({1}%)", currentChild.GetVertexCountF(), percent);
    //         }
    //     }
    //     /// <summary>
    //     /// mesh个数占比
    //     /// </summary>
    //     /// <returns></returns>
    //     public string GetMeshString()
    //     {
    //         if (currentChild == null) return "0";
    //         else
    //         {
    //             float percent = (float)Math.Round(currentChild.meshPercent * 100, 1);
    //             return string.Format("{0}个({1}%)", currentChild.meshCount, percent);
    //         }
    //     }
    //     private int CaculteVertex(LOD lodInfo)
    //     {
    //         if (lodInfo.renderers == null) return 0;
    //         else
    //         {
    //             int vertexCount = 0;
    //             foreach(var item in lodInfo.renderers)
    //             {
    //                 if(item==null)continue;
    //                 MeshFilter fliter= item.transform.GetComponent<MeshFilter>();
    //                 if (fliter) vertexCount += fliter.sharedMesh == null ? 0 : fliter.sharedMesh.vertexCount;
    //             }
    //             return vertexCount;
    //         }
    //     }

    //     /// <summary>
    //     /// 查找当前场景下，所有的LodGroup信息
    //     /// </summary>
    //     public static List<LODGroupDetails> GetSceneLodGroupInfo()
    //     {
    //         // if (lodInfos != null) return;
    //         var lodInfos = new List<LODGroupDetails>();
    //         LODGroup[] groups = GameObject.FindObjectsOfType<LODGroup>(true);
    //         int i = 0;
    //         foreach(var item in groups)
    //         {
    //             i++;
    //             LODGroupDetails detail = new LODGroupDetails(item);
    //             lodInfos.Add(detail);
    //         }
    //         return lodInfos;
    //     }

    //     /// <summary>
    //     /// 计算当前视角下，场景Lod信息
    //     /// </summary>
    //     public static int[] CaculateGroupInfo(List<LODGroupDetails> lodInfos,LODSceneView viewType,LODSortType sortType,Camera cam=null)
    //     {
    //         DateTime now = DateTime.Now;
    //         int allVertexCount = 0;
    //         int allMeshCount = 0;
    //         if (lodInfos == null) return new int[2]{allVertexCount,allMeshCount};
    //         // Camera cam = gameCamera.GetComponent<Camera>();
    //         //1.计算当前所有LODGroup的level层级，并统计顶点和mesh的总数
    //         foreach (var item in lodInfos)
    //         {            
    //             LODChildInfo info = item.CaculateLODValueInfo(viewType, cam);
    //             if(info!=null)
    //             {
    //                 allVertexCount += info.vertexCount;
    //                 allMeshCount += info.meshCount;
    //             }
    //         }
    //         //计算子物体所占百分比
    //         foreach(var item in lodInfos)
    //         {
    //             LODChildInfo child = item.currentChild;
    //             if (child == null) continue;//culloff层级时，是没有lod信息的
    //             child.vertexPercent = allVertexCount == 0 ? 0 : (float)child.vertexCount / allVertexCount;
    //             child.meshPercent = allMeshCount == 0 ? 0 : (float)child.meshCount / allMeshCount;
    //             //Debug.LogError("v:"+child.vertexPercent+" m:"+child.meshPercent+" "+child.vertexCount+" "+allVertexCount);
    //         }      
    //         lodInfos.Sort((a,b)=> 
    //         {
    //             if(sortType==LODSortType.Mesh)
    //             {
    //                 int aMesh = a.currentChild == null ? 0 : a.currentChild.meshCount;
    //                 int bMesh = b.currentChild == null ? 0 : b.currentChild.meshCount;
    //                 return bMesh.CompareTo(aMesh);
    //             }
    //             else
    //             {
    //                 int aVetex = a.currentChild == null ? 0 : a.currentChild.vertexCount;
    //                 int bVetex = b.currentChild == null ? 0 : b.currentChild.vertexCount;
    //                 return bVetex.CompareTo(aVetex);
    //             }
    //         });
    //         //isCaculate = false;
    //         Debug.Log($"CaculateGroupInfo完成，耗时{(DateTime.Now-now).TotalSeconds.ToString("f1")}s allVertexCount:{allVertexCount/10000f:F1}, allMeshCount:{allMeshCount}");
    //         return new int[2]{allVertexCount,allMeshCount};
    //     }
    // }
}

public class LODLevelCaculateTool : EditorWindow
{
    #region Private Property
    private static Rect windowRect = new Rect(0,0,800,500);
    static LODSceneView sceneView;
    //static bool limitInParent;//是否限制在父物体下查询
    //static GameObject parentObj;

    static Vector2 scrollPos;
    static bool isSearching;

    static GameObject gameCamera;
    static float caculateTimeSpan=0;
    static DateTime currentTime;

    static LODSortType sortType;
    #endregion

    #region UI
    [MenuItem("Tools/Model/LodLevelCaculateWindow")]
    static void OpenWindow()
    {
        EditorWindow.GetWindow(typeof(LODLevelCaculateTool));
        currentTime = DateTime.Now;
        if (Application.isPlaying) sceneView = LODSceneView.GameView;
        else sceneView = LODSceneView.SceneView;
    }

    private void OnGUI()
    {
        GUILayout.Label("GameView:程序运行时统计信息  SceneView:Scene视图下统计信息", GUILayout.Width(400));
        sceneView = (LODSceneView)EditorGUILayout.EnumPopup(sceneView, GUILayout.Width(300));
        if (gameCamera == null) gameCamera = Camera.main.gameObject;
        gameCamera = EditorGUILayout.ObjectField("Camera", gameCamera, typeof(GameObject), true, GUILayout.Width(500))as GameObject;
        caculateTimeSpan = EditorGUILayout.FloatField("刷新间隔时间（秒）",caculateTimeSpan,GUILayout.Width(300));

        sortType = (LODSortType)EditorGUILayout.EnumPopup(sortType, GUILayout.Width(300));

        bool isRefresh = false;
        if(GUILayout.Button("刷新信息", GUILayout.Width(300)))
        {
            isRefresh = true;
            isSearching = true;
        }
        DrawTitleView();
        DrawDetailView(isRefresh);     
    }
    /// <summary>
    /// 绘制标题
    /// </summary>
    private void DrawTitleView()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("序号", GUILayout.Width(60));
        GUILayout.Label("物体", GUILayout.Width(200));
        GUILayout.Label("Active", GUILayout.Width(60));        
        GUILayout.Label("名称", GUILayout.Width(150));
        GUILayout.Label("当前LOD层级名称", GUILayout.Width(150));
        GUILayout.Label("当前LOD层级Index", GUILayout.Width(150));
        GUILayout.Label("Vertex占比", GUILayout.Width(150));
        GUILayout.Label("Mesh个数占比", GUILayout.Width(150));
        GUILayout.EndHorizontal();
    }

    private void DrawDetailView(bool isRefresh)
    {        
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(250));
        if(isSearching)
        {
            GetSceneLodGroupInfo();
            if (isRefresh || (caculateTimeSpan>0&&(DateTime.Now - currentTime).TotalSeconds > caculateTimeSpan))
            {
                currentTime = DateTime.Now;
                CaculateGroupInfo();
            }               
            if (lodInfos != null)
            {
                SetAllMeshCount();
                for (int i = 0; i < lodInfos.Count; i++)
                {
                    LODGroupDetails detai = lodInfos[i];
                    GUILayout.BeginHorizontal();
                    int index = i + 1;
                    GUILayout.Label(index.ToString(), GUILayout.Width(60));
                    GameObject obj = EditorGUILayout.ObjectField("", detai.group==null?null:detai.group.gameObject, typeof(GameObject), true, GUILayout.Width(200)) as GameObject;
                    bool value = GUILayout.Toggle( obj==null?false:obj.activeSelf,"", GUILayout.Width(60));
                    if (obj != null && obj.activeSelf != value) obj.SetActive(value);
                    GUILayout.Label(obj==null?"":obj.name, GUILayout.Width(150));
                    GUILayout.Label(detai.childObj==null?"CullOff":detai.childObj.name, GUILayout.Width(150));
                    GUILayout.Label(detai.currentInfo==null?"empty":detai.currentInfo.currentLevel.ToString(), GUILayout.Width(150));
                    GUILayout.Label(detai.GetVertexString(), GUILayout.Width(150));
                    GUILayout.Label(detai.GetMeshString(), GUILayout.Width(150));
                    GUILayout.EndHorizontal();
                }
            }
        }
        GUILayout.EndScrollView();
    }
    private void SetAllMeshCount()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Width(60));
        GUILayout.Label("", GUILayout.Width(200));
        GUILayout.Label("", GUILayout.Width(60));
        GUILayout.Label("", GUILayout.Width(150));
        GUILayout.Label("", GUILayout.Width(250));
        // GUILayout.Label("TotalVertex:"+allVertexCount, GUILayout.Width(150));
        GUILayout.Label($"TotalVertex:{allVertexCount/10000f:F2}", GUILayout.Width(150));
        GUILayout.Label("TotalMeshCount:"+allMeshCount, GUILayout.Width(150));
        GUILayout.EndHorizontal();
    }
    private void OnDestroy()
    {
        isSearching = false;
        lodInfos = null;
        isCaculate = false;
        allVertexCount = 0;
        allMeshCount = 0;
        Debug.Log("Close window,clear info...");
    }
    #endregion
    #region Method
    static List<LODGroupDetails> lodInfos;
    /// <summary>
    /// 查找当前场景下，所有的LodGroup信息
    /// </summary>
    private void GetSceneLodGroupInfo()
    {
        if (lodInfos != null) return;
        lodInfos = LODGroupDetails.GetSceneLodGroupInfo(null);
    }

    static bool isCaculate;
    static int allVertexCount;
    static int allMeshCount;
    /// <summary>
    /// 计算当前视角下，场景Lod信息
    /// </summary>
    private void CaculateGroupInfo()
    {
        DateTime now = DateTime.Now;
        if (lodInfos == null||isCaculate) return;
        isCaculate = true;
        // Camera cam = gameCamera.GetComponent<Camera>();
        // allVertexCount = 0;
        // allMeshCount = 0;
        // //1.计算当前所有LODGroup的level层级，并统计顶点和mesh的总数
        // foreach (var item in lodInfos)
        // {            
        //     LODChildInfo info = item.CaculateLODValueInfo(sceneView, cam);
        //     if(info!=null)
        //     {
        //         allVertexCount += info.vertexCount;
        //         allMeshCount += info.meshCount;
        //     }
        // }
        // //计算子物体所占百分比
        // foreach(var item in lodInfos)
        // {
        //     LODChildInfo child = item.currentChild;
        //     if (child == null) continue;//culloff层级时，是没有lod信息的
        //     child.vertexPercent = allVertexCount == 0 ? 0 : (float)child.vertexCount / allVertexCount;
        //     child.meshPercent = allMeshCount == 0 ? 0 : (float)child.meshCount / allMeshCount;
        //     //Debug.LogError("v:"+child.vertexPercent+" m:"+child.meshPercent+" "+child.vertexCount+" "+allVertexCount);
        // }      
        // lodInfos.Sort((a,b)=> 
        // {
        //     if(sortType==LODSortType.Mesh)
        //     {
        //         int aMesh = a.currentChild == null ? 0 : a.currentChild.meshCount;
        //         int bMesh = b.currentChild == null ? 0 : b.currentChild.meshCount;
        //         return bMesh.CompareTo(aMesh);
        //     }
        //     else
        //     {
        //         int aVetex = a.currentChild == null ? 0 : a.currentChild.vertexCount;
        //         int bVetex = b.currentChild == null ? 0 : b.currentChild.vertexCount;
        //         return bVetex.CompareTo(aVetex);
        //     }
        // });
        Camera cam = gameCamera.GetComponent<Camera>();
        int[] infos=LODGroupDetails.CaculateGroupInfo(lodInfos,sceneView,sortType,cam);
        allVertexCount=infos[0];
        allMeshCount=infos[1];

        isCaculate = false;
        Debug.LogFormat("刷新数据完成，耗时{0}s",(DateTime.Now-now).TotalSeconds.ToString("f1"));
    }

    #endregion
}
