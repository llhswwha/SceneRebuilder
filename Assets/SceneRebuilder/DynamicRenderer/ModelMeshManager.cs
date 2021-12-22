using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ModelMeshManager : MonoBehaviour
{
    public List<string> StartWithNames = new List<string>();

    public List<string> ContainsNames = new List<string>();

    public List<GameObject> TargetRoots = new List<GameObject>();

    public List<MeshRenderer> AllRenderers = new List<MeshRenderer>();

    public bool includeInactive = true;

    [ContextMenu("GetAllRenderers")]
    public List<MeshRenderer> GetAllRenderers()
    {
        if (TargetRoots.Count == 0)
        {
            Debug.LogError("GetAllRenderers TargetRoots.Count == 0");
        }
        List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
        foreach(var target in TargetRoots)
        {
            if (target == null) continue;
            var rs = target.GetComponentsInChildren<MeshRenderer>(includeInactive);
            meshRenderers.AddRange(rs);
        }
        return meshRenderers;
    }

    public ModelClassDict<MeshRenderer> ModelClassDict = new ModelClassDict<MeshRenderer>();

    public ModelClassDict<MeshRenderer> ModelClassDict_Auto = new ModelClassDict<MeshRenderer>();

    public List<string> PrefixNames = new List<string>();

    public static List<string> DefindPrefixNames_Default = new List<string>() { "20LDB10AA" };

    public List<string> DefinedPrefixNames = new List<string>() ;

    public string TestName = "";

    public void InitPrefixNames()
    {
        foreach(var n in DefindPrefixNames_Default)
        {
            if (!DefinedPrefixNames.Contains(n))
            {
                DefinedPrefixNames.Add(n);
            }
        }

        Debug.Log("InitPrefixNames");
    }

    public string GetPrefix(string n)
    {
        foreach (var pre in DefinedPrefixNames)
        {
            if (n.StartsWith(pre))
            {
                return pre;
            }
        }
        string pre2= TransformHelper.GetPrefix(n, PrefixDividers);
        string[] parts = pre2.Split('-');
        if(parts.Length>=4)
        {
            string pr = parts[0];
            for(int i=1;i<4;i++)
            {
                pr += "-" + parts[i];
            }
            return pr;
        }
        return pre2;
    }

    [ContextMenu("TestGetPrefix")]
    private void TestGetPrefix()
    {
        TransformHelper.GetPrefix(TestName);
    }

    public char[] PrefixDividers = new char[] { ' ', '_','-' };

    [ContextMenu("GetPrefixNames")]
    public void GetPrefixNames()
    {
        List<string> otherNames = new List<string>();
        var list = GetAllRenderers();
        foreach(var item in list)
        {
            var n = item.name;

            string pre = GetPrefix(n);
            ModelClassDict_Auto.AddModel(pre, item);
        }

        ModelClassDict_Auto.PrintList();

        otherNames.Sort();
        StringBuilder sb = new StringBuilder();
        otherNames.ForEach(i => sb.AppendLine(i));
        Debug.LogError($"otherNames:{sb}");

        PrefixNames = ModelClassDict_Auto.GetKeys();
    }

    [ContextMenu("GetModels")]
    public void GetModels()
    {
        ModelClassDict = new ModelClassDict<MeshRenderer>();

        var meshRenderers = GetAllRenderers();
        foreach (var n in StartWithNames)
        {
            List<MeshRenderer> renderers1 = new List<MeshRenderer>();
            for (int i = 0; i < meshRenderers.Count; i++)
            {
                MeshRenderer renderer = meshRenderers[i];
                if (renderer.name.StartsWith(n))
                {
                    renderers1.Add(renderer);
                    meshRenderers.RemoveAt(i);
                    i--;
                }
            }
            ModelClassDict.AddModel(n, renderers1);

            Debug.Log($"GetModels AddModel name:{n} renderers:{renderers1.Count}");
        }

        Debug.Log($"GetModels Dict:{ModelClassDict.Dict.Count}");

    }

}

public class ModelClass<T>: List<T> 
{
    public string ClassName { get; set; }

    //public List<MeshRenderer> Renderers = new List<MeshRenderer>();

    public ModelClass()
    {

    }

    public ModelClass(string name,List<T> list)
    {
        this.ClassName = name;
        //this.Renderers = list;
        this.AddRange(list);
    }

    public ModelClass(string name)
    {
        this.ClassName = name;
    }

    public ModelClass(string name,T item)
    {
        this.ClassName = name;
        this.Add(item);
    }
}

public class ModelClassDict<T>
{
    public Dictionary<string, ModelClass<T>> Dict = new Dictionary<string, ModelClass<T>>();

    public void AddModel(string name,List<T> list)
    {
        if (!Dict.ContainsKey(name))
        {
            Dict.Add(name, new ModelClass<T>(name, list));
        }
        Dict[name].AddRange(list);
    }
    public void AddModel(string name, T item)
    {
        if (!Dict.ContainsKey(name))
        {
            Dict.Add(name, new ModelClass<T>(name, item));
        }
        Dict[name].Add(item);
    }

    public void PrintList()
    {
        foreach(var key in GetKeys())
        {
            var list = Dict[key];
            Debug.Log($"ModelClassDict PrintList key:{key} list:{list.Count}");
        }
    }

    public List<string> GetKeys()
    {
        var list = Dict.Keys.ToList();
        list.Sort();
        return list;
    }
}
