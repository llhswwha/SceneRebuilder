using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class GetScanePoints : MonoBehaviour
{
    public int ScreenWidth = 1922;

    public int ScreenHeight = 1080;

    public int RecursiveCount = 1;

    public List<Vector2> points = new List<Vector2>();

    public int PointCount = 0;

    public List<RectInfo> rects = new List<RectInfo>();

    public List<List<RectInfo>> rectsList = new List<List<RectInfo>>();

    public Vector2 RectSize;

    // Start is called before the first frame update
    void Start()
    {
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;
        InitOne();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("GetRectSize")]
    public void GetRectSize()
    {
        if (rects.Count > 0)
        {
            RectSize = rects[0].GetRectSize();
        }
    }

    private RectInfo rect0;

    [ContextMenu("InitOne")]
    public void InitOne()
    {
        rects.Clear();
        rect0 = new RectInfo(ScreenWidth, ScreenHeight);
        rects.Add(rect0);

        //print(Screen.currentResolution);
        //print(Screen.safeArea);
    } 

    [ContextMenu("InitTwo")]
    public void InitTwo()
    {
        rects.Clear();
        var rect1 = new RectInfo(ScreenWidth/2, ScreenHeight);
        rects.Add(rect1);

        var  rect2 = new RectInfo(ScreenWidth / 2, 0, ScreenWidth, ScreenHeight);
        rects.Add(rect2);

        //print(Screen.currentResolution);
        //print(Screen.safeArea);
    }

    [ContextMenu("CreateRects")]
    public List<RectInfo> CreateRects()
    {
            List<RectInfo> rectsNew = new List<RectInfo>();
            for (int j = 0; j < rects.Count; j++)
            {
                RectInfo rect = rects[j];
                rectsNew.AddRange(rect.CreateSubRects());
            }
            rects = rectsNew;
        return rectsNew;
    }

    public List<Vector2> GetPoints()
    {
        List<Vector2> ps = new List<Vector2>();
        for (int j = 0; j < rects.Count; j++)
        {
            RectInfo rect = rects[j];
            for (int i = 0; i < rect.points.Count; i++)
            {
                Vector2 p = rect.points[i];
                if (!ps.Contains(p))
                {
                    ps.Add(p);
                }
            }
        }
        return ps;
    }

    public float PosScale = 0.01f;

    public List<GameObject> objs = new List<GameObject>();

    [ContextMenu("DestroyPoints")]
    private void DestroyPoints()
    {
        foreach (var obj in objs)
        {
            GameObject.DestroyImmediate(obj);
        }
        objs.Clear();
    }

    public GameObject SphereRoot = null;

    private void CreatePointsCore(List<Vector2> ps,Vector3 scale)
    {
        if (SphereRoot == null)
        {
            SphereRoot = new GameObject("SphereRoot");
        }
        if (CreateSphere == false) return;
        for (int i = 0; i < ps.Count; i++)
        {
            Vector2 p = ps[i];
            Vector3 pos = p;
            pos *= PosScale;
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = pos;
            obj.transform.SetParent(SphereRoot.transform);
            obj.name = i + "|" + p.ToString();
            obj.transform.localScale = scale;
            objs.Add(obj);
        }
    }

    [ContextMenu("CreatePoints")]
    public void CreatePoints()
    {
        DestroyPoints();
        List<Vector2> ps = GetPoints();
        points = ps;
        PointCount = points.Count;
        CreatePointsCore(ps, GetSphereScale());
    }

    public bool OnlyNew = false;

    public List<Vector2> GetPointsEx()
    {
        List<Vector2> psNew = new List<Vector2>();
        List<Vector2> ps = new List<Vector2>();
        for(int k=0;k< rectsList.Count; k++)
        {
            var list = rectsList[k];
            psNew = new List<Vector2>();
            for (int j = 0; j < list.Count; j++)
            {
                RectInfo rect = list[j];
                for (int i = 0; i < rect.points.Count; i++)
                {
                    Vector2 p = rect.points[i];
                    if (!ps.Contains(p))
                    {
                        ps.Add(p);
                        psNew.Add(p);
                    }
                }
            }
        }
        if (OnlyNew)
        {
            return psNew;
        }
        
        return ps;
    }

    public bool CreateSphere = false;

    [ContextMenu("CreatePointsEx1")]
    public void CreatePointsEx1()
    {
        var start = DateTime.Now;

        rectsList.Clear();
        InitOne();
        rectsList.Add(rects);
        for (int i = 0; i < RecursiveCount; i++)
        {
            CreateRects();
            rectsList.Add(rects);
        }

        DestroyPoints();
        List<Vector2> ps = GetPointsEx();
        points = ps;
        PointCount = points.Count;
        CreatePointsCore(ps, GetSphereScale());

        GetRectSize();

        var t = (DateTime.Now - start);
        Debug.Log("CreatePointsEx1 用时:" + t);
    }

    public Vector3 Scale;

    private Vector3 GetSphereScale()
    {
        var p = 1f;
        if (RecursiveCount == 3)
        {
            p = 0.5f;
        }
        if (RecursiveCount == 4)
        {
            p = 0.4f;
        }
        if (RecursiveCount == 5)
        {
            p = 0.2f;
        }
        if (RecursiveCount == 6)
        {
            p = 0.1f;
        }
        if (RecursiveCount == 7)
        {
            p = 0.05f;
        }
        if (RecursiveCount == 8)
        {
            p = 0.025f;
        }
        if (RecursiveCount == 9)
        {
            p = 0.01f;
        }
        if (RecursiveCount > 9)
        {
            p = 0.001f;
        }
        var scale = new Vector3(p,p,p);
        Scale = scale;
        return scale;
    }

    [ContextMenu("CreatePointsEx2")]
    public void CreatePointsEx2()
    {
        var start = DateTime.Now;

        rectsList.Clear();
        InitTwo();
        rectsList.Add(rects);
        
        for (int i = 0; i < RecursiveCount; i++)
        {
            CreateRects();
            rectsList.Add(rects);
        }

        DestroyPoints();
        List<Vector2> ps = GetPointsEx();
        points = ps;
        PointCount = points.Count;
        CreatePointsCore(ps, GetSphereScale());

        GetRectSize();

        var t = (DateTime.Now - start);
        Debug.Log("CreatePointsEx2 用时:" + t);
    }
}

