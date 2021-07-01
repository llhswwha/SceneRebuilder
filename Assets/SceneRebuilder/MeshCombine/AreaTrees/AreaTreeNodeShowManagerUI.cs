using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaTreeNodeShowManagerUI : MonoBehaviour
{
    public AreaTreeNodeShowManager areaTreeNodeShowManager;
    public InputField inputDistance;

    public Text ModelInfo;

    public Text ShowHideInfo;

    public Toggle ToggleIsDisToBounds;

    public void ClickSetDistance()
    {
        Debug.Log("ClickSetDistance");
        float dis=float.Parse(inputDistance.text);
        areaTreeNodeShowManager.ShowNodeDistance=dis;
        areaTreeNodeShowManager.IsDisToBounds = ToggleIsDisToBounds.isOn;
        ClickUpdateInfo();
    }

    public void ClickUpdateInfo()
    {
        Debug.Log("ClickUpdateInfo");
        ModelInfo.text=ShowModelInfo();
    }

    //private MeshRenderer[] allRenderers = null;

    [ContextMenu("ShowModelInfo")]
    private string ShowModelInfo()
    {
        int renderCount = 0;
        var renderers = GameObject.FindObjectsOfType<MeshRenderer>(false);
        int vertextCount = 0;
        List<Material> mats = new List<Material>();
        foreach (var render in renderers)
        {
            if (render.enabled == false) continue;
            if (!mats.Contains(render.sharedMaterial))
            {
                mats.Add(render.sharedMaterial);
            }
            MeshFilter meshFilter = render.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;
            if (meshFilter.sharedMesh == null) continue;
            vertextCount += meshFilter.sharedMesh.vertexCount;
            renderCount++;
            //Debug.Log("render:"+render.name+"|"+render.transform.parent);
        }
        int w = vertextCount / 10000;

        //ModelInfoText.text = $"renders:{renderCount}(+{renderCount - lastRenderCount}),mats:{mats.Count}(+{mats.Count - lastMatCount}) \nvertext:{w}w(+{w - lastVertextCount})";

        //lastRenderCount = renderCount;
        //lastVertextCount = w;
        //lastMatCount = mats.Count;

        Debug.LogError($"ShowModelInfo renders:{renderCount}/{renderers.Length},mats:{mats.Count},vertext:{w}w");
        return $"renders:{renderCount},mats:{mats.Count},vertext:{w}w";
    }

    private void Start()
    {
        if (areaTreeNodeShowManager == null)
            areaTreeNodeShowManager = GameObject.FindObjectOfType<AreaTreeNodeShowManager>(true);
    }

    public int ShowNodeCount = 0;

    public Toggle ToggleAutoUpdate;

    // Update is called once per frame
    void Update()
    {
        if(areaTreeNodeShowManager!=null){
            int count1= areaTreeNodeShowManager.ShownNodes.Count;
            //if (ShowNodeCount != count1)
            //{
            //    ClickUpdateInfo();
            //}
            ShowNodeCount = count1;
            ShowHideInfo.text=string.Format("ShowNode:{0},HideNode:{1},StaticNode:{2}", ShowNodeCount, areaTreeNodeShowManager.HiddenNodes.Count,areaTreeNodeShowManager.ShownLeafNodes.Count);
        }
    }
}
