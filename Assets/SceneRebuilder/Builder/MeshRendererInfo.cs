using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshRendererInfo : MonoBehaviour
{
    public Vector3 position;

    public Vector3 center;

    public float disToCenter;

    public Vector3 size;

    public float vertexCount;

    // public Bounds bounds;

    public MeshFilter meshFilter;

    public MeshRenderer meshRenderer;

    public Vector3[] minMax;

    public static Vector3[] GetMinMax(GameObject go)
    {
        MeshRendererInfo info = go.GetComponent<MeshRendererInfo>();
        if (info == null)
        {
            info = go.AddComponent<MeshRendererInfo>();
            info.Init();
        }
        return info.minMax;
    }

    public static MeshRendererInfo GetInfo(GameObject go)
    {
        //Debug.Log($"MeshRendererInfo go:{go}");
        MeshRendererInfo info = go.GetComponent<MeshRendererInfo>();
        //Debug.Log($"info:{info} go==null:{go == null}");
        if (info == null)
        {
            info = go.AddComponent<MeshRendererInfo>();
            info.Init();
            //Debug.Log($"AddComponent info:{info} info==null:{info == null}");
        }
        return info;
    }

    //public static List<MeshRendererInfo> GetLod0s(GameObject go)
    //{
    //    var renderers = go.GetComponentsInChildren<MeshRenderer>(true);
    //    List<MeshRendererInfo> list = GetLod0s(renderers);
    //    return list;
    //}

    //public static List<MeshRendererInfo> GetLod0s(MeshRenderer[] renderers)
    //{
    //    List<MeshRendererInfo> list = new List<MeshRendererInfo>();
    //    foreach (var renderer in renderers)
    //    {
    //        var info = MeshRendererInfo.GetInfo(renderer.gameObject);
    //        if (info.LodIds.Contains(0) || info.LodIds.Contains(-1) || info.LodIds.Count==0)
    //        {
    //            list.Add(info);
    //        }
    //    }
    //    //var infos= go.GetComponentsInChildren<MeshRendererInfo>(true);
    //    //list = infos.Where(i => i.LodId == 0|| i.LodId == -1).ToList();
    //    return list;
    //}

    public static List<MeshRendererInfo> GetLodN(GameObject go,int lv)
    {
        var renderers = go.GetComponentsInChildren<MeshRenderer>(true);
        List<MeshRendererInfo> list = GetLodN(renderers, lv);
        return list;
    }


    public static List<MeshRendererInfo> GetLodN(MeshRenderer[] renderers,int lv)
    {
        List<MeshRendererInfo> list = new List<MeshRendererInfo>();
        foreach (var renderer in renderers)
        {
            var info = MeshRendererInfo.GetInfo(renderer.gameObject);
            if (info.IsLodN(lv))
            {
                list.Add(info);
            }
        }
        return list;
    }

    public static MeshRendererInfoList[] SplitByLOD(MeshRenderer[] renderers)
    {
        MeshRendererInfoList[] lodList = new MeshRendererInfoList[4];
        for(int i = 0; i < lodList.Length; i++)
        {
            lodList[i] = new MeshRendererInfoList();
        }

        foreach (var renderer in renderers)
        {
            var info = MeshRendererInfo.GetInfo(renderer.gameObject);
            for(int i=0;i<lodList.Length;i++)
            {
                if (info.IsLodN(i))//0,1,2,3
                {
                    lodList[i].Add(info);
                }
            }
        }
        return lodList;
    }

    public MeshRendererType rendererType;

    [ContextMenu("GetRendererType")]
    public void GetRendererType()
    {
        bool isDetail=RendererManager.Instance.IsDetail(this.gameObject);
        if(isDetail)
        {
            rendererType=MeshRendererType.Detail;
        }
        Debug.Log("isDetail:"+isDetail);
    }

    [ContextMenu("Init")]
    public void Init()
    {
        //Debug.Log("Init");
        position=this.transform.position;
        meshRenderer=gameObject.GetComponent<MeshRenderer>();
        meshFilter=gameObject.GetComponent<MeshFilter>();
        if(meshFilter!=null){
            if(meshFilter.sharedMesh!=null)
            {
                vertexCount=meshFilter.sharedMesh.vertexCount;
                minMax=MeshHelper.GetMinMax(meshFilter);
                if(minMax!=null && minMax.Length>3){
                    center=minMax[3];
                    size=minMax[2];
                    disToCenter=Vector3.Distance(center,position);
                }
            }
            else{
                Debug.LogError($"MeshRendererInfo.Init() meshFilter.sharedMesh==null:"+this.name);
            }
        }
        else{
            Debug.LogError($"MeshRendererInfo.Init() meshFilter==null:"+this.name);
        }
        // if(rendererType!=MeshRendererType.Detail)
        // {
        //     if(IsStatic()){
        //         rendererType=MeshRendererType.Static;
        //     }
        //     else{
        //         rendererType=MeshRendererType.None;
        //     }
        // }
    }

    // public bool IsStatic()
    // {
    //     List<string> detailNames=new List<string>(){
    //         //"90 Degree Direction Change"
    //         };
    //     foreach(var dn in detailNames){
    //         if(this.name.Contains(dn) || this.transform.parent.name.Contains(dn))
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }

    [ContextMenu("ShowBounds")]
    public void ShowBounds()
    {
        Debug.Log("ShowBounds");
        CreatePoint(minMax[0],"min");
        CreatePoint(minMax[1],"max");
        CreatePoint(minMax[2],"size");
        CreatePoint(minMax[3],"center");

        GameObject centerGo=GameObject.CreatePrimitive(PrimitiveType.Cube);
        centerGo.name=this.name+"_Bounds";
        centerGo.transform.position=minMax[3];
        centerGo.transform.localScale=minMax[2];
        centerGo.transform.SetParent(this.transform);
    }

    private void CreatePoint(Vector3 pos,string name)
    {
        GameObject centerGo=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        centerGo.name=name;
        centerGo.transform.position=pos;
        centerGo.transform.localScale=new Vector3(0.1f,0.1f,0.1f);
        centerGo.transform.SetParent(this.transform);
    }

    [ContextMenu("ShowCenter")]
    public void ShowCenter()
    {
        Debug.Log("ShowCenter");
        GameObject centerGo=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        centerGo.name=this.name+"_center";
        centerGo.transform.position=center;
        centerGo.transform.localScale=new Vector3(0.1f,0.1f,0.1f);
        centerGo.transform.SetParent(this.transform);
    }

    [ContextMenu("CenterPivot")]
    public void CenterPivot()
    {
        var oldP=this.transform.parent;
        
        if(oldP!=null){
            float disToParent=Vector3.Distance(oldP.transform.position,center);
            Debug.Log("disToParent:"+disToParent);
            if(disToParent<0.0001)//
            {
                Debug.Log("CenterPivot 0:"+this.name);
                return;
            }
        }
        if(oldP!=null && oldP.childCount==1 /* && oldP.GetComponents<Component>().Length==1 */){

            Debug.Log("CenterPivot 1:"+this.name);
            this.transform.SetParent(null);
            oldP.position=center;
            this.transform.SetParent(oldP);
        }
        else{
            Debug.Log("CenterPivot 2:"+this.name);
            GameObject centerGo=new GameObject();
            centerGo.name=this.name+"_center";
            centerGo.transform.position=center;
            
            this.transform.SetParent(centerGo.transform);
            centerGo.transform.SetParent(oldP);
        }
    }

    [ContextMenu("TestCenterPivot")]
    public void TestCenterPivot()
    {
        CenterPivot(0.0001f);
    }

    public bool CenterPivot(float dis)
    {
        if(disToCenter>dis)
        {
            CenterPivot();
            return true;
        }
        else{
            Debug.Log($"No CenterPivot dis:{disToCenter} max:{dis}");
            return false;
        }
    }

    // public void OnDisable()
    // {
    //     //Debug.Log($"OnDisable {this.name} p:{transform.parent}");
    // }
    // public void OnTransformParentChanged()
    // {
    //     //Debug.Log($"OnTransformParentChanged {this.name} p:{transform.parent}");
    // }

    public List<int> LodIds = new List<int>();

    public string GetLODIds()
    {
        string txt = "";
        for (int i = 0; i < LodIds.Count; i++)
        {
            int ld = LodIds[i];
            txt += ld.ToString();
            if(i< LodIds.Count-1)
            {
                txt += ";";
            }
        }
        return txt;
    }

    public bool IsLodN(int lv)
    {
        if (lv == 0)
        {
            return LodIds.Count == 0 || LodIds.Contains(0);
        }
        else
        {
            return LodIds.Contains(lv);
        }
    }
}


public class MeshRendererInfoList:List<MeshRendererInfo>
{
    public List<MeshRenderer> GetRenderers()
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        foreach(var item in this)
        {
            renderers.Add(item.meshRenderer);
        }
        return renderers;
    }
}

public enum MeshRendererType
{
    None,
    Static,//(Big)
    Detail,//(Small)
}
