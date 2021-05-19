using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NGS.SuperLevelOptimizer
{
    public class CoefficientTableEditorWindow : EditorWindow
    {
        public CoefficientTable table;
        
        public void OnGUI()
        {
            if (table == null)
            {
                Close();
                return;
            }

            titleContent = new GUIContent("Table");

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            table.floatThreshold = EditorGUILayout.FloatField("Float threshold : ", table.floatThreshold);

            EditorGUILayout.Space();

            Vector4 vectorValue = table.vectorThreshold;

            vectorValue.x = EditorGUILayout.FloatField("Vector threshold x : ", vectorValue.x);
            vectorValue.y = EditorGUILayout.FloatField("Vector threshold y : ", vectorValue.y);
            vectorValue.z = EditorGUILayout.FloatField("Vector threshold z : ", vectorValue.z);
            vectorValue.w = EditorGUILayout.FloatField("Vector threshold w : ", vectorValue.w);

            table.vectorThreshold = vectorValue;

            EditorGUILayout.Space();

            Color colorValue = table.colorThreshold;

            colorValue.r = EditorGUILayout.FloatField("Color threshold r : ", colorValue.r);
            colorValue.g = EditorGUILayout.FloatField("Color threshold g : ", colorValue.g);
            colorValue.b = EditorGUILayout.FloatField("Color threshold b : ", colorValue.b);
            colorValue.a = EditorGUILayout.FloatField("Color threshold a : ", colorValue.a);

            table.colorThreshold = colorValue;

            EditorGUILayout.Space();

            if (GUILayout.Button("Reset Values"))
            {
                table.floatThreshold = 0.1f;

                table.vectorThreshold = Vector4.one * 0.1f;

                table.colorThreshold = new Color(0.1f, 0.1f, 0.1f, 0.1f);
            }
        }
    }

    public class ManagerWindow : EditorWindow
    {
        public SuperLevelOptimizer optimizer;


        public void OnGUI()
        {
            if (optimizer == null)
                Close();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Add Selection"))
                AddSelection();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Remove Selection"))
                RemoveSelection();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Remove All"))
                RemoveAll();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Close"))
                Close();
        }


        private void AddSelection()
        {
            int count = 0;

            if (Selection.gameObjects.Length == 0)
            {
                Debug.Log("No objects selected");
                return;
            }

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                foreach (var renderer in Selection.gameObjects[i].GetComponentsInChildren<Renderer>())
                {
                    if (optimizer.CanCombineRenderer(renderer))
                    {
                        optimizer.AddObjectForCombine(renderer);
                        count++;
                    }
                }
            }

            Debug.Log("Added " + count + " objects");
        }

        private void RemoveSelection()
        {
            int count = optimizer.objectsForCombine.Count;

            for (int i = 0; i < Selection.gameObjects.Length; i++)
                optimizer.DeleteObjectsForCombine(Selection.gameObjects[i].GetComponentsInChildren<Renderer>());

            count = count - optimizer.objectsForCombine.Count;

            Debug.Log("Removed " + count + " objects");
        }

        private void RemoveAll()
        {
            optimizer.ClearObjectsForCombine();

            Debug.Log("Removed all objects");
        }
    }
}