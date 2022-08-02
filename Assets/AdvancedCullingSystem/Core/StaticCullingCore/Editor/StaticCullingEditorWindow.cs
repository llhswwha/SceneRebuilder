using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using CodeStage.AdvancedFPSCounter.Editor.UI;
using CommonUtils;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public enum AreasPlacing { Around_Static, Placing_By_User, PlacingAround, Around_Camera }

    public class StaticCullingEditorWindow : EditorWindow
    {
        

        static FoldoutEditorArg selectedObjectsArg = new FoldoutEditorArg(true, true,true,true,true);
        static FoldoutEditorArg selectedOccludersArg = new FoldoutEditorArg(true, true, true, true, true);

        public Vector2 size
        {
            get
            {
                return position.size;
            }
        }

        private int _selectedTool;
        private string[] _toolNames;
        private Action[] _toolGUIFuncs;
        private Vector2[] _scrolls;

        private List<Camera> _cameras = new List<Camera>();

        private List<MeshRenderer> _objects = new List<MeshRenderer>();
        private List<Collider> _occluders = new List<Collider>();
        private bool _showSelectedObjects = true;

        private List<Transform> _areasTransforms = new List<Transform>();
        private List<Bounds> _areasBounds = new List<Bounds>();
        private AreasPlacing _areasPlacing;
        private float _cellSize = 3f;
        private float _areaSize = 2f;
        private float _cellSplitCount = 1f;
        private float _cellVisibleDistance = 2f;
        private int _jobsPerObject = 20;
        private bool _fastBake;
        private bool _showCells = false;

        private int _castersCount;
        private float _bakingTime;

        private GUIStyle _smallTextStyle;



        [MenuItem("Tools/NGSTools/Advanced Culling System/Static Culling")]
        private static void CreateWindow()
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

            _castersCount = 0;
            _bakingTime = 0;

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

                            for (int i = 0; i < _cameras.Count; i++)
                                EditorGUILayout.ObjectField(_cameras[i], typeof(Camera), false);

                        GUILayout.EndScrollView();
                    }
                    else
                        EditorGUILayout.HelpBox("Cameras non assigned", MessageType.Warning);

                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical();

                    if (GUILayout.Button("AddMain")) AddMainCamera();
                    if (GUILayout.Button("AddAll")) AddAllCameras();
                    if (GUILayout.Button("AddSelected")) AddSelectedCameras();
                    if (GUILayout.Button("RemoveSelected")) RemoveSelectedCameras();
                    if (GUILayout.Button("RemoveAll")) RemoveAllCameras();

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
            _showSelectedObjects = GUILayout.Toggle(_showSelectedObjects, "Show Selected");
            IsMustStatic = GUILayout.Toggle(IsMustStatic, "MustStatic");
            IncludeBoundsBox = GUILayout.Toggle(IncludeBoundsBox, "BoundsBox");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("AddStatic")) AddStaticObjects();
            if (GUILayout.Button("AddSelected")) AddSelectedObjects();
            if (GUILayout.Button("RemSelected")) RemoveSelectedObjects();
            //if (GUILayout.Button("RemAll")) RemoveAllObjects();
            
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IncludeBuildingIn = GUILayout.Toggle(IncludeBuildingIn, "In");
            IncludeBuildingOut1 = GUILayout.Toggle(IncludeBuildingOut1, "Out1");
            IncludeBuildingLOD = GUILayout.Toggle(IncludeBuildingLOD, "LOD");
            IncludeBuildingOut0 = GUILayout.Toggle(IncludeBuildingOut0, "Out0");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("TreeNodes")) AddTreeNodes();
            if (GUILayout.Button("AllBuilds")) AddAllBuildings(false);
            //if (GUILayout.Button("BuildingAreas")) AddBuildingAreas();
            if (GUILayout.Button("SelectednBuilds")) AddSelectedBuildings();
            GUILayout.EndHorizontal();

            BaseEditorHelper.DrawObjectList(selectedObjectsArg, "Objects", _objects, null, null, null);


            //GUILayout.BeginHorizontal();
            //    GUILayout.Space(10);
            //    GUILayout.Label($"Objects ({_objects.Count}):");
            //GUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();

            //    GUILayout.Space(10);
            //    EditorGUILayout.BeginVertical();
            //        if (_objects.Count > 0)
            //        {
            //            _scrolls[_selectedTool] = GUILayout.BeginScrollView(_scrolls[_selectedTool]);

            //            for (int i = 0; i < _objects.Count; i++)
            //                EditorGUILayout.ObjectField(_objects[i], typeof(GameObject), false);

            //            GUILayout.EndScrollView();

            //            GUILayout.Space(10);
            //        }
            //        else
            //            EditorGUILayout.HelpBox("Objects non assigned", MessageType.Warning);
            //    EditorGUILayout.EndVertical();


            //    GUILayout.FlexibleSpace();

            //    EditorGUILayout.BeginVertical();

            //        if (GUILayout.Button("Add All Static")) AddStaticObjects();
            //        if (GUILayout.Button("Add Selected")) AddSelectedObjects();
            //        if (GUILayout.Button("Remove Selected")) RemoveSelectedObjects();
            //        if (GUILayout.Button("Remove All")) RemoveAllObjects();

            //        _showSelectedObjects = GUILayout.Toggle(_showSelectedObjects, "Show Selected");

            //    EditorGUILayout.EndVertical();

            //    GUILayout.Space(10);

            //EditorGUILayout.EndHorizontal();
        }

        private void OccludersToolGUI()
        {
            GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Here You can add colliders thats only occlude other objects. Unity Terrain for example", MessageType.Info);
                GUILayout.Space(10);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Selected")) AddSelectedOccluders();
            if (GUILayout.Button("Remove Selected")) RemoveSelectedOccluders();
            if (GUILayout.Button("Remove All")) RemoveAllOccluders();
            GUILayout.EndHorizontal();

            BaseEditorHelper.DrawObjectList(selectedOccludersArg, "Occluders", _occluders, null, null, null);

            //GUILayout.BeginHorizontal();
            //    GUILayout.Space(10);
            //    GUILayout.Label("Occluders :");
            //GUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //    GUILayout.Space(10);
            //    EditorGUILayout.BeginVertical();
            //        if (_occluders.Count > 0)
            //        {
            //            _scrolls[_selectedTool] = GUILayout.BeginScrollView(_scrolls[_selectedTool]);

            //                for (int i = 0; i < _occluders.Count; i++)
            //                    EditorGUILayout.ObjectField(_occluders[i], typeof(Collider), false);

            //            GUILayout.EndScrollView();

            //            GUILayout.Space(10);
            //        }
            //        else
            //            EditorGUILayout.HelpBox("Occluders non assigned", MessageType.Warning);
            //    EditorGUILayout.EndVertical();

            //    GUILayout.FlexibleSpace();

            //    EditorGUILayout.BeginVertical();

            //        if (GUILayout.Button("Add Selected")) AddSelectedOccluders();
            //        if (GUILayout.Button("Remove Selected")) RemoveSelectedOccluders();
            //        if (GUILayout.Button("Remove All")) RemoveAllOccluders();

            //    EditorGUILayout.EndVertical();

            //    GUILayout.Space(10);

            //EditorGUILayout.EndHorizontal();
        }

        private bool IsPlaceing()
        {
            return _areasPlacing == AreasPlacing.Placing_By_User || _areasPlacing == AreasPlacing.PlacingAround;
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

                    if (IsPlaceing())
                    {
                        GUILayout.Label("Areas :");

                        _scrolls[_selectedTool] = EditorGUILayout.BeginScrollView(_scrolls[_selectedTool]);

                            for (int i = 0; i < _areasTransforms.Count; i++)
                            {
                                EditorGUILayout.BeginHorizontal();

                                    EditorGUILayout.ObjectField(_areasTransforms[i], typeof(Transform), false);

                                    if (GUILayout.Button("-")) RemoveCullingArea(i);

                                EditorGUILayout.EndHorizontal();
                            }

                        EditorGUILayout.EndScrollView();

                        GUILayout.BeginHorizontal();

                            if (GUILayout.Button("Add")) AddCullingArea();
                            if (GUILayout.Button("Selected")) AddSelectedCullingArea();
                            if (GUILayout.Button("Buildings")) AddBuildingAreas();
                            if (GUILayout.Button("SelectedBuildings")) AddSelectedBuildingsAreas();
                            _areaSize = Mathf.Max(EditorGUILayout.DelayedFloatField("AreaSize", _areaSize), 1f);
                GUILayout.FlexibleSpace();

                            if (_areasTransforms.Count > 0)
                                if (GUILayout.Button("Clear"))
                                    RemoveAllCullingAreas();

                        GUILayout.EndHorizontal();
                        GUILayout.Space(20);
                    }

                    EditorGUILayout.BeginHorizontal();
                            _cellSize = Mathf.Max(EditorGUILayout.DelayedFloatField("Cell Size", _cellSize), 0.5f);
                            StaticCulling._cellSizeS = _cellSize;
                            GUILayout.FlexibleSpace();
                            _cellSplitCount = EditorGUILayout.DelayedFloatField("Cell Split Count", _cellSplitCount);
                            StaticCulling._cellSplitCountS = _cellSize;
                            GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    //EditorGUILayout.BeginHorizontal();
                    //_cellSplitCount = EditorGUILayout.DelayedFloatField("Cell Split Count", _cellSplitCount);
                    //StaticCulling._cellSplitCountS = _cellSize;
                    //GUILayout.FlexibleSpace();
                    //EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    _cellVisibleDistance = EditorGUILayout.DelayedFloatField("Cell Visible Distance", _cellVisibleDistance);
                    //StaticCulling._cellSplitCountS = _cellSize;
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                        _jobsPerObject = Mathf.Max(EditorGUILayout.DelayedIntField("Jobs Count Per Object", _jobsPerObject), 1);
                        GUILayout.FlexibleSpace();
                        CasterUtility.CastersBatch = Mathf.Max(EditorGUILayout.DelayedIntField("CastersBatch", CasterUtility.CastersBatch), 10);
                        GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _fastBake = EditorGUILayout.Toggle("Fast Bake", _fastBake);
            GUILayout.FlexibleSpace();
            if (_fastBake)
            {
                StaticCullingMaster.FastInitJobsCount = Mathf.Max(EditorGUILayout.DelayedIntField("JobsCount_F", StaticCullingMaster.FastInitJobsCount), 6000);
            }
            else
            {
                StaticCullingMaster.StandardInitJobsCount = Mathf.Max(EditorGUILayout.DelayedIntField("JobsCount_S", StaticCullingMaster.StandardInitJobsCount), 2000);
            }
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _showCells = EditorGUILayout.Toggle("Show Cells", _showCells);
            GUILayout.FlexibleSpace();
            StaticCullingMaster.RaysBatch = Mathf.Max(EditorGUILayout.DelayedIntField("RaysBatch", StaticCullingMaster.RaysBatch), 100000);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
;
                    GUILayout.Label("Casters count : " + _castersCount, _smallTextStyle);

                    GUILayout.BeginHorizontal();

                            GUILayout.BeginVertical();

                                GUILayout.Label("Baking time : ~" + _bakingTime + " min." + "(Experimental)", _smallTextStyle);
                                if (GUILayout.Button("Calculate time")) CalculateBakingTime();

                            GUILayout.EndVertical();

                            GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }

        private void OnGUI()
        {
            UpdateAssignedData();

            _selectedTool = GUILayout.Toolbar(_selectedTool, _toolNames);
            _toolGUIFuncs[_selectedTool].Invoke();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

                GUILayout.Space(10);

                if (IsDataBaked())
                    if (GUILayout.Button("Clear Data", GUILayout.Height(30), GUILayout.Width(80))) Clear();

                GUILayout.FlexibleSpace();
                    if (GUILayout.Button("AutoSet", GUILayout.Height(30), GUILayout.Width(80))) AutoSet();
                    if (GUILayout.Button("Bake", GUILayout.Height(30), GUILayout.Width(80))) Bake();

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

        private void AddMainCamera()
        {
            Camera camera = Camera.main;
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


        private void AddStaticObjects()
        {
            foreach (var renderer in FindObjectsOfType<MeshRenderer>())
                if (IsValidGameObject(renderer.gameObject))
                    if (!_objects.Contains(renderer))
                        _objects.Add(renderer);
        }

        private void AddTreeNodes()
        {
            var trees= GameObject.FindObjectsOfType<ModelAreaTree>(true);
            int leafCount = 0;
            foreach(var tree in trees)
            {
                foreach(var leaf in tree.TreeLeafs)
                {
                    leafCount++;
                    AddObjects(leaf.gameObject);
                }
            }
            Debug.Log($"AddTreeNodes trees:{trees.Length} _objects:{_objects.Count} ");
        }

        private bool IsBuildingHaveInPart(BuildingModelInfo[] floors)
        {
            foreach(var floor in floors)
            {
                if (floor.InPart != null && floor.InPart.transform.childCount > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void AutoSet()
        {
            AddAllBuildings(false);
            AddBuildingAreas();
            AddMainCamera();
           
        }

        private void AddSelectedBuildingsAreas()
        {
            List<BuildingModelInfo> allBuildings = new List<BuildingModelInfo>();
            List<BuildingController> buildings = new List<BuildingController>();
            var objs = Selection.gameObjects;
            foreach(var obj in objs)
            {
                allBuildings.AddRange(obj.GetComponentsInChildren<BuildingModelInfo>(true));
                buildings.AddRange(obj.GetComponentsInChildren<BuildingController>(true));
            }
            AddBuildingAreas(allBuildings, buildings);
        }

        private void AddBuildingAreas()
        {
            List<BuildingModelInfo> allBuildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true).ToList();
            List<BuildingController> buildings = GameObject.FindObjectsOfType<BuildingController>(true).ToList();

            AddBuildingAreas(allBuildings, buildings);
        }

        private void AddBuildingAreas(List<BuildingModelInfo> allBuildings, List<BuildingController> buildings)
        {
            //List<BuildingModelInfo> allBuildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true).ToList();
            //List<BuildingController> buildings = GameObject.FindObjectsOfType<BuildingController>(true).ToList();

            DateTime startT = DateTime.Now;
            _areasPlacing = AreasPlacing.PlacingAround;

            _areasTransforms.Clear();

            int floorCount = allBuildings.Count;
            int floorId = 0;
            for (int i = 0; i < buildings.Count; i++)
            {
                BuildingController building = buildings[i];
                BuildingModelInfo[] floors = building.GetComponentsInChildren<BuildingModelInfo>(true);
                if (floors.Length == 0) continue;
                //AddBuildings(building.Buildings);
                if (IsBuildingHaveInPart(floors) == false) continue;
                for (int j = 0; j < floors.Length; j++)
                {
                    BuildingModelInfo floor = floors[j];
                    if (floor == null) continue;
                    allBuildings.Remove(floor);
                }
                var bounds = GetBuildingBounds(building.gameObject);
                bounds.size *= _areaSize;
                AddCullingArea("CullingArea_" + building.name, bounds);
            }

            for (int i = 0; i < allBuildings.Count; i++)
            {
                BuildingModelInfo floor = allBuildings[i];
                floorId++;
                if (ProgressBarHelper.DisplayCancelableProgressBar("AddAllBuildings2", floorId, floorCount, $">{floor.name}"))
                {
                    break;
                }

                if (floor.InPart == null)
                {
                    continue;
                }
                if (floor.InPart.transform.childCount == 0)
                {
                    continue;
                }

                var bounds = GetBuildingBounds(floor.gameObject);
                bounds.size *= _areaSize;
                AddCullingArea("CullingArea_" + floor.name, bounds);
            }
            ProgressBarHelper.ClearProgressBar();

            Debug.LogError($"AddBuildingAreas time:{DateTime.Now - startT} buildings:{buildings.Count} allBuildings:{allBuildings.Count} _objects:{_objects.Count} _areasTransforms:{_areasTransforms.Count} ");
        }

        private void AddAllBuildings(bool isTest)
        {
            DateTime startT = DateTime.Now;

            _objects.Clear();
            //_areasTransforms.Clear();
            _occluders.Clear();

            var allBuildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true).ToList();
            var buildings = GameObject.FindObjectsOfType<BuildingController>(true);

            int floorCount = allBuildings.Count;
            //for (int i = 0; i < buildings.Length; i++)
            //{
            //    BuildingModelInfoList building = buildings[i];
            //    for (int j = 0; j < building.Buildings.Length; j++)
            //    {
            //        floorCount++;
            //    }
            //}

            int floorId = 0;
            for (int i = 0; i < buildings.Length; i++)
            {
                BuildingController building = buildings[i];
                BuildingModelInfo[] floors = building.GetComponentsInChildren<BuildingModelInfo>(true);
                if (floors.Length == 0) continue;
                //AddBuildings(building.Buildings);
                if (IsBuildingHaveInPart(floors) == false) continue;

                for (int j = 0; j < floors.Length; j++)
                {
                    BuildingModelInfo floor = floors[j];
                    if (floor == null) continue;
                    floorId++;
                    if (ProgressBarHelper.DisplayCancelableProgressBar("AddAllBuildings1", floorId, floorCount, $"{building.name}>{floor.name}"))
                    {
                        break;
                    }
                    if (isTest == false)
                    {
                        AddBuildings(floor);
                    }
                    
                    allBuildings.Remove(floor);
                }
                var bounds = GetBuildingBounds(building.gameObject);
                bounds.size *= 2;
                //if (isTest == false)
                //{
                //    AddCullingArea("CullingArea_" + building.name, bounds);
                //}
            }

            for (int i = 0; i < allBuildings.Count; i++)
            {
                BuildingModelInfo floor = allBuildings[i];
                floorId++;
                if (ProgressBarHelper.DisplayCancelableProgressBar("AddAllBuildings2", floorId, floorCount, $">{floor.name}"))
                {
                    break;
                }

                if (floor.InPart == null)
                {
                    continue;
                }
                if (floor.InPart.transform.childCount == 0)
                {
                    continue;
                }

                if (isTest == false)
                {
                    AddBuildings(floor);
                }
                //var bounds = GetBuildingBounds(floor.gameObject);
                //bounds.size *= 2;
                //if (isTest == false)
                //{
                //    AddCullingArea("CullingArea_" + floor.name, bounds);
                //}
            }
            ProgressBarHelper.ClearProgressBar();
            //var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
            //AddBuildings(buildings);

            Debug.LogError($"AddAllBuildings time:{DateTime.Now-startT} buildings:{buildings.Length} allBuildings:{allBuildings.Count} _objects:{_objects.Count} _areasTransforms:{_areasTransforms.Count} ");
        }

        private Bounds GetBuildingBounds(GameObject root)
        {
            Bounds bounds = ColliderExtension.CaculateBounds(root);
            return bounds;
        }

        private void AddSelectedBuildings()
        {
            var buildings = Selection.activeGameObject.GetComponentsInChildren<BuildingModelInfo>(true);
            AddBuildings(buildings);
        }

        private void AddBuildings(BuildingModelInfo[] buildings)
        {
            foreach (var building in buildings)
            {
                AddBuildings(building);
            }
            Debug.Log($"AddBuildings buildings:{buildings.Length} _objects:{_objects.Count} ");
        }

        public void AddBuildings(BuildingModelInfo building)
        {
            if (building == null) return;
            if (building.gameObject == null) return;

            GPUIRoot[] gpuis1 = building.GetComponentsInChildren<GPUIRoot>(true);
            SubScene_GPUI[] gpuis2 = building.GetComponentsInChildren<SubScene_GPUI>(true);
            if(gpuis1.Length>0 || gpuis2.Length > 0)
            {
                Debug.LogError($"AddBuildings gpuis1.Length>0 ({gpuis1.Length}) || gpuis2.Length > 0 ({gpuis2.Length}) building:{building.name}");
                return;
            }

            List<Transform> ts = new List<Transform>();
            building.InitInOut(false);
            if (IncludeBuildingIn)
            {
                AddObjects(building.InPart);
                if(building.InPart)
                    ts.Add(building.InPart.transform);
            }
            if (IncludeBuildingOut1)
            {
                AddObjects(building.OutPart1);
                if(building.OutPart1)
                    ts.Add(building.OutPart1.transform);
            }
            if (IncludeBuildingLOD)
            {
                AddObjects(building.LODPart);
                if(building.LODPart)
                    ts.Add(building.LODPart.transform);
            }
            //if (IncludeBuildingOut0)
            //{
            //    AddObjects(building.OutPart0);
            //}
            AddOccluders(building.OutPart0);
            if(building.OutPart0)
                ts.Add(building.OutPart0.transform);

            for (int i = 0; i < building.transform.childCount; i++)
            {
                Transform t = building.transform.GetChild(i);
                if (ts.Contains(t)) continue;
                //if (t.name.Contains("Doors"))
                //{
                //    AddObjects(t.gameObject);
                //}
                //else
                //{
                //    AddOccluders(t.gameObject);
                //}

                AddObjects(t.gameObject);
            }
        }

        private void AddSelectedObjects()
        {
            GameObject[] objs = Selection.gameObjects;
            Debug.Log($"AddSelectedObjects _objects:{_objects} objs:{objs.Length}");
            foreach (var selected in objs)
            {
                AddObjects(selected);
            }
        }

        private void AddObjects(GameObject root)
        {
            if (root == null) return;
            foreach (var renderer in root.GetComponentsInChildren<MeshRenderer>())
                if (IsValidGameObject(renderer.gameObject))
                    if (!_objects.Contains(renderer))
                        _objects.Add(renderer);
        }

        private void RemoveSelectedObjects()
        {
            foreach (var selected in Selection.gameObjects)
            {
                foreach (var renderer in selected.GetComponentsInChildren<MeshRenderer>())
                    if (_objects.Contains(renderer))
                        _objects.Remove(renderer);
            }
        }

        private void RemoveAllObjects()
        {
            _objects.Clear();
        }


        private void AddSelectedOccluders()
        {
            foreach (var selected in Selection.gameObjects)
            {
                Collider collider = selected.GetComponent<Collider>();

                if (collider != null && !_occluders.Contains(collider))
                    _occluders.Add(collider);
            }
        }

        private void AddOccluders(GameObject root)
        {
            Debug.LogError($"AddOccluders root:{root}");
            MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>(true);
            foreach(var renderer in renderers)
            {
                Collider collider = renderer.GetComponent<Collider>();
                if (collider == null)
                {
                    MeshCollider meshCollider = renderer.gameObject.AddComponent<MeshCollider>();
                    MeshFilter mf = renderer.GetComponent<MeshFilter>();
                    if (mf != null)
                    {
                        meshCollider.sharedMesh = mf.sharedMesh;
                    }
                    collider = meshCollider;
                }
            }


            Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
            foreach (var collider in colliders)
            {
                if (collider != null && !_occluders.Contains(collider))
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
            Bounds bounds=CalculateBoundingBox();
            //GameObject newArea = new GameObject(name);
            //if(_areasPlacing == AreasPlacing.PlacingAround)
            //{
            //    newArea.transform.position = bounds.center;
            //    newArea.transform.localScale = bounds.size;
            //}
            //_areasTransforms.Add(newArea.transform);
            AddCullingArea(name, bounds);
        }

        private GameObject CullingAreaRoot = null;

        private GameObject AddCullingArea(string name,Bounds bounds)
        {
            if (CullingAreaRoot == null)
            {
                CullingAreaRoot = GameObject.Find("CullingAreaRoot");
            }
            if (CullingAreaRoot == null)
            {
                CullingAreaRoot = new GameObject("CullingAreaRoot");
            }
            //string name = "Culling Area " + (_areasTransforms.Count + 1);
            //Bounds bounds = CalculateBoundingBox();
            GameObject newArea = GameObject.Find(name);
            if (newArea == null)
            {
                //newArea = new GameObject(name);
                newArea = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newArea.name = name;
                MeshRenderer meshRenderer = newArea.GetComponent<MeshRenderer>();
                if (meshRenderer)
                {
                    meshRenderer.enabled = false;
                }
                Collider collider = newArea.GetComponent<Collider>();
                if (collider)
                {
                    collider.enabled = false;
                }
            }
            StaticCullingArea cullingArea = newArea.GetComponent<StaticCullingArea>();
            if (cullingArea == null)
            {
                cullingArea = newArea.AddComponent<StaticCullingArea>();
            }

            //if (_areasPlacing == AreasPlacing.PlacingAround)
            {
                newArea.transform.position = bounds.center;
                newArea.transform.localScale = bounds.size;
            }
            _areasTransforms.Add(newArea.transform);
            newArea.transform.SetParent(CullingAreaRoot.transform);
            return newArea;
        }

        public void AddSelectedCullingArea()
        {
            GameObject newArea = Selection.activeGameObject;
            _areasTransforms.Add(newArea.transform);
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


        private void Bake()
        {
            DateTime start = DateTime.Now;
            if (!IsAssignedDataValid())
                return;

            Clear();

            CalculateAreasBounds();

            StaticCullingMaster cullingMaster = new StaticCullingMaster(_cameras.ToArray(), _objects.ToArray(), _occluders.ToArray(), 
                _areasBounds.ToArray(), _fastBake, _jobsPerObject, _cellSize, ACSInfo.CullingLayer,_cellSplitCount,_cellVisibleDistance);

            cullingMaster.Compute();

            Debug.Log($"Bake time:{DateTime.Now-start} cameras:{_cameras.Count} objects:{_objects.Count} occluders:{_occluders.Count} areasBounds:{_areasBounds.Count}");
        }

        private void Clear()
        {
            foreach (var culling in FindObjectsOfType<StaticCulling>())
                DestroyImmediate(culling.gameObject);

            foreach (var tree in FindObjectsOfType<BinaryTree>())
                DestroyImmediate(tree.gameObject);
        }



        private void UpdateAssignedData()
        {
            _cameras = _cameras.Where(c => c != null).ToList();
            _objects = _objects.Where(obj => obj != null && IsValidGameObject(obj.gameObject)).ToList();
            _occluders = _occluders.Distinct().Where(c => c != null).ToList();
            _areasTransforms = _areasTransforms.Where(area => area != null).ToList();
            _castersCount = CalculateCastersCount();
        }

        private bool IsAssignedDataValid()
        {
            if (_objects.Count == 0)
            {
                Debug.Log("No objects assign");
                return false;
            }

            if (IsPlaceing() && _areasTransforms.Count == 0)
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

        public Bounds CalculateBoundingBox()
        {
            return StaticCullingMaster.CalculateBoundingBox(_objects);
        }

        private void CalculateAreasBounds()
        {
            _areasBounds.Clear();

            if (_areasPlacing == AreasPlacing.Around_Static && _objects.Count > 0)
                _areasBounds.Add(CalculateBoundingBox());

            else if (IsPlaceing() && _areasTransforms.Count > 0)
            {
                for (int i = 0; i < _areasTransforms.Count; i++)
                    _areasBounds.Add(new Bounds(_areasTransforms[i].position, _areasTransforms[i].lossyScale));
            }
        }

        private int CalculateCastersCount()
        {
            CalculateAreasBounds();

            return StaticCullingMaster.CalculateCastersCount(_areasBounds, _cellSize);
        }

        private void CalculateBakingTime()
        {
            if (!IsAssignedDataValid())
                return;

            CalculateAreasBounds();

            StaticCullingMaster cullingMaster = new StaticCullingMaster(_cameras.ToArray(), _objects.ToArray(), _occluders.ToArray(),
                _areasBounds.ToArray(), _fastBake, _jobsPerObject, _cellSize, ACSInfo.CullingLayer, _cellSplitCount, _cellVisibleDistance);

            _bakingTime = cullingMaster.CalculateComputingTime();
        }

        private bool IsDataBaked()
        {
            if (FindObjectOfType<StaticCulling>() != null || FindObjectOfType<BinaryTree>() != null)
                return true;

            return false;
        }

        public bool IsMustStatic = false;

        public bool IncludeBoundsBox = false;

        public bool IncludeBuildingIn = true;

        public bool IncludeBuildingOut1 = true;

        public bool IncludeBuildingLOD = true;

        public bool IncludeBuildingOut0 = false;

        private bool IsValidGameObject(GameObject go)
        {
            if (IsMustStatic && !go.isStatic)
                return false;

            if (IncludeBoundsBox == false && go.GetComponent<BoundsBox>() != null)
            {
                return false;
            }

            MeshFilter filter = go.GetComponent<MeshFilter>();

            if (filter == null || filter.sharedMesh == null)
                return false;

            MeshRenderer renderer = go.GetComponent<MeshRenderer>();

            if (renderer == null || !renderer.enabled)
                return false;

            return true;
        }



        private void OnSceneGUI(SceneView sceneView)
        {
            UpdateAssignedData();

            CalculateAreasBounds();

            if (_showSelectedObjects && _selectedTool == 1)
            {
                Handles.color = Color.blue;
                foreach (var obj in _objects)
                {
                    Bounds bounds = obj.GetComponent<MeshRenderer>().bounds;

                    Handles.DrawWireCube(bounds.center, bounds.size);
                }
            }

            if (_selectedTool == 3)
            {
                for (int i = 0; i < _areasBounds.Count; i++)
                {
                    if (!_showCells)
                    {
                        Handles.color = Color.blue;
                        Handles.DrawWireCube(_areasBounds[i].center, _areasBounds[i].size);
                    }
                    else
                    {
                        Handles.color = Color.yellow;
                        foreach (var bounds in StaticCullingMaster.CalculateCellsBounds(_areasBounds[i], _cellSize))
                            Handles.DrawWireCube(bounds.center, bounds.size);
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
