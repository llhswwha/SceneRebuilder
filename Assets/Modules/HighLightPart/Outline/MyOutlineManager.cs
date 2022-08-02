using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class MyOutlineManager : MonoBehaviour
{
    public static MyOutlineManager Instance;

    [ColorUsage(false,true)]
    public Color defaultColor;

    public bool IsAutoColor=false;

    public LayerMask outlineLayer;

    Dictionary<GameObject,int> go2layer=new Dictionary<GameObject, int>();

    // Update is called once per frame

    public bool MultiSelect=false;
    public GameObject selectedGO=null;

    public List<GameObject> selectedGOs=new List<GameObject>();

    public MyOutlinePass FirstPass;
    public MyOutlinePass CurrentPass;
    public List<MyOutlinePass> passPool=new List<MyOutlinePass>();

    //Dictionary<Color,int> color2layer=new Dictionary<Color, int>();

    public CustomPassVolume volume;

    public int MaxPassCount=6;

    public int CurrentPassId=1;

    void Awake()
    {
        Instance=this;
    }
    void Start()
    {
        InitDefaultPass();
    }

    public bool EnableClick=true;

    void Update()
    {
        if(ObjBuffer.Count>0){
            if(IsKeep){
                for(int i=0;i<ObjBuffer.Count;i++){
                    GameObject go=ObjBuffer[i];
                    if(go==null){
                        ObjBuffer.RemoveAt(i);
                        i--;
                        continue;
                    }
                    //SelectGO(go,true,false);
                    string layerName=LayerMask.LayerToName(go.layer);
                    if(!layerName.StartsWith("Outline"))
                    {
                        SelectGO(go,true,false);
                    }
                }
            }
            else{
                SelectGOs(ObjBuffer,false);
                ObjBuffer.Clear();
            }
        }
        
        if(EnableClick){
            if(Input.GetKey(KeyCode.LeftControl)||Input.GetKey(KeyCode.RightControl))
            {
                MultiSelect=true;
            }
            else{
                MultiSelect=false;
            }

            if(Input.GetMouseButtonUp(0)){
                Debug.Log("Click");
                //var pos=
                var ray=Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if(Physics.Raycast(ray,out hitInfo)){
                    var go=hitInfo.collider.gameObject;
                    SelectGO(go,MultiSelect,false);
                }
                else{
                    RecoverGO();
                }
            }
        }
    }

    public void RecoverGO()
    {
        foreach(GameObject g in selectedGOs){
            if(g!=null){
                SetLayerAll(g,go2layer[g]);
            }
        }
        selectedGOs.Clear();
        if(selectedGO!=null){
            SetLayerAll(selectedGO,go2layer[selectedGO]);
            selectedGO=null;
        }
    }

    private MyOutlinePass CreateOutlinePass(Color color,LayerMask layerMask){
        MyOutlinePass outlinePass=new MyOutlinePass();
        outlinePass.outlineColor=color;
        outlinePass.outlineLayer=layerMask;
        outlinePass.threshold=0;
        volume.customPasses.Add(outlinePass);

        passPool.Add(outlinePass);

        //int newLayer=layerMask.GetLayerNumber();//Set Outline
        //color2layer.Add(color,newLayer);
        CurrentPass=outlinePass;
        if(FirstPass==null){
            FirstPass=outlinePass;
        }
        return outlinePass;
    }

    private MyOutlinePass SetOutlinePass(Color color,int id){
        MyOutlinePass outlinePass=passPool[id];
        outlinePass.outlineColor=color;
        passPool.Add(outlinePass);
        //int newLayer=layerMask.GetLayerNumber();//Set Outline
        //color2layer.Add(color,newLayer);
        //CurrentPass=outlinePass;
        return outlinePass;
    }

    private int GetNextOutlinePass(Color color){
        MyOutlinePass outlinePass=SetOutlinePass(color,CurrentPassId);
        CurrentPassId++;
        if(CurrentPassId>MaxPassCount){
            CurrentPassId=2;
        }
        int newLayer=outlinePass.outlineLayer.GetLayerNumber();
        return newLayer;
    }

    [ContextMenu("InitDefaultPass")]
    private void InitDefaultPass(){
        if(volume==null){
            volume=gameObject.GetComponent<CustomPassVolume>();
        }
        if(volume==null){
            volume=gameObject.AddComponent<CustomPassVolume>();
        }
        if(volume.customPasses.Count==0){
            Debug.Log("InitDefaultPass 1");
            for(int i=0;i<MaxPassCount;i++){
                outlineLayer=LayerMask.GetMask("Outline"+CurrentPassId);
                MyOutlinePass outlinePass=CreateOutlinePass(defaultColor,outlineLayer);
                CurrentPassId++;
            }
        }
        else{
            Debug.Log("InitDefaultPass 2");
            for(int i=0;i<volume.customPasses.Count;i++){
                MyOutlinePass pass=volume.customPasses[i] as MyOutlinePass;
                if(pass!=null){
                    passPool.Add(pass);
                    CurrentPassId++;
                    CurrentPass=pass;
                    if(FirstPass==null){
                        FirstPass=pass;
                    }
                }
            }
        }
        CurrentPassId=2;
    }

     private void SetOutline(GameObject go,bool isMulti,bool isFirst)
     {
        int newLayer=go.layer;
        MyOutlineInfo info=go.GetComponent<MyOutlineInfo>();
        Color color=defaultColor;
        if(info!=null){
            color=info.color;
        }

        if(IsAutoColor){
            CurrentPassId++;
            if(CurrentPassId>MaxPassCount){
                CurrentPassId=1;
            }

            //CurrentPass=passPool[CurrentPassId-1];
            //CurrentPass=FirstPass;
            //FirstPass.outlineColor=CurrentPass.outlineColor;

            FirstPass=passPool[CurrentPassId-1];
        }

        if(isMulti)
        {
            if(isFirst)
            {
                Debug.Log("SetOutline 1");

                FirstPass.outlineColor=color;
                newLayer=FirstPass.outlineLayer.GetLayerNumber();
                //newLayer=GetNextOutlinePass(color);
                SetLayerAll(go,newLayer);
            }
            else
            {
                Debug.Log("SetOutline 2");
                newLayer=GetNextOutlinePass(color);
                SetLayerAll(go,newLayer);
            }
        }
        else
        {
            //Debug.Log("SetOutline 3");

            if(IsAutoColor==false){
                FirstPass.outlineColor=color;
            }
            newLayer=FirstPass.outlineLayer.GetLayerNumber();
            //newLayer=GetNextOutlinePass(color);
            SetLayerAll(go,newLayer);
        }
    }

    private void SetLayerAll(GameObject go,int ly){
        go.layer=ly;
        Transform[] children=go.GetComponentsInChildren<Transform>(true);
        foreach(var child in children){
            child.gameObject.layer=ly;
        }
    }

    public void SelectGO(GameObject go,bool isMulti,bool isFirst){
        if(!go2layer.ContainsKey(go)){
            go2layer.Add(go,go.layer);
            Debug.Log("AddLayer:"+go+"|"+go.layer);
        }

        if(isMulti==false){
            RecoverGO();
            selectedGO=go;
            selectedGOs.Add(go);
            SetOutline(go,isMulti,isFirst);
        }
        else{
            if(selectedGOs.Contains(go)){
                selectedGOs.Remove(go);
                SetLayerAll(go,go2layer[go]);
                Debug.Log("GetLayer:"+go+"|"+go.layer);
            }
            else{
                selectedGOs.Add(go);
                SetOutline(go,isMulti,isFirst);
            }
        }
    }

    public List<GameObject> TestObjs;

    public bool IsOnlyFirst=false;

    [ContextMenu("TestSelect")]
    public void TestSelect()
    {
        SelectGOs(TestObjs,IsOnlyFirst);
    }

    public void SelectGOs(List<GameObject> objs,bool isFirst)
    {
        RecoverGO();
        foreach(GameObject go in objs){
            SelectGO(go,true,isFirst);
        }
    }

    public List<GameObject> ObjBuffer=new List<GameObject>();

    public bool IsKeep=false;


    [ContextMenu("TestSelectBuffer")]
    public void TestSelectBuffer()
    {
        SelectGOsBuffer(TestObjs,true);
    }
    public void SelectGOsBuffer(List<GameObject> objs,bool isKeep)
    {
        ObjBuffer=new List<GameObject>();
        ObjBuffer.AddRange(objs);
        IsKeep=isKeep;
    }

     public void SelectGOsBuffer(GameObject obj,bool isKeep)
    {
        List<GameObject> objs =new List<GameObject>();
        var rs=obj.GetComponentsInChildren<Renderer>();
        foreach(var r in rs){
            objs.Add(r.gameObject);
        }
        ObjBuffer=new List<GameObject>();
        ObjBuffer.AddRange(objs);
        IsKeep=isKeep;
    }
}
