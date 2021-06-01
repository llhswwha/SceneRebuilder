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

public class PrefabInstanceBuilder : MonoBehaviour
{
    public DebugStep step;

    public RotateMode mode;

    public GameObject TargetRoots;

    public int TargetCount=0;

    public int MaxTargetCount=0;

    public float MaxAngle = 10;

    public double maxDistance = 5;
    public double zero = 0.0004f;
    public int zeroMax = 100;

    public int ICPMaxCount = 20;

    public float ICPMinDis = 0.2f;

    public List<GameObject> TargetList=new List<GameObject>();

    public List<MeshNode> TargetNodeList=new List<MeshNode>();

    public List<GameObject> PrefabList=new List<GameObject>();

    public List<PrefabInfo> PrefabInfoList=new List<PrefabInfo>();

    public List<PrefabInfo> PrefabInfoList1 =new List<PrefabInfo>();
    public List<PrefabInfo> PrefabInfoList2 =new List<PrefabInfo>();
    public List<PrefabInfo> PrefabInfoList3 =new List<PrefabInfo>();
     public List<PrefabInfo> PrefabInfoList4 =new List<PrefabInfo>();
    public List<PrefabInfo> PrefabInfoList5 = new List<PrefabInfo>();

    public List<PrefabInfo> PrefabInfoList6 = new List<PrefabInfo>();

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

    [ContextMenu("DestroySimgleOnes")]
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

    [ContextMenu("ResetPrefabs")]
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

    [ContextMenu("DestoryInstances")]
    public void DestoryInstances()
    {
        
        foreach(var info in PrefabInfoList)
        {
            info.DestroyInstances();
        }
    }

    [ContextMenu("ShowPrefabCount")]
    public void ShowPrefabCount()
    {
        int count1=0;
        int count2=0;
        int count3=0;
        int count4=0;
        int count5=0;
        int count6_10=0;
        int count11_20=0;
        int count21_100=0;
        int count101_500=0;
        int count500B=0;
        foreach(var info in PrefabInfoList)
        {
            int count=info.InstanceCount+1;
            if(count==1){
                count1++;
            }
            else if(count==2){
                count2++;
            }
            else if(count==3){
                count3++;
            }
            else if(count==4){
                count4++;
            }
            else if(count==5){
                count5++;
            }
            else if(count<=10){
                count6_10++;
            }
            else if(count<=20){
                count11_20++;
            }
            else if(count<=100){
                count21_100++;
            }
            else if(count<=500){
                count101_500++;
            }
            else{
                count500B++;
            }
        }

        Debug.Log($"1={count1};2={count2};3={count3};4={count4};5={count5};6_10={count6_10};11_20={count11_20};21_100={count21_100};101_500={count101_500};>500={count500B};");
        Debug.Log($"1\t{count1}\n2\t{count2}\n3\t{count3}\n4\t{count4}\n5\t{count5}\n6_10\t{count6_10}\n11_20\t{count11_20}\n21_100\t{count21_100}\n101_500\t{count101_500}\n>500\t{count500B}\n");
    }

    [ContextMenu("ShowInstanceCount")]
    public void ShowInstanceCount()
    {
        int count1=0;
        int count2=0;
        int count3=0;
        int count4=0;
        int count5=0;
        int count6_10=0;
        int count11_20=0;
        int count21_100=0;
        int count101_500=0;
        int count500B=0;
        foreach(var info in PrefabInfoList)
        {
            int count=info.InstanceCount+1;
            if(count==1){
                count1+=count;
            }
            else if(count==2){
                count2+=count;
            }
            else if(count==3){
                count3+=count;
            }
            else if(count==4){
                count4+=count;
            }
            else if(count==5){
                count5+=count;
            }
            else if(count<=10){
                count6_10+=count;
            }
            else if(count<=20){
                count11_20+=count;
            }
            else if(count<=100){
                count21_100+=count;
            }
            else if(count<=500){
                count101_500+=count;
            }
            else{
                count500B+=count;
            }
        }

        Debug.Log($"1={count1};2={count2};3={count3};4={count4};5={count5};6_10={count6_10};11_20={count11_20};21_100={count21_100};101_500={count101_500};>500={count500B};");
        Debug.Log($"1\t{count1}\n2\t{count2}\n3\t{count3}\n4\t{count4}\n5\t{count5}\n6_10\t{count6_10}\n11_20\t{count11_20}\n21_100\t{count21_100}\n101_500\t{count101_500}\n>500\t{count500B}\n");
    }

