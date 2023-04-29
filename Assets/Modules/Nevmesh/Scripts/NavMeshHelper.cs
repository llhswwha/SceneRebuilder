using Dest.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshHelper
{
    public static void ShowNavMesh(GameObject obj)
    {
        var triangles = NavMesh.CalculateTriangulation();
        Vector3[] vertices = triangles.vertices;
        //for (int i = 0; i < vertices.Length; ++i)
        //{
        //    vertices[i].x *= 1.0f / 30.0f;
        //    vertices[i].z *= 1.0f / 40.0f;
        //}
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles.indices;
        obj.GetComponent<MeshFilter>().sharedMesh = mesh;

        //var triangles = NavMesh.CalculateTriangulation();
        //vertices = triangles.vertices;//顶点
        var indicesCount = vertices.Length;
        var indices = triangles.indices;//索引
        if (indices.Length % 3 != 0)
        {
            Debug.LogError("顶点数量不对 不是3的倍数 :" + indices.Length);
        }
        else
        {
            int count = 0;
            for (int i = 0; i < indices.Length; i += 3)
            {
                var i1 = indices[i];
                var i2 = indices[i + 1];
                var i3 = indices[i + 2];
                var p1 = vertices[i1];
                var p2 = vertices[i2];
                var p3 = vertices[i3];
                ShowTriangle(obj, count, p1, p2, p3);

                //segments.Add(new Segment3(p1, p2));
                //segments.Add(new Segment3(p2, p3));
                //segments.Add(new Segment3(p3, p1));

                //planes.Add(new Plane3(p1, p2, p3));
                //triangle3s.Add(new Triangle3(p1, p2, p3));
                count++;
            }
        }
    }

    private static GameObject ShowTriangle(GameObject parent, int count, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        GameObject triangle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        triangle.name = "triangle" + count;
        Mesh triangleMesh = new Mesh();
        triangleMesh.vertices = new Vector3[] { p1, p2, p3 };
        triangleMesh.triangles = new int[] { 0, 1, 2 };
        triangle.GetComponent<MeshFilter>().sharedMesh = triangleMesh;

        if (parent != null)
        {
            triangle.transform.parent = parent.transform;
        }

        //var pO1 = NavMeshHelper.CreatePoint(p1, "p" + count + "[1]", 0.1f, Color.white);
        //pO1.transform.parent = triangle.transform;

        //var pO2 = NavMeshHelper.CreatePoint(p2, "p" + count + "[2]", 0.1f, Color.white);
        //pO2.transform.parent = triangle.transform;

        //var pO3 = NavMeshHelper.CreatePoint(p3, "p" + count + "[3]", 0.1f, Color.white);
        //pO3.transform.parent = triangle.transform;

        return triangle;
    }

    public class NavMeshInfo
    {

        public List<Triangle3> triangle3s = new List<Triangle3>();
        public List<Plane3> planes = new List<Plane3>();//平面 （三角形）
        public List<Segment3> segments = new List<Segment3>();
        public int indicesCount = 0;

        public Vector3[] vertices;

        public NavMeshInfo()
        {
            var triangles = NavMesh.CalculateTriangulation();
            vertices = triangles.vertices;//顶点
            indicesCount = vertices.Length;
            var indices = triangles.indices;//索引
            if (indices.Length % 3 != 0)
            {
                Debug.LogError("顶点数量不对 不是3的倍数 :" + indices.Length);
            }
            else
            {
                for (int i = 0; i < indices.Length; i += 3)
                {
                    var i1 = indices[i];
                    var i2 = indices[i + 1];
                    var i3 = indices[i + 2];
                    var p1 = vertices[i1];
                    var p2 = vertices[i2];
                    var p3 = vertices[i3];

                    segments.Add(new Segment3(p1, p2));
                    segments.Add(new Segment3(p2, p3));
                    segments.Add(new Segment3(p3, p1));

                    //Debug.DrawLine(p1, p2, Color.red, 300);
                    //Debug.DrawLine(p2, p3, Color.red, 300);
                    //Debug.DrawLine(p3, p1, Color.red, 300);

                    planes.Add(new Plane3(p1, p2, p3));
                    triangle3s.Add(new Triangle3(p1, p2, p3));
                }
                //Debug.LogErrorFormat("Navmeshinfo Set:{0} Planes:{1}",segments.Count,planes.Count);
            }

        }
    }
    public static NavMeshInfoT navMeshInfoT = null;

    public class NavMeshInfoT
    {
        public Vector3 startCenterPos=new Vector3(-268,-10,-38);
        public Vector3 endCenterPos=new Vector3(265,60,24);
        public float cubeSize = 10;//10*10*10的unity单位
        public float maxCheckRadius = 15;
        public List<NavMeshCubeInfo> CubeList = new List<NavMeshCubeInfo>();

        public List<Triangle3> triangle3s = new List<Triangle3>();
        public List<Plane3> planes = new List<Plane3>();//平面 （三角形）
        public List<Segment3> segments = new List<Segment3>();

        public NavMeshInfoT()
        {
            var triangles = NavMesh.CalculateTriangulation();
            var vertices = triangles.vertices;//顶点
            var indicesCount = vertices.Length;
            var indices = triangles.indices;//索引
            if (indices.Length % 3 != 0)
            {
                Debug.LogError("顶点数量不对 不是3的倍数 :" + indices.Length);
            }
            else
            {
                //DateTime nowT = DateTime.Now;
                //string logT = "";
                InitCubeInfoByIndex(vertices);//根据定位位置，算出对角坐标，生成CubeList
                //logT += string.Format("InitCubeInfo Count:{0},CostTime:{1} ms\n",CubeList.Count,(DateTime.Now-nowT).TotalMilliseconds);
                //nowT = DateTime.Now;
                int useSegment=0;
                int usePlane = 0;
                for (int i = 0; i < indices.Length; i += 3)
                {
                    var i1 = indices[i];
                    var i2 = indices[i + 1];
                    var i3 = indices[i + 2];
                    var p1 = vertices[i1];
                    var p2 = vertices[i2];
                    var p3 = vertices[i3];
                   
                    bool isInCube = false;
                    for (int j=0;j< CubeList.Count;j++)
                    {
                        var cubeT = CubeList[j];
                        if(cubeT.isPosInRange(p1) && cubeT.isPosInRange(p2) && cubeT.isPosInRange(p3))
                        {                            
                            cubeT.AddSegments(new Segment3(p1, p2));
                            cubeT.AddSegments(new Segment3(p2, p3));
                            cubeT.AddSegments(new Segment3(p3, p1));
                            cubeT.AddPlanes(new Plane3(p1,p2,p3));
                            cubeT.AddTrangle3(new Triangle3(p1,p2,p3));
                            useSegment+=3;
                            usePlane++;
                            isInCube = true;
                            break;
                        }
                    } 
                    if(!isInCube)
                    {
                        segments.Add(new Segment3(p1, p2));
                        segments.Add(new Segment3(p2, p3));
                        segments.Add(new Segment3(p3, p1));

                        planes.Add(new Plane3(p1, p2, p3));
                        triangle3s.Add(new Triangle3(p1, p2, p3));
                    }
                }
                //logT += string.Format("SortCubeInfo CostTime:{0} ms\n", (DateTime.Now - nowT).TotalMilliseconds);
                //nowT = DateTime.Now;
                RemvoeEmptyCube();
                //logT += string.Format("SortCubeInfo CostTime:{0} ms  UserFulCube:{1} CubeSeg:{2} CubePlane:{3} NormalSeg:{4} NormalPlane:{5} \n"
                //    , (DateTime.Now - nowT).TotalMilliseconds,CubeList.Count,useSegment,usePlane,segments.Count,planes.Count);
                //Debug.LogError("Value:"+logT);
            }
        }
        private void InitCubeInfoByIndex(Vector3[] vertices)
        {
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 pos = vertices[i];
                maxX = pos.x > maxX ? pos.x : maxX;
                maxY = pos.y > maxY ? pos.y : maxY;
                maxZ = pos.z > maxZ ? pos.z : maxZ;

                minX = pos.x < minX ? pos.x : minX;
                minY = pos.y < minY ? pos.y : minY;
                minZ = pos.z < minZ ? pos.z : minZ;
            }
            startCenterPos = new Vector3(minX,minY,minZ);
            endCenterPos = new Vector3(maxX,maxY,maxZ);
            InitCubeInfo();
        }
        private void RemvoeEmptyCube()
        {
            int removeCount=CubeList.RemoveAll(i => i.segments.Count == 0 && i.triangle3s.Count == 0);//&&i.planes.Count==0);
        }

        public Vector3 GetClosedPointBySegment(Vector3 targetT)
        {
            var minDistance = float.MaxValue;
            Vector3 r = targetT;
            bool findResult = false;
            foreach (var item in CubeList)
            {
                float distanceToPoint = Vector3.Distance(item.centerPos, targetT);
                if(distanceToPoint < maxCheckRadius)
                {
                    findResult = true;
                    foreach(var seg in item.segments)
                    {
                        Segment3 segment = seg;
                        Vector3 point = targetT;
                        Vector3 closestPoint;
                        float dist0 = Distance.Point3Segment3(ref point, ref segment, out closestPoint);
                        if (dist0 < minDistance)
                        {
                            minDistance = dist0;
                            r = closestPoint;
                        }
                    }
                }
            }
            //空中的点，不在包围和范围内。需要整体查找一遍
            if (!findResult)
            {
                //Debug.Log("!Find GetClosedPointBySegment:" + CubeList.Count);
                foreach (var item in CubeList)
                {
                    foreach (var segT in item.segments)
                    {
                        Segment3 segment = segT;
                        Vector3 point = targetT;
                        Vector3 closestPoint;
                        float dist0 = Distance.Point3Segment3(ref point, ref segment, out closestPoint);
                        if (dist0 < minDistance)
                        {
                            minDistance = dist0;
                            r = closestPoint;
                        }
                    }
                }
            }
            //找完包围盒的点，再比较不在包围和内的点
            foreach (var item in segments)
            {
                Segment3 segment = item;
                Vector3 point = targetT;
                Vector3 closestPoint;
                float dist0 = Distance.Point3Segment3(ref point, ref segment, out closestPoint);
                if (dist0 < minDistance)
                {
                    minDistance = dist0;
                    r = closestPoint;
                }
            }
            return r;
        }

        public Vector3 GetClosedTrianglesByPoint(Vector3 targetT)
        {
            Vector3 r = Vector3.zero;
            var minDistance = float.MaxValue;
            bool findResult = false;
            foreach (var item in CubeList)
            {
                float distanceToPoint = Vector3.Distance(item.centerPos, targetT);
                if (distanceToPoint < maxCheckRadius)
                {
                    findResult = true;
                    for(int i=0;i<item.triangle3s.Count;i++)
                    {
                        Triangle3 triangle3 = item.triangle3s[i];
                        Plane3 plane = item.planes[i];
                        Vector3 point = targetT;
                        Vector3 closestPoint;
                        float dist0 = Distance.Point3Plane3(ref point, ref plane, out closestPoint);
                        Line3 line = new Line3(targetT, closestPoint - targetT);
                        Triangle3 triangle = triangle3;
                        Line3Triangle3Intr info;
                        bool find = Intersection.FindLine3Triangle3(ref line, ref triangle, out info);
                        if (find) //只有点在三角形内才有效
                        {
                            if (dist0 < minDistance)
                            {
                                minDistance = dist0;
                                r = closestPoint;
                            }
                        }
                    }                    
                }
            }
            if (!findResult)
            {
                foreach (var item in CubeList)
                {
                    for(int i=0;i<item.triangle3s.Count;i++)
                    {
                        Triangle3 triangle3 = item.triangle3s[i];
                        Plane3 plane = item.planes[i];
                        Vector3 point = targetT;
                        Vector3 closestPoint;
                        float dist0 = Distance.Point3Plane3(ref point, ref plane, out closestPoint);
                        Line3 line = new Line3(targetT, closestPoint - targetT);
                        Triangle3 triangle = triangle3;
                        Line3Triangle3Intr info;
                        bool find = Intersection.FindLine3Triangle3(ref line, ref triangle, out info);
                        if (find) //只有点在三角形内才有效
                        {
                            if (dist0 < minDistance)
                            {
                                minDistance = dist0;
                                r = closestPoint;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < triangle3s.Count; i++)
            {
                Triangle3 triangle3 = triangle3s[i];
                Plane3 plane = planes[i];
                Vector3 point = targetT;
                Vector3 closestPoint;
                float dist0 = Distance.Point3Plane3(ref point, ref plane, out closestPoint);
                Line3 line = new Line3(targetT, closestPoint - targetT);
                Triangle3 triangle = triangle3;
                Line3Triangle3Intr info;
                bool find = Intersection.FindLine3Triangle3(ref line, ref triangle, out info);
                if (find) //只有点在三角形内才有效
                {
                    if (dist0 < minDistance)
                    {
                        minDistance = dist0;
                        r = closestPoint;
                    }
                }
            }
            return r;
        }
        public void InitCubeInfo()
        {
            int xCubeCount = Mathf.CeilToInt(Mathf.Abs(endCenterPos.x - startCenterPos.x) / cubeSize);
            int yCubeCount = Mathf.CeilToInt(Mathf.Abs(endCenterPos.y - startCenterPos.y) / cubeSize);
            int zCubeCount = Mathf.CeilToInt(Mathf.Abs(endCenterPos.z - startCenterPos.z) / cubeSize);

            for(int i=0;i<xCubeCount;i++)
            {
                for(int j=0;j<yCubeCount;j++)
                {
                    for(int k=0;k<zCubeCount;k++)
                    {
                        Vector3 centerPos = startCenterPos + new Vector3(i*cubeSize,j*cubeSize,k*cubeSize);
                        NavMeshCubeInfo infoT = new NavMeshCubeInfo(i,j,k, centerPos, cubeSize);
                        CubeList.Add(infoT);
                    }
                }
            }
        }
    }

    public class NavMeshCubeInfo
    {
        public float MinX;
        public float MaxX;
        public float MinY;
        public float MaxY;
        public float MinZ;
        public float MaxZ;
        public Vector3 centerPos;

        public List<Triangle3> triangle3s = new List<Triangle3>();
        public List<Plane3> planes = new List<Plane3>();//平面 （三角形）
        public List<Segment3> segments = new List<Segment3>();

        public NavMeshCubeInfo(int xCount,int yCounnt,int zCount,Vector3 centerPosT,float width)
        {
            float halfWidth = width / 2;
            MinX = centerPosT.x-halfWidth;
            MaxX = centerPosT.x + halfWidth;
            MinY = centerPosT.y - halfWidth;
            MaxY = centerPosT.y+ halfWidth;
            MinZ = centerPosT.z- halfWidth;
            MaxZ = centerPosT.z+ halfWidth;
            centerPos = centerPosT;
        }

        public void AddSegments(Segment3 segT)
        {
            segments.Add(segT);
        }

        public void AddPlanes(Plane3 planeT)
        {
            planes.Add(planeT);
        }
        public void AddTrangle3(Triangle3 triT)
        {
            triangle3s.Add(triT);
        }
        public bool isPosInRange(Vector3 pos)
        {
            return pos.x > MinX && pos.x < MaxX && pos.y > MinY && pos.y < MaxY && pos.z > MinZ && pos.z < MaxZ;
        }
    }


    public static Dictionary<string,GameObject> navPoints = new Dictionary<string, GameObject>();

    public static void RefreshNavMeshInfo()
    {
        if(LocationManager.Instance&&LocationManager.Instance.UseNavmeshTCaculate)
        {
            navMeshInfoT = new NavMeshInfoT();
        }
        else
        {
            navMeshInfo = new NavMeshInfo();
        }        
    }
    public static void InitNavmeshInfo()
    {
        if (LocationManager.Instance && LocationManager.Instance.UseNavmeshTCaculate)
        {
            if(navMeshInfoT==null)navMeshInfoT = new NavMeshInfoT();
        }
        else
        {
            if (navMeshInfo == null) navMeshInfo = new NavMeshInfo();
        }
    }
    public static string currentName = "";


    //public static void GetClosetPointAsync_Old(Vector3 target, string name, NavMeshAgent agent, Action<Vector3, GameObject> callback)
    //{
    //    currentTarget = target;
    //    currentName = name;
    //    Log = "";
    //    var psList = GetSampleAndEdge(target);


    //    CreatePoint(target, "target", 0.1f, Color.green);//目标点

    //    if (navMeshInfo == null)
    //        navMeshInfo = new NavMeshInfo();
    //    var p1 = Vector3.zero;
    //    //var p2 = Vector3.zero;
    //    ThreadManager.Run(() =>
    //    {
    //        try
    //        {
    //            //p1 = GetClosetPointByTriangle(target);
    //            //p2 = GetClosetPointBySegment(target);

    //            p1 = GetClosetPointByMesh(target);
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError("GetClosetPointAsync:" + e.ToString());
    //        }
    //    }, () =>
    //    {
    //        try
    //        {
    //            p1 = GetSamplePosition(p1);
    //            psList.Add(p1);
    //            CreatePoint(p1, "ByMesh", 0.1f, Color.red);//用三角形平面计算出来的最近的点
    //            //p2 = GetSamplePosition(p2);
    //            //psList.Add(p2);
    //            //CreatePoint(p2, "BySegment", 0.1f, Color.yellow);//用线段计算出来的最近的点

    //            var p3 = GetDownSample(target);
    //            if (p3 != target)
    //            {
    //                CreatePoint(p3, "Down", 0.1f, Color.magenta);
    //            }

    //            //var p4 = GetUpPosition(target);
    //            //if (p4 != target)
    //            //{
    //            //    CreatePoint(p4, "Up", 0.1f, Color.blue);
    //            //}


    //            var r = GetClosedPoint(psList.ToArray(), target, agent);
    //            var p = CreatePoint(r, "Closed", 0.1f, Color.cyan);//前面这些点中的最近的点，及结果。

    //            if (callback != null)
    //            {
    //                callback(r, p);
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            if (callback != null)
    //            {
    //                callback(target, null);
    //            }
    //        }
    //    }, "GetClosetPointByTriangleEx+GetClosetPointBySegmentEx");
    //}

    public static void ClearTestPoints()
    {
        if (IsDebug)
        {
            foreach (var point in navPoints)
            {
                GameObject.DestroyImmediate(point.Value);
            }

            navPoints.Clear();
        }
        
    }

    private static List<Vector3> GetSampleAndEdge(Vector3 target)
    {
        ClearTestPoints();

        List<Vector3> psList = new List<Vector3>();
        NavMeshHit hitInfo;

        if (NavMesh.FindClosestEdge(target, out hitInfo, -1))
        {
            psList.Add(hitInfo.position);
            //var t1=CreatePoint(hitInfo.position, "Edge", 0.1f, Color.green);//Edge：边缘上的点
        }
        else
        {
            //Debug.LogError("GetClosetPoint No FindClosestEdge ");
        }

        if (NavMesh.SamplePosition(target, out hitInfo, 100, -1))
        {
            psList.Add(hitInfo.position);
            //var t2=CreatePoint(hitInfo.position, "Sample", 0.1f, Color.blue);//Sample：采样点
        }
        else
        {
            //Debug.LogError("GetClosetPoint No SamplePosition ");
        }

        return psList;
    }

    public static Vector3 currentTarget;

    public static string LogText = "";

    public static Vector3 globalOffset = new Vector3(0, -0.1f, 0);


    public static void GetClosetPointAsync(Vector3 target, string name, NavMeshAgent agent, Action<Vector3, GameObject> callback)
    {
        //Debug.LogError("GetClosetPointAsync:" + target);

        string value = "";
        DateTime m = DateTime.Now;
        if (target.y > 1)
        {
            target = target + globalOffset;
        }       
        currentTarget = target;
        currentName = name;
        LogText = "";
        //var psList = GetSampleAndEdge(target);

        //ClearTestPoints();
        NavPointList psList = GetSampleAndEdgeEx(target, agent);//使用NavMesh.FindClosestEdge，NavMesh.SamplePosition计算点

        //value += string.Format("GetSampleAndEdgeEx.CostTime:{0}ms\n",(DateTime.Now-m).TotalMilliseconds);
        //m = DateTime.Now;

        CreatePoint(target, "target", 0.1f, Color.green, agent);//目标点

        //value += string.Format("CreatePoint.CostTime:{0}ms\n", (DateTime.Now - m).TotalMilliseconds);
        //m = DateTime.Now;
        InitNavmeshInfo();
        var p1 = Vector3.zero;
        var p2 = Vector3.zero;
        //Debug.LogError("target11111:" + target);
        ThreadManager.Run(() =>
        {
            try
            {
                //p1 = GetClosetPointByTriangle(target);
                //p2 = GetClosetPointBySegment(target);

                if (!IsYouHua)
                {
                    p1 = GetClosetPointByMesh(target);//YZL20210330:方法中的GetClosetPointByTriangle1，这个结果等于SamplePosition计算结果
                }
                else
                {
                    p1 = GetClosetPointBySegment1(target);
                }

                p2 = target;
            }
            catch (Exception e)
            {
                //Debug.LogError("GetClosetPointAsync:" + e.ToString());
            }
        }, () =>
        {
            try
            {
                if (!IsYouHua)
                {
                    p1 = GetSamplePosition(p1);
                }

                psList.Mesh = new NavPoint("ByMesh", p1, target, Color.red, agent);


                if (p2 != Vector3.zero)
                {
                    //Debug.LogError("GetSamplePosition Down1:" + p2);
                    p2 = GetSamplePosition(p2);

                    //value += string.Format("p2->GetSamplePosition.CostTime:{0}ms\n", (DateTime.Now - m).TotalMilliseconds);
                    //m = DateTime.Now;

                    //Debug.LogError("GetSamplePosition Down2:" + p2);
                    psList.Down = new NavPoint("Down", p2, target, Color.yellow, agent);

                    //GetDownCast(p2, "Down");
                    CreatePoint(target, "GetSamplePosition(p2)", 0.1f, Color.green, agent);//目标点
                }

                //var p3 = GetDownSample(target);
                //if (p3 != target)
                //{
                //    CreatePoint(p3, "Down", 0.1f, Color.magenta);
                //}

                //var r = GetClosedPoint(psList.ToArray(), target, agent);
                //var p = CreatePoint(r, "Closed", 0.1f, Color.cyan);//前面这些点中的最近的点，及结果。

                //Debug.LogError("psList.GetMinDistancePoint");
                var r = psList.GetMinDistancePoint(target, agent);

                //value += string.Format("GetMinDistancePoint.CostTime:{0}ms\n", (DateTime.Now - m).TotalMilliseconds);
                //m = DateTime.Now;

                if (IsDebug)
                {
                    var closedObj = CreatePoint(r.Position, "最终位置点_Closed", 0.2f, Color.black, agent);
                    if (closedObj != null)
                    {
                        closedObj.AddComponent<NavTestInfo>().agent = agent;
                    }
                }

                //Debug.Log("GetClosetPointAsync,CostTime sample:\n"+value);
                Debug.DrawLine(segmentT.P0, segmentT.P1, Color.green, 1F);
                if (callback != null)
                {
                    callback(r.Position, null);
                }
            }
            catch (Exception e)
            {
                if (callback != null)
                {
                    callback(target, null);
                }
            }
        }, "GetClosetPointByTriangleEx+GetClosetPointBySegmentEx");
    }

    /// <summary>
    /// 获取移动巡检点在NavMesh上最近的点
    /// </summary>
    /// <param name="target"></param>
    /// <param name="name"></param>
    /// <param name="agent"></param>
    /// <param name="callback"></param>
    public static void GetMobileInspectionClosetPointAsync(Vector3 target,int IndexT, Action<Vector3,int, GameObject> callback)
    {
        string value = "";
        DateTime m = DateTime.Now;
        //currentTarget = target;
        //currentName = name;
        LogText = "";
        //NavPointList psList = GetSampleAndEdgeEx(target, agent);//使用NavMesh.FindClosestEdge，NavMesh.SamplePosition计算点
        NavPointList psList = GetSampleAndEdgeEx(target, null);//使用NavMesh.FindClosestEdge，NavMesh.SamplePosition计算点

        //CreatePoint(target, "target", 0.1f, Color.green, agent);//目标点
        InitNavmeshInfo();
        var p1 = Vector3.zero;
        var p2 = Vector3.zero;
        ThreadManager.Run(() =>
        {
            try
            {
                p1 = GetClosetPointByMesh(target);

                p2 = target;
            }
            catch (Exception e)
            {
                //Debug.LogError("GetClosetPointAsync:" + e.ToString());
            }
        }, () =>
        {
            try
            {
                p1 = GetSamplePosition(p1);

                psList.Mesh = new NavPoint("ByMesh", p1, target, Color.red, null);

                if (p2 != Vector3.zero)
                {
                    p2 = GetSamplePosition(p2);
                    psList.Down = new NavPoint("Down", p2, target, Color.yellow, null);

                }

                //var r = psList.GetMinDistancePoint(target, agent);
                var r = psList.GetMinDistancePoint(target, null);

                if (IsDebug)
                {
                    var closedObj = CreatePoint(r.Position, "Closed", 0.2f, Color.black, null);
                    if (closedObj != null)
                    {
                        closedObj.AddComponent<NavTestInfo>().agent = null;
                    }
                }

                if (callback != null)
                {
                    callback(r.Position, IndexT, null);
                }
            }
            catch (Exception e)
            {
                if (callback != null)
                {
                    callback(target, IndexT, null);
                }
            }
        }, "GetMobileInspectionClosetPointAsync");
    }

    public static Vector3 GetClosetPointEx(Vector3 target, NavMeshAgent agent)
    {
        Debug.Log("-------------------GetClosetPointEx--------------");
        if (target.y > 1)
        {
            target = target + globalOffset;
        }
        currentTarget = target;
        //var psList = GetSampleAndEdge(target);

        ClearTestPoints();

        //List<NavPoint> psList = new List<NavPoint>();
        NavPointList psList = GetSampleAndEdgeEx(target, agent);

        //var p1 = GetClosetPointByTriangle(target);
        //psList.Add(new NavPoint("ByTriangle", p1, Color.red, agent));

        //var p2 = GetClosetPointBySegment(target);
        //psList.Add(new NavPoint("BySegment", p2, Color.yellow, agent));

        var p1 = GetClosetPointByMeshEx(target);
        //psList.Add(new NavPoint("ByMesh", p1, Color.red, agent));
        psList.Mesh = new NavPoint("ByMesh", p1, target, Color.red, agent);
        var p3 = GetDownSample(target,downCount,downStep, agent);
        if (p3 != target)
        {
            //psList.Add(new NavPoint("Down", p3, Color.magenta, agent));
            psList.Down = new NavPoint("Down", p3, target, Color.magenta, agent);
        }

        var r = psList.GetMinDistancePoint(target, agent);
        if (r == null)
        {
            
            Log.Error("GetClosetPointEx","NoClosetPoint:"+target+","+agent.transform.name+"|"+psList);
            return Vector3.zero;
        }
        else
        {
            //var t4 = CreatePoint(r.Position, "Closed", 0.2f, Color.black);
            //t4.AddComponent<NavTestInfo>().agent = agent;
            return r.Position;
        }
        
    }

    //public static bool IsYouHua = true;//代码优化
    public static bool IsYouHua
    {
        get {
            return LocationManager.Instance.IsPathFindYouHua;
        }
    }//代码优化

    public static float MaxDistance = 15;

    private static NavPointList GetSampleAndEdgeEx(Vector3 target, NavMeshAgent agent)
    {
        NavPointList psList = new NavPointList();
        NavMeshHit hitInfo;

        if (!IsYouHua)
        {
            if (NavMesh.FindClosestEdge(target, out hitInfo, -1))
            {
                NavPoint navP = new NavPoint("Edge", hitInfo.position, target, Color.green, agent);
                //psList.Add(navP);
                psList.Edge = navP;

                GetDownCast(hitInfo.position, "Edge", agent);
            }
            else
            {
                //Debug.LogError("GetClosetPoint No FindClosestEdge ");
            }
        }

        if (NavMesh.SamplePosition(target, out hitInfo, 100, -1))
        {
            var dis = Vector3.Distance(hitInfo.position, target);
            if (dis < MaxDistance)
            {
                NavPoint navP = new NavPoint("Sample", hitInfo.position, target, Color.blue, agent);
                //psList.Add(navP);
                psList.Sample = navP;

                GetDownCast(hitInfo.position, "Sample", agent);
            }
        }
        else
        {
            //Debug.LogError("GetClosetPoint No SamplePosition ");
        }

        return psList;
    }

    public static Vector3 GetClosetPoint(Vector3 target,NavMeshAgent agent)
    {
        currentTarget = target;
        //var psList = GetSampleAndEdge(target);

        ClearTestPoints();

        List<Vector3> psList = new List<Vector3>();
        NavMeshHit hitInfo;

        if (NavMesh.FindClosestEdge(target, out hitInfo, -1))
        {
            psList.Add(hitInfo.position);
            //var t01 = CreatePoint(hitInfo.position, "Edge", 0.1f, Color.green);//Edge：边缘上的点

            //NavTestInfo info=t01.AddComponent<NavTestInfo>();
            //info.agent = agent;
        }
        else
        {
            //Debug.LogError("GetClosetPoint No FindClosestEdge ");
        }

        if (NavMesh.SamplePosition(target, out hitInfo, 100, -1))
        {
            psList.Add(hitInfo.position);
            //var t02 = CreatePoint(hitInfo.position, "Sample", 0.1f, Color.blue);//Sample：采样点
            //                                                                    //t01.AddComponent<NavTestInfo>();

            //NavTestInfo info = t02.AddComponent<NavTestInfo>();
            //info.agent = agent;
        }
        else
        {
            //Debug.LogError("GetClosetPoint No SamplePosition ");
        }

        //var p1 = GetClosetPointByTriangleEx(target);
        //psList.Add(p1);
        //var t1=CreatePoint(p1, "ByTriangle", 0.1f, Color.red);
        //t1.AddComponent<NavTestInfo>().agent=agent;

        //var p2 = GetClosetPointBySegmentEx(target);
        //psList.Add(p2);
        //var t2=CreatePoint(p2, "BySegment", 0.1f, Color.yellow);
        //t2.AddComponent<NavTestInfo>().agent = agent;

        var p1 = GetClosetPointByMeshEx(target);

        var p3 = GetDownSample(target, downCount, downStep, agent);
        //if (p3 != target)
        //{
        //    var t3=CreatePoint(p3, "Down", 0.1f, Color.magenta);
        //    t3.AddComponent<NavTestInfo>().agent = agent;
        //}

        var r = GetClosedPoint(psList.ToArray(), target, agent);
        //var t4=CreatePoint(r, "Closed", 0.1f, Color.cyan);
        //t4.AddComponent<NavTestInfo>().agent = agent;
        return r;
    }

    public static float GetPathDistance(NavMeshPath path)
    {
        float d = 0;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            //Debug.DrawLine(Path.corners[i], Path.corners[i + 1], Color.red);
            var d2 = Vector3.Distance(path.corners[i], path.corners[i + 1]);
            d += d2;
        }
        return d;
    }

    public static int downCount = 10;
    public static float downStep = 0.2f;



    //public static Vector3 GetClosetPointBySegmentEx(Vector3 target)
    //{
    //    var r = GetClosetPointBySegment(target);
    //    r = GetSamplePosition(r);
    //    return r;

    //}
    //public static Vector3 GetClosetPointByTriangleEx(Vector3 target)
    //{
    //    var r = GetClosetPointByTriangle(target);
    //    r = GetSamplePosition(r);
    //    return r;
    //}

    public static Vector3 GetSamplePosition(Vector3 r)
    {
        NavMeshHit hitInfo;
        if (NavMesh.SamplePosition(r, out hitInfo, 100, -1))
        {
            r = hitInfo.position;


        }
        else
        {
            //Debug.LogError("GetClosetPointByVertices No SamplePosition ");
        }

        return r;
    }

    public static int downcastmask = LayerMask.GetMask(Layers.Floor, Layers.Wall, Layers.Railing);

    public static Vector3 GetDownCast(Vector3 r,string name,NavMeshAgent agentT)
    {
        var p1 = r + new Vector3(0, 0.1f, 0);
        var direction = new Vector3(0, -1, 0);
        var p2 = p1 + direction * 0.3f;

        if (IsDebug && agentT)
        {
            CreatePoint(p1, name + "_CastStart", 0.1f, Color.black, agentT);
            CreatePoint(p2, name + "_CastEnd", 0.1f, Color.black, agentT);
        }

        if (IsDebug)
        {
            RaycastHit hitInfo2;
            if (Physics.Raycast(p1, direction, out hitInfo2, 0.3f, downcastmask))
            {
                string layer = LayerMask.LayerToName(hitInfo2.transform.gameObject.layer);
                if (agentT)
                {
                    CreatePoint(hitInfo2.point, name + "_CastHit_" + layer, 0.1f, Color.black, agentT);
                }
                return hitInfo2.point;
            }
            else
            {
                return r;
            }
        }
        else
        {
            return Vector3.zero;
        }
    }

    public static string GetLayer(Vector3 r)
    {
        try
        {
            var p1 = r + new Vector3(0, 0.1f, 0);
            var direction = new Vector3(0, -1, 0);
            //var p2 = p1 + direction * 0.3f;

            RaycastHit hitInfo;
            if (Physics.Raycast(p1, direction, out hitInfo, 0.3f, downcastmask))
            {
                string layer = LayerMask.LayerToName(hitInfo.transform.gameObject.layer);
                return layer;
            }
            else
            {
                return "";
            }
        }
        catch (Exception ex)
        {
            Log.Error("NavMeshHelper.GetLayer", ex);
            return "";
        }

    }

    public static Vector3 GetSamplePositionEx(Vector3 r)
    {
        NavMeshHit hitInfo;
        if (NavMesh.SamplePosition(r, out hitInfo, 100, -1))
        {
            r = hitInfo.position;
        }
        else
        {
            //Debug.LogError("GetClosetPointByVertices No SamplePosition ");
        }

        return r;
    }

    //public static Vector3 GetDownPosition(Vector3 r)
    //{
    //    NavMeshHit hitInfo;
    //    Vector3 r1 = new Vector3(r.x, r.y - 100, r.z);
    //    CreatePoint(r1, "DownTarget", 0.1f, Color.red);
    //    if (NavMesh.Raycast(r, r1,out hitInfo, -1))
    //    {
    //        r = hitInfo.position;
    //    }
    //    else
    //    {
    //        //Debug.LogError("GetDownPosition No DownPosition ");
    //    }
    //    return r;
    //}

    public static Vector3 GetDownSample(Vector3 r, int count, float step, NavMeshAgent agentT)
    {
        Vector3 down = r;
        float distance = float.MaxValue;
        float floorHeight = 0;
        for (int i = 0; i < count; i++)
        {
            Vector3 r1 = new Vector3(r.x, r.y - i * step, r.z);
            var p = GetClosetPointByMeshEx(r1);

            if (IsDebug)
            {
                var t1 = NavMeshHelper.CreatePoint(r1, "DownTarget" + i, 0.1f, Color.red, agentT);
                var t2 = NavMeshHelper.CreatePoint(p, "DownTarget_Mesh" + i, 0.1f, Color.red, agentT);
                if (t1 != null && t2 != null)
                {
                    t2.transform.parent = t1.transform;
                }
            }

            if (p.y < r.y)
            {
                if (floorHeight == 0)
                {
                    floorHeight = p.y;
                }
                var dis = Vector3.Distance(p, new Vector3(r1.x,floorHeight,r1.z));
                if (dis < distance)
                {
                    distance = dis;
                    down = p;
                }
            }
        }
        return down;
    }

    public static Vector3 GetDownMesh(Vector3 r,int count,float step)
    {
        //count->10,step->0.2
        Vector3 down = r;
        float distance = float.MaxValue;
        float floorHeight = 0;
        for (int i = 1; i <= count; i++)
        {
            Vector3 r1 = new Vector3(r.x, r.y - i * step, r.z);

            var p = GetClosetPointByMesh(r1);

            //if (IsDebug)
            //{
            //    var t1 = NavMeshHelper.CreatePoint(r1, "DownTarget" + i, 0.1f, Color.red);
            //    var t2 = NavMeshHelper.CreatePoint(p, "DownTarget_Mesh" + i, 0.1f, Color.red);
            //    t2.transform.parent = t1.transform;
            //}

                if (p.y < r.y)
            {
                if (floorHeight == 0)
                {
                    floorHeight = p.y;
                }
                var dis = Vector3.Distance(p, new Vector3(r1.x, floorHeight, r1.z));
                if (dis < distance)
                {
                    distance = dis;
                    down = p;
                }
            }
        }
        return down;
    }

    //public static Vector3 GetUpPosition(Vector3 r)
    //{
    //    NavMeshHit hitInfo;
    //    Vector3 r1 = new Vector3(r.x, r.y + 100, r.z);
    //    if (NavMesh.Raycast(r, r1, out hitInfo, -1))
    //    {
    //        r = hitInfo.position;
    //    }
    //    else
    //    {
    //        //Debug.LogError("GetUpPosition No UpPosition ");
    //    }
    //    return r;
    //}
    public static Segment3 segmentT;
    private static Vector3 GetClosetPointBySegment1(Vector3 target)
    {
        var distance = float.MaxValue;
        DateTime start = DateTime.Now;

        Vector3 r = target;
        InitNavmeshInfo();
        if (LocationManager.Instance && LocationManager.Instance.UseNavmeshTCaculate)
        {
            r = navMeshInfoT.GetClosedPointBySegment(target);
        }
        else
        {
            for (int i = 0; i < navMeshInfo.segments.Count; i++)
            {
                Segment3 segment = navMeshInfo.segments[i];
                Vector3 point = target;
                Vector3 closestPoint;
                float dist0 = Distance.Point3Segment3(ref point, ref segment, out closestPoint);
                if (dist0 < distance)
                {
                    distance = dist0;
                    r = closestPoint;
                    segmentT = segment;
                }
            }         
        }
        TimeSpan time = DateTime.Now - start;
        LogText += (string.Format(
            "GetClosetPointBySegment,indices count:{0},distance:{1},closestPoint{2},time:{3}ms ",
            navMeshInfo.indicesCount,
            distance, r, time.TotalMilliseconds));
        return r;
    }

    //private static Vector3[] vertices;
    private static NavMeshInfo navMeshInfo = null;

    //public static string LogText = "";

    public static void ShowTriangles()
    {
        for (int i = 0; i < navMeshInfo.triangle3s.Count; i++)
        {
            Triangle3 triangle3 = navMeshInfo.triangle3s[i];
            Plane3 plane = navMeshInfo.planes[i];

            //Vector3 point = target;
            //Vector3 closestPoint;
            //float dist0 = Distance.Point3Plane3(ref point, ref plane, out closestPoint);

            //Line3 line = new Line3(point, target);
            //Triangle3 triangle = triangle3;
            //Line3Triangle3Intr info;
            //bool find = Intersection.FindLine3Triangle3(ref line, ref triangle, out info);

            //if (find) //只有点在三角形内才有效
            //{
            //    //Debug.Log(string.Format("[{0}]{1} {2} {3}", i, dist0, closestPoint, find));

            //    if (dist0 < distance)
            //    {
            //        distance = dist0;
            //        r = closestPoint;
            //    }
            //}
        }

        //TimeSpan time = DateTime.Now - start;
        //Log += (string.Format(
        //    "GetClosetPointByTriangle,indices count:{0},distance:{1},closestPoint{2},target:{3},time:{4}ms ",
        //    navMeshInfo.indicesCount, distance, r, target, time.TotalMilliseconds));
    }


    private static Vector3 GetClosetPointByTriangle1(Vector3 target)
    {
        //var triangleParent=CreatePoint(Vector3.zero,"TriangleParent",1,Color.black);
        DateTime start = DateTime.Now;
        Vector3 r = Vector3.zero;
        //if (vertices == null)
        {
            var distance = float.MaxValue;

            InitNavmeshInfo();
            if (LocationManager.Instance && LocationManager.Instance.UseNavmeshTCaculate)
            {
                r = navMeshInfoT.GetClosedTrianglesByPoint(target);
            }
            else
            {
                for (int i = 0; i < navMeshInfo.triangle3s.Count; i++)
                {
                    Triangle3 triangle3 = navMeshInfo.triangle3s[i];
                    Plane3 plane = navMeshInfo.planes[i];

                    Vector3 point = target;
                    Vector3 closestPoint;
                    float dist0 = Distance.Point3Plane3(ref point, ref plane, out closestPoint);

                    Line3 line = new Line3(target, closestPoint - target);
                    Triangle3 triangle = triangle3;
                    Line3Triangle3Intr info;
                    bool find = Intersection.FindLine3Triangle3(ref line, ref triangle, out info);

                    if (find) //只有点在三角形内才有效
                    {
                        //Debug.Log(string.Format("[{0}]{1} {2} {3}", i, dist0, closestPoint, find));

                        if (dist0 < distance)
                        {
                            distance = dist0;
                            r = closestPoint;

                            //var tri=ShowTriangle(triangleParent, i, triangle3.V0, triangle3.V1, triangle3.V2);

                            //var pt=CreatePoint(info.Point, i + "," + info.IntersectionType + "," + info.LineParameter + "," + info.TriBary0 + "," + info.TriBary1 + "," + info.TriBary2, 0.1f, Color.black);
                            //pt.transform.parent = tri.transform;

                            //var pt2 = CreatePoint(closestPoint, i+","+closestPoint+","+ dist0, 0.1f, Color.black);
                            //pt2.transform.parent = tri.transform;
                        }
                    }
                }                
            }
            TimeSpan time = DateTime.Now - start;
            LogText += (string.Format(
                "GetClosetPointByTriangle,indices count:{0},distance:{1},closestPoint{2},target:{3},time:{4}ms ",
                navMeshInfo.indicesCount, distance, r, target, time.TotalMilliseconds));
        }
        return r;
    }

    public static Vector3 GetClosetPointByMesh(Vector3 target)
    {
        Vector3 p1 = GetClosetPointBySegment1(target);
        //CreatePoint(p1, "BySegment", 0.1f, Color.black);
        Vector3 p2 = Vector3.zero;
        p2 = GetClosetPointByTriangle1(target);//YZL20210330:这个结果等于SamplePosition计算结果

        if (p2 != Vector3.zero)
        {
            // CreatePoint(p2, "ByTriangle", 0.1f, Color.black);
            Vector3 p3 = GetMinDistancePoint(target, p1, p2);
            return p3;
        }
        else
        {
            //Debug.LogError("No ByTriangle");
            return p1;
        }
        
    }

    //public static Vector3 GetClosetPointByMeshY(Vector3 target)
    //{
    //    Vector3 p1 = GetClosetPointBySegment1(target);
    //    //CreatePoint(p1, "BySegment", 0.1f, Color.black);
    //    Vector3 p2 = Vector3.zero;
    //    p2 = GetClosetPointByTriangle1(target);//YZL20210330:这个结果等于SamplePosition计算结果

    //    if (p2 != Vector3.zero)
    //    {
    //        // CreatePoint(p2, "ByTriangle", 0.1f, Color.black);
    //        Vector3 p3 = GetMinDistancePoint(target, p1, p2);
    //        return p3;
    //    }
    //    else
    //    {
    //        //Debug.LogError("No ByTriangle");
    //        return p1;
    //    }

    //}

    private static Vector3 GetClosetPointByMeshEx(Vector3 target)
    {
        Vector3 p1 = GetClosetPointByMesh(target);
        p1 = GetSamplePosition(p1);
        return p1;
    }

    //public static Vector3 GetClosetPointByVertices(Vector3 target, int sampleCount)
    //{
    //    if (vertices == null)
    //    {
    //        var triangles = NavMesh.CalculateTriangulation();
    //        vertices = triangles.vertices;//顶点
    //    }
    //    var i1 = GetClosedPointIndex(vertices, target);

    //    List<Vector3> psList = new List<Vector3>();
    //    if (i1 < vertices.Length - 1)
    //    {
    //        var ps1 = GetPointsOfLine(vertices[i1], vertices[i1 + 1], sampleCount);
    //        psList.AddRange(ps1);
    //    }

    //    if (i1 > 0)
    //    {
    //        var ps2 = GetPointsOfLine(vertices[i1], vertices[i1 - 1], sampleCount);
    //        psList.AddRange(ps2);
    //    }

    //    Vector3 r = GetClosedPoint(psList.ToArray(), target);
    //    r = GetSamplePosition(r);
    //    return r;
    //}

    //public static List<Vector3> GetPointsOfLine(Vector3 start, Vector3 end, int count)
    //{
    //    List<Vector3> list = new List<Vector3>();
    //    var distance = Vector3.Distance(end, start);
    //    var interval = distance / count;
    //    Debug.Log("count:" + count);
    //    Debug.Log("distance:" + distance);
    //    Debug.Log("interval:" + interval);
    //    var direction = (end - start).normalized;
    //    for (int i = 0; i < count; i++)
    //    {
    //        var p = start + direction * interval * i;
    //        list.Add(p);
    //    }
    //    return list;
    //}

    public static int ScalePower = 1;

    public static bool IsDebug = false;

    public static string currentTestTagName;//当前创建测试小球的人员标签

    public static GameObject CreatePoint(Vector3 v, string name, float scale, Color color)
    {
        // if (IsDebug)
        // {
        //     if (!currentName.Contains(PathFindingManager.Instance.TestTagName)) return null;//(Nav)
        //     if (!currentName.Contains("(Nav)")) return null;

        //     if (string.IsNullOrEmpty(currentTestTagName) && string.IsNullOrEmpty(PathFindingManager.Instance.TestTagName))
        //     {
        //         ClearTestPoints();
        //         return null;
        //     }

        //     if (currentTestTagName != PathFindingManager.Instance.TestTagName)
        //     {
        //         currentTestTagName = PathFindingManager.Instance.TestTagName;
        //         ClearTestPoints();
        //     }

        //     name = currentName + name;
        //     GameObject sphere = null;
        //     if (navPoints != null && navPoints.ContainsKey(name))
        //     {
        //         sphere = navPoints[name];
        //         sphere.transform.position = v;
        //     }
        //     else
        //     {
        //         sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //         sphere.transform.localScale = new Vector3(scale * ScalePower, scale * ScalePower, scale * ScalePower);
        //         sphere.transform.position = v;
        //         sphere.name = name;
        //         sphere.GetComponent<Renderer>().material.color = color;
        //         sphere.SetTransparent(0.5f);
        //         Collider col = sphere.GetComponent<Collider>();
        //         if (col) col.enabled = false;
        //         ////Name存在，会导致异常（不停的创建ByTriangle）
        //         //if(navPoints!=null&&navPoints.ContainsKey(name))
        //         //{
        //         //    GameObject.DestroyImmediate(navPoints[name]);
        //         //    navPoints.Remove(name);
        //         //}
        //         navPoints.Add(name, sphere);
        //     }

        //     return sphere;
        // }
        // else
        {
            return null;
        }
    }

    public static NavMeshAgent currentNavMeshAgentT;
    /// <summary>
    /// 创建定位测试小球，（新）YZL_2020.11.17
    /// </summary>
    /// <param name="v"></param>
    /// <param name="name"></param>
    /// <param name="scale"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static GameObject CreatePoint(Vector3 v, string name, float scale, Color color,NavMeshAgent navMeshAgentT)
    {
        //return null;
        if (IsDebug && navMeshAgentT)
        {
            // if (string.IsNullOrEmpty(PathFindingManager.Instance.TestTagName))
            // {
            //     ClearTestPoints();
            //     return null;
            // }
            // if (navMeshAgentT == null) return null;//(Nav)
            // if (!navMeshAgentT.name.Contains(PathFindingManager.Instance.TestTagName)) return null;//(Nav)
            // if (!navMeshAgentT.name.Contains("(Nav)")) return null;

            if (currentNavMeshAgentT != navMeshAgentT)
            {
                ClearTestPoints();
                currentNavMeshAgentT = navMeshAgentT;
            }

            name = currentName + name;
            GameObject sphere = null;
            if (navPoints != null && navPoints.ContainsKey(name))
            {
                sphere = navPoints[name];
                sphere.transform.position = v;
            }
            else
            {
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.localScale = new Vector3(scale * ScalePower, scale * ScalePower, scale * ScalePower);
                sphere.transform.position = v;
                sphere.name = name;
                sphere.GetComponent<Renderer>().material.color = color;
                //sphere.SetTransparent(0.5f);
                Collider col = sphere.GetComponent<Collider>();
                if (col) col.enabled = false;
                ////Name存在，会导致异常（不停的创建ByTriangle）
                //if(navPoints!=null&&navPoints.ContainsKey(name))
                //{
                //    GameObject.DestroyImmediate(navPoints[name]);
                //    navPoints.Remove(name);
                //}
                navPoints.Add(name, sphere);
            }

            return sphere;
        }
        else
        {
            return null;
        }
    }



    public static NavPoint GetMinDistancePointEx(NavPoint[] ps, Vector3 p, NavMeshAgent agent)
    {
        Debug.Log(string.Format("GetClosedPoint,{0},{1}", p, agent));
        NavPoint r = null;
        float dis = float.MaxValue;
        foreach (NavPoint i in ps)
        {
            float d = 0;
            if (agent != null)
            {
                d = CalCulatePathLength(i, agent);//计算出来的候选点到人的路线的距离他
            }
            else
            {
                d = Vector3.Distance(i.Position, p);
            }
            if (d < dis)
            {
                dis = d;
                r = i;
            }
        }
        Debug.Log(string.Format("ClosedPoint={0}", r));
        return r;
    }

    public static Vector3 GetMinDistancePoint(Vector3 p, params Vector3[] ps)
    {
        Vector3 r = Vector3.zero;
        float dis = float.MaxValue;
        foreach (var i in ps)
        {
            var d = Vector3.Distance(i, p);
            if (d < dis)
            {
                dis = d;
                r = i;
            }
        }
        return r;
    }

    public static Vector3 GetClosedPoint(Vector3[] ps, Vector3 p,NavMeshAgent agent)
    {
        Debug.Log(string.Format("GetClosedPoint,{0},{1}",p,agent));
        Vector3 r = Vector3.zero;
        float dis = float.MaxValue;
        foreach (Vector3 i in ps)
        {
            float d = 0;
            if (agent != null)
            {
                d = CalCulatePathLength(i, agent);//计算出来的候选点到人的路线的距离他
            }
            else
            {
                d = Vector3.Distance(i, p);
            }
            if (d < dis)
            {
                dis = d;
                r = i;
            }
        }
        return r;
    }

    public static float CalCulatePathLength(NavPoint source, NavMeshAgent agent)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        agent.CalculatePath(source.Position, navMeshPath);

        //foreach (var item in navMeshPath.corners)
        //{
        //    GameObject tp = NavMeshHelper.CreatePoint(item, "" + item, 0.1f, Color.black);
        //    testPoints.Add(tp);
        //}

        float sum = GetPathDistance(navMeshPath);
        Debug.Log(string.Format("{0},{1},{2},{3},{4}", source.Name, navMeshPath.status, navMeshPath.corners.Length, source.Position, sum));
        return sum;
    }

    public static float CalCulatePathLength(Vector3 source, NavMeshAgent agent)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        agent.CalculatePath(source, navMeshPath);
       
        //foreach (var item in navMeshPath.corners)
        //{
        //    GameObject tp = NavMeshHelper.CreatePoint(item, "" + item, 0.1f, Color.black);
        //    testPoints.Add(tp);
        //}

        float sum = GetPathDistance(navMeshPath);
        Debug.Log(string.Format("{0},{1},{2},{3}", navMeshPath.status, navMeshPath.corners.Length,source,sum));
        return sum;
    }

    public static float CalCulatePathLength(Vector3 source, Vector3 target)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        NavMesh.CalculatePath(source, target, -1, navMeshPath);
        Debug.Log(string.Format("{0},{1}", navMeshPath.status, navMeshPath.corners.Length));
        //foreach (var item in navMeshPath.corners)
        //{
        //    GameObject tp = NavMeshHelper.CreatePoint(item, "" + item, 0.1f, Color.black);
        //    testPoints.Add(tp);
        //}
        float sum = 0;
        for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
        {
            var p1 = navMeshPath.corners[i];
            var p2 = navMeshPath.corners[i + 1];
            var dis = Vector3.Distance(p1, p2);
            sum += dis;
        }
        return 0;
    }

    ////函数返回声音传播路径长度
    //public static float CalCulatePathLength(NavMeshAgent nav,Vector3 targetPosition)
    //{
    //    NavMeshPath path = new NavMeshPath();//是一个引用类，所以可以分配到其他函数中
    //    if (nav.enabled)
    //        nav.CalculatePath(targetPosition, path);//计算路径
    //    //NavMeshPath类中有一个" Vector3"的数组，叫"Corners"，数组存储所有路径的拐角点
    //    //我们把敌人和玩家位置添加到数组中，那么这个路径就是完整的了，所以数组长度+2
    //    Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];

    //    //数组第一个和最后一个值分别是敌人的位置和玩家的位置
    //    allWayPoints[0] = transform.position;
    //    allWayPoints[allWayPoints.Length - 1] = targetPosition;

    //    //中间几个值等于"corners"数组的值，通过for循环来进行赋值
    //    for (int i = 0; i < path.corners.Length; i++)
    //    {
    //        allWayPoints[i + 1] = path.corners[i];
    //    }

    //    //路径长度初始值设为0，如果不进行设置，那么变量将无法增加
    //    float pathLength = 0f;

    //    //循环迭代计算路径长度，循环次数等于路径点个数-1(五个点相连组成四条线...)
    //    for (int i = 0; i < allWayPoints.Length - 1; i++)
    //    {
    //        pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
    //    }

    //    return pathLength;
    //}

    public static int GetClosedPointIndex(Vector3[] ps, Vector3 p)
    {
        int r = 0;
        float dis = float.MaxValue;
        for (int i1 = 0; i1 < ps.Length; i1++)
        {
            Vector3 i = ps[i1];
            var d = Vector3.Distance(i, p);
            if (d < dis)
            {
                dis = d;
                r = i1;
            }
        }
        return r;
    }
}

