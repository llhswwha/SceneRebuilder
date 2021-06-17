using Jacovone.AssetBundleMagic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetBundleInfo : MonoBehaviour {

    public string SceneName;

    public string AssetName;

    protected Action<DepNode> callback;

    public bool IsLoaded = false;

    public bool IsLoading = false;

    public int LoadCount = 0;

    public string ObjectInfo = "";

    public int instanceId = 0;
    public int hashCode = 0;

    [ContextMenu("TestLoadAsset")]
    public void TestLoadAsset()
    {
        LoadAsset(null, false);
    }

    [ContextMenu("TestLoadAndUnload")]
    public void TestLoadAndUnload()
    {
        LoadAsset((node)=>
        {
        }, false);
        UnloadAsset("TestLoadAndUnload");//在加载好之前马上卸载掉
    }

    public virtual bool LoadAsset(Action<DepNode> callback,bool isFocus=true)
    {
        if (IsLoading)
        {
            //Debug.Log("AssetBundleInfo.LoadAsset IsLoading:"+ AssetName);
            return false;
        }
        if (IsLoaded)
        {
            DepNode depNode = null;
            if (rootObjs.Length > 0)
            {
                GameObject rootObj = rootObjs[0];
                this.callback = null;//某个操作后，根据距离加载精细模型，就回掉到聚焦建筑去了（wk）
                depNode = LoadRootObject(rootObj, isFocus);//代码运行到子类实现中
                if(depNode!=null&& callback!=null)
                {
                    callback(depNode);
                }
            }
            else
            {
                Debug.LogError("rootObjs.Length == 0 ");
            }
            return false;
        }
        this.callback = callback;
        LoadCount++;
        hashCode = this.GetHashCode();
        instanceId = this.GetInstanceID();
        ObjectInfo += gameObject.GetObjectInfo()+"\nhash:"+this.hashCode+"\nid:"+ instanceId;
        Debug.LogError("BuildingBox.LoadBuilding:" + this);
        IsLoading = true;
        var cm = ChunkManager.Instance;
        if (cm == null)
        {
            Log.Error("ChunkManager.Instance is Null");
            return false;
        }
        if (SceneAssetManager.Instance)
            SceneAssetManager.Instance.SetChunkManager(cm);

        if (SceneName == "Devices_J1_F2")
        {
            int i = 0;
        }
        cm.LoadBundle(AssetName, SceneName, () =>
        {
            AfterLoadBundle(isFocus);
        });
        return true;
    }

    private void AfterLoadBundle(bool isFocus)
    {
        try
        {
            IsLoading = false;
            IsLoaded = true;
            Debug.Log("GetSceneByName:" + SceneName);
            var scene = SceneManager.GetSceneByName(SceneName);
            rootObjs = scene.GetRootGameObjects();
            DepNode depNode = null;
            if (rootObjs.Length > 0)
            {
                GameObject rootObj = rootObjs[0];
                depNode = LoadRootObject(rootObj, isFocus);//代码运行到子类实现中
            }
            else
            {
                Debug.LogError("rootObjs.Length == 0 ");
            }

            SceneAssetManager.Instance.AfterLoadAsset(this);
            SceneManager.UnloadSceneAsync(scene);
            //Hide();
            AfterLoad();
            if (callback != null)
            {               
                callback(depNode);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("AssetBundleInfo.AfterLoadBundle:" + ex);
        }
    }

    protected virtual DepNode LoadRootObject(GameObject obj,bool isFocus=true)
    {
        return null;
    }

    public GameObject[] rootObjs;

    protected virtual void AfterLoad()
    {

    }


    public virtual void SetUnload(bool isImmediate)
    {
        SceneAssetManager.Instance.BeforeUnloadAsset(this, isImmediate);//这里不直接卸载
    }

    [ContextMenu("TestUnloadAsset")]
    public void TestUnloadAsset()
    {
        UnloadAsset("TestUnloadAsset");
    }

    public virtual BuildingController UnloadAsset(string tag)
    {
        if (IsLoaded == false)
        {
            Debug.Log("BuildingBox.UnloadAsset IsLoaded == false:" + this);
            return null;
        }
        IsLoaded = false;
        //GameObject.DestroyImmediate(buildingController.gameObject);
        if (rootObjs != null)
        {
            foreach (var item in rootObjs)
            {
                if (item == null) continue;
                GameObject.DestroyImmediate(item);
            }
            rootObjs = null;
        }
        else
        {
            Debug.LogError("AssetBundleInfo.UnloadAsset rootObjs == null");
        }
        AssetBundleMagic.UnloadBundleEx(AssetName);
        return null;
    }

    /// <summary>
    /// 加载后就不卸载了 S1
    /// </summary>
    public bool IsNotUnload = false;

    //public void OnDestroy()
    //{
    //    UnloadAsset();
    //}
}