    [ContextMenu("GetTargetCount")]
    public void GetTargetCount()
    {
        TargetCount=GetMeshFilters().Length;
    }

    
    [ContextMenu("CreatePrefab")]
    public void CreatePrefab()
    {
        #if UNITY_EDITOR
        if(TargetRoots==null&&transform.childCount==1){
            TargetRoots=transform.GetChild(0).gameObject;
        }
        string path="Assets/Models/Prefabs/"+TargetRoots.name+".prefab";
        PrefabUtility.SaveAsPrefabAssetAndConnect(TargetRoots,path,InteractionMode.UserAction);
        #endif
    }

    [ContextMenu("CreateTestModel")]
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
            
            if(ProgressBarHelper.DisplayCancelableProgressBar("ReplaceToPrefab", $"{i}/{TargetCount} {percents}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
            MeshFilter mf = meshFilters[i];
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

    private void UpackPrefab_One(GameObject go)
    {
#if UNITY_EDITOR
        GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
        if (root != null)
        {
            PrefabUtility.UnpackPrefabInstance(root, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        }
#endif
    }

    [ContextMenu("UnpackPrefab")]
    private void UnpackPrefab()
    {
        if(TargetRoots==null&&transform.childCount==1){
            TargetRoots=transform.GetChild(0).gameObject;
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

    [ContextMenu("ShowRenderers")]
    public void ShowRenderers()
    {
        DateTime start=DateTime.Now;

        TargetRoots.gameObject.SetActive(true);
        var ts=AreaTreeHelper.GetAllTransforms(TargetRoots.transform);
        foreach(var t in ts){
            t.gameObject.SetActive(true);
        }

        var renderers=GameObject.FindObjectsOfType<MeshRenderer>();
        //var renderers=Target.GetComponentsInChildren<MeshRenderer>();
        foreach(var render in renderers){
            render.enabled=true;
            render.gameObject.SetActive(true);
        }
        Debug.LogError($"ShowRenderers renderers:{renderers.Length},\t{(DateTime.Now-start).ToString()}");
    }

    public int maxLoopCount=0;

    [ContextMenu("CreateInstance")]
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
            
            if(ProgressBarHelper.DisplayCancelableProgressBar("ReplaceToPrefab_Core", $"{i}/{models.Count} {percents}% of 100%", progress))
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
                prefabInfo.Add(newGo);
            }
        }
        ProgressBarHelper.ClearProgressBar();

        if(isShowDetail) Debug.LogWarning($"ReplaceToPrefab_Core Count:{models.Count},Time:{(DateTime.Now-start).ToString()},NewCount:{newModels.Count}");
        return newModels;
    }

    public int CurrentTestId=0;

    [ContextMenu("TestOne1")]
    public void TestOne1()
    {
        GetVertexCenterInfos();

        CurrentPrefab=TargetList[0];//第一个作为预设
        List<GameObject> newModels=ReplaceToPrefab_Test(CurrentPrefab,TargetList,CurrentTestId);
        CurrentTestId++;
    }

    #if UNITY_EDITOR

    public PrefabComparer prefabComparer;
    [ContextMenu("TestOne2")]
    public void TestOne2()
    {
        GetVertexCenterInfos();
        prefabComparer = this.gameObject.GetComponent<PrefabComparer>();
        prefabComparer.Prefab = TargetList[0];
        prefabComparer.Target = TargetList[1];
        prefabComparer.ReplaceToPrefab();
    }
    #endif

    public MeshComparer meshComparer;

    [ContextMenu("TestOne3")]
    public void TestOne3()
    {
        GetVertexCenterInfos();
        meshComparer = this.gameObject.GetComponent<MeshComparer>();
        meshComparer.goFrom = MeshHelper.CopyGO(TargetList[0]);
        meshComparer.goTo = TargetList[1];
        meshComparer.AlignModels();
    }

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
        else{
            nodes=GameObject.FindObjectsOfType<MeshNode>();
        }
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
            if(list==null||!list.Contains(mf)){
                GameObject.DestroyImmediate(mf.gameObject);
            }
        }
    }

