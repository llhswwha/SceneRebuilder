using CommonUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;

public class ModelPivotTool : MonoBehaviour
{
    public string PrefabPath="";

    [ContextMenu("TestPrefab")]
    public void TestPrefab(){
        PrefabPath=PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
        PrefabUtility.UnpackPrefabInstance(gameObject,PrefabUnpackMode.OutermostRoot,InteractionMode.UserAction);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("TestBounds")]
    public void TestBounds()
    {
        var ps = ColliderExtension.GetBoundsMaxMinPoints(gameObject);
        var center = (ps[0] + ps[1]) / 2;
        var max = ps[0];
        var min = ps[1];

        //MeshRenderer render = this.GetComponent<MeshRenderer>();
        var t1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        t1.transform.position = min;
        print(min);

        var t2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        t2.transform.position = max;
        print(max);

        var t3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        t3.transform.position = center;
        print(center);

        //var ps=ColliderHelper.GetBoundsMaxMinPoints(gameObject);
        //foreach (var item in ps)
        //{
        //    print(item);
        //}
    }

    public Vector3 GetCenter()
    {
        var ps = ColliderExtension.GetBoundsMaxMinPoints(gameObject);
        var c = (ps[0] + ps[1]) / 2;
        return c;
    }

    // [MenuItem("Tools/Prefab/去除引用")]

    // public static void BreakPrefabRef()
    // {
    //     var select = Selection.activeGameObject;
    //     if (select.activeInHierarchy)
    //     {
    //         PrefabUtility.DisconnectPrefabInstance(select);
    //         Selection.activeGameObject = null;
    //         var prefab = PrefabUtility.CreateEmptyPrefab("Assets/empty.prefab");
    //         PrefabUtility.ReplacePrefab(select, prefab, ReplacePrefabOptions.ConnectToPrefab);
    //         PrefabUtility.DisconnectPrefabInstance(select);
    //         AssetDatabase.DeleteAsset("Assets/empty.prefab");
    //     }
    // }

    public static void SetPivotCenterInner(GameObject obj)
    {
        Debug.Log("SetPivotCenterInner:"+obj);
        string path=PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
        Debug.Log("path:"+path);
        if(!string.IsNullOrEmpty(path)){
            PrefabUtility.UnpackPrefabInstance(obj,PrefabUnpackMode.OutermostRoot,InteractionMode.UserAction);
        }


        // PrefabUtility.SaveAsPrefabAsset(obj, "Assets/Editor/Prefabs/parent.prefab");
        var renderers = obj.GetComponentsInChildren<MeshRenderer>(true);
        foreach(var render in renderers)
        {
            render.enabled = true;
        }

        var ps = ColliderExtension.GetBoundsMaxMinPoints(obj);
        var center = (ps[0] + ps[1]) / 2;
        List<Transform> items = new List<Transform>();
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            items.Add(obj.transform.GetChild(i));
        }
        foreach (var item in items)
        {
            item.parent = null;
        }
        Debug.Log("position:" + obj.transform.position);
        obj.transform.position = center;
        Debug.Log("center:" + center);
        foreach (var item in items)
        {
            item.parent = obj.transform;
        }

        if(!string.IsNullOrEmpty(path)){
            string localPath = AssetDatabase.GenerateUniqueAssetPath(path);
            Debug.Log("localPath:"+localPath);
            // Create the new Prefab.
            PrefabUtility.SaveAsPrefabAssetAndConnect(obj, localPath, InteractionMode.UserAction);
        }
    }

    [ContextMenu("SetPivotCenter")]
    public void SetPivotCenter()
    {

        SetPivotCenterInner(gameObject);
    }

    [ContextMenu("SetPivotCenterAll")]
    public void SetPivotCenterAll()
    {
        LODGroup[] gs = this.GetComponentsInChildren<LODGroup>();
        print(gs.Length);
        List<Transform> ps = new List<Transform>();
        foreach (var item in gs)
        {
            print(item);
            if (!ps.Contains(item.transform.parent))
            {
                ps.Add(item.transform.parent);
            }
        }
        print(ps.Count);
        foreach (var item in ps)
        {
            print(item);
            SetPivotCenterInner(item.gameObject);
        }
    }
}
#endif