using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MeshProfilerNS
{
    public class MPGUIStyles
    {
        public static bool IsDirty=false;
        public const int SCREEN_WIDTH = 1500;
        public const int SCREEN_HEIGHT = 770;
        public const int PAGE_COUNT = 100;//ini

        public static Rect PREVIEW_BLOCK = new Rect(0, 5, 300, 200);
        public static Rect PREVIEW_BLOCK_CENTER = new Rect(10, 15, 280, 180);
        public static Rect SEARCH_BLOCK = new Rect(301, 5, 1200, 20);
        public static Rect LIST_BLOCK = new Rect(301, 20, 1200, 515);
        public static Rect SETTING_BLOCK = new Rect(0, 210, 300, 250);
        public static Rect TOOL_BLOCK = new Rect(0, 465, 300, 295);
        public static Rect PAGEINDEX_BLOCK = new Rect(300, 535, 1200, 25);
        public static Rect CHART_BLOCK = new Rect(300, 560, 598, 200);
        public static Rect CHART_PARAS_BLOCK = new Rect(750, 620, 150, 180);
        public static Rect DATA_BLOCK = new Rect(902, 560, 595, 200);
        public static GUIStyle itemStyle = new GUIStyle(UnityEditor.EditorStyles.label);
        public static GUIStyle[] itemBtnStyles = new GUIStyle[2] { new GUIStyle(UnityEditor.EditorStyles.toolbarButton), new GUIStyle(UnityEditor.EditorStyles.toolbarButton) };
        public static GUIStyle[] itemBtnStyles_child = new GUIStyle[2] { new GUIStyle(UnityEditor.EditorStyles.toolbarButton), new GUIStyle(UnityEditor.EditorStyles.toolbarButton) };
        public static GUIStyle TextFieldRoundEdge;
        public static GUIStyle TextFieldRoundEdgeCancelButton;
        public static GUIStyle TextFieldRoundEdgeCancelButtonEmpty;
        public static GUIStyle TransparentTextField;

        public static GUILayoutOption[] options_none;
        public static GUILayoutOption[] options_exist;
        public static GUILayoutOption[] options_icon;

        public static GUILayoutOption[] options_child_none;
        public static GUILayoutOption[] options_child_exist;
        public static GUILayoutOption[] options_child_icon;

        public static GUIContent icon_right_Content;
        public static GUIContent icon_down_Content;
        public static GUIContent[] icon_retract_Contents;
        public static GUIStyle icon_tab_Style;
        public static GUIStyle icon_tab_normal_Style;
        public static GUIStyle boxStyle;
        public static GUIStyle dataAreaStyle=new GUIStyle(UnityEditor.EditorStyles.toolbarButton);
        public static GUIStyle centerStyle=new GUIStyle();
        public static List<Texture2D> texList=new List<Texture2D>();

        public static void InitGUIStyles()
        {
            texList.Clear();
            
            itemStyle.fontSize = 12;
            itemStyle.fontStyle = FontStyle.Bold;
            itemStyle.alignment = TextAnchor.MiddleCenter;
            itemStyle.fixedHeight = 25;
            itemStyle.margin = new RectOffset(0, 0, 0, 0);
            itemStyle.padding = new RectOffset(0, 0, 0, 0);
            itemStyle.overflow = new RectOffset(0, -2, 0, 0);
            itemStyle.normal.background = GetColorTex(new Color(1, 1, 1, 0.6f));
            itemStyle.normal.textColor = Color.black;


            itemBtnStyles[0].fontSize = 10;
            itemBtnStyles[0].alignment = TextAnchor.MiddleCenter;
            itemBtnStyles[0].fixedHeight = 25;
            itemBtnStyles[0].normal.background = GetColorTex(new Color(1, 1, 1, 0.3f));
            itemBtnStyles[0].active.background = GetColorTex(new Color(1, 1, 1, 0.3f));
            itemBtnStyles[1].normal.background = GetColorTex(new Color(0, 1, 0, 0.2f));
            itemBtnStyles[1].active.background = GetColorTex(new Color(0, 1, 0, 0.2f));
            itemBtnStyles[1].margin = new RectOffset(0, 0, 0, 0);
            itemBtnStyles[1].overflow = new RectOffset(-2, 0, 0, 0);
            itemBtnStyles[1].fontSize = 11;
            itemBtnStyles[1].fontStyle = FontStyle.Bold;
            itemBtnStyles[1].alignment = TextAnchor.MiddleCenter;
            itemBtnStyles[1].fixedHeight = 25;

            itemBtnStyles_child[0].fontSize = 10;
            itemBtnStyles_child[0].alignment = TextAnchor.MiddleCenter;
            itemBtnStyles_child[0].fixedHeight = 20;

            itemBtnStyles_child[0].normal.background = GetColorTex(new Color(1, 1, 1, 0.3f));
            itemBtnStyles_child[0].active.background = GetColorTex(new Color(1, 1, 1, 0.3f));
            itemBtnStyles_child[1].normal.background = GetColorTex(new Color(1, 0, 0, 0.2f));
            itemBtnStyles_child[1].active.background = GetColorTex(new Color(1, 0, 0, 0.2f));
            itemBtnStyles_child[1].margin = new RectOffset(0, 0, 0, 0);
            itemBtnStyles_child[1].overflow = new RectOffset(0, 0, 0, 0);
            itemBtnStyles_child[1].fontSize = 10;
            itemBtnStyles_child[1].alignment = TextAnchor.MiddleCenter;
            itemBtnStyles_child[1].fixedHeight = 20;


            TextFieldRoundEdge = new GUIStyle("SearchTextField");
            TextFieldRoundEdgeCancelButton = new GUIStyle("SearchCancelButton");
            TextFieldRoundEdgeCancelButtonEmpty = new GUIStyle("SearchCancelButtonEmpty");
            TransparentTextField = new GUIStyle(EditorStyles.whiteLabel);
            TransparentTextField.normal.textColor = EditorStyles.textField.normal.textColor;

            options_none = new GUILayoutOption[2] { GUILayout.Width(80), GUILayout.Height(30) };
            options_exist = new GUILayoutOption[2] { GUILayout.Width(60), GUILayout.Height(30) };
            options_icon = new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(23) };

            options_child_none = new GUILayoutOption[2] { GUILayout.Width(80), GUILayout.Height(25) };
            options_child_exist = new GUILayoutOption[2] { GUILayout.Width(60), GUILayout.Height(25) };
            options_child_icon = new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(20) };

            icon_down_Content = EditorGUIUtility.IconContent("Icon Dropdown");
            icon_right_Content = EditorGUIUtility.IconContent("Clipboard");
            icon_tab_Style = new GUIStyle("Button");
            icon_tab_Style.padding = new RectOffset(-2, -2, -2, -2);
            icon_tab_Style.margin = new RectOffset(0, 0, 0, 0);
            icon_tab_Style.normal.background =  GetColorTex(new Color(0.5f, 0.8f, 0, 0.5f));
            
            icon_tab_normal_Style = new GUIStyle(UnityEditor.EditorStyles.toolbarButton);
            icon_tab_normal_Style.normal.background = EditorGUIUtility.isProSkin ? GetColorTex(new Color(0.3f, 0.3f, 0.3f, 0.5f)) : GetColorTex(new Color(1f, 1f, 1f, 0.6f));
            icon_tab_normal_Style.padding = new RectOffset(0, 0, 0, 0);
            icon_tab_normal_Style.fixedHeight = 25;


            icon_retract_Contents = new GUIContent[2] { EditorGUIUtility.IconContent("Toolbar Plus"), EditorGUIUtility.IconContent("Toolbar Minus") };

            boxStyle = new GUIStyle();
            boxStyle.normal.background = GetColorTex(new Color(1, 1, 0, 0.2f));
            IsDirty = true;

            dataAreaStyle.normal.background = EditorGUIUtility.isProSkin ? GetColorTex(new Color(1, 1, 1, 0.2f)):GetColorTex(new Color(1, 1, 1, 0.8f));
            centerStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f, 1) : Color.black;
            centerStyle.alignment = TextAnchor.MiddleCenter;
        }
        public static Rect BorderArea(Rect rect, int border = 1)
        {
            return new Rect(rect.x + border, rect.y + border, rect.width - 2 * border, rect.height - 2 * border);
        }

        static Texture2D GetColorTex(Color color)
        {
            Texture2D tex = new Texture2D(1, 1);
            for (int i = 0; i < tex.width; i++)
                for (int j = 0; j < tex.height; j++)
                {
                    tex.SetPixel(j, i, color);
                }
            tex.Apply();
            texList.Add(tex);
            return tex;
        }
    }
}
