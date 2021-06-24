using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace MeshProfilerNS
{
    public class ShowDataList : EditorWindow
    {
        public static void AddWindow(List<StringBuilder> list, string title)
        {
            ShowDataList window = (ShowDataList)EditorWindow.GetWindowWithRect(typeof(ShowDataList), new Rect(0, 0, 320, 400), true, title);
            _title = title;
            window.Init(list);
            window.ShowAuxWindow();
        }
        Vector2 screenUV;
        static string _title;
        private List<StringBuilder> strList;
        void Init(List<StringBuilder> list)
        {
            strList = list;
        }
        private void OnGUI()
        {
            screenUV = EditorGUILayout.BeginScrollView(screenUV);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            for (int i = 0; i < strList.Count; i++)
            {
                GUILayout.Label(strList[i].ToString());
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.Space(5);
            if(GUILayout.Button("Output TXT File",GUILayout.Height(30)))
            {
                string fileName = _title;
                fileName = fileName.Replace(".","_");
                fileName = fileName.Replace("=>", "_");
                string exportPath = EditorUtility.SaveFilePanel("Save Text File", "", fileName + ".txt", "txt");
                if (!string.IsNullOrEmpty(exportPath))
                {
                    if (File.Exists(exportPath))
                    {
                        File.Delete(exportPath);
                    }
                    FileInfo myFile = new FileInfo(@exportPath);
                    StreamWriter sw = myFile.CreateText();
                    for (int i = 0; i < strList.Count; i++)
                    {
                        sw.Write(strList[i]);
                    }
                    sw.Close();
                    EditorUtility.DisplayDialog("Tips", "Output txt file successfully！", "ok");
                }
            }
            GUILayout.Space(5);
        }
    }

    public class ShowRefList : EditorWindow
    {
        public static void AddWindow(List<GameObject> list, string title)
        {
            ShowRefList window = (ShowRefList)EditorWindow.GetWindowWithRect(typeof(ShowRefList), new Rect(0, 0, 300, 400), true, title);
            window.Init(list);
            window.ShowAuxWindow();
        }
        Vector2 screenUV;

        private List<string> strList;
        private List<GameObject> objList;
        void Init(List<GameObject> list)
        {
            strList = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                strList.Add(list[i].name);
            }
            objList = list;
        }
        private void OnGUI()
        {
            if (GUILayout.Button("Select All Objs—" + objList.Count, GUILayout.Height(30)))
            {
                Selection.objects = objList.ToArray();
            }

            screenUV = EditorGUILayout.BeginScrollView(screenUV);

            for (int i = 0; i < strList.Count; i++)
            {
                if (GUILayout.Button(strList[i]))
                {
                    Selection.activeObject = objList[i];
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}