    private MeshFilter[] GetMeshFilters()
    {
        
        MeshFilter[] meshFilters=null;
        if(TargetRoots){
            //meshFilters=TargetRoots.GetComponentsInChildren<MeshFilter>();
            
            if(IsCopyTargetRoot)
            {
                TargetRoots.SetActive(false);
                GameObject copy=MeshHelper.CopyGO(TargetRoots);
                copy.SetActive(true);
                meshFilters=copy.GetComponentsInChildren<MeshFilter>();
                ClearMeshFilters(meshFilters.ToList());
            }
            else{
                meshFilters=TargetRoots.GetComponentsInChildren<MeshFilter>();
                ClearMeshFilters(meshFilters.ToList());
            }
            
            
        }
        else{
            meshFilters=GameObject.FindObjectsOfType<MeshFilter>();
        }
        return meshFilters;
        //List<MeshFilter> list=new List<MeshFilter>();
        // list.AddRange(meshFilters);
        // list.Sort((a,b)=>{
        //     return a.name.CompareTo(b.name);
        // });
        // return list;
    }

    public int JobSize=40000;

    public bool SetParentNull=true;

    [ContextMenu("GetThreePointJobs")]
    private void GetThreePointJobs()
    {
        UnpackPrefab();
         Debug.Log("GetThreePointJobs");
        var meshFilters=GetMeshFilters();
        MeshJobHelper.NewThreePointJobs(meshFilters,JobSize);
    }

    [ContextMenu("MeshAlignJobs")]
    private void MeshAlignJobs()
    {
        UnpackPrefab();
        CleanNodes();
        Debug.Log("MeshAlignJob");
        var meshFilters=GetMeshFilters();
        PrefabInfoList.Clear();
        PrefabInfoList = MeshJobHelper.NewMeshAlignJobs(meshFilters,JobSize);
    }

    [ContextMenu("RTAlignJobs")]
    private void RTAlignJobs()
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
        DistanceSetting.maxDistance = this.maxDistance;
        DistanceSetting.zero = this.zero;//0.0004f
        DistanceSetting.zeroMax = this.zeroMax;

