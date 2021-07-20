using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class MeshCombineHelper
{

    public static bool AddScripted = false;

    public static GameObject SimpleCombine(MeshCombineArg source,GameObject target){
        
        CombinedMesh combinedMesh=new CombinedMesh(source.transform,null,null);
        combinedMesh.DoCombine(true);
        target=combinedMesh.CreateNewGo(false,target);
        target.AddComponent<MeshFilterInfo>();
        Debug.Log("Combine:"+source+"->"+target);

        MeshHelper.CenterPivot(target.transform,combinedMesh.minMax[3]);

        return target;
    }

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

    public static GameObject SplitByMaterials(GameObject go)
    {
        int count = 0;
        MeshCombineArg arg = new MeshCombineArg(go);
        GameObject result = CombineMaterials(arg, out count);
        return result;
    }

    public static GameObject CombineMaterials(MeshCombineArg go,out int  count){
        DateTime start=DateTime.Now;
        GameObject goNew=new GameObject();
        goNew.name=go.name+"_Combined_M";
        MeshRenderer[] renderers = go.GetRenderers();

        // MeshFilter[] mfList =go.GetComponentsInChildren<MeshFilter>(true);
        // var minMax=MeshHelper.GetMinMax(mfList);
        // goNew.transform.position=minMax[3];

        //int count=0;
        List<MeshFilter> mfList=new List<MeshFilter>();
        Dictionary<Material,List<MeshFilter>> mat2Filters=GetMatFilters(renderers, out count);
        string mats="";
        int allVs=0;
        foreach(var item in mat2Filters)
        {
            Material material=item.Key;
            mats+=material.name+";";
            List<MeshFilter> list=item.Value;
            string meshNames = "";
            list.ForEach(i => meshNames += i.name + ";");
            mfList.AddRange(list);
            CombinedMesh combinedMesh=new CombinedMesh(go.transform,list,material);
            int vs=combinedMesh.DoCombine(true);
            if(vs>0){
                GameObject matGo=combinedMesh.CreateNewGo(false,null);
                matGo.name=material.name;
                matGo.transform.SetParent(goNew.transform);
            }
            else{
                Debug.LogWarning($"CombineMaterials vs==0 material:{material},list:{list.Count}");
            }
            allVs+=vs;
            Debug.Log($"CombineMaterials material:{material.name} meshes:{list.Count} meshNames:{meshNames}");
        }

        goNew.transform.SetParent(go.transform.parent);
        Debug.Log(string.Format("CombineMaterials name:{5} 用时:{0} \tMesh数量:{2} \tMat数量:{1} \tMats:{3} \tVertex:{4:F1}", (DateTime.Now-start),mat2Filters.Count,count,mats,(allVs/10000f),go.name));
        
        // var minMax=MeshHelper.GetMinMax(mfList);
        MeshHelper.CenterPivot(goNew.transform,mfList);
        
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
        int count=0;
        MeshRenderer[] renderers = go.GetRenderers();
        Dictionary<Material,List<MeshFilter>> mat2Filters=GetMatFilters(renderers, out count);
        yield return null;
        int i=0;
        foreach(var item in mat2Filters)
        {
            Material material=item.Key;
            List<MeshFilter> list=item.Value;
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
        Debug.LogError(string.Format("CombineMaterials 用时:{0},Mat数量:{1},Mesh数量:{2}",(DateTime.Now-start),mat2Filters.Count,count));
        yield return goNew;
    }

    //public static void SetMaterials(GameObject go)
    //{
    //    int count = 0;
    //    var mats = MeshCombineHelper.GetMatFilters(go, out count);
    //    SetMaterials(mats);
    //}

    public static void SetMaterials(Dictionary<Material, List<MeshFilter>> mats)
    {
        //int count = 0;
        //var mats = MeshCombineHelper.GetMatFilters(go, out count);
        foreach (var mat in mats.Keys)
        {
            string matKey= MatInfo.GetMatKey(mat);
            var list = mats[mat];
            foreach (MeshFilter meshFilter in list)
            {
                if(meshFilter==null)continue;
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

    public static Dictionary<Material, List<MeshFilter>> GetMatFiltersInner(MeshRenderer[] renderers, out int count)
    {
        DateTime start = DateTime.Now;
        Dictionary<Material, List<MeshFilter>> mat2Filters = new Dictionary<Material, List<MeshFilter>>();
        //MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
        count = renderers.Length;
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
                    mat2Filters.Add(mat, new List<MeshFilter>());
                }
                List<MeshFilter> list = mat2Filters[mat];
                MeshFilter filter = renderer.GetComponent<MeshFilter>();
                list.Add(filter);
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

        Dictionary<Material, List<MeshFilter>> mat2Filters2 = new Dictionary<Material, List<MeshFilter>>();
        foreach (var info in infos.Values)
        {
            mat2Filters2.Add(info.mat, info.MeshFilters);
        }
        return mat2Filters2;
    }

    public static Dictionary<Material, List<MeshFilter>> GetMatFiltersInnerEx(MeshRenderer[] renderers, out int count)
    {
        DateTime start = DateTime.Now;
        MeshMaterialList meshMaterials = new MeshMaterialList();

        Dictionary<Material, List<MeshFilter>> mat2Filters = new Dictionary<Material, List<MeshFilter>>();
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
                    mat2Filters.Add(mat, new List<MeshFilter>());
                }
                List<MeshFilter> list = mat2Filters[mat];
                list.Add(filter);

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

        Dictionary<Material, List<MeshFilter>> mat2Filters2 = new Dictionary<Material, List<MeshFilter>>();
        foreach (var info in infos.Values)
        {
            mat2Filters2.Add(info.mat, info.MeshFilters);
        }
        return mat2Filters2;
    }

    public static Dictionary<Material, List<MeshFilter>> GetMatFilters(GameObject go, out int count, bool isSetMaterial = false)
    {
        MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
        Dictionary<Material, List<MeshFilter>> mat2Filters = GetMatFiltersInner(renderers, out count);
        if (isSetMaterial)
        {
            SetMaterials(mat2Filters);
            mat2Filters = GetMatFiltersInner(renderers, out count);
        }
        return mat2Filters;
    }

    public static Dictionary<Material, List<MeshFilter>> GetMatFilters(MeshRenderer[] renderers, out int count,bool isSetMaterial=false){
        Dictionary<Material, List<MeshFilter>> mat2Filters = GetMatFiltersInner(renderers, out count);
        if (isSetMaterial)
        {
            SetMaterials(mat2Filters);
            mat2Filters = GetMatFiltersInner(renderers, out count);
        }
        return mat2Filters;
    }

    public static GameObject Combine(MeshCombineArg source){
        DateTime start=DateTime.Now;
        int count=0;
        GameObject goNew=CombineMaterials(source,out count);//按材质合并
        CombinedMesh combinedMesh=new CombinedMesh(goNew.transform,null,null);
        combinedMesh.DoCombine(false);
        GameObject target=combinedMesh.CreateNewGo(false,null);
        target.name=source.name+"_Combined_C";
        goNew.transform.SetParent(target.transform);
        GameObject.DestroyImmediate(goNew);
        //Debug.LogError(string.Format("CombinedMesh 用时:{0}ms,数量:{1}",(DateTime.Now-start).TotalMilliseconds,count));
        MeshHelper.CenterPivot(target.transform,combinedMesh.minMax[3]);
        return target;
    }

    public static IEnumerator Combine_Coroutine(MeshCombineArg source,int waitCount,bool isDestroy){
        DateTime start=DateTime.Now;
        int count=0;
        GameObject goNew=CombineMaterials(source,out count);//按材质合并
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

    public static GameObject CombineEx(MeshCombineArg source,int mode=0){
        if(mode==0){
            return Combine(source);
        }
        else if(mode ==1 )
        {
            int count=0;
            return CombineMaterials(source,out count);
        }
        else{
            return SimpleCombine(source,null);
        }
    }

    public static IEnumerator CombineEx_Coroutine(MeshCombineArg source,bool isDestroy,int waitCount,int mode=0){
        if(mode==0){
            yield return Combine_Coroutine(source,waitCount,isDestroy);
        }
        else if(mode ==1 )
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

    public static void RemveGo(GameObject gameObject){
        CombinedMesh mesh=GetMesh(gameObject);
        if(mesh!=null){
            Debug.Log("mesh:"+mesh.name);
            Debug.Log("mesh.meshFilters:"+mesh.meshFilters.Count);
            MeshFilter[] meshFilters=gameObject.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i < meshFilters.Length; i++)
            {
                MeshFilter mf = meshFilters[i];
                //mesh.meshFilters.Remove(mf);
                int id = mesh.meshFilters.IndexOf(mf);
                if (id != -1)
                {
                    mesh.meshFilters.RemoveAt(id);
                    mesh.meshIndexes.RemoveAt(id);
                }
            }
            MeshRenderer[] meshRenderers=gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach(var mr in meshRenderers){
                mr.enabled=true;
                mr.material.SetColor("_BaseColor",Color.red);
            }
            Debug.Log("mesh.meshFilters:"+mesh.meshFilters.Count);
            mesh.Refresh();//重新合并
        }
        else{
            Debug.LogError("未找到CombinedMesh:"+gameObject);
        }
    }

}

public class MeshCombineArg
{
    public GameObject source;

    public MeshCombineArg(GameObject source)
    {
        this.source = source;
        this.renderers = source.GetComponentsInChildren<MeshRenderer>(true);
    }

    public MeshCombineArg(GameObject source,MeshRenderer[] rs){
        this.source=source;
        this.renderers=rs;
    }

    public MeshRenderer[] renderers;

    public MeshRenderer[] GetRenderers()
    {
        if(renderers!=null){
            return renderers;
        }
        return source.GetComponentsInChildren<MeshRenderer>(true);
    }

    public string name{
        get{
            if(source==null)return "";
            return source.name;
        }
    }

    public Transform transform
    {
        get{
            if(source==null)return null;
            return source.transform;
        }
    }

    public void DestroySource()
    {
        if(renderers!=null){
            foreach(var r in renderers)
            {
                if(r==null)continue;
                GameObject.Destroy(r.gameObject);
            }
        }
        else{
            GameObject.Destroy(source);
        }
        
    }
}