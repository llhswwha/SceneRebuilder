using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
using System;

namespace MeshJobs
{
    public class ThreePointJobResultList
    {
        public static ThreePointJobResultList Instance = new ThreePointJobResultList();

        public void Print()
        {
            foreach (var r in results)
            {
                r.Print();
            }
        }

        public Dictionary<int, ThreePointJobResult> go2Result = new Dictionary<int, ThreePointJobResult>();

        public void InitDict()
        {
            go2Result.Clear();
            foreach (var r in results)
            {
                AddDict(r);
            }
        }

        public ThreePointJobResult[] GetAllResults()
        {
            return results;
        }

        internal ThreePointJobResult GetThreePointResult(MeshPoints go1)
        {
            int goId = MeshHelper.GetInstanceID(go1);

            if (go2Result.ContainsKey(goId))
            {
                return go2Result[goId];
            }

            //不知道为什么，打印显示有关key是0,就是找不到的那个
            // Debug.LogWarning($"GetThreePoint1 go2Result Not Found Go:{goId},count:{go2Result.Count}");
            // foreach(var key in go2Result.Keys){
            //     Debug.LogWarning($"key:{key}");
            // }

            foreach (var r in results)
            {
                if (r.gId == goId)
                {
                    go2Result.Add(goId, r);
                    return r;
                }
            }

            //不知道为什么，打印显示有关key是0,就是找不到的那个
            Debug.LogWarning($"GetThreePoint1 go2Result Not Found Go:{goId},count:{go2Result.Count}");
            foreach (var key in go2Result.Keys)
            {
                Debug.LogWarning($"key:{key}");
            }

            Debug.LogError($"GetThreePoint2 results Not Found Go:{goId},count:{results.Length}");
            return new ThreePointJobResult() { IsNull = true };
        }

        public ThreePoint[] GetThreePoints(MeshPoints mf)
        {
            Transform t = mf.transform;
            ThreePointJobResult resultFrom = ThreePointJobResultList.Instance.GetThreePointResult(mf);
            resultFrom.localToWorldMatrix = t.localToWorldMatrix;
            var tpsFrom = resultFrom.GetThreePoints(t.localToWorldMatrix);
            return tpsFrom;
        }

        private ThreePointJobResult[] results;

        private void AddDict(ThreePointJobResult r)
        {
            // if (r == null)
            // {
            //     Debug.LogError("AddDict r==null");
            //     return;
            // }
            int goId = r.gId;
            if (go2Result.ContainsKey(goId))
            {
                go2Result[goId] = r;
            }
            else
            {
                go2Result.Add(goId, r);
            }
        }

        public void SetResult(ThreePointJobResult r, int id, int goId)
        {
            r.gId = goId;
            results[id] = r;
            //   Debug.Log($"SetResult id:{id},gId:{goId},r:{r},count:{go2Result.Count}");
        }

        internal void InitResult(int count)
        {
            results = new ThreePointJobResult[count];
        }
    }

    public static class ThreePointHelper
    {
        public static float planeScale = 0.1f;

        public static float pScale = 0.01f;

        public static float lineScale = 0.005f;

        public static GameObject RTPrefab = null;

        public static int TotalRTCount;

        public static RTTransform GetRT(ref ThreePoint tp, Transform parent)
        {
            //GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Plane);
            if (RTPrefab == null)
            {
                RTPrefab = new GameObject("RT");
                RTPrefab.SetActive(false);
            }
            GameObject tmp = RTPrefab;//重复利用同一个物体，获取不同的变换数据

            Transform t = tmp.transform;
            // tmp.name="RT";
            // t.localScale = new Vector3(planeScale, planeScale, planeScale);

            t.position = tp.GetCenterP();
            t.up = tp.GetLongShortNormal();
            var angle = Vector3.Angle(t.right, tp.GetShortLine());
            t.Rotate(Vector3.up, angle);

            RTTransform rT = new RTTransform(t.rotation, t.position);//获取不同的变换数据
                                                                     // t.SetParent(parent);
            TotalRTCount++;
            return rT;
        }

