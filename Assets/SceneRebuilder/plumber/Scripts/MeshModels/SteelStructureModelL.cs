using CommonUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelStructureModelL : SteelStructureBaseModel
{
    public SteelModelLData modelData;

    [ContextMenu("ClearData")]
    public override void ClearData()
    {
        modelData = new SteelModelLData();
    }

    [ContextMenu("GetModelInfo_LMesh")]
    public void GetModelInfo_LMesh()
    {
        ClearDebugInfoGos();
        verticesToPlaneInfos = GetVerticesToPlaneInfos(0.00025f,20,100);
        //Debug.Log($"GetLModelInfo verticesToPlaneInfos:{verticesToPlaneInfos.Count}");
        if (verticesToPlaneInfos.Count < 1)
        {
            IsGetInfoSuccess = false;
            Debug.LogError($"GetLModelInfo verticesToPlaneInfos.Count < 1 count:{verticesToPlaneInfos.Count},gameObject:{this.name}");
            return;
        }
        modelData = GetModelData_LMesh(verticesToPlaneInfos);
        IsGetInfoSuccess = !modelData.IsError(this.name);
        if (isShowDebug)
        {
            Debug.Log($"GetLModelInfo modelData:{modelData}");
        }
    }

    public List<SteelModelGenerateArg> GenerateLModel(float angle)
    {
        SteelModelLData lData = this.modelData;
        lData.angle = angle;
        return GenerateModel_LMesh(lData);
    }

    public List<SteelModelGenerateArg> GenerateModel_LMesh(SteelModelLData lData)
    {
        List<SteelModelGenerateArg> list = new List<SteelModelGenerateArg>();
        float offXY = lData.height - lData.width;
        if (offXY < maxOffXY)
        {
            list.Add(GenerateModelInner_LMesh(lData));
        }
        else
        {
            //Debug.LogWarning($"GenerateLModel_{this.name} offXY> 0.00001 :{offXY},lData:{lData}");
            lData.isMirror = false;
            list.Add(GenerateModelInner_LMesh(lData));
            lData.isMirror = true;
            list.Add(GenerateModelInner_LMesh(lData));
        }

        return list;
    }


    protected SteelModelGenerateArg GenerateModelInner_LMesh(SteelModelLData lData)
    {
        string id = lData.GetId();
        GameObject goNew = new GameObject(gameObject.name);
        goNew.transform.position = this.transform.position+oBBCollider.OBB.Center;
        LMeshGenerator hMesh = goNew.AddComponent<LMeshGenerator>();
        hMesh.SetData(lData);
        hMesh.CreateLine();

        float dis = VertexHelper.GetVertexDistanceEx(goNew, this.gameObject); 
        goNew.name += "_New_" + lData.angle + "_" + dis;
        goNew.transform.parent = this.transform.parent;
        return (new SteelModelGenerateArg(goNew, dis, lData.angle));
    }

    public override void GetModelInfo()
    {
        GetModelInfo_LMesh();
    }

    [ContextMenu("DebugGetModelInfo(Debug_True)")]
    public void DebugGetModelInfoDebug_True()
    {
        isShowDebug = true;
        GetModelInfo_LMesh();
    }

    [ContextMenu("DebugGetModelInfo(Debug_False)")]
    public void DebugGetModelInfoDebug_False()
    {
        isShowDebug = false;
        GetModelInfo_LMesh();
    }

    private float maxOffXY = 0.0002f;

    public bool RendererModel_LMesh()
    {
        SteelModelLData lData = this.modelData;

        if (IsGetInfoSuccess == false)
        {
            Debug.LogError($"GenerateLModel_{this.name} IsGetInfoSuccess lData:{lData}");
            return false;
        }

        List<SteelModelGenerateArg> gos = new List<SteelModelGenerateArg>();

        float offXY = lData.height - lData.width;
        if (offXY > maxOffXY)
        {
            //Debug.LogWarning($"GenerateLModel_{this.name} offXY> {maxOffXY} :{offXY},lData:{lData}"); 
        }
        gos.AddRange(GenerateLModel(0));
        gos.AddRange(GenerateLModel(90));
        gos.AddRange(GenerateLModel(180));
        gos.AddRange(GenerateLModel(270));
        gos.Sort();

        for (int i = 1; i < gos.Count; i++)
        {
            if (gos[i].go == null) continue;
            GameObject.DestroyImmediate(gos[i].go);
        }

        GameObject goNew = gos[0].go;

        //MeshAlignHelper.AcRTAlign(goNew, this.gameObject, false);
        //var meshDis = VertexHelper.GetVertexDistanceEx(goNew, this.gameObject);
        //goNew.name += "_" + meshDis;

        string txt = "";
        for (int i = 0; i < gos.Count; i++)
        {
            txt += $"arg[{i}]:{gos[i]}";
        }
        float dis = gos[0].meshDis;
        float angle = gos[0].angle;
        meshDis = dis;

        if (dis > 0.2f)
        {
            GameObject.DestroyImmediate(gos[0].go);
            List<SteelModelGenerateArg> gos2 = new List<SteelModelGenerateArg>();
            for (int i = 0; i < 10; i++)
            {
                gos2.AddRange(GenerateLModel(angle + (i - 5) * 1));
            }
            gos2.Sort();
            float angle2 = gos2[0].angle;
            for (int i = 1; i < gos2.Count; i++)
            {
                if (gos2[i].go == null) continue;
                GameObject.DestroyImmediate(gos2[i].go);
            }

            GameObject.DestroyImmediate(gos2[0].go);
            List<SteelModelGenerateArg> gos3 = new List<SteelModelGenerateArg>();
            for (int i = 0; i < 20; i++)
            {
                gos3.AddRange(GenerateLModel(angle2 + (i - 5) * 0.1f));
            }
            gos3.Sort();
            float angle3 = gos3[0].angle;
            for (int i = 1; i < gos3.Count; i++)
            {
                if (gos3[i].go == null) continue;
                GameObject.DestroyImmediate(gos3[i].go);
            }
            dis = gos3[0].meshDis;
            angle = gos3[0].angle;
            goNew = gos3[0].go;
            meshDis = dis;  
        }

        if (dis > 0.2f)
        {
            if (isShowDebug)
            {
                Debug.LogError($"GetLModelInfo({this.name}) dis:{dis} angle:{angle} txt:{txt}"); 
            }
            if (isShowDebug == false)
            {
                GameObject.DestroyImmediate(goNew); 
                ResultGo = null;
                IsGetInfoSuccess = false;
            }
            else
            {
                ResultGo = goNew;
            }
            //IsGetInfoSuccess = false;
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

    public override bool RendererModel()
    {
        return RendererModel_LMesh();
    }
}
