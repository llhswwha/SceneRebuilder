using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CategoryInfoList:List<CategoryInfo>
{
    public static CategoryInfoList Instance;

    public CategoryInfoList()
    {
        Instance = this;
    }

    CategoryInfo other;
    public CategoryInfoList(string txt)
    {
        Instance = this;
        string[] lines = txt.Split('\n');
        foreach (var item in lines)
        {
            var line = item.Trim();
            if (string.IsNullOrEmpty(line)) continue;
            CategoryInfo info = new CategoryInfo(item);
            this.Add(info);
        }

        other = new CategoryInfo("Other:Other:");
        this.Add(other);
    }

    internal CategoryInfo GetCategory(string name)
    {
        foreach (var item in this)
        {
            bool b1 = item.IsThis(name);
            if (b1 == true)
            {
                return item;
            }
        }
        return other;
    }
}

public class CategoryInfo
{
    public string ClassName;
    public string CategoryName;
    public List<string> TypeKeys = new List<string>();

    public CategoryInfo()
    {

    }
    public CategoryInfo(string line)
    {
        //Frame:地板:Floor_;底板_;楼板_;_楼板边缘_;排水沟;_钢盖板_;_坡道_;
        string[] parts1 = line.Trim().Split(':');
        ClassName = parts1[0];
        CategoryName = parts1[1];
        string[] parts2 = parts1[2].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        TypeKeys = parts2.ToList();
    }

    public bool IsThis(string typeName)
    {
        foreach (var item in TypeKeys)
        {
            if (typeName.Contains(item))
            {
                return true;
            }
        }
        return false;
    }

    public override string ToString()
    {
        return ClassName + "." + CategoryName;
    }
}
