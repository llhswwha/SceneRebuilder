using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Bundles;
using Loxodon.Framework.Contexts;
using Mogoson.CameraExtension;
using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.RenderStreaming;
using UnityEngine;
using Y_UIFramework;

public class ModelsManage : MonoBehaviour
{
    private static ModelsManage instance;
    public static ModelsManage Instance
    {
        get
        {
            if (instance == null)
            {    //查找场景中是否已经存在单例
                instance = GameObject.FindObjectOfType<ModelsManage>();
                if (instance == null)
                {    //创建游戏对象然后绑定单例脚本
                    GameObject go = new GameObject("ModelsManage");
                    instance = go.AddComponent<ModelsManage>();
                    DontDestroyOnLoad(go);
                }
            }

            return instance;
        }
    }
    private IResources resources;
    public int Allnum;//要加载的模型数量
    public int loadedNum;//已加载的模型数量

    Action CompleteCallBack;

    public const string ResourcesRootName = "FactoryPrefabs";

    #region Start注释
    //#if UNITY_WEBGL
    //    public IEnumerator Start()
    //    {
    //        ApplicationContext context = Context.GetApplicationContext();

    //        while (this.resources == null)
    //        {
    //            this.resources = context.GetService<IResources>();//跟ModelsLauncher.cs相关联
    //            yield return null;
    //        }

    //        //this.Load(new string[] { "LoxodonFramework/BundleExamples/Models/Red/Red.prefab", "LoxodonFramework/BundleExamples/Models/Green/Green.prefab" });
    //        //this.StartCoroutine(Load2("Models/Zibo/ZiBoFBX/ZiBoFBX.FBX")); 
    //        //yield return this.StartCoroutine(LoadBuilding2());
    //        LoadBuilding();
    //    }
    //#else
    //    void Start()
    //    {
    //        ApplicationContext context = Context.GetApplicationContext();
    //        this.resources = context.GetService<IResources>();

    //        //this.Load(new string[] { "Models/Zibo/淄博电厂FBX/淄博电厂FBX.FBX"});
    //        //this.StartCoroutine(Load2("LoxodonFramework/BundleExamples/Models/Plane/Plane.prefab"));

    //        //startLoadsModelList = CSVManage.Instance.StartLoadsModelList;
    //    //LoadDiXin();
    //    //LoadBuilding();
    //    //yield return StartCoroutine(LoadBuilding2());
    //    //yield return this.StartCoroutine(LoadBuilding2());
    //    }
    //#endif
    #endregion

    public bool isSceneLoadFinish;

