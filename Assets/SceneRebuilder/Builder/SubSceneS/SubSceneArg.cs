using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SubSceneArg 
{
    public string path;
    public int index;
    public bool isOveride = true;
    public bool isOpen = false;
    public List<GameObject> objs = new List<GameObject>();

    public SubSceneArg()
    {

    }
    public SubSceneArg(string path, bool isOveride, bool isOpen, bool isOnlyChildren, params GameObject[] gos)
    {
        this.path = path;
        this.isOveride = isOveride;
        this.isOpen = isOpen;
        //this.objs = objs;
        if (isOnlyChildren)
        {
            foreach(var go in gos)
            {
                for(int i=0;i< go.transform.childCount;i++)
                {
                    objs.Add(go.transform.GetChild(i).gameObject);
                }
            }
        }
        else
        {
            this.objs.AddRange(gos);
        }
    }

    public string GetRalativePath()
    {
        //string rPath = scenePath;
        //if(rPath.Contains(":"))
        //{
        //    rPath = EditorHelper.PathToRelative(rPath);
        //}
        //return rPath;

        string rPath = EditorHelper.PathToRelative(path);
        return rPath;
    }
}
