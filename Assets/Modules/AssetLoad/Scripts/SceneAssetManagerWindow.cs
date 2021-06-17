using Base.Common.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneAssetManagerWindow : MonoBehaviour {

    public GameObject window;

    public SceneAssetManager manager;

    public Text LoadCount;
    public Text DeviceLoadCount;
    public Text VertexCount;
    public InputField UnloadMode;
    public InputField CacheCount;
    public InputField DeviceCacheCount;
    public InputField MaxVertex;
    public Toggle LoadFromFile;
    public InputField HttpUrl;

    public void RefreshInfo()
    {
        LoadCount.text = manager.BuildingCount + "";
        DeviceLoadCount.text = manager.SimgleDeviceCount + "";
        VertexCount.text = manager.SceneVertexs + "";
    }


    public void LoadSetting()
    {
        UnloadMode.text = manager.UnloadMode + "";
        CacheCount.text = manager.CacheCount + "";
        DeviceCacheCount.text = manager.DeviceCacheCount + "";
        MaxVertex.text = manager.MaxVertex + "";
        LoadFromFile.isOn = manager.LoadFromFile;
        HttpUrl.text = manager.HttpUrl + "";
    }

    public void SaveSetting()
    {
        manager.UnloadMode = UnloadMode.text.ToInt();
        manager.CacheCount = CacheCount.text.ToInt();
        manager.DeviceCacheCount = DeviceCacheCount.text.ToInt();
        manager.MaxVertex = MaxVertex.text.ToInt();
        manager.LoadFromFile=LoadFromFile.isOn;
        manager.HttpUrl = HttpUrl.text;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void ShowSetting()
    {
        window.SetActive(true);
    }

    public void HideSetting()
    {
        window.SetActive(false);
    }
}
