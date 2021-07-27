#if UNITY_EDITOR
namespace CodeStage.AdvancedFPSCounter.Editor.UI
{
    using System.Collections.Generic;
    using UnityEditor;
	using UnityEngine;

	[System.Serializable]
	public class FoldoutEditorArg
    {
        public bool isEnabled=false;
        public bool isSelected = false;
        public bool isExpanded = false;
        public bool isToggle = true;
		public string caption="";
		public string info=""; 
		public bool bold = false; 
		public bool separator = false; 
		public bool background = false;

		public int pageSize_selected = 10;
        public string[] pageSize_names = new string[] {"5", "10", "15","20","50","100","200","500","1000","2000"};
        public int[] pageSize_sizes = {5, 10, 15,20,50,100,200,500,1000,2000};
		public int pageId_selected=1;
		public string[] pageId_names=null;
        public int[] pageId_sizes =null;
		public int pageCount=1;

        public int listFilterId = 1;
        public string[] listFilterId_Names = new string[] { "All" };
        //public int[] pageSize_sizes = { 5, 10, 15, 20, 50, 100, 200, 500, 1000, 2000 };

        public FoldoutEditorArg()
        {

        }

        public FoldoutEditorArg(bool isEnabled, bool isExpanded)
        {
            this.isEnabled = isEnabled;
            this.isExpanded = isExpanded;
        }

        public FoldoutEditorArg(bool isEnabled,bool isExpanded,bool isToggle)
        {
            this.isEnabled = isEnabled;
            this.isExpanded = isExpanded;
            this.isToggle = isToggle;
        }

        public FoldoutEditorArg(bool isEnabled, bool isExpanded, bool isToggle,bool separator,bool background)
        {
            this.isEnabled = isEnabled;
            this.isExpanded = isExpanded;
            this.isToggle = isToggle;
            this.separator = separator;
            this.background = background;
        }

        public int DrawFilterList(int width,params string[] filters)
        {
            if(listFilterId_Names==null|| listFilterId_Names.Length != filters.Length)
            {
                listFilterId_Names = filters;
                listFilterId = 1;
            }
            listFilterId = EditorGUILayout.Popup(listFilterId, listFilterId_Names,GUILayout.Width(width));
            return listFilterId;
        }

        public int DrawPageSizeList()
		{
            EditorGUILayout.LabelField("PageSize:", GUILayout.Width(80));
            pageSize_selected = EditorGUILayout.IntPopup(pageSize_selected, pageSize_names, pageSize_sizes);
            
            //EditorGUILayout.Popup("Size", 0, pageSize_names);
			return pageSize_selected;	
		}

		public void DrawPageIndexList(float count)
		{
			pageCount=(int)Mathf.Ceil(count/pageSize_selected);
			if(pageId_sizes==null || pageId_sizes.Length!=pageCount){
				pageId_sizes=new int[pageCount];
				pageId_names=new string[pageCount];
				for(int i=0;i<pageCount;i++)
				{
					pageId_sizes[i]=i+1;
					pageId_names[i]=(i+1).ToString();
				}
				pageId_selected=1;
			}
            EditorGUILayout.LabelField("PageIndex:",GUILayout.Width(100));
            pageId_selected = EditorGUILayout.IntPopup( pageId_selected, pageId_names, pageId_sizes);
            EditorGUILayout.LabelField("/"+pageCount, GUILayout.Width(30));
        }

		public void DrawPageToolbar(int count)
		{
			EditorGUILayout.BeginHorizontal();
			DrawPageSizeList();
			DrawPageIndexList(count);
			EditorGUILayout.EndHorizontal();
		}

        public void DrawPageToolbar<T>(List<T> list,System.Action<T,int> drawItemAction)
        {
            this.DrawPageToolbar(list.Count);
            int c = 0;
            for (int i = this.GetStartId(); i < list.Count && i < this.GetEndId(); i++)
            {
                c++;
                var item = list[i];
                if (drawItemAction != null)
                {
                    drawItemAction(item,i);
                }

                //var arg = editorArgs[meshMat];
                //arg.caption = meshMat.mat.name;
                //EditorUIUtils.ObjectFoldout(arg, meshMat.mat, () =>
                //{
                //    EditorGUILayout.ColorField("Color", meshMat.color, GUILayout.Width(50));
                //});
            }
        }

