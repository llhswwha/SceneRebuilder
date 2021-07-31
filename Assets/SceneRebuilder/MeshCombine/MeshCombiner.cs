using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

//[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
public class MeshCombiner : MonoBehaviour
{
    public GameObject sourceRoot;

    public MeshCombineSourceType sourceType;//0:sourceRoot合并为一个模型，1:sourceRoot的子物体分别合并为一个模型


    public List<GameObject> sourceList=new List<GameObject>();

    public List<GameObject> resultList=new List<GameObject>();

    public bool Auto=false;

    public bool IsCombine=false;

    public MeshCombinerSetting Setting;

    // public bool IsDestroySource=false;
    // public bool IsCoroutine=false;

    // public int WaitCount=10000;


    // Start is called before the first frame update
    void Start()
    {

        GetSetting();

        if(Auto)
        {
            CombineByMaterial();
        }
    }

    public MeshCombinerSetting GetSetting()
    {
        if (Setting == null)
        {
            Setting = this.GetComponent<MeshCombinerSetting>();
        }
        if(Setting==null){
            Setting=MeshCombinerSetting.Instance;
        }
        if(Setting==null){
            Setting=this.gameObject.AddComponent<MeshCombinerSetting>();
        }
        Setting.SetSetting();
        return Setting;
    }

    public bool CombineBySub=false;

    public List<MeshCombiner> SubCombiners=new List<MeshCombiner>();

    public void ClearResult()
    {
        foreach (var result in resultList)
        {
            if (result == null) continue;
            GameObject.DestroyImmediate(result);
        }
        resultList = new List<GameObject>();
        foreach(var combineArg in combineArgs)
        {
            if (combineArg != null)
            {
                combineArg.ShowRendererers();
            }
        }
        combineArgs.Clear();
    }

    public void SaveResult()
    {
        DateTime start = DateTime.Now;
        for (int i = 0; i < resultList.Count; i++)
        {
            GameObject result = resultList[i];
            if (result == null) continue;
            GameObject source = sourceList[i];

            float progress = (float)i / resultList.Count;
            if (ProgressBarHelper.DisplayCancelableProgressBar("CombineEx", $"{i}/{resultList.Count} {progress:P1} source:{result.name}", progress))
            {
                break;
            }

            
            EditorHelper.SaveMeshAsset(source, result);
        }

        ProgressBarHelper.ClearProgressBar();

        Debug.Log($"SaveResult resultList:{resultList.Count} time:{DateTime.Now - start}");
    }

    private void Combine(MeshCombineMode mode)
    {
        DateTime start = DateTime.Now;

        GetSetting();

        Debug.LogError("Start CombineEx mode:" + mode + "|" + gameObject);

        ClearResult();

        InitSourceList();

        if (Setting.IsCoroutine)
        {
            StartCoroutine(CombineEx_Coroutine(mode));
        }
        else
        {
            for (int i = 0; i < sourceList.Count; i++)
            {
                GameObject source = sourceList[i];
                Debug.Log(string.Format("CombineEx {0} ({1}/{2})", source, i + 1, sourceList.Count));

                if (source == null) continue;

                float progress = (float)i / sourceList.Count;
                if (ProgressBarHelper.DisplayCancelableProgressBar("CombineEx", $"{i}/{sourceList.Count} {progress:P1} source:{source.name}", progress))
                {
                    break;
                }

                if (CombineBySub)
                {
                    MeshCombiner subCombiner = gameObject.AddComponent<MeshCombiner>();
                    subCombiner.Auto = true;
                    //subCombiner.IsCoroutine=this.IsCoroutine;
                    subCombiner.CombineBySub = false;
                    //subCombiner.WaitCount=this.WaitCount;
                    //subCombiner.IsDestroySource=false;
                    subCombiner.sourceList.Add(source);
                    SubCombiners.Add(subCombiner);
                }
                else
                {
                    var combineArg = new MeshCombineArg(source);
                    combineArgs.Add(combineArg);
                    GameObject target = MeshCombineHelper.CombineEx(combineArg, mode);
                    resultList.Add(target);
                    Debug.Log("Combine:" + source + "->" + target);
                    if (Setting.IsDestroySource)
                    {
                        target.name = source.name;
                        GameObject.DestroyImmediate(source);
                    }
                }
            }
            ProgressBarHelper.ClearProgressBar();

            Debug.Log($"CombineEx mode:{mode} souces:{sourceList.Count} time:{DateTime.Now - start}");
        }
    }

    public List<MeshCombineArg> combineArgs = new List<MeshCombineArg>();

