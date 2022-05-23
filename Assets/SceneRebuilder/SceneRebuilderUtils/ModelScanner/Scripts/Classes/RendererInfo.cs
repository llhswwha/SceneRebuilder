using CommonUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RendererInfo:IComparable<RendererInfo>
{
    public static int ID=0;
    public int id=0;

    public bool visible = false;
    public bool visibleNew = false;
    public bool isHideByCollider = false;

    public float distance;

    public int VertexCount=0;

    public float angle = 0;

    public float fullDistance;
    public Renderer renderer;

    public Transform transform;

    public Transform hitObj;

    public Vector3 position;

    public GameObject gameObject;

    public Vector3 size;

    public float diameter;
    public float fullDiameter;

    public void SetPower(float p){
        fullDiameter=diameter*p;
    }

    public Vector3 scale;

    private bool isFirst = true;
    public float maxAngle=0;

    public void Show(){
        gameObject.SetActive(true);
        renderer.enabled = true;
    }

    public void RemoveScripts(){
        MonoBehaviour b=gameObject.GetComponent<MonoBehaviour>();
        if(b!=null){
            GameObject.DestroyImmediate(b);
        }
    }

    public void AddMeshCollider(){
        gameObject.AddComponentEx<MeshCollider>();
    }

    public void RemoveCollider()
    {
        Collider collider=gameObject.GetComponent<Collider>();
        if(collider!=null){
            GameObject.DestroyImmediate(collider);
        }
    }

    public void DisableCollider()
    {
        Collider collider=gameObject.GetComponent<Collider>();
        if(collider!=null){
            collider.enabled=false;
        }
    }

    public void EnableCollider()
    {
        Collider collider=gameObject.GetComponent<Collider>();
        if(collider!=null){
            collider.enabled=true;
        }
    }

    public void SetVisible(bool isV)
    {
        // if(isV==false){
        //     Debug.Log("SetVisible false:"+gameObject);
        // }

        //1.修改GameObject
        // if (isFirst)
        // {
        //     gameObject.SetActive(isV);
        //     isFirst = false;
        // }
        // if (visible != isV)
        // {
        //     gameObject.SetActive(isV);
        // }

        //要碰撞检测来设置物体显示隐藏的话 不能完全隐藏 ，需要有Collider
        //2.修改MeshRender
        if(gameObject==null){
            return;
        }
        if(isV==false){
            if(gameObject.tag=="ScanStatic")
            {
                isV=true;
                //Debug.Log("ScanStatic:"+gameObject);
            }
        }
        if (isFirst)
        {
            if(renderer)renderer.enabled = isV;
            isFirst = false;
        }
        if (renderer&&renderer.enabled != isV)
        {
           renderer.enabled = isV;
        }
        
        visible = isV;
    }

    public void InitState(){
        gameObject.SetActive(true);
        if(renderer)
        {
            if (renderer.sharedMaterial)
            {
                var color = renderer.sharedMaterial.color;
                if (color.a < 1)
                {
                    renderer.enabled = true;
                    visible = true;
                }
                else
                {
                    renderer.enabled = false;
                    visible = false;
                }
            }
            else
            {
                Debug.LogError("RendererInfo.InitState:"+this);
            }
            
        }

        
        
    }

    public void InitState(int layer){
        if(gameObject==null)
        {
            Debug.LogError("InitState gameObject==null:"+this);
            return;
        }
        gameObject.SetActive(true);
        //var color = renderer.sharedMaterial.color;
        if (gameObject.layer==layer && gameObject.tag!="ScanStatic")
        {
            renderer.enabled = false;
            visible=false;
        }
        else
        {
            renderer.enabled = true;
            visible=true;
        }
    }

    public RendererInfo(GameObject obj)
    {
        Renderer r=obj.GetComponent<Renderer>();
        if (r)
        {
            Init(r);
        }
    }

    private void Init(Renderer r){
        this.id=ID++;

        this.renderer = r;
        this.size = r.bounds.size;
        this.diameter = Vector3.Distance(r.bounds.min, r.bounds.max);
        SetPower(40);

        this.transform = r.transform;
        this.position=this.transform.position;//模型位置信息基本不会变化的，除非运行过程中发生了模型移动
        this.gameObject = r.gameObject;
        this.scale = this.transform.lossyScale;
        MeshFilter meshFilter =  this.gameObject.GetComponent<MeshFilter>();
        if(meshFilter!=null && meshFilter.sharedMesh!=null){
            VertexCount=meshFilter.sharedMesh.vertexCount;
        }
        // objs.Add(null);
        // objs.Add(null);

        // visible = this.gameObject.activeSelf;


    }

    public RendererInfo(Renderer r)
    {
        Init(r);
    }

    // public List<GameObject> objs=new List<GameObject>();

    public int CompareTo(RendererInfo other)
    {
        return other.distance.CompareTo(this.distance);
    }

    public override string ToString()
    {
        if(gameObject==null){
return string.Format("name:{0};distance:{1};fullDistance:{2};diameter:{3};fullDiameter:{4}","NULL",distance,fullDistance,diameter,fullDiameter);
        }
        else{
return string.Format("name:{0};distance:{1};fullDistance:{2};diameter:{3};fullDiameter:{4}",gameObject.name,distance,fullDistance,diameter,fullDiameter);
        }
        
    }
}