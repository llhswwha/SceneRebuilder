using System;
using System.Collections;
using System.Collections.Generic;
//using Base.Common;
using Unity.ComnLib.Consts;
using UnityEngine;
using Object = UnityEngine.Object;

public static class AssetbundleGetSuffixalName
{
    public const string prefab = ".prefab";
    public const string png = ".png";
    public const string jpg = ".jpg";
    /// <summary>
    /// 用于缩略图
    /// </summary>
    public const string png2 = "_t.png";
    /// <summary>
    /// 用于材质
    /// </summary>
    public const string mat = ".mat";
}

public static class AssetBundleHelper
{
    //public static IEnumerator LoadAssetObject(string txt, Action<Object> createAction = null)
    //{
    //    return LoadAssetObject<Object>(txt, createAction);
    //}

    public static Dictionary<string, Object> models = new Dictionary<string, Object>();

    public static List<string> names = new List<string>();

    //public static IEnumerator LoadAssetGameObject(string txt, Action<GameObject> createAction = null)
    //{
    //    //LogInfo.Info("AssetBundleHelper.LoadAssetGameObject","txt:" + txt);
    //    if (unityType == UnityType.UNITY_EDITOR_ANDROID || unityType == UnityType.UNITY_ANDROID || unityType == UnityType.UNITY_WEBGL)
    //    {
    //        //txt = PinYinConverter.Get(txt);//android打包Assets时不能有中文字符
    //    }
    //    //var path = GetAssetPath();
    //    string path = GetAssetPath();
    //    //LogInfo.Info("AssetBundleHelper.LoadAssetGameObject","path:" + path);
    //    yield return LoadAssetObjectWithNoDep(path, txt, AssetbundleGetSuffixalName.prefab,createAction);

    //    //Debug.LogInfo("LoadAssetGameObject:" + txt);
    //    //if (names.Contains(txt))
    //    //{
    //    //    while (!models.ContainsKey(txt))
    //    //        //等待同名的资源被加载,
    //    //        //因为已经被加载的资源,不能重复加载，
    //    //        //因为yield return的关系，可能在之前模型还没被获取的时候就又再次进来获取相同的模型了。
    //    //    {
    //    //        Debug.LogInfo("Wait LoadAssetGameObject:" + txt);
    //    //        yield return null;
    //    //        //yield return new WaitForEndOfFrame();
    //    //    }
    //    //    yield return models[txt];
    //    //}
    //    //else
    //    //{
    //    //    names.Add(txt);

    //    //    //不同平台下StreamingAssets的路径是不同的，这里需要注意一下。
    //    //    var path = GetAssetPath();
    //    //    string url = path + txt;
    //    //    WWW www = WWW.LoadFromCacheOrDownload(url, 5);
    //    //    yield return www;
    //    //    if (www.error != null)
    //    //    {
    //    //        //Could not resolve host的话是路径有问题
    //    //        Debug.LogError(www.error + ":" + url + "|" + txt + "|" + path);
    //    //        models.Add(txt, null);
    //    //        yield return null;
    //    //    }
    //    //    else
    //    //    {
    //    //        AssetBundle bundle = www.assetBundle;
    //    //        //LogInfo("bundle:"+bundle);
    //    //        yield return bundle;
    //    //        GameObject obj = bundle.LoadAsset<GameObject>(txt);
    //    //        if (obj == null)
    //    //        {
    //    //            models.Add(txt, null);
    //    //            Debug.LogError("加载的资源为空:" + url + "|" + txt);
    //    //            yield return null;
    //    //        }
    //    //        else
    //    //        {
    //    //            //T instance = GameObject.Instantiate(obj);
    //    //            GameObject go = (obj as GameObject);
    //    //            go.tag = Tags.Prefab;
    //    //            go.transform.position = new Vector3(1000, 1000, 1000);
    //    //            if (createAction != null)
    //    //            {
    //    //                createAction(obj);
    //    //            }
    //    //            models.Add(txt, obj);
    //    //            //instance.name = txt;

