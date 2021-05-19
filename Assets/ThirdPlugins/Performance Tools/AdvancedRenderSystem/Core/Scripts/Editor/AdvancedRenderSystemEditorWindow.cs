using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NGS.SuperLevelOptimizer;

namespace NGS.AdvancedRenderSystem
{
    public class AdvancedRenderSystemEditorWindow : EditorWindow
    {
        private static AdvancedRenderSystem _ars;

        private List<MeshRenderer> _renderers;
        private List<BillboardOfGroupList> _groupedRenderers;

        private Vector2 _billboardsScroll;
        private Vector2 _billboardsGroupScroll;
        private bool _showAssignedObjects;


        public static void OpenWindow(AdvancedRenderSystem ars)
        {
            _ars = ars;

            _ars.UpdateAssignedData();

            GetWindow<AdvancedRenderSystemEditorWindow>("Objects Manager");
        }

        private void OnEnable()
        {
            minSize = new Vector2(350, 350);

            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        private void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }


        private void OnGUI()
        {
            if (_ars == null)
            {
                Close();
                return;
            }
            
            LoadDataFromARS();

            try
            {
                DrawGUI();
            }
            catch (MissingReferenceException)
            {
                _ars.UpdateAssignedData();
            }
            catch(Exception ex)
            {
                Debug.Log(ex.Message);
                Close();
            }
        }

        private void DrawGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical();

                    GUILayout.FlexibleSpace();

                    DrawBillboardsBlock();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    DrawBillboardsGroupBlock();

                    GUILayout.FlexibleSpace();

                EditorGUILayout.EndVertical();

                DrawButtonsBlock();

                EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < 5; i++)
                EditorGUILayout.Space();
        }

        private void LoadDataFromARS()
        {
            _renderers = _ars.EditorGetRenderers();
            _groupedRenderers = _ars.EditorGetGroupedRenderers();
        }

        private void DrawBillboardsBlock()
        {
            GUILayout.Label("Billboards :");

            if (_renderers.Count == 0)
            {
                GUILayout.Label("not assigned");
                return;
            }

            EditorGUILayout.Space();

            _billboardsScroll = EditorGUILayout.BeginScrollView(_billboardsScroll);

            for (int i = 0; i < _renderers.Count; i++)
                EditorGUILayout.ObjectField(_renderers[i], typeof(MeshRenderer), false);

            EditorGUILayout.EndScrollView();
        }

        private void DrawBillboardsGroupBlock()
        {
            GUILayout.Label("Grouped Billboards :");

            if (_groupedRenderers.Count == 0)
            {
                GUILayout.Label("not assigned");
                return;
            }

            EditorGUILayout.Space();

            _billboardsGroupScroll = EditorGUILayout.BeginScrollView(_billboardsGroupScroll);

                for (int i = 0; i < _groupedRenderers.Count; i++)
                {
                    GUILayout.Label("Group :");

                for (int c = 0; c < _groupedRenderers[i].renderers.Count; c++)
                    EditorGUILayout.ObjectField(_groupedRenderers[i][c], typeof(MeshRenderer), false);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawButtonsBlock()
        {
            EditorGUILayout.BeginVertical();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add All Static")) _ars.EditorAddAllStaticRenderers();

                EditorGUILayout.Space();

                if (GUILayout.Button("Add Selected")) _ars.EditorAddSelectedRenderers();
                if (GUILayout.Button("Add Selected As Group")) _ars.EditorAddSelectedAsGroup();

                EditorGUILayout.Space();

                if (GUILayout.Button("Remove Selected")) _ars.EditorRemoveSelectedRenderers();
                if (GUILayout.Button("Remove All")) _ars.EditorRemoveAllRenderers();

                EditorGUILayout.Space();

                _showAssignedObjects = EditorGUILayout.ToggleLeft("Show Assigned Objects", _showAssignedObjects);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndVertical();
        }


        private void OnSceneGUI(SceneView sceneView)
        {
            if (!_showAssignedObjects)
                return;

            LoadDataFromARS();

            try
            {
                DrawSceneGUI();
            }
            catch (MissingReferenceException)
            {
                _ars.UpdateAssignedData();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                Close();
            }
        }

        private void DrawSceneGUI()
        {
            UnityEditorHelper.SetFunction(DrawFunction.OnSceneGUI);
            UnityEditorHelper.color=Color.blue;
            for (int i = 0; i < _renderers.Count; i++)
            {
                Bounds bounds = _renderers[i].bounds;
                UnityEditorHelper.DrawWireCube(bounds.center, bounds.size);
            }

            UnityEditorHelper.color=Color.yellow;
            for (int i = 0; i < _groupedRenderers.Count; i++)
            {
                Bounds bounds = _groupedRenderers[i].GetBounds();
                UnityEditorHelper.DrawWireCube(bounds.center, bounds.size);
            }
        }
    }
}
