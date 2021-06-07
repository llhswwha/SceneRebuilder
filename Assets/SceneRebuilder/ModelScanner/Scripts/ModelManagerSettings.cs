using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ModelManagerSettings : MonoBehaviour
{
    public static ModelManagerSettings Instance;
    public List<ModelManager> Managers=new List<ModelManager>();
    public List<SubModelController> Controllers = new List<SubModelController>();

    public int AllCount = 0;
    //动态
    public int VisibleCount = 0;
    public int HiddenCount = 0;
    public int ChangedCount = 0;

    public Text LogText;
    public bool ShowLog=true;

    public Material TransparentMaterial;

    public Material RedMaterial;

    public Material BlueMaterial;

    public Material GreenMaterial;

    public bool TestDistance=false;

    public string ScanLayerName = "ScanModel";

    public string TransparentLayer = "TransparentFX";

    public int ScanLayer ;

    public LayerMask ScanLayerMask;

    void Awake(){
        Instance=this;
    }

    void Start(){

        ScanLayer = LayerMask.NameToLayer(ScanLayerName);
        ScanLayerMask = LayerMask.GetMask(ScanLayerName);
        foreach (var m in Managers){
            if (m == null) continue;
            if(m.gameObject.activeSelf==false)continue;
            m.ShowLog=false;
            Controllers.AddRange(m.Controllers);
            AllCount+=m.AllCount;
        }

        StartCoroutine(ShowLogInfo());
    }

    [ContextMenu("TransparentAll")]
    public void TransparentAll(){
        foreach(var m in Managers){
            if(m.gameObject.activeSelf==false)continue;
            m.SetMaterial(TransparentMaterial);
        }
    }

    [ContextMenu("SetLayer")]
    public void SetLayer()
    {
        DateTime start = DateTime.Now;
        int l = LayerMask.NameToLayer(ScanLayerName);
        int tl= LayerMask.NameToLayer(TransparentLayer);
        foreach (var m in Managers)
        {
            if (m.gameObject.activeSelf == false) continue;
            m.SetLayer(l,tl);
        }
        TimeSpan t = DateTime.Now - start;
        Debug.Log(string.Format("SetLayer 用时{0}", t));
    }

    [ContextMenu("InitState")]
    public void InitState(){
        DateTime start = DateTime.Now;
        int l = LayerMask.NameToLayer(ScanLayerName);
        foreach (var m in Managers)
        {
            if(m==null)continue;
            if(m.gameObject==null)continue;
            if (m.gameObject.activeSelf == false) continue;
            m.InitState(l);
        }
        TimeSpan t = DateTime.Now - start;
        Debug.Log(string.Format("InitState 用时{0}", t));
    }

    [ContextMenu("InitStateEx")]
    public void InitStateEx(){
        StaticCount=0;
        DateTime start = DateTime.Now;
        int l = LayerMask.NameToLayer(ScanLayerName);
        int tl = LayerMask.NameToLayer(TransparentLayer);
        foreach (var m in Managers)
        {
            if(m==null)continue;
            if(m.gameObject==null)continue;
            if (m.gameObject.activeSelf == false) continue;
            m.ResetInit();
            m.SetLayer(l,tl);
            m.InitState(l);
            StaticCount+=m.GetStaticCount();
        }
        TimeSpan t = DateTime.Now - start;
        Debug.Log(string.Format("InitState 用时{0}", t));
    }

    public int StaticCount=0;

    [ContextMenu("ShowNotInLayer")]
    public void ShowNotInLayer()
    {
        DateTime start = DateTime.Now;
        int l = LayerMask.NameToLayer(ScanLayerName);
        foreach (var m in Managers)
        {
            if (m.gameObject.activeSelf == false) continue;
            m.ShowNotInLayer(l);
        }
        TimeSpan t = DateTime.Now - start;
        Debug.Log(string.Format("SetLayer 用时{0}", t));
    }

    [ContextMenu("SetLayerAll")]
    public void SetLayerAll()
    {
        DateTime start = DateTime.Now;
        int l = LayerMask.NameToLayer(ScanLayerName);
        int tl = LayerMask.NameToLayer(TransparentLayer);
        foreach (var m in Managers)
        {
            // if (m.gameObject.activeSelf == false) continue;
            m.SetLayer(l, tl);
        }
        TimeSpan t = DateTime.Now - start;
        Debug.Log(string.Format("SetLayerAll 用时{0}", t));
    }

    // [ContextMenu("TransparentAll")]
    public void SetMaterial(Material mat){
        foreach(var m in Managers){
             if(m==null)continue;
            if(m.gameObject==null)continue;
            if (m.gameObject.activeSelf == false) continue;
            m.SetMaterial(mat);
        }
    }

    [ContextMenu("HideAll")]
    public void HideAll(){
        foreach(var m in Managers){
             if(m==null)continue;
            if(m.gameObject==null)continue;
            if (m.gameObject.activeSelf == false) continue;
            m.HideAll();
        }
    }

    [ContextMenu("SetAllHiddenNew")]
    public int SetAllHiddenNew()
    {
        int sum=0;
        foreach (var m in Managers)
        {
             if(m==null)continue;
            if(m.gameObject==null)continue;
            if (m.gameObject.activeSelf == false) continue;
            sum+=m.SetAllHiddenNew();
        }
        return sum;
    }

    public int GetStateCount(bool state)
    {
        int sum=0;
        foreach (var m in Managers)
        {
            if (m.gameObject.activeSelf == false) continue;
            sum+=m.GetStateCount(state);
        }
        return sum;
    }

    public int GetCount()
    {
        int sum=0;
        foreach (var m in Managers)
        {
            if(m==null)continue;
            if(m.gameObject==null)continue;
            if (m.gameObject.activeSelf == false) continue;
            sum+=m.GetCount();
        }
        return sum;
    }

    [ContextMenu("ShowAll")]
    public void ShowAll(){
        foreach(var m in Managers){
             if(m==null)continue;
            if(m.gameObject==null)continue;
            if (m.gameObject.activeSelf == false) continue;
            m.ShowAll();
        }
    }


    public bool IsUpdating()
    {
        for (int i = 0; i < Controllers.Count; i++)
        {
            var controller = Controllers[i];
            if(controller.UpdateEnbale==true)return true;
        }
        return false;
    }

    private IEnumerator ShowLogInfo(){
        while(ShowLog){
            VisibleCount = 0;
            HiddenCount = 0;
            ChangedCount = 0;
            double t = 0;
            for (int i = 0; i < Controllers.Count; i++)
            {
                var controller = Controllers[i];
                VisibleCount += controller.VisibleCount;
                HiddenCount+= controller.HiddenCount;
                ChangedCount+= controller.ChangedCount;
                t += controller.UpdateTime;
            }

            if (LogText != null)
            {
                LogText.text = string.Format("Count={0}\nVisible={1}\nHidden={2}\nChanged={3}\nTime={4}\nUpdating={5}"
            , AllCount, VisibleCount, HiddenCount,ChangedCount, t,this.IsUpdating());
            }
            yield return new WaitForSeconds(0.5f);
        } 
    }

    public Material LastClickMaterial;

    public MeshRenderer LastClickRenderer;

    public bool CheckClick = true;

    void Update()
    {
        // if (CheckClick && Input.GetMouseButtonUp(0))
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线  
        //     RaycastHit hitInfo;
        //     if (Physics.Raycast(ray, out hitInfo))
        //     {
        //         Debug.DrawLine(ray.origin, hitInfo.point);//划出射线，只有在scene视图中才能看到  
        //         GameObject gameObj = hitInfo.collider.gameObject;
        //         Debug.Log("click object name is " + gameObj.name);
        //         //if (gameObj.tag == "boot")//当射线碰撞目标为boot类型的物品 ，执行拾取操作  
        //         //{
        //         //    Debug.Log("pick up!");
        //         //}

        //         MeshRenderer renderer=gameObj.GetComponent<MeshRenderer>();
        //         LastClickMaterial = renderer.material;
        //         renderer.material = RedMaterial;

        //         if(LastClickRenderer!= renderer && LastClickRenderer!=null)
        //         {
        //             LastClickRenderer.material = LastClickMaterial;
        //             LastClickRenderer = renderer;
        //         }
        //     }
        // }
    }
}