using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CommonUtils;
using CommonExtension;

public static class MeshCombineHelper
{

    public static bool AddScripted = false;

    //public static GameObject SimpleCombine(MeshCombineArg source,GameObject target){
        
    //    CombinedMesh combinedMesh=new CombinedMesh(source.transform,null,null);
    //    combinedMesh.DoCombine(true);
    //    target=combinedMesh.CreateNewGo(false,target);
    //    target.AddComponent<MeshFilterInfo>();
    //    Debug.Log("Combine:"+source+"->"+target);

    //    MeshHelper.CenterPivot(target.transform,combinedMesh.minMax[3]);

    //    return target;
    //}

    public static IEnumerator SimpleCombine_Coroutine(MeshCombineArg source,GameObject target,int waitCount,bool isDestroy){
        
        CombinedMesh combinedMesh=new CombinedMesh(source.transform,null,null);
        yield return combinedMesh.DoCombine_Coroutine(true,waitCount);
        target=combinedMesh.CreateNewGo(false,target);
        target.AddComponent<MeshFilterInfo>();
        Debug.Log("Combine:"+source+"->"+target);
        if(isDestroy){
            source.DestroySource();
        }
        yield return target;
    }

    public static GameObject SplitByMaterials(GameObject obj,bool isCenterPivot,bool isDestroy=true)
    {
        if (obj == null)
        {
            Debug.LogError("SplitByMaterials obj == null");
            return obj;
        }
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            Debug.LogWarning("SplitByMaterials renderer == null :"+obj);
            return obj;
        }
        if (renderer.sharedMaterials.Length == 1) return obj;
        //SplitByMaterials：
        //1.ResetTransform
        //2.CombineByMaterial
        //3.SetParent To Source
        //4.RecoverTransform
        //5.SetParent To Source.parent

        InnerEditorHelper.UnpackPrefab(obj);

        TransformData t = new TransformData(obj.transform);
        t.Reset();

        //GameObject goNew = CombineMaterials(obj);
        MeshCombineArg arg = new MeshCombineArg(obj);
        arg.isCenterPivot = isCenterPivot;
        arg.prefix = obj.name+"_";
        GameObject goNew = CombineInner_Multi(arg,false);

        goNew.transform.SetParent(obj.transform);
        t.Recover();
        goNew.transform.SetParent(obj.transform.parent);
        //goNew.name = obj.name + "_Split";
        goNew.name = obj.name;

        if(isDestroy)
        {
            GameObject.DestroyImmediate(obj);
        }
        else
        {
            obj.SetActive(false);
        }

