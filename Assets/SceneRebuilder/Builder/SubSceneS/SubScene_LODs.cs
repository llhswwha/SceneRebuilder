using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubScene_LODs : SubScene_Part
{
    [ContextMenu("CreateAreaTree")]
    public AreaTreeNode CreateAreaTree()
    {
        AreaTreeNode areaTreeNode = gameObject.AddMissingComponent<AreaTreeNode>();
        //areaTreeNode.Renderers = Renderers;
        areaTreeNode.RenderersId = RenderersId;
        //areaTreeNode.CreateDictionary();  
        return areaTreeNode;
    }

    //public List<MeshRenderer> Renderers = new List<MeshRenderer>();

    public List<string> RenderersId = new List<string>();

    public void Start()
    {
        this.gameObject.SetActive(false);

        
    }

    public void CreateDictionary()
    {
        AreaTreeNode areaTreeNode = CreateAreaTree();
        if (areaTreeNode != null)
        {
            areaTreeNode.CreateDictionary();
        }
    }

    protected override string BoundsName
    {
        get
        {
            return this.name + "_Bounds_Out0_" + contentType;
        }
    }

    public override int UnLoadGosM()
    {
        SubScene_Ref.BeforeUnloadScene(this.gameObject); 
        int r= base.UnLoadGosM();
        //Renderers.Clear();
        return r;
    }

    public override void DestroyScene()
    {
        SubScene_Ref.ClearRefs(this.gameObject);
        base.DestroyScene();
    }
#if UNITY_EDITOR
    public override void EditorLoadScene()
    {
        base.EditorLoadScene();
        SubScene_Ref.AfterLoadScene(this.gameObject);
    }
#endif
    public override void GetSceneObjects()
    {
        base.GetSceneObjects();
        SubScene_Ref.AfterLoadScene(this.gameObject);
    }

    public override void SetObjects(List<GameObject> goList)
    {
        base.SetObjects(goList);

        //Renderers.Clear();
        RenderersId.Clear();
        foreach (var go in goList)
        {
            MeshRenderer[] mrs = go.GetComponentsInChildren<MeshRenderer>(true);
            foreach(var mr in mrs)
            {
                MeshRendererInfo mri = mr.GetComponent<MeshRendererInfo>();
                if (mri != null && mri.IsLodNs(2, 3, 4)) continue;
                if (mr.GetComponent<BoundsBox>() != null) continue;
                var id = RendererId.GetId(mr);
                //Renderers.Add(mr);
                RenderersId.Add(id);
            }
            RendererId[] rids= go.GetComponentsInChildren<RendererId>(true);
            foreach(var rid in rids)
            {
                //Renderers.Add(mr);
                RenderersId.Add(rid.Id);
            }
        }
    }

    [ContextMenu("SetRendererParent")]
    public override void SetRendererParent()
    {
        UpdateRidParent();
        //RendererId[] rIds = this.GetComponentsInChildren<RendererId>(true);
        //foreach (var rI in rIds)
        //{
        //    rI.SetParent();
        //    IdDictionary.SetId(rI);
        //    AreaTreeNodeShowManager.Instance.MoveRenderer(rI);
        //}
        base.SetRendererParent();
    }


}

