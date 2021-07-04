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
        
        var renderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        int vertextCount = 0;
        int renderCount = 0;
        int vertextCount_hidden = 0;
        int renderCount_hidden = 0;
        int vertextCount_all = 0;
        int renderCount_all = renderers.Length;
        List<Material> mats = new List<Material>();
        foreach (var render in renderers)
        {
            
            if (!mats.Contains(render.sharedMaterial))
            {
                mats.Add(render.sharedMaterial);
            }
            MeshFilter meshFilter = render.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;
            if (meshFilter.sharedMesh == null) continue;
            var vc=meshFilter.sharedMesh.vertexCount;
            vertextCount_all += vc;
            if (render.enabled == false && render.gameObject.activeInHierarchy==false)
            {
                renderCount_hidden++;
                vertextCount_hidden += vc;
            }
            else{
                vertextCount += vc;
                renderCount++;
            }
            
            //Debug.Log("render:"+render.name+"|"+render.transform.parent);
        }
        //int w = vertextCount / 10000;

        //ModelInfoText.text = $"renders:{renderCount}(+{renderCount - lastRenderCount}),mats:{mats.Count}(+{mats.Count - lastMatCount}) \nvertext:{w}w(+{w - lastVertextCount})";

        //lastRenderCount = renderCount;
        //lastVertextCount = w;
        //lastMatCount = mats.Count;
        string log=$"mats:{mats.Count},renders:{renderCount}/{renderers.Length},vertext:{vertextCount/ 10000}w/{vertextCount_all/1000}\nhiddenR:{renderCount_hidden},hiddenV:{vertextCount_hidden/10000}w";
        Debug.LogError($"ShowModelInfo {log}");
        return log;
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