        public int GetStartId()
		{
			return (pageId_selected-1)*pageSize_selected;
		}
		public int GetEndId()
		{
			return (pageId_selected)*pageSize_selected;
		}
    }

    [System.Serializable]
    public class FoldoutEditorArg<T>: FoldoutEditorArg
    {
        public List<T> Items = new List<T>();

        public FoldoutEditorArg()
        {

        }

        public FoldoutEditorArg(bool isEnabled, bool isExpanded):base(isEnabled,isExpanded)
        {
            
        }

        public FoldoutEditorArg(bool isEnabled, bool isExpanded, bool isToggle) : base(isEnabled, isExpanded, isToggle)
        {
            
        }

        public FoldoutEditorArg(bool isEnabled, bool isExpanded, bool isToggle, bool separator, bool background) : base(isEnabled, isExpanded, isToggle,separator,background)
        {

        }
    }

    public struct EditorUIUtils : System.IDisposable
	{



		public static GUIStyle richBoldFoldout;
		public static GUIStyle richMiniLabel;
		public static GUIStyle line;
		public static GUIStyle panelWithBackground;

		public static void SetupStyles()
		{
			if (richBoldFoldout != null) return;

			richBoldFoldout = new GUIStyle(EditorStyles.foldout)
			{
				richText = true,
				fontStyle = FontStyle.Bold
			};

			richMiniLabel = new GUIStyle(EditorStyles.miniLabel)
			{
				wordWrap = true,
				richText = true
			};

			panelWithBackground = new GUIStyle(GUI.skin.box)
			{
				padding = new RectOffset()
			};

			line = new GUIStyle(GUI.skin.box);
		}

		public static void Separator(int padding = 0)
		{
			if (padding != 0) GUILayout.Space(padding);

			var position = EditorGUILayout.GetControlRect(false, 1f);
			position = EditorGUI.PrefixLabel(position, GUIContent.none);

			var bgTexture = line.normal.background;

#if UNITY_2019_3_OR_NEWER
			if (bgTexture == null)
			{
				var scaledBackgrounds = line.normal.scaledBackgrounds;
				if (scaledBackgrounds != null && scaledBackgrounds.Length > 0)
				{
					bgTexture = line.normal.scaledBackgrounds[0];
				}
			}
#endif

			if (bgTexture != null)
			{
				var texCoordinates = new Rect(0f, 1f, 1f, 1f - 1f / bgTexture.height);
				GUI.DrawTextureWithTexCoords(position, bgTexture, texCoordinates);
			}

			if (padding != 0) GUILayout.Space(padding);
		}

		public static void Header(string header)
		{
			var rect = EditorGUILayout.GetControlRect(false, 24);
			rect.y += 8f;
			rect = EditorGUI.IndentedRect(rect);
			GUI.Label(rect, header, EditorStyles.boldLabel);
		}

		public static void Indent(int topPadding = 2)
		{
			EditorGUI.indentLevel++;
			GUILayout.Space(topPadding);
		}

		public static void UnIndent(int bottomPadding = 5)
		{
			EditorGUI.indentLevel--;
			GUILayout.Space(bottomPadding);
		}

		public static void DoubleIndent(int topPadding = 2)
		{
			Indent(topPadding);
			Indent(0);
		}

		public static void DoubleUnIndent(int bottomPadding = 5)
		{
			UnIndent(0);
			UnIndent(bottomPadding);
		}

