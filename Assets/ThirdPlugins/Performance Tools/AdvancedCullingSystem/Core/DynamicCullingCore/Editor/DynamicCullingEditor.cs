using System;
using UnityEngine;
using UnityEditor;
using CodeStage.AdvancedFPSCounter.Editor.UI;

namespace AdvancedCullingSystem.DynamicCullingCore
{
    [CustomEditor(typeof(DynamicCulling))]
    public class DynamicCullingEditor 
        //: Editor
        : BaseFoldoutEditor<DynamicCulling>
    {
        protected new DynamicCulling target
        {
            get
            {
                return base.target as DynamicCulling;
            }
        }

        private SerializedProperty _camerasProperty;
        private SerializedProperty _jobsPerFrameProperty;
        private SerializedProperty _objectsLifetimeProperty;
        private SerializedProperty _startRenderersProperty;
        private SerializedProperty _occludersProperty;

        private SerializedProperty UnitSizeProperty;
        private SerializedProperty FieldOfViewProperty;

        private string[] _toolbarLabels;
        private Action[] _toolbarGUIFuncs;
        private int _toolbarIdx;
        private Vector2 _renderersScroll;
        private Vector2 _occludersScroll;


        [MenuItem("Tools/NGSTools/Advanced Culling System/Dynamic Culling")]
        public static DynamicCulling CreateCullingInstance()
        {
            LayersHelper.CreateLayer(ACSInfo.CullingLayerName);

            DynamicCulling culling = FindObjectOfType<DynamicCulling>();

            if (culling == null)
                culling = new GameObject("Dynamic Culling").AddComponent<DynamicCulling>();

            Selection.activeGameObject = culling.gameObject;
            return culling;
        }

        public static DynamicCulling CreateCullingInstance(GameObject root)
        {
            LayersHelper.CreateLayer(ACSInfo.CullingLayerName);

            DynamicCulling culling = FindObjectOfType<DynamicCulling>();

            if (culling == null)
                culling = root.AddComponent<DynamicCulling>();

            Selection.activeGameObject = culling.gameObject;
            return culling;
        }

        private void OnEnable()
        {
            _camerasProperty = serializedObject.FindProperty("_cameras");
            _jobsPerFrameProperty = serializedObject.FindProperty("_jobsPerFrame");
            _objectsLifetimeProperty = serializedObject.FindProperty("_objectsLifetime");
            _startRenderersProperty = serializedObject.FindProperty("_startRenderers");
            _occludersProperty = serializedObject.FindProperty("_occluders");

            UnitSizeProperty = serializedObject.FindProperty("UnitSize");

            FieldOfViewProperty = serializedObject.FindProperty("FieldOfView");

            _toolbarLabels = new string[] { "Cameras", "Renderers", "Occluders" };
            _toolbarGUIFuncs = new Action[] { CamerasGUI, RenderersGUI, OccludersGUI };
            _toolbarIdx = 0;
        }


        public override void OnInspectorGUI()
        {
            _toolbarIdx = GUILayout.Toolbar(_toolbarIdx, _toolbarLabels);

            EditorGUILayout.Space();

            _toolbarGUIFuncs[_toolbarIdx].Invoke();
        }

        private void CamerasGUI()
        {
            EditorGUILayout.PropertyField(_camerasProperty, new GUIContent("Cameras"), true);

            _jobsPerFrameProperty.intValue = Mathf.Max(1, EditorGUILayout.IntField("Jobs Per Frame", _jobsPerFrameProperty.intValue));
            _objectsLifetimeProperty.floatValue = Mathf.Max(0.25f, EditorGUILayout.FloatField("Object Lifetime", _objectsLifetimeProperty.floatValue));
            UnitSizeProperty.intValue = Math.Max(1, EditorGUILayout.IntField("UnitSize", UnitSizeProperty.intValue));
            FieldOfViewProperty.intValue = Math.Max(30, EditorGUILayout.IntField("FieldOfView", FieldOfViewProperty.intValue));

            if (!Application.isPlaying)
                serializedObject.ApplyModifiedProperties();
        }

        private void RenderersGUI()
        {
            if (!Application.isPlaying)
            {
                if (GUILayout.Button("Add All Renderers")) target.OnEditorAddAllStartRenderers();
                if (GUILayout.Button("Remove All Renderers")) target.OnEditorRemoveAllStartRenderers();

                EditorGUILayout.Space();

                if (GUILayout.Button("Add Selected Renderers")) target.OnEditorAddSelectedStartRenderers();
                if (GUILayout.Button("Remove Selected Renderers")) target.OnEditorRemoveSelectedStartRenderers();
            }

            //GUILayout.Label(_startRenderersProperty.arraySize == 0 ? "No renderers assigned" : "Renderers :"+ _startRenderersProperty.arraySize);

            _renderersScroll = EditorGUILayout.BeginScrollView(_renderersScroll);

            //for (int i = 0; i < _startRenderersProperty.arraySize && i<25; i++)
            //    EditorGUILayout.ObjectField(_startRenderersProperty.GetArrayElementAtIndex(i), typeof(MeshRenderer));

            DrawRendererList(rendererListFoldoutArg, target);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

        }

        private void DrawRendererList(FoldoutEditorArg foldoutArg, DynamicCulling target)
        {
            foldoutArg.caption = $"Renderer List";
            var renderers= target.GetStartRenderers();
            EditorUIUtils.ToggleFoldout(foldoutArg,
            (arg) =>
            {
                System.DateTime start = System.DateTime.Now;
                var nodes = renderers;
                InitEditorArg(nodes);
                arg.caption = $"Renderer List({nodes.Count})";
                var time = System.DateTime.Now - start;
        },
           null);
            if (foldoutArg.isExpanded && foldoutArg.isEnabled)
            {
                foldoutArg.DrawPageToolbar(renderers, (r, i) =>
                {
                    var arg = FoldoutEditorArgBuffer.editorArgs[r];
                    arg.caption = r.name;
                    EditorUIUtils.ObjectFoldout(arg, r.gameObject, null);
                });
            }
        }

        FoldoutEditorArg rendererListFoldoutArg = new FoldoutEditorArg(true, false);

        private void OccludersGUI()
        {
            EditorGUILayout.HelpBox("Here You can add colliders thats only occlude other objects. Unity Terrain for example", MessageType.Info);

            if (_occludersProperty.arraySize != 0)
            {
                GUILayout.Label("Occluders :");

                _occludersScroll = EditorGUILayout.BeginScrollView(_occludersScroll);

                for (int i = 0; i < _occludersProperty.arraySize; i++)
                    EditorGUILayout.ObjectField(_occludersProperty.GetArrayElementAtIndex(i), typeof(Collider));

                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space();
            }

            if (!Application.isPlaying)
            {
                if (GUILayout.Button("Add Selected Colliders")) target.OnEditorAddSelectedOccluders();
                if (GUILayout.Button("Remove Selected Colliders")) target.OnEditorRemoveSelectedOccluders();
                if (GUILayout.Button("Remove All Occluders")) target.OnEditorRemoveAllOccluders();
            }
        }
    }
}
