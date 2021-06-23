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

    public static float Layer1Width = 115;
    public static float Layer2Width = 65;
    public static float Layer3Width = 45;

    private static void hierarchyOnGUI(int instancedId,Rect selectionRect)
    {
        //EditorUtility.Hi
        //Debug.Log("SubSceneEditorInitializer.hierarchyOnGUI");
        var go = EditorUtility.InstanceIDToObject(instancedId) as GameObject;
        if (go)
        {
            if (go.GetComponent<SubScene_List>() != null)
            {
                CreateLabel("[SceneList]", selectionRect, Layer2Width, new Color(1, 0.5f, 0.5f), Color.blue);
            }
            else if (go.GetComponent<SubScene_Base>()!=null)
            {
                SubScene_Base scene = go.GetComponent<SubScene_Base>();
                bool isLoaded = scene.HaveGos();
                Rect r = CreateRect(selectionRect, Layer3Width);

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
                CreateLabel("[ModelTree]", selectionRect, Layer2Width, new Color(0, 1f, 1), Color.blue);
            }
            else if (go.GetComponent<AreaTreeNode>() != null)
            {
                AreaTreeNode node = go.GetComponent<AreaTreeNode>();

                if (node.IsLeaf)
                {
                    CreateLabel($"[LeafNode]", selectionRect, Layer2Width, new Color(0, 0.7f, 1), Color.red);
                }
                else
                {
                    CreateLabel($"[TreeNode]", selectionRect, Layer2Width, new Color(0, 0.5f, 1), Color.red);
                }
                
            }
            else if (go.GetComponent<BuildingModelInfoList>() != null)
            {
                if (go.GetComponent<BuildingController>() != null)
                {
                    CreateLabel("[Models][Build]", selectionRect, Layer1Width, new Color(0.9f, 0.3f, 0.3f), Color.red);
                }
                else
                {
                    CreateLabel("[Models]", selectionRect, Layer1Width, new Color(0.9f, 0.3f, 1), Color.red);
                }
            }

            else if (go.GetComponent<BuildingModelInfo>() != null)
            {
                BuildingModelInfo modelInfo = go.GetComponent<BuildingModelInfo>();
                bool r = modelInfo.IsModelSceneFinish();
                Color c = new Color(0, 1, 0);
                if (go.GetComponent<BuildingController>() != null)
                {
                    if (r)
                    {
                        CreateLabel($"[{modelInfo.GetInfoName()}][Build]", selectionRect, Layer1Width, c, Color.red);
                    }
                    else
                    {
                        CreateLabel($"[{modelInfo.GetInfoName()}][Build]", selectionRect, Layer1Width, new Color(1, 0.5f, 0.5f), Color.red);
                    }
                    
                }
                else if (go.GetComponent<FloorController>() != null)
                {
                    //CreateLabel($"[{modelInfo.GetInfoName()}][Floor]", selectionRect, Layer1Width, new Color(1, 0.7f, 0.3f), Color.red);

                    if (r)
                    {
                        CreateLabel($"[{modelInfo.GetInfoName()}][Build]", selectionRect, Layer1Width, c, Color.red);
                    }
                    else
                    {
                        CreateLabel($"[{modelInfo.GetInfoName()}][Floor]", selectionRect, Layer1Width, new Color(1, 0.5f, 0.5f), Color.red);
                    }
                }
                else
                {
                    //CreateLabel($"[{modelInfo.GetInfoName()}]", selectionRect, Layer1Width, new Color(1, 0.5f, 1), Color.red);

                    if (r)
                    {
                        CreateLabel($"[{modelInfo.GetInfoName()}]", selectionRect, Layer1Width, c, Color.red);
                    }
                    else
                    {
                        CreateLabel($"[{modelInfo.GetInfoName()}]", selectionRect, Layer1Width, new Color(1, 0.5f, 0.5f), Color.red);
                    }
                }
            }
            else if (go.GetComponent<BoundsBox>() != null)
            {
                CreateLabel("[Bounds]", selectionRect, Layer2Width, Color.white, Color.red);
            }
            else if (go.GetComponent<BuildingController>() != null)
            {
                CreateLabel("[Build]", selectionRect, Layer1Width, new Color(0.4f, 0.5f, 1), Color.red);
            }
            else if (go.GetComponent<FloorController>() != null)
            {
                CreateLabel("[Floor]", selectionRect, Layer2Width, new Color(0.6f, 0.5f, 0.8f), Color.red);
            }
            else if (go.GetComponent<Camera>() != null)
            {
                CreateLabel("[Camera]", selectionRect, Layer2Width, Color.gray, Color.red);
            }
            else if (go.GetComponent<Light>() != null)
            {
                CreateLabel("[Light]", selectionRect, Layer2Width, Color.gray, Color.red);
            }
            else if (go.GetComponent<Canvas>() != null)
            {
                CreateLabel("[Canvas]", selectionRect, Layer2Width, Color.gray, Color.red);
            }
            else
            {
                if(go.name=="In" || go.name == "Out1" || go.name == "Out0")
                {
                    CreateLabel("[Part]", selectionRect, Layer2Width, Color.grey, Color.red);
                }
            }
        }
    }
}