public class NavPoint
{
    public string Name;

    public Vector3 Position;

    public Color Color;

    public float distanceOffset = 0;

    public string Layer;

    public NavPoint(string n, Vector3 p,Vector3 target, Color color, NavMeshAgent agent)
    {
        Name = n;
        Position = p;
        Color = color;

        if (NavMeshHelper.IsDebug)
        {
            var t01 = NavMeshHelper.CreatePoint(p, n, 0.1f, color, agent);
            if (t01 != null)
            {
                NavTestInfo info = t01.AddComponent<NavTestInfo>();
                info.agent = agent;
                info.Target = target;
            }
        }

        Layer = NavMeshHelper.GetLayer(p);
    }

    public override string ToString()
    {
        return "[" + Name + "," + Position + "]";
    }
}

public class NavPointList
{
    public NavPoint Sample;
    public NavPoint Edge;
    public NavPoint Mesh;
    public NavPoint Down;

    public NavPoint Closed;

    public static float floorOffsetY = 0.1f;

    public static float meshDisOff = 0.25f;

    internal NavPoint GetMinDistancePoint(Vector3 target, NavMeshAgent agent)
    {
        //Debug.Log(string.Format("GetMinDistancePoint,{0},{1}", target, agent));
        //Debug.Log(string.Format("Sample:{0},Mesh:{1},Down:{2},Edge:{3}", Sample, Mesh, Down, Edge));
        List<NavPoint> ps = new List<NavPoint>() { Sample, Edge, Mesh, Down };
        //ps.Add(Sample);
        NavPoint r = null;
        if (Sample == null)
        {
            Debug.Log(string.Format("Route01"));
            r = GetMinDistancePoint(ps, target);
            return r;
        }

        if (Mesh != null)
        {
            Mesh.distanceOffset = meshDisOff;
        }


        foreach (var item in ps)
        {
            if (item == null) continue;
            //Log.Error("GetMinDistancePoint", string.Format("name:{0},layer:{1},pos:{2},target:{3}", item.Name, item.Layer, item.Position, target));
            if (item.Layer == "Railing")
            {
                return item;//在楼梯上时以楼梯为优先，宝信1楼到2楼楼梯 3楼到4楼楼梯
            }
        }

        //yzl20201108注释：获取NavPointList四个点Sample、Edge、Mesh、Down中最近一个点比较好，以下注释部分会导致人员从高层直接瞬移到下层
        //if (Sample.Position.y > target.y || Mesh.Position.y > target.y)//避免从1楼飘到2楼
        //{
        //    if(Sample.Position.y > target.y + floorOffsetY)
        //    {
        //        ps.Remove(Sample);
        //    }

        //    if (Mesh.Position.y > target.y + floorOffsetY)
        //    {
        //        ps.Remove(Mesh);
        //    }
        //    //if (Sample.Position.y > target.y+ floorOffsetY && Mesh.Position.y> target.y + floorOffsetY)//避免上到二楼就不下一楼了
        //    //{
        //    //    Debug.Log(string.Format("Route1"));
        //    //    return Down;
        //    //}
        //    //else
        //    //{
        //    Debug.Log(string.Format("Route2"));
        //        float dis = float.MaxValue;
        //        foreach (NavPoint i in ps)
        //        {
        //            if (i == null) continue;
        //            if (i.Position.y > target.y + floorOffsetY) continue;//不能比目标点高，不然跑到上一层去了
        //            float d = 0;
        //            //if (agent != null)
        //            //{
        //            //    d = NavMeshHelper.CalCulatePathLength(i, agent);//计算出来的候选点到人的路线的距离他
        //            //}
        //            //else
        //            {
        //                d = Vector3.Distance(i.Position, target) + i.distanceOffset;
        //            }
        //            if (d < dis)
        //            {
        //                dis = d;
        //                r = i;
        //            }
        //        }
        //    if (r == null)
        //    {
        //        r = GetMinDistancePoint(ps, target);
        //    }
        //    //}
        //}
        //else
        //{
        //    Debug.Log(string.Format("Route3"));
        //r = GetMinDistancePoint(ps, target);
        //}
        //Debug.Log(string.Format("ClosedPoint={0}", r));

        //yzl20201108注释：获取NavPointList四个点Sample、Edge、Mesh、Down中最近一个点比较好
        r = GetMinDistancePoint(ps, target);
        return r;
    }

    private NavPoint GetMinDistancePoint(List<NavPoint> ps, Vector3 target)
    {
        NavPoint r = null;
        float dis = float.MaxValue;
        foreach (NavPoint i in ps)
        {
            if (i == null) continue;
            float d = Vector3.Distance(i.Position, target)+i.distanceOffset;
            if (d < dis)
            {
                dis = d;
                r = i;
            }
        }
        return r;
    }

    public override string ToString()
    {
        return string.Format("Sample:{0},Mesh:{1},Down:{2},Edge:{3}", Sample, Mesh, Down, Edge);
    }
}
