// using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Icao;
using LitJson;
using RevitTools.Infos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if UNITY_ECS
using Unity.Entities;
#endif
// using Unity.Entities;
// using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class SceneRebuilder : MonoBehaviour
{
    public static SceneRebuilder Instance;
    //public List<GameObject> Units = new List<GameObject>();

    public GameObject UnitRoot;

    public GameObject TargetScene;

    public GameObject PrefabRoot;

    public List<MeshPrefab> MeshPrefabs = new List<MeshPrefab>();

    public List<GameObject> Prefabs = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //InitPrefabs();
    }

    internal void LoadNodeType(NodeTypeInfo typeInfo,bool setCenter)
    {
        Debug.Log("LoadNodeType ModelType:" + ModelType);
        IsLOD = false;
        if (ModelType == 0)
        {
            //CreateObjects(list, false, true);//Normal
            ShowPrefabs();
            CreateObjectsByTypeItem(typeInfo, false,0,newPrefabsParent);
            HidePrefabs();
        }
        else if (ModelType == 1)
        {
            IsLOD = true;
            //CreateObjects(list, false, true);//Normal(LOD)
            ShowPrefabs();
            CreateObjectsByTypeItem(typeInfo, false,0, newPrefabsParent);
            HidePrefabs();
        }
        else if (ModelType == 2)//ConvertToEntity
        {
            //CreateObjects(list, true, true);
            ShowPrefabs();
            CreateObjectsByTypeItem(typeInfo, true, 0, newPrefabsParent);
            HidePrefabs();
        }
        else if (ModelType == 3)//CreateEntity
        {
            CreateEntitiesByTypeListItem(typeInfo, false,0);
        }
        else if (ModelType == 4)//CreateEntity(LOD)
        {
            IsLOD = true;
            CreateEntitiesByTypeListItem(typeInfo, false, 0);
        }
        else if (ModelType == 5)//CreateEntity(Mesh)
        {
            CreateEntitiesByTypeListItem(typeInfo, true, 0);
        }
        else
        {
            ShowPrefabs();
            CreateEntitiesByTypeListItem(typeInfo, false, 0);
            HidePrefabs();
        }

        if (setCenter)
        {
            var center = typeInfo.GetCenter();
            SetCenter(center);
        }
    }

    public void UnLoadNodeNodeType(NodeTypeInfo typeInfo)
    {
        if (TypeInfoObjects.ContainsKey(typeInfo))
        {
            GameObject obj = TypeInfoObjects[typeInfo];
            GameObject.DestroyImmediate(obj);
        }
    }

    public void HideNodeNodeType(NodeTypeInfo typeInfo)
    {
        if (TypeInfoObjects.ContainsKey(typeInfo))
        {
            GameObject obj = TypeInfoObjects[typeInfo];
            obj.SetActive(false);
        }
    }

    public void ShowNodeNodeType(NodeTypeInfo typeInfo)
    {
        if (TypeInfoObjects.ContainsKey(typeInfo))
        {
            GameObject obj = TypeInfoObjects[typeInfo];
            obj.SetActive(true);
        }
    }

    [ContextMenu("Rebuild")]
    public void Rebuild()
    {
        StartCoroutine(StartRebuild());
    }

    [ContextMenu("Search")]
    public void Search()
    {
        DateTime start = DateTime.Now;
        foreach (var prefab in MeshPrefabs)
        {
            prefab.Search();
        }
        Debug.Log(string.Format("StartRebuild 数量:{0},用时:{1:F1}s", MeshPrefabs.Count, (DateTime.Now - start).TotalSeconds));
    }

    private IEnumerator StartRebuild()
    {
        //MeshHelper.showLog = false;
        DateTime start = DateTime.Now;
        foreach (var prefab in MeshPrefabs)
        {
            yield return prefab.StartReplaceTargets();
        }
        Debug.LogWarning(string.Format("StartRebuild 数量:{0},用时:{1:F1}s", MeshPrefabs.Count, (DateTime.Now - start).TotalSeconds));
    }

    private void InitPrefabs()
    {
        Prefabs.Clear();
        if (PrefabRoot == null)
        {
            PrefabRoot = new GameObject();
            PrefabRoot.transform.parent = this.transform;
            PrefabRoot.name = "Prefabs";
        }
        if(UnitRoot!=null)
            for(int i=0;i< UnitRoot.transform.childCount; i++)
            {
                Transform t = UnitRoot.transform.GetChild(i);
                GameObject item = t.gameObject;
                Prefabs.Add(item);

                //AutomaticLOD automaticLOD = item.GetComponent<AutomaticLOD>();
                //if (automaticLOD == null)
                //{
                //    automaticLOD = item.AddComponent<AutomaticLOD>();
                //}

                //CreateDefaultLODS(3, automaticLOD, false);
                //UltimateGameTools.MeshSimplifier.Simplifier.Cancelled = false;
                //automaticLOD.ComputeLODData(true, null);

                //GameObject prefab = new GameObject();
                //prefab.transform.parent = PrefabRoot.transform;
                //prefab.name = item.name + "_Prefab";
                //MeshPrefab meshPrefab = prefab.AddComponent<MeshPrefab>();
                //meshPrefab.Prefab = item;
                //meshPrefab.TargetScene = TargetScene;
                //MeshPrefabs.Add(meshPrefab);
            }
    }
    public List<NodeInfoAsset> NodeInfoAssets = new List<NodeInfoAsset>();

    public NodeInfoAsset GetNodeInfoAsset(string assetName)
    {
        return NodeInfoAssets.Find(i => i!=null && i.name == assetName);
    }

    [ContextMenu("LoadJson")]
    public void LoadJson()
    {
        SetSceneName(SceneName);
        StartCoroutine(LoadJsonInner(false, null));
    }

    //private 

    [ContextMenu("LoadScene")]
    public void LoadScene()
    {
        SetSceneName(SceneName);
        StartCoroutine(LoadJsonInner(true, null));
    }

    private void SetSceneName(string sName)
    {
        SceneName = sName;
        ScenePath = "RvtScenes/" + sName + "/scene";
        ScenePartsPath = "RvtScenes/" + sName + "/SceneParts/";
        SceneRevitPath= "RvtScenes/" + sName + "/RevitElementGroup";
    }

    public void LoadScene(string sceneName, Action<GameObject> afterLoad)
    {
        SetSceneName(sceneName);
        StartCoroutine(LoadJsonInner(true, afterLoad));
    }

    public void LoadTypes(string sceneName, Action<GameObject> afterLoad)
    {
        SetSceneName(sceneName);
        StartCoroutine(LoadJsonInner(true, afterLoad));
    }

    public string SceneName = "PoliceStation";

    //public NodeInfoAsset NodeAsset;

    public string ScenePath = "";
    public string ScenePartsPath = "";
    public string SceneRevitPath = "";

    private IEnumerator LoadJsonInner(bool isParse, Action<GameObject> afterLoad)
    {
        Debug.LogError(string.Format("LoadJsonInner ScenePath:{0}", ScenePath));

        InitBeforeParseJson();
        print("IsLoadUnits:" + IsLoadUnits);
        NodeInfo node = null; 

        //if (NodeAsset != null) //加载大概需要4s的时间 40m的文件
        //{
        //    node = NodeAsset.Node;
        //}
        //else
        {
            node= LoadNodeInfoFromResource(ScenePath, null, null);
        }

        

        if (IsUpdateChildren)//有新的子场景时需要手动放开这部分代码
        {
            LoadJsonListFromResource(ScenePartsPath, lst =>
            {
                List<NodeInfo> allSubSceneNodes = UpdateSubNodeInfo(node, lst);//更新子场景信息
                NodeInfoAsset.Save(nodeInfo);//保存起来，下次就不用更新了

                if (isParse)
                {
                    CreateNodeInfoModels(node, allSubSceneNodes);
                }
                else
                {
                    Debug.Log(node.ToString());
                }

                if (afterLoad != null)
                {
                    Debug.Log("回调1");
                    afterLoad(newPrefabsParent);
                }
                else
                {
                    Debug.Log("无回调1");
                }
            });
        }
        else
        {
            if (isParse)
            {
                CreateNodeInfoModels(node, new List<NodeInfo>());
            }
            else
            {
                Debug.Log(node.ToString());
            }

            if (afterLoad != null)
            {
                Debug.Log("回调2");
                afterLoad(newPrefabsParent);
            }
            else
            {
                Debug.Log("无回调2");
            }
        }
        //}
        yield return null;
    }

    private string Base64ToString(string base64)
    {
        byte[] bytes=Convert.FromBase64String(base64);
        string txt = Encoding.UTF8.GetString(bytes);
        return txt;
    }
    public NodeInfo nodeInfo;

    public Dictionary<string, GameObject> resourcePrefabs = new Dictionary<string, GameObject>();

    public Dictionary<string, GameObject> instancePrefabs = new Dictionary<string, GameObject>();

    public bool IsLOD;

    public void HidePrefabs()
    {
        foreach (var item in instancePrefabs)
        {
            GameObject prefab = item.Value;
            if (prefab != null)
            {
                prefab.SetActive(false);
            }

        }
    }

    public void ShowPrefabs()
    {
        foreach (var item in instancePrefabs)
        {
            GameObject prefab = item.Value;
            if (prefab != null)
            {
                prefab.SetActive(true);
            }
        }
    }

    public void DestoryPrefabs()
    {
        //foreach (var item in instancePrefabs)
        //{
        //    GameObject prefab = item.Value;
        //    if (prefab != null)
        //    {
        //        prefab.SetActive(true);
        //    }
        //}
    }

    

    private GameObject GetPrefabInstance(NodeTypeInfo nodeTypeInfo)
    {
        string prefabName = nodeTypeInfo.typeName;
        if (!instancePrefabs.ContainsKey(prefabName))
        {
            GameObject prefab = GetPrefab(nodeTypeInfo,false);
            if (prefab != null)
            {
                nodeTypeInfo.SetVertexCount(prefab);

                GameObject prefabInstance = GameObject.Instantiate(prefab);
                prefabInstance.transform.parent = UnitRoot.transform;
                instancePrefabs[prefabName] = prefabInstance;

                //prefabInstance.AddComponent<MeshCollider>();
            }
            else
            {
                instancePrefabs[prefabName] = null;
            }
        }
        return instancePrefabs[prefabName];
    }

    public GameObject TestPrefab;

    public bool UseTestPrefab = true;

    private string GetDetailPrefabName(string prefabName)
    {
        int level = DetailLevel;
        string levelTxt = "";
        if (level == 0)//高质量
        {
            levelTxt = "";
        }
        else if (level == 1)//一般质量
        {
            levelTxt = "_lod1";
        }
        else if (level == 2)//低质量
        {
            levelTxt = "_lod2";
        }
        else if (level == 3)//超低质量
        {
            levelTxt = "_lod3";
        }

        //string modelPath = "RvtScenes/" + SceneName + "/Models/Units/" + prefabName + levelTxt;
        return prefabName + levelTxt;
    }

    private string GetLodPrefabName(string prefabName)
    {
        int level = DetailLevel;
        string levelTxt = "";
        if (level == 0)//高质量
        {
            levelTxt = "_lod";
        }
        else if (level == 1)//一般质量
        {
            levelTxt = "_lod_low";
        }
        else if (level == 2)//低质量
        {
            levelTxt = "_lod_low";
        }
        else if (level == 3)//超低质量
        {
            levelTxt = "_lod_low";
        }

        //string modelPath = "RvtScenes/" + SceneName + "/Models/Units/" + prefabName + levelTxt;
        return prefabName + levelTxt;
    }

    public bool IsTest = true;

    private GameObject GetPrefab(NodeTypeInfo nodeTypeInfo,bool setVertCount)
    {
        if (UseTestPrefab)
        {
            if (TestPrefab == null)
            {
                TestPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                
            }
            return TestPrefab;
        }

        string typeName = nodeTypeInfo.GetTypeName(IsTest);
        string prefabName = GetDetailPrefabName(typeName); //根据不同的质量设置，修改名称 

        GameObject prefab = null;
        if (resourcePrefabs.ContainsKey(typeName))
        {
            prefab = resourcePrefabs[typeName];
        }
        else
        {
            prefab = Prefabs.Find(i => i.name == typeName); //在Scene中的预设
            if (prefab == null)
            {
                //string modelPath = "RvtScenes/" + SceneName + "/Models/Units/" + prefabName;
                //print("modelPath:" + modelPath);
                GameObject orig = null;

                GameObject lodObj = null;
                GameObject preObj = null;
                prefab = orig;

                if (IsLOD)
                {
                    string lodName = GetLodPrefabName(typeName);
                    string lodPath = "RvtScenes/" + SceneName + "/Models/UnitsLOD/" + lodName;
                    lodObj = Resources.Load<GameObject>(lodPath);
                    if (lodObj != null)
                    {
                        prefab = lodObj;
                        print("lodPath:" + lodPath);
                        nodeTypeInfo.isLod = true;
                    }
                    else
                    {
                        print("noLOD lodPath:" + lodPath);

                        string prefabPath = "RvtScenes/" + SceneName + "/Models/UnitsPrefab/" + prefabName;
                        preObj = Resources.Load<GameObject>(prefabPath);
                        if (preObj != null)
                        {
                            prefab = preObj;
                            //print("prefab2:" + modelPath);
                        }
                        else
                        {
                            string modelPath = "RvtScenes/" + SceneName + "/Models/Units/" + prefabName;
                            //print("modelPath:" + modelPath);
                            orig = Resources.Load<GameObject>(modelPath);
                            prefab = orig;
                        }
                    }
                }
                else
                {
                    string prefabPath = "RvtScenes/" + SceneName + "/Models/UnitsPrefab/" + prefabName;
                    preObj = Resources.Load<GameObject>(prefabPath);
                    if (preObj != null)
                    {
                        prefab = preObj;
                        //print("prefab2:" + modelPath);
                    }
                    else
                    {
                        string modelPath = "RvtScenes/" + SceneName + "/Models/Units/" + prefabName;
                        //print("modelPath:" + modelPath);
                        orig = Resources.Load<GameObject>(modelPath);
                        prefab = orig;
                    }
                }
               
                if (prefab == null)
                {
                    //Debug.LogWarning("找不到预设 prefabPath:" + prefabPath);
                }
                else
                {
                    //Debug.Log("增加预设 prefabP:" + prefabName);
                }
                resourcePrefabs[typeName] = prefab;

                if (setVertCount)
                {
                    if (orig != null)
                    {
                        nodeTypeInfo.SetVertexCount(orig);
                    }
                    else if (preObj != null)
                    {
                        nodeTypeInfo.SetVertexCount(preObj);
                    }
                    else if (lodObj != null)
                    {
                        nodeTypeInfo.SetVertexCount(lodObj);
                    }
                }
            }
            else
            {
                nodeTypeInfo.SetVertexCount(prefab);
                resourcePrefabs[typeName] = prefab;
            }

            if (prefab == null)
            {
                if (SubSceneNodeTypes.Contains(typeName))
                {
                    //Debug.Log("找不到模型:" + prefabName);
                }
            }
        }
        return prefab;
        //return null;
    }

    public GameObject newPrefabsParent = null;

    public GameObject root = null;


    ////public bool IsECS1 { get; internal set; }

    ////public bool IsECS2 { get; internal set; }

    public int ModelType { get; set; }

    public bool IsLoadUnits { get; internal set; }
    public bool IsLoadFrame { get; internal set; }
    public int TypeMaxCount { get; internal set; }
    public int DetailLevel { get; internal set; }

    public void Clear()
    {
        if (root != null)
        {
            GameObject.DestroyImmediate(root);
            root = null;
        }

#if UNITY_ECS
        foreach (SpawnerPrefabScript s in spawnerList)
        {
            s.Clear();
            GameObject.Destroy(s);
        }
        spawnerList.Clear();


        if (ModelType==1)
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entities = entityManager.GetAllEntities();
            foreach (var item in entities)
            {
                entityManager.DestroyEntity(item);
            }
        }
