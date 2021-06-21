using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SubSceneEditorInitializer
{
    static SubSceneEditorInitializer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += hierarchyOnGUI;
        Debug.Log("SubSceneEditorInitializer");
    }

    private static void hierarchyOnGUI(int instancedId,Rect selectionRect)
    {
        //Debug.Log("SubSceneEditorInitializer.hierarchyOnGUI");
        var go = EditorUtility.InstanceIDToObject(instancedId) as GameObject;
        if (go)
        {
            if(go.GetComponent<SubScene_Base>()!=null)
            {
                Rect r = new Rect(selectionRect);
                r.x = r.width;
                r.width = 80;
                //r.y += 2;
                var style = new GUIStyle();
                style.normal.textColor = Color.yellow;
                style.hover.textColor = Color.green;
                GUI.Label(r, "[SubScene]", style);
            }
            else if (go.GetComponent<ModelAreaTree>() != null)
            {
                Rect r = new Rect(selectionRect);
                r.x = r.width+20;
                r.width = 80;
                //r.y += 2;
                var style = new GUIStyle();
                style.normal.textColor = Color.green;
                style.hover.textColor = Color.blue;
                GUI.Label(r, "[ModelTree]", style);
            }
        }
    }
}
