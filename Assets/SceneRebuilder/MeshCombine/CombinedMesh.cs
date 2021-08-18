using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

 [Serializable]
public class CombinedMesh{

    public string name;
    //public Mesh mesh;

    public List<MeshPartInfo> meshPartList;

    public Material mat;

    public Transform source;

    //public List<MeshFilter> meshFilters;
    //public List<int> meshIndexes;

    public SubMeshList meshList;

    public int VertexCount=0;

    public static int MaxVertex=65535;

    public static UnityEngine.Rendering.IndexFormat indexFormat=UnityEngine.Rendering.IndexFormat.UInt16;

    public Vector3[] minMax;

    public MeshCombineArg arg;

    public bool isCenterPivot = false;

    public CombinedMesh(MeshCombineArg arg, SubMeshList mfs, Material mat)
    {
        this.arg = arg;
        isCenterPivot = arg.isCenterPivot;
        Init(arg.transform, mfs, mat);
    }

    public CombinedMesh(Transform source, SubMeshList mfs, Material mat)
    {
        Init(source, mfs, mat);
    }

    private void Init(Transform source, SubMeshList mfs, Material mat)
    {
        //Debug.Log($"CombinedMesh.Init source:[{source}] mfs:[{mfs}] mat:{mat}");
        this.name = source.name;
        this.source = source;

        if (mfs == null || mfs.Count == 0)
        {
            var meshFilters = source.GetComponentsInChildren<MeshFilter>(true).ToList();
            meshList = new SubMeshList();
            foreach (var mf in meshFilters)
            {
                meshList.Add(new SubMesh(mf, 0));
            }
        }
        else
        {
            this.meshList = mfs;
        }
        //meshIndexes = new List<int>();
        this.mat = mat;
        minMax = MeshHelper.GetMinMax(meshList.GetMeshFilters());
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    /// <param name="renders"></param>
    /// <returns></returns>
    public static Bounds CaculateBounds(IEnumerable<MeshFilter> meshFilters)
    {
        //Debug.Log($"CaculateBounds renders:{renders.Count()},isAll:{isAll}");
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (MeshFilter meshFilter in meshFilters)
        {
            center += meshFilter.sharedMesh.bounds.center;
            count++;
        }

        if (count > 0)
        {
            center /= count;
        }
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (MeshFilter meshFilter in meshFilters)
        {
            bounds.Encapsulate(meshFilter.sharedMesh.bounds);
        }
        return bounds;
    }

    public static Mesh CloneMesh(MeshFilter mf)
    {
        Mesh mesh=GameObject.Instantiate(mf.sharedMesh);
        //Mesh mesh = mf.mesh;
        return mesh;
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

        Bounds bounds = CaculateBounds(mfs);
        //Vector3 off= new Vector3(bounds.extents.x, 0, 0);


        for (int i=0;i<count;i++)
        {
            MeshFilter mf=mfs[i];
            MeshRenderer mr=mf.GetComponent<MeshRenderer>();
            if(mr==null)continue;
            Mesh msold=mf.sharedMesh;
            if(msold.isReadable==false){
                Debug.LogError("模型必须设置为 Read/Write Enabled = True !! :"+ msold);
                continue;
            }


            Mesh ms = msold;
            if (isCenterPivot)
            {
                ms = CloneMesh(mf);
                //CenterPivot
                var vs = ms.vertices;
                var subMesh = ms.GetSubMesh(info.GetMeshIndex(i));
                var subMeshBounds = subMesh.bounds;
                Debug.LogError($"InnerDoCombine id:{id} i:{i} count:{count} name:{mf.name} subMeshIndex:{info.GetMeshIndex(i)},bounds:{subMeshBounds}");
                Vector3 off = -subMeshBounds.center;
                for (int j = 0; j < subMesh.vertexCount; j++)
                {
                    vs[subMesh.firstVertex + j] += off;
                }
                if (count == 1)
                {
                    info.offset = off;
                }
                ms.vertices = vs;
            }

            combines[i].mesh=ms;
            combines[i].subMeshIndex = info.GetMeshIndex(i);
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
        //Debug.Log("indexFormat:"+indexFormat);
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

    public List<MeshPartInfo> GetMeshPartInfoList()
    {
        List<MeshPartInfo> allList = new List<MeshPartInfo>();
        MeshPartInfo list = new MeshPartInfo();
        allList.Add(list);

        VertexCount = 0;
        int vcSum = 0;
        //int count = meshFilters.Count;
        //if(meshFilters.Count!= meshIndexes.Count)
        //{
        //    Debug.LogError($"meshFilters.Count!= meshIndexes.Count meshes:{meshFilters.Count},indexes{meshIndexes.Count},name:{name}");
        //    return null;
        //}
        for (int i = 0; i < meshList.Count; i++)
        {
            var mesh = meshList[i];
            if (mesh.sharedMesh == null) continue;
            int meshId = mesh.meshIndex;
            Mesh ms = mesh.sharedMesh;
            MeshFilter mf = mesh.meshFilter;
            if(meshId>=ms.subMeshCount)
            {
                Debug.LogError($"CombinedMesh.GetMeshPartInfoList[{i}] mesh:{mesh.sharedMesh.name} id:{meshId} count:{ms.subMeshCount}");
                meshId = 0;
            }

            //Debug.Log(string.Format("DoCombine[{0}/{1}]:{2}",i,count,VertexCount));
            //int vc=ms.vertexCount;
            int vc = (int)(ms.GetIndexCount(meshId));
            VertexCount += vc;

            if (vcSum + vc > MaxVertex)//判断数量
            {
                vcSum = vc;
                list = new MeshPartInfo();//新的
                list.Add(mf, meshId, vc);
                allList.Add(list);
            }
            else
            {
                vcSum += vc;
                list.Add(mf, meshId, vc);
            }
        }
        return allList;
    }

    public int DoCombine(bool logTime){

        DateTime start = DateTime.Now;

        List<MeshPartInfo> allList = GetMeshPartInfoList();

        meshPartList =new List<MeshPartInfo>();

        //Debug.Log(string.Format("DoCombine allList.Count:",allList.Count));
        for(int i=0;i<allList.Count;i++)
        {
            //Debug.LogWarning(string.Format("DoCombine {0} ({1}/{2})",allList[i].mesh,i+1,allList.Count));
            var partInfo = allList[i];
            if (partInfo.GetMeshCount() == 0)
            {
                Debug.LogError($"DoCombine partInfo.GetMeshCount()== 0 MaxVertex:{MaxVertex},all:{allList.Count} id:{i} mesh:{name} VertexCount:{VertexCount}");
                continue;
            }
            var newMesh=InnerDoCombine(partInfo, i);//合并核心代码
            if(newMesh!=null) meshPartList.Add(newMesh);
        }

        
        //Debug.LogWarning(
        //    string.Format("CombinedMesh 用时:{1}ms,Mesh数量:{1} 子模型数:{2},VertexCount:{3},Mat:{4}"
        //    , (DateTime.Now - start).TotalMilliseconds, meshList.Count, allList.Count, VertexCount, mat)
        //    );

        return VertexCount;
    }

    public IEnumerator DoCombine_Coroutine(bool logTime,int waitCount){

        List<MeshPartInfo> allList=new List<MeshPartInfo>();
        List<MeshFilter> list=new List<MeshFilter>();
        List<int> indexes = new List<int>();
        allList.Add(new MeshPartInfo(list, indexes));

        VertexCount=0;
        int vcSum=0;
        int count=meshList.Count;
        for(int i=0;i<count;i++)
        {
            var mesh = meshList[i];
            MeshFilter mf = mesh.meshFilter;
            int id = mesh.meshIndex;
            Mesh ms=mf.sharedMesh;
            //Debug.Log(string.Format("DoCombine[{0}/{1}]:{2}",i,count,VertexCount));
            int vc=ms.vertexCount;
            VertexCount+=vc;

            if(vcSum+vc>MaxVertex)
            {
                vcSum=vc;
                list=new List<MeshFilter>();
                list.Add(mf);
                indexes = new List<int>();
                indexes.Add(id);
                allList.Add(new MeshPartInfo(list, indexes));
            }
            else{
                vcSum+=vc;
                list.Add(mf);
                indexes.Add(id);
            }
        }
        meshPartList=new List<MeshPartInfo>();
        //Debug.Log(string.Format("DoCombine[{0}/{1}]:{2}",i,count,VertexCount));
        Debug.Log(string.Format("DoCombine allList.Count:{0}",allList.Count));
        for(int i=0;i<allList.Count;i++)
        {
            Debug.LogWarning(string.Format("DoCombine_Coroutine {0} ({1}/{2})",allList[i].mesh,i+1,allList.Count));                                                                                                                                                                                                                                                                                                                                                                                                                            
            if(i % waitCount == 0){
                yield return null;
            }
            var partInfo = allList[i];
            if (partInfo.GetMeshCount() == 0)
            {
                Debug.LogError($"DoCombine_Coroutine partInfo.GetMeshCount()== 0 all:{allList.Count} id:{i} mesh:{name}");
                continue;
            }
            var newMesh=InnerDoCombine(partInfo, i);
            if(newMesh!=null) meshPartList.Add(newMesh);
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
        Vector3 offset = Vector3.zero;
        if(target==null){
            target=new GameObject();
            target.name=source.name+"_Combined_N";
        }
        if(meshPartList.Count==1){
            this.SetRendererAndFilter(target,meshPartList[0]);
            if(enableCollider)
                this.SetCollider(target,meshPartList[0]);
            offset = meshPartList[0].offset;
        }
        else{
            for(int i=0;i<meshPartList.Count;i++){
                var info=meshPartList[i];
                if (info == null) continue;
                GameObject subObj=new GameObject();
                //subObj.name = i + "_" + info.mesh.name;
                subObj.name = info.mesh.name;
                subObj.transform.SetParent(target.transform);

                this.SetRendererAndFilter(subObj,info);
                if(enableCollider)
                    this.SetCollider(subObj,info);
            }
        }
        
        target.transform.position=source.transform.position- offset;//坐标一致,不设置的话，就是按照新的target的坐标来
        //target.transform.position=Vector3.zero;
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

        //MeshCombineHelper.CenterPivot(target.transform,minMax[3]);

        return target;
    }


}