    private void InitSourceList()
    {
        string sourceName = "";
        if ((sourceList.Count == 0 || sourceType == MeshCombineSourceType.All) && sourceRoot != null)
        {
            sourceList = new List<GameObject>();
            if (sourceType == MeshCombineSourceType.All)
            {
                sourceList.Add(sourceRoot);
                sourceName += sourceRoot.name;
            }
            else
            {
                for (int i = 0; i < sourceRoot.transform.childCount; i++)
                {
                    GameObject child = sourceRoot.transform.GetChild(i).gameObject;
                    sourceList.Add(child);
                    sourceName += child.name + ";";
                }
            }
        }
        Debug.Log("CombineEx sourceList:" + sourceName);
    }

    private IEnumerator CombineEx_Coroutine(MeshCombineMode mode)
    {
        DateTime start=DateTime.Now;
        for (int i = 0; i < sourceList.Count; i++)
        {
            GameObject source = (GameObject)sourceList[i];
             Debug.LogError(string.Format("CombineEx {0} ({1}/{2})",source,i+1,sourceList.Count));
            if (source!=null)
            {
                yield return MeshCombineHelper.CombineEx_Coroutine(new MeshCombineArg(source,null),Setting.IsDestroySource,Setting.WaitCount,mode);
            }
        }
        Setting.WriteLog("完成合并 用时:"+(DateTime.Now-start));
        yield return null;
    }


    [ContextMenu("CombineByMaterial")]

    public void CombineByMaterial(){
        GetSetting();
        sourceList.Clear();
        Combine(MeshCombineMode.MultiByMat);
    }


    [ContextMenu("CombineEx")]

    public void CombineEx(){
        GetSetting();
        sourceList.Clear();
        Combine(MeshCombineMode.OneMesh);
    }

    //public bool AutoAdd;

    //public List<GameObject> mergedObjs=new List<GameObject>();

    //public GameObject mergedObjRoot=null;

    //public GameObject mergedObj=null;

    //// Update is called once per frame
    //void Update()
    //{
    //    // if(Input.GetMouseButtonUp(0)){
    //    //     Debug.Log("Click");
    //    //     Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
    //    //     RaycastHit hit;
    //    //     if(Physics.Raycast(ray,out hit))
    //    //     {
    //    //         GameObject go=hit.collider.gameObject;
    //    //         Debug.Log("Hit:"+go);
    //    //         if(AutoAdd){
    //    //             if(!mergedObjs.Contains(go)){
    //    //                 if(mergedObjRoot==null){
    //    //                     mergedObjRoot=new GameObject();
    //    //                     mergedObjRoot.name="mergedObjRoot";
    //    //                 }

    //    //                 mergedObjs.Add(go);
    //    //                 go.transform.SetParent(mergedObjRoot.transform);

    //    //                 mergedObj=MeshCombineHelper.SimpleCombine(mergedObjRoot,mergedObj);
    //    //                 mergedObj.transform.SetParent(this.transform);
    //    //             }
    //    //         }
    //    //         if(AutoRemove){
    //    //             MeshCombineHelper.RemveGo(go);
    //    //         }
    //    //     }
    //    // }
    //}

    
    //public bool AutoRemove;


    [ContextMenu("ShowRenderers")]

    public void ShowRenderers(){
        var renderers=sourceRoot.GetComponentsInChildren<MeshRenderer>(true);
        foreach(var renderer in renderers){
            renderer.enabled=true;
        }
    }

    [ContextMenu("GetMaterilasDict")] 
    public void GetMaterilasDict()
    {
        var renderers = sourceRoot.GetComponentsInChildren<MeshRenderer>(true);
        int count = 0;
        var dict=MeshCombineHelper.GetMatFiltersInner(renderers, out count);
        foreach(var key in dict.Keys)
        {
            
            var list = dict[key];
            string names = "";
            foreach(var item in list)
            {
                names += item.meshFilter.name + ";\t";
            }

            Debug.Log($"mat:{key.name}\t\t list:{names}");
        }
    }

    [ContextMenu("GetMaterilasList")]
    public void GetMaterilasList()
    {
        var renderers = sourceRoot.GetComponentsInChildren<MeshRenderer>(true);
        //int count = 0;
        var list = MeshMaterialList.GetMeshMaterialList(renderers);
        list.SortByMat();
        Debug.LogError("----------SortByMat -----------------");
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            Debug.Log($"[{i}] {item}");
        }

        list.SortByMesh();
        Debug.LogError("----------SortByMesh -----------------");
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            Debug.Log($"[{i}] {item}");
        }

        list.SortByKey();
        Debug.LogError("----------SortByKey -----------------");
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            Debug.Log($"[{i}] {item}");
        }
    }
}

public enum MeshCombineSourceType
{
    All,//sourceRoot合并为一个模型
    Self,//sourceRoot的子物体分别合并为一个模型
}
