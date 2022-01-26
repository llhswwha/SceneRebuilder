using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Y_UIFramework;

/// <summary>
/// 项目流程控制脚本
/// </summary>
public class MainManage : MonoBehaviour
{
    //private static MainManage instance;
    //public static MainManage Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {    //查找场景中是否已经存在单例
    //            instance = GameObject.FindObjectOfType<MainManage>();
    //            if (instance == null)
    //            {    //创建游戏对象然后绑定单例脚本
    //                GameObject go = new GameObject("MainManage");
    //                instance = go.AddComponent<MainManage>();
    //            }
    //            DontDestroyOnLoad(instance.gameObject);
    //        }

    //        return instance;
    //    }
    //}

    ///// <summary>
    ///// 是Resources加载，还是Assetbundle加载
    ///// </summary>
    //public bool IsResourcesLoadBuilding = true;

    //public bool SubSceneLoading = true;
    //public bool IsSubSceneLoadBuilding;
    //public bool IsSubSceneInnerLoad;

    //public void Awake()
    //{
    //    UIManager.GetInstance();
    //    #if !UNITY_EDITOR
    //        IsResourcesLoadBuilding=false;
    //    #endif
    //}

    //public DateTime startT;

    //public TimeSpan timeSpan;

    //public double TotalMilliseconds;

    //// Start is called before the first frame update
    //IEnumerator Start()
    //{
    //    startT = DateTime.Now;
    //    LoadCSV();//文本数据加载
    //    if (SubSceneLoading)
    //    {
    //        yield return StartCoroutine(ModelsManage.Instance.LoadSubSceneStart(AfterLoadbuilds));
    //    }
    //    else
    //    {
    //        yield return StartCoroutine(ModelsManage.Instance.AStart(AfterLoadbuilds));//需要建筑加载之后运行的代码需要放在AfterLoadbuilds函数里面
    //    }
    //    Debug.Log("运行完毕Start！");
    //}

    //[ContextMenu("LoadCSV")]
    //public void LoadCSV()
    //{
    //    CSVManage.Instance.Init();//文本数据加载
    //    CsvModels=new List<CsvModel>();
    //    CsvModels.AddRange(CSVManage.Instance.StartLoadsModelList);
    //}

    //[ContextMenu("LoadModels")]
    //public void LoadModels()
    //{
    //    // LoadCSV();
    //    foreach(var m in CsvModels){
    //        var p=m.PrefabPath;//Resources/FactoryPrefabs/BuildsPrefabs/Builds/K1UCC-集中控制楼.prefab
    //        p=p.Replace("Resources/","");
    //        p=p.Replace(".prefab","");
    //        var go=Resources.Load<GameObject>(p);
    //        if(go==null){
    //            Debug.LogError("go==null:"+p);
    //        }
    //        else{
    //            var goIns=GameObject.Instantiate(go);
    //        }
            
    //    }
    //}

    //[ContextMenu("LoadModelAssets")]
    //public void LoadModelAssets()
    //{
    //    // LoadCSV();
    //    foreach(var m in CsvModels){
    //        var p=m.PrefabPath;//Resources/FactoryPrefabs/BuildsPrefabs/Builds/K1UCC-集中控制楼.prefab
    //        p=p.Replace("Resources/","");
    //        p=p.Replace(".prefab","");
    //        var go=Resources.Load<GameObject>(p);
    //        if(go==null){
    //            Debug.LogError("go==null:"+p);
    //        }
    //        else{
    //            var goIns=GameObject.Instantiate(go);
    //        }
            
    //    }
    //}

    //public List<CsvModel> CsvModels=new List<CsvModel>();

    //public GameObject[] ModelList;

    ///// <summary>
    ///// 建筑加载完之后
    ///// </summary>
    //public void AfterLoadbuilds()
    //{
    //    timeSpan = DateTime.Now - startT;
    //    TotalMilliseconds = timeSpan.TotalMilliseconds;
    //    Debug.Log("AfterLoadbuilds！");
    //    this.ModelList = ModelsManage.Instance.ModelList;

    //    //StartCoroutine(LoadInBuildingObjs());
    //    //注释：中山的消防相关数据在宿迁没用，YZL20200108
    //    //LoadRounte.Instance.InitBuildList();
    //    //LoadRounte.Instance.ReadJson();
    //    //LoadRounte.Instance.ReadParkJson();
    //    //LoadRounte.Instance.SetRounteShow(false);
    //    //BuildManager.Instance.InitBuildList(); 
    //    if (DynamicCullingManage.Instance != null) DynamicCullingManage.Instance.SetCullingRenders();
    //}

    ///// <summary>
    ///// 加载建筑内部细节
    ///// </summary>
    //public IEnumerator LoadInBuildingObjs()
    //{
    //    InBuildingObjs[] inBuildingObjsArr = GameObject.FindObjectsOfType<InBuildingObjs>();
    //    foreach (InBuildingObjs build in inBuildingObjsArr)
    //    {
    //        yield return StartCoroutine(build.LoadInBuildingObjs(null));
    //    }
    //    yield return null;
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
