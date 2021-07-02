using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSceneShowManager : MonoBehaviour
{
    public static SubSceneShowManager Instance;

    public SubSceneManager sceneManager;
    public SubScene_Out0[] scenes_Out0;
    public SubScene_Out1[] scenes_Out1;
    public SubScene_In[] scenes_In;

    public List<SubScene_In> scenes_In_Part = new List<SubScene_In>();
    public List<SubScene_In> scenes_In_Tree = new List<SubScene_In>();

    public List<SubScene_Out0> scenes_Out0_Part = new List<SubScene_Out0>();
    public List<SubScene_Out0> scenes_Out0_Tree = new List<SubScene_Out0>();
    //public List<SubScene_Out0> scenes_Out0_TreeNode = new List<SubScene_Out0>();
    public List<SubScene_Out0> scenes_Out0_TreeNode_Hidden = new List<SubScene_Out0>();
    public List<SubScene_Out0> scenes_Out0_TreeNode_Shown = new List<SubScene_Out0>();
    public Camera[] cameras;

    public string GetSceneCountInfo()
    {

        var alls=GameObject.FindObjectsOfType<SubScene_Base>(true);
        var allsInfo=GetSceneRenderInfo(alls);
        var ins=GameObject.FindObjectsOfType<SubScene_In>(true);
        var insInfo=GetSceneRenderInfo(ins);
        var out0s=GameObject.FindObjectsOfType<SubScene_Out0>(true);
        var out0sInfo=GetSceneRenderInfo(out0s);
        var out1s=GameObject.FindObjectsOfType<SubScene_Out1>(true);
        var out1sInfo=GetSceneRenderInfo(out1s);
        
        // List<SubScene_Out0> out0s_tree
        
        
        return $"all:{alls.Length}(v:{allsInfo[0]:F0},r:{allsInfo[1]:F0}) | out0:{out0s.Length}(v:{out0sInfo[0]:F0},r:{out0sInfo[1]:F0})\nout1:{out1s.Length}(v:{out1sInfo[0]:F0},r:{out1sInfo[1]:F0}) | ins:{ins.Length}(v:{insInfo[0]:F0},r:{insInfo[1]:F0})";
    }

    public string GetShowSceneCountInfo()
    {
        var scenes_Out0_Part_Info=GetSceneRenderInfo(scenes_Out0_Tree);
        var scenes_In_Tree_Info=GetSceneRenderInfo(scenes_In_Tree);
        var scenes_Out0_TreeNode_Shown_Info=GetSceneRenderInfo(scenes_Out0_TreeNode_Shown);
        var scenes_Out0_TreeNode_Hidden_Info=GetSceneRenderInfo(scenes_Out0_TreeNode_Hidden);

        return $"out0_tree:{scenes_Out0_Tree.Count}(v:{scenes_Out0_Part_Info[0]:F0},r:{scenes_Out0_Part_Info[1]:F0})"
        +$" out0_node_show:{scenes_Out0_TreeNode_Shown.Count}(v:{scenes_Out0_TreeNode_Shown_Info[0]:F0},r:{scenes_Out0_TreeNode_Shown_Info[1]:F0})"
        +$"\nin_tree:{scenes_In_Tree.Count}(v:{scenes_In_Tree_Info[0]:F0},r:{scenes_In_Tree_Info[1]:F0})"
        +$" out0_node_hide:{scenes_Out0_TreeNode_Hidden.Count}(v:{scenes_Out0_TreeNode_Hidden_Info[0]:F0},r:{scenes_Out0_TreeNode_Hidden_Info[1]:F0})";
    }

    public float[] GetSceneRenderInfo<T>(IEnumerable<T> scenes) where T : SubScene_Base
    {
        float vCount=0;
        float rCount=0;
        foreach(T scene in scenes){
            vCount+=scene.vertexCount;
            rCount+=scene.rendererCount;
        }
        return new float[]{vCount,rCount};
    }
    
    public string GetSceneInfo()
    {
        return $"visible:{visibleScenes.Count},loaded:{loadScenes.Count},hidden:{hiddenScenes.Count},unloaded:{unloadScenes.Count}";
    }

    public string GetDisInfo()
    {
        return $"Min:{MinDisToCam:F0},Max:{MaxDisToCam:F0},MinSqrt:{MinDisSqrtToCam:F0},MaxSqrt:{MaxDisSqrtToCam:F0},time:{TimeOfDis:F1}";
    }

    [ContextMenu("Init")]
    public void Init()
    {
        sceneManager = GameObject.FindObjectOfType<SubSceneManager>(true);
        scenes_In = GameObject.FindObjectsOfType<SubScene_In>(true);
        scenes_Out0 = GameObject.FindObjectsOfType<SubScene_Out0>(true);

        foreach(var s in scenes_Out0)
        {
            //s.gameObject.SetActive(true);
            if(s.contentType==SceneContentType.Part)
            {
                scenes_Out0_Part.Add(s);
                s.HideBoundsBox();

            }
            if (s.contentType == SceneContentType.Tree)
            {
                scenes_Out0_Tree.Add(s);
            }
            //if (s.contentType == SceneContentType.TreeNode)
            //{
            //    scenes_Out0_TreeNode.Add(s);
            //}
        }

        foreach(var s in scenes_In)
        {
            //s.gameObject.SetActive(true);
            if(s.contentType==SceneContentType.Part)
            {
                scenes_In_Part.Add(s);
                s.HideBoundsBox();

            }
            if (s.contentType == SceneContentType.Tree)
            {
                scenes_In_Tree.Add(s);
            }
            //if (s.contentType == SceneContentType.TreeNode)
            //{
            //    scenes_Out0_TreeNode.Add(s);
            //}
        }


        scenes_Out1 = GameObject.FindObjectsOfType<SubScene_Out1>(true);

        cameras = GameObject.FindObjectsOfType<Camera>();

        List<ModelAreaTree> HiddenTrees = new List<ModelAreaTree>();
        List<ModelAreaTree> ShownTrees = new List<ModelAreaTree>();
        var ts = GameObject.FindObjectsOfType<ModelAreaTree>(true);

        foreach (ModelAreaTree t in ts)
        {
            if (t.IsHidden && !HiddenTrees.Contains(t))
            {
                HiddenTrees.Add(t);
                //HiddenTreesVertexCount += t.VertexCount;
                var scenes = t.GetComponentsInChildren<SubScene_Out0>(true);
                scenes_Out0_TreeNode_Hidden.AddRange(scenes);
            }
            else if (t.IsHidden == false && !ShownTrees.Contains(t))
            {
                ShownTrees.Add(t);
                //ShownTreesVertexCount += t.VertexCount;

                var scenes = t.GetComponentsInChildren<SubScene_Out0>(true);
                scenes_Out0_TreeNode_Shown.AddRange(scenes);
            }
        }
    }

    public bool IsAutoLoad = false;

    private void Awake()
    {
        Instance = this;
        Init();
    }

    public List<SubScene_Base> WaitingScenes = new List<SubScene_Base>();

    public bool IsUpdateTreeNodeByDistance = false;
    public bool IsUpdateDistance = true;
    public bool IsEnableLoad = false; 
    public bool IsEnableUnload=false;
    public bool IsEnableHide = false; 
    public bool IsEnableShow=false;

    public void LoadStartScens()
    {
        List<SubScene_Out0> scene_Out0s=new List<SubScene_Out0>();
        scene_Out0s.AddRange(scenes_Out0_Tree);
        scene_Out0s.AddRange(scenes_Out0_TreeNode_Shown);
        AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = false;
        //sceneManager.LoadScenesEx(scenes_Out0_Tree.ToArray());
        sceneManager.LoadScenesEx(scene_Out0s.ToArray(), () =>
            {
                //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = true;

                WaitingScenes.AddRange(scene_Out0s);

                AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = IsUpdateTreeNodeByDistance;
            });
    }

    public void LoadOut0TreeScenes()
    {
        //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = false;
        //sceneManager.LoadScenesEx(scenes_Out0_Tree.ToArray());
        sceneManager.LoadScenesEx(scenes_Out0_Tree.ToArray(), () =>
            {
                //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = true;

                WaitingScenes.AddRange(scenes_Out0_Tree);

                //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = IsUpdateTreeNodeByDistance;
            });
    }

    public void LoadOut0TreeNodeSceneTop1()
    {
        //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = false;
        //sceneManager.LoadScenesEx(scenes_Out0_Tree.ToArray());
        sceneManager.LoadScenesEx(scenes_Out0_TreeNode_Shown.ToArray(), () =>
        {
                //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = true;
                WaitingScenes.AddRange(scenes_Out0_TreeNode_Shown);
                //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = IsUpdateTreeNodeByDistance;
        });
    }

    public void LoadOut0TreeNodeSceneTopN(int index)
    {
        scenes_Out0_TreeNode_Shown.Sort((a,b)=>b.vertexCount.CompareTo(a.vertexCount));
        var scene=scenes_Out0_TreeNode_Shown[index];
        Debug.Log($"LoadOut0TreeNodeSceneTopN index:{index} scene:{scene.name} vertexCount:{scene.vertexCount}");
        scene.LoadSceneAsync(null);
    }

    public void LoadOut0TreeNodeSceneTopBiggerN(int index)
    {
        scenes_Out0_TreeNode_Shown.Sort((a,b)=>b.vertexCount.CompareTo(a.vertexCount));
        List<SubScene_Base> topsScenes=new List<SubScene_Base>();
        float vertexCount=0;
        for(int i=index;i<scenes_Out0_TreeNode_Shown.Count;i++)
        {
             var scene=scenes_Out0_TreeNode_Shown[i];
            topsScenes.Add(scene);
            vertexCount+=scene.vertexCount;
        }
        sceneManager.LoadScenesEx(topsScenes.ToArray(), () =>
        {
                WaitingScenes.AddRange(topsScenes);
        });

        Debug.Log($"LoadOut0TreeNodeSceneTopN index:{index} scene:{topsScenes.Count} vertexCount:{vertexCount}");
    }

    public void LoadOut0TreeNodeScenes()
    {
        //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = false;
        //sceneManager.LoadScenesEx(scenes_Out0_Tree.ToArray());
        sceneManager.LoadScenesEx(scenes_Out0_TreeNode_Shown.ToArray(), () =>
        {
                //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = true;
                WaitingScenes.AddRange(scenes_Out0_TreeNode_Shown);
                //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = IsUpdateTreeNodeByDistance;
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        //Init();

        if (IsAutoLoad)
        {
            LoadStartScens();
        }

        if (AreaTreeNodeShowManager.Instance)
        {
            AreaTreeNodeShowManager.Instance.ShowNodeDistance = this.DisOfVisible;
            AreaTreeNodeShowManager.Instance.HideNodeDistance = this.DisOfHidden;
        }
    }

    public float DisOfVisible = 1600;//40
    public float DisOfLoad = 2500;//50
    public float DisOfHidden = 3600;//60
    public float DisOfUnLoad = 4900;//70

    public float MinDisSqrtToCam = 0;

    public float MaxDisSqrtToCam = 0;

    public float MinDisToCam = 0;

    public float MaxDisToCam = 0;

    public float AvgDisToCam = 0;

    public double TimeOfDis = 0;

    public double TimeOfLoad = 0;

    public SubScene_Base MinDisScene;


    public List<SubScene_Base> visibleScenes = new List<SubScene_Base>();
    public List<SubScene_Base> loadScenes = new List<SubScene_Base>();
    public List<SubScene_Base> hiddenScenes = new List<SubScene_Base>();
    public List<SubScene_Base> unloadScenes = new List<SubScene_Base>();

   void LoadUnloadScenes()
    {
        //if (EnableLoadUnload == false) return;
        DateTime start = DateTime.Now;
        if(IsEnableShow)
            foreach (var scene in visibleScenes)
            {
                scene.ShowObjects();
            }
        if(IsEnableHide)
            foreach (var scene in hiddenScenes)
            {
                scene.HideObjects();
            }
        if(IsEnableLoad)
            foreach (var scene in loadScenes)
            {
                
                scene.LoadSceneAsync(null);
            }
        if(IsEnableUnload)
            foreach (var scene in unloadScenes)
            {
                scene.UnLoadSceneAsync();
            }
        TimeOfLoad = (DateTime.Now - start).TotalMilliseconds;
    }

    void CalculateDistance(List<SubScene_Base> scenes)
    {
        DateTime start = DateTime.Now;

        MaxDisSqrtToCam = 0;
        MinDisSqrtToCam = float.MaxValue;
        float sumDis = 0;
        visibleScenes = new List<SubScene_Base>();
        hiddenScenes = new List<SubScene_Base>();
        loadScenes = new List<SubScene_Base>();
        unloadScenes = new List<SubScene_Base>();

        foreach (var scene in scenes)
        {
            float disToCams = 0;
            foreach (var cam in cameras)
            {
                Vector3 cP = cam.transform.position;
                float dis = scene.bounds.SqrDistance(cP);
                if (dis > disToCams)
                {
                    disToCams = dis;
                }
            }

            if (disToCams > MaxDisSqrtToCam)
            {
                MaxDisSqrtToCam = disToCams;
            }
            if (disToCams < MinDisSqrtToCam)
            {
                MinDisSqrtToCam = disToCams;
                MinDisScene = scene;
            }
            sumDis += disToCams;

            if (disToCams <= DisOfLoad)
            {
                loadScenes.Add(scene);
            }
            if (disToCams <= DisOfVisible)
            {
                visibleScenes.Add(scene);
            }
            if (disToCams > DisOfUnLoad)
            {
                unloadScenes.Add(scene);
            }
            if (disToCams > DisOfHidden)
            {
                hiddenScenes.Add(scene);
            }

            if (disToCams <= DisOfLoad)
            {
                loadScenes.Add(scene);

                
            }

            if (disToCams <= DisOfVisible)
            {
                scene.boundsGo.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.2f);
            }
            else if (disToCams <= DisOfLoad)
            {
                scene.boundsGo.GetComponent<MeshRenderer>().material.color = new Color(1, 0.5f, 0, 0.2f);
            }
            else if (disToCams <= DisOfHidden)
            {
                scene.boundsGo.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 0, 0.2f);
            }
            else if (disToCams <= DisOfUnLoad)
            {
                scene.boundsGo.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0.2f);
            }

            scene.DisToCam = disToCams;
        }

        AvgDisToCam = Mathf.Sqrt(sumDis / scenes.Count);
        MinDisToCam = Mathf.Sqrt(MinDisSqrtToCam);
        MaxDisToCam = Mathf.Sqrt(MaxDisSqrtToCam);

        TimeOfDis = (DateTime.Now - start).TotalMilliseconds;
    }

    //int updateCount = 0;
    //private void FixedUpdate()
    //{
    //    updateCount++;
    //    if (IsAutoLoad&& updateCount>100)
    //    {
    //        IsAutoLoad = false;
    //        //sceneManager.LoadScenesEx(scenes_Out0_Tree.ToArray());//1.����ʱ�Զ�����ģ��
    //        sceneManager.LoadScenesEx(scenes_Out0_TreeNode.ToArray(), () =>
    //        {
    //            //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = true;
    //        });//1.����ʱ�Զ�����ģ��
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        if (WaitingScenes.Count > 0)
        {
            Debug.LogError("CheckWaittingScenes 1:"+ WaitingScenes.Count);
            for(int i = 0; i < WaitingScenes.Count; i++)
            {
                var scene = WaitingScenes[i];
                
                if (scene.GetSceneObjectCount()>0)
                {
                    WaitingScenes.RemoveAt(i);
                    i--;
                }
                else
                {
                    scene.CheckGetSceneObjects();
                }


            }
            Debug.LogError("CheckWaittingScenes 2:" + WaitingScenes.Count);
        }

        if (IsUpdateDistance)
        {
            List<SubScene_Base> subScenes = new List<SubScene_Base>();
            //foreach (var scene in scenes_In)
            //{
            //    subScenes.Add(scene);
            //}
            //foreach (var scene in scenes_Out1)
            //{
            //    subScenes.Add(scene);
            //}
            foreach (var scene in scenes_Out0_TreeNode_Hidden)
            {
                subScenes.Add(scene);
            }
            CalculateDistance(subScenes);
            LoadUnloadScenes();
        }
    }
}
