using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshRendererInfo))]
public class MeshRendererInfoEditor : BaseFoldoutEditor<MeshRendererInfo>
{
    public override void OnEnable()
    {
        base.OnEnable();
        
    }

    //private MeshRendererType rendererType1;
    //private MeshRendererType rendererType2;

    public override void OnToolLayout(MeshRendererInfo item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
        var rts = item.GetRendererTypes();
        if(rts.Count>1)
        {
            GUILayout.Label($"RType:{item.rendererType}", GUILayout.Width(70));
        }
        else
        {
            GUILayout.Label($"RType:", GUILayout.Width(70));
        }
       
        item.rendererType = (MeshRendererType)EditorGUILayout.EnumFlagsField(item.rendererType,GUILayout.Width(100));
        
        foreach (var rt in rts)
        {
            //GUILayout.Label($"{rt}", GUILayout.Width(100));
            GUILayout.Button($"{rt}");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Init"))
        {
            DateTime start = DateTime.Now;
            item.Init();
            Debug.Log($"Init {(DateTime.Now - start).TotalMilliseconds}ms");
        }
        if (GUILayout.Button("GetMinMax"))
        {
            DateTime start = DateTime.Now;
            Vector3[] minMax = MeshRendererInfo.GetMinMax(item.gameObject);
            foreach(var v in minMax)
            {
                Debug.Log($"v:{v}");
            }
            Debug.Log($"GetMinMax {(DateTime.Now - start).TotalMilliseconds}ms");
        }
        if (GUILayout.Button("ShowBounds"))
        {
            item.ShowBounds();
        }
        if (GUILayout.Button("ShowCenter"))
        {
            item.ShowCenter();
        }
        if (GUILayout.Button("ShowWeight"))
        {
            item.ShowWeightCenter();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("GetAssetPath"))
        {
            Debug.Log(item.GetAssetPath());
        }
        EditorGUILayout.EndHorizontal();
    }
}
