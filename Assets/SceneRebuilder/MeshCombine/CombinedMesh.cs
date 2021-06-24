using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

 [Serializable]
public class MeshPartInfo{
    public Mesh mesh;

    public Material[] mats;

    public List<MeshFilter> meshFilters;

    public int vertexCount=0;

    public MeshPartInfo(List<MeshFilter> mfs){
        meshFilters=mfs;
    }

    public MeshPartInfo(){
        meshFilters=new List<MeshFilter>();
    }

    public void Add(MeshFilter mf)
    {
        meshFilters.Add(mf);
        vertexCount+=mf.sharedMesh.vertexCount;
    }

    public void Add(MeshFilter mf,int vc)
    {
        meshFilters.Add(mf);
        vertexCount+=vc;
    }
}

 [Serializable]
public class CombinedMesh{

    public string name;
    //public Mesh mesh;

    public List<MeshPartInfo> meshes;

    public Material mat;

    public Transform source;
    public List<MeshFilter> meshFilters;

    public int VertexCount=0;

    public static int MaxVertex=65535;

    public static UnityEngine.Rendering.IndexFormat indexFormat=UnityEngine.Rendering.IndexFormat.UInt16;

    public CombinedMesh(Transform source,List<MeshFilter> mfs, Material mat){
        this.source=source;

        if(mfs==null || mfs.Count==0){
            this.meshFilters=source.GetComponentsInChildren<MeshFilter>().ToList();
        }
        else{
            this.meshFilters=new List<MeshFilter>(mfs);
        }
        this.mat=mat;
        //SimpleCombine();

        //Debug.LogError("CombinedMesh Read/Write Enabled = True !! :"+ms);
    }

    private MeshPartInfo InnerDoCombine(MeshPartInfo info,int id){

        List<MeshFilter> mfs=info.meshFilters;
        int count=mfs.Count;
        if(count==0){
            Debug.LogError("meshFiltersToCombined.Count == 0");
            return null;
        }
        CombineInstance[] combines=new CombineInstance[count];
        Material[] mats=null;
        if(mat==null)
            mats=new Material[count];
        Matrix4x4 matrix=source.worldToLocalMatrix;
        for(int i=0;i<count;i++)
        {
            MeshFilter mf=mfs[i];
            MeshRenderer mr=mf.GetComponent<MeshRenderer>();
            if(mr==null)continue;
            Mesh ms=mf.sharedMesh;
            if(ms.isReadable==false){
                Debug.LogError("模型必须设置为 Read/Write Enabled = True !! :"+ms);
                continue;
            }
            combines[i].mesh=ms;
            combines[i].transform=matrix*mf.transform.localToWorldMatrix;
            //combines[i].transform=mf.transform;
            mr.enabled=false;//不渲染原模型

            if(mat==null)
                mats[i]=mr.sharedMaterial;

            MeshCombineHelper.AddGo(mf.gameObject,this);
        }
        //source.gameObject.SetActive(false);

        Mesh newMesh=new Mesh();
        newMesh.indexFormat=indexFormat;
        newMesh.name=source.name+"_Combined"+id;
        if(mat!=null){
            newMesh.name=mat.name+"_Combined"+id;
        }
        bool mergeSubMeshes=mat!=null;

        newMesh.CombineMeshes(combines,mergeSubMeshes);//核心 合并网格API

        // MeshPartInfo info=new MeshPartInfo();
        info.mesh=newMesh;
        info.mats=mats;
        return info;
    }

    public GameObject Refresh(){
        this.DoCombine(true);
        GameObject target=this.CreateNewGo(false,NewGo);
        return target;
    }