[Serializable]
public class RectInfo
{
    public List<Vector2> points = new List<Vector2>();
    public RectInfo()
    {

    }
    public RectInfo(int w,int h)
    {
        points.Add(new Vector2(0, 0));
        points.Add(new Vector2(w, 0));
        points.Add(new Vector2(0, h));
        points.Add(new Vector2(w, h));
    }

    public RectInfo(int x1,int y1,int x2, int y2)
    {
        points.Add(new Vector2(x1, y1));
        points.Add(new Vector2(x1, y2));
        points.Add(new Vector2(x2, y1));
        points.Add(new Vector2(x2, y2));
    }

    public List<RectInfo> CreateSubRects()
    {
        List<RectInfo> list = new List<RectInfo>();
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 p1 = points[i];
            RectInfo rect = new RectInfo();
            
            for(int j = 0; j < points.Count; j++)
            {
                Vector2 p2 = points[j];
                Vector2 pN = (p1 + p2) / 2;
                rect.points.Add(pN);
            }
            list.Add(rect);
        }
        return list;
    }

    internal Vector2 GetRectSize()
    {
        var p1 = points[0];
        float sizeX = 0;
        float sizeY = 0;
        List<float> disList = new List<float>();
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] == p1) continue;
            if (points[i].x == p1.x)
            {
                sizeY = Math.Abs(p1.y - points[i].y);
            }
            if (points[i].y == p1.y)
            {
                sizeX = Math.Abs(p1.x - points[i].x);
            }
        }
        return new Vector2(sizeX, sizeY);
    }
}
