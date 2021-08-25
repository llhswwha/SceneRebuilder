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
    }
}
