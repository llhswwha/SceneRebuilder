using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MeshJobs;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using GPUInstancer;
using System.IO;

public class PrefabInstanceBuilder : SingletonBehaviour<PrefabInstanceBuilder>
{
    public AlignDebugStep step;

    public AlignRotateMode mode;

    public GameObject TargetRoots;

    public void SetTarget(GameObject root)
    {
        Debug.Log($"PrefabInstanceBuilder.SetTarget root:{root}");
        TargetRoots = root;

        if(TargetRootsCopy!=null)
            GameObject.DestroyImmediate(TargetRootsCopy);
    }

    public GameObject TargetRootsCopy;

    public int TargetCount=0;

    public int MaxTargetCount=0;

    public float MaxAngle = 10;

    //public double maxDistance = 5;
    //public double zero = 0.0004f;
    //public int zeroMax = 100;

    //public int ICPMaxCount = 20;

    //public float ICPMinDis = 0.2f;

    public DisSetting disSetting = new DisSetting();

    public List<GameObject> TargetList=new List<GameObject>();

    public List<MeshNode> TargetNodeList=new List<MeshNode>();

    public List<GameObject> PrefabList=new List<GameObject>();

    public int[] InsCountList=new int[]{1,5,10,50,100};

    public PrefabInfoList PrefabInfoList=new PrefabInfoList();

    public PrefabInfoListBags PrefabInfoListBags = new PrefabInfoListBags();

    public GameObject CurrentPrefab=null;

    public int TryRotateAngleCount=20;

    public Vector3 centerOffset=new Vector3(0.0005f,0.0010f,0.0015f);

    public Vector3 testModelOff=new Vector3(1f,1f,1f);

    public enum TestPosMode
    {
        None,
        RandomSphere,
        Line
    }

    public bool IsCopyTargetRoot=false;

    public TestPosMode PosMode;

    //public bool RandomPos=true;

    public float RandomRadius=10;

    public void DestroySimgleOnes()
    {
        List<Transform> children=new List<Transform>();
         for(int i=0;i<TargetRoots.transform.childCount;i++)
        {
            var child=TargetRoots.transform.GetChild(i);
            children.Add(child);
        }

         for(int i=0;i<children.Count;i++)
        {
            var child=children[i];
            if(child.childCount==0){
                GameObject.DestroyImmediate(child.gameObject);
            }
            else{

            }
        }

        // var mfs=GetMeshFilters();
        // for(int i=0;i<mfs.Length;i++)
        // {
        //     var t=mfs[i].transform;
        //     //MeshHelper.SetParentZero(t.gameObject);
        //     t.transform.position=testModelOff*i;
        // }
    }

    public void ResetPrefabs()
    {
        // PrefabInfoList.Sort((a,b)=>{
        //     return a.VertexCount.CompareTo(b.VertexCount);
        // });
        GameObject PrefabRoots=new GameObject("Prefabs"+PrefabInfoList.Count);
        PrefabInfoList.Sort((a,b)=>{
            return a.SizeVolumn.CompareTo(b.SizeVolumn);
        });
    for (int i = 0; i < PrefabInfoList.Count; i++)
        {
            PrefabInfo info = PrefabInfoList[i];
            info.Prefab.transform.position=Vector3.zero;
            info.Prefab.transform.position = testModelOff * i;
            info.Prefab.transform.SetParent(PrefabRoots.transform);
        }
    }

    public void DestoryInstances()
    {

        PrefabInfoList.DestroyInstances();
    }

    public void ShowPrefabListCount()
    {
        //DateTime start=DateTime.Now;
        //Debug.Log($"all:{PrefabInfoList.Count}={PrefabInfoList.GetInstanceCount()}\t list1:{PrefabInfoList1.Count}={PrefabInfoList1.GetInstanceCount()}\t list2:{PrefabInfoList2.Count}={PrefabInfoList2.GetInstanceCount()}\t list3:{PrefabInfoList3.Count}={PrefabInfoList3.GetInstanceCount()}\t list4:{PrefabInfoList4.Count}={PrefabInfoList4.GetInstanceCount()}\t list5:{PrefabInfoList5.Count}={PrefabInfoList5.GetInstanceCount()} \tlist6:{PrefabInfoList6.Count}={PrefabInfoList6.GetInstanceCount()},");
        //Debug.Log($"ShowPrefabListCount Time:{(DateTime.Now-start).ToString()}");
        PrefabInfoListBags.ShowPrefabListCount();
    }

    //public void ShowPrefabCount()
    //{
    //    PrefabInfoList.ShowPrefabCount();
    //}

    public void ShowInstanceCount()
    {
        PrefabInfoList.ShowInstanceCount();
    }

    public void GetTargetCount()
    {
        var target=GetTarget();
        TargetCount=GetMeshFilters().Length;
        Debug.Log("TargetCount:"+TargetCount);
    }

    
    public void CreatePrefab()
    {
        #if UNITY_EDITOR
        if(TargetRoots==null&&transform.childCount==1){
            SetTarget(transform.GetChild(0).gameObject);
        }
        string path="Assets/Models/Prefabs/"+TargetRoots.name+".prefab";
        PrefabUtility.SaveAsPrefabAssetAndConnect(TargetRoots,path,InteractionMode.UserAction);
        #endif
    }