        public static Transform CreateNormalPlane(ref ThreePoint tp, Transform parent, bool showDetail)
        {
            var normalPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Transform t = normalPlane.transform;

            Vector3 center = tp.GetCenterP();
            Vector3 minMaxNormal = tp.GetLongShortNormal();

            t.localScale = new Vector3(planeScale, planeScale, planeScale);
            t.position = center;
            t.up = minMaxNormal;
            //t.right=this.GetShortLine();

            var angle = Vector3.Angle(t.right, tp.GetShortLine());
            t.Rotate(Vector3.up, angle);

            // normalPlane.name = "NormalPlane"+ log;
            // t.SetParent(this.transform);
            // vertextObjects.Add(normalPlane);

            if (showDetail)
            {
                Vector3 maxP = tp.GetMaxP();
                Vector3 minP = tp.GetMinP();
                CreateWorldPoint(center, "CenterP", t);
                CreateWorldPoint(maxP, "MaxP", t);
                CreateWorldPoint(minP, "MinP", t);
                CreateWorldLine(center, maxP, "LoneLine", t);
                CreateWorldLine(center, minP, "ShortLine", t);

                // Vector3 normalPoint = minMaxNormal + center;
                //  CreateWorldPoint(normalPoint, string.Format("[{0}]({1},{2},{3})", "minMaxNormal1", minMaxNormal.x, minMaxNormal.y, minMaxNormal.z),t);
                //  //NormalLineGo = CreateWorldLine(center, normalPoint, "minMaxNormal1:" + Vector3.Distance(center, normalPoint));
                // CreateWorldLine(center, normalPoint, "minMaxNormal1_" + Vector3.Distance(center, normalPoint),t);
            }
            t.SetParent(parent);
            return t;
        }

        private static GameObject CreateWorldLine(Vector3 p1, Vector3 p2, string n, Transform t)
        {
            GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g1.transform.position = (p1 + p2) / 2;
            g1.transform.forward = p2 - p1;
            var dis = Vector3.Distance(p2, p1);
            if (dis < 0.1f)
            {
                dis = 0.1f;
            }
            Vector3 scale = new Vector3(1f * lineScale, 1f * lineScale, dis);
            g1.transform.localScale = scale;
            g1.name = n;
            g1.transform.SetParent(t);
            return g1;
        }

