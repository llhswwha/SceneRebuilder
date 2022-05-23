using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class InnerMeshHelper 
{
    public static string CompareSize_Debug(MeshPoints ps1, MeshPoints ps2)
    {
        StringBuilder sbScaleLog = new StringBuilder();
        List<float> sizeFrom = ps1.GetSizeList();
        List<float> sizeTo = ps2.GetSizeList();
        List<float> scaleList = new List<float>();
        for (int i = 0; i < 3; i++)
        {
            float sizeScale = sizeTo[i] / sizeFrom[i];
            if (scaleList.Contains(sizeScale))
            {
                continue;
            }
            sbScaleLog.Append($"sizeScale[{i}]:{sizeScale}({sizeTo[i]}/{sizeFrom[i]});\t");
            scaleList.Add(sizeScale);
        }

        if (scaleList.Count == 1)
        {
            //
        }

        scaleList.Sort();

        string txt = "";
        float sum = 0;
        float avg = 0;
        for (int i = 0; i < scaleList.Count; i++)
        {
            float scale = scaleList[i];
            sbScaleLog.Append($"scale[{i}]:{scale};\t");
            sum += scale;
            txt += scale + ";";
        }
        avg = sum / scaleList.Count;

        var scaleMin = scaleList.First();
        var scaleMax = scaleList.Last();
        float scale_scale = scaleMax / scaleMin;
        sbScaleLog.AppendLine($"scale_scale:{scale_scale}");
        //bool isSame = scale_scale < 1.01;

        //Debug.LogError($"IsSameSize ps1:{ps1.name} ps2:{ps2.name} result:{isSame} ss:{scale_scale} log:{sbScaleLog}");
        return $"avg:{avg} max/min:{scale_scale} list:{txt}";
    }

    public static GameObject CopyGO(this GameObject go2, bool isDebug = false)
    {
        DateTime start = DateTime.Now;

        if (go2 == null)
        {
            Debug.LogError("CopyGO go2==null");
            return null;
        }
        Transform t1 = go2.transform;

        Transform parent1 = t1.parent;
        //go2.transform.SetParent(null);

        //List<Transform> children = new List<Transform>();
        //for(int i=0;)

        GameObject go2Copy = GameObject.Instantiate(go2);

        //EditorHelper.RemoveAllComponents(go2Copy);

        //GameObject go2Copy=new GameObject(go2.name);

        Transform t2 = go2Copy.transform;
        t2.SetParent(parent1);//复制后的默认的父是null的
        t2.localPosition = t1.localPosition;
        t2.localRotation = t1.localRotation; //我发现Instantiate居然不会复制比例，会变成(0,0,0)
        t2.localScale = t1.localScale; //我发现Instantiate居然不会复制比例，会变成(1,1,1)
        //发现必须这样设置parent，localxxx，不然会有有位置偏移!?

        double t = (DateTime.Now - start).TotalMilliseconds;

        //TotalCopyTime += t;
        //TotalCopyCount++;

        //var ids = go2Copy.GetComponentsInChildren<RendererId>();
        InnerRendererId.NewIds(go2Copy);

        return go2Copy;
    }

    public static GameObject EditorCopyGo(GameObject sourceGo)
    {
#if UNITY_EDITOR
        GameObject newObj = null;
        GameObject root = PrefabUtility.GetNearestPrefabInstanceRoot(sourceGo);
        //Debug.Log($"root:[{root}]");
        if (root != null)
        {
            string prefabPath = UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root);
            //Debug.Log($"prefabPath:[{prefabPath}]");
            var prefabAsset = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            //Debug.Log($"prefabAsset:[{prefabAsset}]");
            var prefab = PrefabUtility.InstantiatePrefab(prefabAsset);
            //Debug.Log($"prefab:[{prefab}]");
            newObj = prefab as GameObject;
        }
        else
        {
            newObj = CopyGO(sourceGo);
        }
#else
        GameObject newObj = CopyGO(sourceGo);
#endif
        return newObj;
    }

    /// <summary>
    /// 用一个单元模型，替换掉场景中的相同模型。
    /// </summary>
    /// <param name="oldObj"></param>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public static GameObject ReplaceByPrefab(GameObject oldObj, GameObject prefab)
    {
        if (prefab == null) return null;
        GameObject newObj = ReplaceGameObject(oldObj, prefab, false, null);
        newObj.transform.localEulerAngles = oldObj.transform.localEulerAngles + new Vector3(0, 90, 90);

        //GameObject.Destroy(item.gameObject.transform);
#if UNITY_EDITOR
        if (oldObj.GetComponent<InnerMeshNode>() == null)
        {
            oldObj.AddComponent<InnerMeshNode>();
        }
        if (newObj.GetComponent<InnerMeshNode>() == null)
        {
            newObj.AddComponent<InnerMeshNode>();
        }
#endif
        return newObj;
    }

    public static GameObject ReplaceGameObject(GameObject oldObj, GameObject prefab, bool isDestoryOriginal, TransfromAlignSetting transfromAlignSetting)
    {
        if (prefab == null) return null;
#if UNITY_EDITOR
        InnerEditorHelper.UnpackPrefab(oldObj);
        InnerEditorHelper.UnpackPrefab(prefab);
#endif

        Transform parentOldObj = oldObj.transform.parent;
        Transform parentPrefab = prefab.transform.parent;
        oldObj.transform.parent = null;
        prefab.transform.parent = null;

        GameObject newObj = EditorCopyGo(prefab);

        newObj.SetActive(true);

        //newObj.transform.position = oldObj.transform.position;
        //newObj.transform.eulerAngles = oldObj.transform.eulerAngles;
        //newObj.transform.parent = oldObj.transform;

        newObj.transform.position = prefab.transform.position;

        newObj.transform.parent = oldObj.transform.parent;
        if (transfromAlignSetting == null)
        {
            newObj.transform.localPosition = oldObj.transform.localPosition;
        }
        else
        {
            var minMax_TO = InnerMeshRendererInfo.GetMinMax(oldObj);
            var minMax_From = InnerMeshRendererInfo.GetMinMax(prefab);

            var pos = newObj.transform.localPosition;
            if (transfromAlignSetting.Align == TransfromAlignMode.Pivot)
            {
                if (transfromAlignSetting.SetPosX)
                {
                    pos.x = oldObj.transform.localPosition.x;
                }
                if (transfromAlignSetting.SetPosY)
                {
                    pos.y = oldObj.transform.localPosition.y;
                }
                if (transfromAlignSetting.SetPosZ)
                {
                    pos.z = oldObj.transform.localPosition.z;
                }
            }
            else if (transfromAlignSetting.Align == TransfromAlignMode.Min)
            {
                var dis = minMax_TO[0] - minMax_From[0];
                if (transfromAlignSetting.SetPosX)
                {
                    pos.x += dis.x;
                }
                if (transfromAlignSetting.SetPosY)
                {
                    pos.y += dis.y;
                }
                if (transfromAlignSetting.SetPosZ)
                {
                    pos.z += dis.z;
                }
            }
            else if (transfromAlignSetting.Align == TransfromAlignMode.Max)
            {
                var dis = minMax_TO[1] - minMax_From[1];
                if (transfromAlignSetting.SetPosX)
                {
                    pos.x += dis.x;
                }
                if (transfromAlignSetting.SetPosY)
                {
                    pos.y += dis.y;
                }
                if (transfromAlignSetting.SetPosZ)
                {
                    pos.z += dis.z;
                }
            }
            else if (transfromAlignSetting.Align == TransfromAlignMode.Center)
            {
                var dis = minMax_TO[3] - minMax_From[3];
                if (transfromAlignSetting.SetPosX)
                {
                    pos.x += dis.x;
                }
                if (transfromAlignSetting.SetPosY)
                {
                    pos.y += dis.y;
                }
                if (transfromAlignSetting.SetPosZ)
                {
                    pos.z += dis.z;
                }
            }

            if (transfromAlignSetting.SetPosition)
            {
                newObj.transform.localPosition = pos;
            }
        }

        if (transfromAlignSetting.SetScale)
        {
            newObj.transform.localScale = oldObj.transform.localScale;
        }
        if (transfromAlignSetting.SetRotation)
        {
            newObj.transform.localEulerAngles = oldObj.transform.localEulerAngles;
        }
        //newObj.transform.localEulerAngles = oldObj.transform.localEulerAngles + new Vector3(0, 90, 90);

        if (isDestoryOriginal)
        {
            newObj.name = oldObj.name;
            GameObject.DestroyImmediate(oldObj);
        }
        else
        {
            newObj.name = oldObj.name + "_New";
        }

        newObj.transform.parent = parentOldObj;
        oldObj.transform.parent = parentOldObj;
        prefab.transform.parent = parentPrefab;

        return newObj;
    }
}

public enum TransfromAlignMode
{
    Pivot, Min, Max, Center, MinMax, MaxMin
}

[System.Serializable]
public class TransfromAlignSetting
{

    public bool SetPosition = true;
    public bool SetScale = true;
    public bool SetRotation = true;
    public TransfromAlignMode Align = TransfromAlignMode.Max;
    public bool SetPosX = true;
    public bool SetPosY = true;
    public bool SetPosZ = true;

    //public bool SetPosByMinX = true;
    //public bool SetPosByMinY = true;
    //public bool SetPosByMinZ = true;
}
