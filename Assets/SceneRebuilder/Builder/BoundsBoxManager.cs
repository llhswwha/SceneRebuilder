using AdvancedCullingSystem.DynamicCullingCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsBoxManager : SingletonBehaviour<BoundsBoxManager>
{
    public BoundsBox[] boxes;

    public bool IsHide = false;
    public bool IsDestroyMesh = false;

    private void Start()
    {
        DynamicCulling dynamicCulling = GameObject.FindObjectOfType<DynamicCulling>(true);
        if (dynamicCulling != null)
        {
            IsDestroyMesh = true;
        }
        //#if !UNITY_EDITOR
        if (IsHide)
            HideBoxes();
        if (IsDestroyMesh)
            DestroyBoxes();
//#endif
    }

    [ContextMenu("GetBoxes")]
    public void GetBoxes()
    {
        boxes=GameObject.FindObjectsOfType<BoundsBox>(true);
        int showCount=0;
        int hidenCount=0;
        foreach(var box in boxes){
            if(box.gameObject.activeInHierarchy){
                hidenCount++;
            }
            else{
                showCount++;
            }
        }
        Debug.Log($"GetBoxes boxes:{boxes.Length} showCount:{showCount} hidenCount:{hidenCount}");
    }

    [ContextMenu("HideBoxes")]
    public void HideBoxes()
    {
        GetBoxes();
        foreach(var box in boxes){
            MeshRenderer renderer=box.GetComponent<MeshRenderer>();
            if(renderer==null)continue;
            renderer.enabled=false;

            //box.gameObject.SetActive(false);
        }
    }

    [ContextMenu("DestroyBoxes")]
    public void DestroyBoxes()
    {
        GetBoxes();
        foreach(var box in boxes){
            if(box==null)continue;
            if(box.gameObject==null)continue;
            //GameObject.DestroyImmediate(box.gameObject);
            //GameObject.DestroyImmediate(box);
            box.DestroyMesh();
        }
    }

    [ContextMenu("ShowBoxes")]
    public void ShowBoxes()
    {
        GetBoxes();
        foreach(var box in boxes){
            //box.gameObject.SetActive(true);
            MeshRenderer renderer = box.GetComponent<MeshRenderer>();
            if (renderer == null) continue;
            renderer.enabled = true;
        }
    }

    [ContextMenu("DisableShadow")]
    public void DisableShadow()
    {
        GetBoxes();
        foreach(var box in boxes){
            MeshRenderer renderer=box.GetComponent<MeshRenderer>();
            if(renderer==null)continue;
            renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;

            //box.gameObject.SetActive(false);
        }
    }

    public GameObject SourceRoot;

    public GameObject TargetRoot;

    [ContextMenu("MoveSourceToTargetByBounds")]
    public void MoveSourceToTargetByBounds()
    {

    }
}
