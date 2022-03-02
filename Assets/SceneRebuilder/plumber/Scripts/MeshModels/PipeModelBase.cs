using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeModelBase : PipeModelComponent, IComparable<PipeModelBase>
{
    public virtual Vector3 GetStartPoint()
    {
        return Vector3.zero;
    }

    public virtual Vector3 GetEndPoint()
    {
        return Vector3.zero;
    }

    public virtual void CreateBoxLine()
    {
        TransformHelper.CreateBoxLine(GetStartPoint(), GetEndPoint(), PipeRadius * 2, this.name + "_BoxLine", this.transform.parent);
    }



    public void SetPrefabTransfrom(GameObject prefab)
    {
        var p1 = GetStartPoint();
        var p2 = GetEndPoint();
        var dir = p1 - p2;
        prefab.transform.position = (p1 + p2) / 2;
        prefab.transform.right = dir;
        prefab.transform.localScale = new Vector3(dir.magnitude, PipeRadius * 2, PipeRadius * 2);
    }

    public void SetPipeLineGeneratorArg(PipeMeshGenerator pipe, PipeGenerateArg arg, Vector4 startP, Vector4 endP)
    {
        pipe.points = new List<Vector3>() { startP, endP };
        arg.SetArg(pipe);
        var radius = (startP.w + endP.w) / 2;
        pipe.pipeRadius = radius;
        pipe.pipeRadius1 = radius;
        pipe.pipeRadius2 = radius;
        pipe.IsGenerateEndWeld = true;
        pipe.generateEndCaps = false;

        if (radius < 0.01)
        {
            //pipe.weldRadius = 0.003f;
            pipe.weldPipeRadius = arg.weldRadius * 0.6f;
        }
    }



    //void Awake()
    //{
    //    //Debug.Log($"PipeModelBase.Awake go:{this.name}");
    //}

    //void Start()
    //{
    //    Debug.Log($"PipeModelBase.Start go:{this.name}");
    //}

    public bool IsRendererOnStart = false;

    //void Start()
    //{
    //    if (IsRendererOnStart)
    //    {
    //        //RendererEachPipesEx();
    //        RendererModel();
    //    }
    //}

    public int sharedMinCount = 32;


    public static GameObject CreateSubTestObj(string objName, Transform parent)
    {
        return TransformHelper.CreateSubTestObj(objName, parent);
    }

    public void DebugShowKeyPoints()
    {
        //ClearChildren();
        ClearDebugInfoGos();
        GameObject trianglesObj = CreateSubTestObj($"KeyPoints", this.transform);

        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        MeshTriangles meshTriangles = new MeshTriangles(mesh);
        meshTriangles.ShowKeyPointsById(trianglesObj.transform, PointScale, sharedMinCount, minRepeatPointDistance,false);
        meshTriangles.Dispose();
    }



    public void DebugShowPointGroups()
    {
        ClearDebugInfoGos();
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        //MeshTriangles meshTriangles = new MeshTriangles(mesh);

        //meshTriangles.ShowPointGroups(this.transform, PointScale, 0, 3, minRepeatPointDistance);
        //meshTriangles.Dispose();

        //MeshHelper.GetPointGroups();
        
    }




   

    public virtual string GetDictKey()
    {
        return "";
    }

    public virtual string GetSortKey()
    {
        return "";
    }





    public List<PipeModelBase> ConnectedModels = new List<PipeModelBase>();

    public virtual void AddConnectedModel(PipeModelBase other)
    {
        if (!ConnectedModels.Contains(other))
        {
            ConnectedModels.Add(other);
        }
        if (ConnectedModels.Count > 2)
        {
            if (this.GetType() != typeof(PipeTeeModel))
            {
                Debug.LogError($"AddConnectedModel ConnectedModels.Count > 3 count:{ConnectedModels.Count} model:{this.name}");
            }
        }
    }

    public static string Vector4String(Vector4 v)
    {
        return $"({v.x.ToString("F3")},{v.y.ToString("F3")},{v.z.ToString("F3")},{v.w.ToString("F3")})";
    }

    public static string Vector3String(Vector3 v)
    {
        return $"({v.x.ToString("F3")},{v.y.ToString("F3")},{v.z.ToString("F3")})";
    }

    public virtual bool IsConnected(Vector3 keyPoint)
    {
        return false;
    }

    public virtual void SetUniformRadius(bool isUniformRaidus, Vector4 p2, int p1Id)
    {
        //if (isUniformRaidus)
        //{
        //    //p1.w = p2.w;//这个怎么保存回去呢？
        //    this.SetRadius(p1Id, p2.w);
        //    //this.PipeRadius = model2.PipeRadius;
        //    this.generateArg.generateEndCaps = false;
        //}
        //else
        //{
        //    this.generateArg.generateEndCaps = true;
        //}
    }

    public virtual int ConnectedModel(PipeModelBase model2, float minPointDis, bool isShowLog, bool isUniformRaidus, float minRadiusDis)
    {
        PipeModelBase model1 = this;
        int ConnectedCount = 0;
        var points1 = model1.GetModelKeyPoints();
        var points2 = model2.GetModelKeyPoints();

        if (isShowLog)
        {
            GameObject test = new GameObject("TestIsConnected");
            for (int i = 0; i < points1.Count; i++)
            {
                TransformHelper.ShowPoint(points1[i],0.001f,test.transform).name = $"Point1[{i}]_{points1[i]}";
            }
            for (int i = 0; i < points2.Count; i++)
            {
                TransformHelper.ShowPoint(points2[i], 0.001f, test.transform).name = $"Point2[{i}]_{points2[i]}";
            }
        }

        for (int i = 0; i < points1.Count; i++)
        {
            Vector4 p1 = points1[i];
            int cCount = 0;
            for (int i1 = 0; i1 < points2.Count; i1++)
            {
                Vector4 p2 = points2[i1];
                float dis12= Vector3.Distance(p1, p2);

                if (isShowLog)
                {
                    Debug.Log($"ConnectedModel model1:{model1.name} model2:{model2.name} [{i},{i1}]  dis:{dis12} p1:{p1} p2:{p2}");
                }

                if (dis12 < minPointDis)
                {
                    if (Mathf.Abs(p1.w - p2.w) > minRadiusDis)
                    {
                        if(model1.IsSpecial || model2.IsSpecial)
                        {
                            
                        }
                        else
                        {
                            Debug.Log($"ConnectedModel Radius IsNot Equal model1:{model1.name} model2:{model2.name} [{i},{i1}]  dis:{dis12} p1R:{p1.w} p2R:{p2.w}");
                        }
                    }
                    model1.AddConnectedModel(model2);
                    model2.AddConnectedModel(model1);
                    cCount++;
                    SetUniformRadius(isUniformRaidus, p2, i);

                }
            }
            if(cCount== points2.Count)
            {
                Debug.LogError($"ConnectedModel Error1 cCount== points2.Count model1:{model1.name} model2:{model2.name} p1:{p1} points2:{points2.Count} cCount:{cCount}");
                //return -1;
            }
            ConnectedCount += cCount;
        }

        //float dis11 = Vector3.Distance(model1.GetModelStartPoint(), model2.GetModelStartPoint());
        //float dis12 = Vector3.Distance(model1.GetModelStartPoint(), model2.GetModelEndPoint());
        //float dis21 = Vector3.Distance(model1.GetModelEndPoint(), model2.GetModelStartPoint());
        //float dis22 = Vector3.Distance(model1.GetModelEndPoint(), model2.GetModelEndPoint());
        //if (isShowLog)
        //{
        //    Debug.Log($"IsConnected model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
        //}

        return ConnectedCount;
    }

    public static int ConnectedModelEx(PipeElbowModel model1, PipeModelBase model2, float minPointDis, bool isUniformRaidus, float minRadiusDis = 0.0001f)
    {
        int ConnectedCount = 0;
        float dis11 = Vector3.Distance(model1.GetEndPointIn1(), model2.GetModelStartPoint());
        float dis12 = Vector3.Distance(model1.GetEndPointIn1(), model2.GetModelEndPoint());
        float dis21 = Vector3.Distance(model1.GetEndPointIn2(), model2.GetModelStartPoint());
        float dis22 = Vector3.Distance(model1.GetEndPointIn2(), model2.GetModelEndPoint());

        //Debug.LogError($"IsConnected model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");

        if (dis11 < minPointDis && dis12 < minPointDis)
        {
            //Error
            Debug.LogError($"IsConnected Error1 dis11 < minDis && dis12 < minDis dis11:{dis11} dis12:{dis12} minDis:{minPointDis}");
            return -1;
        }
        if (dis11 < minPointDis)
        {
            if (Mathf.Abs(model1.ModelStartPoint.w - model2.ModelStartPoint.w) > minRadiusDis)
            {
                Debug.LogWarning($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} radius1:{model1.ModelStartPoint.w} radius2:{model2.ModelStartPoint.w} ");
                if (isUniformRaidus)
                {
                    model1.ModelStartPoint.w = model2.ModelStartPoint.w;
                    model1.PipeRadius = model2.PipeRadius;
                }
                else
                {
                    model1.generateArg.generateEndCaps = true;
                }

            }
            model1.AddConnectedModel(model2);
            model2.AddConnectedModel(model1);


            ConnectedCount++;
        }
        if (dis12 < minPointDis)
        {
            if (Mathf.Abs(model1.ModelStartPoint.w - model2.ModelEndPoint.w) > minRadiusDis)
            {
                Debug.LogWarning($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} radius1:{model1.ModelStartPoint.w} radius2:{model2.ModelEndPoint.w} ");
                if (isUniformRaidus)
                {
                    model1.ModelStartPoint.w = model2.ModelEndPoint.w;
                    model1.PipeRadius = model2.PipeRadius;
                }
                else
                {
                    model1.generateArg.generateEndCaps = true;
                }
            }
            model1.AddConnectedModel(model2);
            model2.AddConnectedModel(model1);
            ConnectedCount++;
        }

        if (dis21 < minPointDis && dis22 < minPointDis)
        {
            //Error
            Debug.LogError($"IsConnected Error2 dis21 < minDis && dis22 < minDis dis21:{dis21} dis22:{dis22} minDis:{minPointDis}");
            return -1;
        }
        if (dis21 < minPointDis)
        {
            if (Mathf.Abs(model1.ModelEndPoint.w - model2.ModelStartPoint.w) > minRadiusDis)
            {
                Debug.LogWarning($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} radius1:{model1.ModelEndPoint.w} radius2:{model2.ModelStartPoint.w} ");
                if (isUniformRaidus)
                {
                    model1.ModelEndPoint.w = model2.ModelStartPoint.w;
                    model1.PipeRadius = model2.PipeRadius;
                }
                else
                {
                    model1.generateArg.generateEndCaps = true;
                }
            }
            model1.AddConnectedModel(model2);
            model2.AddConnectedModel(model1);
            ConnectedCount++;
        }
        if (dis22 < minPointDis)
        {
            if (Mathf.Abs(model1.ModelEndPoint.w - model2.ModelEndPoint.w) > minRadiusDis)
            {
                Debug.LogWarning($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} radius1:{model1.ModelEndPoint.w} radius2:{model2.ModelEndPoint.w} ");
                if (isUniformRaidus)
                {
                    model1.ModelEndPoint.w = model2.ModelEndPoint.w;
                    model1.PipeRadius = model2.PipeRadius;
                }
                else
                {
                    model1.generateArg.generateEndCaps = true;
                }
            }
            model1.AddConnectedModel(model2);
            model2.AddConnectedModel(model1);
            ConnectedCount++;
        }

        return ConnectedCount;
    }


    public Vector4 ModelStartPoint;

    public Vector4 TransformPoint(Vector4 p1)
    {
        Vector4 p = transform.TransformPoint(p1);
        p.w = p1.w;
        return p;
    }

    public Vector4 GetModelStartPoint()
    {
        Vector4 p= this.TransformPoint(ModelStartPoint);
        return p;
    }

    public Vector4 ModelEndPoint;

    public Vector4 GetModelEndPoint()
    {
        Vector4 p = this.TransformPoint(ModelEndPoint);
        return p;
    }

    public virtual List<Vector4> GetModelKeyPoints()
    {
        List<Vector4> list = new List<Vector4>();
        list.Add(GetModelStartPoint());
        list.Add(GetModelEndPoint());
        return list;
    }

    public virtual Vector3[] GetAlignPoints()
    {
        return null;
    }

    public List<Vector4> ModelKeyPoints = new List<Vector4>();


    public float PipeRadius = 0;

    public float PipeRadius1 = 0;
    public float PipeRadius2 = 0;


    public bool IsObbError = false;

    protected bool RendererErrorModel()
    {
        ClearGo();
        if (IsGetInfoSuccess == false)
        {
            Debug.LogWarning($"RendererErrorModel IsGetInfoSuccess == false gameObject:{this.name}");
            //this.gameObject.SetActive(true);
            //return true;
            return false;
        }
        return false;
    }

    public int VertexCount = 0;

    //public float ObbDistance = 0;

    //public float MeshDistance = 0;

    //public float SizeDistance = 0;

    //public float RTDistance = 0;



    public bool IsSpecial = false;

    //public bool IsModelDataInited = false;

    public void SetRadius()
    {
        //if (PipeRadius == 0)
        {
            PipeRadius1 = ModelStartPoint.w;
            PipeRadius2 = ModelStartPoint.w;
            PipeRadius = (PipeRadius1 + PipeRadius2) / 2;
        }
    }

    public static float GetRadiusValue(float v)
    {
        float p = 10000;
        int a = (int)(v * p);
        float b = a / p;
        return b;
    }


    public int KeyPointCount = 0;

    public virtual string GetPipeArgString()
    {
        if (PipeRadius == 0)
        {
            SetRadius();
        }
        return $"Radius:{PipeRadius}({PipeRadius1},{PipeRadius2}) Keys:{KeyPointCount} V:{VertexCount}";
    }

    public string GetCompareString()
    {
        string a = "1";
        if (IsGetInfoSuccess)
        {
            a = "0";
        }
        else
        {
            a = "1";
        }
        string radius = PipeRadius.ToString("F5");
        if (PipeRadius == 0)
        {
            radius = "9.99999";
        }
        //float dis = mcResult.SizeDistance + mcResult.ObbDistance + mcResult.MeshDistance;

        ////return $"{ObbDistance:F5}_{MeshDistance:F5}_{a}_{radius:00000}_{this.GetType().Name}_{this.VertexCount}";
        //return $"{mcResult.SizeDistance:F5}_[{dis:F5}]_{mcResult.ObbDistance:F5}_{mcResult.RTDistance:F5}_{mcResult.MeshDistance:F5}_{a}_{radius:00000}_{this.GetType().Name}_{this.VertexCount}";

        //return $"{ObbDistance:F5}_{MeshDistance:F5}_{a}_{radius:00000}_{this.GetType().Name}_{this.VertexCount}";
        return $"{mcResult}_{a}_{radius:00000}_{this.GetType().Name}_{this.VertexCount}";
    }

    public int CompareTo(PipeModelBase other)
    {
        return other.GetCompareString().CompareTo(this.GetCompareString());
        //if (this.IsGetInfoSuccess == false) return -1;
        //if (other.IsGetInfoSuccess == false) return 1;

        //if (other.PipeRadius == 0) return 1;
        //if (this.PipeRadius == 0) return -1;

        //int r = other.PipeRadius.ToString("F5").CompareTo(this.PipeRadius.ToString("F5"));
        //if (r == 0)
        //{
        //    r = this.GetType().Name.CompareTo(other.GetType().Name);
        //}
        //if (r == 0)
        //{
        //    r = other.VertexCount.CompareTo(this.VertexCount);
        //}
        //return r;
    }

    public override string ToString()
    {
        return $"T:[{this.GetType().Name}] [{mcResult}] R:{PipeRadius:F5}| V:{VertexCount:00000}|{IsGetInfoSuccess} ";
        //return GetCompareString();
    }

    //public virtual void GetModelInfo()
    //{

    //}


    //public override void RendererModel()
    //{
    //    MeshRenderer r = this.GetComponent<MeshRenderer>();
    //    if (generateArg.pipeMaterial == null)
    //    {
    //        generateArg.pipeMaterial = r.sharedMaterial;
    //    }
    //    if (generateArg.weldMaterial == null)
    //    {
    //        generateArg.weldMaterial = r.sharedMaterial;
    //    }
    //    RendererModel(generateArg, "_New");
    //}


    //public virtual GameObject RendererModel(PipeGenerateArg arg, string afterName)
    //{
    //    return null;
    //}

    private static int[] PipeSegmentsLOD4 = new int[] { 24, 12, 8, 6 };

    //private int[] PipeSegmentsLOD3 = new int[] { 36, 12, 6 };

    private static int[] ElbowSegmentsLOD4 = new int[] { 8, 6, 5, 4 };

    //private int[] ElbowSegmentsLOD3 = new int[] { 8, 4, 2 };

    //private float[] LODLevels_2 = new float[] { 0.5f, 0.2f, 0.001f };

    private static float[] LODLevels_3 = new float[] { 0.5f, 0.2f, 0.05f, 0.001f };

    public List<Transform> PipeWelds = new List<Transform>();

    public void SetModelLOD(Transform parent,int[] pipeSegmentsLODs, int[] elbowSegmentsLODs)
    {
        bool generateWeld = generateArg.generateWeld;
        List<Transform> welds = new List<Transform>();
        for (int i = 0; i < pipeSegmentsLODs.Length; i++)
        {
            if (i == 0)
            {
                generateArg.generateWeld = generateWeld;
            }
            else
            {
                generateArg.generateWeld = false;
            }
            generateArg.pipeSegments = pipeSegmentsLODs[i];
            generateArg.elbowSegments = elbowSegmentsLODs[i];
            RendererModel(generateArg, "_LOD"+ i);
            GameObject lod0 = ResultGo;
            lod0.transform.SetParent(parent);

            

            if (i == 0)
            {
                List<Transform> children = new List<Transform>();
                for (int j=0;j< lod0.transform.childCount;j++)
                {
                    children.Add(lod0.transform.GetChild(j));
                }
                foreach(var item in children)
                {
                    item.SetParent(null);
                }
                PipeWelds = new List<Transform>(children);


            }
            else
            {
                
            }

            ResultGo = null;
        }
        generateArg.generateWeld = generateWeld;

        //LODManager.Instance.LODLevels_2 = LODLevels_2;
        LODManager.Instance.LODLevels_3 = LODLevels_3;

        //generateArg.pipeSegments = PipeSegmentsLOD4[1];
        //RendererModel(generateArg, "_LOD1");
        //GameObject lod1 = ResultGo;
        //lod1.transform.SetParent(parent);
        //ResultGo = null;

        //generateArg.pipeSegments = PipeSegmentsLOD4[2];
        //RendererModel(generateArg, "_LOD2");
        //GameObject lod2 = ResultGo;
        //lod2.transform.SetParent(parent);
        //ResultGo = null;

        //generateArg.pipeSegments = PipeSegmentsLOD4[3];
        //RendererModel(generateArg, "_LOD3");
        //GameObject lod3 = ResultGo;
        //lod3.transform.SetParent(parent);
        //ResultGo = null;

        //LODManager.Instance.LODLevels_2 = LODLevels_2;
        //LODManager.Instance.LODLevels_3 = LODLevels_3;
    }

    //public void RendererModelLOD3(Transform parent)
    //{
    //    generateArg.pipeSegments = 36;
    //    RendererModel(generateArg, "_LOD0");
    //    GameObject lod0 = ResultGo;
    //    lod0.transform.SetParent(parent);
    //    ResultGo = null;

    //    generateArg.pipeSegments = 12;
    //    RendererModel(generateArg, "_LOD2");
    //    GameObject lod1 = ResultGo;
    //    lod1.transform.SetParent(parent);
    //    ResultGo = null;

    //    generateArg.pipeSegments = 6;
    //    RendererModel(generateArg, "_LOD3");
    //    GameObject lod2 = ResultGo;
    //    lod2.transform.SetParent(parent);
    //    ResultGo = null;

    //    generateArg.pipeSegments = 3;
    //    RendererModel(generateArg, "_LOD4");
    //    GameObject lod3 = ResultGo;
    //    lod3.transform.SetParent(parent);
    //    ResultGo = null;

    //    //LODManager.Instance.LODLevels_2 = LODLevels_2;
    //    LODManager.Instance.LODLevels_3 = LODLevels_3;
    //}

    public void RendererModelLOD(int lodCount)
    {
        MeshRenderer r = this.GetComponent<MeshRenderer>();
        if (generateArg.pipeMaterial == null)
        {
            generateArg.pipeMaterial = r.sharedMaterial;
        }
        if (generateArg.weldMaterial == null)
        {
            generateArg.weldMaterial = r.sharedMaterial;
        }

        GameObject lodGroup = new GameObject(this.name);
        lodGroup.transform.position = this.transform.position;
        lodGroup.transform.SetParent(this.transform.parent);

        //if (lodCount==4)
        {
            SetModelLOD(lodGroup.transform, PipeSegmentsLOD4, ElbowSegmentsLOD4);
        }
        //if (lodCount == 3)
        //{
        //    SetModelLOD(lodGroup.transform, PipeSegmentsLOD3, ElbowSegmentsLOD3);
        //}

        LODGroupInfo groupInfo=LODHelper.CreateLODs(lodGroup);
        ResultGo = lodGroup;

        foreach(var item in PipeWelds)
        {
            item.SetParent(lodGroup.transform);
        }

        groupInfo.AddRenderers(0, PipeWelds);

        //LODHelper.CreateLODs(ResultGo);

        //lodGroup.AddComponent<RendererId>();
    }

    public override void InitSaveData(MeshModelSaveData data)
    {
        base.InitSaveData(data);

        data.PipeWelds = GetWeldsSaveData();
    }


    public virtual void SetSaveData(MeshModelSaveData data)
    {
        //this.LineInfo = data.Info;
        //SetModelData(data.Data);
        Debug.LogError("SetSaveData Not Override:"+ data);
    }

    

    //public void GetSizeDistance()
    //{

    //}


    [ContextMenu("ClearChildren")]
    public void ClearChildren()
    {
        TransformHelper.ClearChildren(gameObject);
        ClearCheckDistance();
    }

    public void DestroyMeshComponent()
    {
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        if (renderer)
        {
            GameObject.DestroyImmediate(renderer);
        }

        MeshFilter filter = this.GetComponent<MeshFilter>();
        if (filter)
        {
            GameObject.DestroyImmediate(filter);
        }
    }

    public GameObject CreateKeyPointsGo()
    {
        return CreateDebugInfoRoot("KeyPoints");
    }

    //public bool IsNewGo = true;

    public bool IsNoMesh()
    {
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        MeshFilter mf = this.GetComponent<MeshFilter>();
        return renderer == null || mf == null || mf.sharedMesh == null;
    }

    public T GetGenerator<T>(PipeGenerateArg arg, string afterName,bool isNewGo) where T : PipeMeshGeneratorBase
    {
        GameObject pipeNew = null;

        if (isNewGo == true)
        {
            pipeNew = GetPipeNewGo(arg, afterName);
        }
        else
        {
            //MeshRenderer renderer = this.GetComponent<MeshRenderer>();
            //MeshFilter mf = this.GetComponent<MeshFilter>();
            if (IsNoMesh())
            {
                pipeNew = this.gameObject;
            }
            else
            {
                pipeNew = GetPipeNewGo(arg, afterName);
            }
        }

        T pipe = pipeNew.GetComponent<T>();
        if (pipe == null)
        {
            pipe = pipeNew.AddComponent<T>();
        }
        pipe.Target = this.gameObject;
        ResultGo = pipe.gameObject;
        ResultGo.SetActive(true);
        return pipe;
    }

    public GameObject GetPipeNewGo(PipeGenerateArg arg, string afterName)
    {
        GameObject pipeNew = new GameObject(this.name + afterName);

        //pipeNew.transform.position = this.transform.position + arg.Offset;
        ////pipeNew.transform.rotation = this.transform.rotation;

        pipeNew.transform.SetParent(this.transform.parent);

        pipeNew.transform.position = this.transform.position + arg.Offset;

        return pipeNew;
    }



    public GameObject RenderPipeLine(PipeGenerateArg arg, string afterName, Vector4 startP, Vector4 endP, bool isNewGo=true)
    {
        //arg = arg.Clone();
        //PipeMeshGenerator pipe = GetGenerator<PipeMeshGenerator>(arg, afterName);
        //pipe.generateElbows = false;
        //pipe.points = new List<Vector3>() { startP, endP };
        //pipe.pipeRadius = (startP.w + endP.w) / 2;
        //arg.SetArg(pipe);
        ////pipe.generateWeld = gWeld;
        ////pipe.IsGenerateEndWeld = true;
        //pipe.avoidStrangling = true;
        //pipe.RenderPipe();
        //return pipe.gameObject;

        //arg = arg.Clone();
        PipeMeshGeneratorEx pipe = GetGenerator<PipeMeshGeneratorEx>(arg, afterName, isNewGo);
        pipe.generateElbows = false;
        pipe.points = new List<Vector4>() { startP, endP };
        //pipe.pipeRadius = (startP.w + endP.w) / 2;
        arg.SetArg(pipe);
        //pipe.generateWeld = gWeld;
        //pipe.IsGenerateEndWeld = true;
        pipe.avoidStrangling = true;
        pipe.RenderPipe();
        return pipe.gameObject;
    }





    public List<string> GetPath()
    {
        var ts = gameObject.GetComponentsInParent<Transform>();
        List<string> path = new List<string>();
        for (int i = 0; i < ts.Length; i++)
        {
            path.Add(ts[i].name);
        }
        return path;
    }

    public GameObject CopyMeshComponentsEx(GameObject source)
    {
        //MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
        if (IsNoMesh())
        {
            MeshHelper.CopyMeshComponents(source, this.gameObject);
            this.gameObject.transform.rotation = source.transform.rotation;

            List<Transform> children = new List<Transform>();
            for (int i = 0; i < source.transform.childCount; i++)
            {
                children.Add(source.transform.GetChild(i));
            }
            foreach (var child in children)
            {
                child.SetParent(this.transform);
            }
            GameObject.DestroyImmediate(source);
            source = this.gameObject;
            source.SetActive(true);
        }
        return source;
    }


    internal void RemomveMesh()
    {
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        mf.sharedMesh = null;

        //GameObject.DestroyImmediate(mf);
        //MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        //GameObject.DestroyImmediate(mr);
    }
}


