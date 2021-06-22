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

    public static void CreateLabel(string text,Rect selectionRect,float width,Color color1,Color color2)
    {
        Rect r = CreateRect(selectionRect, width);
        var style = new GUIStyle();
        style.normal.textColor = color1;
        style.hover.textColor = color2;
        GUI.Label(r, text, style);
    }

    private static void hierarchyOnGUI(int instancedId,Rect selectionRect)
    {
        //EditorUtility.Hi
        //Debug.Log("SubSceneEditorInitializer.hierarchyOnGUI");
        var go = EditorUtility.InstanceIDToObject(instancedId) as GameObject;
        if (go)
        {
            if (go.GetComponent<SubScene_List>() != null)
            {
                CreateLabel("[SceneList]", selectionRect, 65, new Color(1, 0.5f, 0.5f), Color.blue);
            }
            else if (go.GetComponent<SubScene_Base>()!=null)
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
                CreateLabel("[ModelTree]", selectionRect, 65, Color.green, Color.blue);
            }
            else if (go.GetComponent<AreaTreeNode>() != null)
            {
                CreateLabel("[TreeNode]", selectionRect, 65, new Color(0, 0.5f, 1), Color.red);
            }
            else if (go.GetComponent<BuildingModelInfoList>() != null)
            {
                if (go.GetComponent<BuildingController>() != null)
                {
                    CreateLabel("[Models][Building]", selectionRect, 90, new Color(0.9f, 0.3f, 0.3f), Color.red);
                }
                else
                {
                    CreateLabel("[Models]", selectionRect, 90, new Color(0.9f, 0.3f, 1), Color.red);
                }
            }

            else if (go.GetComponent<BuildingModelInfo>() != null)
            {
                if (go.GetComponent<BuildingController>() != null)
                {
                    CreateLabel("[Model][Building]", selectionRect, 90, new Color(1, 0.5f, 0.5f), Color.red);
                }
                else if (go.GetComponent<FloorController>() != null)
                {
                    CreateLabel("[Model][Floor]", selectionRect, 90, new Color(1, 0.7f, 0.3f), Color.red);
                }
                else
                {
                    CreateLabel("[Model]", selectionRect, 90, new Color(1, 0.5f, 1), Color.red);
                }
            }
            else if (go.GetComponent<BuildingController>() != null)
            {
                CreateLabel("[Building]", selectionRect, 90, new Color(0.4f, 0.5f, 1), Color.red);
            }
            else if (go.GetComponent<FloorController>() != null)
            {
                CreateLabel("[Floor]", selectionRect, 65, new Color(0.6f, 0.5f, 0.8f), Color.red);
            }
            else if (go.GetComponent<Camera>() != null)
            {
                CreateLabel("[Camera]", selectionRect, 65, Color.gray, Color.red);
            }
            else if (go.GetComponent<Light>() != null)
            {
                CreateLabel("[Light]", selectionRect, 65, Color.gray, Color.red);
            }
            else if (go.GetComponent<Canvas>() != null)
            {
                CreateLabel("[Canvas]", selectionRect, 65, Color.gray, Color.red);
            }
            else
            {
                if(go.name=="In" || go.name == "Out1" || go.name == "Out0")
                {
                    CreateLabel("[Part]", selectionRect, 65, Color.cyan, Color.red);
                }
            }
        }
    }
}
