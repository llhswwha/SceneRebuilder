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

    public void ClickSetDistance()
    {
        float dis=float.Parse(inputDistance.text);
        areaTreeNodeShowManager.ShowNodeDistance=dis;
        ClickUpdateInfo();
    }

    public void ClickUpdateInfo()
    {
        ModelInfo.text=ShowModelInfo();
    }

    [ContextMenu("ShowModelInfo")]
    private string ShowModelInfo()
    {
        int renderCount = 0;
        var renderers = GameObject.FindObjectsOfType<MeshRenderer>();
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
            vertextCount += meshFilter.sharedMesh.vertexCount;
            renderCount++;
            Debug.Log("render:"+render.name+"|"+render.transform.parent);
        }
        int w = vertextCount / 10000;

        //ModelInfoText.text = $"renders:{renderCount}(+{renderCount - lastRenderCount}),mats:{mats.Count}(+{mats.Count - lastMatCount}) \nvertext:{w}w(+{w - lastVertextCount})";

        //lastRenderCount = renderCount;
        //lastVertextCount = w;
        //lastMatCount = mats.Count;

        Debug.LogError($"ShowModelInfo renders:{renderCount},mats:{mats.Count},vertext:{w}w");
        return $"renders:{renderCount},mats:{mats.Count},vertext:{w}w";
    }

    private void Start()
    {
        if (areaTreeNodeShowManager == null)
            areaTreeNodeShowManager = GameObject.FindObjectOfType<AreaTreeNodeShowManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ShowHideInfo.text=string.Format("Show:{0},Hide:{1},Static:{2}",areaTreeNodeShowManager.ShownNodes.Count,areaTreeNodeShowManager.HiddenNodes.Count,areaTreeNodeShowManager.ShownLeafNodes.Count);
    }
}
