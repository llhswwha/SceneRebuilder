using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitNavisFileInfoByModelSetting : SingletonBehaviour<InitNavisFileInfoByModelSetting>
{
    public bool IsFindInCurrentModels = false;

    public List<string> CurrentModels = new List<string>();

    public List<GameObject> initInfoBuildings = new List<GameObject>();

    [ContextMenu("UpdateBuildings")]
    public void UpdateBuildings()
    {
        //initInfoBuildings.Clear();
        var bs1 = this.initInfoBuildings;
        this.initInfoBuildings.Clear();
        foreach (var b in bs1)
        {
            if (b == null) continue;
            this.initInfoBuildings.Add(b.gameObject);
        }

        BuildingController[] bs = GameObject.FindObjectsOfType<BuildingController>(true);
        foreach (var b in bs)
        {
            if (this.initInfoBuildings.Contains(b.gameObject)) continue;
            this.initInfoBuildings.Add(b.gameObject);
        }
    }

    public bool enableDistance2 = false;

    public bool enableDistance3 = false;

    public float MinDistance1 = 0.005f;

    public float MinDistance2 = 0.05f;

    public float MinDistance3 = 0.15f;

    public float MinDistance4 = 0.3f;

    public float MinDistance5 = 0.6f;

    public List<string> FilterNames1 = new List<string>() { "In", "Out0", "Out1", "LOD", "LODs" };

    public List<string> FilterNames2 = new List<string>() { "_F1", "_F2", "_F3", "_F4", "_F5", "_F6" };

    public List<string> FilterNames3 = new List<string>() { "合成部分" };
}