    public IEnumerator LoadSubSceneStart(Action callback)
    {
        //Debug.LogError($"ModelsManage.LoadSubSceneStart_1 resources:{resources} ");
        //1.初始化资源加载器（设备编辑用）
        ApplicationContext context = Context.GetApplicationContext();
        //Debug.LogError($"ModelsManage.LoadSubSceneStart_2 resources:{resources} ");
        while (this.resources == null)
        {
            //Debug.LogError($"ModelsManage.LoadSubSceneStart_3 resources:{resources} ");
            this.resources = context.GetService<IResources>();//跟ModelsLauncher.cs相关联
            yield return null;
        }

        //Debug.LogError($"ModelsManage.LoadSubSceneStart_4 resources:{resources} ");

        //2.加载建筑SubScene
        if (!MainManage.Instance.IsSubSceneLoadBuilding)
        {
            //不加载建筑，适用于快速调试ui
            InitFactoryInfo(() =>
            {
                if (callback != null) callback();
            });
            yield break;
        }
        else
        {
            Debug.Log("开始采用SubScene方式加载建筑");
            DateTime recordT = DateTime.Now;
            string log = "";
            // SubSceneShowManager manageT = SubSceneShowManager.Instance;
            // if(manageT)
            // {
            //     manageT.LoadUserBuildings((p) =>
            //     {
            //         var progress = p.progress;
            //         string percentTips = MainManage.Instance.IsSubSceneInnerLoad ? "(1/2)" : "";
            //         if (ProgressbarLoad.Instance) ProgressbarLoad.Instance.Show(progress, percentTips+"建筑外墙加载中...");                    
            //         var result = p.isAllFinished;
            //         if (result)
            //         {
            //             log += string.Format("LoadStartScens,CostTime:{0}s\n", (DateTime.Now - recordT).TotalSeconds);
            //             recordT = DateTime.Now;
            //             if (MainManage.Instance.IsSubSceneInnerLoad)
            //             {
            //                 manageT.LoadHiddenTreeNodes((p2) =>
            //                 {
            //                     if (!isSceneLoadFinish)
            //                     {
            //                         var progress2 = p2.progress;
            //                         if (ProgressbarLoad.Instance) ProgressbarLoad.Instance.Show(progress2, "(2/2)内部细节加载中...");
            //                         var result2 = p2.isAllFinished;
            //                         if (result2 == false && progress2 ==1)
            //                         {
            //                             Debug.LogError($"LoadSubSceneStart result2 == false && progress2 ==100:{progress2}");
            //                         }
            //                         Debug.Log($"LoadSubSceneStart (2/2)内部细节加载中...:{progress2} {result2} { progress2 == 1}");
            //                         if (result2 || progress2 ==1)
            //                         {
            //                             isSceneLoadFinish = true;
            //                             log += string.Format("LoadHiddenTreeNodes,CostTime:{0}s\n", (DateTime.Now - recordT).TotalSeconds);
            //                             recordT = DateTime.Now;
            //                             InitFactoryInfo(() =>
            //                             {
            //                                 if (ProgressbarLoad.Instance) ProgressbarLoad.Instance.Hide();
            //                                 log += string.Format("InitFactoryInfo,CostTime:{0}s\n", (DateTime.Now - recordT).TotalSeconds);
            //                                 Debug.Log("加载建筑及初始化建筑信息完成，耗时如下：\n" + log);
            //                                 if (callback != null) callback();
            //                             });
            //                         }
            //                     }
            //                     else
            //                     {
            //                         Debug.Log("Scene Load has finished...");
            //                     }
            //                 });
            //             }
            //             else
            //             {
            //                 InitFactoryInfo(() =>
            //                 {
            //                     if (ProgressbarLoad.Instance) ProgressbarLoad.Instance.Hide();
            //                     log += string.Format("InitFactoryInfo,CostTime:{0}s\n", (DateTime.Now - recordT).TotalSeconds);
            //                     Debug.LogError("加载建筑及初始化建筑信息完成，耗时如下：\n" + log);
            //                     if (callback != null) callback();
            //                 });
            //             }
            //         }
                    
                    
            //     });
            // }
        }
    }

    public IEnumerator AStart(Action callback)
    {
        CompleteCallBack = callback;
        ApplicationContext context = Context.GetApplicationContext();

        while (this.resources == null)
        {
            this.resources = context.GetService<IResources>();//跟ModelsLauncher.cs相关联
            yield return null;
        }

        Debug.LogError($"ModelsManage.InitResources1 resources:{resources}");

        //this.Load(new string[] { "LoxodonFramework/BundleExamples/Models/Red/Red.prefab", "LoxodonFramework/BundleExamples/Models/Green/Green.prefab" });
        //this.StartCoroutine(Load2("Models/Zibo/ZiBoFBX/ZiBoFBX.FBX")); 


        Allnum = CSVManage.Instance.StartLoadsModelList.Count + CSVManage.Instance.StartDiXinLoadsModelList.Count;
        loadedNum = 0;
        yield return this.StartCoroutine(LoadDiXin());
        //LoadBuilding();
        yield return this.StartCoroutine(LoadBuilding2());
        //ProgressbarLoad.Instance.Hide();
        if (ProgressbarLoadBottom.Instance) ProgressbarLoadBottom.Instance.Hide();
    }


    /// <summary>
    /// 加载建筑模型资源
    /// </summary>
    private void LoadBuilding()
    {
        List<string> buildnames = new List<string>();
        foreach (StartLoadsModel model in CSVManage.Instance.StartLoadsModelList)
        {
            buildnames.Add(model.PrefabPath);
        }

        //加载建筑
        this.Load(buildnames.ToArray(), (results) =>
        {
            AfterLoadBuilding(results);
        });

    }

