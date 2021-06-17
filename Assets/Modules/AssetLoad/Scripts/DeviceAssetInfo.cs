using Jacovone.AssetBundleMagic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 整个主厂房1楼或者二楼的全部设备模型
/// </summary>
public class DeviceAssetInfo : AssetBundleInfo
{
    public override bool LoadAsset(Action<DepNode> callback, bool isFocus = true)
    {
        if (SystemSettingHelper.assetLoadSetting.LoadDeviceAsset == false)
        {
            if (callback != null)
            {
                callback(null);
            }
            return false;//测试用，不加载设备模型
        }
        return base.LoadAsset(callback, isFocus);
    }

    public override void SetUnload(bool isImmediate)
    {
        if (SystemSettingHelper.assetLoadSetting.LoadDeviceAsset == false) return;//测试用，不加载设备模型
        SceneAssetManager.Instance.BeforeUnloadAsset(this, isImmediate);//这里不直接卸载
    }

    protected override DepNode LoadRootObject(GameObject obj, bool isFocus = true)
    {
        DepNode depController = gameObject.GetComponent<DepNode>();
        if(depController == null)
        {
            Debug.LogError("FloorController is null:"+gameObject.name);
            depController = transform.GetComponentInParent<DepNode>();
        }
        if (depController != null)
        {
            depController.StaticDevContainer = obj;
        }
        obj.transform.parent = this.transform;
        AddScprits(obj.transform);

        //var renderers= obj.GetComponentsInChildren<MeshRenderer>();
        //foreach (var item in renderers)
        //{
        //    var t = item.transform;
        //    t.gameObject.AddMissingComponent<MeshCollider>();
        //    t.gameObject.AddMissingComponent<FacilityDevController>();
        //    //AddSubDevScripts(t);
        //}

        return depController;
    }
    //J1一楼，下面两个Devices,再往下才是设备。需要单独处理
    private string J1F1StaticDev = "J1_F1_Devices";

    public void AddScprits(Transform deviceT)
    {
        int childCount = deviceT.childCount;
        bool isJ1F1Devs = deviceT.name.StartsWith(J1F1StaticDev);//或者J1_F1_Devices_Loader，不小心改名了
        for (int i = 0; i < childCount; i++)
        {
            GameObject obj = deviceT.GetChild(i).gameObject;
            if (obj.name.Contains("Invalid")) continue;            
            if (!isJ1F1Devs)
            {
                FacilityDevController devController = obj.AddMissingComponent<FacilityDevController>();
                if (RoomFactory.Instance) RoomFactory.Instance.SaveStaticDevInfo(devController);
                MeshRenderer render = obj.transform.GetComponent<MeshRenderer>();
                if (render != null)
                {
                    MeshCollider meshCollider = obj.AddMissingComponent<MeshCollider>();
                }
            }
            AddSubDevScripts(obj.transform,isJ1F1Devs);
        }
    }
    DateTime recordTime1;
    DateTime recordTime2;
    private void AddSubDevScripts(Transform childTransform, bool isJ1F1 = false)
    {
        if(isJ1F1)recordTime1 = DateTime.Now;
        for (int i = 0; i < childTransform.childCount; i++)
        {
            if (isJ1F1) recordTime2 = DateTime.Now;
            Transform child = childTransform.GetChild(i);
            if (isJ1F1)
            {
                FacilityDevController devController = child.gameObject.AddMissingComponent<FacilityDevController>();
                if (RoomFactory.Instance) RoomFactory.Instance.SaveStaticDevInfo(devController);
            }
            MeshRenderer render = child.GetComponent<MeshRenderer>();
            if (render != null)
            {
                child.gameObject.AddMissingComponent<MeshCollider>();
                if (child.GetComponent<FacilityDevController>() == null && child.GetComponent<FacilityDevController>() == null)
                {
                    child.gameObject.AddMissingComponent<FacilityMeshTrigger>();
                }
            }
            AddSubDevScripts(child);
            //Debug.LogErrorFormat("AddSubDevCostTime:{0} ms", (DateTime.Now - recordTime2).TotalMilliseconds);
        }
        //Debug.LogErrorFormat("AddSubDevCostTime:{0} ms",(DateTime.Now-recordTime1).TotalMilliseconds);
    }
    /// <summary>
    /// 添加子物体脚本
    /// </summary>
    /// <param name="childTransform"></param>
    IEnumerator AddSubDevScriptsCorutine(Transform childTransform,bool isJ1F1=false,Action onComplete=null)
    {
        for (int i = 0; i < childTransform.childCount; i++)
        {
            Transform child = childTransform.GetChild(i);
            if (isJ1F1)
            {
                FacilityDevController devController = child.gameObject.AddMissingComponent<FacilityDevController>();
                if (RoomFactory.Instance) RoomFactory.Instance.SaveStaticDevInfo(devController);
            }
            MeshRenderer render = child.GetComponent<MeshRenderer>();
            if (render != null)
            {
                child.gameObject.AddMissingComponent<MeshCollider>();
                if (child.GetComponent<FacilityDevController>() == null&&child.GetComponent<FacilityDevController>()==null)
                {
                    child.gameObject.AddMissingComponent<FacilityMeshTrigger>();
                }
            }
            StartCoroutine(AddSubDevScriptsCorutine(child));            
            yield return null;
        }
        if (onComplete != null) onComplete();
    }

    protected override void AfterLoad()
    {
        
    }
}