#endif

        resourcePrefabs.Clear();

        foreach (var item in instancePrefabs)
        {
            GameObject.Destroy(item.Value);
        }
        instancePrefabs.Clear();
    }

    private void InitRoot()
    {
        if (root == null)
        {
            root = new GameObject();
            root.name = "Root";
            root.transform.SetParent(this.transform);
        }
    }

    private void InitPrefabParent()
    {
        newPrefabsParent = new GameObject();
        newPrefabsParent.name = SceneName;
        newPrefabsParent.transform.SetParent(root.transform);
    }

    private void LoadSceneFrame()
    {
        print("IsLoadFrame:" + IsLoadFrame);
        if (IsLoadFrame)
            CreateScenFrame();
    }

    private void InitBeforeParseJson()
    {
        InitPrefabs();

        InitRoot();
        InitPrefabParent();

        LoadSceneFrame();
    }

    //public GameObject

    public int CopyCount = 3;

    public Vector3 CopyOffset = new Vector3(-100, 0, 0);

#if UNITY_ECS
    private List<SpawnerPrefabScript> spawnerList = new List<SpawnerPrefabScript>();
#endif

    public GameObject centerObject;

    public GameObject FpsObject;

    public List<string> SubSceneNodeTypes = new List<string>();

    private List<NodeInfo> UpdateSubNodeInfo(NodeInfo nodeInfo, List<NodeInfo> subJsons)
    {
        DateTime start = DateTime.Now;
        List<NodeInfo> allSubSceneNodes = new List<NodeInfo>();
        print("UpdateChildren开始");
        foreach (var subNode in subJsons)
        {
            //string txt = sub.text;
            //NodeInfo subNode = JsonMapper.ToObject<NodeInfo>(txt);
            var subChildren = subNode.GetAllChildren(true,false);
            //print("subChildren:" + subChildren.Count);
            //print("subChildren1:" + subChildren[1]);
            allSubSceneNodes.AddRange(subChildren);
            List<NodeInfo> updateNodes = nodeInfo.UpdateChildren(subChildren);
            //break;
            //print("updateNodes:" + updateNodes.Count);
            //print("updateNodes1:" + updateNodes[1]);

            foreach (var item in subChildren)
            {
                string nodeType = item.typeName;
                if (string.IsNullOrEmpty(nodeType))
                {
                    print("nodeType == null:" + item.nodeName);
                    continue;
                }
                if (!SubSceneNodeTypes.Contains(nodeType))
                {
                    SubSceneNodeTypes.Add(nodeType);
                }
            }
            //break;
        }
        print("SubSceneNodeTypes:" + SubSceneNodeTypes.Count);
        foreach (var nodeType in SubSceneNodeTypes)
        {
            //print("nodeType:" + nodeType);
        }
        var time = DateTime.Now - start;
        print("UpdateChildren结束 用时:"+ time);
        return allSubSceneNodes;
    }

    private void SetCenter(Vector3 center)
    {
        if (centerObject)
            centerObject.transform.position = center;
        if (FpsObject)
            FpsObject.transform.position = center;
    }

    public bool IsUpdateChildren = false;

    private void CreateNodeInfoModels(NodeInfo nodeInfo, List<NodeInfo> allSubSceneNodes)
    {
        var start = DateTime.Now;
        print("ParseJson");
        //Debug.Log(json);

        //File.WriteAllText("D:\\a.json", json);
        //print("开始解析json");
        //nodeInfo =JsonMapper.ToObject<NodeInfo>(json.text);
        //print("解析json结束");

        Vector3 center= nodeInfo.GetCenter();
        SetCenter(center);


        if (CopyCount > 0)
        {
            nodeInfo.CopyChildren(CopyCount, CopyOffset * 1000);
        }
       
        Debug.Log(nodeInfo.nodeName);
        var list = nodeInfo.GetAllChildren(true,false);
        //list = allSubSceneNodes;


        print("list:" + list.Count);
        print("children:" + nodeInfo.children.Count);

        print("IsLoadUnits:" + IsLoadUnits);
        print("ModelType:" + ModelType);


        //NodeTypeInfoList typeList = new NodeTypeInfoList(allSubSceneNodes);
        //print("allSubSceneNodes:" + allSubSceneNodes.Count);

        //print("type1:" + typeList[1]);

        //return;
        //LoadUnitPrefabs(list);
        if (IsLoadUnits || isLoadTypes) //加载类型
        {
            LoadUnitPrefabs(list);
        }

        ElementGroupList elementGroupList = LoadElementGroupFromResourceEx("RvtScenes/"+SceneName, null, null);

        var categoryList = LoadCategoryListFromResource();


        

        if (IsLoadUnits) //加载模型
        {
            if (elementGroupList == null)
            {
                CreateUnitPrefabsByTypeList();
            }
            else
            {
                ModelBuildingInfo buildingInfo = new ModelBuildingInfo(list, elementGroupList, categoryList);
                CreateUnitPrefabsByBuilding(buildingInfo);

                if (SceneFrame != null)
                {
                    MoveGameObjectToLevel(SceneFrame, elementGroupList);
                }
                if (SceneSubFrames != null)
                {
                    MoveGameObjectToLevel(SceneSubFrames, elementGroupList);
                }

                modelBuildingScript.CombineLevel(SceneName);//室外地坪和1F合并;女儿墙和RF合并
            }
            //

            
        }

        

        

        var maxMin=ColliderHelper.GetBoundsMaxMinPoints(newPrefabsParent);
        Debug.LogError("maxMin:" + maxMin[0]+","+ maxMin[1]+","+ maxMin.Count);


        var time = DateTime.Now - start;
        Debug.LogError("ParseJson 完成，用时:" + time);
        //string newJson = JsonMapper.ToJson(nodeInfo);
        //Debug.Log(newJson);
    }

    private void MoveGameObjectToLevel(GameObject root, ElementGroupList elementGroupList)
    {
        if (elementGroupList == null)
        {
            Debug.LogWarning("MoveGameObjectToLevel elementGroupList == null");
            return;
        }
        Transform[] transforms = root.GetComponentsInChildren<Transform>();
        foreach (var item in transforms)
        {
            string n = item.name;
            string id = NodeInfo.GetId(n);
            if (id == n)
            {
                continue;//空物体
            }
            var ele = elementGroupList.GetElementInfo(id,n);
            if (ele != null)
            {
                var lvName = ele.LevelName;
                if (!string.IsNullOrEmpty(lvName))
                {
                    ModelLevelScript levelScript = modelBuildingScript.GetLevel(lvName);
                    //item.SetParent(levelObj);
                    item.gameObject.AddComponent<NodeInfoScript>();
                    levelScript.AddObject(item);
                }
                else
                {
                    if (ele.FamilyName == "焊缝") //焊缝没关系，不显示就是了
                    {
                        Debug.LogWarning("LevelName == null :" + ele);
                    }
                    else
                    {
                        Debug.LogError("LevelName == null :" + ele);
                    }

                }
            }
            else
            {
                Debug.LogError("ElementInfo == null :" + item);
            }
        }

        //Transform[] transforms2 = root.GetComponentsInChildren<Transform>();
        //foreach (var item in transforms2)
        //{
        //    //if (item.childCount == 0 && item.GetComponent<MeshFilter>() == null)
        //    //{
        //    //    GameObject.DestroyImmediate(item.gameObject);
        //    //}
        //    //GameObject.DestroyImmediate(item.gameObject);
        //    print("剩下:"+item);
        //}

        //if (root.transform.childCount == 0)
        //{
        //    //GameObject.DestroyImmediate(root);
        //}
    }

    private void CreateUnitPrefabsByBuilding(ModelBuildingInfo buildingInfo)
    {
        var start = DateTime.Now;

        print("CreateUnitPrefabs Start" + " typeList1:" + typeList.Count);
        if (typeList == null) return;
        ShowPrefabs();

        IsLOD = false;
        if (ModelType == 0)
        {
            //CreateObjects(list, false, true);//Normal
            CreateObjectsByBuilding(buildingInfo, false);
            HidePrefabs();
        }
        else if (ModelType == 1)
        {
            IsLOD = true;
            //CreateObjects(list, false, true);//Normal(LOD)
            CreateObjectsByBuilding(buildingInfo, false);
            HidePrefabs();
        }
        else if (ModelType == 2)//ConvertToEntity
        {
            //CreateObjects(list, true, true);
            CreateObjectsByBuilding(buildingInfo, true);
            HidePrefabs();
        }
        else if (ModelType == 3)//CreateEntity
        {
            CreateEntitiesByBuilding(buildingInfo, false);
        }
        else if (ModelType == 4)//CreateEntity(LOD)
        {
            IsLOD = true;
            CreateEntitiesByBuilding(buildingInfo, false);
        }
        else if (ModelType == 5)//CreateEntity(Mesh)
        {
            CreateEntitiesByBuilding(buildingInfo, true);
        }
        else
        {
            CreateEntitiesByBuilding(buildingInfo, false);
            HidePrefabs();
        }

        var time = DateTime.Now - start;
        print("CreateUnitPrefabs End 用时:" + time);
    }

    private void CreateEntitiesByBuilding(ModelBuildingInfo buildingInfo, bool byMesh)
    {
#if UNITY_ECS
        foreach (var item in spawnerList)
        {
            GameObject.Destroy(item);
        }
        spawnerList.Clear();
#endif


        //int count = 0;
        //for (int i = startTypeId; i < tl.Count; i++)
        //{
        //    if (TypeMaxCount > 0 && i >= TypeMaxCount)
        //    {
        //        Debug.Log("CreateEntities break:" + TypeMaxCount);
        //        break;
        //    }
        //    NodeTypeInfo typeInfo = tl[i];
        //    count = CreateEntitiesByTypeListItem(typeInfo, byMesh, count);
        //}

        modelBuildingScript = newPrefabsParent.GetComponent<ModelBuildingScript>();
        if (modelBuildingScript == null)
        {
            modelBuildingScript = newPrefabsParent.AddComponent<ModelBuildingScript>();
        }
        modelBuildingScript.Info = buildingInfo;

        for (int i1 = 0; i1 < buildingInfo.Levels.Count; i1++)
        {
            var level = buildingInfo.Levels[i1];
            GameObject levelObj = new GameObject();
            levelObj.name = level.Name;
            levelObj.transform.SetParent(newPrefabsParent.transform);

            foreach (var typeInfo in level.TypeList)
            {
                CreateEntitiesByTypeListItem(typeInfo, byMesh, 0);
            }
        }
    }

    ModelBuildingScript modelBuildingScript;

    private void CreateObjectsByBuilding(ModelBuildingInfo buildingInfo, bool isConvert)
    {
        Debug.Log("CreateObjectsByBuilding Start:" + buildingInfo.Levels.Count);

        modelBuildingScript = newPrefabsParent.GetComponent<ModelBuildingScript>();
        if (modelBuildingScript == null)
        {
            modelBuildingScript = newPrefabsParent.AddComponent<ModelBuildingScript>();
        }
        modelBuildingScript.Info = buildingInfo;
        modelBuildingScript.Levels.Clear();

        for (int i1 = 0; i1 < buildingInfo.Levels.Count; i1++)
        {
            var level = buildingInfo.Levels[i1];
            GameObject levelObj = new GameObject();
            ModelLevelScript levelScript = levelObj.AddComponent<ModelLevelScript>();
            levelScript.Info = level;
            modelBuildingScript.Levels.Add(levelScript);

            levelObj.name = level.Name;
            levelObj.transform.SetParent(newPrefabsParent.transform);

            foreach (ModelCategory category in level.CategoryList)
            {
                GameObject categoryObj = new GameObject();
                categoryObj.name = category.Info.ToString();
                categoryObj.transform.SetParent(levelObj.transform);
                ModelCategoryScript categoryScript=categoryObj.AddComponent<ModelCategoryScript>();
                categoryScript.Cate = category;
                levelScript.Categories.Add(categoryScript);
                CreateObjectsByTypeList(category.TypeList, isConvert, categoryObj);
            }
        }
        Debug.Log("CreateObjectsByBuilding End");
    }

    public bool isLoadTypes = false;

    public NodeTypeInfoList typeList;

    public string GetStatisticInfo()
    {
        if (typeList != null)
        {
            return typeList.GetStatisticInfo(startTypeId, TypeMaxCount);
        }
        return null;
    }

    private void LoadUnitPrefabs(List<NodeInfo> list)
    {
        var start = DateTime.Now;
        print("LoadUnitPrefabs Start");
        typeList = new NodeTypeInfoList(list);
        print("typeList:" + typeList.Count);

        IsLOD = false;
        if (ModelType == 0)
        {
        }
        else if (ModelType == 1)
        {
            IsLOD = true;
        }
        else if (ModelType == 2)//ConvertToEntity
        {
        }
        else if (ModelType == 3)//CreateEntity
        {
        }
        else if (ModelType == 4)//CreateEntity(LOD)
        {
            IsLOD = true;
        }
        else if (ModelType == 5)//CreateEntity(Mesh)
        {
        }
        else
        {
        }

        foreach (var typeInfo in typeList)
        {
            GameObject prefab = GetPrefab(typeInfo, true);
            //if (prefab != null)
            //{
            //    typeInfo.SetVertexCount(prefab);
            //}
            //else
            //{

            //}
        }

        typeList.SetPercent();
        typeList.Sort();

        string txt = "";
        for (int i = 0; i < 10 && i<typeList.Count; i++)
        {
            txt += string.Format("[{0}]:{1}\n", (i + 1), typeList[i]);
            //Debug.Log(string.Format("[{0}]:{1}",(i+1),typeList[i]));
        }
        Debug.Log(txt);

        var time = DateTime.Now - start;
        print("LoadUnitPrefabs End 用时:"+ time);
    }

    private void CreateUnitPrefabsByTypeList()
    {
        var start = DateTime.Now;

        print("CreateUnitPrefabs Start"+ " typeList1:" + typeList.Count);
        if (typeList == null) return;
        ShowPrefabs();

        IsLOD = false;
        if (ModelType == 0)
        {
            //CreateObjects(list, false, true);//Normal
            CreateObjectsByTypeList(typeList, false,newPrefabsParent);
            HidePrefabs();
        }
        else if (ModelType == 1)
        {
            IsLOD = true;
            //CreateObjects(list, false, true);//Normal(LOD)
            CreateObjectsByTypeList(typeList, false, newPrefabsParent);
            HidePrefabs();
        }
        else if (ModelType == 2)//ConvertToEntity
        {
            //CreateObjects(list, true, true);
            CreateObjectsByTypeList(typeList, true, newPrefabsParent);
            HidePrefabs();
        }
        else if (ModelType == 3)//CreateEntity
        {
            CreateEntitiesByTypeList(typeList, false);
        }
        else if (ModelType == 4)//CreateEntity(LOD)
        {
            IsLOD = true;
            CreateEntitiesByTypeList(typeList, false);
        }
        else if (ModelType == 5)//CreateEntity(Mesh)
        {
            CreateEntitiesByTypeList(typeList, true);
        }
        else
        {
            CreateObjectsByTypeList(typeList, false, newPrefabsParent);
            HidePrefabs();
        }

        var time = DateTime.Now - start;
        print("CreateUnitPrefabs End 用时:" + time);
    }

    private void CreateEntitiesByTypeList(NodeTypeInfoList tl,bool byMesh)
    {
#if UNITY_ECS
        foreach (var item in spawnerList)
        {
            GameObject.Destroy(item);
        }
        spawnerList.Clear();
#endif

        int count = 0;
        for (int i = startTypeId; i < tl.Count; i++)
        {
            if (TypeMaxCount > 0 && i >= TypeMaxCount)
            {
                Debug.Log("CreateEntities break:" + TypeMaxCount);
                break;
            }
            NodeTypeInfo typeInfo = tl[i];
            count=CreateEntitiesByTypeListItem(typeInfo, byMesh, count);
        }
    }

    private int CreateEntitiesByTypeListItem(NodeTypeInfo typeInfo, bool byMesh,int count)
    {
        GameObject prefab = GetPrefabInstance(typeInfo);
        if (prefab != null)
        {
#if UNITY_ECS
            SpawnerPrefabScript spawner = gameObject.AddComponent<SpawnerPrefabScript>();
            spawner.MaxCount = ModelMaxCount;

            spawnerList.Add(spawner);

            spawner.Prefab = prefab;
            spawner.nodes = typeInfo.nodes;
            spawner.AutoCreate = true;
            spawner.CreateByEntityMesh = byMesh;
#endif
            count += typeInfo.nodes.Count;
            if (ModelMaxCount > 0 && count > ModelMaxCount)
            {
                return count;
            }
        }
        return count;
    }

    GameObject SceneFrame;

    GameObject SceneSubFrames;

    private void CreateScenFrame()
    {
        Debug.Log("CreateScenFrame Start");
        string sceneObjPath = "RvtScenes/" + SceneName + "/Models/Frame";

        Debug.Log("sceneObjPath:" + sceneObjPath);
        GameObject sceneObjPrefab = Resources.Load<GameObject>(sceneObjPath);
        if (sceneObjPrefab != null)
        {
            GameObject sceneObj = GameObject.Instantiate(sceneObjPrefab);
            sceneObj.name = sceneObjPrefab.name;
            sceneObj.transform.parent = newPrefabsParent.transform;

            SceneFrame = sceneObj;
        }

        string partScenePath= "RvtScenes/" + SceneName + "/Models/Frames/";
        GameObject[] subScenObjs = Resources.LoadAll<GameObject>(partScenePath);
        Debug.Log("subScenObjs:" + subScenObjs);

        if (subScenObjs != null)
        {
            GameObject subSceneRoot = new GameObject();
            subSceneRoot.name = "SubFrames";
            subSceneRoot.transform.SetParent(newPrefabsParent.transform);

            foreach (var item in subScenObjs)
            {
                GameObject subScene = GameObject.Instantiate(item);
                subScene.name = item.name;
                subScene.transform.SetParent(subSceneRoot.transform);
            }

            SceneSubFrames = subSceneRoot;
        }

        Debug.Log("CreateScenFrame End");
    }

    public int ModelMaxCount = 100;

    public int currentCount = 0;

    public int startTypeId = 1;
        
    private void CreateObjectsByTypeList(NodeTypeInfoList tl, bool isConvert, GameObject typeObjParent)
    {
        Debug.Log("CreateObjectsByType Start:"+ tl.Count);

        int count = 0;
        for (int i1 = startTypeId; i1 < tl.Count; i1++)
        {
            if (TypeMaxCount > 0 && i1 >= TypeMaxCount)
            {
                Debug.Log("CreateObjectsByType break:" + TypeMaxCount);
                break;
            }
            NodeTypeInfo typeInfo = tl[i1];
            count = CreateObjectsByTypeItem(typeInfo, isConvert, count, typeObjParent);
        }
        Debug.Log("CreateObjectsByType End");
    }

    public Dictionary<NodeTypeInfo, GameObject> TypeInfoObjects = new Dictionary<NodeTypeInfo, GameObject>();

    //public float PositionPower = 10f;

    private int CreateObjectsByTypeItem(NodeTypeInfo typeInfo, bool isConvert, int count,GameObject typeObjParent)
    {
        //Point3Info.Power = PositionPower;

        GameObject prefab = GetPrefabInstance(typeInfo);
        if (typeInfo.nodes.Count ==0 || prefab == null || typeObjParent == null) return count;

        GameObject typeObject = new GameObject();
        typeObject.name = typeInfo.typeName;
        typeObject.transform.SetParent(typeObjParent.transform);
        //Debug.Log("CreateObjectsByTypeItem typeInfo:" + typeInfo.typeName);
        

        if (TypeInfoObjects.ContainsKey(typeInfo))
        {
            GameObject oldTypeObjects = TypeInfoObjects[typeInfo];
            GameObject.Destroy(oldTypeObjects);
            TypeInfoObjects[typeInfo] = typeObject;
        }
        else
        {
            TypeInfoObjects.Add(typeInfo, typeObject);
        }

        if (prefab != null)
        {
            //Debug.Log("CreateObjectsByTypeItem typeInfo.nodes:" + typeInfo.nodes.Count);
            int c = 0;
            for (int i = 0; i < typeInfo.nodes.Count; i++)
            {
                var child = typeInfo.nodes[i];
                count++;
                //Debug.Log("CreateObjects child:" + child);
                if (ModelMaxCount > 0 && count > ModelMaxCount)
                {
                    Debug.LogError("count > ModelMaxCount："+ ModelMaxCount);
                    break;
                }

                GameObject newObj = GameObject.Instantiate(prefab);
                NodeInfoScript nodeInfoScript = newObj.AddComponent<NodeInfoScript>();
                nodeInfoScript.NodInfo = child;
                newObj.name = child.nodeName;
                child.SetTransform(newObj);
                newObj.transform.SetParent(typeObject.transform);
#if UNITY_ECS
                if (isConvert)
                    newObj.AddComponent<ConvertToEntity>();
#endif

                c++;
            }

            //Debug.Log("CreateObjectsByTypeItem typeInfo.nodes:" + c);
        }
        else
        {
            Debug.LogError("CreateObjectsByTypeItem prefab == null:" + typeInfo.typeName);
        }

        return count;
    }

    //private void CreateObjects(IEnumerable<NodeInfo> children,bool isConvert,bool isRoot)
    //{
    //    if (isRoot)
    //    {
    //        Debug.Log("CreateObjects Start");
    //        if (children != null)
    //        {
    //            Debug.Log("CreateObjects children:"+ children);
    //        }
    //    }
    //    if (children != null)
    //    {
    //        int count = 0;
    //        foreach (NodeInfo child in children)
    //        {
    //            GameObject prefab = GetPrefabInstance(child.typeName);
    //            if (prefab != null)
    //            {
    //                count++;
    //                //Debug.Log("CreateObjects child:" + child);
    //                if (maxCount > 0 && count > maxCount)
    //                {
    //                    break;
    //                }

    //                GameObject newObj = GameObject.Instantiate(prefab);
    //                newObj.name = child.nodeName;
    //                child.SetTransform(newObj);
    //                newObj.transform.SetParent(newPrefabsParent.transform);
    //                if(isConvert)
    //                    newObj.AddComponent<ConvertToEntity>();
    //            }
    //            else
    //            {

    //                //Debug.LogError("prefab==null:"+ child.typeName);
    //            }

    //            CreateObjects(child.children, isConvert,false);//递归
    //        }
    //    }
    //    if (isRoot)
    //        Debug.Log("CreateObjects End");
    //}

    private IEnumerator LoadJsonFromAssets(string path,Action<string> jsonCallback)
    {
        //string filePath = Application.streamingAssetsPath + "/SceneJson/max2.json";
        string filePath = Application.streamingAssetsPath + path;
        WWW www = new WWW("file://" + filePath);
        yield return www;
        string json = www.text;
        Debug.Log(json);
        if (jsonCallback != null)
        {
            jsonCallback(json);
        }
        yield return json;
    }

    private CategoryInfoList LoadCategoryListFromResource()
    {
        //ScenePath = "RvtScenes/" + sName + "/scene";
        string path= "RvtScenes/Categories";
        Debug.LogError(string.Format("LoadCategoryListFromResource path:{0}", path));
        //string filePath = "SceneJson/max2";//不能加上文件名后缀.json
        string filePath = path;
        TextAsset txt = Resources.Load(filePath) as TextAsset;
        CategoryInfoList list = new CategoryInfoList(txt.text);

        return list;
    }

    private NodeInfo LoadNodeInfoFromResource(string path, TextAsset txt,string partName)
    {
        Debug.LogError(string.Format("LoadJsonFromResource SceneName:{0}", SceneName));
        //string filePath = "SceneJson/max2";//不能加上文件名后缀.json
        string filePath = path;
        string abPath = path + "_ab";
        if(partName==null)
            Debug.Log("LoadJsonFromResource:"+ filePath);

        NodeInfoAsset asset = GetNodeInfoAsset(SceneName + partName);
        if(asset==null)
            asset = Resources.Load<NodeInfoAsset>(abPath);//LoadAsync
        NodeInfo nodeInfo;
        if (asset == null)
        {
            if(txt==null)
                txt = Resources.Load(filePath) as TextAsset;
            //Debug.Log("txt:" + txt);
            nodeInfo = JsonMapper.ToObject<NodeInfo>(txt.text);

            //Debug.Log("nodeInfo:" + nodeInfo);
            //Debug.Log("Application.dataPath:" + Application.dataPath);
#if UNITY_EDITOR
            string assetPath = "Assets/Resources/" + path + "_ab.asset";
            NodeInfoAsset.Save(nodeInfo,SceneName+partName, assetPath);
#endif
        }
        else
        {
            nodeInfo = asset.Node;
        }

        NodeInfoAsset.Set(nodeInfo, SceneName + partName, "Assets/Resources/" + path + "_ab.asset");
        return nodeInfo;
    }

    public static ElementGroupList LoadElementGroupFromResourceEx(string dirPath, TextAsset txt, string partName)
    {
        string filePath1 = dirPath + "/RevitElementGroup";
        ElementGroupList elementGroups = LoadElementGroupFromResource(filePath1, txt, partName);
        string filePath2 = dirPath + "/RevitElementGroup2";
        ElementGroupList elementGroups2 = LoadElementGroupFromResource(filePath2, txt, partName);//链接的Revit文件里面的信息
        if(elementGroups!=null)
        {
            elementGroups.AddList(elementGroups2);
        }
        
        return elementGroups;
    }

        public static  ElementGroupList LoadElementGroupFromResource(string path, TextAsset txt, string partName)
    {
        Debug.LogError(string.Format("LoadJsonFromResource path:{0}", path));
        //string filePath = "SceneJson/max2";//不能加上文件名后缀.json
        string filePath = path;
        string abPath = path + "_ab";
        if (partName == null)
            Debug.Log("LoadJsonFromResource:" + filePath);

        //NodeInfoAsset asset = GetNodeInfoAsset(SceneName + partName);
        //if (asset == null)
        //    asset = Resources.Load<NodeInfoAsset>(abPath);//LoadAsync
        ElementGroupList nodeInfo;
        //if (asset == null)
        //{
            if (txt == null)
                txt = Resources.Load(filePath) as TextAsset;
            Debug.Log("txt:" + txt);
        if (txt == null) return null;
        Debug.Log("text:" + txt.text);
        nodeInfo = JsonMapper.ToObject<ElementGroupList>(txt.text);
        nodeInfo.VCount = nodeInfo.Count;

            //Debug.Log("nodeInfo:" + nodeInfo);
            //Debug.Log("Application.dataPath:" + Application.dataPath);
#if UNITY_EDITOR
        string assetPath = "Assets/Resources/" + path + "_ab.asset";
            //NodeInfoAsset.Save(nodeInfo, SceneName + partName, assetPath);
#endif
 
        return nodeInfo;
    }

    private void LoadJsonListFromResource(string dirPath, Action<List<NodeInfo>> jsonListCallback)
    {
#if UNITY_EDITOR
#endif
        //string filePath = "SceneJson/max2";//不能加上文件名后缀.json
        string filePath = dirPath;
        Debug.Log("读取 开始");
        TextAsset[] txtList = Resources.LoadAll<TextAsset>(filePath);
        List<NodeInfo> jsonList = new List<NodeInfo>();
        //foreach (var item in txtList)
        //{
        //    jsonList.Add(item);
        //    //Debug.Log(item.name);
        //}
        Debug.Log("读取 结束");
        foreach (var item in txtList)
        {
            string rPath = dirPath + item.name;
            //print(rPath);
            //NodeInfo node = JsonMapper.ToObject<NodeInfo>(item.text);
            NodeInfo node = LoadNodeInfoFromResource(rPath, item, item.name);
            jsonList.Add(node);
        }
        Debug.Log("解析 结束");
        if (txtList == null)
        {
            Debug.LogError("txtList == null:" + filePath);
            if (jsonListCallback != null)
            {
                jsonListCallback(null);
            }
            return;
        }
        if (jsonListCallback != null)
        {
            jsonListCallback(jsonList);
        }
    }

    [ContextMenu("InstiatePrefabs")]
    public void InstiatePrefabs()
    {

    }

    [ContextMenu("InstiateLODs")]
    public void InstiateLODs()
    {

    }
}
