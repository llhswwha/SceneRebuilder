using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ModelAssetLoaderSettingWindow : EditorWindow
{
    [MenuItem("Tools/ModelAsset/DeviceSetting...")]
    static void CreateWindow()
    {
        var window = (ModelAssetLoaderSettingWindow)EditorWindow.GetWindow(typeof(ModelAssetLoaderSettingWindow));
        window.Show();
    }

    ModelAssetLoader modelAssetLoader = null;

    private Vector2 scrollPos;

    public ColumnSortComponent sortComponent = new ColumnSortComponent();
    public TableComponent tableComponent = new TableComponent();

    private void GUI_InitTableComponent()
    {
        int idColumnWidth = 20;
        int countColumnWidth = 60;
        int nameColumnWidth = 70;
        int simpleColumnWidth = 100;
        tableComponent.Clear();
        tableComponent.AddColumn("ID", idColumnWidth);
        tableComponent.AddColumn("Count", countColumnWidth);
        tableComponent.AddColumn("Name", nameColumnWidth);
        tableComponent.AddColumn("Simple", simpleColumnWidth);
        tableComponent.AddColumn("Count2", countColumnWidth);
        tableComponent.OnGUI();
    }

    void OnGUI()
    {
        modelAssetLoader = GameObject.FindObjectOfType<ModelAssetLoader>();
        if (modelAssetLoader == null)
        {
            Debug.LogError("modelAssetLoader == null");
            return;
        }

        sortComponent.OnGUI();

        var root = GameObject.FindObjectOfType<ModelAssetLoader>();
        if (root)
        {
            var allCount = EditorHelper.GetVertexs(root.gameObject);
            GUILayout.Label("AllCount:" + allCount, EditorStyles.boldLabel);
        }

        var heightOffset = 100;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height - heightOffset));//开始滚动条

        GUI_InitTableComponent();

        var meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>().ToList();
        meshRenderers.Sort(sortComponent.SortMethod);
        //模型按从位置进行排序
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("{0:00}", i + 1), tableComponent.GetGUIWidth("ID"));
            MeshRenderer meshRenderer = meshRenderers[i];
            var obj = meshRenderer.gameObject;
            var vertexCount = EditorHelper.GetVertexs(obj);
            GUILayout.Label(vertexCount + "", tableComponent.GetGUIWidth("Count"));
            if (GUILayout.Button(obj.name, tableComponent.GetGUIWidth("Name")))
            {
                EditorHelper.SelectObject(obj);
            }

            GameObject simpleOjb = GetSimpleModel(obj);
            EditorGUILayout.ObjectField(simpleOjb, typeof(GameObject), false, tableComponent.GetGUIWidth("Simple"));

            var vertexCount2 = 0;
            if (simpleOjb != null)
            {
                vertexCount2 = EditorHelper.GetVertexs(simpleOjb);
                GUILayout.Label(vertexCount2 + "", tableComponent.GetGUIWidth("Count2"));
            }
            else
            {
                GUILayout.Label(vertexCount2 + "", tableComponent.GetGUIWidth("Count2"));
            }
            

            if (GUILayout.Button("CreatePrefab"))
            {
                CreatePrefab(obj,true, vertexCount <= vertexCount2);
            }
            //if (GUILayout.Button("Rename"))
            //{
            //    RenameAsset(obj);
            //}
            if(vertexCount > vertexCount2)
            {
                if (GUILayout.Button("ReplaceSimple"))
                {
                    ReplaceSimple(obj, simpleOjb);
                }
            }
            else
            {
                if (GUILayout.Button("------------"))
                {

                    //ReplaceSimple(obj, simpleOjb);
                }
            }
            
            //if (GUILayout.Button("ReplaceSimple"))
            //{
            //    //RenameAsset(obj);
            //}
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("CreatePrefab"))
        {
            modelAssetLoader.modelList.Clear();
            for (int i = 0; i < meshRenderers.Count; i++)
            {
                MeshRenderer meshRenderer = meshRenderers[i];
                var obj = meshRenderer.gameObject;
                //CreatePrefab(obj,false);

                GameObject simpleOjb = GetSimpleModel(obj);
                if (simpleOjb != null)
                {
                    var vertexCount = EditorHelper.GetVertexs(obj);
                    var vertexCount2 = EditorHelper.GetVertexs(simpleOjb);
                    if (vertexCount > vertexCount2)
                    {
                        //ReplaceSimple(obj, simpleOjb);
                         CreatePrefab(obj, false,false);//还没有替换简化模型
                    }
                    else
                    {
                        CreatePrefab(obj, false, true);//已经替换了建行模型
                    }
                }
                else
                {
                    CreatePrefab(obj, false, false);//不存在简化模型
                }
            }
        }
        if (GUILayout.Button("ReplaceSimple"))
        {
            
            for (int i = 0; i < meshRenderers.Count; i++)
            {
                MeshRenderer meshRenderer = meshRenderers[i];
                var obj = meshRenderer.gameObject;


                GameObject simpleOjb = GetSimpleModel(obj);
                if (simpleOjb != null)
                {
                    var vertexCount = EditorHelper.GetVertexs(obj);
                    var vertexCount2 = EditorHelper.GetVertexs(simpleOjb);
                    if(vertexCount> vertexCount2)
                    {
                        ReplaceSimple(obj, simpleOjb);
                    }
                }
            }
        }
        if (GUILayout.Button("RenameModel"))
        {
            for (int i = 0; i < meshRenderers.Count; i++)
            {
                MeshRenderer meshRenderer = meshRenderers[i];
                var obj = meshRenderer.gameObject;
                RenameAsset(obj);
            }
        }
        if (GUILayout.Button("RenameSimple"))
        {
            string path = "Assets/Models/SiHuiFactory/J1_Devices/简化模型";
            var guids = AssetDatabase.FindAssets("t:model", new string[] { path });//再找模型
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                //Debug.Log(assetPath);
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                //主厂房设备简化1.FBX
                RenameAsset(obj);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private static void ReplaceSimple(GameObject obj, GameObject simpleOjb)
    {
        if (simpleOjb != null)
        {
            EditorHelper.SelectObject(obj);

            //RenameAsset(obj);
            GameObject simpleInstance = GameObject.Instantiate(simpleOjb, obj.transform.parent);
            if (simpleInstance.transform.childCount == 1)//之前有问题的多了个分层的模型
            {
                var child = simpleInstance.transform.GetChild(0);
                child.transform.parent = obj.transform.parent;
                GameObject.DestroyImmediate(simpleInstance);
                simpleInstance = child.gameObject;
            }
            simpleInstance.transform.localPosition = obj.transform.localPosition;
            simpleInstance.transform.localRotation = obj.transform.localRotation;
            simpleInstance.transform.localScale = obj.transform.localScale;
            simpleInstance.name = obj.transform.name;

            GameObject.DestroyImmediate(obj);

            EditorHelper.SelectObject(simpleInstance);
        }
    }

    public List<GameObject> SimpleModels = new List<GameObject>();

    public GameObject GetSimpleModel(GameObject obj)
    {
        string modelName = EditorHelper.GetModelName(obj);
        GameObject simpleOjb = GetSimpleModel(modelName);
        return simpleOjb;
    }

    public GameObject GetSimpleModel(string objName)
    {
        if (SimpleModels.Count == 0)
        {
            string path = "Assets/Models/SiHuiFactory/J1_Devices/简化模型";
            var guids = AssetDatabase.FindAssets("t:model", new string[] { path });//再找模型
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                //Debug.Log(assetPath);
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                //主厂房设备简化1.FBX
                Debug.Log("GetSimpleModel:" + obj.name);
                SimpleModels.Add(obj);
            }
        }
        GameObject simple = SimpleModels.Find(i => i.name == objName);
        return simple;
    }

    private void CreatePrefab(GameObject obj, bool isSelect, bool isSimple)
    {
        string after = "";
        if (isSimple)
        {
            after = "_s";
        }
        if (obj == null) return;
        Debug.Log("CreatePrefab:" + obj.name);
        //1.找到原模型所在目录
        //2.使用当前游戏物体创建预设，在该模型位置
        var prefatPath = EditorHelper.GetPrefabPath(obj);
        Debug.Log("prefatPath:" + prefatPath);
        EditorHelper.makePrefab(prefatPath, obj);

        var modelName = EditorHelper.GetModelName(obj);
        Debug.Log("modelName:" + modelName);

        EditorHelper.SetAssetBundleName(prefatPath, modelName+ after);//3.将原模型和预设打包到assetbundle中

        if(isSelect)
            EditorHelper.SelectObject(obj);

        ModelInfo info = new ModelInfo();
        info.bundleName = modelName;
        info.assetName = obj.name + ".prefab";
        info.parentName = obj.transform.parent.name;
        info.isSimple = isSimple;
        modelAssetLoader.modelList.Add(info);
        //4.删除游戏物体

        var meshPath = EditorHelper.GetMeshPath(obj);
        EditorHelper.SetAssetBundleName(meshPath, modelName+ after);

        GameObject.DestroyImmediate(obj);
    }

    private static void RenameAsset(GameObject obj)
    {
        var path = EditorHelper.GetMeshPath(obj);//Assets/Models/SiHuiFactory/J1_Devices/主厂房设备8/主厂房设备8.FBX
        var modelName = EditorHelper.GetModelName(obj);
        Debug.Log("modelName:" + modelName);
        
        if (modelName.StartsWith("主厂房设备简化"))//主厂房设备简化1.FBX -> J1_F1_1
        {
            var newName = modelName.Replace("主厂房设备简化", "J1_F1_");
            Debug.Log("newName:" + newName);
            AssetDatabase.RenameAsset(path, newName);
        }
        else if (modelName.StartsWith("J1_F1_简化"))//J1_F1_简化1.FBX -> J1_F1_1
        {
            var newName = modelName.Replace("J1_F1_简化", "J1_F1_");
            Debug.Log("newName:" + newName);
            AssetDatabase.RenameAsset(path, newName);
        }
        if (modelName.StartsWith("主厂房设备"))
        {
            var newName = modelName.Replace("主厂房设备", "J1_F1_");
            Debug.Log("newName:" + newName);
            AssetDatabase.RenameAsset(path, newName);
        }
        else if (modelName.StartsWith("主厂房大设备"))
        {
            var newName = modelName.Replace("主厂房大设备", "J1_F2_");
            Debug.Log("newName:" + newName);
            AssetDatabase.RenameAsset(path, newName);
        }
        else
        {

        }
    }    
}
