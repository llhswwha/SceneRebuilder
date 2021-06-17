using Jacovone.AssetBundleMagic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAssetLoader : MonoBehaviour {

    public static ModelAssetLoader Instance;

    void Awake()
    {
        Instance = this;
    }

    public ChunkManager ChunkManager;

    [ContextMenu("LoadModelAssets")]
    public void LoadModelAssets()
    {
        if (ChunkManager == null)
        {
            ChunkManager = ChunkManager.Instance;
        }
        if (ChunkManager == null)
        {
            Debug.LogError("ModelAssetLoader.LoadModelAssets ChunkManager == null");
            return;
        }
        allStart= DateTime.Now;
        LoadLog = "";

        //加载简化模型，在该脚本被加载时
        LoadSimpleModel(0,()=>
        {
            DeviceAssetInfo assetInfo = gameObject.GetComponentInParent<DeviceAssetInfo>();
            if (assetInfo)
            {
                assetInfo.AddScprits(transform);
            }
            else
            {
                Debug.LogError("DeviceAssetInfo == null");
            }

            if (SceneAssetManager.Instance)
            {
                SceneAssetManager.Instance.ShowVertexs();
                SceneAssetManager.Instance.EnableLoadDevices = true;
            }
        });
    }

    void OnDestroy()
    {
        if (SceneAssetManager.Instance)
        {
            SceneAssetManager.Instance.ShowVertexs();
            SceneAssetManager.Instance.EnableLoadDevices = false;
        }
    }

    private void SetProgress(int index)
    {
        float progress = (float)index / (float)modelList.Count;
        Debug.Log("SetProgress:"+progress);
        if (LoadingIndicatorScript.Instance)
        {
            LoadingIndicatorScript.Instance.SetProgress(progress);
        }
        else
        {
            Debug.LogError("LoadingIndicatorScript.Instance==null");
        }
    }

    public string Prefix = "J1Devices/";

    public int Resolution = 0;//0 512 1024 2048 4096 8192

    private string LoadLog = "";

    private DateTime allStart;

    private string GetFullBundleName(string n)
    {
        var bundleName = n;
        if (Resolution != 0)
        {
            bundleName = Prefix + Resolution + "/" + n;//"J1Devices/1024/j1_f1_1"
        }
        else
        {
            bundleName = Prefix + n;//"J1Devices/j1_f1_1"
        }
        return bundleName;
    }

    /// <summary>
    /// 加载简化模型，在该脚本被加载时
    /// </summary>
    /// <param name="index"></param>
    /// <param name="finished"></param>
    private void LoadSimpleModel(int index,Action finished)
    {
        Debug.Log("LoadModel :" + index);
        SetProgress(index);
        if (index >= modelList.Count)
        {
            TimeSpan time = DateTime.Now - allStart;
            Debug.Log("LoadModel Finished time:"+ time);
            Debug.Log(LoadLog);
            if (finished != null)
            {
                finished();
            }
            return;
        }
        var item = modelList[index];
        DateTime start = DateTime.Now;
        var bundleName = GetFullBundleName(item.GetSimpleBundleName());
        ChunkManager.LoadBundle(bundleName, item.assetName, obj =>
        {
            Debug.Log("LoadBundle :" + obj);
            if (obj != null)
            {
                item.isLoaded = true;
                var parent = transform.FindChildByName(item.parentName);
                var instance = GameObject.Instantiate(obj, parent);
                instance.name = obj.name;

                DeviceModelInfo info=instance.AddComponent<DeviceModelInfo>();
                info.ModelInfo = item;
                //if (item.isSimple == false)
                //{
                //    info.IsLoaded = true;//有些设备不是简化模型，直接就是原模型，不需要再加载精细模型
                //}
            }
            else
            {

            }

            TimeSpan time = DateTime.Now - start;
            string log= string.Format("{0}:{1}", index, time);
            LoadLog += log += "\n";
            LoadSimpleModel(index+1, finished);
        });
    }

    /// <summary>
    /// 加载复杂模型，在距离够近时
    /// </summary>
    /// <param name="item"></param>
    /// <param name="finished"></param>
    public void LoadComplexModel(ModelInfo item,Action<GameObject> finished)
    {
        DateTime start = DateTime.Now;
        var bundleName = GetFullBundleName(item.GetBundleName());
        ChunkManager.LoadBundle(bundleName, item.assetName, obj =>
        {
            Debug.Log("LoadBundle :" + obj);
            GameObject instance = null;
            if (obj != null)
            {
                item.isLoaded = true;
                var parent = transform.FindChildByName(item.parentName);
                instance = GameObject.Instantiate(obj, parent);
                instance.name = obj.name;

                //DeviceModelInfo info = instance.AddComponent<DeviceModelInfo>();
                //info.ModelInfo = item;
            }
            else
            {

            }

            TimeSpan time = DateTime.Now - start;
            string log = string.Format("{0}:{1}", item.GetBundleName(), time);
            //LoadLog += log += "\n";
            //LoadModel(index + 1);
            Debug.Log(log);
            if (finished != null)
            {
                finished(instance);
            }

            if (LoadingIndicatorScript.Instance)
            {
                LoadingIndicatorScript.Instance.SetProgress(1);
            }
        });
    }

    public bool UnloadModel(ModelInfo item)
    {
        return UnloadModel(item.GetBundleName());
    }

    public void UnloadModels(ModelInfo item)
    {
        var bundleNames = item.GetBundleNames();
        foreach(var bundle in bundleNames)
        {
            UnloadModel(bundle);
        }
    }

    public bool UnloadModel(string modelName)
    {
        var bundleName = GetFullBundleName(modelName);
        if (AssetBundleMagic.ContainsBundle(bundleName))
        {
            return AssetBundleMagic.UnloadBundleEx(bundleName);
        }
        else
        {
            //两个模型公用一个bundle
            return true;
        }
    }

    public bool AutoLoad = true;

    public void Start()
    {
        if (AutoLoad)
        {
            LoadModelAssets();
        }
    }

    public List<ModelInfo> modelList = new List<ModelInfo>();

}

[Serializable]
public class ModelInfo
{
    public string assetName;
    public string bundleName;
    public string parentName;
    /// <summary>
    /// 是否存在简化模型
    /// </summary>
    public bool isSimple;
    /// <summary>
    /// 简化模型是否已经加载了
    /// </summary>
    public bool isLoaded;

    public string GetBundleName()
    {
        return bundleName;
    }

    public string GetSimpleBundleName()
    {
        if (isSimple)//有简化模型
        {
            return bundleName + "_s";
        }
        else//不存在简化模型（因为模型本身点面就少）
        {
            return bundleName;
        }
    }

    public string[] GetBundleNames()
    {
        return new string[] { bundleName, bundleName + "_s" };
    }
}
