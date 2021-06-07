using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModelCategory
{
    public string Name = "";

    public CategoryInfo Info;

    public NodeTypeInfoList TypeList = new NodeTypeInfoList();

    public ModelCategory(CategoryInfo info)
    {
        this.Info = info;
        Name = info.ToString();
    }

    public void AddType(NodeTypeInfo typeInfo)
    {
        TypeList.Add(typeInfo);
    }
}

public class ModelCategoryList:List<ModelCategory>
{
    public Dictionary<CategoryInfo, ModelCategory> Dic = new Dictionary<CategoryInfo, ModelCategory>();

    public ModelCategoryList(CategoryInfoList categoryInfos,NodeTypeInfoList typeList)
    {
        foreach (var typeInfo in typeList)
        {
            string name = typeInfo.typeName;
            CategoryInfo categoryInfo = categoryInfos.GetCategory(name);

            ModelCategory category = null;
            if (Dic.ContainsKey(categoryInfo))
            {
                category = Dic[categoryInfo];
            }
            else
            {
                category = new ModelCategory(categoryInfo);
                Dic.Add(categoryInfo, category);
                this.Add(category);
            }
            category.AddType(typeInfo);
        }
    }

}

