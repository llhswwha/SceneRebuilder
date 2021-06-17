using HighlightingSystem;
using Jacovone.AssetBundleMagic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单个主厂房设备模型
/// </summary>
public class DeviceModelInfo : MonoBehaviour {

    public static List<DeviceModelInfo> Buffer = new List<DeviceModelInfo>();

    public ModelInfo ModelInfo;

    public DistanceChecker checher;

    [ContextMenu("InitDistanceBounds")]
    private void InitDistanceBounds()
    {
        if (checher == null)
        {
            checher = gameObject.AddComponent<DistanceChecker>();
            checher.roamDistancePower = 3.5f;
            checher.roamDistanceOutPower = 4;
            checher.freeDistancePower = 3.6f;
            checher.InitDistanceBounds();
        }
    }

    void Awake()
    {
        Buffer.Add(this);
    }

    // Use this for initialization
    void Start()
    {
        InitDistanceBounds();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    private MeshRenderer meshRenderer;

    public bool IsInDistance(Transform t, bool isRoam)
    {
        return checher.IsInDistance(t, isRoam, true);
    }

    public bool IsOutDistance(Transform t, bool isRoam)
    {
        return checher.IsOutDistance(t, isRoam,true);
    }

    /// <summary>
    /// 精细模型是否有加载
    /// </summary>
    public bool IsLoaded = false;

    public bool IsLoading = false;

    public bool LoadAsset(Action finished)
    {
        if (ModelInfo.isSimple == false)//不存在简化模型，也就不用加载精细模型了
        {
            return true;
        }
        if (IsLoading)
        {
            return false;
        }
        if (IsLoaded)
        {
            return false;
        }
        IsLoading = true;
        var cm = ChunkManager.Instance;
        if (cm == null)
        {
            Log.Error("ChunkManager.Instance is Null");
            return false;
        }

        if (LoadingIndicatorScript.Instance)
        {
            //LoadingIndicatorScript.Instance.IsWait = false;
            //LoadingIndicatorScript.Instance.HaveShade = false;
            LoadingIndicatorScript.Instance.Enabled = false;
        }
        if (ModelAssetLoader.Instance)
            ModelAssetLoader.Instance.LoadComplexModel(ModelInfo, (obj) =>
             {
                 IsLoading = false;
                 IsLoaded = true;

                 modelObject = obj;
                 if (obj != null)
                 {
                     obj.transform.SetParent(transform, true);
                 }
                 SetNormalMeshHighlight(false);
                 if (finished != null)
                 {
                     finished();
                 }
                 if (LoadingIndicatorScript.Instance)
                 {
                     //LoadingIndicatorScript.Instance.IsWait = true;
                     //LoadingIndicatorScript.Instance.HaveShade = true;
                     LoadingIndicatorScript.Instance.Enabled = true;
                 }
             });
        return true;
    }

    public GameObject modelObject;

    public void UnloadAsset()
    {
        Debug.Log("UnloadAsset:"+this);
        if (IsLoaded == false)
        {
            Debug.Log("UnloadAsset IsLoaded == false :" + this);
            return;
        }
        if (modelObject == null)
        {
            Debug.Log("UnloadAsset modelObject == null:" + this);
            return;
        }
        IsLoaded = false;
        SetNormalMeshHighlight(true);
        ModelAssetLoader.Instance.UnloadModel(ModelInfo);//不能用AssetBundleMagic.UnloadBundle(ModelInfo.GetBundleName(), true)，上面的代码 ModelAssetLoader里面还有个前缀
        //ModelInfo.isLoaded = false;

        if (modelObject != null)
        {
            GameObject.Destroy(modelObject);
        }
    }

    /// <summary>
    /// 设置粗模的mesh和高亮
    /// </summary>
    private void SetNormalMeshHighlight(bool isOn)
    {
        if (meshRenderer)
        {
            meshRenderer.enabled = isOn;
        }
        HightlightModuleBase highLight = transform.GetComponent<HightlightModuleBase>();
        if(highLight)
        {
            highLight.enabled = false;
            highLight.enabled = true;
            HighlightManage manager = HighlightManage.Instance;
            if (manager)
            {
                DevNode dev = manager.GetCurrentHighLightDev();
                if(dev!=null&&dev.gameObject==gameObject)
                {
                    dev.HighlightOn();
                }
            }
        }
    }

    void OnDestroy()
    {
        Buffer.Remove(this);
        if (IsLoaded)//加载了精细设备模型的
        {
            bool r1=ModelAssetLoader.Instance.UnloadModel(ModelInfo.GetBundleName());//卸载掉精细设备模型
            if (r1 == false)
            {
                Debug.LogError("DeviceModel.OnDestroy 1 不存在该Bundle:" + ModelInfo.GetBundleName());
            }
        }
        bool r2=ModelAssetLoader.Instance.UnloadModel(ModelInfo.GetSimpleBundleName());//卸载掉简化设备模型
        if (r2 == false)
        {
            Debug.LogError("DeviceModel.OnDestroy 2 不存在该Bundle:" + ModelInfo.GetSimpleBundleName());
        }
        if(SceneAssetManager.Instance&&SceneAssetManager.Instance.LoadedDevices!=null)
        {
            if(SceneAssetManager.Instance.LoadedDevices.Contains(this)) SceneAssetManager.Instance.LoadedDevices.Remove(this);
        }
    }

List<Vector3> points = new List<Vector3>();
    [ContextMenu("GetTestPoints")]
    public List<Vector3> GetTestPoints()
    {
        if (points.Count > 0) return points;
        //distanceChecker
        var pos = transform.position;
        points.Add(pos);

        var bounds = ColliderHelper.CaculateBounds(gameObject.transform, false);//不用碰撞体，计算包围盒就行了
        var size = bounds.size/2;
        points.Add(pos + new Vector3(-size.x, -size.y, -size.z));
        points.Add(pos + new Vector3(-size.x, -size.y, size.z));
        points.Add(pos + new Vector3(-size.x, size.y, -size.z));
        points.Add(pos + new Vector3(size.x, -size.y, -size.z));
        points.Add(pos + new Vector3(-size.x, size.y, size.z));
        points.Add(pos + new Vector3(size.x, -size.y, size.z));
        points.Add(pos + new Vector3(size.x, size.y, -size.z));
        points.Add(pos + new Vector3(size.x, size.y, size.z));

        points.Add(pos + new Vector3(0, -size.y, -size.z));
        points.Add(pos + new Vector3(0, -size.y, size.z));
        points.Add(pos + new Vector3(0, size.y, -size.z));
        points.Add(pos + new Vector3(0, size.y, size.z));

        points.Add(pos + new Vector3(size.x, 0, -size.z));
        points.Add(pos + new Vector3(size.x, 0, size.z));
        points.Add(pos + new Vector3(size.x, 0, -size.z));
        points.Add(pos + new Vector3(size.x, 0, size.z));

        points.Add(pos + new Vector3(-size.x, -size.y, 0));
        points.Add(pos + new Vector3(size.x, -size.y, 0));
        points.Add(pos + new Vector3(-size.x, size.y, 0));
        points.Add(pos + new Vector3(size.x, size.y, 0));
        return points;
    }
}
