using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInfoManager : SingletonBehaviour<PrefabInfoManager>
{
    public PrefabInfoList prefabInfos = new PrefabInfoList();

    public void AddPrefabInfo(PrefabInfo prefabInfo)
    {
        prefabInfos.Add(prefabInfo);
    }

    public void DestroyPrefab(GameObject go)
    {
        PrefabInfo prefab=prefabInfos.FindItem(go);
        if (prefab == null)
        {
            //Debug.LogError($"PrefabInfoManager.DestroyPrefab prefab == null go:{go}");
            GameObject.DestroyImmediate(go);
            return;
        }
        else
        {
            Debug.LogWarning($"PrefabInfoManager.DestroyPrefab prefab == null go:{go} prefab:{prefab}");
            prefab.DestroyPrefab();
        }
        
    }
}
