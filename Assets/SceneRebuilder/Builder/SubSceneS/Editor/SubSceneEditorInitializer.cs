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

    public static Rect CreateRect(Rect selectionRect,float width)
    {
        //float width = 45;
        Rect r = new Rect(selectionRect);
        r.x = selectionRect.x + selectionRect.width - width;
        r.width = width;
        return r;
    }

    private static void hierarchyOnGUI(int instancedId,Rect selectionRect)
    {
        //EditorUtility.Hi
        //Debug.Log("SubSceneEditorInitializer.hierarchyOnGUI");
        var go = EditorUtility.InstanceIDToObject(instancedId) as GameObject;
        if (go)
        {
            if(go.GetComponent<SubScene_Base>()!=null)
            {
                SubScene_Base scene = go.GetComponent<SubScene_Base>();
                bool isLoaded = scene.HaveGos();
                Rect r = CreateRect(selectionRect, 45);

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
                Rect r = CreateRect(selectionRect, 55);
                var style = new GUIStyle();
                style.normal.textColor = Color.green;
                style.hover.textColor = Color.blue;
                GUI.Label(r, "[ModelTree]", style);
            }
            else if (go.GetComponent<AreaTreeNode>() != null)
            {
                Rect r = CreateRect(selectionRect, 55);
                var style = new GUIStyle();
                style.normal.textColor = new Color(0, 0.5f, 1);
                style.hover.textColor = Color.red;
                GUI.Label(r, "[TreeNode]", style);
            }
        }
    }
}