    public void DoCombine(bool logTime){

        List<MeshPartInfo> allList=new List<MeshPartInfo>();
        MeshPartInfo list=new MeshPartInfo();
        allList.Add(list);

        VertexCount=0;
        int vcSum=0;
        int count=meshFilters.Count;
        for(int i=0;i<count;i++)
        {
            MeshFilter mf=meshFilters[i];
            if (mf.sharedMesh == null) continue;
            Mesh ms=mf.sharedMesh;
            //Debug.Log(string.Format("DoCombine[{0}/{1}]:{2}",i,count,VertexCount));
            int vc=ms.vertexCount;
            VertexCount+=vc;

            if(vcSum+vc>MaxVertex)//判断数量
            {
                vcSum=vc;
                list=new MeshPartInfo();//新的
                list.Add(mf,vc);
                allList.Add(list);
            }
            else{
                vcSum+=vc;
                list.Add(mf,vc);
            }
        }
        meshes=new List<MeshPartInfo>();

        //Debug.Log(string.Format("DoCombine allList.Count:",allList.Count));
        for(int i=0;i<allList.Count;i++)
        {
            //Debug.LogWarning(string.Format("DoCombine {0} ({1}/{2})",allList[i].mesh,i+1,allList.Count));
            var newMesh=InnerDoCombine(allList[i],i);//合并核心代码
            if(newMesh!=null) meshes.Add(newMesh);
        }

        DateTime start=DateTime.Now;
        // Debug.LogWarning(
        //     string.Format("CombinedMesh 用时:{1}ms,Mesh数量:{1} 子模型数:{2},VertexCount:{3},Mat:{4}"
        //     ,(DateTime.Now-start).TotalMilliseconds,count,allList.Count,VertexCount,mat)
        //     );
    }

    public IEnumerator DoCombine_Coroutine(bool logTime,int waitCount){

        List<MeshPartInfo> allList=new List<MeshPartInfo>();
        List<MeshFilter> list=new List<MeshFilter>();
        allList.Add(new MeshPartInfo(list));

        VertexCount=0;
        int vcSum=0;
        int count=meshFilters.Count;
        for(int i=0;i<count;i++)
        {
            MeshFilter mf=meshFilters[i];
            Mesh ms=mf.sharedMesh;
            //Debug.Log(string.Format("DoCombine[{0}/{1}]:{2}",i,count,VertexCount));
            int vc=ms.vertexCount;
            VertexCount+=vc;

            if(vcSum+vc>MaxVertex)
            {
                vcSum=vc;
                list=new List<MeshFilter>();
                list.Add(mf);
                allList.Add(new MeshPartInfo(list));
            }
            else{
                vcSum+=vc;
                list.Add(mf);
            }
        }
        meshes=new List<MeshPartInfo>();
        //Debug.Log(string.Format("DoCombine[{0}/{1}]:{2}",i,count,VertexCount));
        Debug.Log(string.Format("DoCombine allList.Count:{0}",allList.Count));
        for(int i=0;i<allList.Count;i++)
        {
            Debug.LogWarning(string.Format("DoCombine_Coroutine {0} ({1}/{2})",allList[i].mesh,i+1,allList.Count));                                                                                                                                                                                                                                                                                                                                                                                                                            
            if(i % waitCount == 0){
                yield return null;
            }
            var newMesh=InnerDoCombine(allList[i],i);
            if(newMesh!=null) meshes.Add(newMesh);
        }

        DateTime start=DateTime.Now;
        Debug.LogWarning(
            string.Format("CombinedMesh 用时:{1},Mesh数量:{1} 子模型数:{2},VertexCount:{3},Mat:{4}"
            ,(DateTime.Now-start),count,allList.Count,VertexCount,mat)
            );
        yield return null;
    }

    public void SetRendererAndFilter(GameObject go,MeshPartInfo ms){
        MeshFilter meshFilter=go.GetComponent<MeshFilter>();
        if(meshFilter==null){
            meshFilter=go.AddComponent<MeshFilter>();
        }
        meshFilter.mesh=ms.mesh;
        MeshRenderer meshRenderer=go.GetComponent<MeshRenderer>();
        if(meshRenderer==null){
            meshRenderer=go.AddComponent<MeshRenderer>();
        }

        if(mat!=null){
            meshRenderer.sharedMaterial=mat;
        }
        else
        {
            meshRenderer.sharedMaterials=ms.mats;
        }
        
        meshRenderer.enabled=true;
    }
    