        private static GameObject CreateWorldPoint(Vector3 p, string n, Transform t)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.localScale = new Vector3(pScale, pScale, pScale);
            go.transform.position = p;
            go.name = n;
            go.transform.SetParent(t);
            return go;
        }

    }

    public struct ThreePoint
    {
        public Vector3 center;

        public Vector3 minP;

        public int minId;

        public Vector3 maxP;

        public int maxId;

        public Matrix4x4 localToWorldMatrix;

        public ThreePoint(Vector3 c, Vector3 min, Vector3 max, int minId, int maxId, Matrix4x4 matrix)
        {
            this.center = c;
            this.minP = min;
            this.maxP = max;
            this.minId = minId;
            this.maxId = maxId;
            this.localToWorldMatrix = matrix;
        }

        public override string ToString()
        {
            return string.Format("(ThreePoint center:{0},maxP:{1},minP:{2})",
                 center.Vector3ToString(), maxP.Vector3ToString(), minP.Vector3ToString());
        }

        internal Vector3 GetCenterP(Transform t)
        {
            // return t.TransformPoint(center);
            //return TransformPoint(t.localToWorldMatrix,center);

            var c1 = t.TransformPoint(center);
            // var c2=t.localToWorldMatrix.MultiplyPoint3x4(center);
            // c1.PrintVector3("c1");
            // c2.PrintVector3("c2");
            return c1;
        }

        internal Vector3 GetCenterP()
        {
            // return t.TransformPoint(center);
            //return TransformPoint(t.localToWorldMatrix,center);

            var c1 = localToWorldMatrix.MultiplyPoint3x4(center);
            // var c2=t.localToWorldMatrix.MultiplyPoint3x4(center);
            // c1.PrintVector3("c1");
            // c2.PrintVector3("c2");
            return c1;
        }

        internal Vector3 GetNormalP()
        {
            return GetCenterP() + GetLongShortNormal();
        }

        internal Vector3 GetMaxP()
        {
            var c1 = localToWorldMatrix.MultiplyPoint3x4(maxP);
            return c1;
        }

        internal Vector3 GetMinP()
        {
            var c1 = localToWorldMatrix.MultiplyPoint3x4(minP);
            return c1;
        }

        internal Vector3 GetMaxPN()
        {
            return GetCenterP() + GetLongLineN();
        }

        internal Vector3 GetMinPN()
        {
            return GetCenterP() + GetShortLineN();
        }


        internal Vector3 GetCenterP(Matrix4x4 matrix)
        {
            var c1 = matrix.MultiplyPoint3x4(center);
            return c1;
        }

        public Vector3 GetLongShortNormal()
        {
            return GetLongShortNormal(this.localToWorldMatrix);
        }

        private Vector3 GetLongShortNormal(Matrix4x4 matrix)
        {
            var maxP = matrix.MultiplyPoint3x4(this.maxP);
            var minP = matrix.MultiplyPoint3x4(this.minP);
            var centerP = matrix.MultiplyPoint3x4(this.center);

            var longLine = maxP - centerP;
            longLine = Vector3.Normalize(longLine);

            var shortLine = minP - centerP;
            shortLine = Vector3.Normalize(shortLine);

            //Vector3 nor= Vector3.Cross(longLine,shortLine);

            Vector3 nor = Vector3.Cross(shortLine, longLine);

            // float angle1=Vector3.Angle(longLine,nor);//90度
            // float angle2=Vector3.Angle(shortLine,nor);//90度
            // Debug.Log($"GetLongShortNormal2 longLine:{longLine},shortLine:{shortLine},nor:({nor.x},{nor.y},{nor.z}),angle1:{angle1},angle2:{angle2}");
            return nor;
        }

        internal void ShowDebugDetail(Transform t)
        {
            var centerP = GetCenterP(t);
            CreateWorldPoint(centerP, "CenterP", t);

            //   var minP=
        }

        private GameObject CreateWorldPoint(Vector3 p, string n, Transform t)
        {
            float pScale = 0.1f;
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.localScale = new Vector3(pScale, pScale, pScale);
            go.transform.position = p;
            go.name = n;
            go.transform.SetParent(t);
            // vertextObjects.Add(go);
            return go;
        }

        //   private Vector3 GetShortLine(Matrix4x4 matrix)
        //   {
        //     throw new NotImplementedException();
        //   }

        //   private Vector3 GetLongLine(Matrix4x4 matrix)
        //   {
        //     throw new NotImplementedException();
        //   }

        internal Vector3 GetLongLine()
        {
            var maxP = localToWorldMatrix.MultiplyPoint3x4(this.maxP);
            var centerP = localToWorldMatrix.MultiplyPoint3x4(this.center);
            var longLine = maxP - centerP;
            return longLine;
        }

        internal Vector3 GetShortLine()
        {
            var minP = localToWorldMatrix.MultiplyPoint3x4(this.minP);
            var centerP = localToWorldMatrix.MultiplyPoint3x4(this.center);
            var shortLine = minP - centerP;
            return shortLine;
        }


        internal Vector3 GetLongLineN()
        {
            var maxP = localToWorldMatrix.MultiplyPoint3x4(this.maxP);
            var centerP = localToWorldMatrix.MultiplyPoint3x4(this.center);
            var longLine = maxP - centerP;
            return longLine.normalized;
        }

        internal Vector3 GetShortLineN()
        {
            var minP = localToWorldMatrix.MultiplyPoint3x4(this.minP);
            var centerP = localToWorldMatrix.MultiplyPoint3x4(this.center);
            var shortLine = minP - centerP;
            return shortLine.normalized;
        }

        internal float GetLongShortAngle()
        {
            return Vector3.Angle(this.GetLongLine(), this.GetShortLine());
        }

        internal void PrintLog(string tag)
        {
            Debug.Log($"[{tag}]{this.ToString()}");
        }
    }

    public struct ThreePointJobResult
    {
        public bool IsNull;
        public int id;

        public int gId;
        public int count;

        public int vertexCount;

        public double time;

        public Vector3 center;

        public float maxDis;
        public float minDis;

        public Vector3[] maxPList;

        public int minPCount;

        public Vector3[] minPList;

        public int maxPCount;

        public void Init()
        {
            // maxPList=new List<Vector3>();
            // minPList=new List<Vector3>();
        }

        public void Print()
        {
            // Debug.LogWarning(string.Format("ThreePointJobResult[{9}] vertexCount:{0},time:{1}ms,center:{2},maxP:{3},minP:{4},maxDis:{5},minDis:{6},{7},{8};",
            //  this.vertexCount,time,center,0,0,maxDis,minDis,0,0,count));

            Debug.LogWarning(string.Format("ThreePointJobResult[{9}] vertexCount:{0},time:{1}ms,center:{2},maxP:{3},minP:{4},maxDis:{5},minDis:{6},{7},{8};",
             this.vertexCount, time, center, maxPList[0], minPList[0], maxDis, minDis, maxPList.Length, minPList.Length, count));
        }

        public override string ToString()
        {
            return string.Format("(Result vertex:{0},center:{1},maxP:{2},minP:{3},maxDis:{4},minDis:{5},{6},{7};)",
                 this.vertexCount, center, maxPList[0], minPList[0], maxDis, minDis, maxPList.Length, minPList.Length);
        }

        public List<MinMaxId> GetAllMinMaxIds()
        {
            var meshData = this;

            List<MinMaxId> ids = new List<MinMaxId>();
            for (int i = 0; i < meshData.maxPList.Length; i++)
            {
                for (int j = 0; j < meshData.minPList.Length; j++)
                {
                    ids.Add(new MinMaxId(i, j));
                }
            }
            //allMinMax = ids;
            return ids;
        }

        public ThreePoint[] tps;

        public ThreePoint[] GetThreePoints(Matrix4x4 matrix)
        {
            var meshData = this;
            int count = meshData.maxPList.Length * meshData.minPList.Length;
            int id = 0;
            ThreePoint[] ids = new ThreePoint[count];
            for (int i = 0; i < meshData.maxPList.Length; i++)
            {
                var max = meshData.maxPList[i];
                for (int j = 0; j < meshData.minPList.Length; j++)
                {
                    var min = meshData.minPList[j];
                    ids[id] = new ThreePoint(center, min, max, j, i, matrix);
                    // ids.Add(new ThreePoint(center,min,max,j,i,matrix));
                    id++;//这个不能忘记了
                }
            }
            //allMinMax = ids;

            this.tps = ids;
            return ids;
        }

        public List<RTTransform> GetRTTransforms(Matrix4x4 matrix)
        {
            if (rts != null)
            {
                return rts;
            }
            GetThreePoints(matrix);
            return GetRTTransforms();
        }

        public List<RTTransform> rts;

        public List<RTTransform> GetRTTransforms()
        {
            List<RTTransform> tmp = new List<RTTransform>();
            for (int l = 0; l < tps.Length; l++)
            {
                var id1 = tps[l];

                var rt = ThreePointHelper.GetRT(ref id1, null);
                tmp.Add(rt);
            }
            this.rts = tmp;
            return tmp;
        }

        //     public Vector3 GetLongLine(Transform t)
        // {
        //     if(IsWorld)return maxPList[maxPId]-center;
        //     if(t==null) t=_obj.transform;
        //     return t.TransformPoint(maxPList[maxPId]) - t.TransformPoint(center);
        // }

        // public Vector3 GetShortLine(Transform t)
        // {
        //     if(IsWorld)return minPList[minPId]-center;
        //     if(t==null) t=_obj.transform;
        //     return t.TransformPoint(minPList[minPId]) - t.TransformPoint(center);
        // }


        public Vector3 GetLongShortNormal(Transform t, ThreePoint id)
        {
            var longLine = t.TransformPoint(id.maxP) - t.TransformPoint(center);
            longLine = Vector3.Normalize(longLine);

            var shortLine = t.TransformPoint(id.minP) - t.TransformPoint(center);
            shortLine = Vector3.Normalize(shortLine);

            Vector3 nor = Vector3.Cross(longLine, shortLine);

            Vector3 r = nor;

            // float angle1=Vector3.Angle(longLine,nor);//90度
            // float angle2=Vector3.Angle(shortLine,nor);//90度
            // Debug.Log($"GetLongShortNormal1 longLine:{longLine},shortLine:{shortLine},nor:({nor.x},{nor.y},{nor.z}),angle1:{angle1},angle2:{angle2}");

            // GetLongShortNormal(t.localToWorldMatrix,id);

            return r;
        }

        public Matrix4x4 localToWorldMatrix;

        public Vector3 GetLongShortNormal(ThreePoint id)
        {
            return GetLongShortNormal(this.localToWorldMatrix, id);
        }

        public Vector3 GetLongShortNormal(Matrix4x4 localToWorldMatrix, ThreePoint id)
        {
            var maxP = localToWorldMatrix.MultiplyPoint3x4(id.maxP);
            var minP = localToWorldMatrix.MultiplyPoint3x4(id.minP);
            var centerP = localToWorldMatrix.MultiplyPoint3x4(center);
            //var centerP=center*t.localToWorldMatrix;
            var longLine = maxP - centerP;

            longLine = Vector3.Normalize(longLine);
            var shortLine = minP - centerP;
            shortLine = Vector3.Normalize(shortLine);

            Vector3 nor = Vector3.Cross(longLine, shortLine);

            // float angle1=Vector3.Angle(longLine,nor);//90度
            // float angle2=Vector3.Angle(shortLine,nor);//90度
            // Debug.Log($"GetLongShortNormal2 longLine:{longLine},shortLine:{shortLine},nor:({nor.x},{nor.y},{nor.z}),angle1:{angle1},angle2:{angle2}");
            return nor;
        }


        public Vector3 TransformPoint(Matrix4x4 localToWorld, Vector3 pos)
        {
            //   Vector4 v = pos;
            //     v.w = 1;
            //     Vector4 wPos = localToWorld * v;
            //     return v;

            return localToWorld.MultiplyPoint3x4(pos);
        }


        internal Vector3 GetCenterP(Transform t)
        {
            // return t.TransformPoint(center);
            //return TransformPoint(t.localToWorldMatrix,center);

            var c1 = t.TransformPoint(center);
            // var c2=t.localToWorldMatrix.MultiplyPoint3x4(center);
            // c1.PrintVector3("c1");
            // c2.PrintVector3("c2");
            return c1;
        }
    }

    public static class TreePointJobHelper
    {
        public static JobList<ThreePointJob> CreateThreePointJobs(MeshPoints[] meshFilters, int size)
        {
            //DateTime start = DateTime.Now;
            int count = meshFilters.Length;
            JobList<ThreePointJob> handles = new JobList<ThreePointJob>("ThreePointJob", size);

            ThreePointJobResultList.Instance.InitResult(count);

            for (int i = 0; i < count; i++)
            {
                float progress = (float)i / count;
                float percents = progress * 100;
                if (ProgressBarHelper.DisplayCancelableProgressBar("CompleteAllPage", $"CreateThreePointJobs:{i}/{count} {percents:F1}%", progress))
                {
                    break;
                }

                MeshPoints filter = meshFilters[i];

                ThreePointJob job1 = new ThreePointJob();
                job1.id = i;
                //job1.goId = MeshHelper.GetInstanceID(filter);//模型会复制，但是sharedMesh不会改变
                job1.goId = filter.InstanceId;
                                                              //job1.results=results;
                job1.InitVertex(filter.vertices);
                handles.Add(job1);
            }
            ProgressBarHelper.ClearProgressBar();
            //Debug.Log($"CreateThreePointJobs Time:{(DateTime.Now - start).TotalMilliseconds}ms");
            return handles;
        }

        public static MeshPoints[] NewThreePointJobs(GameObject[] objs, int size)
        {
            MeshPoints[] meshFilters = new MeshPoints[objs.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                meshFilters[i] = new MeshPoints(objs[i]);
            }
            NewThreePointJobs(meshFilters, size);
            return meshFilters;
        }

        public static void NewThreePointJobs(MeshPoints[] meshFilters, int size)
        {
            //DateTime start = DateTime.Now;
            Debug.Log("NewThreePointJobs:" + meshFilters.Length);
            JobList<ThreePointJob> jobList = CreateThreePointJobs(meshFilters, size);
            //handles.CompleteAll();
            jobList.CompleteAllPage();
            foreach(var job in jobList.Jobs)
            {
                job.Dispose();
            }
            jobList.Dispose();

            ThreePointJobResultList.Instance.InitDict();

            //ThreePointJobResultList.Instance.Print();//打印确认结果

            //下面的不行，ThreePointJobResult是结构体的情况下，下面的没有数据
            //    List<ThreePointJobResult> results=new List<ThreePointJobResult>();
            //     for (int i = 0; i < jobList.Jobs.Count; i++)
            //    {
            //         ThreePointJob job = jobList.Jobs[i];
            //         //results.Add(job.Result);
            //         //job.Result.Print();
            //         //job.PrintResult();
            //         //job.GetResult().Print();
            //         // job.center.PrintVector3($"[{i}]job.center");
            //         Debug.Log($"[{i}] Job id:{job.id},count:{ThreePointJob.InvokeCount},minDis:{job.minDis},maxDis:{job.maxDis},center:{job.center},center2:{job.center2}");
            //    }

            //Debug.Log($">>NewThreePointJobs Count:{jobList.Length},Time:{(DateTime.Now - start).TotalMilliseconds}ms");//15/35
        }
    }

    [BurstCompile]
    public struct ThreePointJob : IJob
    {
        public int id;

        public int goId;

        public int vertexCount;
        // public NativeArray<float3> vertices;

        [ReadOnly] public NativeArray<Vector3> vertices;

        // public ThreePointJobResult Result;

        //public NativeArray<ThreePointJobResult> results;

        //public static int InvokeCount;

        public Vector3 center;

        public float3 center2;

        public float maxDis;
        public float minDis;

        public NativeList<Vector3> maxPList;

        public int minPCount;

        public NativeList<Vector3> minPList;

        public void Dispose()
        {
            maxPList.Dispose();
            minPList.Dispose();
            vertices.Dispose();
        }

        public int maxPCount;

        public void Execute()
        {
#if UNITY_EDITOR
            //DateTime start = DateTime.Now;

            // Mesh mesh=ManagedObjectWorld.Instance.Get(MeshRef);
            // Vector3[] vertices=mesh.vertices;//UnityException: get_canAccess can only be called from the main thread.
            // vertexCount=vertices.Length;

            Vector3 sumP = Vector3.zero;
            for (int i = 0; i < vertexCount; i++)
            {
                var v = vertices[i];
                sumP += v;
            }

            //center = sumP / this.vertexCount+centerOffset;//偏移一下避免对称模型无法得到唯一的特征点

            this.center = sumP / this.vertexCount;

            // center=Result.center ;
            // center2=Result.center;

            Vector3 maxP = Vector3.zero;
            Vector3 minP = Vector3.zero;
            this.maxDis = 0;
            this.minDis = float.MaxValue;

            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 p = vertices[i];
                var dis = Vector3.Distance(p, this.center);
                if (dis == this.maxDis)
                {
                    if (p == maxP) continue;//重复点
                    if (!this.maxPList.Contains(p))
                    {
                        this.maxPList.Add(p);
                    }
                }
                if (dis > this.maxDis)
                {
                    this.maxPList.Clear();
                    this.maxPList.Add(p);
                    this.maxDis = dis;
                    maxP = p;
                }

                if (dis == this.minDis)
                {
                    if (p == minP) continue;//重复点
                                            //minPList.Add(p);
                    if (!this.minPList.Contains(p))
                    {
                        this.minPList.Add(p);
                    }
                }
                if (dis < this.minDis)
                {
                    this.minPList.Clear();
                    this.minPList.Add(p);
                    this.minDis = dis;
                    minP = p;
                }
            }

            //InvokeCount++;

            // Debug.LogWarning(string.Format("GetVertexCenterInfo[{9}] vertexCount:{0},time:{1}ms,center:{2},maxP:{3},minP:{4},maxDis:{5},minDis:{6},{7},{8};",
            //     this.vertexCount,(DateTime.Now-start).TotalMilliseconds,this.center,maxP,minP,this.maxDis,this.minDis,this.maxPList.Length,this.minPList.Length,InvokeCount));

            // this.id=this.id;
            // this.count=InvokeCount;
            // this.time=(DateTime.Now-start).TotalMilliseconds;

            //Result.Print();

            // if(this.maxPList.Length>1||this.minPList.Length>1){
            //     Debug.LogWarning(string.Format("模型中心可能是对称的，存在多个最远点和最近点! maxDis:{0},minDis:{1}",this.maxPList.Length,this.minPList.Length));
            // }

            //Debug.Log(string.Format("ThreePointJob[{0}] vertexCount:{1}",InvokeCount,vertexCount));

            //results[id]=Result;

            //ThreePointJobResultList.Instance.Add(Result);//不能加

            // // 保存结果，后续使用
            // ThreePointJobResult result = GetResult();

            ThreePointJobResult result = new ThreePointJobResult();
            result.id = this.id;
            //result.count = InvokeCount;
            result.vertexCount = this.vertexCount;
            result.center = this.center;

            result.maxPList = this.maxPList.ToArray();
            result.maxPCount = this.maxPCount;
            result.maxDis = this.maxDis;

            result.minPList = this.minPList.ToArray();
            result.minPCount = this.minPCount;
            result.minDis = this.minDis;


            //result.time = (DateTime.Now - start).TotalMilliseconds;
            ThreePointJobResultList.Instance.SetResult(result, id, goId);
#endif
        }


        // public void PrintResult()
        // {
        //     Result.Print();
        // }

        // public ThreePointJobResult GetResult()
        // {
        //     ThreePointJobResult result = new ThreePointJobResult();
        //     result.id = this.id;
        //     //result.count = InvokeCount;
        //     result.vertexCount = this.vertexCount;
        //     result.center = this.center;

        //     result.maxPList = this.maxPList.ToArray();
        //     result.maxPCount = this.maxPCount;
        //     result.maxDis = this.maxDis;

        //     result.minPList = this.minPList.ToArray();
        //     result.minPCount = this.minPCount;
        //     result.minDis = this.minDis;
        //     return result;
        // }

        //private ManagedObjectRef<Mesh> MeshRef;

        //private ManagedObjectRef<Vector3[]> vs;

        //public void InitVertex(Mesh mesh)
        //{
        //    //MeshRef=ManagedObjectWorld.Instance.Add(mesh);//78ms

        //    //DateTime start=DateTime.Now;
        //    vertexCount = mesh.vertexCount;
        //    //vs=ManagedObjectWorld.Instance.Add(mesh.vertices);//958ms/484ms
        //    vertices = new NativeArray<Vector3>(mesh.vertices, Allocator.TempJob);//14641个2318个点要1117ms，没有这句话要90ms
        //                                                                          //vertexCount=0;
        //                                                                          //vertices= new NativeList<Vector3>(Allocator.TempJob);
        //                                                                          //   Result=new ThreePointJobResult();
        //                                                                          //   Result.minPList= new NativeList<Vector3>(Allocator.Persistent);
        //                                                                          //   Result.maxPList= new NativeList<Vector3>(Allocator.Persistent);

        //    minPList = new NativeList<Vector3>(Allocator.TempJob);
        //    maxPList = new NativeList<Vector3>(Allocator.TempJob);

        //    //   Vector3[] vs=mesh.vertices;//发现mesh.vertices放遍历里面的成本很大，要事先取出来再用
        //    //   vertices=new NativeArray<Vector3>(vertexCount,Allocator.TempJob);
        //    //   Vector3 sum=Vector3.zero;
        //    //   for(int i=0;i<vertexCount;i++) //遍历要花费时间，2318多个点要120-140ms
        //    //   {
        //    //     //var v=mesh.vertices[i];//120-140ms
        //    //     //sum+=v;
        //    //     //vertices[i]=mesh.vertices[i];//两个的话双倍

        //    //     var v=vs[i];
        //    //   }

        //    //Debug.Log($"InitVertex Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //}

        public void InitVertex(Vector3[] vs)
        {
            //MeshRef=ManagedObjectWorld.Instance.Add(mesh);//78ms

            //DateTime start=DateTime.Now;
            vertexCount = vs.Length;
            //vs=ManagedObjectWorld.Instance.Add(mesh.vertices);//958ms/484ms
            vertices = new NativeArray<Vector3>(vs, Allocator.TempJob);//14641个2318个点要1117ms，没有这句话要90ms
                                                                                  //vertexCount=0;
                                                                                  //vertices= new NativeList<Vector3>(Allocator.TempJob);
                                                                                  //   Result=new ThreePointJobResult();
                                                                                  //   Result.minPList= new NativeList<Vector3>(Allocator.Persistent);
                                                                                  //   Result.maxPList= new NativeList<Vector3>(Allocator.Persistent);

            minPList = new NativeList<Vector3>(Allocator.TempJob);
            maxPList = new NativeList<Vector3>(Allocator.TempJob);

            //   Vector3[] vs=mesh.vertices;//发现mesh.vertices放遍历里面的成本很大，要事先取出来再用
            //   vertices=new NativeArray<Vector3>(vertexCount,Allocator.TempJob);
            //   Vector3 sum=Vector3.zero;
            //   for(int i=0;i<vertexCount;i++) //遍历要花费时间，2318多个点要120-140ms
            //   {
            //     //var v=mesh.vertices[i];//120-140ms
            //     //sum+=v;
            //     //vertices[i]=mesh.vertices[i];//两个的话双倍

            //     var v=vs[i];
            //   }

            //Debug.Log($"InitVertex Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        }
    }
}