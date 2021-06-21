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
                SubScene_Base scene = go.GetComponent<SubScene_Base>();
                bool isLoaded = scene.HaveGos();
                Rect r = new Rect(selectionRect);
                r.x = r.width;
                r.width = 80;
                //r.y += 2;
                var style = new GUIStyle();
                if (isLoaded)
                {
                    style.normal.textColor = new Color(1,0.5f,0);
                }
                else
                {
                    style.normal.textColor = Color.yellow;
                }
                
                style.hover.textColor = Color.green;
                //GUI.Label(r, $"[Scene][{isLoaded}]", style);
                GUI.Label(r, $"[Scene]", style);
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
