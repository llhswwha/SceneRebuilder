// using Mogoson.CameraExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class MeshTestUI : MonoBehaviour
{
    public static MeshTestUI Instance;

    void Awake()
    {
        Instance = this;
    }

    public InputField InputCount;

    public InputField InputModelMaxCount;

    public InputField InputTypeMaxCount;

    public InputField InputTypeStartId;

    public InputField InputOffset;

    public GameObject Target;

    public Text TextMeshVertsCount;

    public Toggle ToggleFrame;

    public Toggle ToggleUnits;

    //public Toggle ToggleECS1;

    //public Toggle ToggleECS2;

    public Dropdown DropdownModelType;

    public Dropdown DropdownDetailLevel;

    public InputField InputCopyCount;

    public Toggle ToggleUpdateChildren;

    public Toggle ToggleIsTest;

    // Start is called before the first frame update
    void Start()
    {
        GetMeshVertsCount(Target, 1,false);
        DropdownModelType.onValueChanged.AddListener(ModeTypeListChanged);

        if (rebuilder == null)
            rebuilder = SceneRebuilder.Instance;
    }

    private void ModeTypeListChanged(int id)
    {
        Debug.Log("ModeTypeListChanged:"+id);
        rebuilder.ModelType = id;
    }

    public MeshFilter[] meshFilters;
    public static int verts;
    public static int tris;
    public static int nodeCount;

    private void GetMeshVertsCount(GameObject target, int count,bool fromBuilder)
    {
        Debug.Log("GetMeshVertsCount Start:" + count);


        if(SceneRebuilder.Instance!=null && fromBuilder)
         {
            TextMeshVertsCount.text = SceneRebuilder.Instance.GetStatisticInfo();
        }
        else
        {
            CalculateVertsInfo(target, count);

            TextMeshVertsCount.text = string.Format("tris:{0}w,verts:{1}w,count:{2}", tris / 10000, verts / 10000, nodeCount);
        }

        Debug.Log("GetMeshVertsCount End:" + verts);
    }

    private void CalculateVertsInfo(GameObject target, int count)
    {
        tris = 0;
        verts = 0;
        nodeCount = 0;
        if (target)
        {
            meshFilters = target.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter f in meshFilters)
            {
                tris += f.sharedMesh.triangles.Length / 3;
                verts += f.sharedMesh.vertexCount;

            }
            nodeCount = meshFilters.Length;
        }
        verts *= count;
        tris *= count;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<GameObject> copedObjects = new List<GameObject>();

    private Vector3 GetVector3(string txt)
    {
        string[] parts = txt.Split(',');
        float x = float.Parse(parts[0]);
        float y = float.Parse(parts[1]);
        float z = float.Parse(parts[2]);
        Vector3 off = new Vector3(x, y, z);
        return off;
    }

    private void DestroyCopedObjects()
    {
        foreach (var item in copedObjects)
        {
            GameObject.DestroyImmediate(item);
        }
        copedObjects.Clear();
    }

    int c = 0;
    Vector3 center = Vector3.zero;
    private void CreateObjects()
    {
        Vector3 offset = GetVector3(InputOffset.text);
        Vector3 count = GetVector3(InputCount.text);

        c = 0;
        center = Vector3.zero;

        int max = int.Parse(InputModelMaxCount.text);

        for (int i = 0; i < count.x; i++)
        {
            for (int j = 0; j < count.y; j++)
            {
                for (int k = 0; k < count.z; k++)
                {
                    if (i == 0 && j == 0 && k == 0) continue;
                    c++;
                    if (max > 0 && c >= max) return;

                    Vector3 offEx = new Vector3(offset.x * i, offset.y * j, offset.z * k);
                    Debug.Log(string.Format("[{0}]({1},{2},{3}),{4}", c, i, j, k, offEx));
                    GameObject newObj = GameObject.Instantiate(Target);
                    Vector3 posNew = Target.transform.position + offEx;
                    newObj.transform.position = posNew;
                    center += posNew;
                    copedObjects.Add(newObj);
                }
            }
        }
    }

    public void Copy()
    {
        DestroyCopedObjects();

        CreateObjects();
        center /= (c);
        if (centerObj == null)
        {
            centerObj = new GameObject();
            centerObj.name = "center";

        }
        centerObj.transform.position = center;

        // if (aroundCamera != null)
        // {
        //     aroundCamera.SetTarget(centerObj.transform);
        // }

        GetMeshVertsCount(Target, (int)(c),false);
    }


    // public AroundCamera aroundCamera;

    public GameObject centerObj = null;


    public Dropdown dropdownSceneList;

    [ContextMenu("Load")]
    public void Load()
    {
        DestroyCopedObjects();
        var item = dropdownSceneList.options[dropdownSceneList.value];
        Debug.Log(item.text);
        SceneRebuilder.Instance.LoadScene(item.text, obj =>
         {
             Debug.Log("AfterLoad:" + obj);
             this.Target = obj;
             GetMeshVertsCount(Target, 1,true);
         });
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        if (ModelParent != null)
        {
            GameObject.DestroyImmediate(ModelParent);
        }

        DestroyCopedObjects();
        GameObject.DestroyImmediate(Target);
        //GetMeshVertsCount();

        SceneRebuilder.Instance.Clear();

        tris = 0;
        verts = 0;
        TextMeshVertsCount.text = string.Format("tris:{0},verts:{1}", tris, verts);
    }

    [ContextMenu("GetModels")]
    public void GetModels()
    {
        Debug.Log("GetModels Start");
        string dir = @"H:\Workspace\GitHub\RevitToUnity\Assets\Resources\Models\地铁站";

        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        if (dirInfo.Exists == false)
        {
            Debug.Log("文件夹不存在:" + dir);
            return;
        }
        FileInfo[] fileInfos = dirInfo.GetFiles();
        Debug.Log("文件数量:" + fileInfos.Length);

        List<ModelFile> modelFiles = new List<ModelFile>();
        foreach (FileInfo fileInfo in fileInfos)
        {
            if (fileInfo.Extension == ".meta") continue;
            string model = fileInfo.Name;
            model = model.Replace(".fbx", "");
            model = model.Replace(".FBX", "");

            ModelFile modelFile = new ModelFile();
            modelFile.Name = model;
            modelFile.Length = fileInfo.Length;
            modelFiles.Add(modelFile);
        }
        modelFiles.Sort();
        Debug.Log("模型数量:" + modelFiles.Count);

        ModelListDropdown.ClearOptions();

        List<OptionData> options = new List<OptionData>();
        string models = "";
        foreach (ModelFile modelFile in modelFiles)
        {
            string fileLength = GetLength(modelFile.Length);
            models += modelFile.Name + "|" + fileLength + "\n";

            OptionData option = new OptionData();
            option.text = modelFile.Name;
            options.Add(option);
        }
        ModelListDropdown.AddOptions(options);

        Debug.Log(models);

        Debug.Log("GetModels End");
    }

    public Dropdown ModelListDropdown;

    public class ModelFile : IComparable<ModelFile>
    {
        public string Name;
        public long Length;

        public int CompareTo(ModelFile other)
        {
            //return other.Length.CompareTo(this.Length);
            return this.Length.CompareTo(other.Length);
        }
    }
    static string GetLength(long lengthOfDocument)
    {

        if (lengthOfDocument < 1024)
            return string.Format(lengthOfDocument.ToString() + 'B');
        else if (lengthOfDocument > 1024 && lengthOfDocument <= Math.Pow(1024, 2))
            return string.Format((lengthOfDocument / 1024.0).ToString() + "KB");
        else if (lengthOfDocument > Math.Pow(1024, 2) && lengthOfDocument <= Math.Pow(1024, 3))
            return string.Format((lengthOfDocument / 1024.0 / 1024.0).ToString("F1") + "M");
        else
            return string.Format((lengthOfDocument / 1024.0 / 1024.0 / 1024.0).ToString() + "GB");
    }

    public GameObject ModelParent;

    public Dictionary<string, GameObject> LoadedModels = new Dictionary<string, GameObject>();

    public void LoadModel()
    {
        string sceneName = GetCurrentModelName();
        LoadModel(sceneName);
        GetMeshVertsCount(ModelParent, 1,false);
    }

    private string GetCurrentModelName()
    {
        if (ModelParent == null)
        {
            ModelParent = new GameObject();
            ModelParent.name = "ModelParent";
        }

        UnLoadModel();

        string sceneName = "";
        if (ModelListDropdown != null)
        {
            var item = ModelListDropdown.options[ModelListDropdown.value];
            sceneName = item.text;
        }
       
        return sceneName;
    }

    private void LoadModel(string mName)
    {
        Debug.Log(mName);
        //string PrebPath1 = "Models/地铁站/" + mName;
        string PrebPath1 = "RvtScenes/" + mName + "/Models/Scene";

        Debug.Log(PrebPath1);
        GameObject preb1 = Resources.Load<GameObject>(PrebPath1);
        if (preb1 == null)
        {
            Debug.LogError("文件1不存在:" + PrebPath1);
            PrebPath1 = "Models/地铁站/" + mName; //以前的测试用例
            preb1 = Resources.Load<GameObject>(PrebPath1);
        }
        if (preb1 == null)
        {
            Debug.LogError("文件2不存在:"+ PrebPath1);
        }
        else
        {
            //用加载得到的资源对象，实例化游戏对象，实现游戏物体的动态加载
            GameObject prebLoad1 = Instantiate(preb1) as GameObject;
            prebLoad1.transform.parent = ModelParent.transform;
            LoadedModels.Add(mName, prebLoad1);
        }
    }

    public SceneRebuilder rebuilder;

    [ContextMenu("LoadModelEx")]
    public void LoadModelEx()
    {
        var start = DateTime.Now;
        Debug.Log("GetModelEx Start");
        string sceneName = GetCurrentModelName();

        SetRebuider();
        //SceneRebuilder.Instance.IsECS1 = ToggleECS1.isOn;
        //SceneRebuilder.Instance.IsECS2 = ToggleECS2.isOn;
        rebuilder.LoadScene(sceneName, obj =>
        {
            ShowModelTypes(rebuilder.typeList,true);
            ModelTypeItemUI.isFirst = false;
            GetMeshVertsCount(obj,true);
            var time = DateTime.Now - start;
            Debug.Log("GetModelEx End 用时:"+ time);
        });
    }

    public void ShowMeshVertsCount()
    {
        //GetMeshVertsCount(null,false);

        GetMeshVertsCount(SceneRebuilder.Instance.root, 1, false);
    }

    private void GetMeshVertsCount(GameObject obj,bool fromRebulid)
    {
        if (obj == null)
        {
            Debug.LogError("SceneRebuilder.Instance.LoadScene obj == null");
            //LoadModel(sceneName);
            GetMeshVertsCount(ModelParent, 1, fromRebulid);
        }
        else
        {
            this.Target = obj;
            GetMeshVertsCount(Target, 1, fromRebulid);
        }
    }

    public void SetRebuider()
    {
        if (rebuilder == null)
            rebuilder = SceneRebuilder.Instance;
        rebuilder.IsLoadFrame = ToggleFrame.isOn;
        rebuilder.IsLoadUnits = ToggleUnits.isOn;
        rebuilder.isLoadTypes = false;
        rebuilder.ModelType = DropdownModelType.value;
        rebuilder.DetailLevel = DropdownDetailLevel.value;
        rebuilder.CopyCount = int.Parse(InputCopyCount.text);
        rebuilder.ModelMaxCount = int.Parse(InputModelMaxCount.text);
        rebuilder.TypeMaxCount = int.Parse(InputTypeMaxCount.text);
        rebuilder.startTypeId = int.Parse(InputTypeStartId.text);
        rebuilder.IsUpdateChildren = ToggleUpdateChildren.isOn;
        rebuilder.IsTest = ToggleIsTest.isOn;
        print("SetRebuider DetailLevel:" + rebuilder.DetailLevel);
    }

    [ContextMenu("LoadModelTypes")]
    public void LoadModelTypes()
    {
        var start = DateTime.Now;
        ModeTypeListContainer.SetActive(true);
        Debug.Log("LoadModelTypes Start");
        if (rebuilder == null)
            rebuilder = SceneRebuilder.Instance;
        string sceneName = GetCurrentModelName();
        Debug.Log("sceneName:"+ sceneName);
        SetRebuider();
        rebuilder.IsLoadFrame = false;
        rebuilder.IsLoadUnits = false;
        rebuilder.isLoadTypes = true;
        rebuilder.LoadTypes(sceneName, obj =>
        {
            ShowModelTypes(rebuilder.typeList,false);
            var time = DateTime.Now - start;
            Debug.Log("LoadModelTypes End 用时:" + time);
        });
    }


    public GameObject ModeTypeListContainer;

    public Text HideModelTypeListText;

    public void HideModeTypeList()
    {
        if (HideModelTypeListText.text == "隐藏列表")
        {
            HideModelTypeListText.text = "显示列表";
            ModeTypeListContainer.SetActive(false);
        }
        else
        {
            HideModelTypeListText.text = "隐藏列表";
            ModeTypeListContainer.SetActive(true);
        }
        
    }

    public int maxTypeUICount = 100;

    private void ShowModelTypes(NodeTypeInfoList list,bool setLoad)
    {
        if (list == null)
        {
            Debug.LogError("ShowModelTypes list == null");
            return;
        }

        foreach (var item in TypeItemUIList)
        {
            GameObject.Destroy(item);
        }
        TypeItemUIList.Clear();

        for (int i = 0; i < list.Count && i< maxTypeUICount; i++)
        {
            NodeTypeInfo item = list[i];
            if (item.GetTotalVertCount() == 0) continue;
            GameObject itemUi = GameObject.Instantiate(TypeItemUI);
            TypeItemUIList.Add(itemUi);
            itemUi.SetActive(true);
            itemUi.name = item.typeName;
            itemUi.transform.SetParent(TypeListPanel);

            ModelTypeItemUI ui = itemUi.GetComponent<ModelTypeItemUI>();
            ui.SetInfo(item);
            if (setLoad)
            {
                ui.SetIsLoad(true);
            }
        }
    }

    public List<GameObject> TypeItemUIList = new List<GameObject>();

    public RectTransform TypeListPanel;

    public GameObject TypeItemUI;

    public void UnLoadModel()
    {
        if (ModelListDropdown != null)
        {
            var item = ModelListDropdown.options[ModelListDropdown.value];
            if (LoadedModels.ContainsKey(item.text))
            {
                GameObject.DestroyImmediate(LoadedModels[item.text]);
                LoadedModels.Remove(item.text);
            }
        }
        
    }
}