    public void SetCollider(GameObject go,MeshPartInfo ms){
        Collider collider=go.GetComponent<Collider>();
        if(collider!=null){
            GameObject.DestroyImmediate(collider);
        }
        MeshCollider meshCollider=go.AddComponent<MeshCollider>();
        meshCollider.sharedMesh=ms.mesh;
    }

    public GameObject NewGo;

    public GameObject CreateNewGo(bool enableCollider,GameObject target){
        if(target==null){
            target=new GameObject();
            target.name=source.name+"_Combined";
        }
        if(meshes.Count==1){
            this.SetRendererAndFilter(target,meshes[0]);
            if(enableCollider)
                this.SetCollider(target,meshes[0]);
        }
        else{
            for(int i=0;i<meshes.Count;i++){
                var info=meshes[i];
                if (info == null) continue;
                GameObject subObj=new GameObject();
                subObj.name=info.mesh.name;
                subObj.transform.SetParent(target.transform);

                this.SetRendererAndFilter(subObj,info);
                if(enableCollider)
                    this.SetCollider(subObj,info);
            }
        }
        
        target.transform.position=source.transform.position;//坐标一致,不设置的话，就是按照新的target的坐标来
        target.transform.localRotation=source.transform.localRotation;//坐标一致,不设置的话，就是按照新的target的坐标来
        target.transform.localScale=source.transform.localScale;//坐标一致,不设置的话，就是按照新的target的坐标来

        if(MeshCombineHelper.AddScripted)
        {
            var meshInfo = target.AddComponent<CombinedMeshInfo>();
            meshInfo.combinedMesh = this;
        }

        name=target.name;
        NewGo=target;

        target.transform.SetParent(source.transform.parent);
        return target;
    }
}

public static class MeshCombineHelper
{

    public static bool AddScripted = false;

    public static GameObject SimpleCombine(GameObject source,GameObject target){
        
        CombinedMesh combinedMesh=new CombinedMesh(source.transform,null,null);
        combinedMesh.DoCombine(true);
        target=combinedMesh.CreateNewGo(false,target);
        target.AddComponent<MeshFilterInfo>();
        Debug.Log("Combine:"+source+"->"+target);
        return target;
    }

    public static IEnumerator SimpleCombine_Coroutine(GameObject source,GameObject target,int waitCount,bool isDestroy){
        
        CombinedMesh combinedMesh=new CombinedMesh(source.transform,null,null);
        yield return combinedMesh.DoCombine_Coroutine(true,waitCount);
        target=combinedMesh.CreateNewGo(false,target);
        target.AddComponent<MeshFilterInfo>();
        Debug.Log("Combine:"+source+"->"+target);
        if(isDestroy){
            GameObject.Destroy(source);
        }
        yield return target;
    }
    
    public static GameObject CombineMaterials(GameObject go,out int  count){
        DateTime start=DateTime.Now;
        GameObject goNew=new GameObject();
        goNew.name=go.name+"_Combined";
        MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>(true);
        //int count=0;
        Dictionary<Material,List<MeshFilter>> mat2Filters=GetMatFilters(renderers, out count);
        string mats="";
        foreach(var item in mat2Filters)
        {
            Material material=item.Key;
            mats+=material.name+";";
            List<MeshFilter> list=item.Value;

            CombinedMesh combinedMesh=new CombinedMesh(go.transform,list,material);
            combinedMesh.DoCombine(true);
            GameObject matGo=combinedMesh.CreateNewGo(false,null);
            matGo.name=material.name;
            matGo.transform.SetParent(goNew.transform);
        }

        goNew.transform.SetParent(go.transform.parent);
        Debug.Log(string.Format("CombineMaterials 用时:{0} \tMesh数量:{2} \tMat数量:{1} \tMats:{3}",(DateTime.Now-start),mat2Filters.Count,count,mats));
        return goNew;
    }