    /// <summary>
    /// 加载建筑模型资源
    /// </summary>
    private IEnumerator LoadBuilding2()
    {
        DateTime start0 = DateTime.Now;
        Debug.Log("LoadBuilding2:"+ CSVManage.Instance.StartLoadsModelList.Count);
        // List<string> buildnames = new List<string>();
        // foreach (StartLoadsModel model in CSVManage.Instance.StartLoadsModelList)
        // {
        //     buildnames.Add(model.PrefabPath);
        // }

        List<GameObject> buildobjs = new List<GameObject>();
        //foreach (string buildname in buildnames)
        foreach (StartLoadsModel item in CSVManage.Instance.StartLoadsModelList)
        {
            StartLoadsModel model=item;
            string buildname=model.PrefabPath;
            DateTime start = DateTime.Now;
            if (!MainManage.Instance.IsResourcesLoadBuilding)
            {
                //yield return StartCoroutine(Load2(buildname, (obj) =>
                //{
                //    loadedNum++;
                //    if (ProgressbarLoadBottom.Instance) ProgressbarLoadBottom.Instance.Show((float)loadedNum / Allnum);
                //    GameObject o = GameObject.Instantiate(obj);
                //    buildobjs.Add(o);
                //    Debug.Log("AssetBundle加载：" + buildname);
                //}));

                MessageCenter.SendMsg(MsgType.LoadModelMsg.TypeName, MsgType.LoadModelMsg.OnBeforeLoadModel, buildname);
                yield return Load2(buildname, (obj) =>
                {
                    TimeSpan t = DateTime.Now - start;
                    Debug.Log("LoadBuilding2 Load2:" + buildname + "用时:" + t);
                    model.LoadTime=(int)t.TotalMilliseconds;

                    MessageCenter.SendMsg(MsgType.LoadModelMsg.TypeName, MsgType.LoadModelMsg.OnAfterLoadModel, obj);
                    loadedNum++;
                    if (ProgressbarLoadBottom.Instance) ProgressbarLoadBottom.Instance.Show((float)loadedNum / Allnum);
                    GameObject o = GameObject.Instantiate(obj);
                    buildobjs.Add(o);
                    Debug.Log("AssetBundle加载：" + buildname);
                });
            }
            else
            {
                string buildnameT = buildname.Substring(buildname.IndexOf(ResourcesRootName));
                buildnameT = buildnameT.Substring(0, buildnameT.IndexOf("."));
                //yield return StartCoroutine(RloadAsync<GameObject>(buildnameT, (obj) =>
                //{
                //    loadedNum++;
                //    if (ProgressbarLoadBottom.Instance) ProgressbarLoadBottom.Instance.Show((float)loadedNum / Allnum);
                //    if (obj)
                //    {
                //        GameObject o = GameObject.Instantiate(obj as GameObject);
                //        buildobjs.Add(o);
                //        InBuildingObjs inBuildingObjs = o.GetComponent<InBuildingObjs>();
                //        if (inBuildingObjs)
                //        {
                //            inBuildingObjs.LoadInBuildingObjs(null);//加载建筑内部细节
                //        }
                //    }
                //}));
                MessageCenter.SendMsg(MsgType.LoadModelMsg.TypeName, MsgType.LoadModelMsg.OnBeforeLoadModel, buildname);
                yield return RloadAsync<GameObject>(buildnameT, (obj) =>
                {
                    TimeSpan t = DateTime.Now - start;
                    Debug.Log("LoadBuilding RloadAsync:" + buildname + "用时:" + t);
                    model.LoadTime=(int)t.TotalMilliseconds;

                    MessageCenter.SendMsg(MsgType.LoadModelMsg.TypeName, MsgType.LoadModelMsg.OnAfterLoadModel, obj);
                    loadedNum++;
                    if (ProgressbarLoadBottom.Instance) ProgressbarLoadBottom.Instance.Show((float)loadedNum / Allnum);
                    if (obj)
                    {
                        GameObject o = GameObject.Instantiate(obj as GameObject);
                        buildobjs.Add(o);
                        InBuildingObjs inBuildingObjs = o.GetComponent<InBuildingObjs>();
                        if (inBuildingObjs)
                        {
                            inBuildingObjs.LoadInBuildingObjs(null);//加载建筑内部细节
                        }
                    }
                });
            }
            
        }
        AfterLoadBuilding(buildobjs.ToArray(), false);

        TimeSpan t0 = DateTime.Now - start0;
        Debug.Log("LoadBuilding2:总共用时:" + t0);
    }

