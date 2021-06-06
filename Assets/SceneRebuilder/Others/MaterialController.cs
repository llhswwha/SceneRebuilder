using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    public Material material;

    public Material[] materials;

    //public Material sharedMaterial;

    //public Material[] sharedMaterials;

    public Shader[] shaders;

    public MeshRenderer meshRender;

    //public Collider collider;

    void Awake()
    {
        GetMaterials();
        //collider = this.gameObject.GetComponent<Collider>();
        //if (collider == null)
        //{
        //    Debug.LogError("collider==null:" + this);
        //}
    }

    private bool isFirst = true;

    [ContextMenu("GetMaterials")]
    public void GetMaterials()
    {
        try
        {
            if (isFirst == false) return;
            isFirst = false;
            meshRender = this.gameObject.GetComponent<MeshRenderer>();
            material = meshRender.material;
            materials = meshRender.materials;

            //sharedMaterial = meshRender.sharedMaterial;
            //sharedMaterials = meshRender.sharedMaterials;

            shaders = new Shader[meshRender.materials.Length];
            for (int i = 0; i < meshRender.materials.Length; i++)
            {
                //meshRender.materials[i] = new Material(Transparent_Mat);
                shaders[i] = meshRender.materials[i].shader;
            }

        }
        catch (System.Exception ex)
        {
            Debug.LogError("Awake:"+ex);
        }
    }

    [ContextMenu("ClearMaterial")]
    public void ClearMaterial()
    {
        meshRender.materials = new Material[0];
    }

    [ContextMenu("ResetMaterial")]
    public void ResetMaterial()
    {
        //meshRender.materials = materials;
        for (int i = 0; i < meshRender.materials.Length; i++)
        {
            //meshRender.materials[i] = new Material(Transparent_Mat);
            meshRender.materials[i].shader=shaders[i];
        }
    }

    [ContextMenu("SetTransparent")]
    public void SetTransparent()
    {
        print("SetTransparent:"+this);
        for (int i = 0; i < meshRender.materials.Length; i++)
        {
            //meshRender.materials[i] = new Material(Transparent_Mat);
            meshRender.materials[i].shader = NodeHighlightSystem.Instance.Transparent_Shader;
        }

        //meshRender.material = Transparent_Mat;
    }

    [ContextMenu("SetHighlight")]
    public void SetHighlight()
    {
        GetMaterials();
        for (int i = 0; i < meshRender.materials.Length; i++)
        {
            //meshRender.materials[i] = new Material(Highlight_Mat);
            meshRender.materials[i].shader = NodeHighlightSystem.Instance.Hightlight_Shader;
        }
        //meshRender.material = Highlight_Mat;
    }

    public void OnMouseEnter()
    {
        //print("OnMouseEnter");
    }

    public void OnMouseExit()
    {
        //print("OnMouseExit");
    }
}
