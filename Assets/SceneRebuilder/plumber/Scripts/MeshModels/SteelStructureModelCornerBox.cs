using CommonUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelStructureModelCornerBox : SteelStructureBaseModel
{
    public CornerBoxMeshData modelData;

    public override void ClearData()
    {
        modelData = new CornerBoxMeshData();
    }

    [ContextMenu("GetModelInfo_CornerBox")]
    public void GetModelInfo_CornerBox()
    {
        ClearDebugInfoGos();
        verticesToPlaneInfos = GetVerticesToPlaneInfos(0.00025f,50,100);
        //Debug.Log($"GetLModelInfo verticesToPlaneInfos:{verticesToPlaneInfos.Count}");
        if (verticesToPlaneInfos.Count < 1)
        {
            IsGetInfoSuccess = false;
            if (isShowDebug)
            {
                Debug.LogWarning($"GetModelInfo_CornerBox verticesToPlaneInfos.Count < 1 count:{verticesToPlaneInfos.Count},gameObject:{this.name}");
            }
            return;
        }
        modelData = GetModelData_CornerBox(verticesToPlaneInfos);
        if (modelData.length == 0)
        {
            IsGetInfoSuccess = false;
            if (isShowDebug)
            {
                Debug.LogWarning($"GetModelInfo_CornerBox length == 0 modelData:{modelData},gameObject:{this.name}");
            }
            
            return;
        } 
        IsGetInfoSuccess = true;

        //IsGetInfoSuccess = !modelData.IsError(this.name);
        //if (isShowDebug)
        //{
        //    Debug.Log($"GetModelInfo_CornerBox modelData:{modelData}");
        //}
    }

    public override void GetModelInfo()
    {
        GetModelInfo_CornerBox();
    }

    public override bool RendererModel()
    {
        return RendererModel_CornerBox();
    }

    public bool RendererModel_CornerBox()
    {
        CornerBoxMeshData lData = this.modelData;

        if (IsGetInfoSuccess == false)
        {
            Debug.LogError($"GenerateLModel_{this.name} IsGetInfoSuccess lData:{lData}");
            return false;
        }

        List<SteelModelGenerateArg> gos = new List<SteelModelGenerateArg>();

        float offXY = lData.height - lData.width;

        //if (offXY > maxOffXY)
        //{
        //    //Debug.LogWarning($"GenerateLModel_{this.name} offXY> {maxOffXY} :{offXY},lData:{lData}"); 
        //}

        gos.AddRange(GenerateLModel(0));
        //gos.AddRange(GenerateLModel(90));
        //gos.AddRange(GenerateLModel(180));
        //gos.AddRange(GenerateLModel(270));
        //gos.Sort();

        for (int i = 1; i < gos.Count; i++)
        {
            if (gos[i].go == null) continue;
            if (isShowDebug == false)
            {
                GameObject.DestroyImmediate(gos[i].go);
            }
        }

        GameObject goNew = gos[0].go;

        MeshAlignHelper.AcRTAlign(goNew, this.gameObject,false);
        meshDis = VertexHelper.GetVertexDistanceEx(goNew, this.gameObject);
        //Debug.Log($"RendererModel_CornerBox meshDis:{meshDis}");
        goNew.name += "_" + meshDis;

        string txt = "";
        for (int i = 0; i < gos.Count; i++)
        {
            txt += $"arg[{i}]:{gos[i]}";
        }
        
        //float dis = gos[0].meshDis;
        if (meshDis > 0.2f)
        {
            if(isShowDebug)
            {
                Debug.LogError($"RendererModel_CornerBox({this.name}) dis:{meshDis}");
            }
            
            if (isShowDebug == false)
            {
                GameObject.DestroyImmediate(goNew);
            }
            ResultGo = null;
            IsGetInfoSuccess = false;
            return false;
        }
        else
        {
            //Debug.Log($"GetLModelInfo({this.name}) dis:{dis} txt:{txt} ");
        }

        if (ResultGo != null)
        {
            GameObject.DestroyImmediate(ResultGo);
        }

        ResultGo = goNew;
        return true;
    }

    public List<SteelModelGenerateArg> GenerateModel_CornerBox(CornerBoxMeshData lData)
    {
        List<SteelModelGenerateArg> list = new List<SteelModelGenerateArg>();
        //float offXY = lData.height - lData.width;
        //if (offXY < maxOffXY)
        //{
        //    list.Add(GenerateModelInner_CornerBox(lData));
        //}
        //else
        //{
        //    //Debug.LogWarning($"GenerateLModel_{this.name} offXY> 0.00001 :{offXY},lData:{lData}");
        //    lData.isMirror = false;
        //    list.Add(GenerateModelInner_CornerBox(lData));
        //    lData.isMirror = true;
        //    list.Add(GenerateModelInner_CornerBox(lData));
        //}
        list.Add(GenerateModelInner_CornerBox(lData));

        return list;
    }

    public List<SteelModelGenerateArg> GenerateLModel(float angle)
    {
        List<SteelModelGenerateArg> list = new List<SteelModelGenerateArg>();
        CornerBoxMeshData lData = this.modelData;
        lData.angle = angle;
        list.Add(GenerateModelInner_CornerBox(lData));
        return list;
    }

    public SteelModelGenerateArg GenerateModelInner_CornerBox(CornerBoxMeshData lData)
    {
        //string id = lData.GetId();
        GameObject goNew = new GameObject(gameObject.name);
        //goNew.transform.position = this.transform.position;
        goNew.transform.position = MeshRendererInfo.GetCenterPos(gameObject);
        CornerBoxGenerator hMesh = goNew.AddComponent<CornerBoxGenerator>();
        hMesh.SetData(lData);
        hMesh.CreateLine();

        float dis = VertexHelper.GetVertexDistanceEx(goNew, this.gameObject);
        //goNew.name += "_New_" + lData.angle + "_" + dis;
        goNew.name += "_New";
        goNew.transform.parent = this.transform.parent;
        return (new SteelModelGenerateArg(goNew, dis, lData.angle));
    }

    [ContextMenu("ShowDirection")]
    public void ShowDirection()
    {
        //transform.forward = modelData.forward;
        Vector3 forwardW = transform.TransformDirection(transform.forward);
        Vector3 upW = transform.TransformDirection(transform.up);
        float angleOff = Vector3.Angle(modelData.up, upW);
        Debug.Log($"ShowDirection forward:{modelData.forward.Vector3ToString5()} transform.forward:{transform.forward.Vector3ToString5()} forwardW:{forwardW.Vector3ToString5()}");
        Debug.Log($"ShowDirection up:{modelData.up.Vector3ToString5()} transform.up:{transform.up.Vector3ToString5()} upW:{upW.Vector3ToString5()} angleOff:{angleOff}");
    }
}
