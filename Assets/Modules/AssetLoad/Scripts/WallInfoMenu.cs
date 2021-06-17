using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class WallInfoMenu : MonoBehaviour {

    //public static GameObject j1;
    [MenuItem("Tools/SetWallInfos")]
	static void SetWallInfos()
    {
        Debug.Log("SetWallInfos");
        GameObject j1 = GameObject.Find("J1");
        BuildingController bc = j1.GetComponent<BuildingController>();
        bc.isExpandFloor = false;
        bc.isHideWall = true;
        bc.TopFloor = new List<GameObject>();
        bc.TopFloor.Add(j1.transform.FindChildByName("J1_TOP").gameObject);
        Debug.Log(j1);
        //var b1=j1.transform.FindChildByName("F2_WallB1");
        //var b1c=b1.gameObject.AddComponent<WallController>();
        //b1c.direction = new Vector3(0, 0, -1);
        SetWallInfo(j1, "F2_WallB1", new Vector3(0, 0, -1));
        SetWallInfo(j1, "F2_WallB2", new Vector3(0, 0, 1));
        SetWallInfo(j1, "F2_WallB3", new Vector3(-1, 0, 0));
        SetWallInfo(j1, "F2_WallB4", new Vector3(1, 0, 0));

        SetWallInfo(j1, "F1_WallA1", new Vector3(0, 0, -1));
        SetWallInfo(j1, "F1_WallA2", new Vector3(0, 0, 1));
        SetWallInfo(j1, "F1_WallA3", new Vector3(-1, 0, 0));
        SetWallInfo(j1, "F1_WallA4", new Vector3(1, 0, 0));
        var f1 = j1.transform.FindChildByName("J1_F1");
        var f1devices=f1.gameObject.AddMissingComponent<DeviceAssetInfo>();
        f1devices.SceneName = "Devices_J1_F1";
        f1devices.AssetName = "Devices_J1_F1";
        var f2 = j1.transform.FindChildByName("J1_F2");
        var f2devices = f2.gameObject.AddMissingComponent<DeviceAssetInfo>();
        f2devices.SceneName = "Devices_J1_F2";
        f2devices.AssetName = "Devices_J1_F2";
        /*
F2_WallB1 (0,0,-1)
F2_WallB2 (0,0,1)
F2_WallB3 (-1,0,0)
F2_WallB4 (1,0,0)
         */
    }

    static void SetWallInfo(GameObject parent, string name, Vector3 dir)
    {
        var b1 = parent.transform.FindChildByName(name);
        if (b1 == null)
        {
            Debug.LogError("未找到:" + name);
            return;
        }
        var b1c = b1.gameObject.AddMissingComponent<WallController>();
        b1c.direction = dir;
    }
}
#endif