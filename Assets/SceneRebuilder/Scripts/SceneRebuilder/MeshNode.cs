using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshNode : MonoBehaviour,IComparable<MeshNode>
{
    public string MeshTypeName = "";

    public List<MeshType> TypesList = new List<MeshType>();

    public MeshData meshData;

    //public List<IJ> allIds;

    public List<MinMaxId> GetAllMinMaxIds()
    {
        var allIds = meshData.GetAllMinMaxIds();
        return allIds;
    }

    public TransformData transformData;

    public int GetVertexCount()
    {
        if (meshData != null)
        {
            return meshData.vertexCount;
        }
        return 0;
    }

    public string GetMeshKey1()
    {
        if (meshData != null)
        {
            if (meshData.GetInfo() != null)
            {
                return meshData.GetInfo().m_meshFeature;
            }
        }
        return MeshTypeName;
    }

    public string GetMeshKey2()
    {
        if (meshData != null)
        {
            if (meshData.GetInfo() != null)
            {
                return meshData.GetInfo().m_vertexFeature;
            }
        }
        return MeshTypeName;
    }

    public int VertexCount = 0;

    //public MeshInfo Info = new MeshInfo();

    public List<MeshNode> subMeshes = new List<MeshNode>();

    public MeshNode parentMesh = null;

    public MeshData totalMesh = null;

    private  void AddSubMeshInfo(MeshNode mn)
    {
        meshData.Add(mn.meshData);
        AddType(mn);
        if (parentMesh != null)
        {
            parentMesh.AddSubMeshInfo(mn);
        }
        VertexCount += mn.GetVertexCount();
    }

    // Start is called before the first frame update

    public bool initAllChildren = true;

    public void InitInfo()
    {
        if (gameObject.isStatic) return;
        if (meshData == null || meshData.vertexCount==0 || meshData._obj==null)
        {
            //GameObjectUtility.SetStaticEditorFlags.
            
            VertexCount=0;
            meshData = new MeshData(gameObject);
            meshData.IsWorld=this.IsWorld;

            //if (filter != null)
            //{
            //    Info.m_Mesh = filter.mesh;
            //    Info.Update();
            //}

            VertexCount += meshData.vertexCount;
        }

        //if (transformData == null)
        {
            transformData = new TransformData(transform);
        }

    }

    [ContextMenu("RefreshInfo")]
    public void RefreshInfo()
    {
        Debug.Log("RefreshInfo");
        meshData = null;
        transformData = null;
        InitInfo();
    }

    void Start()
    {
        Init();
    }

    public bool isInited = false;

    public void Init()
    {
        if (isInited == true) return;
        isInited = true;

        MeshTypeName = MeshType.GetTypeName(gameObject.name);

        if (gameObject.isStatic)
        {
            Debug.LogError("IsStatic:" + gameObject);
            return;
        }

        InitInfo();

        if (initAllChildren)
        {
            subMeshes.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                MeshNode subMesh = child.GetComponent<MeshNode>();
                if (subMesh == null)
                {
                    subMesh = child.AddComponent<MeshNode>();
                    subMesh.Init();
                }
                AddSubMesh(subMesh);
                subMesh.parentMesh = this;
                subMesh.AddInfoToParent();
            }
            subMeshes.Sort();
            TypesList.Sort();

            int allCount = 0;
            foreach (MeshType meshType in TypesList)
            {
                allCount += meshType.VertexCount;
            }

            if(allCount>0)
                foreach (MeshType meshType in TypesList)
                {
                    meshType.Percent = meshType.VertexCount * 100f / allCount;
                }
        }
    }

    public void AddInfoToParent()
    {
        if (parentMesh != null)
        {
            parentMesh.AddSubMeshInfo(this);
        }
    }

    public void AddSubMesh(MeshNode mn)
    {
        if (!subMeshes.Contains(mn))
        {
            subMeshes.Add(mn);
            AddType(mn);
        }
        
    }

    public void AddType(MeshNode mn)
    {
        MeshType meshType = TypesList.Find(i => i.TypeName == mn.MeshTypeName);
        if (meshType == null)
        {
            meshType = new MeshType();
            meshType.TypeName = mn.MeshTypeName;
            TypesList.Add(meshType);
        }
        meshType.AddItem(mn);
    }

    [ContextMenu("SortSubMeshes")]
    public void SortSubMeshes()
    {
        subMeshes.Sort();
    }

    internal bool IsSameMesh(MeshNode node)
    {
        return this.meshData.IsSameMesh(node.meshData);
    }

    public float pScale = 0.01f;

    public List<GameObject> vertextObjects = new List<GameObject>();

    [ContextMenu("TestGetVertexCenterInfo")]
    public void TestGetVertexCenterInfo()
    {
        GetVertexCenterInfo(true,true,centerOffset);
    }

    public float normalPlaneScale=100;

    public Vector3 GetCenterP()
    {
        return meshData.GetCenterP();
    }

    public Vector3 GetMaxP()
    {
        return meshData.GetMaxP();
    }

    public Vector3 GetMinP()
    {
        return meshData.GetMinP();
    }

    public Vector3 GetLongLine()
    {
        return meshData.GetLongLine();
    }

    public Vector3 GetShortLine()
    {
        return meshData.GetShortLine();
    }

    public Vector3 GetLongShortNormal()
    {
        return meshData.GetLongShortNormal();
    }

    public float GetLongShortAngle()
    {
        return meshData.GetLongShortAngle();
    }

    public Vector3 GetCenterPNew()
    {
        if(IsWorld) return CenterGo.transform.position;
        return this.GetCenterP();
    }

    public Vector3 GetMaxPNew()
    {
        if(IsWorld) return MaxPGo.transform.position;
        return this.GetMaxP();
    }

    public Vector3 GetMinPNew()
    {
        if(IsWorld) return MinPGo.transform.position;
        return this.GetMinP();
    }

    public Vector3 GetLongLineNew()
    {
        if(IsWorld) return LongLineGo.transform.forward;
        return this.GetLongLine();
    }

    public Vector3 GetShortLineNew()
    {
        if(IsWorld) return ShortLineGo.transform.forward;
        return this.GetShortLine();
    }

    public Vector3 GetLongShortNormalNew()
    {
        if(IsWorld) return NormalLineGo.transform.forward;
        return this.GetLongShortNormal();
    }

    //public float GetLongShortAngle()
    //{
    //    return meshData.GetLongShortAngle();
    //}

    public GameObject CenterGo;

    public GameObject MaxPGo;

    public GameObject MinPGo;

    public GameObject NormalPGo;

    public GameObject LongLineGo;

    public GameObject ShortLineGo;

    public GameObject NormalLineGo;

    private float PlaneScale = 0.1f;

    [ContextMenu("ShowNormalInfo")]
    private void ShowNormalInfo()
    {
        //旋转前法线比较
        Vector3 normal1 = this.GetLongShortNormal();
        Debug.Log($"[ShowNormalInfo]normal1:({normal1.x},{normal1.y},{normal1.z}) |" );

        //旋转后法线比较
        Vector3 normal12 = this.GetLongShortNormalNew();
        Debug.Log($"[ShowNormalInfo] normal12:({normal12.x},{normal12.y},{normal12.z}) |" );

    }

    [ContextMenu("CreateTempGo")]
    public GameObject CreateTempGo(string log)
    {
        GameObject tempCenter=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //tempCenter.transform.SetParent(node2.transform.parent);
        tempCenter.transform.up= this.GetLongShortNormalNew();
        tempCenter.transform.localScale=new Vector3(0.03f,0.03f,0.03f);
        tempCenter.name = this.name + "_TempCenter_"+log;

        tempCenter.transform.position=this.GetCenterP();
        //tempCenter.transform.position=CenterGo.transform.position;

        if(CenterGo){
            GameObject centerP2=MeshHelper.CopyGO(CenterGo);
            centerP2.transform.SetParent(tempCenter.transform);
        }
        this.transform.SetParent(tempCenter.transform);

        return tempCenter;
    }

    public float LongLineDistance = 0;

    public float ShortLineDistance = 0;

    public float LongShortRate = 0;

    [ContextMenu("CreateCenterGO")]
    private Vector3 CreateCenterGO()
    {
        var center = GetCenterP();
        CenterGo = CreateWorldPoint(center, string.Format("[{0}]({1},{2},{3})", "Center", center.x, center.y, center.z));

        //CenterGo.transform.position.PrintVector3("CenterGo.transform.position");
        //center.PrintVector3("centerP");
        return center;
        //var center = GetCenterP();
        //center.PrintVector3("centerP");
        //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ////go.transform.SetParent(this.transform);
        //go.transform.localScale = new Vector3(pScale, pScale, pScale);
        //go.transform.position = center;
        //go.name = "CenterGO";
        // go.transform.position.PrintVector3("CenterGo.transform.position1");
        //go.transform.SetParent(this.transform);
        //vertextObjects.Add(go);
        //return center;
    }

    [ContextMenu("CreateCenterGO2")]
    private Vector3 CreateCenterGO2()
    {
        var center = GetCenterP();

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.localScale = new Vector3(pScale, pScale, pScale);
        go.transform.position = center;
        go.name = "CenterGO2";
        //go.transform.SetParent(this.transform);
        vertextObjects.Add(go);

        go.transform.position.PrintVector3("CenterGo.transform.position");
        center.PrintVector3("centerP");

        return center;
    }

    public GameObject normalPlane;

    [ContextMenu("CreateNormalPlane")]
    public void CreateNormalPlane(string log)
    {
        //Debug.LogError("CreateNormalPlane:"+log);
        if(normalPlane!=null)
        {
            normalPlane.SetActive(false);
            GameObject.DestroyImmediate(normalPlane);
        }
        normalPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        normalPlane.transform.localScale = new Vector3(PlaneScale, PlaneScale, PlaneScale);
        normalPlane.transform.position = GetCenterP();
        normalPlane.transform.up = this.GetLongShortNormal();
        //normalPlane.transform.right=this.GetShortLine();

        var angle=Vector3.Angle(normalPlane.transform.right,this.GetShortLine());
        normalPlane.transform.Rotate(Vector3.up,angle);

        normalPlane.name = "NormalPlane"+ log;
        normalPlane.transform.SetParent(this.transform);
        vertextObjects.Add(normalPlane);
    }

    [ContextMenu("CreateNormalLine")]
    public void CreateNormalLine(string log)
    {
        var center=GetCenterP();

        Vector3 minMaxNormal = this.GetLongShortNormal();
        
        //minMaxNormal.PrintVector3("minMaxNormal");

        //minMaxNormal.normalized.PrintVector3("minMaxNormal.normalized");

        //center.PrintVector3("center");

        Vector3 normalPoint = minMaxNormal + center;
        //normalPoint.PrintVector3("normalPoint");

        Vector3 minMaxNormal2 = normalPoint - center;
        //minMaxNormal2.PrintVector3("minMaxNormal2");

        if(NormalPGo!=null)
        {
            NormalPGo.SetActive(false);
            GameObject.DestroyImmediate(NormalPGo);
        }
        if(NormalLineGo!=null)
        {
            NormalLineGo.SetActive(false);
            GameObject.DestroyImmediate(NormalLineGo);
        }

        NormalPGo = CreateWorldPoint(normalPoint, string.Format("[{0}]({1},{2},{3})", "minMaxNormal1"+log, minMaxNormal.x, minMaxNormal.y, minMaxNormal.z));
        //NormalLineGo = CreateWorldLine(center, normalPoint, "minMaxNormal1:" + Vector3.Distance(center, normalPoint));
        NormalLineGo = CreateWorldLine(center, normalPoint, "minMaxNormal1"+log+"_" + Vector3.Distance(center, normalPoint));
        //CreateWorldLine(Vector3.zero, minMaxNormal, "minMaxNormal1:" + Vector3.Distance(Vector3.zero, minMaxNormal));
    }

    public void CreateNormalLineAndPlane(string log)
    {
        CreateNormalLine(log);
        CreateNormalPlane(log);
    }

    public void ShowDebugDetail(bool isClear = true)
    {
        if (isClear)
            ClearChildren();

        if (true)
        {
            //世界坐标
            var center =CreateCenterGO();


            var maxP = GetMaxP();
            MaxPGo = CreateWorldPoint(maxP, string.Format("[{0}]({1},{2},{3})", "maxP", maxP.x, maxP.y, maxP.z));

            var minP = GetMinP();
            MinPGo = CreateWorldPoint(minP, string.Format("[{0}]({1},{2},{3})", "minP", minP.x, minP.y, minP.z));

            // CreateLocalLine(meshData.center, meshData.maxP, "MaxDis:" + Vector3.Distance(meshData.center, meshData.maxP));
            // CreateLocalLine(meshData.center, meshData.minP, "MinDis:" + Vector3.Distance(meshData.center, meshData.minP));

            LongLineDistance = Vector3.Distance(center, maxP);
            LongLineGo = CreateWorldLine(center, maxP, "MaxDis:" + LongLineDistance);
            ShortLineDistance = Vector3.Distance(center, minP);
            ShortLineGo = CreateWorldLine(center, minP, "MinDis:" + ShortLineDistance);

            LongShortRate = LongLineDistance / ShortLineDistance;

            CreateNormalLineAndPlane("_Init");

            //ShowNormalInfo();
        }
        else
        {
            // // //本地坐标
            // var center=meshData.center;
            // GameObject go = CreateLocalPoint(center, string.Format("[{0}]({1},{2},{3})", "Center", center.x,center.y,center.z));

            // var maxP=meshData.maxP;
            // CreateLocalPoint(maxP, string.Format("[{0}]({1},{2},{3})", "maxP", maxP.x,maxP.y,maxP.z));

            // var minP=meshData.minP;
            // CreateLocalPoint(minP, string.Format("[{0}]({1},{2},{3})", "minP", minP.x,minP.y,minP.z));

            // CreateLocalLine(center, maxP, "MaxDis:" + Vector3.Distance(center, maxP));
            // CreateLocalLine(center, minP, "MinDis:" + Vector3.Distance(center, minP));

            // Vector3 shortLine=minP-center;
            // Debug.Log(string.Format("[{0}]({1},{2},{3})", "shortLine", shortLine.x,shortLine.y,shortLine.z));
            // Vector3 longLine=maxP-center;
            // Debug.Log(string.Format("[{0}]({1},{2},{3})", "longLine", longLine.x,longLine.y,longLine.z));

            // Vector3 minMaxNormal=Vector3.Cross(longLine,shortLine);
            // float angle1=Vector3.Angle(longLine,minMaxNormal);
            // float angle2=Vector3.Angle(shortLine,minMaxNormal);
            // Debug.Log($"GetLongShortNormal nor:({minMaxNormal.x},{minMaxNormal.y},{minMaxNormal.z}),angle1:{angle1},angle2:{angle2}");

            // CreateLocalPoint(minMaxNormal, string.Format("[{0}]({1},{2},{3})", "minMaxNormal2", minMaxNormal.x,minMaxNormal.y,minMaxNormal.z));
            // CreateLocalLine(center, minMaxNormal, "minMaxNormal2:" + Vector3.Distance(center, minMaxNormal));

            // GameObject normalPlane=GameObject.CreatePrimitive(PrimitiveType.Plane);
            // normalPlane.transform.position=GetCenterP();
            // normalPlane.transform.up=minMaxNormal;
            // normalPlane.name="NormalPlane";
            // normalPlane.transform.SetParent(this.transform);
        }
    }

    public bool IsWorld=false;

    public void GetVertexCenterInfo(bool showDebugDetails,bool isForce,Vector3 off)
    {
        this.centerOffset=off;
        
        if (meshData==null || meshData.vertexCount == 0||meshData._obj==null || isForce)
        {
            isInited = false;
            Init();
        }

        meshData.GetVertexCenterInfo(isForce,off,IsWorld);

        if(showDebugDetails)
        {
            ShowDebugDetail();
        }

        // if(showDebugDetails==false)
        // {
        //     foreach (var item in vertextObjects)
        //     {
        //         if(item==null)continue;
        //         item.SetActive(false);
        //     }
        // }
    }

    [ContextMenu("TestGetVertexCenterInfoEx")]
    public void TestGetVertexCenterInfoEx()
    {
        GetVertexCenterInfoEx(true,true);
    }

    public Vector3 centerOffset=Vector3.zero;

    public void GetVertexCenterInfoEx(bool showDebugDetails,bool isForce=false)
    {
        if (meshData==null || meshData.vertexCount == 0)
        {
            isInited = false;
            Init();
        }
        meshData.GetVertexCenterInfo(isForce,centerOffset,IsWorld);

        if(showDebugDetails)
        {
            ClearChildren();
            var center=meshData.GetCenterP();
            GameObject go = CreateWorldPoint(center, string.Format("[{0}]{1}", "Center", center));

            for(int i=0;i<meshData.maxPList.Count;i++)
            {
                var maxP=meshData.GetMaxP(i);
                CreateWorldPoint(maxP, string.Format("[{0}]({1},{2},{3})", "maxP", maxP.x,maxP.y,maxP.z));
                CreateWorldLine(center, maxP, "MaxDis:" + Vector3.Distance(center, maxP));
                // CreateLocalLine(meshData.center, maxP, "MaxDis:" + Vector3.Distance(meshData.center, maxP));
            }
            
            for(int i=0;i<meshData.minPList.Count;i++)
            {
                var minP=meshData.GetMinP(i);
                CreateWorldPoint(minP, string.Format("[{0}]({1},{2},{3})", "minP", minP.x,minP.y,minP.z));
                CreateWorldLine(center, minP, "MinDis:" + Vector3.Distance(center, minP));
                // CreateLocalLine(meshData.center, minP, "MinDis:" + Vector3.Distance(meshData.center, minP));
            }

            TransformNode tn = go.AddComponent<TransformNode>();
            tn.Init();
        }
    }

    [ContextMenu("ClearChildren")]
    public void ClearChildren()
    {
        MeshHelper.ClearChildren(this.transform);
    }

    private GameObject CreateLocalLine(Vector3 p1, Vector3 p2, string n)
    {
        p1 = transform.TransformPoint(p1);
        p2 = transform.TransformPoint(p2);

        return CreateWorldLine(p1,p2,n);
    }

    private GameObject CreateWorldLine(Vector3 p1, Vector3 p2, string n)
    {
        GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g1.transform.position = (p1 + p2) / 2;
        g1.transform.forward = p2 - p1;
        var dis=Vector3.Distance(p2, p1);
        if(dis<0.1f){
            dis=0.1f;
        }
        Vector3 scale = new Vector3(1f*pScale, 1f*pScale, dis);
        g1.transform.localScale = scale;
        g1.name = n;

         g1.transform.SetParent(this.transform);

        vertextObjects.Add(g1);
        return g1;
    }

    private GameObject CreateLocalPoint(Vector3 p,string n)
    {
        GameObject go = CreateWorldPoint(transform.TransformPoint(p),n);
        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // go.transform.localScale = new Vector3(pScale, pScale, pScale);
        // go.transform.position = transform.TransformPoint(p);
        // go.name = n;
        // go.transform.parent = this.transform;
        // vertextObjects.Add(go);
        return go;
    }

    private GameObject CreateWorldPoint(Vector3 p,string n)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.localScale = new Vector3(pScale, pScale, pScale);
        go.transform.position = p;
        go.name = n;
        go.transform.SetParent(this.transform);
        vertextObjects.Add(go);
        return go;
    }

    [ContextMenu("ShowVertexes")]
    public void ShowVertexes()
    {
        meshData.vertexCount=meshData.mesh.vertices.Length;
        if (meshData.vertexCount == 0)
        {
            isInited = false;
            Init();
        }
        
        Debug.Log("ShowVertexes:"+ meshData.vertexCount);
        ClearVertexes();
        var vs=meshData.mesh.vertices;

        var vs2=MeshHelper.GetWorldVertexes(vs,this.transform);
        var gos=MeshHelper.ShowVertexes(vs2,pScale,this.transform);
        vertextObjects.AddRange(gos);

        // var vcount=vs.Length;
        // for (int i = 0; i < vcount; i++)
        // {
        //     Vector3 p = vs[i];
        //     GameObject go = CreateLocalPoint(p, string.Format("[{0}]{1}", i, p));
        //     //go.transform.parent = this.transform;
        //     //go.transform.localScale = new Vector3(pScale, pScale, pScale);
        //     //go.transform.localPosition = p;

        //     //go.transform.parent = this.transform;
        //     //go.transform.localScale = new Vector3(pScale, pScale, pScale);
        //     //go.transform.position = transform.TransformPoint(p);
        //     go.name = string.Format("[{0}]{1}", i, go.transform.position);
        //     //go.transform.parent = this.transform;

        //     //vertextObjects.Add(go);

        //     TransformNode tn=go.AddComponent<TransformNode>();
        //     tn.Init();
        // }

        // string freatures=meshData.GetInfo().GetFeatures();
        // Debug.Log("freatures:"+ freatures);
    }

    [ContextMenu("ClearVertexes")]
    public void ClearVertexes()
    {
        foreach (var item in vertextObjects)
        {
            GameObject.DestroyImmediate(item);
        }
        vertextObjects.Clear();
    }

    // Update is called once per frame

    public int CompareTo(MeshNode other)
    {
        return other.GetVertexCount().CompareTo(this.GetVertexCount());
    }

    private void OnDestroy()
    {
        this.ClearChildren();
    }

    public static MeshNode CreateNew(GameObject go)
    {
        MeshNode node=go.GetComponent<MeshNode>();
        if(node!=null){
            node.ClearVertexes();
            GameObject.DestroyImmediate(node);
        }
        node=go.AddComponent<MeshNode>();
        return node;
    }
}