		public static bool Foldout(SerializedProperty foldout, string caption)
		{
			Separator(5);
			GUILayout.BeginHorizontal(panelWithBackground);
			GUILayout.Space(13);
			foldout.isExpanded = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), foldout.isExpanded, caption, true, richBoldFoldout);
			GUILayout.EndHorizontal();
			return foldout.isExpanded;
		}

        public static bool Foldout(bool isExpanded, string caption)
        {
            //Separator(5);
            //GUILayout.BeginHorizontal(panelWithBackground);
            GUILayout.BeginHorizontal();
            GUILayout.Space(13);
            isExpanded = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), isExpanded, caption, true, richBoldFoldout);
            GUILayout.EndHorizontal();
            return isExpanded;
        }

        public static bool ToggleFoldout(SerializedProperty toggle, SerializedProperty foldout, string caption, bool bold = true, bool separator = true, bool background = true)
		{
			if (separator) Separator(5);

			if (background)
			{
				GUILayout.BeginHorizontal(panelWithBackground);
			}
			else
			{
				GUILayout.BeginHorizontal();
			}

			var currentLabelWidth = EditorGUIUtility.labelWidth;

			EditorGUIUtility.labelWidth = 1;
			EditorGUILayout.PropertyField(toggle, GUIContent.none, GUILayout.ExpandWidth(false));
			EditorGUIUtility.labelWidth = currentLabelWidth;
			
			GUILayout.Space(10);
			var rect = EditorGUILayout.GetControlRect(); 
			foldout.isExpanded = EditorGUI.Foldout(rect, foldout.isExpanded, caption, true, bold ? richBoldFoldout : EditorStyles.foldout);
			GUILayout.EndHorizontal();

			return toggle.boolValue;
		}

		public static bool ToggleFoldout(FoldoutEditorArg arg, System.Action<FoldoutEditorArg> toggleEvent,System.Action toolbarEvent)
		{
			if (arg.separator) Separator(5);

			if (arg.background)
			{
				GUILayout.BeginHorizontal(panelWithBackground);
			}
			else
			{
				GUILayout.BeginHorizontal();
			}

			var currentLabelWidth = EditorGUIUtility.labelWidth;

            if (arg.isToggle)
            {
                EditorGUIUtility.labelWidth = 1;
                //EditorGUILayout.PropertyField(toggle, GUIContent.none, GUILayout.ExpandWidth(false));
                arg.isEnabled = EditorGUILayout.Toggle(arg.isEnabled, GUILayout.Width(15));
                if (arg.isEnabled)
                {
                    if (toggleEvent != null)
                    {
                        toggleEvent(arg);
                    }
                }
                EditorGUIUtility.labelWidth = currentLabelWidth;
            }



            GUILayout.Space(10);
			var rect = EditorGUILayout.GetControlRect(); 
			arg.isExpanded = EditorGUI.Foldout(rect, arg.isExpanded, arg.caption, true, arg.bold ? richBoldFoldout : EditorStyles.foldout);


			EditorGUIUtility.labelWidth = 1;
            var contentStyle = new GUIStyle(EditorStyles.label);
            contentStyle.alignment = TextAnchor.MiddleRight;
            //EditorGUILayout.PropertyField(toggle, GUIContent.none, GUILayout.ExpandWidth(false));
            GUILayout.Label(arg.info, contentStyle);
			// if(GUILayout.Button(btnName,GUILayout.Width(60)))
			// {
			// 	if(clickEvent!=null){
			// 		clickEvent();
			// 	}
			// }

			if(toolbarEvent!=null){
				toolbarEvent();
			}

            EditorGUIUtility.labelWidth = currentLabelWidth;

			GUILayout.EndHorizontal();

			return arg.isEnabled;
		}

		public static bool ButtonFoldout(bool isExpanded, string caption,string info, bool bold = true, bool separator = true, bool background = true,string btnName="",System.Action clickEvent=null)
        {
            if (separator) Separator(5);

            if (background)
            {
                GUILayout.BeginHorizontal(panelWithBackground);
            }
            else
            {
                GUILayout.BeginHorizontal();
            }

            var currentLabelWidth = EditorGUIUtility.labelWidth;

            GUILayout.Space(10);
            var rect = EditorGUILayout.GetControlRect();
            isExpanded = EditorGUI.Foldout(rect, isExpanded, caption, true, bold ? richBoldFoldout : EditorStyles.foldout);

            EditorGUIUtility.labelWidth = 1;
            var contentStyle = new GUIStyle(EditorStyles.label);
            contentStyle.alignment = TextAnchor.MiddleRight;
            //EditorGUILayout.PropertyField(toggle, GUIContent.none, GUILayout.ExpandWidth(false));
            GUILayout.Label(info, contentStyle);
			if(GUILayout.Button(btnName,GUILayout.Width(100)))
			{
				// Selection.activeObject = obj;
				// EditorGUIUtility.PingObject(obj);
				// EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
				if(clickEvent!=null){
					clickEvent();
				}
			}
            EditorGUIUtility.labelWidth = currentLabelWidth;

            GUILayout.EndHorizontal();

            return isExpanded;
        }

        public static bool ObjectFoldout(FoldoutEditorArg arg, Object obj = null, System.Action itemToolbarEvent = null)
        {
            if (arg.separator) Separator(5);

            if (arg.background)
            {
                GUILayout.BeginHorizontal(panelWithBackground);
            }
            else
            {
                GUILayout.BeginHorizontal();
            }

            var currentLabelWidth = EditorGUIUtility.labelWidth;

            if (obj != null && obj is GameObject)
            {
                GameObject go = obj as GameObject;
                EditorGUIUtility.labelWidth = 1;
                //EditorGUILayout.PropertyField(toggle, GUIContent.none, GUILayout.ExpandWidth(false));
                EditorGUILayout.Toggle(go.activeInHierarchy, GUILayout.Width(15));
                bool isOn = EditorGUILayout.Toggle(go.activeSelf, GUILayout.Width(15));
                if (isOn != go.activeSelf)
                {
                    go.SetActive(isOn);
                }
                // if(arg.isEnabled){
                // 	if(toggleEvent!=null){
                // 		toggleEvent(arg);
                // 	}
                // }
                EditorGUIUtility.labelWidth = currentLabelWidth;
            }


            GUILayout.Space(10);
            var rect = EditorGUILayout.GetControlRect(GUILayout.Width(100));
            // rect.
            arg.isExpanded = EditorGUI.Foldout(rect, arg.isExpanded, arg.caption, true, (arg.bold || arg.isSelected) ? richBoldFoldout : EditorStyles.foldout);

            EditorGUIUtility.labelWidth = 1;
            var contentStyle = new GUIStyle(EditorStyles.label);
            contentStyle.alignment = TextAnchor.MiddleRight;
            //EditorGUILayout.PropertyField(toggle, GUIContent.none, GUILayout.ExpandWidth(false));
            GUILayout.Label(arg.info, contentStyle);

            if (itemToolbarEvent != null)
            {
                itemToolbarEvent();
            }

            if (obj != null)
            {
                // if(GUILayout.Button("P", EditorStyles.miniButtonLeft, GUILayout.Width(20)))
                // {
                // 	EditorGUIUtility.PingObject(obj);
                // }
                var btnStyle = new GUIStyle(EditorStyles.miniButton);
                btnStyle.margin = new RectOffset(0, 0, 0, 0);
                btnStyle.padding = new RectOffset(0, 0, 0, 0);
                if (GUILayout.Button(">", btnStyle, GUILayout.Width(20)))
                {
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                    EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
                    arg.isSelected = true;
                }
            }

            EditorGUIUtility.labelWidth = currentLabelWidth;

            GUILayout.EndHorizontal();

            return arg.isExpanded;
        }

        public static bool ObjectFoldout(bool isExpanded, string caption,string info, bool bold = true, bool separator = true, bool background = true,GameObject obj=null,System.Action itemToolbarEvent=null)
        {
            if (separator) Separator(5);

            if (background)
            {
                GUILayout.BeginHorizontal(panelWithBackground);
            }
            else
            {
                GUILayout.BeginHorizontal();
            }

			var currentLabelWidth = EditorGUIUtility.labelWidth;

			if(obj!=null){
				EditorGUIUtility.labelWidth = 1;
				//EditorGUILayout.PropertyField(toggle, GUIContent.none, GUILayout.ExpandWidth(false));
				EditorGUILayout.Toggle(obj.activeInHierarchy,GUILayout.Width(15));
				bool isOn=EditorGUILayout.Toggle(obj.activeSelf,GUILayout.Width(15));
				if(isOn!=obj.activeSelf)
				{
					obj.SetActive(isOn);
				}
				// if(arg.isEnabled){
				// 	if(toggleEvent!=null){
				// 		toggleEvent(arg);
				// 	}
				// }
				EditorGUIUtility.labelWidth = currentLabelWidth;
			}


            GUILayout.Space(10);
            var rect = EditorGUILayout.GetControlRect(GUILayout.Width(100));
			// rect.
            isExpanded = EditorGUI.Foldout(rect, isExpanded, caption, true, bold ? richBoldFoldout : EditorStyles.foldout);

            EditorGUIUtility.labelWidth = 1;
            var contentStyle = new GUIStyle(EditorStyles.label);
            contentStyle.alignment = TextAnchor.MiddleRight;
            //EditorGUILayout.PropertyField(toggle, GUIContent.none, GUILayout.ExpandWidth(false));
            GUILayout.Label(info, contentStyle);
			
			if(itemToolbarEvent!=null){
				itemToolbarEvent();
			}

			if(obj!=null){
				// if(GUILayout.Button("P", EditorStyles.miniButtonLeft, GUILayout.Width(20)))
				// {
				// 	EditorGUIUtility.PingObject(obj);
				// }
				var btnStyle = new GUIStyle(EditorStyles.miniButton);
                    btnStyle.margin=new RectOffset(0,0,0,0);
                    btnStyle.padding=new RectOffset(0,0,0,0);
				if(GUILayout.Button(">", btnStyle, GUILayout.Width(20)))
				{
					Selection.activeObject = obj;
					EditorGUIUtility.PingObject(obj);
					EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
				}
			}

            EditorGUIUtility.labelWidth = currentLabelWidth;

            GUILayout.EndHorizontal();

            return isExpanded;
        }

        public static bool DrawProperty(SerializedProperty property, System.Action setter, params GUILayoutOption[] options)
		{
			return DrawProperty(property, (GUIContent)null, setter, options);
		}

		public static bool DrawProperty(SerializedProperty property, string content, System.Action setter, params GUILayoutOption[] options)
		{
			return DrawProperty(property, new GUIContent(content), setter, options);
		}

		public static bool DrawProperty(SerializedProperty property, GUIContent content, System.Action setter, params GUILayoutOption[] options)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(property, content, options);
			if (EditorGUI.EndChangeCheck())
			{
				setter.Invoke();
				return true;
			}

			return false;
		}

		// ----------------------------------------------------------------------------
		// tooling for "using" keyword
		// ----------------------------------------------------------------------------

		private readonly LayoutMode mode;

		public static EditorUIUtils Horizontal(params GUILayoutOption[] options)
		{
			return Horizontal(GUIStyle.none, options);
		}

		public static EditorUIUtils Horizontal(GUIStyle style, params GUILayoutOption[] options)
		{
			return new EditorUIUtils(LayoutMode.Horizontal, style, options);
		}

		public static EditorUIUtils Vertical(params GUILayoutOption[] options)
		{
			return Vertical(GUIStyle.none, options);
		}

		public static EditorUIUtils Vertical(GUIStyle style, params GUILayoutOption[] options)
		{
			return new EditorUIUtils(LayoutMode.Vertical, style, options);
		}

		private EditorUIUtils(LayoutMode layoutMode, GUIStyle style, params GUILayoutOption[] options)
		{
			mode = layoutMode;

			if (mode == LayoutMode.Horizontal)
			{
				GUILayout.BeginHorizontal(style, options);
			}
			else
			{
				GUILayout.BeginVertical(style, options);
			}
		}

		public void Dispose()
		{
			if (mode == LayoutMode.Horizontal)
			{
				GUILayout.EndHorizontal();
			}
			else
			{
				GUILayout.EndVertical();
			}
		}

		private enum LayoutMode : byte
		{
			Horizontal,
			Vertical
		}
	}
}
#endif