    public void CreateTestModel()
    {
UnpackPrefab();

        DateTime start=DateTime.Now;

        CleanNodes();

        TargetList.Clear();
        var meshFilters=GetMeshFilters();
        TargetCount=meshFilters.Length;
        int max=MaxTargetCount;
        if(max==0){
            max=int.MaxValue;
        }
        for (int i = 0; i < TargetCount; i++)
        {
            float progress = (float)i / TargetCount;
            float percents = progress * 100;
            
            if(ProgressBarHelper.DisplayCancelableProgressBar("CreateTestModel", $"{i}/{TargetCount} {percents:F2}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
            var mf = meshFilters[i];
            if(mf==null)
            {
                continue;
            }
            if(PosMode==TestPosMode.RandomSphere){
                mf.gameObject.transform.position=UnityEngine.Random.insideUnitSphere*RandomRadius;
            }
            else if(PosMode==TestPosMode.Line){
                mf.gameObject.transform.position = testModelOff * i;
            }

            if (i<max){
                TargetList.Add(mf.gameObject);
            }
            else{
                GameObject.DestroyImmediate(mf.gameObject);
            }

            
        }

        ProgressBarHelper.ClearProgressBar();

        Debug.LogWarning($"CreateTestModel Count:{TargetCount},Time:{(DateTime.Now-start).ToString()}");
    }

    public static void UpackPrefab_One(GameObject go)
    {
#if UNITY_EDITOR
        GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
        if (root != null)
        {
            PrefabUtility.UnpackPrefabInstance(root, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        }
#endif
    }
    public void UnpackPrefab()
    {
        if(TargetRoots==null&&transform.childCount==1){
            SetTarget(transform.GetChild(0).gameObject);
        }
#if UNITY_EDITOR
        if(TargetRoots==null) return;
        UpackPrefab_One(TargetRoots);


        for (int i = 0; i < TargetRoots.transform.childCount; i++)
        {
            var child = TargetRoots.transform.GetChild(i);
            UpackPrefab_One(child.gameObject);
        }
#endif

        ShowRenderers();
    }

    public void ShowRenderers()
    {
        DateTime start=DateTime.Now;

        if(TargetRoots==null && TargetRootsCopy==null)
        {
            GetTarget();
        }

        if(TargetRoots && TargetRootsCopy==null)
        {
            TargetRoots.gameObject.SetActive(true);
            var ts=AreaTreeHelper.GetAllTransforms(TargetRoots.transform);
            foreach(var t in ts){
                t.gameObject.SetActive(true);
            }
        }

        else if(TargetRootsCopy)
        {
            TargetRootsCopy.gameObject.SetActive(true);
            var ts=AreaTreeHelper.GetAllTransforms(TargetRootsCopy.transform);
            foreach(var t in ts){
                t.gameObject.SetActive(true);
            }
        }
        

        var renderers=this.GetMeshRenderers();
        foreach(var render in renderers){
            render.enabled=true;
            render.gameObject.SetActive(true);
        }
        Debug.Log($"ShowRenderers renderers:{renderers.Length},\t{(DateTime.Now-start).ToString()}");
    }

    public int maxLoopCount=0;

    public void CreateInstance()
    {
        UnpackPrefab();

        MeshData.InvokeGetVertexCenterInfoCount=0;
        MeshHelper.InvokeGetVertexDistanceExCount=0;
        MeshHelper.TryAlignMeshNodeCount=0;
        MeshHelper.NoAlignCount=0;

        MeshHelper.step = this.step;
        MeshHelper.mode = this.mode;

        DateTime start=DateTime.Now;

        //1.统一获取信息，有进度条
        GetVertexCenterInfos();

        int max=int.MaxValue;
        if(maxLoopCount>0){
            max=maxLoopCount;
        }

        PrefabList.Clear();
        PrefabInfoList.Clear();

        var currentTarget=TargetList;
        int count=TargetList.Count;

        //2.逐个替换模型为预设
        for(int i=0;i<count && i<max;i++)
        {
            CurrentPrefab=TargetList[0];//第一个作为预设
            PrefabList.Add(CurrentPrefab);
            PrefabInfo prefabInfo=new PrefabInfo(CurrentPrefab);
            PrefabInfoList.Add(prefabInfo);

            List<GameObject> newModels=ReplaceToPrefab_Core(prefabInfo,TargetList);
            if(newModels.Count<=0){
                break;//退出
            }
            else{
                TargetList=newModels;//继续
            }
        }

        CleanNodes();

        Debug.LogWarning($"CreateInstance Count:{count},Time:{(DateTime.Now-start).ToString()}");
        Debug.LogWarning($"GetVertexCenterInfo:{MeshData.InvokeGetVertexCenterInfoCount},GetVertexDistanceEx:{MeshHelper.InvokeGetVertexDistanceExCount},TryAlignMeshNodeCount:{MeshHelper.TryAlignMeshNodeCount},NoAlignCount:{MeshHelper.NoAlignCount}");
    }

    private void CleanNodes()
    {
        var nodes=GetNodes();
        foreach(var node in nodes)
        {
            if(node==null)continue;
            if(node.gameObject==null)continue;
            node.ClearVertexes();
            GameObject.DestroyImmediate(node);
        }
    }
    
    public bool isShowDetail = false;

    public List<GameObject> ReplaceToPrefab_Core(PrefabInfo prefabInfo,List<GameObject> models)
    {
        GameObject prefabGo=prefabInfo.Prefab;

        List<GameObject> newModels=new List<GameObject>();
        //List<GameObject> instances=new List<GameObject>();
        DateTime start=DateTime.Now;
        for (int i = 0; i < models.Count; i++)
        {
            float progress = (float)i /models.Count;
            float percents = progress * 100;
            
            if(ProgressBarHelper.DisplayCancelableProgressBar("ReplaceToPrefab_Core", $"{i}/{models.Count} {percents:F2}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
            GameObject go = models[i];
            if(go==null)
            {
                continue;
            }
            if(go==prefabGo)//项目的模型
            {
                continue;
            }

            GameObject newGo=MeshHelper.CopyGO(prefabGo);//复制预设
            var r = MeshHelper.ReplaceToPrefab(newGo,go,TryRotateAngleCount,centerOffset, isShowDetail);//替换模型
            if(r==false)//无法替换
            {
                //不是同类模型
                if(isShowDetail==false)
                {
                    GameObject.DestroyImmediate(newGo);
                }
                newModels.Add(go);
            }
            else{
                prefabInfo.AddInstance(newGo);
            }
        }
        ProgressBarHelper.ClearProgressBar();

        if(isShowDetail) Debug.LogWarning($"ReplaceToPrefab_Core Count:{models.Count},Time:{(DateTime.Now-start).ToString()},NewCount:{newModels.Count}");
        return newModels;
    }

    public int CurrentTestId=0;
    
    public MeshComparer meshComparer;

    public List<GameObject> ReplaceToPrefab_Test(GameObject prefabGo,List<GameObject> models,int i)
    {
        List<GameObject> newModels=new List<GameObject>();
        List<GameObject> instances=new List<GameObject>();
        DateTime start=DateTime.Now;
        //for (int i = 0; i < models.Count; i++)
        //{
            float progress = (float)i /models.Count;
            float percents = progress * 100;
            
            GameObject go = models[i];
            if(go==null)
            {
                Debug.LogWarning("go==null");
                return newModels;
            }
            if(go==prefabGo)//项目的模型
            {
                Debug.LogWarning("go==prefabGo");
                return newModels;
            }

            GameObject newGo=MeshHelper.CopyGO(prefabGo);
            var r = MeshHelper.ReplaceToPrefab(newGo,go,TryRotateAngleCount,centerOffset,true);
            if(r==false){//不是同类模型
                GameObject.DestroyImmediate(newGo);
                newModels.Add(go);
            }
            else{
                instances.Add(newGo);
            }
        //}
        ProgressBarHelper.ClearProgressBar();

        Debug.LogWarning($"ReplaceToPrefab_Test i:{i},r:{r},Count:{models.Count},Time:{(DateTime.Now-start).ToString()},NewCount:{newModels.Count}");
        return newModels;
    }


    private List<MeshNode> GetNodes()
    {
        List<MeshNode> nodes1=new List<MeshNode>();
        MeshNode[] nodes=null;
        if(TargetRoots){
            nodes=TargetRoots.GetComponentsInChildren<MeshNode>();
        }
        //else{
        //    nodes=GameObject.FindObjectsOfType<MeshNode>();
        //}
        if(nodes!=null)
            nodes1.AddRange(nodes);
       //TargetNodeList.Clear();
        nodes1.AddRange(TargetNodeList);
        return nodes1;
    }

    public void ClearMeshFilters(List<MeshFilter> list)
    {
        var mfs=GameObject.FindObjectsOfType<MeshFilter>();
        foreach(var mf in mfs){
            if(mf==null)continue;
            if (mf.gameObject == null) continue;
            try
            {
                if (list == null || !list.Contains(mf))
                {
                    GameObject.DestroyImmediate(mf.gameObject);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("ClearMeshFilters Exception:" + mf.name+"|"+ex.ToString());
            }

        }
    }

    public GameObject GetTarget()
    {
        if(TargetRoots==null&& TargetRootsCopy==null)
        {
            var modelRoot=GameObject.FindObjectOfType<ModelRoot>();
            if(modelRoot){
                SetTarget(modelRoot.gameObject);
            }
        }
        if(TargetRootsCopy)
        {
            return TargetRootsCopy;
        }
        return TargetRoots;
    }

    public MeshFilter[] meshFilters;

    public void SetMeshFilters(MeshFilter[] mfs)
    {
        this.meshFilters = mfs;
    }

    public MeshPoints[] GetMeshFilters()
    {
        if(TargetRoots==null&&TargetRootsCopy==null)
        {
            GetTarget();
        }

        if(TargetRoots){
            //meshFilters=TargetRoots.GetComponentsInChildren<MeshFilter>();
            if(IsCopyTargetRoot && TargetRootsCopy==null)
            {
                // if(TargetRootsCopy!=null){
                //     GameObject.DestroyImmediate(TargetRootsCopy);
                // }
                CopyTarget();
            }
        }
        MeshFilter[] meshFilters=null;
        if(TargetRootsCopy){
            var meshFilters1=TargetRootsCopy.GetComponentsInChildren<MeshFilter>(true);
            var meshFilters2 = TargetRoots.GetComponentsInChildren<MeshFilter>(true);
            if (meshFilters1.Length == 0 && meshFilters2.Length > 0)
            {
                CopyTarget();
                //meshFilters1 = TargetRootsCopy.GetComponentsInChildren<MeshFilter>(true);
                return MeshPoints.GetMeshPointsNoLOD(TargetRootsCopy).ToArray();
            }
            //meshFilters = meshFilters1;
            return MeshPoints.GetMeshPointsNoLOD(TargetRoots).ToArray();
        }
        else if(TargetRoots){
            //meshFilters=TargetRoots.GetComponentsInChildren<MeshFilter>(true);
            return MeshPoints.GetMeshPointsNoLOD(TargetRoots).ToArray();
        }
        else
        {
            return null;
        }
        ////else{
        ////    meshFilters=GameObject.FindObjectsOfType<MeshFilter>(true);
        ////}
        //List<MeshPoints> meshPoints = MeshPoints.GetMeshPoints(meshFilters);

        //Debug.LogError($"PrefabInstanceBuilder.GetMeshFilters meshFilters:{meshFilters.Length} TargetRootsCopy:[{GetGoName(TargetRootsCopy)}] TargetRoots:[{GetGoName(TargetRoots)}]");

        //return meshPoints.ToArray() ;
    }

    public string GetGoName(GameObject go)
    {
        if (go == null)
        {
            return "NULL";
        }
        if(go.transform.parent!=null)
        {
            return go.transform.parent.name + ">" + go.name;
        }
        else
        {
            return go.name;
        }
    }

    private void CopyTarget()
    {
        if (TargetRootsCopy != null)
        {
            GameObject.DestroyImmediate(TargetRootsCopy);
        }
        TargetRoots.SetActive(false);
        GameObject copy = MeshHelper.CopyGO(TargetRoots);
        copy.SetActive(true);
        TargetRootsCopy = copy;
    }

    private MeshRenderer[] GetMeshRenderers()
    {
        MeshRenderer[] meshRenderers=null;
        if(TargetRootsCopy){
            MeshRenderer[] meshRenderers1 = TargetRootsCopy.GetComponentsInChildren<MeshRenderer>(true);
            MeshRenderer[] meshRenderers2 = TargetRoots.GetComponentsInChildren<MeshRenderer>(true);
            if(meshRenderers1.Length==0&& meshRenderers2.Length>0)
            {
                CopyTarget();
                meshRenderers1 = TargetRootsCopy.GetComponentsInChildren<MeshRenderer>(true);
            }
            meshRenderers = meshRenderers1;
        }
        else if(TargetRoots){
            meshRenderers = TargetRoots.GetComponentsInChildren<MeshRenderer>(true);
        }
        //else{
        //    meshRenderers=GameObject.FindObjectsOfType<MeshRenderer>();
        //}
        return meshRenderers;
    }

    public int JobSize = 100;//40000;

    public bool SetParentNull=true;

    public void GetThreePointJobs()
    {
        UnpackPrefab();
         Debug.Log("GetThreePointJobs");
        var meshFilters=GetMeshFilters();
        MeshJobHelper.NewThreePointJobs(meshFilters,JobSize);
    }

    public void MeshAlignJobs()
    {
        UnpackPrefab();
        CleanNodes();
        Debug.Log("MeshAlignJob");
        var meshFilters=GetMeshFilters();
        PrefabInfoList.Clear();
        PrefabInfoList = MeshJobHelper.NewMeshAlignJobs(meshFilters,JobSize);
    }
    public void RTAlignJobs()
    {
        UnpackPrefab();
        CleanNodes();
        Debug.Log("RTAlignJobs");
        var meshFilters=GetMeshFilters();
        PrefabInfoList.Clear();
        PrefabInfoList = MeshJobHelper.NewRTAlignJobs(meshFilters,JobSize);
    }

    private void SetDistanceSettings()
    {
        AcRTAlignJob.MaxAngle = this.MaxAngle;
        //DistanceSetting.zeroMMaxDis = this.maxDistance;
        //DistanceSetting.zeroP = this.zero;//0.0004f
        //DistanceSetting.zeroPMaxCount = this.zeroMax;

        //DistanceSetting.ICPMaxCount=this.ICPMaxCount;
        //DistanceSetting.ICPMinDis=this.ICPMinDis;

        DistanceSetting.Set(this.disSetting);
    }

    public PrefabInfoList AcRTAlignJobs()
    {
        var meshFilters=GetMeshFilters();
        return AcRTAlignJobs(meshFilters);
        //return null;
    }

    public PrefabInfoList AcRTAlignJobs(GameObject target,bool isCopy)
    {

        SetTarget(target);
        IsCopyTargetRoot = isCopy;
        //if (isCopy == false && TargetRootsCopy!=null)
        //{
        //    GameObject.DestroyImmediate(TargetRootsCopy);
        //}
        return AcRTAlignJobs();
    }

    public PrefabInfoList AcRTAlignJobs(MeshPoints[] meshPoints)
    {
        SetDistanceSettings();
        UnpackPrefab();
        CleanNodes();
        Debug.Log("AcRTAlignJobs");
        SetPrefabInfoList(MeshJobHelper.NewAcRTAlignJobs(meshPoints, JobSize));
        return PrefabInfoList;
    }

    public PrefabInfoList AcRTAlignJobsEx()
    {
        var meshFilters = GetMeshFilters();
        return AcRTAlignJobsEx(meshFilters);
    }

    public PrefabInfoList AcRTAlignJobsEx(GameObject target, bool isCopy)
    {
        SetTarget(target);
        IsCopyTargetRoot = isCopy;
        //if (isCopy == false && TargetRootsCopy != null)
        //{
        //    GameObject.DestroyImmediate(TargetRootsCopy);
        //}
        return AcRTAlignJobsEx();
    }

    public float[] TestMinSizeList = new float[] { 1.001f, 1.005f, 1.01f, 1.025f, 1.05f, 1.1f, 1.25f, 1.5f };

    public int MaxVertexCount = 50000;

    public MeshPoints[] FilterMeshPoints(MeshPoints[] meshPoints)
    {
        List<MeshPoints> list = new List<MeshPoints>();
        foreach(var mp in meshPoints)
        {
            //if (mp.name.Contains("_Combined_"))
            //{
            //    Debug.LogWarning($"FilterMeshPoints12 Contains(_Combined_) name:{mp.name} vertexCount:{mp.vertexCount} MaxVertexCount:{MaxVertexCount}");
            //    continue;
            //}
            ////if (mp.mf == null)
            ////{
            ////    Debug.LogWarning($"FilterMeshPoints11 mp.mf==null name:{mp.name} vertexCount:{mp.vertexCount} MaxVertexCount:{MaxVertexCount}");
            ////    continue;
            ////}
            //if (mp.mf != null)
            //{
            //    if (mp.mf.name.Contains("_Combined_"))
            //    {
            //        Debug.LogWarning($"FilterMeshPoints13 Contains(_Combined_) name:{mp.name} vertexCount:{mp.vertexCount} MaxVertexCount:{MaxVertexCount}");
            //        continue;
            //    }
            //    if (mp.mf.sharedMesh.name.Contains("_Combined_"))
            //    {
            //        Debug.LogWarning($"FilterMeshPoints14 Contains(_Combined_) name:{mp.name} vertexCount:{mp.vertexCount} MaxVertexCount:{MaxVertexCount}");
            //        continue;
            //    }
            //}
            if(mp.sharedMesh.name=="Cube")
            {
                continue;
            }
 
            if (mp.vertexCount> MaxVertexCount)
            {
                Debug.LogWarning($"FilterMeshPoints2 vertexCount> MaxVertexCount name:{mp.name} vertexCount:{mp.vertexCount} MaxVertexCount:{MaxVertexCount}");
                continue;
            }
            list.Add(mp);
        }
        return list.ToArray();
    }

    public MeshPoints[] FilterMeshPoints(GameObject root)
    {
        List<MeshPoints> meshFilters = MeshPoints.GetMeshPointsNoLOD(root);
        return FilterMeshPoints(meshFilters.ToArray());
    }

    public int vertexCountOffset = 10;

    public PrefabInfoList AcRTAlignJobsEx(MeshPoints[] meshPoints)
    {
        if(meshPoints==null|| meshPoints.Length == 0)
        {
            //Debug.LogError("AcRTAlignJobsEx meshPoints==null|| meshPoints.Length == 0");
            PrefabInfoList = new PrefabInfoList();
            return PrefabInfoList;
        }
        meshPoints = FilterMeshPoints(meshPoints);

        //Debug.Log($"AcRTAlignJobsEx Start {meshPoints.Length}");
        SetAcRTAlignJobSetting();
        SetDistanceSettings();
        UnpackPrefab();
        CleanNodes();

        SetPrefabInfoList(MeshJobHelper.NewAcRTAlignJobsEx(meshPoints, JobSize, vertexCountOffset));

        PrefabInfoList.RemoveNullGos();
        return PrefabInfoList;
    }


    public void SortPrefabInfoList()
    {
        SetPrefabInfoList(PrefabInfoList);
    }


    public void RemoveInstances1()
    {
        PrefabInfoListBags.RemoveInstances1();
    }

    public void RemoveInstances2()
    {
        PrefabInfoListBags.RemoveInstances2();
    }

    public void HideInstances1()
    {
        PrefabInfoListBags.HideInstances1();
    }

    public void HideInstances2()
    {
        PrefabInfoListBags.HideInstances2();
    }

    public void ShowInstances1()
    {
        PrefabInfoListBags.ShowInstances1();
    }

    public void ShowInstances2()
    {
        PrefabInfoListBags.ShowInstances2();
    }

    public void ClearPrefabs16()
    {
        PrefabInfoListBags.ClearPrefabs16();
    }

    public void ClearPrefabs()
    {
        PrefabInfoListBags.ClearPrefabs();
    }


    public bool SetPrefabInfoList(PrefabInfoList list){

        //PrefabInfoList.Clear();
        PrefabInfoList = list;
        if (list == null)
        {
            return false;
        }
        PrefabInfoListBags.SetPrefabInfoList(list, InsCountList);
        return true;
    }

    // public bool IsTryAngles=true;

    // public bool IsTryAngles_Scale=true;

    // public bool IsCheckResult=false;

    // public bool IsSetParent=false;

    // public int MaxVertexCount=2400;

    //public AcRTAlignJobSetting JobSetting;

    private void SetAcRTAlignJobSetting()
    {
        //  AcRTAlignJob.IsTryAngles=this.IsTryAngles;
        //  AcRTAlignJob.IsTryAngles_Scale=this.IsTryAngles_Scale;
        //  AcRTAlignJobContainer.IsCheckResult=this.IsCheckResult;

        //  AcRTAlignJobContainer.IsSetParent=this.IsSetParent;

        //  AcRTAlignJobContainer.MaxVertexCount=this.MaxVertexCount;
        AcRTAlignJobSetting.Instance.SetAcRTAlignJobSetting();
    }

    public List<GameObject> TestList=new List<GameObject>();

    public void AcRTAlignJobsExTestList()
    {
        SetAcRTAlignJobSetting();
        IsCopyTargetRoot=false;
        string log="";
        foreach(var item in TestList){
            GameObject testRoot=GameObject.Instantiate(item);
            Debug.LogWarning("AcRTAlignJobsExTestList:"+testRoot.name);
            SetTarget(testRoot);
            AcRTAlignJobsEx();
            ClearMeshFilters(null);
            log+=AcRTAlignJobContainer.ReportLog+"\n";
        }
        Debug.LogError(log);
    }

    public void AcRTAlignJobsEx2()
    {
        SetDistanceSettings();
        UnpackPrefab();
        CleanNodes();
        Debug.Log("AcRTAlignJobsEx2");
        var meshFilters = GetMeshFilters();
         PrefabInfoList.Clear();
        PrefabInfoList = MeshJobHelper.NewAcRTAlignJobsEx2(meshFilters, JobSize, vertexCountOffset);
    }

    public AcRigidTransform acRigidTransform;

    public void TestRTs()
    {
        if(acRigidTransform==null){
            acRigidTransform=AcRigidTransform.Instance;
        }
        if(acRigidTransform==null){
            acRigidTransform=gameObject.GetComponent<AcRigidTransform>();
        }
        if(acRigidTransform==null){
            acRigidTransform=gameObject.AddComponent<AcRigidTransform>();
        }
        acRigidTransform.CleanObjects();


        UnpackPrefab();
         Debug.Log("TestRTs");
        var meshFilters=GetMeshFilters();
        foreach(var mf in meshFilters)
        {
            MeshHelper.SetParentZero(mf.gameObject);
        }

        MeshJobHelper.NewThreePointJobs(meshFilters,JobSize);
        foreach(var mf in meshFilters){
            var result=ThreePointJobResultList.Instance.GetThreePointResult(mf);
            var ps=result.GetThreePoints(mf.transform.localToWorldMatrix);
      for (int i = 0; i < ps.Length; i++)
            {
                ThreePoint tp1 = ps[i];
                acRigidTransform.RTPoints.p01=tp1.GetMaxP();
                acRigidTransform.RTPoints.p02=tp1.GetMinP();
                acRigidTransform.RTPoints.p03=tp1.GetNormalP();
                acRigidTransform.RTPoints.p04=tp1.GetCenterP();
                acRigidTransform.RTPoints.p05=tp1.GetMaxP();

                acRigidTransform.RTPoints.p11=tp1.GetMaxP();
                acRigidTransform.RTPoints.p12=tp1.GetMinP();
                acRigidTransform.RTPoints.p13=tp1.GetNormalP();
                acRigidTransform.RTPoints.p14=tp1.GetCenterP();
                acRigidTransform.RTPoints.p15=tp1.GetMaxP();
                acRigidTransform.GetRTMatrix();


                Debug.LogError($"TestRTs\t[{i}] \tResult:{acRigidTransform.IsZero} \tIsRelection:{acRigidTransform.IsReflection}");

                if(acRigidTransform.IsZero==false)
                {
                    acRigidTransform.RTPoints.p01=tp1.GetMaxP();
                    acRigidTransform.RTPoints.p02=tp1.GetMinP();
                    acRigidTransform.RTPoints.p03=-tp1.GetNormalP();
                    acRigidTransform.RTPoints.p04=tp1.GetCenterP();
                    acRigidTransform.RTPoints.p05=tp1.GetMaxP();

                    acRigidTransform.RTPoints.p11=tp1.GetMaxP();
                    acRigidTransform.RTPoints.p12=tp1.GetMinP();
                    acRigidTransform.RTPoints.p13=-tp1.GetNormalP();
                    acRigidTransform.RTPoints.p14=tp1.GetCenterP();
                    acRigidTransform.RTPoints.p15=tp1.GetMaxP();
                    acRigidTransform.GetRTMatrix();
                    Debug.LogError($"TestRTsN\t[{i}] \tResult:{acRigidTransform.IsZero} \tIsRelection:{acRigidTransform.IsReflection}");
                    //break;
                }


                //break;
            }

break;
        }
    }

    //private BigSmallListInfo TestGetBigSmallRenderers()
    //{
    //    //List<MeshRenderer> bigModels=new List<MeshRenderer>();
    //    //List<MeshRenderer> smallModels=new List<MeshRenderer>();
    //    if(JobSetting==null){
    //        JobSetting=this.GetComponent<AcRTAlignJobSetting>();
    //    }
    //    var meshFilters=GetMeshFilters();
    //    return GetBigSmallRenderers(meshFilters,JobSetting.MaxModelLength);
    //}

    //public BigSmallListInfo GetBigSmallRenderers()
    //{
    //    var meshFilters=GetMeshFilters();
    //    return new BigSmallListInfo(meshFilters);
    //}

    //public static BigSmallListInfo GetBigSmallRenderers(MeshPoints[] meshFilters,float maxLength)
    //{
    //    BigSmallListInfo info = new BigSmallListInfo(meshFilters, maxLength);
    //    return info;
    //}
    // public float MaxModelLength=1.2f;

    public void GetMeshSizeInfo()
    {
        DateTime start=DateTime.Now;
        var meshFilters=GetMeshFilters();
        // float minCount=float.MaxValue;
        // float maxCount=0;
        // float sumCount=0;
        // float avgCount=0;
        List<string> sizeList = new List<string>();
        List<float> lengthList = new List<float>();
        List<MeshPoints> bigModels=new List<MeshPoints>();
        List<MeshPoints> smallModels=new List<MeshPoints>();
        for(int i=0;i<meshFilters.Length;i++)
        {
            var mf=meshFilters[i];

            float progress = (float)i / meshFilters.Length;
            float percents = progress * 100;
            if(ProgressBarHelper.DisplayCancelableProgressBar("GetMeshSizeInfo", $"{i}/{meshFilters.Length} {percents:F2}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            } 

            Bounds bounds = mf.sharedMesh.bounds;
            Vector3 size=bounds.size;
            string strSize=$"({size.x},{size.y},{size.z})";
            float length=size.x;
            if(size.y>length){
                length=size.y;
            }
            if(size.z>length){
                length=size.z;
            }

            // if(!sizeList.Contains(size))
            // {
            //     sizeList.Add(size);
            // }

            if(!lengthList.Contains(length))
            {
                lengthList.Add(length);
            }

            if(!sizeList.Contains(strSize))
            {
                sizeList.Add(strSize);
            }

            // var count=mf.sharedMesh.vertexCount;
            // sumCount+=count;
            // if(count>maxCount){
            //     maxCount=count;
            // }
            // if(count<minCount){
            //     minCount=count;
            // }

            if(length< AcRTAlignJobSetting.Instance.MaxModelLength)
            {
                smallModels.Add(mf);
            }
            else{
                bigModels.Add(mf);
            }
        }



        sizeList.Sort();
        sizeList.Reverse();

        string sizeStr = "";
        for(int i=0;i<500&&i<sizeList.Count;i++)
        {
            sizeStr += sizeList[i] + "; ";
        }

        lengthList.Sort();
        lengthList.Reverse();
        string lengthStr="";
        for(int i=0;i<500&&i<lengthList.Count;i++)
        {
            lengthStr += lengthList[i] + "; ";
        }

        ProgressBarHelper.ClearProgressBar();

        Debug.LogWarning($"GetMeshSizeInfo bigModels:{bigModels.Count},smallModels:{smallModels.Count},Renderers:{meshFilters.Length},Time:{(DateTime.Now-start).TotalMilliseconds}ms\n lengthStr: {lengthStr} \n sizeStr :{sizeStr}");
    }

    // private void GetVertexCountInfo()
    // {
    //     ShowRenderers();
    //     DateTime start=DateTime.Now;
    //     var meshFilters=GetMeshFilters();
    //     float minCount=float.MaxValue;
    //     float maxCount=0;
    //     float sumCount=0;
    //     float avgCount=0;
    //     List<int> countList = new List<int>();
    //     foreach(MeshFilter mf in meshFilters)
    //     {
    //         var count=mf.sharedMesh.vertexCount;
    //         sumCount+=count;
    //         if(count>maxCount){
    //             maxCount=count;
    //         }
    //         if(count<minCount){
    //             minCount=count;
    //         }
    //         if(!countList.Contains(count))
    //             countList.Add(count);
    //     }
    //     countList.Sort();
    //     countList.Reverse();
    //     string countStr = "";
    //     for(int i=0;i<500&&i<countList.Count;i++)
    //     {
    //         countStr += countList[i] + "; ";
    //     }
    //     avgCount =sumCount/(float)meshFilters.Length;

    //     Debug.LogWarning($"GetVertexCountInfo maxCount:{maxCount},minCount:{minCount},avgCount:{avgCount},Renderers:{meshFilters.Length},Time:{(DateTime.Now-start).TotalMilliseconds}ms\ncountStr:{countStr}");
    // }


    public void GetVertexCenterInfos()
    {
        UnpackPrefab();

        Debug.Log("GetVertexCenterInfos");
        DateTime start=DateTime.Now;
        
        CleanNodes();

        TargetList.Clear();
        TargetNodeList.Clear();
        var meshFilters=GetMeshFilters();
        
        TargetCount=meshFilters.Length;
        int max=MaxTargetCount;
        if(max==0){
            max=int.MaxValue;
        }

        //parentDict.Clear();
        for (int i = 0; i < TargetCount && i<max; i++)
        {
            
            float progress = (float)i / TargetCount;
            float percents = progress * 100;
            
            if(ProgressBarHelper.DisplayCancelableProgressBar("ReplaceToPrefab", $"{i}/{TargetCount} {percents:F2}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
            var mf = meshFilters[i];
            if(mf==null)
            {
                continue;
            }
            TargetList.Add(mf.gameObject);

            if(SetParentNull)
            {
                //mf.gameObject.transform.SetParent(null);//需要设置，不然会有误差
                MeshHelper.SetParentZero(mf.gameObject);//放到一个原点空物体底下也可以

                //parentDict.Add(mf.transform, mf.transform.parent);
            }

            MeshNode node1=mf.gameObject.GetComponent<MeshNode>();
            if(node1==null){
                node1=mf.gameObject.AddComponent<MeshNode>();
            }
            node1.GetVertexCenterInfo(false,true,centerOffset);

            TargetNodeList.Add(node1);

            //ReplaceToPrefab(newGo,mf.gameObject);
        }

        ProgressBarHelper.ClearProgressBar();

        Debug.LogWarning($"GetVertexCenterInfos Count:{TargetCount},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
    }

    void Progress(string strTitle, string strMessage, float fT)
    {
        int nPercent = Mathf.RoundToInt(fT * 100.0f);
#if UNITY_EDITOR
        if (EditorUtility.DisplayCancelableProgressBar(strTitle, strMessage, fT))
        {
            UltimateGameTools.MeshSimplifier.Simplifier.Cancelled = true;
        }
#endif
    }

    public Material[] LODMaterials ;

    public List<MeshRenderer> GetCombinedRenderers()
    {
        var list2=GetHiddenRenderers();

        List<MeshRenderer> list = new List<MeshRenderer>();
        // list.AddRange(PrefabInfoList1.GetRenderers());
        // list.AddRange(PrefabInfoList2.GetRenderers());
        // list.AddRange(PrefabInfoList3.GetRenderers());
        // list.AddRange(PrefabInfoList4.GetRenderers());

        var renderers=GetMeshRenderers();
        Debug.LogError("renderers "+renderers.Length);
        list.AddRange(renderers);
        
        List<MeshRenderer> list3 = new List<MeshRenderer>();
        foreach(var r in list)
        {
            if(list2.Contains(r))
            {

            }
            else{
                list3.Add(r);
            }
        }

        Debug.LogError("GetCombinedRenderers "+list3.Count);
        return list3;
    }

    public List<MeshRenderer> GetHiddenRenderers()
    {
        return PrefabInfoListBags.GetHiddenRenderers();
    }

    private void CreateInstancesInner(bool userLOD,bool userGPUPrefab,int maxInstanceCount,float[] lvs,float[] lodVertexPercents)
    {
        GPUInstancerPrefabManager prefabManager = GameObject.FindObjectOfType<GPUInstancerPrefabManager>();
        if (prefabManager == null)
        {
            prefabManager = this.gameObject.AddComponent<GPUInstancerPrefabManager>();
        }

        //prefabManager.InitPrefabs(PrefabList);
        if(prefabManager)
            prefabManager.ClearPrefabList();

        Debug.LogError($"CreateInstancesInner userLOD:{userLOD},userGPUPrefab:{userGPUPrefab}");

        if (maxInstanceCount == 0)
        {
            maxInstanceCount = int.MaxValue;
        }
#if UNITY_EDITOR
        DateTime start = DateTime.Now;
        int totalCount = 0;

        //PrefabInfoList lodPrefabInfoList
        PrefabInfoList list = new PrefabInfoList();
        //list.AddRange(PrefabInfoListBags.PrefabInfoList3);

        //list.AddRange(PrefabInfoListBags.PrefabInfoList4);
        list.AddRange(PrefabInfoListBags.PrefabInfoList5);
        list.AddRange(PrefabInfoListBags.PrefabInfoList6);

        for (int i = 0; i < list.Count; i++)
        {
            var info = list[i];
            totalCount += info.InstanceCount;
        }

        int currentCount = 0;
        bool isBreak = false;
        for (int i = 0; i < list.Count; i++)
        {
            var info = list[i];

           
            GameObject go = info.Prefab;
            string path = "Assets/Models/Instances/Prefabs/" + go.name + go.GetInstanceID() + ".prefab";
            string path2 = Application.dataPath + "/Models/Instances/Prefabs/" + go.name + go.GetInstanceID() + ".prefab";
            //path2 = path2.Replace("/", "\\");
            bool fileExists = File.Exists(path2);
            if (fileExists)
            {
                currentCount += info.InstanceCount;
                GameObject obj2 = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (userGPUPrefab && prefabManager)
                {
                    GPUInstancerPrefab gpuPrefab = go.GetComponent<GPUInstancerPrefab>();
                    if (gpuPrefab == null)
                    {
                        gpuPrefab = go.AddComponent<GPUInstancerPrefab>();
                    }

                    prefabManager.AddPrefabObject(obj2);

                    //Debug.LogError("CreateInstancesInner.gpuPrefab:" + gpuPrefab.prefabPrototype);
                }
                continue;
            }
            if (userLOD)
            {
                //new float[] { 0.6f, 0.2f, 0.07f, 0.01f }

                if(info.VertexCount<=150)
                {
                    lvs=null;
                    lodVertexPercents=null;
                }
                else if(info.VertexCount<=300)
                {
                    lvs=new float[] { 0.4f, 0.02f };
                    lodVertexPercents=new float[] { 1f,0.7f };
                }
                else{
                    // lvs=new float[] { 0.6f, 0.2f, 0.01f };
                    // lodVertexPercents=new float[] { 1f,0.7f,0.2f };
                }

                // lvs=null;
                // lodVertexPercents=null;

                AutomaticLODHelper.CreateAutoLOD(go, LODMaterials, lvs,lodVertexPercents);
            }

            if (userGPUPrefab && prefabManager)
            {
                GPUInstancerPrefab gpuPrefab = go.AddComponent<GPUInstancerPrefab>();
            }

            GameObject prefabAsset = PrefabUtility.SaveAsPrefabAssetAndConnect(go, path, InteractionMode.UserAction);

            if (userGPUPrefab && prefabManager)
            {
                prefabManager.AddPrefabObject(prefabAsset);
            }


            //Debug.Log("prefab:" + prefab);
            //var prefab2 = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            //Debug.Log("prefab2:" + prefab2);

            //Debug.LogError($"insList:{insList.Count}");

            var insList = info.GetInstances();
            List<GameObject> tmp = new List<GameObject>();
            for (int j = 0; j < insList.Count; j++)
            {
                tmp.Add(insList[j]);
            }

            //Debug.Log("insList:" + insList.Count);
            for (int j = 0; j < tmp.Count&& j< maxInstanceCount; j++)
            {
                currentCount++;
                float progress = (float)currentCount / totalCount;
                float percents = progress * 100;

                if (ProgressBarHelper.DisplayCancelableProgressBar("CreatePrefabs", $"{currentCount}/{totalCount} {percents:F2}% of 100%", progress))
                {
                    isBreak = true;
                    //ProgressBarHelper.ClearProgressBar();
                    break;
                }

                //if (ProgressBarHelper.DisplayCancelableProgressBar("CreatePrefabs111", "12123123", progress))
                //{
                //    isBreak = true;
                //    //ProgressBarHelper.ClearProgressBar();
                //    break;
                //}
                var instance = tmp[j];
                if (instance == null) continue;
                var t1 = instance.transform;
                //Debug.Log("instance:" + instance.name);

                //GameObject prefabInstance1 = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

                GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefabAsset) as GameObject;
                prefabInstance.name = instance.name;
                var t2 = prefabInstance.transform;
                //Debug.Log("prefabInstance:" + prefabInstance);

                t2.SetParent(t1.parent);
                t2.localPosition = t1.localPosition;
                t2.localScale = t1.localScale;
                t2.localRotation = t1.localRotation;

                GameObject.DestroyImmediate(instance);
                info.AddInstance(prefabInstance);
            }
            if (isBreak)
            {
                break;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.LogWarning($"CreateInstancesInner Count:{totalCount},Time:{(DateTime.Now - start).ToString()}ms");
#endif
    }

    private void CreatePrefabInner(bool userLOD, int maxInstanceCount,float[] lvs,float[] lodVertexPercents)
    {
        if (maxInstanceCount == 0)
        {
            maxInstanceCount = int.MaxValue;
        }
#if UNITY_EDITOR
        DateTime start = DateTime.Now;
        for (int i = 0; i < PrefabInfoList.Count; i++)
        {
            var info = PrefabInfoList[i];
            var insList = info.GetInstances();

            GameObject go = info.Prefab;
            string path = "Assets/Models/Instances/Prefabs/" + go.name+go.GetInstanceID() + ".prefab";

           
            if (userLOD)
                AutomaticLODHelper.CreateAutoLOD(go, LODMaterials, lvs,lodVertexPercents);

            GameObject prefabAsset = PrefabUtility.SaveAsPrefabAssetAndConnect(go, path, InteractionMode.UserAction);

            info.PrefAsset = prefabAsset;

            float progress = (float)i / PrefabInfoList.Count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("CreatePrefabs", $"{i}/{PrefabInfoList.Count} {percents:F2}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.LogWarning($"CreatePrefabInner Count:{PrefabInfoList.Count},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
#endif
    }

    public float[] LODLevels = new float[] { 0.6f, 0.2f, 0.07f, 0.01f };

    public void CreatePrefabs()
    {
        CreatePrefabInner(true, 0,LODLevels,null);
    }

    public void CreateInstances()
    {
        CreateInstancesInner(false,true,0,LODLevels,null);
    }

    public void CreateInstances_LOD()
    {
        CreateInstancesInner(true,true,0,LODLevels,null);
    }

    // [ContextMenu("CreateInstances(LOD5)")]
    // private void CreateInstances_LOD5()
    // {
    //     CreateInstancesInner(true,true,0,new float[] { 0.7f, 0.4f, 0.2f, 0.07f, 0.01f },null);
    // }

    // [ContextMenu("CreateInstances(LOD4)")]
    // private void CreateInstances_LOD4()
    // {
    //     CreateInstancesInner(true,true,0,new float[] { 0.6f, 0.2f, 0.07f, 0.01f },new float[] { 1f,0.6f,0.3f,0.1f });
    // }

    // [ContextMenu("CreateInstances(LOD3)")]
    // private void CreateInstances_LOD3()
    // {
    //     CreateInstancesInner(true,true,0,new float[] { 0.6f, 0.2f, 0.01f },new float[] { 1f,0.7f,0.2f });
    // }

    public void ShowPrefabInfo()
    {
        PrefabInfoList.Sort((a,b)=>{
            return a.VertexCount.CompareTo(b.VertexCount);
        });
        foreach(var info in PrefabInfoList){
            Debug.LogError("VertexCount:"+info.VertexCount);
        }
    }

    public void OneKey_Align_Remove_Instance()
    {
        DateTime start = DateTime.Now;
        AcRTAlignJobsEx();
        //RemoveInstances();
        CreateInstances();
        Debug.LogWarning($"OneKey_Align_Remove_Instance Time:{(DateTime.Now - start).ToString()}ms");
    }
    public void OneKey_Align_Remove_Instance_LOD()
    {
        DateTime start = DateTime.Now;
        AcRTAlignJobsEx();
        //RemoveInstances();
        CreateInstances_LOD();
        Debug.LogWarning($"OneKey_Align_Remove_Instance_LOD Time:{(DateTime.Now - start).ToString()}ms");
    }

    public PrefabInfoList GetPrefabInfos<T>(List<T> list, bool align) where T: class,IPrefab<T>
    {
        List<MeshPoints> meshFilters = new List<MeshPoints>();
        foreach(var item in list)
        {
            foreach(var mf in item.GetMeshFilters())
            {
                if (mf == null)
                {
                    Debug.LogError($"GetPrefabInfos mf == null item:{item} mf:{mf}");
                    continue;
                }
                if (mf.gameObject == null)
                {
                    Debug.LogError($"GetPrefabInfos mf.gameObject == null item:{item} mf:{mf}");
                    continue;
                }
                meshFilters.Add(new MeshPoints(mf.gameObject));
            }
            //meshFilters.AddRange();
        }
        //var prefabList=PrefabInfoListHelper.GetPrefabInfos(list, align);
        //SetPrefabInfoList(prefabList);
        //return prefabList;
        return AcRTAlignJobsEx(meshFilters.ToArray());
    }

    public PrefabInfoList GetPrefabsOfList<T>(List<T> list, bool align, string tag,bool enableRTC=false) where T : Component
    {
        bool isTryRt = AcRTAlignJobSetting.Instance.IsTryRT;
        AcRTAlignJobSetting.Instance.IsTryRT = enableRTC;

        LogTag.logTag = tag;
        List<MeshPoints> meshFilters = MeshPoints.GetMeshPoints(list);
        var preList= AcRTAlignJobsEx(meshFilters.ToArray());
        LogTag.logTag = "";

        AcRTAlignJobSetting.Instance.IsTryRT = isTryRt;
        return preList;
    }

    public PrefabInfoList GetPrefabsOfList(GameObject root)
    {
        List<MeshPoints> meshFilters = MeshPoints.GetMeshPointsNoLOD(root);
        return AcRTAlignJobsEx(meshFilters.ToArray());
    }

    public PrefabInfoList GetPrefabsOfList(Transform root)
    {
        List<MeshPoints> meshFilters = MeshPoints.GetMeshPointsNoLOD(root.gameObject);
        return AcRTAlignJobsEx(meshFilters.ToArray());
    }
}
