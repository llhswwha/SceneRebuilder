// using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Icao;
using RevitTools.Infos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class ModelBuildingInfo 
{
    public List<ModelLevelInfo> Levels = new List<ModelLevelInfo>();

    private Dictionary<string, ModelLevelInfo> levelIndex = new Dictionary<string, ModelLevelInfo>();

    public ModelLevelInfo GetLevel(string levelName,double levelHeight,int levelId)
    {
        ModelLevelInfo level = null;
        if (levelIndex.ContainsKey(levelName))
        {
            level = levelIndex[levelName];
        }
        else
        {
            level = new ModelLevelInfo(levelName, levelHeight, levelId);
            Levels.Add(level);
            levelIndex.Add(levelName, level);

            Levels.Sort();
        }
        //ModelLevelInfo level = Levels.FirstOrDefault(i => i.Name == levelName);
        //if (level == null)
        //{
        //    level = new ModelLevelInfo(levelName);
        //    Levels.Add(level);
        //}
        return level;
    }

    public ModelBuildingInfo(List<NodeInfo> nodes,ElementGroupList elementGroups, CategoryInfoList categoryInfos)
    {
        if (elementGroups == null)
        {
            Debug.LogWarning("elementGroups == null");
            return;
        }
        foreach (var item in nodes)
        {
            var ele = elementGroups.GetElementInfo(item.GetId(), item.nodeName);
            if (ele != null)
            {
                var lvName = ele.LevelName;
                if (!string.IsNullOrEmpty(lvName))
                {
                    var level = GetLevel(lvName,ele.LevelHeight, ele.LevelId);
                    level.AddNode(item);
                }
                else
                {
                    if (ele.FamilyName == "焊缝") //焊缝没关系，不显示就是了
                    {
                        Debug.LogWarning("LevelName == null :" + ele);
                    }
                    else
                    {
                        Debug.LogError("LevelName == null :" + ele);
                    }
                    
                }
            }
            else
            {
                Debug.LogError("ElementInfo == null :"+item);
            }
        }

        foreach (var level in Levels)
        {
            level.InitTypeList(categoryInfos);
        }
    }
}