        DistanceSetting.ICPMaxCount=this.ICPMaxCount;
        DistanceSetting.ICPMinDis=this.ICPMinDis;
    }

    [ContextMenu("AcRTAlignJobs")]
    private void AcRTAlignJobs()
    {
        
        SetDistanceSettings();
        UnpackPrefab();
        CleanNodes();
        Debug.Log("AcRTAlignJobs");
        var meshFilters=GetMeshFilters();
         PrefabInfoList.Clear();
        PrefabInfoList = MeshJobHelper.NewAcRTAlignJobs(meshFilters,JobSize);
    }

    [ContextMenu("AcRTAlignJobsEx")]
    private void AcRTAlignJobsEx()
    {
        SetAcRTAlignJobSetting();
        SetDistanceSettings();
        UnpackPrefab();
        CleanNodes();
        Debug.Log("AcRTAlignJobsEx");
        var meshFilters = GetMeshFilters();
        SetPrefabInfoList(MeshJobHelper.NewAcRTAlignJobsEx(meshFilters, JobSize));
    }

     [ContextMenu("SortPrefabInfoList")]

    public void SortPrefabInfoList()
    {
        SetPrefabInfoList(PrefabInfoList);
    }

    [ContextMenu("RemoveInstances")]

    public void RemoveInstances()
    {
        PrefabInfoList1.RemoveInstances();
        PrefabInfoList2.RemoveInstances();
        PrefabInfoList3.RemoveInstances();
        //PrefabInfoList4.RemoveInstances();
    }

    public void SetPrefabInfoList(List<PrefabInfo> list){
        //PrefabInfoList.Clear();
        PrefabInfoList = list;

        PrefabInfoList1.Clear();
        PrefabInfoList2.Clear();
        PrefabInfoList3.Clear();
        PrefabInfoList4.Clear();
        PrefabInfoList5.Clear();
        PrefabInfoList6.Clear();
        for (int i=0;i<list.Count;i++)
        {
            if(list[i].InstanceCount==0)
            {
                PrefabInfoList1.Add(list[i]);
            }
            else if(list[i].InstanceCount<5)
            {
                PrefabInfoList2.Add(list[i]);
            }
            else if (list[i].InstanceCount < 10)
            {
                PrefabInfoList3.Add(list[i]);
            }
            else if (list[i].InstanceCount < 50)
            {
                PrefabInfoList4.Add(list[i]);
            }
            else if (list[i].InstanceCount < 100)
            {
                PrefabInfoList5.Add(list[i]);
            }
            else
            {
                PrefabInfoList6.Add(list[i]);
            }
        }

        Debug.LogError($"SetPrefabInfoList all:{PrefabInfoList.Count}={PrefabInfoList.GetInstanceCount()}\t list1:{PrefabInfoList1.Count}={PrefabInfoList1.GetInstanceCount()}\t list2:{PrefabInfoList2.Count}={PrefabInfoList2.GetInstanceCount()}\t list3:{PrefabInfoList3.Count}={PrefabInfoList3.GetInstanceCount()}\t list4:{PrefabInfoList4.Count}={PrefabInfoList4.GetInstanceCount()}\t list5:{PrefabInfoList5.Count}={PrefabInfoList5.GetInstanceCount()} \tlist6:{PrefabInfoList6.Count}={PrefabInfoList6.GetInstanceCount()},");
    }

    public bool IsTryAngles=true;

    public bool IsTryAngles_Scale=true;

    public bool IsCheckResult=false;

    public bool IsSetParent=false;

    private void SetAcRTAlignJobSetting()
    {
         AcRTAlignJob.IsTryAngles=this.IsTryAngles;
         AcRTAlignJob.IsTryAngles_Scale=this.IsTryAngles_Scale;
         AcRTAlignJobContainer.IsCheckResult=this.IsCheckResult;

         AcRTAlignJobContainer.IsSetParent=this.IsSetParent;
    }

    public List<GameObject> TestList=new List<GameObject>();

    [ContextMenu("AcRTAlignJobsEx(TestList)")]
    private void AcRTAlignJobsExTestList()
    {
        SetAcRTAlignJobSetting();
        IsCopyTargetRoot=false;
        string log="";
        foreach(var item in TestList){
            GameObject testRoot=GameObject.Instantiate(item);
            Debug.LogWarning("AcRTAlignJobsExTestList:"+testRoot.name);
            TargetRoots=testRoot;
            AcRTAlignJobsEx();
            ClearMeshFilters(null);
            log+=AcRTAlignJobContainer.ReportLog+"\n";
        }
        Debug.LogError(log);
    }

    [ContextMenu("AcRTAlignJobsEx2")]
    private void AcRTAlignJobsEx2()
    {
        SetDistanceSettings();
        UnpackPrefab();
        CleanNodes();
        Debug.Log("AcRTAlignJobsEx2");
        var meshFilters = GetMeshFilters();
         PrefabInfoList.Clear();
        PrefabInfoList = MeshJobHelper.NewAcRTAlignJobsEx2(meshFilters, JobSize);
    }

    public AcRigidTransform acRigidTransform;

    [ContextMenu("TestRTs")]
    private void TestRTs()
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

    [ContextMenu("GetVertexCenterInfos")]
    private void GetVertexCenterInfos()
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
        for (int i = 0; i < TargetCount && i<max; i++)
        {
            
            float progress = (float)i / TargetCount;
            float percents = progress * 100;
            
            if(ProgressBarHelper.DisplayCancelableProgressBar("ReplaceToPrefab", $"{i}/{TargetCount} {percents}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
            MeshFilter mf = meshFilters[i];
            if(mf==null)
            {
                continue;
            }
            TargetList.Add(mf.gameObject);

            if(SetParentNull)
            {
                //mf.gameObject.transform.SetParent(null);//需要设置，不然会有误差
                MeshHelper.SetParentZero(mf.gameObject);//放到一个原点空物体底下也可以
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
        List<MeshRenderer> list = new List<MeshRenderer>();
        list.AddRange(PrefabInfoList1.GetRenderers());
        list.AddRange(PrefabInfoList2.GetRenderers());
        list.AddRange(PrefabInfoList3.GetRenderers());
        list.AddRange(PrefabInfoList4.GetRenderers());
        return list;
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

        //List<PrefabInfo> lodPrefabInfoList
        List<PrefabInfo> list = new List<PrefabInfo>();
        //list.AddRange(PrefabInfoList3);

        //list.AddRange(PrefabInfoList4);
        list.AddRange(PrefabInfoList5);
        list.AddRange(PrefabInfoList6);

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

                AutomaticLODHelper.CreateLOD(go, LODMaterials, lvs,lodVertexPercents);
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

                if (ProgressBarHelper.DisplayCancelableProgressBar("CreatePrefabs", $"{currentCount}/{totalCount} {percents}% of 100%", progress))
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
                info.Add(prefabInstance);
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
                AutomaticLODHelper.CreateLOD(go, LODMaterials, lvs,lodVertexPercents);

            GameObject prefabAsset = PrefabUtility.SaveAsPrefabAssetAndConnect(go, path, InteractionMode.UserAction);

            info.PrefAsset = prefabAsset;

            float progress = (float)i / PrefabInfoList.Count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("CreatePrefabs", $"{i}/{PrefabInfoList.Count} {percents}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.LogWarning($"CreatePrefabInner Count:{PrefabInfoList.Count},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
#endif
    }

    public float[] LODLevels=new float[] { 0.6f, 0.2f, 0.07f, 0.01f };

    [ContextMenu("CreatePrefabs")]
    private void CreatePrefabs()
    {
        CreatePrefabInner(true, 0,LODLevels,null);
    }

    [ContextMenu("CreateInstances")]
    private void CreateInstances()
    {
        CreateInstancesInner(false,true,0,LODLevels,null);
    }

    [ContextMenu("CreateInstances(LOD)")]
    private void CreateInstances_LOD()
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

    [ContextMenu("ShowPrefabInfo")]
    private void ShowPrefabInfo()
    {
        PrefabInfoList.Sort((a,b)=>{
            return a.VertexCount.CompareTo(b.VertexCount);
        });
        foreach(var info in PrefabInfoList){
            Debug.LogError("VertexCount:"+info.VertexCount);
        }
    }

    [ContextMenu("OneKey_Align_Remove_Instance")]
    public void OneKey_Align_Remove_Instance()
    {
        DateTime start = DateTime.Now;
        AcRTAlignJobsEx();
        RemoveInstances();
        CreateInstances();
        Debug.LogWarning($"OneKey_Align_Remove_Instance Time:{(DateTime.Now - start).ToString()}ms");
    }

    // [ContextMenu("CreateInstances(LOD_10)")]
    // private void CreateInstances_LOD_10()
    // {
    //     CreateInstancesInner(true, 10);
    // }
}
