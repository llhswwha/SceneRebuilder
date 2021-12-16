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

    public string TestName = "";

    [ContextMenu("TestGetPrefix")]
    private void TestGetPrefix()
    {
        TransformHelper.GetPrefix(TestName);
    }

    

    [ContextMenu("GetPrefixNames")]
    public void GetPrefixNames()
    {
        List<string> otherNames = new List<string>();
        var list = GetAllRenderers();
        foreach(var item in list)
        {
            var n = item.name;
            string pre = TransformHelper.GetPrefix(n);
            ModelClassDict_Auto.AddModel(pre, item);

            //int id = 0;
            //int id1 = n.LastIndexOf(' ');
            //int id2 = n.LastIndexOf('-');
            //int id3 = n.LastIndexOf('-');
            ////12-3 2
            //if (id1 > id)
            //{
            //    id = id1;
            //}
            //if (id2 > id)
            //{
            //    id = id2;
            //}
            //if (id3 > id)
            //{
            //    id = id3;
            //}
            ////if (id == 0 && n.Length>9)
            ////{
            ////    id = 9;//？？？
            ////}

            ////最后一个是数字或者英文字母的情况，最后是多个数字的情况
            //if(id>0)
            //{
            //    string pre = n.Substring(0, id+1);
            //    string after = n.Substring(id + 1);
            //    ModelClassDict_Auto.AddModel(pre, item);
            //}
            //else 
            //{
            //    //otherNames.Add(n);
            //    ModelClassDict_Auto.AddModel(n, item);
            //}
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
