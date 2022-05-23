using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LODTwoRenderers
{
    public MeshRendererInfo renderer_old;
    public string renderer_old_name = "";
    public int vertexCount0;

    public MeshRendererInfo renderer_new;
    public string renderer_new_name = "";
    public int vertexCount1;

    public float dis = 1000f;
    public float meshDis = 1000f;

    public bool isSameName = false;

    public LODTwoRenderers(MeshRendererInfo lod0, MeshRendererInfo lod1, float d, float meshD, int vertexCount0, int vertexCount1)
    {
        //renderer_lod0 = lod0;
        //renderer_lod0_name = renderer_lod0.name;
        //this.vertexCount0 = vertexCount0;
        SetLOD0(lod0);
        SetLOD1(lod1, d, meshD, vertexCount0, vertexCount1);
    }

    public void SetMinDisTarget(MinDisTarget<MeshRendererInfo> min)
    {
        float minDis = min.dis;
        MeshRendererInfo newRenderer = min.target;
        if (newRenderer == null)
        {
            Debug.LogError("render_lod0 == null");
            return; ;
        }
        //MeshFilter filter1 = render_lod1.GetComponent<MeshFilter>();
        //MeshFilter filter0 = render_lod0.GetComponent<MeshFilter>();
        int vertexCount0 = newRenderer.GetMinLODVertexCount();
        int vertexCount1 = renderer_old.GetMinLODVertexCount();
        //LODTwoRenderers lODTwoRenderers = new LODTwoRenderers(render_lod0, render_lod1, minDis, min.meshDis, vertexCount0, vertexCount1);
        //this.Add(lODTwoRenderers);

        this.SetLOD1(newRenderer, minDis, min.meshDis, vertexCount0, vertexCount1);
    }

    public void SetLOD1(GameObject obj2)
    {
        MeshRendererInfo newRenderer = MeshRendererInfo.GetInfo(obj2, false);
        int vertexCount0 = newRenderer.GetMinLODVertexCount();
        int vertexCount1 = renderer_old.GetMinLODVertexCount();
        float minDis = Vector3.Distance(obj2.transform.position, renderer_old.transform.position);
        float meshDis = MeshHelper.GetVertexDistanceEx(obj2, renderer_old.gameObject);
        this.SetLOD1(newRenderer, minDis, meshDis, vertexCount0, vertexCount1);
    }

    public void SetLOD1(MeshRendererInfo lod1, float d, float meshD, int vertexCount0, int vertexCount1)
    {

        renderer_new = lod1;
        renderer_new_name = renderer_new.name;
        this.vertexCount1 = vertexCount1;
        this.vertexCount1 = lod1.GetVertexCount();

        dis = d;
        meshDis = meshD;

        //if(renderer_lod0_name.Contains("LOD")&& renderer_lod1_name.Contains("LOD"))
        {
            renderer_old_name = LODHelper.GetOriginalName(renderer_old_name);
            renderer_new_name = LODHelper.GetOriginalName(renderer_new_name);
        }

        this.isSameName = renderer_old_name == renderer_new_name;
    }

    public LODTwoRenderers(MeshRendererInfo lod0)
    {
        SetLOD0(lod0);
    }

    public LODTwoRenderers(MeshRenderer renderer)
    {
        var lod0 = MeshRendererInfo.GetInfo(renderer, false);
        SetLOD0(lod0);
    }

    public LODTwoRenderers(GameObject go)
    {
        var lod0 = MeshRendererInfo.GetInfo(go, false);
        SetLOD0(lod0);
    }

    private void SetLOD0(MeshRendererInfo lod0)
    {
        renderer_old = lod0;
        renderer_old_name = renderer_old.name;
        this.vertexCount0 = (int)lod0.GetVertexCount();
    }

    private string GetDisCompareStr()
    {
        string dis1 = dis.ToString("F5");
        if (dis >= 1)
        {
            dis1 = dis.ToString("F2");
        }
        else if (dis >= 0.1)
        {
            dis1 = dis.ToString("F3");
        }
        else if (dis == 0)
        {
            dis1 = dis.ToString("F0");
        }

        string meshDis1 = meshDis.ToString("F5");
        if (meshDis >= 1)
        {
            meshDis1 = meshDis.ToString("F2");
        }
        else if (meshDis >= 1)
        {
            meshDis1 = meshDis.ToString("F3");
        }
        else if (meshDis == 0)
        {
            meshDis1 = meshDis.ToString("F0");
        }

        string disCompare = $"<{dis1}|{meshDis1}>";
        return disCompare;
    }

    public string GetCompareCaption(bool isShowSize, bool isShowMaterial)
    {
        if (renderer_old == null) return "";
        string logName = renderer_old_name;
        if (logName.Length > 20)
        {
            logName = logName.Substring(0, 20) + "...";
        }
        var mat0 = renderer_old.GetMats();
        if (isShowMaterial == false)
        {
            mat0 = "mat0";
        }
        string lod0 = $"\"{logName}\" ({MeshHelper.GetVertexCountS(vertexCount0)}w)[{mat0}]";
        if (isShowSize)
        {
            lod0 += $"({renderer_old.size})";
        }
        string disCompare = GetDisCompareStr();
        //string lod0 = $"\"{logName}\" ({MeshHelper.GetVertexCountS(vertexCount0)}w)";
        if (renderer_new == null)
        {
            return $"{disCompare} {lod0}| ({renderer_old.GetRendererTypesS()})({renderer_old.GetLODIds()})"; ;
        }
        bool isSameName = renderer_new_name == renderer_old_name;
        bool isSameSize = renderer_new.size.ToString() == renderer_old.size.ToString();
        if (isSameSize == false)
        {

        }
        float p10 = (float)vertexCount1 / vertexCount0;
        var mat1 = renderer_new.GetMats();
        if (isShowMaterial == false)
        {
            mat1 = "mat1";
        }
        string lod1 = $"{renderer_new_name}({MeshHelper.GetVertexCountS(vertexCount1)}w)[{mat1}]";
        if (isShowSize)
        {
            lod1 += $"({renderer_new.size})";
        }
        //string lod1 = $"{renderer_new_name}({MeshHelper.GetVertexCountS(vertexCount1)}w)";

        if (isSameName)
        {
            string result = "";
            if (isSameSize)
            {
                result = "[T1 T2] ";
                //return $"[T1 T2] [{UpdateMode}][{p10:P0}] {disCompare} {lod0} >> New: [{mat1}] ===({MeshHelper.GetVertexCountS(vertexCount0)}w) [==]";
            }
            else
            {
                result = "[T1 F2] ";
                if (isShowSize)
                {

                }
                //return $"[T1 F2] [{UpdateMode}][{p10:P0}] {disCompare} {lod0} >> New: [{mat1}] ===({MeshHelper.GetVertexCountS(vertexCount0)}w)[{renderer_old.size}]";
            }

            result += $"[{UpdateMode}][{p10:P0}] {disCompare} {lod0} >> New: [{mat1}] ===({MeshHelper.GetVertexCountS(vertexCount0)}w)";

            if (isSameSize)
            {
                result += "[==]";
                //return $"[T1 T2] [{UpdateMode}][{p10:P0}] {disCompare} {lod0} >> New: [{mat1}] ===({MeshHelper.GetVertexCountS(vertexCount0)}w) [==]";
            }
            else
            {

                if (isShowSize)
                {
                    result += $"[{renderer_old.size}]";

                }
                else
                {
                    result += $"[S]";
                }
            }
            return result;
        }
        else
        {
            if (isSameSize)
            {
                return $"[F1 T2] [{UpdateMode}][{p10:P0}] {disCompare} {lod0} >> New:{lod1}   [==]";
            }
            else
            {
                return $"[F1 F2] [{UpdateMode}][{p10:P0}] {disCompare} {lod0} >> New:{lod1}";
            }
            //return $"[F] [{p10:F2}] LOD1:{lod1} <{dis:F5}|{meshDis:F5}> LOD0:{lod0}";
        }
    }

    public string GetLODCaption()
    {
        if (renderer_old == null) return "";
        string logName = renderer_old_name;
        if (logName.Length > 20)
        {
            logName = logName.Substring(0, 20) + "...";
        }
        string lod0 = $"\"{logName}\" ({MeshHelper.GetVertexCountS(vertexCount0)}w)({renderer_old.size})";
        if (renderer_new == null)
        {
            return $"{lod0}| ({renderer_old.GetRendererTypesS()})({renderer_old.GetLODIds()})"; ;
        }
        bool isSameName = renderer_new_name == renderer_old_name;
        bool isSameSize = renderer_new.size.ToString() == renderer_old.size.ToString();
        if (isSameSize == false)
        {

        }
        float p10 = (float)vertexCount1 / vertexCount0;
        string lod1 = $"{renderer_new_name}({vertexCount1})[{renderer_new.size}]";

        string dis1 = dis.ToString("F5");
        if (dis > 1)
        {
            dis1 = dis.ToString("F1");
        }
        string meshDis1 = meshDis.ToString("F5");
        if (meshDis > 1)
        {
            meshDis1 = meshDis.ToString("F1");
        }
        string disCompare = $"<{dis1}|{meshDis1}>";
        if (isSameName)
        {
            if (isSameSize)
            {
                return $"[T1 T2] [{p10:P0}] {disCompare} New:{lod1} || Old:===({vertexCount0}) [==]";
            }
            else
            {
                return $"[T1 F2] [{p10:P0}] {disCompare} New:{lod1} || Old:===({vertexCount0})[{renderer_old.size}]";
            }

        }
        else
        {
            if (isSameSize)
            {
                return $"[F1 T2] [{p10:P0}] {disCompare} New:{lod1} || Old:{lod0} [==]";
            }
            else
            {
                return $"[F1 F2] [{p10:P0}] {disCompare} New:{lod1} || Old:{lod0}";
            }
            //return $"[F] [{p10:F2}] LOD1:{lod1} <{dis:F5}|{meshDis:F5}> LOD0:{lod0}";
        }
    }

    public void Replace()
    {
        //var lod1 = renderer_lod1.meshRenderer;
        //var lod0 = renderer_lod0.meshRenderer;
        //lod1.sharedMaterials = lod0.sharedMaterials;
        //lod1.transform.parent = lod0.transform;
        ////copy scripts
        ////lod0.gameObject.SetActive(false);
        //GameObject.DestroyImmediate(lod0.gameObject);

        var render_lod1 = this.renderer_new;
        if (render_lod1 == null) return;
        var render_lod0 = this.renderer_old;
        if (render_lod0 == null) return;
        var lod0 = this.renderer_old.meshRenderer;

        if (renderer_new.IsRendererType(MeshRendererType.LOD))
        {
            LODGroup group = renderer_new.GetComponentInParent<LODGroup>();
            var renderers = group.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var render in renderers)
            {
                render.sharedMaterials = lod0.sharedMaterials;
            }

            Transform lodRoot = LODHelper.GetFloorLODsRoot(render_lod0.transform);
            group.transform.SetParent(lodRoot);

            GameObject.DestroyImmediate(render_lod0.gameObject);
        }
        else
        {
            var lod1 = this.renderer_new.meshRenderer;

            lod1.sharedMaterials = lod0.sharedMaterials;

            render_lod1.transform.SetParent(render_lod0.transform.parent);
            GameObject.DestroyImmediate(render_lod0.gameObject);
        }
    }

    public void SetColor()
    {
        var lod1 = renderer_new.meshRenderer;
        var lod0 = renderer_old.meshRenderer;
        //item.renderer_lod1.sharedMaterial = item.renderer_lod0.sharedMaterial;
        if (lod1.sharedMaterials.Length == lod0.sharedMaterials.Length)
        {
            lod1.sharedMaterials = lod0.sharedMaterials;
        }
        else if (lod1.sharedMaterials.Length == 1 && lod0.sharedMaterials.Length == 2)
        {
            var mats = lod1.sharedMaterials;
            mats[0] = lod0.sharedMaterials[1];
            lod1.sharedMaterials = mats;
        }
        else
        {


            Debug.LogError($"SetAppendLod3Color lod1:{lod1.name} lod0:{lod0.name} length1:{lod1.sharedMaterials.Length} length0:{lod0.sharedMaterials.Length}");
        }
    }

    public void SetColor1()
    {
        var lod1 = renderer_new.meshRenderer;
        var lod0 = renderer_old.meshRenderer;
        //item.renderer_lod1.sharedMaterial = item.renderer_lod0.sharedMaterial;
        //if (lod1.sharedMaterials.Length == lod0.sharedMaterials.Length)
        //{
        lod1.sharedMaterials = lod0.sharedMaterials;

        //}
        //else
        //{
        //    Debug.LogError($"SetAppendLod3Color lod1:{lod1.name} lod0:{lod0.name} length1:{lod1.sharedMaterials.Length} length0:{lod0.sharedMaterials.Length}");
        //}
    }

    public void SetColor2()
    {
        var lod1 = renderer_new.meshRenderer;
        var lod0 = renderer_old.meshRenderer;
        //item.renderer_lod1.sharedMaterial = item.renderer_lod0.sharedMaterial;
        //if (lod1.sharedMaterials.Length == lod0.sharedMaterials.Length)
        //{
        var mats = lod1.sharedMaterials;
        for (int i = 0; i < lod1.sharedMaterials.Length; i++)
        {
            mats[i] = lod0.sharedMaterials[i];
        }
        lod1.sharedMaterials = mats;
        //}
        //else
        //{
        //    Debug.LogError($"SetAppendLod3Color lod1:{lod1.name} lod0:{lod0.name} length1:{lod1.sharedMaterials.Length} length0:{lod0.sharedMaterials.Length}");
        //}
    }

    internal void Hide01()
    {
        Set01Active(false, false);
    }

    private void Set01Active(bool active0, bool active1)
    {
        if (renderer_new == null) return;
        var lod1 = renderer_new.meshRenderer;
        if (lod1 == null) return;
        lod1.gameObject.SetActive(active1);
        var lod0 = renderer_old.meshRenderer;
        lod0.gameObject.SetActive(active0);
    }

    internal void Show0()
    {
        Set01Active(true, false);
    }

    internal void Show01()
    {
        Set01Active(true, true);
    }

    internal void Show1()
    {
        Set01Active(false, true);
    }

    public void Align()
    {
        if (renderer_new == null) return;
        var lod1 = renderer_new.meshRenderer;
        if (lod1 == null) return;
        var lod0 = renderer_old.meshRenderer;

        lod1.transform.position = lod0.transform.position;
    }

    internal void ClearNew()
    {
        var updateInfo1 = this.renderer_old.gameObject.GetComponent<RendererUpdateInfo>();
        if (updateInfo1 != null)
        {
            GameObject.DestroyImmediate(updateInfo1);
        }

        if (this.renderer_new != null)
        {
            var updateInfo2 = this.renderer_new.gameObject.GetComponent<RendererUpdateInfo>();
            if (updateInfo2 == null)
            {
                GameObject.DestroyImmediate(updateInfo2);
            }
        }

        renderer_new = null;
        renderer_new_name = "";
        this.vertexCount1 = 0;
    }

    public UpdateChangedMode UpdateMode;

    internal void SetUpdateState(UpdateChangedMode updateMode)
    {
        this.UpdateMode = updateMode;

        if (this.renderer_old == null) return;

        var updateInfo = this.renderer_old.gameObject.GetComponent<RendererUpdateInfo>();
        if (updateInfo == null)
        {
            updateInfo = this.renderer_old.gameObject.AddComponent<RendererUpdateInfo>();
        }
        updateInfo.changedMode = updateMode;

        var updateInfo2 = this.renderer_new.gameObject.GetComponent<RendererUpdateInfo>();
        if (updateInfo2 == null)
        {
            updateInfo2 = this.renderer_new.gameObject.AddComponent<RendererUpdateInfo>();
        }
        updateInfo2.changedMode = updateMode;
        updateInfo2.IsNew = true;
    }

    internal void DoUpdate()
    {
        if (UpdateMode == UpdateChangedMode.OldDelete)
        {
            if (this.renderer_old != null && this.renderer_old.gameObject != null)
            {
                EditorHelper.UnpackPrefab(this.renderer_old.gameObject);
                GameObject.DestroyImmediate(this.renderer_old.gameObject);
            }
        }
        if (UpdateMode == UpdateChangedMode.NewDelete)
        {
            if (this.renderer_new != null && this.renderer_new.gameObject != null)
            {
                EditorHelper.UnpackPrefab(this.renderer_new.gameObject);
                GameObject.DestroyImmediate(this.renderer_new.gameObject);
            }
        }
        if (UpdateMode == UpdateChangedMode.NewSame)
        {
            if (this.renderer_new != null && this.renderer_new.gameObject != null)
            {
                EditorHelper.UnpackPrefab(this.renderer_new.gameObject);
                GameObject.DestroyImmediate(this.renderer_new.gameObject);
            }
        }
        if (UpdateMode == UpdateChangedMode.NewChanged)
        {
            renderer_new.transform.SetParent(renderer_old.transform.parent);
            if (this.renderer_old != null && this.renderer_old.gameObject != null)
            {
                EditorHelper.UnpackPrefab(this.renderer_old.gameObject);
                GameObject.DestroyImmediate(this.renderer_old.gameObject);
            }

            //var render_lod1 = this.renderer_new;
            //if (render_lod1 == null) return;
            //var render_lod0 = this.renderer_old;
            //if (render_lod0 == null) return;
            //var lod0 = this.renderer_old.meshRenderer;

            //if (renderer_new.IsRendererType(MeshRendererType.LOD))
            //{
            //    LODGroup group = renderer_new.GetComponentInParent<LODGroup>();
            //    var renderers = group.GetComponentsInChildren<MeshRenderer>(true);
            //    foreach (var render in renderers)
            //    {
            //        render.sharedMaterials = lod0.sharedMaterials;
            //    }

            //    Transform lodRoot = LODHelper.GetFloorLODsRoot(render_lod0.transform);
            //    group.transform.SetParent(lodRoot);

            //    GameObject.DestroyImmediate(render_lod0.gameObject);
            //}
            //else
            //{
            //    var lod1 = this.renderer_new.meshRenderer;

            //    lod1.sharedMaterials = lod0.sharedMaterials;

            //    render_lod1.transform.SetParent(render_lod0.transform.parent);
            //    GameObject.DestroyImmediate(render_lod0.gameObject);
            //}
        }
    }

    internal void Rename()
    {
        this.renderer_old.name = this.renderer_new.name;
        this.renderer_old_name = this.renderer_new_name;
    }

    public void ReplaceMaterialNew()
    {
        var renderNew = this.renderer_new.meshRenderer;
        var renderOld = this.renderer_old.meshRenderer;
        if (renderOld == null) return;
        renderNew.sharedMaterials = renderOld.sharedMaterials;
    }

    public void ReplaceMaterialOld()
    {
        var renderNew = this.renderer_new.meshRenderer;
        var renderOld = this.renderer_old.meshRenderer;
        renderOld.sharedMaterials = renderNew.sharedMaterials;
    }
}
