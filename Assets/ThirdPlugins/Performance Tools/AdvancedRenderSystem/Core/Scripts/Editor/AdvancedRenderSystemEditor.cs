using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NGS.AdvancedRenderSystem
{
    [CustomEditor(typeof(AdvancedRenderSystem))]
    public class AdvancedRenderSystemEditor : Editor
    {
        protected AdvancedRenderSystem _target
        {
            get { return target as AdvancedRenderSystem; }
        }

        [MenuItem("Tools/NGSTools/AdvancedRenderSystem")]
        public static void CreateObject()
        {
            var ars = FindObjectOfType<AdvancedRenderSystem>();

            if (ars == null)
                ars = new GameObject("Advanced Render System").AddComponent<AdvancedRenderSystem>();

            Selection.activeGameObject = ars.gameObject;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            _target.UpdatesPerFrame = EditorGUILayout.IntField("Updates Per Frame : ", _target.UpdatesPerFrame);
            _target.ReplaceDistance = EditorGUILayout.FloatField("Replace Distance : ", _target.ReplaceDistance);
            _target.UpdateAngle = EditorGUILayout.FloatField("Update Angle : ", _target.UpdateAngle);

            EditorGUILayout.Space();

            if (GUILayout.Button("Open Manager Window"))
                AdvancedRenderSystemEditorWindow.OpenWindow(_target);
        }
    }
}