     public static IEnumerator CombineMaterials_Coroutine(GameObject go,int waitCount,bool isDestroy){
        DateTime start=DateTime.Now;
        GameObject goNew=new GameObject();
        goNew.name=go.name+"_Combined";
        goNew.transform.SetParent(go.transform.parent);
        int count=0;
        MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
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
            GameObject.Destroy(go);
        }
        Debug.LogError(string.Format("CombineMaterials 用时:{0},Mat数量:{1},Mesh数量:{2}",(DateTime.Now-start),mat2Filters.Count,count));
        yield return goNew;
    }

    public class MatInfo
    {
        public Material Mat;

        public string Key = "";

        public MatInfo(Material mat)
        {
            this.Mat = mat;

            Key = GetMatKey(mat);
        }

        public static string GetMatKey(Material mat)
        {
            return $"{mat.GetColor("_BaseColor")},{mat.GetFloat("_Metallic")},{mat.GetFloat("_Smoothness")},{mat.GetTexture("_BaseColorMap")},{mat.GetTexture("_NormalMap")}";
        }

        public List<MeshFilter> MeshFilters = new List<MeshFilter>();

        public void AddList(List<MeshFilter> list)
        {
            MeshFilters.AddRange(list);
        }
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
            var list = mats[mat];
            foreach (var item in list)
            {
                MeshRenderer renderer = item.GetComponent<MeshRenderer>();
                renderer.sharedMaterial = mat;
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
            if (!mat2Filters.ContainsKey(renderer.sharedMaterial))
            {
                mat2Filters.Add(renderer.sharedMaterial, new List<MeshFilter>());
            }
            List<MeshFilter> list = mat2Filters[renderer.sharedMaterial];
            MeshFilter filter = renderer.GetComponent<MeshFilter>();
            list.Add(filter);
        }

        Dictionary<string, MatInfo> infos = new Dictionary<string, MatInfo>();
        foreach (var mat in mat2Filters.Keys)
        {
            var list = mat2Filters[mat];
            MatInfo info = new MatInfo(mat);
            if (infos.ContainsKey(info.Key))
            {
                var item = infos[info.Key];
                item.AddList(list);
            }
            else
            {
                infos.Add(info.Key, info);
                info.AddList(list);
            }
        }

        Dictionary<Material, List<MeshFilter>> mat2Filters2 = new Dictionary<Material, List<MeshFilter>>();
        foreach (var info in infos.Values)
        {
            mat2Filters2.Add(info.Mat, info.MeshFilters);
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

    public static GameObject Combine(GameObject source){
        DateTime start=DateTime.Now;
        int count=0;
        GameObject goNew=CombineMaterials(source,out count);//按材质合并
        CombinedMesh combinedMesh=new CombinedMesh(goNew.transform,null,null);
        combinedMesh.DoCombine(false);
        GameObject target=combinedMesh.CreateNewGo(false,null);
        target.name=source.name+"_Combined";
        goNew.transform.SetParent(target.transform);
        GameObject.DestroyImmediate(goNew);
        //Debug.LogError(string.Format("CombinedMesh 用时:{0}ms,数量:{1}",(DateTime.Now-start).TotalMilliseconds,count));
        return target;
    }

    public static IEnumerator Combine_Coroutine(GameObject source,int waitCount,bool isDestroy){
        DateTime start=DateTime.Now;
        int count=0;
        GameObject goNew=CombineMaterials(source,out count);//按材质合并
        CombinedMesh combinedMesh=new CombinedMesh(goNew.transform,null,null);
        yield return combinedMesh.DoCombine_Coroutine(false,waitCount);
        GameObject target=combinedMesh.CreateNewGo(false,null);
        target.name=source.name+"_Combined";
        goNew.transform.SetParent(target.transform);
        GameObject.DestroyImmediate(goNew);
        //Debug.LogError(string.Format("CombinedMesh 用时:{0}ms,数量:{1}",(DateTime.Now-start).TotalMilliseconds,count));
        if(isDestroy)
        {
            GameObject.Destroy(source);
        }
        yield return target;
    }

    public static GameObject CombineEx(GameObject source,int mode=0){
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

    public static IEnumerator CombineEx_Coroutine(GameObject source,bool isDestroy,int waitCount,int mode=0){
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
            foreach(var mf in meshFilters){
                mesh.meshFilters.Remove(mf);
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