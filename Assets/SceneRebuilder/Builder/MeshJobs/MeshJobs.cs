using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
using System;
using System.Linq;

namespace MeshJobs
{
    [Serializable]
    public class MeshPoints
    {
        public bool IsSameSize(MeshPoints other)
        {
            return false;
        }

        //public static List<MeshPoints> GetMeshPoints(MeshFilter[] meshFilters)
        //{
        //    List<MeshPoints> meshPoints = new List<MeshPoints>();
        //    if(meshFilters!=null)
        //        foreach (var mf in meshFilters)
        //        {
        //            meshPoints.Add(new MeshPoints(mf.gameObject));
        //        }
        //    return meshPoints;
        //}

        public static List<MeshPoints> GetMeshPoints(GameObject root)
        {
            var mfs = root.GetComponentsInChildren<MeshFilter>(true);
            return GetMeshPoints(mfs);
        }

        public static List<MeshPoints> GetMeshPointsNoLOD(GameObject root)
        {
            List<MeshPoints> list = new List<MeshPoints>();
            GetMeshPointsNoLOD(root.transform, list);
            return list;
        }

        private static void GetMeshPointsNoLOD(Transform root, List<MeshPoints> list)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                var lod = child.GetComponent<LODGroup>();
                if (lod != null)
                {
                    continue;
                }
                MeshFilter mf = child.GetComponent<MeshFilter>();
                if (mf != null)
                {
                    list.Add(new MeshPoints(mf));
                }

