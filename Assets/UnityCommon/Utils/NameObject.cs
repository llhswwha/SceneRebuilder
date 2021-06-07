using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unity.ComnLib.Utils
{
    /// <summary>
    /// 物体信息
    /// </summary>
    [Serializable]
    public class NameObject //todo:应该改成某个合适的名称了，NameObjectInfo？
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 预设
        /// </summary>
        public GameObject Object;
        /// <summary>
        /// 代表中心的物体
        /// </summary>
        public GameObject Center;
        [HideInInspector]
        public GameObject _instance;
        /// <summary>
        /// 偏移量
        /// </summary>
        public Vector3 Offset = Vector3.zero;
        /// <summary>
        /// 是否显示UI三层界面
        /// </summary>
        public bool staticRoomControlPanel;
        /// <summary>
        /// UI三层界面
        /// </summary>
       // public GameObject StaticRoomControlPanel;
        /// <summary>
        /// Assetbundle动态加载
        /// </summary>
        public bool isAssetbundleLoad;
        /// <summary>
        /// Assetbundle动态加载
        /// </summary>
        public bool isCompleteAssetbundleLoadComplete;

        public void Show()
        {
            if (Object)
            {
                Object.SetActive(true);
            }

        }

        public void Hide()
        {
            if (Object)
            {
                Object.SetActive(false);
            }
        }

        public GameObject GetCenter()
        {
            if (Center) return Center;
            return Object;
        }

        public void Set_instance(GameObject obj)
        {
            _instance = obj;
        }

        /// <summary>
        /// 显示设置的物体，第一次是根据预设创建物体
        /// </summary>
        public void ShowObject(Action<GameObject> action = null)
        {

            //    //if (Object == null) return null;
            //    //if (_instance == null)
            //    //{
            //    //    _instance = GameObject.Instantiate(Object);
            //    //    _instance.transform.position += Offset;
            //    //}
            //    if (isAssetbundleLoad && isCompleteAssetbundleLoadComplete == false)
            //    {
            //        StartCoroutine(AssetBundleHelper.LoadAssetGameObject(Name, (obj) =>
            //    {
            //        _instance = GameObject.Instantiate(obj as GameObject);
            //        _instance.transform.position += Offset;
            //        _instance.SetActive(true);
            //        if (action != null)
            //        {
            //            action(obj);
            //        }
            //    }));
            //    }
            //    else
            //    {
            //        if (Object == null)
            //        {
            //            if (action != null)
            //            {
            //                action(null);
            //            }
            //            return;
            //        }
            //        if (_instance == null)
            //        {
            //            _instance = GameObject.Instantiate(Object);
            //            _instance.transform.position += Offset;
            //            if (action != null)
            //            {
            //                action(_instance);
            //            }
            //        }
            //    }

            //    //_instance.SetActive(true);
            //    //return _instance;

        }

        /// <summary>
        /// 创建物体对象
        /// </summary>
        public GameObject CreateObject()
        {
            if (Object == null) return null;
            _instance = GameObject.Instantiate(Object);
            _instance.transform.position += Offset;
            _instance.SetActive(true);
            return _instance;
        }

        /// <summary>
        /// 创建物体对象
        /// </summary>
        public GameObject CreateObject(GameObject prefab)
        {
            if (prefab == null) return null;
            _instance = GameObject.Instantiate(prefab);
            _instance.transform.position += Offset;
            _instance.SetActive(true);
            return _instance;
        }
    }

}