    //    //            bundle.Unload(false);
    //    //            Debug.LogInfo("Instantiate:" + obj);
    //    //            yield return obj;
    //    //        }
    //    //    }
    //    //}
    //}


    private static string GetAssetPath3()
    {
        string path =
#if UNITY_ANDROID
            		    "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
            	    Application.dataPath + "/Raw/";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            "file://" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_WEBPLAYER
                     Application.dataPath + "/StreamingAssets/";
#else
            string.Empty;
#endif
        return path;
    }

    private static string _assetPath = "";

    public static string GetAssetPath()
    {
        //if (string.IsNullOrEmpty(_assetPath))
        //{
        //    _assetPath = GetAssetPath(unityType);
        //}
        //return _assetPath;
        var path = GetAssetPath(unityType);
        if (IsFromHttp)
            path = HttpUrl + "/";
        return path + CurrentPlatformString()+"/";
    }

    /// <summary>
    /// Return a string representing the current application platform. Used to compose
    /// directory name when create bundles, and when search for bundles locally.
    /// </summary>
    /// <returns></returns>
    private static string CurrentPlatformString()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
                return "macOS";
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return "Windows";
            ///增加webgl
            case RuntimePlatform.WebGLPlayer:
                return "Windows";
            default:
                //  return "unknow";
                //增加webgl后unknown下没有模型写成Windows
                return "Windows";
        }
    }

    public static string GetAssetPath(UnityType ut)
    {
        string path = "";
        if (ut == UnityType.UNITY_EDITOR_ANDROID)
            path = "file://" + Application.dataPath + "/StreamingAssets/";
        else if (ut == UnityType.UNITY_ANDROID)
            path = "jar:file://" + Application.dataPath + "!/assets/";
        else if (ut == UnityType.UNITY_ANDROID)
            path = Application.dataPath + "/Raw/";
        else if (ut == UnityType.UNITY_STANDALONE_WIN || ut == UnityType.UNITY_EDITOR)
            path = "file://" + Application.dataPath + "/StreamingAssets/";
        else if (ut == UnityType.UNITY_WEBGL)
            path = Application.streamingAssetsPath+"/";
        else if (ut == UnityType.UNITY_WEBPLAYER)
            path = Application.streamingAssetsPath + "/";
        return path;
    }


    //private static string GetAssetPath2(string fileName)
    //{
    //    //不同平台下StreamingAssets的路径是不同的，这里需要注意一下。
    //    string path = "";
    //    if (_unityType == UnityType.UNITY_ANDROID)
    //        path = "jar:file://" + Application.dataPath + "!/assets/";
    //    else if (_unityType == UnityType.UNITY_ANDROID)
    //        path = Application.dataPath + "/Raw/";
    //    else if (_unityType == UnityType.UNITY_STANDALONE_WIN || _unityType == UnityType.UNITY_EDITOR)
    //        path = "file://" + Application.dataPath + "/" + fileName + "/";
    //    else if (_unityType == UnityType.UNITY_WEBGL)
    //        path = Application.dataPath + "/" + fileName + "/";
    //    else if (_unityType == UnityType.UNITY_WEBPLAYER)
    //        path = Application.dataPath + "/StreamingAssets/" + "3DModels/";
    //    return path;
    //}

    public static string HttpUrl = "";

    public static bool IsFromHttp = false;

    public static UnityType unityType = UnityType.UNITY_STANDALONE_WIN;

    public static void SetUnityType(UnityType type)
    {
        unityType = type;
        _assetPath = "";
    }


    // 已解压的Asset列表 [prefabPath, asset]
    private static Dictionary<string, UnityEngine.Object> dicAsset = new Dictionary<string, UnityEngine.Object>();
    // "正在"加载的资源列表 [prefabPath, www]
    private static Dictionary<string, WWW> dicLoadingWWW = new Dictionary<string, WWW>();
    private static Dictionary<string, AssetBundle> dicAssetBundle = new Dictionary<string, AssetBundle>(); //对应物体的依赖资源

    //public static WWW wwwManifest = null; //总的Manifest

    private static Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>(); //对应物体的依赖资源

    ///// <summary>
    ///// 读取一个资源,不需要加载依赖文件的使用该方法
    ///// </summary>
    //public static IEnumerator LoadAssetObjectWithNoDep<T>(string ppath, string fileName, string loadName,
    //    string suffixalName, Action<T> action) where T : Object
    //{
    //    var path = GetAssetPath();
    //    return LoadAssetObjectWithNoDep<T>(path, loadName, suffixalName, action);
    //}

    /// <summary>
    /// 读取一个资源,不需要加载依赖文件的使用该方法
    /// </summary>
    public static IEnumerator LoadAssetObject<T>(string subDir,string loadName,
        string suffixalName, Action<T> action) where T : Object
    {
        var path = GetAssetPath()+subDir+"/";
        return LoadAssetObjectWithNoDep<T>(path, loadName, loadName, suffixalName, action);
    }

    /// <summary>
    /// 读取一个资源,不需要加载依赖文件的使用该方法
    /// </summary>
    public static IEnumerator LoadAssetObject<T>(string ppath,string fileName, string loadName,
        string suffixalName, Action<T> action) where T : Object
    {
        var path = GetAssetPath();
        return LoadAssetObjectWithNoDep<T>(path, fileName, loadName, suffixalName, action);
    }

    /// <summary>
    /// 读取一个资源,不需要加载依赖文件的使用该方法
    /// </summary>
    private static IEnumerator LoadAssetObjectWithNoDep<T>(string path,string fileName, string loadName,string suffixalName, Action<T> action) where T :Object
    {
        //默认情况下fileName和loadName相同，但是也可以不同
        //LogInfo.Info("LoadAssetObjectWithNoDep",string.Format("{0}|{1}|{2}",path, loadName,suffixalName));
        if (loadName.IndexOf("机柜") >= 0)
        {
            int i = 0;
        }
        if (loadName.IndexOf("机柜_600X1200X2200_单门_黑色") >= 0)
        {
            int i = 0;
        }
        WWW www2 = null;
        T obj = null;
        string fullname = loadName + suffixalName;
        if (!dicLoadingWWW.ContainsKey(fullname))
        {            
            if (dicAsset.ContainsKey(fullname))
            {
                obj = dicAsset[fullname] as T;
                if (action != null)
                {
                    Debug.Log(string.Format("加载模型{0}, result:{1}",fullname,obj));
                    action(obj);
                    yield break;//已经加载到缓存中，退出
                }
            }
            //加载资源
            //www2 = WWW.LoadFromCacheOrDownload(path + loadName, 0);
            www2 =new WWW(path + fileName);//加载资源
            dicLoadingWWW.Add(fullname, www2);//加入到正在加载中的资源
            while (www2.isDone == false)//等待加载完毕
            {
                yield return null;
            }
        }
        else//是加入到正在加载中的资源
        {
            DateTime waitStartTime = DateTime.Now;
            float waitTimeOut = 5f;
            while (true)
            {
                if((DateTime.Now-waitStartTime).TotalSeconds>waitTimeOut)
                {
                    Debug.Log("[AssetBundleHelper.LoadAssetObjectWithNoDep]加载AssetBundle等待超时");
                    if(action!=null)
                    {
                        action(null);
                        yield break;
                    }
                }
                if (dicAsset.ContainsKey(fullname))
                {                    
                    obj = dicAsset[fullname] as T;
                    if (action != null)
                    {
                        action(obj);
                        yield break;
                    }
                }
                //else
                //{
                //    Debug.LogInfo("加入到正在加载中的资源 2");
                //    if (action != null)
                //    {
                //        action(null);
                //        yield break;
                //    }
                //}
                yield return null;//等待资源加载完毕
            }
        }
        if (loadName.IndexOf("机柜_600X1200X2200_单门_黑色") >= 0)
        {
            int i = 0;
        }
        if (www2.error == null)//加载成功
        {
            AssetBundle assetBundle = www2.assetBundle;
            if (assetBundle == null)
            {
                //assetBundle.Unload(false);
                yield break;//不存在assetBundle
            }

            obj = LoadAssetWithSffixalName(assetBundle, loadName, suffixalName) as T;//实际价值模型的地方

            //AssetBundleRequest assetBundleRequest = LoadAssetWithSffixalNameAsync(assetBundle, loadName, suffixalName);
            //if (assetBundleRequest != null)
            //{
            //    while (assetBundleRequest.isDone == false)
            //        yield return null;
            //    obj = assetBundleRequest.asset as T ;
            //    AfterLoadAsset(fullname, obj);
            //}
            //else
            //{
            //    yield return null;
            //}

            if (obj == null)
            {
                string[] assetNames=assetBundle.GetAllAssetNames();
                LogInfo.Alarm("LoadAssetObjectWithNoDep", "obj==null!!!\n"+ assetNames);
                //没有获取想要的模型，打印AssetBundle的全部内容。
            }

            assetBundle.Unload(false); //从内存中卸载assetBundle

            if (action != null)
            {
                action(obj);//处理加载后的模型
            }
        }
        else//加载失败，文件不存在之类的原因
        {
            Debug.LogError(www2.error+"\n:"+ path + loadName);
            if (action != null)
            {
                action(null);
            }
        }
    }

    /// <summary>
    /// 读取一个资源,不需要加载依赖文件的使用该方法
    /// </summary>
    /// <returns></returns>
    public static IEnumerator LoadAssetObject(string subDir, string loadName,
        string suffixalName, Action<Object> action)
    {
        return LoadAssetObject<Object>(subDir, loadName, suffixalName, action);
    }

    /// <summary>
    /// 读取一个资源,不需要加载依赖文件的使用该方法
    /// </summary>
    /// <returns></returns>
    public static IEnumerator LoadAssetObject(string path, string fileName, string loadName,
        string suffixalName, Action<Object> action)
    {
        return LoadAssetObject<Object>(path, fileName,loadName, suffixalName, action);
    }

    /// <summary>
    /// 加载根据后缀名
    /// </summary>
    /// <param name="assetBundle"></param>
    /// <param name="loadName"></param>
    /// <param name="suffixalName"></param>
    /// <returns></returns>
    private static Object LoadAssetWithSffixalName(AssetBundle assetBundle, string loadName, string suffixalName)
    {
        //Debug.LogInfo("AssetBundleHelper.LoadAssetWithSffixalName:"+loadName+"|"+suffixalName);
        Object obj = null;
        string fullname = loadName + suffixalName;
        string sufName = suffixalName.ToLower();
        if (sufName == AssetbundleGetSuffixalName.prefab)
        {
            obj = assetBundle.LoadAssetAsync(fullname, typeof (GameObject)).asset;//有预设加载预设
            if (obj == null)
            {
                obj = assetBundle.LoadAssetAsync(loadName + ".3ds", typeof (GameObject)).asset;//没有预设加载3ds模型
            }
            if (obj == null)
            {
                obj = assetBundle.LoadAssetAsync(loadName + ".fbx", typeof (GameObject)).asset;//3ds模型也没有的话试试fbx模型
            }
            if (obj == null)
            {
                obj = assetBundle.LoadAssetAsync(loadName + ".fbs", typeof(GameObject)).asset;//3ds模型也没有的话试试fbs模型
            }
            if (obj == null)
            {
                Debug.Log("obj == null:"+ loadName+"|"+ fullname);
                GameObject[] assets=assetBundle.LoadAllAssets<GameObject>();
                foreach (GameObject ass in assets)
                {
                    Debug.Log("ass:" + ass + "|" + ass.name);
                }

                //try
                //{
                //    foreach (GameObject ass in assets)
                //    {
                //        Debug.Log("ass:" + ass + "|" + ass.name + "|" + PinYinConverter.Get(ass.name));
                //        if (PinYinConverter.Get(ass.name).ToLower() == loadName)
                //        {
                //            obj = ass;
                //            break;
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Debug.Log("Exception:" + ex);
                //}
                
                if (obj == null && assets.Length>0)
                {
                    Debug.Log("obj == null.Use Index 0 Asset");
                    obj = assets[0];
                }
            }
        }
        else if (sufName == AssetbundleGetSuffixalName.png)//读取图片
        {
            //obj = assetBundle.LoadAsset(fullname, typeof (Sprite));
            obj = assetBundle.LoadAssetAsync(fullname, typeof (Sprite)).asset;
            if (obj == null)
            {
                obj = assetBundle.LoadAssetAsync(fullname, typeof(Texture)).asset;
                if (obj is Texture2D)
                {
                    Texture2D texture2D = (obj as Texture2D);
                    float w = texture2D.width;
                    float h = texture2D.height;
                    obj = Sprite.Create(texture2D, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));
                }
            }

            if (obj == null)
            {
                obj = assetBundle.LoadAssetAsync(loadName + AssetbundleGetSuffixalName.jpg, typeof(Sprite)).asset;
            }
            //print("");
        }
        else if (sufName == AssetbundleGetSuffixalName.png2)
        {
            obj = assetBundle.LoadAssetAsync(fullname, typeof (Sprite)).asset;
        }
        else if (sufName == AssetbundleGetSuffixalName.mat)
        {
            obj = assetBundle.LoadAssetAsync(fullname, typeof (Material)).asset;
        }

        AfterLoadAsset(fullname, obj);
        return obj;
    }

    private static AssetBundleRequest LoadAssetWithSffixalNameAsync(AssetBundle assetBundle, string loadName, string suffixalName)
    {
        
        AssetBundleRequest assetBundleRequest = null;
        string fullname = loadName + suffixalName;
        string sufName = suffixalName.ToLower();
        if (sufName == AssetbundleGetSuffixalName.prefab)
        {
            if (assetBundle.Contains(fullname))
            {
                assetBundleRequest = assetBundle.LoadAssetAsync(fullname, typeof (GameObject)); //有预设加载预设
            }
            else if (assetBundle.Contains(loadName + ".3ds"))
            {
                assetBundleRequest = assetBundle.LoadAssetAsync(loadName + ".3ds", typeof(GameObject));//没有预设加载3ds模型
            }
            else if (assetBundle.Contains(loadName + ".fbs"))
            {
                assetBundleRequest = assetBundle.LoadAssetAsync(loadName + ".fbs", typeof(GameObject));//3ds模型也没有的话试试fbs模型
            }
            else if (assetBundle.Contains(loadName + ".fbx"))
            {
                assetBundleRequest = assetBundle.LoadAssetAsync(loadName + ".fbx", typeof(GameObject));//3ds模型也没有的话试试fbs模型
            }
        }
        else if (sufName == AssetbundleGetSuffixalName.png)//读取图片
        {
            assetBundleRequest = assetBundle.LoadAssetAsync(fullname, typeof(Sprite));
            if (assetBundleRequest == null)
            {
                assetBundleRequest = assetBundle.LoadAssetAsync(fullname, typeof(Texture));
                //if (assetBundleRequest is Texture2D)
                //{
                //    Texture2D texture2D = (assetBundleRequest as Texture2D);
                //    float w = texture2D.width;
                //    float h = texture2D.height;
                //    assetBundleRequest = Sprite.Create(texture2D, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));
                //}
            }
            //print("");
        }
        else if (sufName == AssetbundleGetSuffixalName.png2)
        {
            assetBundleRequest = assetBundle.LoadAssetAsync(fullname, typeof(Sprite));
        }
        else if (sufName == AssetbundleGetSuffixalName.mat)
        {
            assetBundleRequest = assetBundle.LoadAssetAsync(fullname, typeof(Material));
        }

        return assetBundleRequest;
    }

    private static void AfterLoadAsset(string fullname, Object obj)
    {
        dicLoadingWWW[fullname].Dispose();
        dicLoadingWWW.Remove(fullname);

        if (!dicAsset.ContainsKey(fullname))
        {
            dicAsset.Add(fullname, obj);
        }
    }

    ///// <summary>
    ///// 读取一个资源，加载依赖文件（先不用该方法）
    ///// </summary>
    ///// <returns></returns>
    //public static IEnumerator LoadAssetGameObject7(string ppath, string fileName, string loadName, string suffixalName, Action<Object> action)
    //{
    //    //不同平台下StreamingAssets的路径是不同的，这里需要注意一下。
    //    string path = GetAssetPath2(fileName);
    //    //Caching.CleanCache();
    //    string manifestPath = path + fileName; //fileName是总的Manifest的名称
    //    AssetBundle manifestBundle = null;
    //    WWW wwwManifest = null;
    //    if (!dicAssetBundle.ContainsKey(fileName))
    //    {
    //        if (!dicLoadingWWW.ContainsKey(fileName))
    //        {
    //            wwwManifest = WWW.LoadFromCacheOrDownload(manifestPath, 0);
    //            dicLoadingWWW.Add(fileName, wwwManifest);
    //            yield return null;
    //        }
    //        while (dicLoadingWWW[fileName].isDone == false)
    //        {
    //            yield return null;
    //        }
    //        if (dicLoadingWWW[fileName].error == null)
    //        {
    //            if (!dicAssetBundle.ContainsKey(fileName))
    //            {
    //                dicAssetBundle.Add(fileName, dicLoadingWWW[fileName].assetBundle);
    //                dicLoadingWWW[fileName].Dispose();
    //                dicLoadingWWW.Remove(fileName);
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogInfo(dicLoadingWWW[fileName].error);
    //            yield break;
    //        }
    //    }
    //    manifestBundle = dicAssetBundle[fileName]; // 总的assetBundle
    //    AssetBundleManifest manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
    //    //总的manifest//AssetBundleManifest
    //    //然后根据我们需要加载的资源名称，获得所有依赖资源：
    //    string[] dependentAssetBundles = manifest.GetAllDependencies(loadName);
    //    AssetBundle[] abs = new AssetBundle[dependentAssetBundles.Length];

    //    for (int i = 0; i < dependentAssetBundles.Length; i++)
    //    {
    //        WWW www = WWW.LoadFromCacheOrDownload(path + dependentAssetBundles[i], 0);
    //        yield return null;
    //        abs[i] = www.assetBundle;
    //    }

    //    WWW www2 = null;
    //    UnityEngine.Object obj = null;
    //    string fullname = loadName + suffixalName;

    //    if (!dicLoadingWWW.ContainsKey(fullname))
    //    {
    //        if (dicAsset.ContainsKey(fullname))
    //        {
    //            obj = dicAsset[fullname];
    //            if (action != null)
    //            {
    //                action(obj);
    //                yield break;
    //            }
    //        }
    //        //加载资源
    //        www2 = WWW.LoadFromCacheOrDownload(path + loadName, 0);
    //        dicLoadingWWW.Add(fullname, www2);
    //        while (www2.isDone == false)
    //        {
    //            yield return null;
    //        }
    //    }
    //    else
    //    {
    //        while (true)
    //        {
    //            if (dicAsset.ContainsKey(fullname))
    //            {
    //                obj = dicAsset[fullname];
    //                if (action != null)
    //                {
    //                    action(obj);
    //                    yield break;
    //                }
    //            }
    //            yield return null;
    //        }
    //    }

    //    if (www2.error == null)
    //    {
    //        AssetBundle assetBundle = www2.assetBundle;
    //        if (assetBundle == null)
    //        {
    //            assetBundle.Unload(false);
    //            yield break;
    //        }
    //        obj = LoadAssetWithSffixalName(assetBundle, loadName, suffixalName);

    //        assetBundle.Unload(false); //从内存中卸载assetBundle

    //        if (action != null)
    //        {
    //            action(obj);
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogInfo(www2.error);
    //    }

    //}
}