                GetMeshPointsNoLOD(child, list);
            }
        }

        public static List<MeshPoints> GetMeshPoints<T>(T[] meshFilters) where T :Component
        {
            List<MeshPoints> meshPoints = new List<MeshPoints>();
            if (meshFilters != null)
                foreach (var mf in meshFilters)
                {
                    MeshPoints mps = new MeshPoints(mf.gameObject);
                    if (mps.vertexCount > 0)
                    {
                        meshPoints.Add(mps);
                    }
                }
            return meshPoints;
        }

        public static List<MeshPoints> GetMeshPoints<T>(List<T> meshFilters) where T : Component
        {
            List<MeshPoints> meshPoints = new List<MeshPoints>();
            if (meshFilters != null)
                foreach (var mf in meshFilters)
                {
                    if (mf == null) continue;
                    MeshPoints mps = new MeshPoints(mf.gameObject);
                    if (mps.vertexCount > 0)
                    {
                        meshPoints.Add(mps);
                    }
                }
            return meshPoints;
        }

        private List<float> sizeList = null;

        public List<float> GetSizeList()
        {
            if (sizeList == null)
            {
                sizeList = new List<float>() { size.x, size.y, size.z };
                sizeList.Sort();
            }
            return sizeList;
        }

        public Vector3[] vertices;
        //public Vector3[] localVertices;
        private Vector3[] worldVertices;
        public int vertexCount = 0;
        public Vector3 size;

        public int InstanceId = 0;

        public GameObject gameObject;

        public string name;

        public Transform transform;
        public MeshFilter mf;

        public Mesh sharedMesh;

        public static Dictionary<int, string> dictId2Go = new Dictionary<int, string>();

        public MeshPoints(MeshFilter mf)
        {
            InitGo(mf.gameObject);
            InitMesh(mf);
        }

        public MeshPoints(GameObject root)
        {
            InitGoEx(root);
        }

        public void InitGoEx(GameObject root)
        {
            InitGo(root);
            InitMesh(mf);
        }

        public void InitGo(GameObject root)
        {
            if (root == null)
            {
                Debug.LogError("MeshPoints.ctor root==null");
            }
            this.name = root.name;
            this.transform = root.transform;
            this.gameObject = root;
        }

        public void InitMesh(MeshFilter mf)
        {
            if (mf != null && mf.sharedMesh != null)
            {
                sharedMesh = mf.sharedMesh;
                vertices = sharedMesh.vertices;
                //localVertices = mf.sharedMesh.vertices;
                //InstanceId = sharedMesh.GetInstanceID();
                InstanceId = this.gameObject.GetInstanceID();
                size = sharedMesh.bounds.size;
                //worldVertices = MeshHelper.GetWorldVertexes(mf);
            }
            else
            {
                vertices = MeshHelper.GetChildrenVertexes(this.gameObject);
                //worldVertices = MeshHelper.GetChildrenWorldVertexes(root);
                InstanceId = this.gameObject.GetInstanceID();
                size = MeshHelper.GetMinMax(vertices)[2];
            }

            if (!dictId2Go.ContainsKey(InstanceId))
                dictId2Go.Add(InstanceId, this.gameObject.name);

            vertexCount = vertices.Length;
        }

        public T GetComponent<T>() where T : Component
        {
            return gameObject.GetComponent<T>();
        }

        internal Vector3[] GetWorldVertexes()
        {
            if (mf != null)
            {
                worldVertices = MeshHelper.GetWorldVertexes(mf);
            }
            else
            {
                worldVertices = MeshHelper.GetChildrenWorldVertexes(gameObject);
            }
            return worldVertices;
        }

        public bool IsSameMesh(MeshPoints other)
        {
            return (this.sharedMesh != null && other.sharedMesh != null && this.sharedMesh == other.sharedMesh);
        }

        private static string GetMatId(MeshRenderer renderer)
        {
            Color color = Color.black;
            try
            {
                if (renderer.sharedMaterial != null)
                {
                    if (renderer.sharedMaterial.HasProperty("_Color"))
                        color = renderer.sharedMaterial.color;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"MeshFilterListDict render:{renderer},mat:{renderer.sharedMaterial},matName:{renderer.sharedMaterial.name},ex:{ex.ToString()}");
            }
            var matId = color.ToString();
            return matId;
        }

        public string GetMatId()
        {
            //MeshRenderer renderer = mf.GetComponent<MeshRenderer>();
            //return GetMatId(renderer);

            MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);
            string matId = "";
            List<string> ids = new List<string>();
            foreach(var renderer in renderers)
            {
                string id = GetMatId(renderer);
                ids.Add(id);
            }
            ids.Sort();
            ids.ForEach(i => matId += i+"|");
            return matId;
        }

        public override string ToString()
        {
            return $"{name}({vertexCount})";
        }
    }
    public static class MeshJobHelper
    {
        #region ThreePointJob
        public static JobList<ThreePointJob> CreateThreePointJobs(MeshPoints[] meshFilters, int size)
        {
            return TreePointJobHelper.CreateThreePointJobs(meshFilters, size);
        }

        public static MeshPoints[] NewThreePointJobs(GameObject[] objs, int size)
        {
            return TreePointJobHelper.NewThreePointJobs(objs, size);
        }

        public static void NewThreePointJobs(MeshPoints[] meshFilters, int size)
        {
            TreePointJobHelper.NewThreePointJobs(meshFilters, size);
        }
        #endregion

        public static GameObject CreateTempGo(ThreePointJobResult r, Transform t, string log, ThreePoint id)
        {
            GameObject tempCenter = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //tempCenter.transform.SetParent(node2.transform.parent);
            tempCenter.transform.up = r.GetLongShortNormal(t, id);
            tempCenter.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            tempCenter.name = t.name + "_TempCenter_" + log;

            tempCenter.transform.position = r.GetCenterP(t);
            //tempCenter.transform.position=CenterGo.transform.position;
            t.SetParent(tempCenter.transform);

            return tempCenter;
        }

        #region MeshAlignJob
        public static MeshDistanceJobResult NewMeshAlignJob(GameObject go1, GameObject go2, bool showLog)
        {
            //go1不动，go2匹配go1。
            DateTime start = DateTime.Now;

            var mf1 = new MeshPoints(go1);
            var mf2 = new MeshPoints(go2);

            //1.判断顶点数据是否相同
            ThreePointJobResult r1 = ThreePointJobResultList.Instance.GetThreePointResult(mf1);
            ThreePointJobResult r2 = ThreePointJobResultList.Instance.GetThreePointResult(mf2);
            if (r1.vertexCount != r2.vertexCount)
            {
                return new MeshDistanceJobResult();
            }

            //2.Job1:对齐模型
            MeshAlignJobContainer container = new MeshAlignJobContainer(mf1, mf2, showLog);
            container.InitAlignInfo();
            // container.CreateJob().Complete();//4ms
            container.DoAlignJob();

            //3.Job2:比较距离，找出最小的变换
            container.CreateMeshDistanceJobs();
            container.DoDistanceJobs();

            //4.清理多余的模型，获取结果
            var result = container.GetResult();
            double t = (DateTime.Now - start).TotalMilliseconds;
            MeshAlignJobContainer.TotalTime += t;
            if (showLog) Debug.LogWarning($"NewMeshAlignJob Count:{container.count},Time:{t}ms");
            return result;
        }

        public static PrefabInfoList NewMeshAlignJobs(MeshPoints[] meshFilters, int size)
        {
            MeshAlignJobContainer.InitTime();
            MeshHelper.TotalCopyTime = 0;
            MeshHelper.TotalCopyCount = 0;

            DateTime start = DateTime.Now;
            int targetCount = meshFilters.Length;

            //1.设置父
            SetParentZero(meshFilters);

            //2.获取基本对应点信息
            NewThreePointJobs(meshFilters, size);

            //3.创建预设，替换模型
            PrefabInfoList prefabInfoList = new PrefabInfoList();
            int progressCount = 0;
            bool isCancel = false;

            for (int j = 0; j < targetCount; j++)
            {
                //一次
                MeshPoints first = meshFilters[0];
                GameObject prefab = first.gameObject;
                progressCount++;//一个作为预设

                PrefabInfo prefabInfo = new PrefabInfo(first);
                prefabInfoList.Add(prefabInfo);

                List<MeshPoints> newTargets = new List<MeshPoints>();
                for (int i = 1; i < meshFilters.Length; i++)
                {
                    var item = meshFilters[i];
                    GameObject model = item.gameObject;
                    var result = NewMeshAlignJob(model, prefab, false);//核心
                    if (result.IsZero)
                    {
                        GameObject.DestroyImmediate(model);//匹配成功，删除老的
                        prefabInfo.AddInstance(result.Instance);
                        progressCount++;//一个作为实例
                    }
                    else
                    {
                        newTargets.Add(item);//新的目标
                    }

                    float progress1 = (float)progressCount / targetCount;
                    if (ProgressBarHelper.DisplayCancelableProgressBar("MeshJobHelper", $"NewMeshAlignJobs:{progressCount}/{targetCount} {progress1 * 100:F2}% of 100% ", progress1))
                    {
                        isCancel = true;//取消处理
                        break;
                    }
                }
                if (isCancel)
                {
                    break;
                }
                float progress = (float)progressCount / targetCount;
                if (ProgressBarHelper.DisplayCancelableProgressBar("MeshJobHelper", $"NewMeshAlignJobs:{progressCount}/{targetCount} {progress * 100:F2}% of 100%", progress))
                {
                    break;
                }
                meshFilters = newTargets.ToArray();
                if (newTargets.Count == 0)
                {
                    break;//结束了
                }
            }

            ProgressBarHelper.ClearProgressBar();

            Debug.Log($"NewMeshAlignJobs Count:{targetCount},Time:{(DateTime.Now - start).ToString()}");

            MeshAlignJobContainer.PrintTime();
            return prefabInfoList;
        }

        private static void SetParentZero(MeshPoints[] meshFilters)
        {
            int targetCount = meshFilters.Length;
            for (int j = 0; j < targetCount; j++)
            {
                MeshHelper.SetParentZero(meshFilters[j].gameObject);//要么设置为null，要么设置为zero，不然会影响到精度
            }
        }

        

        #endregion

        #region RTAlignJob //无用

        public static PrefabInfoList NewRTAlignJobs(MeshPoints[] meshFilters, int size)
        {
            MeshAlignJobContainer.InitTime();
            MeshHelper.TotalCopyTime = 0;
            MeshHelper.TotalCopyCount = 0;

            DateTime start = DateTime.Now;
            int targetCount = meshFilters.Length;

            //1.设置父
            SetParentZero(meshFilters);

            //2.获取基本对应点信息
            NewThreePointJobs(meshFilters, size);

            //3.创建预设，替换模型
            PrefabInfoList prefabInfoList = new PrefabInfoList();
            int progressCount = 0;
            bool isCancel = false;

            for (int j = 0; j < targetCount; j++)
            {
                //一次
                var first = meshFilters[0];
                GameObject prefab = first.gameObject;
                progressCount++;//一个作为预设

                PrefabInfo prefabInfo = new PrefabInfo(first);
                prefabInfoList.Add(prefabInfo);

                List<MeshPoints> newTargets = new List<MeshPoints>();
                for (int i = 1; i < meshFilters.Length; i++)
                {
                    var item = meshFilters[i];
                    GameObject model = item.gameObject;
                    var result = NewMeshAlignJob(model, prefab, false);//核心
                    if (result.IsZero)
                    {
                        GameObject.DestroyImmediate(model);//匹配成功，删除老的
                        prefabInfo.AddInstance(result.Instance);
                        progressCount++;//一个作为实例
                    }
                    else
                    {
                        newTargets.Add(item);//新的目标
                    }

                    float progress1 = (float)progressCount / targetCount;
                    if (ProgressBarHelper.DisplayCancelableProgressBar("MeshJobHelper", $"NewRTAlignJobs:{progressCount}/{targetCount} {progress1 * 100:F2}% of 100% ", progress1))
                    {
                        isCancel = true;//取消处理
                        break;
                    }
                }
                if (isCancel)
                {
                    break;
                }
                float progress = (float)progressCount / targetCount;
                if (ProgressBarHelper.DisplayCancelableProgressBar("MeshJobHelper", $"NewRTAlignJobs:{progressCount}/{targetCount} {progress * 100:F2}% of 100%", progress))
                {
                    break;
                }
                meshFilters = newTargets.ToArray();
                if (newTargets.Count == 0)
                {
                    break;//结束了
                }
            }

            ProgressBarHelper.ClearProgressBar();

            Debug.Log($"NewRTAlignJobs Count:{targetCount},Time:{(DateTime.Now - start).ToString()}");

            MeshAlignJobContainer.PrintTime();
            return prefabInfoList;
        }

        public static JobList<RTAlignJob> CreateRTAlignJobs(MeshPoints mf1, MeshPoints mf2, int size)
        {
            DateTime start = DateTime.Now;
            JobList<RTAlignJob> jobList = new JobList<RTAlignJob>(size);
            Transform t1 = mf1.transform;
            Transform t2 = mf2.transform;

            ThreePointJobResult r1 = ThreePointJobResultList.Instance.GetThreePointResult(mf1);
            r1.localToWorldMatrix = t1.localToWorldMatrix;

            ThreePointJobResult r2 = ThreePointJobResultList.Instance.GetThreePointResult(mf2);
            r2.localToWorldMatrix = t2.localToWorldMatrix;

            var rts1 = r1.GetRTTransforms(t1.localToWorldMatrix);
            var rts2 = r2.GetRTTransforms(t2.localToWorldMatrix);

            //Vector3[] vertices1 = mf1.sharedMesh.vertices;
            //Vector3[] vertices1World = MeshHelper.GetWorldVertexes(vertices1, t1);
            Vector3[] vertices1World = mf1.GetWorldVertexes();
            NativeArray<Vector3> verticesArray1 = new NativeArray<Vector3>(vertices1World, Allocator.TempJob);

            //Vector3[] vertices2 = mf2.sharedMesh.vertices;
            //Vector3[] vertices2World = MeshHelper.GetWorldVertexes(vertices1, t2);
            Vector3[] vertices2World = mf2.GetWorldVertexes();
            NativeArray <Vector3> verticesArray2 = new NativeArray<Vector3>(vertices2World, Allocator.TempJob);

            int rId = MeshDistanceJobHelper.InitResult();
            int jobId = 0;
            for (int i = 0; i < rts1.Count; i++)
            {
                for (int j = 0; j < rts2.Count; j++)
                {
                    jobId++;
                    RTAlignJob job = new RTAlignJob()
                    {
                        Id = jobId,
                        RId = rId,
                        rt = RTTransform.FormTo(rts1[i], rts2[j]),
                        // Matrix1=go1.transform.localToWorldMatrix,
                        vertices1 = verticesArray1,
                        vertices2 = verticesArray2
                    };
                    jobList.Add(job);
                }
            }
            Debug.Log($"CreateRTAlignJobs Count:{jobList.Length},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
            return jobList;
        }

        public static void NewRTAlignJobs(MeshPoints mf1, MeshPoints mf2, int size)
        {
            DateTime start = DateTime.Now;
            JobList<RTAlignJob> jobList = CreateRTAlignJobs(mf1, mf2, size);
            jobList.CompleteAllPage();
            Debug.Log($"NewRTAlignJobs Count:{jobList.Length},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
        }

        // public static MeshDistanceJobResult NewMeshAlignJob(GameObject go1,GameObject go2,bool showLog)
        // {
        //     DateTime start=DateTime.Now;

        //     // MeshAlignJobContainer container=CreateMeshAlignJob(go1,go2,showLog);
        //     // if(showLog)Debug.Log($"NewMeshAlignJob32 Count:{container.count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //     // return new MeshDistanceJobResult();

        //     //go1不动，go2匹配go1。
        //     ThreePointJobResult r1=ThreePointJobResultList.Instance.GetThreePoint(go1);
        //     ThreePointJobResult r2=ThreePointJobResultList.Instance.GetThreePoint(go2);
        //     if(r1.vertexCount!=r2.vertexCount){
        //         return new MeshDistanceJobResult();
        //     }

        //     // int count=ps1.Count*ps2.Count;

        //     List<ThreePoint> ps1 = r1.GetThreePoints(go1.transform.localToWorldMatrix);
        //     List<ThreePoint> ps2 = r2.GetThreePoints(go2.transform.localToWorldMatrix); 

        //     int count=ps1.Count*ps2.Count;
        //     if(showLog)Debug.Log($"NewMeshAlignJob01 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //     //1ms

        //     // MeshNode node1=MeshNode.CreateNew(go1);
        //     // node1.GetVertexCenterInfo(true,true,Vector3.zero);

        //     // MeshNode node2=MeshNode.CreateNew(go2);
        //     // node2.GetVertexCenterInfo(true,true,Vector3.zero);

        //     // var c2=node1.GetCenterP();
        //     // var c1=ps1[0].GetCenterP();
        //     // c1.PrintVector3("c1");
        //     // c2.PrintVector3("c2");

        //     // var n1=ps1[0].GetLongShortNormal();
        //     // var n2=node1.GetLongShortNormal();
        //     // n1.PrintVector3("n1");
        //     // n2.PrintVector3("n2");

        //     // return;

        //     if(showLog)Debug.Log($"count1:{ps1.Count},count2:{ps2.Count},countAll:{count},go1:{go1.GetInstanceID()},{r1}|go2:{go2.GetInstanceID()},r2:{r2},");

        //     MeshAlignJob alignJob=new MeshAlignJob();
        //     // job.node1=r1;
        //     // job.node2=r2;
        //     alignJob.Init(count);

        //     r1.localToWorldMatrix=go1.transform.localToWorldMatrix;

        //     Transform[] ts=new Transform[count];

        //     if(showLog)Debug.Log($"NewMeshAlignJob02 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //     //2ms

        //     int i=0;
        //     List<GameObject> copyList=new List<GameObject>();
        //     for(int l=0;l<ps1.Count;l++)
        //     {
        //         var id1=ps1[l];
        //         for(int k= 0; k<ps2.Count;k++)
        //         {
        //             var id2=ps2[k];
        //             alignJob.SetIds(i,id1,id2);

        //             // GameObject go1Copy=MeshHelper.CopyGO(go1);
        //             // GameObject go2Copy=MeshHelper.CopyGO(go2);
        //             // ts[i*2]=go1Copy.transform;
        //             // ts[i*2+1]=go2Copy.transform;

        //             GameObject go2Copy=MeshHelper.CopyGO(go2);
        //             string tag=$"|({id1.max},{id1.min})({id2.max},{id2.min})";
        //             go2Copy.name+=tag;

        //             copyList.Add(go2Copy);
        //             ts[i]=go2Copy.transform;

        //             // GameObject go2Temp=CreateTempGo(r2,go2Copy.transform,tag,id2);
        //             // ts[i]=go2Temp.transform;

        //             i++;
        //             //id2.ShowDebugDetail();
        //             //break;
        //         }
        //         //break;
        //     }
        //     TransformAccessArray tA=new TransformAccessArray(ts);

        //     if(showLog)Debug.Log($"NewMeshAlignJob11 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //     //5ms

        //     alignJob.Schedule(tA).Complete();//4ms
        //     if(showLog)Debug.Log($"NewMeshAlignJob12 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //     //9ms

        //     // JobHandle alignJobHandle=CreateMeshAlignJob(go1,go2,showLog);
        //     // var count=alignJobHandle.cou
        //     // alignJobHandle.Complete();//4ms
        //     // if(showLog)Debug.Log($"NewMeshAlignJob12 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");

        //     // //调试用，显示细节
        //     // foreach(var go in copyList){
        //     //     MeshNode node=MeshNode.CreateNew(go);
        //     //     node.GetVertexCenterInfo(true,true,Vector3.zero);
        //     // }

        //     //-------------------------比较距离，找出最小的
        //     MeshFilter meshFilter1=go1.GetComponent<MeshFilter>();
        //     Vector3[] vertices1=meshFilter1.sharedMesh.vertices;
        //     Vector3[] vertices1World=MeshHelper.GetWorldVertexes(vertices1,go1.transform);
        //     NativeArray<Vector3> verticesArray1=new NativeArray<Vector3>(vertices1World,Allocator.TempJob);

        //     MeshFilter meshFilter2=go2.GetComponent<MeshFilter>();
        //     Vector3[] vertices2=meshFilter2.sharedMesh.vertices;
        //     NativeArray<Vector3> verticesArray2=new NativeArray<Vector3>(vertices2,Allocator.TempJob);

        //     // MeshDistanceJob.minDis=float.MaxValue;
        //     // MeshDistanceJob.minDisId=0;

        //     int rId=MeshDistanceJobHelper.InitResult();

        //     JobList<MeshDistanceJob> jobList=new JobList<MeshDistanceJob>(10000);
        //     for (int i1 = 0; i1 < copyList.Count; i1++)
        //     {
        //         GameObject go = copyList[i1];
        //         MeshDistanceJob job1=new MeshDistanceJob{
        //             Id=i1,
        //             RId=rId,
        //             // Matrix1=go1.transform.localToWorldMatrix,
        //             Matrix2=go.transform.localToWorldMatrix,
        //             vertices1=verticesArray1,
        //             vertices2=verticesArray2
        //         };
        //         jobList.Add(job1);
        //     }

        //     if(showLog)Debug.Log($"NewMeshAlignJob21 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //     //13ms
        //     jobList.CompleteAllPage();//8ms
        //     var result=MeshDistanceJobHelper.results[rId];
        //     if(showLog)Debug.Log($"NewMeshAlignJob22 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms,Dis:{result.min},Id:{result.minId},Zero:{result.IsZero}");
        //     //21ms

        //     //清理多余的模型
        //     GameObject minGo=null;
        //     for (int i1 = 0; i1 < copyList.Count; i1++)
        //     {
        //         GameObject go = copyList[i1];
        //         if(i1!=result.minId)
        //         {
        //             GameObject.DestroyImmediate(go);
        //         }
        //         else
        //         {
        //             go.name=go1.name+"_New";
        //             minGo=go;
        //         }
        //     }
        //     if(result.IsZero){
        //         // GameObject.DestroyImmediate(go1);
        //         // GameObject.DestroyImmediate(go2);
        //         result.Instance=minGo;
        //     }
        //     else
        //     {
        //         if(result.min<0.02f)//判断是否相对很近
        //         {
        //             //if(long22<0.0001 && short22<0.0001 &&angle1<=0.09f&&angle2<=0.09f&&dis2<=0.02f)//几个条件，确保差异不大。这种情况下或许还有结合ICP算法
        //             ThreePoint node1=alignJob.ps1[result.minId];
        //             ThreePoint node2=alignJob.ps2[result.minId];

        //             var longLine1=node1.GetLongLine();
        //             var shortLine1=node1.GetShortLine();
        //             var angel1=Vector3.Angle(longLine1, shortLine1);

        //             var longLine2=node2.GetLongLine();
        //             var shortLine2=node2.GetShortLine();
        //             var angel2=Vector3.Angle(longLine2, shortLine2);

        //             var angle22 = Math.Abs(angel1 - angel2);
        //             var long22 = Math.Abs(longLine1.magnitude- longLine2.magnitude);
        //             var short22= Math.Abs(shortLine1.magnitude - shortLine2.magnitude);
        //             var rate22 = Math.Abs(longLine1.magnitude/shortLine1.magnitude - longLine2.magnitude/shortLine2.magnitude);

        //             var angle1 =Vector3.Angle(shortLine2, shortLine1);
        //             var angle2=Vector3.Angle(longLine2, longLine1);
        //             if(showLog)Debug.Log($"After ShortLine angle1:{angle1} | LongLine angle2:{angle2} | dis:{result.min}");
        //             if(showLog)Debug.Log($"node1-node2 angle:{angle22},long:{long22},short:{short22},rate:{rate22} |");
        //             if(showLog)Debug.LogError($"IsRelativeZero: {long22<0.0001}|{short22<0.0001}|{angle1<=0.09f}|{angle2<=0.09f}|{result.min<=0.02f}");

        //             //if(long22<0.0001 && short22<0.0001 &&angle1<=0.09f&&angle2<=0.09f)//几个条件，确保差异不大。这种情况下或许还有结合ICP算法
        //             if(long22<0.0001 && short22<0.0001)//长短轴相似，角度不管。就算角度不合适，重合就行
        //             {
        //                 result.IsZero=true;
        //             }
        //             else
        //             {
        //                 if(minGo!=null){
        //                     GameObject.DestroyImmediate(minGo);
        //                 }
        //             }
        //         }
        //         else{
        //             if(minGo!=null){
        //                 GameObject.DestroyImmediate(minGo);
        //             }
        //         }
        //     }

        //      if(showLog)Debug.Log($"NewMeshAlignJob32 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //      return result;
        // }

        #endregion
        #region AcRTAlign

        public static bool DoAcRTAlignJob(MeshPoints mfFrom, MeshPoints mfTo, int id)
        {
            AcRTAlignJob job = AcRTAlignJobHelper.NewJob(mfFrom, mfTo, id);
            job.Schedule().Complete();//DoJob
            var result = AcRTAlignJobResult.GetResult(id);
            if (result != null)
            {

                //可以改成这里创建模型，并变换，原来的模型不动。
                result.ApplyMatrix(mfFrom.transform, mfTo.transform); //变换模型

                if (result.Distance < DistanceSetting.zeroM)
                {
                    //Debug.LogWarning($"对齐成功 {mfFrom.name} -> {mfTo.name} 距离:{result.Distance}");

                    var disNew = MeshHelper.GetVertexDistanceEx(mfFrom.transform, mfTo.transform, "测试结果", false);

                    if(disNew< DistanceSetting.zeroM)
                    {
                        RTResult rT = result as RTResult;
                        if (rT != null)
                        {
                            Debug.Log($"对齐成功1 id:{id} zero:{DistanceSetting.zeroM:F5},dis:{result.Distance},disNew:{disNew},Mode:{rT.Mode},from:[{mfFrom.name}({mfFrom.vertexCount})],to:[{mfTo.name}({mfTo.vertexCount})] " + $" Trans:{rT.Translation.Vector3ToString()},Matrix:\n{rT.TransformationMatrix}");
                            //Debug.LogError($"Mode:{rT.Mode},Trans:{rT.Translation.Vector3ToString()},Matrix:\n{rT.TransformationMatrix}");
                        }
                        else
                        {
                            Debug.Log($"对齐成功2 id:{id}  zero:{DistanceSetting.zeroM:F5},dis:{result.Distance},disNew:{disNew},from:[{mfFrom.name}({mfFrom.vertexCount})],to:[{mfTo.name}({mfTo.vertexCount})] rT==null");
                        }
                    }
                    else
                    {
                        RTResult rT = result as RTResult;
                        if (rT != null)
                        {
                            Debug.LogError($"对齐失败3 id:{id} zero:{DistanceSetting.zeroM:F5},dis:{result.Distance},disNew:{disNew},Mode:{rT.Mode},from:[{mfFrom.name}({mfFrom.vertexCount})],to:[{mfTo.name}({mfTo.vertexCount})] " + $" Trans:{rT.Translation.Vector3ToString()},Matrix:\n{rT.TransformationMatrix}");
                            //Debug.LogError($"Mode:{rT.Mode},Trans:{rT.Translation.Vector3ToString()},Matrix:\n{rT.TransformationMatrix}");
                        }
                        else
                        {
                            Debug.LogError($"对齐失败4 id:{id}  zero:{DistanceSetting.zeroM:F5},dis:{result.Distance},disNew:{disNew},from:[{mfFrom.name}({mfFrom.vertexCount})],to:[{mfTo.name}({mfTo.vertexCount})] rT==null");
                        }
                    }
                    return true;
                }
                else
                {
                    Debug.LogError($"对齐失败2 {mfFrom.name}({mfFrom.vertexCount})({mfFrom.size}) -> {mfTo.name}({mfTo.vertexCount})({mfTo.size}) 距离:{result.Distance} zeroM:{DistanceSetting.zeroM:F5} zeroP:{DistanceSetting.zeroP:F5}");
                    return false;
                }

            }
            else
            {
                Debug.LogError($"对齐失败1(result==null) id:{id} {mfFrom.name}({mfFrom.vertexCount}) -> {mfTo.name}({mfTo.vertexCount}) result==null zeroM:{DistanceSetting.zeroM:F5} zeroP:{DistanceSetting.zeroP:F5}");
                return false;
            }
            //return result;
        }

        private static MeshFilterListDict CreateMeshFilterListDict(MeshPoints[] meshFilters, int vertexCountOffset)
        {
            DateTime start = DateTime.Now;
            var mfld = new MeshFilterListDict(meshFilters, vertexCountOffset);
            Debug.Log($"CreateMeshFilterListDict meshFilters:{meshFilters.Length},Time:{(DateTime.Now - start).TotalMilliseconds:F1}ms");
            return mfld;
        }

        public static PrefabInfoList NewAcRTAlignJobsEx_OLD(MeshPoints[] meshFilters, int size, int vertexCountOffset)
        {
            Debug.Log("NewAcRTAlignJobsEx:"+meshFilters.Length);

            AcRtAlignJobArg.CleanArgs();
            AcRTAlignJobResult.CleanResults();

            MeshAlignJobContainer.InitTime();
            MeshHelper.TotalCopyTime = 0;
            MeshHelper.TotalCopyCount = 0;

            DateTime start = DateTime.Now;
            int targetCount = meshFilters.Length;

            //1.设置父
            SetParentZero(meshFilters);

            //2.获取基本对应点信息
            NewThreePointJobs(meshFilters, size);


            //3.创建预设，替换模型
            PrefabInfoList prefabInfoList = new PrefabInfoList();

            int totalJobCount = 0;
            int loopCount = 0;
            string jobCountDetails = "";

            int mfCount = meshFilters.Length;
            var mfld = CreateMeshFilterListDict(meshFilters, vertexCountOffset);
            string loopTimes="";

            for (int kk = 0; kk < mfCount; kk++)
            {
                DateTime loopStart = DateTime.Now;

                loopCount++;

                int count1 = mfld.GetMeshFilterCount();
                string countDetail=mfld.GetGroupCountDetails();
                var mfsList=mfld.GetMeshFiltersList();
                Debug.Log($"MeshFilterListDict List:{mfsList.Count},MeshFiter:{count1}");
                Debug.Log($"countDetail:{countDetail}");

                AcRTAlignJobResult.CleanResults();
                AcRtAlignJobArg.CleanArgs();
                AcRTAlignJobPrefab.Clear();

                int jobId = 0;
                JobList<AcRTAlignJob> jobList = new JobList<AcRTAlignJob>(size);
                List<int> jobIds = new List<int>();
                string ids = "";
                 for (int i1 = 0; i1 < mfsList.Count; i1++)
                {
                    MeshFilterList item = mfsList[i1];
                    var mfList = item.GetList();

                    var mfFrom = mfList[0];
                    GameObject prefab = mfFrom.gameObject;
                    //progressCount++;//一个作为预设
                    PrefabInfo prefabInfo = new PrefabInfo(mfFrom);
                    prefabInfoList.Add(prefabInfo);

                    if(mfList.Count>1){
                        for (int i = 1; i < mfList.Count; i++)
                        {
                            var mfTo = mfList[i];
                            if (mfTo == null) continue;
                            //int jobId = AcRTAlignJobResult.GetId();
                            var job = AcRTAlignJobHelper.NewJob(mfFrom, mfTo, jobId);
                            jobList.Add(job);
                            jobIds.Add(jobId);
                            AcRTAlignJobPrefab.AddItem(jobId, prefabInfo, item);
                            ids += jobId + ";";

                            totalJobCount++;
                            jobId++;
                        }
                    }
                    else{
                        item.Remove(mfFrom);
                    }
                }
                jobCountDetails += jobList.Length + ";";
                AcRTAlignJobResult.InitCount(jobId);
                jobList.CompleteAllPage();//计算

                for (int k = 0; k < jobIds.Count; k++)
                {
                    int id = jobIds[k];
                    AcRtAlignJobArg arg = AcRtAlignJobArg.LoadArg(id);

                    var result = AcRTAlignJobResult.GetResult(id);
                    AcRTAlignJobPrefab pref = AcRTAlignJobPrefab.GetItem(id);
                    if (result != null)
                    {
                        //可以改成这里创建模型，并变换，原来的模型不动。

                        if (result.Distance < DistanceSetting.zeroM)
                        {
                            pref.RemoveMeshFilter(arg.mfFrom);
                            pref.RemoveMeshFilter(arg.mfTo);

                            //Debug.Log($"对齐成功 {arg.mfFrom.name} -> {arg.mfTo.name} 距离:{result.Distance}");
                            GameObject newGo = MeshHelper.CopyGO(pref.PrefabInfo.Prefab);
                            newGo.name = arg.mfTo.name + "_New";
                            pref.AddInstance(newGo);
                            
                            result.ApplyMatrix(newGo.transform, arg.mfTo.transform); //变换模型
                            arg.DestroyToObject();
                        }
                        else
                        {
                            pref.RemoveMeshFilter(arg.mfFrom);
                            //newTargets.Add(arg.mfTo);

                            //Debug.LogWarning($"对齐失败(距离太大) {arg.mfFrom.name} -> {arg.mfTo.name} 距离:{result.Distance}");
                        }
                    }
                    else
                    {
                        pref.RemoveMeshFilter(arg.mfFrom);
                        //newTargets.Add(arg.mfTo);
                        Debug.LogError($"对齐失败(无结果数据) {arg.mfFrom.name} -> {arg.mfTo.name}");
                    }
                }

                mfld.RemoveEmpty();//删除
                if (mfld.Count == 0)
                {
                    break;
                }

                int count2 = mfld.GetMeshFilterCount();
                int progressCount=targetCount-count2;
                float progress1 = (float)progressCount / targetCount;
                if (ProgressBarHelper.DisplayCancelableProgressBar("CompleteAllPage", $"NewMeshAlignJobs:{progressCount}/{targetCount} {progress1 * 100:F2}% of 100% ", progress1))
                {
                   //isCancel = true;//取消处理
                    break;
                }

                Debug.LogError($"完成一轮[{loopCount}]:{count1-count2}={count1}->{count2},PrefabCount:{prefabInfoList.Count}");
                int loopTime=(int)(DateTime.Now-loopStart).TotalMilliseconds;
                loopTimes+=loopTime+";";
            }
            

            ProgressBarHelper.ClearProgressBar();

            Debug.Log($"NewMeshAlignJobs Target:{targetCount},Prefab:{prefabInfoList.Count},Job:{totalJobCount}({jobCountDetails}),Loop:{loopCount},Time:{(DateTime.Now - start).TotalSeconds:F2}s({loopTimes})");
            Debug.Log($"{prefabInfoList.Count}\t{totalJobCount}\t{loopCount}\t{(DateTime.Now - start).TotalSeconds:F2}s");
            MeshAlignJobContainer.PrintTime();
            prefabInfoList.Sort();
            return prefabInfoList;
        }

        public static Dictionary<Transform, Transform> GetParentDict(MeshPoints[] meshFilters)
        {
            Dictionary<Transform, Transform> parentDict = new Dictionary<Transform, Transform>();
            foreach (var meshFilter in meshFilters)
            {
                parentDict.Add(meshFilter.transform, meshFilter.transform.parent);
            }
            return parentDict;
        }

        public static void RestoreParent(Dictionary<Transform, Transform> parentDict)
        {
            List<Transform> parents = new List<Transform>();

            Dictionary<Transform, List<Transform>> parentChildren = new Dictionary<Transform, List<Transform>>();

            foreach (var child in parentDict.Keys)
            {
                if (child == null) continue;
                var parent = parentDict[child];
                if(!parentChildren.ContainsKey(parent))
                {
                    parentChildren.Add(parent, new List<Transform>());
                }
                var children = parentChildren[parent];
                children.Add(child);
            }

            //foreach (var p in parentChildren.Keys)
            //{
            //    var center = Vector3.zero;
            //    var list = parentChildren[p];
            //    for (int i=0;i< list.Count; i++)
            //    {
            //        center += list[i].position;
            //    }
            //    center /= list.Count;

            //    p.position = center;

            //    //Debug.LogError("center:" + center);
            //}
            
            //Debug.LogError("RestoreParent parentDict:" + parentDict.Count);
            foreach (var child in parentDict.Keys)
            {
                if (child == null) continue;
                var parent = parentDict[child];
                child.SetParent(parent);

                if (!parents.Contains(parent))
                {
                    parents.Add(parent);
                }
            }
        }

        public static PrefabInfoList NewAcRTAlignJobsEx(MeshPoints[] meshFilters, int size, int vertexCountOffset)
        {
            float vc = 0;
            foreach(var mf in meshFilters)
            {
                vc += mf.vertexCount;
            }
            DateTime start = DateTime.Now;
            Debug.Log("NewAcRTAlignJobsEx:"+meshFilters.Length);

            var parentDict = GetParentDict(meshFilters);
            //1.设置父
            SetParentZero(meshFilters);

            //2.获取基本对应点信息
            NewThreePointJobs(meshFilters, size);
            
            AcRTAlignJobContainer jobContainer=new AcRTAlignJobContainer(meshFilters, size, vertexCountOffset);
            jobContainer.parentDict = parentDict;
            PrefabInfoList preafbs =jobContainer.GetPrefabs();

            RestoreParent(parentDict);

            float vcAfter = preafbs.GetVertexCount();
            Debug.LogWarning($"NewAcRTAlignJobsEx meshFilters:{meshFilters.Length},vertexCount:({MeshHelper.GetVertexCountS(vcAfter)}/{MeshHelper.GetVertexCountS(vc)}={vcAfter/vc:P2}) Time:{(DateTime.Now - start)}s");

            if (jobContainer.IsBreak)
            {
                Debug.LogError("Job Break!!");
                return null;
            }
            return preafbs;
        }

        public static PrefabInfoList NewAcRTAlignJobsEx2(MeshPoints[] meshFilters, int size, int vertexCountOffset)
        {
            DateTime start = DateTime.Now;
            Debug.Log("NewAcRTAlignJobsEx:" + meshFilters.Length);

            //1.设置父
            SetParentZero(meshFilters);

            //2.获取基本对应点信息
            NewThreePointJobs(meshFilters, size);

            AcRTAlignJobContainer jobContainer = new AcRTAlignJobContainer(meshFilters, size, vertexCountOffset);
            var preafbs = jobContainer.GetPrefabsEx();

            Debug.Log($"NewAcRTAlignJobsEx meshFilters:{meshFilters.Length},Time:{(DateTime.Now - start).TotalMilliseconds:F1}ms");
            return preafbs;
        }


        public static PrefabInfoList NewAcRTAlignJobs(MeshPoints[] meshFilters, int size)
        {
            AcRtAlignJobArg.CleanArgs();
            AcRTAlignJobResult.CleanResults();

            MeshAlignJobContainer.InitTime();
            MeshHelper.TotalCopyTime = 0;
            MeshHelper.TotalCopyCount = 0;

            DateTime start = DateTime.Now;
            int targetCount = meshFilters.Length;

            var parentDict = GetParentDict(meshFilters);

            //1.设置父
            SetParentZero(meshFilters);

            //2.获取基本对应点信息
            NewThreePointJobs(meshFilters, size);

            //3.创建预设，替换模型
            PrefabInfoList prefabInfoList = new PrefabInfoList();

            //JobList<AcRTAlignJob> totalJobList = new JobList<AcRTAlignJob>(size);
            int totalJobCount = 0;
            int loopCount = 0;
            string jobCountDetails = "";

            List<AcRtAlignJobArg> allArg = new List<AcRtAlignJobArg>();

            for (int j = 0; j < targetCount; j++)
            {
                loopCount++;
                JobList<AcRTAlignJob> jobList = new JobList<AcRTAlignJob>(size);
                int jobId = 0;
                List<int> jobIds = new List<int>();
                string ids = "";

                AcRTAlignJobResult.CleanResults();
                AcRtAlignJobArg.CleanArgs();

                //一次
                var mfFrom = meshFilters[0];
                GameObject prefab = mfFrom.gameObject;

                PrefabInfo prefabInfo = new PrefabInfo(mfFrom);
                prefabInfoList.Add(prefabInfo);

                List<MeshPoints> newTargets = new List<MeshPoints>();
                for (int i = 1; i < meshFilters.Length; i++)
                {
                    var mfTo = meshFilters[i];
                    //int jobId = AcRTAlignJobResult.GetId();
                    var job = AcRTAlignJobHelper.NewJob(mfFrom, mfTo, jobId);
                    jobList.Add(job);
                    jobIds.Add(jobId);
                    ids += jobId + ";";

                    totalJobCount++;
                    jobId++;
                }
                jobCountDetails += jobList.Length + ";";

                //Debug.LogError("ids:" + ids);
                AcRTAlignJobResult.InitCount(jobId);
                jobList.CompleteAllPage();//计算

                
                for (int k = 0; k < jobIds.Count; k++)
                {
                    int id = jobIds[k];
                    AcRtAlignJobArg arg = AcRtAlignJobArg.LoadArg(id);
                    allArg.Add(arg);
                    var result = AcRTAlignJobResult.GetResult(id);
                    if (result!=null)
                    {
                        //可以改成这里创建模型，并变换，原来的模型不动。
                        
                        if (result.Distance < DistanceSetting.zeroM)
                        {
                            //Debug.LogError($"对齐成功 {arg.mfFrom.name} -> {arg.mfTo.name} 距离:{result.Distance}");
                            GameObject newGo = MeshHelper.CopyGO(prefab);

                            if (parentDict.ContainsKey(arg.mfTo.transform))
                            {
                                Transform oldTransformParent = parentDict[arg.mfTo.transform];
                                parentDict.Add(newGo.transform, oldTransformParent);
                            }

                            newGo.name = arg.mfTo.name + "_New";
                            prefabInfo.AddInstance(newGo);
                            
                            result.ApplyMatrix(newGo.transform, arg.mfTo.transform); //变换模型

                            arg.DestroyToObject();
                        }
                        else
                        {
                            newTargets.Add(arg.mfTo);
                            Debug.LogWarning($"对齐失败(距离太大) {arg}");
                        }
                    }
                    else
                    {
                        newTargets.Add(arg.mfTo);
                        Debug.LogError($"对齐失败(无结果数据) k:[{k}] id:[{id}] {arg}");
                    }
                }

                int count1 = meshFilters.Length;
                int count2 = newTargets.Count;//还剩多少模型没处理
                Debug.LogWarning($"完成一轮[{loopCount}]:{count1-count2}={count1}->{count2},PrefabCount:{prefabInfoList.Count}");
                meshFilters = newTargets.ToArray();//下一轮
                if (meshFilters.Length == 0)
                {
                    break;//完成了
                }

                int progressCount=targetCount-count2;
                //float progress1 = (float)progressCount / targetCount;
                ProgressArg arg1 = new ProgressArg("NewMeshAlignJobs", progressCount, targetCount);
                JobHandleList.SetJobProgress(arg1);

                jobList.Dispose();//new

                if (ProgressBarHelper.DisplayCancelableProgressBar(arg1))
                {
                    Debug.LogError("Break Jobs!!");
                    break;
                }
            }

            ProgressBarHelper.ClearProgressBar();

            double allT = 0;
            allArg.Sort();
            for (int i = 0; i < allArg.Count && i<10; i++)
            {
                AcRtAlignJobArg arg = allArg[i];
                Debug.LogError($"arg[{i}]:{arg}");
                allT += arg.Time;
            }
            Debug.LogError($"allT:{allT} t:{(DateTime.Now - start)}");

            Debug.LogError($"NewMeshAlignJobs Time:{(DateTime.Now - start)} TargetCount:{targetCount},PrefabCount:{prefabInfoList.Count},JobCount:{totalJobCount}({jobCountDetails}),LoopCount:{loopCount}");
            Debug.LogError($"{prefabInfoList.Count}\t{totalJobCount}\t{loopCount}\t{(DateTime.Now - start)}");
            MeshAlignJobContainer.PrintTime();
            prefabInfoList.Sort();

            RestoreParent(parentDict);

            return prefabInfoList;
        }
        #endregion
    }
}