    public GameObject[] ModelList;

    /// <summary>
    /// 加载到模型资源之后
    /// </summary>
    /// <param name="results"></param>
    private void AfterLoadBuilding(GameObject[] results, bool isprefab = true)
    {
        ModelList = results;
        Debug.LogError("AfterLoadBuilding,StartInit...");
        List<DepNode> ddepNodes = new List<DepNode>();

        foreach (GameObject template in results)
        {
            GameObject o = template;
            if (isprefab) o = GameObject.Instantiate(template);
            o.transform.SetParent(FactoryDepManager.Instance.Facotory.transform);
            DepNode nodet = o.GetComponent<DepNode>();
            if (nodet == null) continue;
            ddepNodes.Add(nodet);
        }
        Debug.LogError("RoomFactory.Instance.Init...");

        InitFactoryInfo();
        Debug.Log("ModelsManage.AfterLoadBuilding 建筑加载成功!");
    }

    private void InitFactoryInfo(Action onComplete=null)
    {
        RoomFactory.Instance.Init(() =>//第一步主要初始化，主要创建区域,建筑与数据节点绑定
        {
            try
            {
                // Debug.Log("RoomFactory.Instance.Init finish...");
                // FactoryDepManager.Instance.InitChildNodes();//园区与区域的关系节点绑定
                // RoomFactory.Instance.StoreDepInfo();//各节点间关系绑定
                // RoomFactory.Instance.StartBindingTopolgy();//第二步建筑与数据节点绑定
                // //if (PersonnelTreePanel.Instance) PersonnelTreePanel.Instance.InitTree();

                // //UIManager.GetInstance().ShowUIPanel(typeof(PersonnelTreePanel).Name);
                // Debug.Log("ModelsManage.InitFactoryInfo 建筑数据初始化成功!");
                // //if(FullViewController.Instance) FullViewController.Instance.EnterFactory();
                // if (BigScreenManager.Instance) BigScreenManager.Instance.SetBigSreenBySetting();

                if (CompleteCallBack != null)
                {
                    CompleteCallBack();
                }
                if (onComplete != null) onComplete();
            }
            catch (Exception e)
            {
                if (onComplete != null) onComplete();
                if (CompleteCallBack != null)
                {
                    CompleteCallBack();
                }
                Debug.LogError("拓扑树建筑相关绑定!:" + e.Message);
            }
        });
    }

    /// <summary>
    /// 加载地形
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadDiXin()
    {
        List<string> buildnames = new List<string>();
        foreach (StartDiXinLoadsModel model in CSVManage.Instance.StartDiXinLoadsModelList)
        {
            buildnames.Add(model.PrefabPath);
        }

        DateTime start0 = DateTime.Now;
        foreach (string buildname in buildnames)
        {
            DateTime start = DateTime.Now;
            if (!MainManage.Instance.IsResourcesLoadBuilding)
            {
                yield return StartCoroutine(Load2(buildname, (obj) =>
                {
                    loadedNum++;
                    if (ProgressbarLoadBottom.Instance) ProgressbarLoadBottom.Instance.Show((float)loadedNum / Allnum);
                    GameObject o = GameObject.Instantiate(obj);
                    o.transform.SetParent(FactoryDepManager.Instance.DX_Container);
                }));
            }
            else
            {
                string buildnameT = buildname.Substring(buildname.IndexOf(ResourcesRootName));
                buildnameT = buildnameT.Substring(0, buildnameT.IndexOf("."));
                yield return StartCoroutine(RloadAsync<GameObject>(buildnameT, (obj) =>
                {
                    loadedNum++;
                    if (ProgressbarLoadBottom.Instance) ProgressbarLoadBottom.Instance.Show((float)loadedNum / Allnum);
                    if (obj)
                    {
                        GameObject o = GameObject.Instantiate(obj as GameObject);
                        o.transform.SetParent(FactoryDepManager.Instance.DX_Container);
                    }
                }));
            }
            TimeSpan t = DateTime.Now - start;
            Debug.Log("LoadDiXin:"+ buildname+"用时:"+ t);
        }

        TimeSpan t0 = DateTime.Now - start0;
        Debug.Log("LoadDiXin:总共用时:" + t0);
    }

