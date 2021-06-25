using MeshProfilerNS;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ListManagerEditorWindow<T1,T2> : EditorWindow where T1 : ListItemElement<T2> where T2:ListItemElementValues,new()
{
    protected List<T1> meshElementList = new List<T1>();
    protected List<T1> originList = new List<T1>();
}