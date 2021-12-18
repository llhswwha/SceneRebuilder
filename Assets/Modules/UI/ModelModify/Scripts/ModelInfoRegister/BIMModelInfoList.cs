using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BIMModelInfo;

public class BIMModelInfoList : List<BIMModelInfo>
{
    public BIMModelInfoList()
    {

    }

    public BIMModelInfoList(List<BIMModelInfo> list)
    {
        this.AddRange(list);
    }

    public List<BIMModelInfo> FindSameList(List<BIMModelInfo> list2)
    {
        Dictionary<string, BIMModelInfo> dict = new Dictionary<string, BIMModelInfo>();
        foreach (var item in this)
        {
            if (dict.ContainsKey(item.Guid))
            {
                Debug.LogError($"dict.ContainsKey(item.Guid) guid:{item.Guid}");
            }
            else
            {
                dict.Add(item.Guid, item);
            }

        }
        List<BIMModelInfo> result = new List<BIMModelInfo>();
        foreach (BIMModelInfo item in list2)
        {
            if (dict.ContainsKey(item.Guid))
            {
                BIMModelInfo item1 = dict[item.Guid];
                result.Add(item1);
            }
            else
            {
                Debug.LogError($"not found BIM dict.ContainsKey(item.Guid)==false guid:{item.Guid}");
            }
        }
        Debug.Log($"FindSameList list2:{list2.Count} result:{result.Count}");
        return result;
    }

    public BIMModelInfoList FindList(string key)
    {
        //doorRootArg.caption = $"[{i + 1:00}] [{listItem.IsFound}][{listItem.FoundType}] {listItem.name}>{listItem.MName}({listItem.MId})"
        var list1 = this.FindAll(i => i.name.Contains(key) || i.MName.Contains(key) || i.MId.Contains(key));
        return new BIMModelInfoList(list1);
    }

    public BIMModelInfoList FindListByType(BIMFoundType foundType)
    {
        var list1 = this.FindAll(a => a.FoundType == foundType);
        return new BIMModelInfoList(list1);
    }
}
