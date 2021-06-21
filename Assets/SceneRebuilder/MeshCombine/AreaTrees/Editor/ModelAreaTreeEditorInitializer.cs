using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ModelAreaTreeEditorInitializer 
{
    static ModelAreaTreeEditorInitializer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += hierarchyOnGUI;
        Debug.Log("ModelAreaTreeEditorInitializer");
    }

    private static void hierarchyOnGUI(int instancedId, Rect selectionRect)
    {
        Debug.Log("ModelAreaTreeEditorInitializer.hierarchyOnGUI");
        var go = EditorUtility.InstanceIDToObject(instancedId) as GameObject;
        if (go)
        {
            if (go.GetComponent<ModelAreaTree>() != null)
            {
                Rect r = new Rect(selectionRect);
                r.x = r.width;
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