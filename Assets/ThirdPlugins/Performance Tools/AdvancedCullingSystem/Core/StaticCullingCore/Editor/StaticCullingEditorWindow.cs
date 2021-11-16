using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using NGS.SuperLevelOptimizer;
using UnityEngine.Rendering;
using AdvancedCullingSystem.Core;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public enum AreasPlacing { Around_Static, Placing_By_User }

    public class StaticCullingEditorWindow : EditorWindow
    {
        public Vector2 size
        {
            get
            {
                return position.size;
            }
        }

        public class RenderInfo:IComparable<RenderInfo>
        {
            public MeshRenderer renderer;

            public Vector3 size;

            public float column;

            public RenderInfo(MeshRenderer ren)
            {
                this.renderer=ren;
                //this.size=ren.gameObject.GetSize();
                //column=size.x*size.y*size.z;
                this.size=ren.bounds.max-ren.bounds.min;
                this.column=this.size.magnitude;
                //Debug.Log($"{ren.name}:{ren.gameObject.GetSize()}|{ren.gameObject.GetLocalSize()}||{ren.gameObject.GetGlobalSize()}|{ren.gameObject.GetOriginalSizeByMeshFilter()}");
            }

            public int CompareTo(RenderInfo other)
            {
                return this.column.CompareTo(other.column);
            }
        }

        private int _selectedTool;
        private string[] _toolNames;
        private Action[] _toolGUIFuncs;
        private Vector2[] _scrolls;

        private List<Camera> _cameras = new List<Camera>();

        private List<MeshRenderer> _objects = new List<MeshRenderer>();

        private void AddObject(MeshRenderer r)
        {
            if (IsValidGameObject(r.gameObject))
                    if (!_objects.Contains(r))
                        {
                            _objects.Add(r);
                            // RenderInfo info=new RenderInfo(r);
                            // _renderInfos.Add(info);
                        }
        }

        private List<RenderInfo> _renderInfos=new List<RenderInfo>();

        //private List<MeshRenderer> _objects = new List<MeshRenderer>();

        private List<Collider> _occluders = new List<Collider>();
        private bool _showSelectedObjects = false;

        private bool _mustStatic = true;

        private string _minSize;

        private List<Transform> _areasTransforms = new List<Transform>();
        private List<Bounds> _areasBounds = new List<Bounds>();
        private AreasPlacing _areasPlacing;

        //private float _cellSize = 3f;
        //private int _jobsPerObject = 20;
        //private bool _fastBake;

        //private int _directionCount=2000;//cww_add

        //private bool _isOptimaizeTree=true;//cww_add
        //private bool _showCells = false;

        //private int _castersCount;
        //private float _bakingTime;

        private CullingSetting _cullingSetting = new CullingSetting();


        private GUIStyle _smallTextStyle;



        [MenuItem("Tools/NGSTools/Advanced Culling System/Static Culling")]
        public static void CreateCullingWindow()
        {
            var window = GetWindow<StaticCullingEditorWindow>(false, "Static Culling", true);

            window.minSize = new Vector2(270, window.minSize.y);             

            window.Show();
        }

        private void OnEnable()
        {
            _selectedTool = 0;

            _toolNames = new string[] { "Cameras", "Objects", "Occluders", "Areas" };
            _toolGUIFuncs = new Action[] { CamerasToolGUI, ObjectsToolGUI, OccludersToolGUI, AreasToolGUI };
            _scrolls = new Vector2[_toolNames.Length];


            _cullingSetting.CastersCount = 0;
            _cullingSetting.BakingTime = 0;

            _smallTextStyle = new GUIStyle()
            {
                fontSize = 9,
                padding = new RectOffset(5, 5, 0, 1),
            };

            SceneView.onSceneGUIDelegate += OnSceneGUI;

            LayersHelper.CreateLayer(ACSInfo.CullingLayerName);
        }


        private void CamerasToolGUI()
        {

            GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                EditorGUILayout.HelpBox("You can assign cameras now or make it later after baking", MessageType.Info);
                GUILayout.Space(10);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                GUILayout.Label("Cameras :");

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginVertical();

                    if (_cameras.Count > 0)
                    {
                        _scrolls[_selectedTool] = GUILayout.BeginScrollView(_scrolls[_selectedTool]);

                            for (int i = 0; i < _cameras.Count && i<20; i++)
                                EditorGUILayout.ObjectField(_cameras[i], typeof(Camera), false);

                        GUILayout.EndScrollView();
                    }
                    else
                        EditorGUILayout.HelpBox("Cameras non assigned", MessageType.Warning);

                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical();

                    if (GUILayout.Button("Add All Cameras")) AddAllCameras();
                    if (GUILayout.Button("Add Selected")) AddSelectedCameras();
                    if (GUILayout.Button("Remove Selected")) RemoveSelectedCameras();
                    if (GUILayout.Button("Remove All")) RemoveAllCameras();

                GUILayout.EndVertical();

                GUILayout.Space(10);

            GUILayout.EndHorizontal();
        }

        private void ObjectsToolGUI()
        {
            GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Assign static objects that are needed to be culled", MessageType.Info);
                GUILayout.Space(10);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                GUILayout.Label("Objects :");

            GUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

                GUILayout.Space(10);

                EditorGUILayout.BeginVertical();
         
                    if (_objects.Count > 0)
                    {
                        _scrolls[_selectedTool] = GUILayout.BeginScrollView(_scrolls[_selectedTool]);

                        for (int i = 0; i < _objects.Count && i<20; i++)
                        {
                            var renderer=_objects[i];
                            EditorGUILayout.ObjectField(renderer, typeof(GameObject), false);
                            //EditorGUILayout.LabelField("");
                        }

                    //      for (int i = 0; i < _renderInfos.Count && i<20; i++)
                    //     {
                    //         var info=_renderInfos[i];
                    //         var renderer=info.renderer;
                    // // EditorGUILayout.BeginHorizontal();
                    //         EditorGUILayout.ObjectField(renderer, typeof(GameObject), false);
                    //         EditorGUILayout.LabelField(info.column.ToString()+"|"+info.size.ToString());
                    // //         GUILayout.FlexibleSpace();
                    // // EditorGUILayout.EndHorizontal();
                    //     }

                        GUILayout.EndScrollView();

                        GUILayout.Space(10);
                    }
                    else
                        EditorGUILayout.HelpBox("Objects non assigned", MessageType.Warning);

                EditorGUILayout.EndVertical();


                GUILayout.FlexibleSpace();


                EditorGUILayout.BeginVertical();

                     _mustStatic = GUILayout.Toggle(_mustStatic, "Must Static");
                    if (GUILayout.Button("Add All")) AddStaticObjects();
                    if (GUILayout.Button("Add Selected")) AddSelectedObjects();
                    if (GUILayout.Button("Remove Selected")) RemoveSelectedObjects();
                    if (GUILayout.Button("Remove All")) RemoveAllObjects();
                    if (GUILayout.Button("Hide Others")) HideOthers();
                    if (GUILayout.Button("Hide Selected")) HideSelected();
                    if (GUILayout.Button("Show All")) ShowAll();
                    if (GUILayout.Button("Disable Renderers")) DisableRenderers();
                    if (GUILayout.Button("Enable Renderers")) EnableRenderers();
                    GUILayout.TextArea(_objects.Count+"");
                    _showSelectedObjects = GUILayout.Toggle(_showSelectedObjects, "Show Selected");

                     _minSize=GUILayout.TextArea(_minSize);
                     if (GUILayout.Button("FilterBySize")) FilterBySize();

                EditorGUILayout.EndVertical();

                GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
        }

        private void OccludersToolGUI()
        {
            GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Here You can add colliders thats only occlude other objects. Unity Terrain for example", MessageType.Info);
                GUILayout.Space(10);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                GUILayout.Label("Occluders :");

            GUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

                GUILayout.Space(10);

                EditorGUILayout.BeginVertical();
            
                    if (_occluders.Count > 0)
                    {
                        _scrolls[_selectedTool] = GUILayout.BeginScrollView(_scrolls[_selectedTool]);

                            for (int i = 0; i < _occluders.Count &&i<20; i++)
                                EditorGUILayout.ObjectField(_occluders[i], typeof(Collider), false);

                        GUILayout.EndScrollView();

                        GUILayout.Space(10);
                    }
                    else
                        EditorGUILayout.HelpBox("Occluders non assigned", MessageType.Warning);

                EditorGUILayout.EndVertical();


                GUILayout.FlexibleSpace();


                EditorGUILayout.BeginVertical();

                    if (GUILayout.Button("Add Selected")) AddSelectedOccluders();
                    if (GUILayout.Button("Remove Selected")) RemoveSelectedOccluders();
                    if (GUILayout.Button("Remove All")) RemoveAllOccluders();

                EditorGUILayout.EndVertical();

                GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
        }

        private void AreasToolGUI()
        {
            GUILayout.BeginHorizontal();

                GUILayout.Space(10);

                EditorGUILayout.HelpBox("Set up areas where cameras may be located", MessageType.Info);

                GUILayout.Space(10);

            GUILayout.EndHorizontal();



            GUILayout.BeginHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginVertical();

                    _areasPlacing = (AreasPlacing)EditorGUILayout.EnumPopup("Areas Placing :", _areasPlacing);

                    if (_areasPlacing == AreasPlacing.Placing_By_User)
                    {
                        GUILayout.Label("Areas :");

                        _scrolls[_selectedTool] = EditorGUILayout.BeginScrollView(_scrolls[_selectedTool]);

                            for (int i = 0; i < _areasTransforms.Count; i++)
                            {
                                if(_areasTransforms[i]==null){
                                    _areasTransforms.RemoveAt(i);
                                    i--;
                                    continue;
                                }
                                EditorGUILayout.BeginHorizontal();

                                    EditorGUILayout.ObjectField(_areasTransforms[i], typeof(Transform), false);
                                    Vector3 scale = _areasTransforms[i].lossyScale;
                                    EditorGUILayout.LabelField(scale.ToString(), new[] { GUILayout.Width(130) });
                                    if (GUILayout.Button("-")) RemoveCullingArea(i);

                                EditorGUILayout.EndHorizontal();
                            }

                        EditorGUILayout.EndScrollView();

                        GUILayout.BeginHorizontal();

                            if (GUILayout.Button("AddArea")) AddCullingArea();
                            if (GUILayout.Button("SelectArea")) AddSelectedAreas();

                         GUILayout.FlexibleSpace();

                            if (_areasTransforms.Count > 0)
                                if (GUILayout.Button("Clear"))
                                    RemoveAllCullingAreas();

                        GUILayout.EndHorizontal();
                        GUILayout.Space(20);
                    }

                    EditorGUILayout.BeginHorizontal();
                            _cullingSetting.CellSize = Mathf.Max(EditorGUILayout.DelayedFloatField("Cell Size", _cullingSetting.CellSize), 0.5f);
                            GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
            _cullingSetting.JobsPerObject = Mathf.Max(EditorGUILayout.DelayedIntField("Jobs Count Per Object", _cullingSetting.JobsPerObject), 1);
                        GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
            _cullingSetting.DirectionCount = Mathf.Max(EditorGUILayout.DelayedIntField("Direction Count", _cullingSetting.DirectionCount), 1);
                        GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

            _cullingSetting.FastBake = EditorGUILayout.Toggle("Fast Bake", _cullingSetting.FastBake);
            _cullingSetting._showCells = EditorGUILayout.Toggle("Show Cells", _cullingSetting._showCells);
            _cullingSetting.IsOptimaizeTree = EditorGUILayout.Toggle("Optimize Tree", _cullingSetting.IsOptimaizeTree);
                    BinaryTreeNode.ShowNodeCasters=EditorGUILayout.Toggle("Show Node's Casters", BinaryTreeNode.ShowNodeCasters);

                    GUILayout.Space(10);
;
                    GUILayout.Label("Casters count : " + _cullingSetting.CastersCount, _smallTextStyle);

                    GUILayout.BeginHorizontal();

                            GUILayout.BeginVertical();

                                GUILayout.Label("Baking time : ~" + _cullingSetting.BakingTime + " min." + "(Experimental)", _smallTextStyle);
                                if (GUILayout.Button("Calculate time")) CalculateBakingTime();

                            GUILayout.EndVertical();

                            GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }

        private int _bakeCount=1;

        private string _fileName = "Tree";

        private void OnGUI()
        {
            //UpdateAssignedData();

            _selectedTool = GUILayout.Toolbar(_selectedTool, _toolNames);
            _toolGUIFuncs[_selectedTool].Invoke();

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
                GUILayout.Space(10);

            if (IsDataBaked())
            {
                if (GUILayout.Button("Clear", GUILayout.Height(30), GUILayout.Width(60))) Clear();
            }

            _fileName = GUILayout.TextField(_fileName , GUILayout.Height(30), GUILayout.Width(60));
            if (GUILayout.Button("Save", GUILayout.Height(30), GUILayout.Width(60))) SaveData();
            if (GUILayout.Button("Load", GUILayout.Height(30), GUILayout.Width(60))) LoadData();
            if (GUILayout.Button("Update", GUILayout.Height(30), GUILayout.Width(60))) UpdateAssignedData();

            GUILayout.FlexibleSpace();

            
            if (GUILayout.Button("Bake", GUILayout.Height(30), GUILayout.Width(60))) Bake();

            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }



        private void AddAllCameras()
        {
            Camera[] cameras = FindObjectsOfType<Camera>();

            if (cameras.Length == 0)
            {
                Debug.Log("Cameras not found");
                return;
            }

            foreach (var camera in cameras)
                if (!_cameras.Contains(camera))
                    _cameras.Add(camera);
        }

        private void AddSelectedCameras()
        {
            foreach (var selected in Selection.gameObjects)
            {
                foreach (var camera in selected.GetComponentsInChildren<Camera>())
                    if (!_cameras.Contains(camera))
                        _cameras.Add(camera);
            }
        }

        private void RemoveSelectedCameras()
        {
            foreach (var selected in Selection.gameObjects)
            {
                foreach (var camera in selected.GetComponentsInChildren<Camera>())
                    if (_cameras.Contains(camera))
                        _cameras.Remove(camera);
            }
        }

        private void RemoveAllCameras()
        {
            _cameras.Clear();
        }

        private void HideOthers()
        {
            MeshRenderer[] array = FindObjectsOfType<MeshRenderer>();
            for (int i = 0; i < array.Length; i++)
            {
                float progress = i * 1 / array.Length;
                ProgressBarHelper.DisplayProgressBar("ShowAll", "HideOthers", progress);
                MeshRenderer renderer = array[i];
                if (_objects.Contains(renderer))
                {
                    //renderer.enabled = true;

                    renderer.ShowRenderer();
                }
                else
                {
                    //renderer.enabled = false;

                    renderer.HideRenderer();
                }
            }
            ProgressBarHelper.ClearProgressBar();
        }

        private void HideSelected()
        {
            foreach (var renderer in _objects)
            {
                //renderer.enabled = false;

                renderer.HideRenderer();
            }
        }

        private void ShowAll()
        {
            MeshRenderer[] array = FindObjectsOfType<MeshRenderer>();
            for (int i = 0; i < array.Length; i++)
            {
                float progress = i * 1 / array.Length;
                ProgressBarHelper.DisplayProgressBar("ShowAll", "DisableRenderers", progress);
                MeshRenderer renderer = array[i];
                //renderer.enabled = true;

                renderer.ShowRenderer();
            }
            ProgressBarHelper.ClearProgressBar();
        }

        public void FilterBySize()
        {
            float min=float.Parse(_minSize);
            for(int i=0;i<_renderInfos.Count;i++)
            {
                if(_renderInfos[i].column<min)
                {
                    _objects.Remove(_renderInfos[i].renderer);
                     _renderInfos.RemoveAt(i);
                    i--;
                }
            }
        }

        private void EnableRenderers()
        {
            MeshRenderer[] array = FindObjectsOfType<MeshRenderer>();
            for (int i = 0; i < array.Length; i++)
            {
                float progress = i * 1 / array.Length;
                ProgressBarHelper.DisplayProgressBar("Progress", "EnableRenderers", progress);
                MeshRenderer renderer = array[i];
                renderer.enabled = true;
            }
            ProgressBarHelper.ClearProgressBar();
        }

        private void DisableRenderers()
        {
            MeshRenderer[] array = FindObjectsOfType<MeshRenderer>();
            for (int i = 0; i < array.Length; i++)
            {
                float progress = i * 1 / array.Length;
                ProgressBarHelper.DisplayProgressBar("Progress", "DisableRenderers", progress);
                MeshRenderer renderer = array[i];
                renderer.enabled = false;
            }
            ProgressBarHelper.ClearProgressBar();
        }


        private void AddStaticObjects()
        {
            ProgressBarHelper.DisplayProgressBar("AddStaticObjects","AddStaticObjects",0);
            MeshRenderer[] array = FindObjectsOfType<MeshRenderer>();
            for (int i = 0; i < array.Length; i++)
            {
                MeshRenderer renderer = array[i];
                AddObject(renderer);
            }

            _renderInfos.Sort();
            ProgressBarHelper.ClearProgressBar();
        }

        private void AddSelectedObjects()
        {
            foreach (var selected in Selection.gameObjects)
            {
                foreach (var renderer in selected.GetComponentsInChildren<MeshRenderer>())
                    AddObject(renderer);
            }
            _renderInfos.Sort();
        }

        private void RemoveSelectedObjects()
        {
            foreach (var selected in Selection.gameObjects)
            {
                foreach (var renderer in selected.GetComponentsInChildren<MeshRenderer>())
                {
                    if (_objects.Contains(renderer))
                    {
                        _objects.Remove(renderer);
                        var info=_renderInfos.Find(i=>i.renderer==renderer);
                        if(info!=null){
                            _renderInfos.Remove(info);
                        }
                    }
                }
            }
        }

        private void RemoveAllObjects()
        {
            _objects.Clear();
            _renderInfos.Clear();
        }


        private void AddSelectedOccluders()
        {
            foreach (var selected in Selection.gameObjects)
            {
                Collider collider = selected.GetComponent<Collider>();

                if (collider != null)
                    _occluders.Add(collider);
            }
        }

        private void RemoveSelectedOccluders()
        {
            foreach (var selected in Selection.gameObjects)
            {
                Collider collider = selected.GetComponent<Collider>();

                if (collider != null && _occluders.Contains(collider))
                    _occluders.Remove(collider);
            }
        }

        private void RemoveAllOccluders()
        {
            _occluders.Clear();
        }


        private void AddCullingArea()
        {
            string name = "Culling Area " + (_areasTransforms.Count + 1);
            GameObject newGo=new GameObject(name);
            if(_cameras.Count==1)
            {
                // newGo.transform.position=_cameras[0].transform.position;
                var bounds=CasterUtility.CalculateBoundingBox(_objects);
                //bounds.Expand(new Vector3(3,3,3));
                newGo.transform.position=bounds.center;
                newGo.transform.localScale=bounds.size*3;
            }
            _areasTransforms.Add(newGo.transform);
        }

        private void AddSelectedAreas()
        {
            foreach (var selected in Selection.gameObjects)
            {
                if(!_areasTransforms.Contains(selected.transform))
                {
                    _areasTransforms.Add(selected.transform);
                }
            }
        }

        private void RemoveCullingArea(int index)
        {
            DestroyImmediate(_areasTransforms[index].gameObject);

            _areasTransforms.RemoveAt(index);
        }

        private void RemoveAllCullingAreas()
        {
            int count = _areasTransforms.Count;

            for (int i = 0; i < count; i++)
                RemoveCullingArea(0);
        }

        private void SaveData()
        {
            StaticCulling staticCulling = GameObject.FindObjectOfType<StaticCulling>();
            staticCulling.FileName = _fileName;
            staticCulling.SaveData();
        }


        private void LoadData()
        {
            Debug.Log("LoadData");
            StaticCulling staticCulling = GameObject.FindObjectOfType<StaticCulling>();
            if (staticCulling == null)
            {
                GameObject go = new GameObject("StaticCulling");
                staticCulling = go.AddComponent<StaticCulling>();
            }
            staticCulling.FileName = _fileName;
            staticCulling.LoadData();
        }

        private void Bake()
        {
            
           
            for(int i=0;i< _bakeCount;i++)
            {
                StaticCullingTestReporter.StartTest("Bake:" + i);
                Debug.LogWarning("Bake:"+i);
                if (!IsAssignedDataValid())
                {
                    Debug.LogError("IsAssignedDataValid == false");
                    return;
                }
                   
                 var _objects2 = GameObject.FindObjectsOfType<MeshRenderer>();//直接全部


                Clear();

                CalculateAreasBounds();

                //StaticCullingMaster cullingMaster = new StaticCullingMaster(_cameras.ToArray(), _objects.ToArray(), _occluders.ToArray(), 
                //    _areasBounds.ToArray(), _cullingSetting._fastBake, _cullingSetting._jobsPerObject, _cullingSetting._cellSize, _cullingSetting.layer, _cullingSetting._directionCount, _cullingSetting._isOptimaizeTree);

                StaticCullingTestReporter.Current.areaSize = _areasBounds[0].size;
                StaticCullingTestReporter.Current.ObjectsCount = _objects2.Length;
                StaticCullingMaster cullingMaster = new StaticCullingMaster(_cameras.ToArray(), _objects2.ToArray(), _occluders.ToArray(), _areasBounds.ToArray(), _cullingSetting);

                cullingMaster.Compute();
            }

            StaticCullingTestReporter.EndTest("Bake_"+ _bakeCount);
        }

        private void Clear()
        {
            StaticCulling.Clear();
        }



        private void UpdateAssignedData()
        {
            _cameras = _cameras.Where(c => c != null).ToList();
            _objects = _objects.Where(obj => obj != null && IsValidGameObject(obj.gameObject)).ToList();
            _occluders = _occluders.Distinct().Where(c => c != null).ToList();
            _areasTransforms = _areasTransforms.Where(area => area != null).ToList();
            _cullingSetting.CastersCount = CalculateCastersCount();
        }

        private bool IsAssignedDataValid()
        {
            // if (_objects.Count == 0)
            // {
            //     Debug.Log("No objects assign");
            //     return false;
            // }

            if (_areasPlacing == AreasPlacing.Placing_By_User && _areasTransforms.Count == 0)
            {
                Debug.Log("No areas assign");
                return false;
            }

            //int castersCount = CalculateCastersCount();
            //if (castersCount > 100000)
            //{
            //    Debug.Log("Casters count more then 100000");
            //    Debug.Log("Please decrease the 'Culling Area' or increase the 'Cell Size'");
            //    Debug.Log("This scene is too big. Computing this scene takes a very lot time");
            //    Debug.Log("I recommend You use Dynamic Culling. Because Dynamic Culling not requre preprocessing");
            //    Debug.Log("Or You can contact me(andre-orsk@yandex.ru) and I will help You to choose the best solution");
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// 计算出采样区域(Areas)的包围盒
        /// </summary>
        private void CalculateAreasBounds()
        {
            _areasBounds.Clear();

            if (_areasPlacing == AreasPlacing.Around_Static && _objects.Count > 0)
            {
                var bounds=CasterUtility.CalculateBoundingBox(_objects);

                _areasBounds.Add(bounds);//获取所有静态物体的包围盒
            }

            else if (_areasPlacing == AreasPlacing.Placing_By_User && _areasTransforms.Count > 0)
            {
                for (int i = 0; i < _areasTransforms.Count; i++)
                {
                    if(_areasTransforms[i]==null){
                        _areasTransforms.RemoveAt(i);
                        i--;
                        continue;
                    }
                    _areasBounds.Add(new Bounds(_areasTransforms[i].position, _areasTransforms[i].lossyScale));//获取手动指定区域的范围
                }
            }
        }

        private int CalculateCastersCount()
        {
            CalculateAreasBounds();

            return CasterUtility.CalculateCastersCount(_areasBounds, _cullingSetting.CellSize);
        }

        private void CalculateBakingTime()
        {
            if (!IsAssignedDataValid())
                return;

            CalculateAreasBounds();

            StaticCullingMaster cullingMaster = new StaticCullingMaster(_cameras.ToArray(), _objects.ToArray(), _occluders.ToArray(), _areasBounds.ToArray(), _cullingSetting);

            _cullingSetting.BakingTime = cullingMaster.CalculateComputingTime();
        }

        private bool IsDataBaked()
        {
            if (FindObjectOfType<StaticCulling>() != null || FindObjectOfType<BinaryTree>() != null)
                return true;

            return false;
        }

        private bool IsValidGameObject(GameObject go)
        {
            //Debug.Log("IsValidGameObject:" + go + "|" + (_mustStatic == false && !go.isStatic));
            if (_mustStatic==true && !go.isStatic)
                return false;

            MeshFilter filter = go.GetComponent<MeshFilter>();

            if (filter == null || filter.sharedMesh == null)
                return false;

            MeshRenderer renderer = go.GetComponent<MeshRenderer>();

            if (renderer == null)
                return false;

            //if (!renderer.enabled)//隐藏了的也要
            //    return false;

            return true;
        }

        private void OnDrawGizmos()
        {
            Debug.Log("StaticCullingEditorWindow.OnDrawGizmos");
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            //UpdateAssignedData();

            CalculateAreasBounds();//计算出物体的包围盒

            //UnityEditorHelper.IsGizmos=true;
            UnityEditorHelper.SetFunction(DrawFunction.OnSceneGUI);
            if (_showSelectedObjects && _selectedTool == 1)
            {
                UnityEditorHelper.color=Color.blue;
                foreach (var obj in _objects)
                {
                    Bounds bounds = obj.GetComponent<MeshRenderer>().bounds;

                    UnityEditorHelper.DrawWireCube(bounds.center, bounds.size);
                }
            }

            if (_selectedTool == 3)
            {
                for (int i = 0; i < _areasBounds.Count; i++)
                {
                    if (!_cullingSetting._showCells)
                    {
                        UnityEditorHelper.DrawWireCube(_areasBounds[i].center, _areasBounds[i].size,Color.blue);
                    }
                    else
                    {
                        UnityEditorHelper.color=Color.yellow;
                        foreach (var bounds in CasterUtility.CalculateCellsBounds(_areasBounds[i], _cullingSetting.CellSize))
                            UnityEditorHelper.DrawWireCube(bounds.center, bounds.size);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;

            RemoveAllCullingAreas();
        }
    }
}
