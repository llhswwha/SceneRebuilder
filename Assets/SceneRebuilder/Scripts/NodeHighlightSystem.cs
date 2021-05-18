using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeHighlightSystem : MonoBehaviour
{
    public Material Transparent_Mat;

    public Material Highlight_Mat;

    public Shader Transparent_Shader;

    public Shader Hightlight_Shader;

    public static NodeHighlightSystem Instance;

    public List<Material> allMaterials = new List<Material>();

    public List<Material> allSharedMaterials = new List<Material>();

    public List<Shader> allSharedShaders = new List<Shader>();

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    [ContextMenu("GetAllMaterials")]
    public void GetAllMaterials()
    {
        allMaterials.Clear();
        MeshRenderer[] meshRenders = GameObject.FindObjectsOfType<MeshRenderer>();
        foreach (var meshRender in meshRenders)
        {
            //MaterialController controller = meshRender.gameObject.AddComponent<MaterialController>();
            Material[] ms = meshRender.materials;
            foreach (var m in ms)
            {
                if (!allMaterials.Contains(m))
                {
                    allMaterials.Add(m);
                }
            }
        }
    }

    [ContextMenu("GetAllSharedMaterials")]
    public void GetAllSharedMaterials()
    {
        allSharedMaterials.Clear();
        allSharedShaders.Clear();
        MeshRenderer[] meshRenders = GameObject.FindObjectsOfType<MeshRenderer>();
        foreach (var meshRender in meshRenders)
        {
            //MaterialController controller = meshRender.gameObject.AddComponent<MaterialController>();
            Material[] ms = meshRender.sharedMaterials;
            foreach (var m in ms)
            {
                if (m == null) continue;
                if (!allSharedMaterials.Contains(m))
                {
                    allSharedMaterials.Add(m);
                    allSharedShaders.Add(m.shader);
                }
            }
        }
    }

    [ContextMenu("TransparentOnAll")]
    public void TransparentOnAll()
    {
        //MeshRenderer[] meshRenders = GameObject.FindObjectsOfType<MeshRenderer>();
        //foreach (var meshRender in meshRenders)
        //{
        //    MaterialController controller = meshRender.gameObject.AddComponent<MaterialController>();
        //    controller.SetTransparent();
        //}
        if (allSharedMaterials.Count == 0)
        {
            GetAllSharedMaterials();
        }
        
        foreach (var item in allSharedMaterials)
        {
            item.shader = Transparent_Shader;
        }
    }

    [ContextMenu("TransparentOffAll")]
    public void TransparentOffAll()
    {
        ResetMaterial();
    }

    [ContextMenu("ResetMaterial")]
    public void ResetMaterial()
    {
        //MaterialController[] meshRenders = GameObject.FindObjectsOfType<MaterialController>();
        //foreach (MaterialController meshRender in meshRenders)
        //{
        //    meshRender.ResetMaterial();
        //}

        for (int i = 0; i < allSharedMaterials.Count; i++)
        {
            Material item = allSharedMaterials[i];
            item.shader = allSharedShaders[i];
        }
    }

    [ContextMenu("TestHighlightOn")]
    public void TestHighlightOn()
    {
        MeshRenderer[] meshRenders = GameObject.FindObjectsOfType<MeshRenderer>();
        foreach (var meshRender in meshRenders)
        {
            MaterialController controller = meshRender.gameObject.AddComponent<MaterialController>();
            controller.SetHighlight();
        }
    }

    [ContextMenu("TestHighlightOff")]
    public void TestHighlightOff()
    {
        ResetMaterial();
    }

    MaterialController materialController;

	public List<MaterialController> materialControllerList = new List<MaterialController>();

    public bool IsTest = false;

    public void Update()
    {
        if (IsTest)
        {
            if (Input.GetMouseButton(0))
            {
                //print("click");

                //Physics.Raycast()

                //if (NodeInfoScript.Current != null)
                //{
                //    TransparentOnAll();
                //    if (materialController != null)
                //    {
                //        materialController.ResetMaterial();
                //    }

                //    MeshRenderer meshRender = NodeInfoScript.Current.GetComponent<MeshRenderer>();
                //    materialController = meshRender.gameObject.AddComponent<MaterialController>();
                //    materialController.SetHighlight();
                //}

                if (NodeInfoScript.Current != null)
                {
                    HighlightOn(NodeInfoScript.Current.gameObject);
                }

            }
        }
    }

    public void HighlightOn(GameObject target)
    {
        if (target != null)
        {
            TransparentOnAll();
            if (materialController != null)
            {
                materialController.ResetMaterial();
            }

            MeshRenderer meshRender = target.GetComponent<MeshRenderer>();
            materialController = meshRender.gameObject.AddComponent<MaterialController>();
            materialController.SetHighlight();
        }
    }

    public void HighlightOff(GameObject obj)
    {
        TransparentOffAll();
        if (materialController != null)
        {
            materialController.ResetMaterial();
        }
    }
	
	public void Highlight(List<GameObject> objs)
    {
        if (objs == null)
        {
            return;
        }

        TransparentOnAll();

        foreach (var item in materialControllerList)
        {
            item.ResetMaterial();
        }

        foreach (var target in objs)
        {
            MeshRenderer meshRender = target.GetComponent<MeshRenderer>();
            materialController = meshRender.gameObject.AddComponent<MaterialController>();
            materialController.SetHighlight();
            materialControllerList.Add(materialController);
        }
        
    }

    public void OnDestroy()
    {
        TransparentOffAll();
    }
}
