using CommonExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcRtAlignJobArg : IComparable<AcRtAlignJobArg>
{
    public int id;

    public MeshPoints mfFrom;

    public MeshPoints mfTo;

    public static bool IsDebug = false;

    public void ReplaceToObject(GameObject newGo, PrefabInfo prefab)
    {

        GameObject go = mfTo.gameObject;

        //Debug.LogError($"ReplaceToObject newGo:{newGo} prefab:{prefab} to:{go}");

        if (mfTo.name == "Z2 2")
        {
            Debug.LogError($"DestroyToObject mfTo:{go}");
        }

        if (go == null)
        {
            Debug.LogError("DestroyToObject mfTo.gameObject == null");
            return;
        }

        //go.SetActive(false);
        //EditorHelper.UnpackPrefab(go);
        //GameObject.DestroyImmediate(go);

        //if (IsDebug)
        //{
        //    if (prefab != null)
        //    {
        //        prefab.InstancesBack.Add(GameObject.Instantiate(newGo));
        //    }
        //}


        go.name += "_New";
        GameObjectExtension.CopyTransformMesh(newGo, go);

        //if (IsDebug)
        //{
        //    newGo.SetActive(false);
        //}
        //else
        //{
        GameObject.DestroyImmediate(newGo);

        if (prefab != null)
        {
            prefab.AddInstance(go);
        }
        //}


    }

    public double Time = 0;

    public AcRtAlignJobArg(int i, MeshPoints from, MeshPoints to)
    {
        this.id = i;
        this.mfFrom = from;
        this.mfTo = to;
    }

    public static Dictionary<int, AcRtAlignJobArg> Dicts = new Dictionary<int, AcRtAlignJobArg>();

    public static AcRtAlignJobArg SaveArg(int i, MeshPoints from, MeshPoints to)
    {
        AcRtAlignJobArg arg = new AcRtAlignJobArg(i, from, to);
        Dicts.Add(i, arg);
        return arg;
    }

    public static AcRtAlignJobArg LoadArg(int id)
    {
        if (Dicts.ContainsKey(id))
        {
            return Dicts[id];
        }
        Debug.LogError("AcRtAlignJobArg.LoadArg NotFound:");
        return null;
    }

    public static void CleanArgs()
    {
        Dicts.Clear();
    }

    public bool AlignResult = false;

    public override string ToString()
    {
        //bool isSameSize = MeshHelper.IsSameSize(this.mfFrom, this.mfTo);
        //float sizeCompare = MeshHelper.CompareSize(mfFrom, mfTo);
        //string argTxt = $"°ætime:{Time:F0}°ø°æ{AlignResult}°ø°æ{sizeCompare:F3}°ø°æ{Log}°ø°æfrom:{mfFrom.name}[v:{mfFrom.vertexCount}]{mfFrom.size},to:{mfTo.name}[v:{mfTo.vertexCount}]{mfTo.size}°ø";

        string argTxt = $"°ætime:{Time:F0}°ø°æ{AlignResult}°ø°æ{InnerMeshHelper.CompareSize_Debug(mfFrom, mfTo)}°ø°æ{Log}°ø°æfrom:{mfFrom.name}[v:{mfFrom.vertexCount}]{mfFrom.size},to:{mfTo.name}[v:{mfTo.vertexCount}]{mfTo.size}°ø";

        if (Result != null)
        {
            return $"{argTxt} æ‡¿Î:{Result.Distance}";
        }
        return argTxt;
    }

    public string Log = "";

    public int CompareTo(AcRtAlignJobArg other)
    {
        int r = other.Time.CompareTo(this.Time);
        if (r == 0)
        {
            r = this.id.CompareTo(other.id);
        }
        return r;
    }

    public IRTResult Result;
}
