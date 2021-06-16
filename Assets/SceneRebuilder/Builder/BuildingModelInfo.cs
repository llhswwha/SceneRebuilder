using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class BuildingModelInfo : MonoBehaviour
{
    public GameObject InPart;

    public GameObject OutPart0;
    public GameObject OutPart1;

    public float InVertextCount = 0;
    public float Out0VertextCount = 0;
    public float Out1VertextCount = 0;

    public int InRendererCount = 0;
    public int Out0RendererCount = 0;
    public int Out1RendererCount = 0;

    public int AllRendererCount = 0;
    public float AllVertextCount = 0;

    //[ContextMenu("* CreateTrees")]
    public ModelAreaTree[] CreateTreesInner()
    {
        var oldTrees = this.GetComponentsInChildren<ModelAreaTree>(true);
        foreach (var oldT in oldTrees)
        {
            GameObject.DestroyImmediate(oldT.gameObject);
        }

        UpackPrefab_One(this.gameObject);
        ModelAreaTree[] trees = new ModelAreaTree[3];
        var tree1 = CreateTree(InPart, "InTree");
        trees[0] = tree1;
        var tree2 = CreateTree(OutPart0, "OutTree0");
        trees[1] = tree2;
        var tree3 = CreateTree(OutPart1, "OutTree1");
        trees[2] = tree3;
        return trees;
    }

    [ContextMenu("* InitInOut")]
    public void InitInOut()
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.name == "In")
            {
                InPart = child.gameObject;
            }
            if (child.name == "Out0" || child.name == "Out")
            {
                OutPart0 = child.gameObject;
            }
            if (child.name == "Out1")
            {
                OutPart1 = child.gameObject;
            }
            children.Add(child);
        }

        if (OutPart1 == null && OutPart0 == null && InPart == null)
        {
            GameObject outRoot = InitPart("Out0");
            foreach (var child in children)
            {
                child.SetParent(outRoot.transform);
            }
        }

        GetInOutVertextCount();

        HideDetail();

        //var manager=GameObject.FindObjectOfType<>
    }

    [ContextMenu("* CreateTrees")]
    public void CreateTrees()
    {
        var trees = CreateTreesInner();

        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if (treeManager)
            treeManager.AddTrees(trees);
    }

    [ContextMenu("ShowRenderers")]
    public void ShowRenderers()
    {
        var renderers = this.GetComponentsInChildren<Renderer>(true) ;
        foreach(var render in renderers)
        {
            render.enabled = true;
        }
    }

    [ContextMenu("ShowAll")]
    public void ShowAll()
    {
        if (InPart)
        {
            InPart.SetActive(true);
        }
        if (OutPart0)
        {
            OutPart0.SetActive(true);
        }
        if (OutPart1)
        {
            OutPart1.SetActive(true);
        }
    }

    

    public static void UpackPrefab_One(GameObject go)
    {
#if UNITY_EDITOR
        GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
        if (root != null)
        {
            PrefabUtility.UnpackPrefabInstance(root, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        }
#endif
    }

 

    private ModelAreaTree CreateTree(GameObject target,string treeName)
    {
        if (target == null) return null;
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if(treeManager)
            AreaTreeHelper.CubePrefab = treeManager.CubePrefab;

        GameObject treeGo1 = new GameObject(treeName);
        treeGo1.transform.position = target.transform.position;
        treeGo1.transform.SetParent(this.transform);
        ModelAreaTree tree1 = treeGo1.AddComponent<ModelAreaTree>();
        tree1.Target = target;
        if (treeManager)
        {
            tree1.nodeSetting = treeManager.nodeSetting;
        }
            //tree1.GenerateTree();//没有合并
            tree1.GenerateMesh();//合并
        //treeGo1.SetActive(target.activeInHierarchy);//该隐藏的继续隐藏
        tree1.IsHidden = !target.activeInHierarchy;//动态隐藏
        return tree1;
    }

    

    public GameObject InitPart(string n)
    {
        GameObject outRoot = new GameObject(n);
        outRoot.transform.position = this.transform.position;
        outRoot.transform.SetParent(this.transform);
        return outRoot;
    }

    public GameObject InitSubPart(string n,Transform p)
    {
        GameObject outRoot = new GameObject(n);
        outRoot.transform.position = p.position;
        outRoot.transform.SetParent(p);
        return outRoot;
    }

    private void GetInOutVertextCount()
    {
        InVertextCount = GetChildrenVertextCount(InPart, out InRendererCount,false);
        Out0VertextCount = GetChildrenVertextCount(OutPart0,out Out0RendererCount, true);
        Out1VertextCount = GetChildrenVertextCount(OutPart1,out Out1RendererCount, false);
        AllVertextCount = InVertextCount + Out0VertextCount + Out1VertextCount;
        AllRendererCount = InRendererCount + Out0RendererCount + Out1RendererCount;
    }

    class VertexCountInfo
    {
        public string name;

        public float count;

        public VertexCountInfo(float c,string n)
        {
            count = c;
            name = n;
        }

        public override string ToString()
        {
            return $"{count:F1}    {name}";
        }
    }

    private float GetChildrenVertextCount(GameObject go, out int renderCount, bool showLog)
    {
        renderCount = 0;
        if (go == null)
        {
            return 0;
        }
        float count = 0;
        List<VertexCountInfo> logs = new List<VertexCountInfo>();
        for (int i = 0; i < go.transform.childCount; i++)
        {
            var child = go.transform.GetChild(i);
            int childRendererCount = 0;
            float childVertexCount = GetVertextCount(child.gameObject, out childRendererCount, showLog);
            count += childVertexCount;
            renderCount += childRendererCount;

            if (childVertexCount > 1)
            {
                //logs.Add($"{childVertexCount:F1}    {go.name}->{child.name}");
                logs.Add(new VertexCountInfo(childVertexCount, $"{go.name}->{child.name}"));
                //Debug.LogError($"GetChildrenVertextCount[{i}]    {childVertexCount:F3}    {go.name}->{child.name}");
            }
        }
        logs.Sort((a,b)=>a.count.CompareTo(b.count));

        for (int i = 0; i < logs.Count; i++)
        {
            Debug.LogError($"GetChildrenVertextCount[{i}]    {logs[i]}");
        }
        return count;
    }

    private float GetVertextCount(GameObject go,out int renderCount,bool showLog)
    {
        renderCount = 0;
        if (go == null)
        {
            return 0;
        }
        float count = 0;
        var filters = go.GetComponentsInChildren<MeshFilter>(true).ToList().FindAll(i=>i!=null&&i.sharedMesh!=null);
        //filters.Sort((a, b) => b.sharedMesh.vertexCount.CompareTo(a.sharedMesh.vertexCount));
        filters.Sort((a, b) => a.sharedMesh.vertexCount.CompareTo(b.sharedMesh.vertexCount));

        for (int i = 0; i < filters.Count; i++)
        {
            MeshFilter filter = filters[i];
            if (filter == null) continue;
            if (filter.sharedMesh == null) continue;
            float vertextCount = filter.sharedMesh.vertexCount / 10000.0f;
            count += vertextCount;
            if (showLog)
            {
                if (vertextCount > 5f)
                {
                    Debug.LogWarning($"GetVertextCount[{i}]    {vertextCount:F3}    {go.name}->{filter.name}|{filter.transform.parent}");
                }
                else if (vertextCount > 1f)
                {
                    Debug.Log($"GetVertextCount[{i}]    {vertextCount:F3}    {go.name}->{filter.name}|{filter.transform.parent}");
                }
            }

        }
        renderCount = filters.Count;
        return count;
    }

    private void Awake()
    {
        InitInOut();
    }

    [ContextMenu("FindInGos")]
    public void FindInGos()
    {

    }

    [ContextMenu("HideIn")]
    public void HideIn()
    {
        if (InPart)
            InPart.SetActive(false);
    }

    [ContextMenu("ShowIn")]
    public void ShowIn()
    {
        if (InPart)
            InPart.SetActive(true);
    }

    public bool IsDetailVisible = false;

    [ContextMenu("HideDetail")]
    public void HideDetail()
    {
        IsDetailVisible = false;
        if(InPart)
            InPart.SetActive(false);
        if(OutPart1)
            OutPart1.SetActive(false);
    }

    [ContextMenu("ShowDetail")]
    public void ShowDetail()
    {
        if(InPart)
            InPart.SetActive(true);
        if(OutPart1)
            OutPart1.SetActive(true);
    }

    [ContextMenu("CombinedInOut")]
    public void CombinedInOut()
    {
        //var inCombined=
    }

    [ContextMenu("CombinedInOut_Tree")]
    public void CombinedInOut_Tree()
    {
        //var inCombined=
    }
}