    /// <summary>
    /// Assetbundle多个加载
    /// </summary>
    /// <param name="name"></param>
    public void Load(string[] names, Action<GameObject[]> Callback)
    {
        IProgressResult<float, GameObject[]> result = resources.LoadAssetsAsync<GameObject>(names);
        result.Callbackable().OnProgressCallback(p =>
        {
            Debug.LogFormat("Progress:{0}%", p * 100);
        });
        result.Callbackable().OnCallback((r) =>
        {
            try
            {
                if (r.Exception != null)
                    throw r.Exception;

                //foreach (GameObject template in r.Result)
                //{
                //    GameObject o = GameObject.Instantiate(template);
                //}

                Callback(r.Result);

            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Load failure.Error:{0}", e);
            }
        });
    }

    /// <summary>
    /// Assetbundle单个加载
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IEnumerator Load2(string name, Action<GameObject> Callback)
    {
        IProgressResult<float, GameObject> result = resources.LoadAssetAsync<GameObject>(name);

        while (!result.IsDone)
        {
            //Debug.LogFormat("Progress:{0}%", result.Progress * 100);
            yield return null;
        }

        try
        {
            if (result.Exception != null)
                throw result.Exception;

            //GameObject.Instantiate(result.Result);
            Callback(result.Result);
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Load failure.Error:{0}", e);
        }
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="modelname"></param>
    /// <param name="Callback"></param>
    /// <returns></returns>
    public IEnumerator LoadObjectByFileName(string modelname, string suffixalName, Action<UnityEngine.Object> Callback)
    {
        //Debug.Log($"ModelsManage.LoadObjectByFileName1!!!!  modelname:{modelname} suffixalName:{suffixalName}");
        string modelFile = ImportedModelDb.GetModelFileEx(modelname);
        if (!string.IsNullOrEmpty(modelFile))
        {
            //string outputFile = ImportedModelDb.GetOutputFile(modelFile);
            DateTime startT = DateTime.Now;
            //Debug.Log($"ModelsManage.LoadObjectByFileName2!!!!  modelname:{modelname} modelFile:{modelFile}");
            ModelImporterClient.Instance.SetSettingToCurrentPos();
            bool isCompleted = false;
            GameObject resultModel = null;
            yield return ModelImporterClient.Instance.LoadModelWithBuffer(modelname,modelFile, obj=>
            {
                //Debug.Log($"ModelsManage.LoadObjectByFileName3!!!!  modelname:{modelname} modelFile:{modelFile} resultModel:{resultModel} time:{(DateTime.Now - startT).TotalMilliseconds:F1}ms [obj:{obj} {obj!=null}]");
                if (obj)
                {
                    //obj.SetActive(false);
                    //obj.name += "_LoadObjectByFileName3";
                }

                resultModel = obj;
                isCompleted = true;
            },false,"LoadObjectByFileName");
            while (isCompleted == false)
            {
                yield return new WaitForSeconds(0.05f);
            }
            //Debug.Log($"ModelsManage.LoadObjectByFileName4!!!! modelname:{modelname} modelFile:{modelFile} resultModel:{resultModel} time:{(DateTime.Now-startT).TotalMilliseconds:F1}ms");
            if (Callback != null)
            {
                Callback(resultModel);
            }
            yield break;
        }

        string fileName = modelname + suffixalName;
        var modelList=CSVManage.Instance.ModelNameToPathModelList;
        ModelNameToPathModel mdelNameToPathModel = modelList.Find((item) => item.FileName == fileName);
        if (mdelNameToPathModel == null)
        {
            if (suffixalName == AssetbundleGetSuffixalName.prefab)
            {
                fileName = modelname + ".3DS";
                mdelNameToPathModel = modelList.Find((item) => item.FileName == fileName);

                if (mdelNameToPathModel == null)
                {
                    fileName = modelname + ".3ds";
                    mdelNameToPathModel = modelList.Find((item) => item.FileName == fileName);

                    if (mdelNameToPathModel == null)
                    {
                        fileName = modelname + ".fbx";
                        mdelNameToPathModel = modelList.Find((item) => item.FileName == fileName);
                        if (mdelNameToPathModel == null)
                        {
                            fileName = modelname + ".FBX";
                            mdelNameToPathModel = modelList.Find((item) => item.FileName == fileName);
                        }
                    }
                }
            }
        }
        if (mdelNameToPathModel == null)
        {
            fileName = modelname + suffixalName;
            Debug.LogError($"ModelsManage.LoadObjectByFileName5!!!! 加载的资源未找到：{fileName} modelFile:{modelFile}");
            yield break;
        }
        Debug.LogFormat("LoadObjectByFileName1 Name:{0} ", mdelNameToPathModel.Path);
        yield return LoadObject(mdelNameToPathModel.Path, Callback);
    }

    public float TimeOut1 = 10;

    public float TimeOut2 = 60;

    public float TimeOut3 = 600;

    /// <summary>
    /// 加载资源
    /// </summary>
    public IEnumerator LoadObject(string nameT, Action<UnityEngine.Object> Callback)
    {
        //Debug.LogFormat("LoadObject_Start Name:{0} ", nameT);
        if (resources == null)
        {
            Debug.LogError($"资源加载器resources还未初始化！ nameT:{nameT}");
            Callback(null);
            yield break;
        }
        IProgressResult<float, UnityEngine.Object> result = resources.LoadAssetAsync<UnityEngine.Object>(nameT);
        DateTime loadStartTime = DateTime.Now;
        while (!result.IsDone)
        {
            //Debug.LogFormat("LoadObject Name:{0} Progress:{1}%", nameT, result.Progress * 100);
            if (result.Progress > 0.99f)
            {
                TimeSpan time = DateTime.Now- loadStartTime;
                if (time.TotalSeconds > TimeOut1)
                {
                    Debug.LogErrorFormat("LoadObject_TimeOut1 Name:{0} Progress:{1}%", nameT, result.Progress * 100);
                    break;
                }
            }
            else if (result.Progress > 0.9f)
            {
                TimeSpan time = DateTime.Now - loadStartTime;
                if (time.TotalSeconds > TimeOut2)
                {
                    Debug.LogErrorFormat("LoadObject_TimeOut2 Name:{0} Progress:{1}%", nameT, result.Progress * 100);
                    break;
                }
            }
            else
            {
                TimeSpan time = DateTime.Now - loadStartTime;
                if (time.TotalSeconds > TimeOut3)
                {
                    Debug.LogErrorFormat("LoadObject_TimeOut3 Name:{0} Progress:{1}%", nameT, result.Progress * 100);
                    break;
                }
            }
            yield return null;
        }

        try
        {
            if (result.Exception != null)
                throw result.Exception;

            //GameObject.Instantiate(result.Result);
            Callback(result.Result);
        }
        catch (Exception e)
        {
            Debug.LogError($"Load failure. nameT:{nameT} Error:{e}");
        }
    }

    public void Rload()
    {

    }

    /// <summary>
    /// Resources异步加载
    /// </summary>
    /// <param name="path"></param>
    /// <param name="Callback"></param>
    /// <returns></returns>
    public IEnumerator RloadAsync<T>(string path, Action<UnityEngine.Object> Callback) where T : UnityEngine.Object
    {

        ResourceRequest request = Resources.LoadAsync<T>(path);

        yield return request;

        Callback(request.asset);
        //Instantiate(request.asset);
    }
}
