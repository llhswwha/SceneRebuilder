using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SubSceneArg 
{
    public string path;
    public bool isOveride = true;
    public bool isOpen = false;
    public GameObject[] objs;

    public SubSceneArg()
    {

    }
    public SubSceneArg(string path, bool isOveride, bool isOpen, params GameObject[] objs)
    {
        this.path = path;
        this.isOveride = isOveride;
        this.isOpen = isOpen;
        this.objs = objs;
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