        //MeshRendererInfoList infos=MeshRendererInfo.InitRenderers(goNew);
        //infos.SetRendererType(MeshRendererType.Splited);
        return goNew;
    }

    public static GameObject CombineMaterials(GameObject obj)
    {
        //TransformData t = new TransformData(obj.transform);
        //t.Reset();

        MeshCombineArg arg = new MeshCombineArg(obj);
        GameObject result = CombineInner_Multi(arg);
        return result;
    }

    public static GameObject CombineInner_Multi(MeshCombineArg arg,bool isCenterPivot=true)
    {
        DateTime start=DateTime.Now;
        GameObject goNew=new GameObject();

        MeshRenderer[] renderers = arg.GetRenderers();
        var minMax = VertexHelper.GetMinMax(renderers);

        goNew.name=arg.name+"_Combined_M";

        if (arg.source != null)
        {
            //arg.source.transform.position= minMax[3];
            //goNew.transform.position = arg.source.transform.position;
            goNew.transform.position = arg.source.transform.position;
        }

        // MeshFilter[] mfList =go.GetComponentsInChildren<MeshFilter>(true);
        // var minMax=MeshHelper.GetMinMax(mfList);
        // goNew.transform.position=minMax[3];
        //goNew.transform.position=minMax[3];

        //int count=0;
        SubMeshList mfList =new SubMeshList();
        Dictionary<Material, SubMeshList> mat2Filters=GetMatFilters(renderers);
        string mats="";
        int allVs=0;
        foreach(var item in mat2Filters)
        {
            Material material=item.Key;
            mats+=material.name+";";
            var list=item.Value;
            string meshNames = "";
            list.ForEach(i => meshNames += i.meshFilter.name + ";");
            mfList.AddRange(list);
            CombinedMesh combinedMesh=new CombinedMesh(arg,list,material);
            int vs=combinedMesh.DoCombine(true);
            if(vs>0){
                GameObject matGo=combinedMesh.CreateNewGo(false,null);
                matGo.name=arg.prefix+material.name;
                combinedMesh.meshPartList[0].mesh.name = matGo.name;
                matGo.transform.SetParent(goNew.transform);
                //matGo.transform.position = Vector3.zero;
            }
            else{
                Debug.LogWarning($"CombineMaterials vs==0 material:{material},list:{list.Count}");
            }
            allVs+=vs;
            //Debug.Log($"CombineMaterials material:[{material.name}] meshes:[{list.Count}] meshNames:[{meshNames}]");
        }

        goNew.transform.SetParent(arg.transform.parent);
        /*Debug.Log($"CombineMaterials name:{arg.name} 用时:{(DateTime.Now - start).TotalMilliseconds:F1}ms \tMesh数量:{renderers.Length} \tMat数量:{mat2Filters.Count} \tMats:{mats} \tVertex:{(allVs / 10000f):F1}");*/

        // var minMax=MeshHelper.GetMinMax(mfList);

        List<MeshFilter> filterlist = new List<MeshFilter>();
        foreach(var item in mfList)
        {
            filterlist.Add(item.meshFilter);
        }

        if (isCenterPivot)
        {
            GameObjectExtension.CenterPivot(goNew.transform, filterlist);
        }
        else
        {

        }

        //EditorHelper.RefreshAssets();
        return goNew;
    }

    // public static void CenterPivot(Transform t,Vector3 center)
    // {
    //     List<Transform> children=new List<Transform>();
    //     for(int i=0;i<t.childCount;i++)
    //     {
    //         children.Add(t.GetChild(i));
    //     }
    //     foreach(var child in children){
    //         child.SetParent(null);
    //     }
    //     t.position=center;

    //     foreach(var child in children){
    //         child.SetParent(t);
    //     }
    // }

     public static IEnumerator CombineMaterials_Coroutine(MeshCombineArg go,int waitCount,bool isDestroy){
        DateTime start=DateTime.Now;
        GameObject goNew=new GameObject();
        goNew.name=go.name+"_Combined_MC";
        goNew.transform.SetParent(go.transform.parent);
        MeshRenderer[] renderers = go.GetRenderers();
        Dictionary<Material, SubMeshList> mat2Filters=GetMatFilters(renderers);
        yield return null;
        int i=0;
        foreach(var item in mat2Filters)
        {
            Material material=item.Key;
            SubMeshList list =item.Value;
            Debug.LogWarning(string.Format("CombineMaterials_Coroutine {0} ({1}/{2})",material,i+1,mat2Filters.Count));
            CombinedMesh combinedMesh=new CombinedMesh(go.transform,list,material);
            yield return combinedMesh.DoCombine_Coroutine(true,waitCount);
            GameObject matGo=combinedMesh.CreateNewGo(false,null);
            matGo.name=material.name;
            matGo.transform.SetParent(goNew.transform);
            yield return goNew;
            i++;
        }
        if(isDestroy){
            go.DestroySource();
        }
        Debug.LogError(string.Format("CombineMaterials 用时:{0},Mat数量:{1},Mesh数量:{2}",(DateTime.Now-start),mat2Filters.Count, renderers.Length));
        yield return goNew;
    }

    //public static void SetMaterials(GameObject go)
    //{
    //    int count = 0;
    //    var mats = MeshCombineHelper.GetMatFilters(go, out count);
    //    SetMaterials(mats);
    //}

    public static void SetMaterials(Dictionary<Material, SubMeshList> mats)
    {
        //int count = 0;
        //var mats = MeshCombineHelper.GetMatFilters(go, out count);
        foreach (var mat in mats.Keys)
        {
            string matKey= MatInfo.GetMatKey(mat);
            var list = mats[mat];
            foreach (SubMesh subMesh in list)
            {
                MeshFilter meshFilter = subMesh.meshFilter;
                if (meshFilter==null)continue;
                MeshRenderer renderer = meshFilter.GetComponent<MeshRenderer>();
                if (renderer == null) continue;
                //renderer.sharedMaterial = mat;

                var mats2 = renderer.sharedMaterials;
                bool isChanged = false;
                for (int i = 0; i < mats2.Length; i++)
                {
                    Material m = (Material)mats2[i];
                    var key1 = MatInfo.GetMatKey(m);
                    if(key1==matKey)
                    {
                        mats2[i] = mat;
                        isChanged = true;
                    }
                }
                if(isChanged)
                    renderer.sharedMaterials = mats2;
            }
        }
    }

    public static Dictionary<Material, SubMeshList> GetMatFiltersInner(MeshRenderer[] renderers)
    {
        DateTime start = DateTime.Now;
        Dictionary<Material, SubMeshList> mat2Filters = new Dictionary<Material, SubMeshList>();
        //MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
        //count = renderers.Length;
        for (int i = 0; i < renderers.Length; i++)
        {
            MeshRenderer renderer = renderers[i];
            if(renderer==null)continue;
            NoCombine noCombine = renderer.GetComponent<NoCombine>();
            if (noCombine != null)
            {
                continue;
            }

            //if (renderer.sharedMaterial==null)continue;
            //if (!mat2Filters.ContainsKey(renderer.sharedMaterial))
            //{
            //    mat2Filters.Add(renderer.sharedMaterial, new List<MeshFilter>());
            //}
            //List<MeshFilter> list = mat2Filters[renderer.sharedMaterial];
            //MeshFilter filter = renderer.GetComponent<MeshFilter>();
            //list.Add(filter);

            for (int i1 = 0; i1 < renderer.sharedMaterials.Length; i1++)
            {
                Material mat = renderer.sharedMaterials[i1];
                if (mat == null) continue;
                if (!mat2Filters.ContainsKey(mat))
                {
                    mat2Filters.Add(mat, new SubMeshList());
                }
                List<SubMesh> list = mat2Filters[mat];
                MeshFilter filter = renderer.GetComponent<MeshFilter>();
                list.Add(new SubMesh(filter,i1));
            }
        }

        Dictionary<string, MatInfo> infos = new Dictionary<string, MatInfo>();
        foreach (var mat in mat2Filters.Keys)
        {
            var list = mat2Filters[mat];
            MatInfo info = new MatInfo(mat);
            if (infos.ContainsKey(info.key))
            {
                var item = infos[info.key];
                item.AddList(list);
            }
            else
            {
                infos.Add(info.key, info);
                info.AddList(list);
            }
        }

        Dictionary<Material, SubMeshList> mat2Filters2 = new Dictionary<Material, SubMeshList>();
        foreach (var info in infos.Values)
        {
            mat2Filters2.Add(info.mat, info.MeshFilters);
        }
        return mat2Filters2;
    }

    public static Dictionary<Material, List<SubMesh>> GetMatFiltersInnerEx(MeshRenderer[] renderers, out int count)
    {
        DateTime start = DateTime.Now;
        MeshMaterialList meshMaterials = new MeshMaterialList();

        Dictionary<Material, List<SubMesh>> mat2Filters = new Dictionary<Material, List<SubMesh>>();
        //MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
        count = renderers.Length;
        for (int i = 0; i < renderers.Length; i++)
        {
            MeshRenderer renderer = renderers[i];
            if (renderer == null) continue;
            NoCombine noCombine = renderer.GetComponent<NoCombine>();
            if (noCombine != null)
            {
                continue;
            }

            for (int i1 = 0; i1 < renderer.sharedMaterials.Length; i1++)
            {
                Material mat = renderer.sharedMaterials[i1];
                if (mat == null) continue;
                MeshFilter filter = renderer.GetComponent<MeshFilter>();

                if (!mat2Filters.ContainsKey(mat))
                {
                    mat2Filters.Add(mat, new List<SubMesh>());
                }
                List<SubMesh> list = mat2Filters[mat];
                list.Add(new SubMesh(filter,i1));

                MeshMaterial meshMaterial = new MeshMaterial(mat, filter, i1);
                meshMaterials.Add(meshMaterial);
            }
        }

        Dictionary<string, MatInfo> infos = new Dictionary<string, MatInfo>();
        foreach (var mat in mat2Filters.Keys)
        {
            var list = mat2Filters[mat];
            MatInfo info = new MatInfo(mat);
            if (infos.ContainsKey(info.key))
            {
                var item = infos[info.key];
                item.AddList(list);
            }
            else
            {
                infos.Add(info.key, info);
                info.AddList(list);
            }
        }

        Dictionary<Material, List<SubMesh>> mat2Filters2 = new Dictionary<Material, List<SubMesh>>();
        foreach (var info in infos.Values)
        {
            mat2Filters2.Add(info.mat, info.MeshFilters);
        }
        return mat2Filters2;
    }

    public static Dictionary<Material, SubMeshList> GetMatFilters(GameObject go,  bool isSetMaterial = false)
    {
        MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
        Dictionary<Material, SubMeshList> mat2Filters = GetMatFiltersInner(renderers);
        if (isSetMaterial)
        {
            SetMaterials(mat2Filters);
            mat2Filters = GetMatFiltersInner(renderers);
        }
        return mat2Filters;
    }

    public static Dictionary<Material, SubMeshList> GetMatFilters(MeshRenderer[] renderers,bool isSetMaterial=false){
        Dictionary<Material, SubMeshList> mat2Filters = GetMatFiltersInner(renderers);
        if (isSetMaterial)
        {
            SetMaterials(mat2Filters);
            mat2Filters = GetMatFiltersInner(renderers);
        }
        return mat2Filters;
    }

    private static GameObject CombineInner_One(MeshCombineArg source){
        DateTime start=DateTime.Now;
        GameObject goNew=CombineInner_Multi(source,source.isCenterPivot);//按材质合并
        CombinedMesh combinedMesh=new CombinedMesh(goNew.transform,null,null);
        combinedMesh.DoCombine(false);
        GameObject target=combinedMesh.CreateNewGo(false,null);
        target.name=source.name+"_Combined_C";
        goNew.transform.SetParent(target.transform);
        GameObject.DestroyImmediate(goNew);
        //Debug.LogError(string.Format("CombinedMesh 用时:{0}ms,数量:{1}",(DateTime.Now-start).TotalMilliseconds,count));
        if(source.isCenterPivot)
            GameObjectExtension.CenterPivot(target.transform,combinedMesh.minMax[3]);

        //Debug.LogError($"CombineInner_One source:{source} target:{target} isCenterPivot:{source.isCenterPivot}");
        return target;
    }

    public static IEnumerator Combine_Coroutine(MeshCombineArg source,int waitCount,bool isDestroy){
        DateTime start=DateTime.Now;
        GameObject goNew=CombineInner_Multi(source);//按材质合并
        CombinedMesh combinedMesh=new CombinedMesh(goNew.transform,null,null);
        yield return combinedMesh.DoCombine_Coroutine(false,waitCount);
        GameObject target=combinedMesh.CreateNewGo(false,null);
        target.name=source.name+"_Combined_CC";
        goNew.transform.SetParent(target.transform);
        GameObject.DestroyImmediate(goNew);
        //Debug.LogError(string.Format("CombinedMesh 用时:{0}ms,数量:{1}",(DateTime.Now-start).TotalMilliseconds,count));
        if(isDestroy)
        {
            source.DestroySource();
        }
        //CenterPivot(target.transform,combinedMesh.minMax[3]);
        yield return target;
    }

    public static GameObject Combine(GameObject root)
    {
        var rotation = root.transform.localRotation;
        root.transform.localRotation = Quaternion.identity;
        MeshCombineArg arg = new MeshCombineArg(root);
        GameObject goNew = CombineEx(arg);
        goNew.name = root.name;
        goNew.transform.localRotation = rotation;
        GameObject.DestroyImmediate(root);
        return goNew;
    }

    public static GameObject CombineEx(MeshCombineArg arg, MeshCombineMode mode = MeshCombineMode.OneMesh)
    {
#if UNITY_EDITOR
        InnerEditorHelper.UnpackPrefab(arg.source);
#endif
        Transform parent = arg.source.transform.parent;
        arg.source.transform.parent = null;

        GameObject result = null;
        if (mode== MeshCombineMode.OneMesh)
        {
            result= CombineInner_One(arg);
        }
        else //if(mode == MeshCombineMode.MultiByMat)
        {
            int count=0;
            result= CombineInner_Multi(arg);
        }
        //else{
        //    return SimpleCombine(source,null);
        //}
        arg.source.transform.parent = parent;
        result.transform.parent = parent;

        return result;
    }

    public static IEnumerator CombineEx_Coroutine(MeshCombineArg source,bool isDestroy,int waitCount, MeshCombineMode mode = MeshCombineMode.OneMesh)
    {
        if(mode== MeshCombineMode.OneMesh)
        {
            yield return Combine_Coroutine(source,waitCount,isDestroy);
        }
        else if(mode == MeshCombineMode.MultiByMat)
        {
            yield return CombineMaterials_Coroutine(source,waitCount,isDestroy);
        }
        else{
           yield return SimpleCombine_Coroutine(source,null,waitCount,isDestroy);
        }
    }

    public static Dictionary<GameObject,CombinedMesh> go2ms=new Dictionary<GameObject, CombinedMesh>();
    public static void AddGo(GameObject gameObject,CombinedMesh mesh){
        if(go2ms.ContainsKey(gameObject)){
            go2ms[gameObject]=mesh;
        }
        else{
            go2ms.Add(gameObject,mesh);
        }
        
    }

    public static CombinedMesh GetMesh(GameObject gameObject){
        if(go2ms.ContainsKey(gameObject)){
            return go2ms[gameObject];
        }
        else{
            return null;
        }
    }

    //public static void RemveGo(GameObject gameObject){
    //    CombinedMesh mesh=GetMesh(gameObject);
    //    if(mesh!=null){
    //        Debug.Log("mesh:"+mesh.name);
    //        Debug.Log("mesh.meshFilters:"+mesh.meshFilters.Count);
    //        MeshFilter[] meshFilters=gameObject.GetComponentsInChildren<MeshFilter>();
    //        for (int i = 0; i < meshFilters.Length; i++)
    //        {
    //            MeshFilter mf = meshFilters[i];
    //            //mesh.meshFilters.Remove(mf);
    //            int id = mesh.meshFilters.IndexOf(mf);
    //            if (id != -1)
    //            {
    //                mesh.meshFilters.RemoveAt(id);
    //                mesh.meshIndexes.RemoveAt(id);
    //            }
    //        }
    //        MeshRenderer[] meshRenderers=gameObject.GetComponentsInChildren<MeshRenderer>();
    //        foreach(var mr in meshRenderers){
    //            mr.enabled=true;
    //            mr.material.SetColor("_BaseColor",Color.red);
    //        }
    //        Debug.Log("mesh.meshFilters:"+mesh.meshFilters.Count);
    //        mesh.Refresh();//重新合并
    //    }
    //    else{
    //        Debug.LogError("未找到CombinedMesh:"+gameObject);
    //    }
    //}

}

