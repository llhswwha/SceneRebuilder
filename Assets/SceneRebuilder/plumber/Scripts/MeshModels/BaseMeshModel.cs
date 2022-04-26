using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMeshModel : MonoBehaviour
{
    public virtual void RendererModel()
    {
    }

    public float minRepeatPointDistance = 0.00005f;

    public void DebugShowSharedPoints()
    {
        //ClearChildren();
        ClearDebugInfoGos();
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        MeshTriangles meshTriangles = new MeshTriangles(mesh);

        Debug.Log($"ShowSharedPoints mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        //meshTriangles.ShowSharedPointsById(this.transform, PointScale, 10);

        //meshTriangles.ShowSharedPointsByIdEx(this.transform, PointScale, 15,int.MaxValue, minRepeatPointDistance);
        //meshTriangles.ShowSharedPointsByIdEx(this.transform, PointScale, 0, int.MaxValue, minRepeatPointDistance);
        //meshTriangles.ShowSharedPointsByIdEx(this.transform, PointScale, 0,3, minRepeatPointDistance);

        meshTriangles.ShowCirclesById(this.transform, PointScale, 0, 3, minRepeatPointDistance);
        //meshTriangles.ShowSharedPointsByPoint(this.transform, PointScale, 10);
        //meshTriangles.ShowSharedPointsByPointExEx(this.transform, PointScale, sharedMinCount, minRepeatPointDistance);
        meshTriangles.Dispose();
    }

    public void DebugShowNormalPoints()
    {
        //ClearChildren();
        ClearDebugInfoGos();
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        MeshTriangles meshTriangles = new MeshTriangles(mesh);

        Debug.Log($"DebugShowNormalPoints mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        //meshTriangles.ShowSharedPointsById(this.transform, PointScale, 10);

        //meshTriangles.ShowSharedPointsByIdEx(this.transform, PointScale, 15,int.MaxValue, minRepeatPointDistance);
        //meshTriangles.ShowSharedPointsByIdEx(this.transform, PointScale, 0, int.MaxValue, minRepeatPointDistance);
        //meshTriangles.ShowSharedPointsByIdEx(this.transform, PointScale, 0,3, minRepeatPointDistance);

        meshTriangles.ShowNormalPlanes(this.transform, PointScale, 2, 0, minRepeatPointDistance);
        //meshTriangles.ShowSharedPointsByPoint(this.transform, PointScale, 10);
        //meshTriangles.ShowSharedPointsByPointExEx(this.transform, PointScale, sharedMinCount, minRepeatPointDistance);
        meshTriangles.Dispose();
    }


    public float PointScale = 0.001f;

    public void DebugShowTriangles()
    {
        ClearDebugInfoGos();
        MeshTriangles.DebugShowTriangles(this.gameObject, PointScale);
    }

    public GameObject ResultGo = null;

    [ContextMenu("ReplaceOld")]
    public void ReplaceOld()
    {
        BaseMeshModel model = this;
        if (model == null) return;
        GameObject newGo = model.ResultGo;
        if (newGo == null)
        {
            model.ClearDebugInfoGos();
            model.gameObject.SetActive(true);
            Debug.LogWarning($"ReplaceOld newGo == null go:{model.name} IsGetInfoSuccess:{IsGetInfoSuccess}");
            return;
        }
        if (newGo == model.gameObject)
        {
            return;
        }

        if (IsGetInfoSuccess == false)
        {
            Debug.LogWarning($"ReplaceOld IsGetInfoSuccess == false go:{model.name}");
            GameObject.DestroyImmediate(newGo);
            model.ClearDebugInfoGos();
            model.gameObject.SetActive(true);
            return;
        }

        //newGo.transform.SetParent(model.transform.parent);
        //newGo.name = model.name;
        //model.gameObject.SetActive(false);
        //RemoveAllComponents();

        MeshHelper.CopyTransformMesh(newGo, model.gameObject);
        newGo.SetActive(false);
        model.ResultGo = model.gameObject;

        GameObject.DestroyImmediate(newGo);
        model.ClearDebugInfoGos();
        model.gameObject.SetActive(true);
    }

    public void RemoveAllComponents()
    {
        try
        {
            if (IsGetInfoSuccess == false) return;
            ClearDebugInfoGos();
            EditorHelper.RemoveAllComponents(this.gameObject, typeof(BaseMeshModel), typeof(RendererId));


            if (gameObject != null)
            {
                gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError($"RemoveAllComponents gameObject == null");
            }
        }
        catch (System.Exception ex)
        {

            Debug.LogError($"RemoveAllComponents Exception ex:{ex} go:{this}");
        }

    }

    [ContextMenu("ClearGo")]
    public void ClearGo()
    {
        if (ResultGo != null)
        {
            if (ResultGo != this.gameObject)
            {
                GameObject.DestroyImmediate(ResultGo);
            }
            else
            {

                //DestroyMeshComponent();
                PipeMeshGeneratorBase generators = ResultGo.GetComponent<PipeMeshGeneratorBase>();
                if (generators != null)
                {
                    generators.ClearResult();
                    GameObject.DestroyImmediate(generators);
                }
                //ResultGo = null;
            }
            ResultGo = null;
        }
    }

    public bool IsGetInfoSuccess = true;

    public int VertexCount = 0;



    public void ShowOBB()
    {
        OBBCollider.ShowOBB(this.gameObject, true);
    }

    public virtual void GetModelInfo()
    {

    }

    public virtual void GetModelInfo_Job()
    {

    }

    public void ClearDebugInfoGos()
    {
        DebugInfoRoot[] debugRoots = this.GetComponentsInChildren<DebugInfoRoot>(true);
        foreach (var item in debugRoots)
        {
            if (item == null) continue;
            GameObject.DestroyImmediate(item.gameObject);
        }
    }

    [ContextMenu("ClearChildren")]
    public void ClearChildren()
    {
        TransformHelper.ClearChildren(gameObject);
        //ClearCheckDistance();
    }

    public GameObject CreateDebugInfoRoot(string goName)
    {
        GameObject go0 = new GameObject(goName);
        go0.AddComponent<DebugInfoRoot>();
        go0.transform.SetParent(this.transform);
        go0.transform.localPosition = Vector3.zero;
        //DebugInfoRootGos.Add(go0);
        return go0;
    }

    public PipeModelCheckResult mcResult;

    public virtual void CheckResult()
    {
        mcResult.ObbDistance = OBBCollider.GetObbDistance(ResultGo, this.gameObject);

        mcResult.MeshDistance = MeshHelper.GetVertexDistanceEx(ResultGo, this.gameObject);
        mcResult.SizeDistance = MeshHelper.GetSizeDistance(ResultGo, this.gameObject);
        mcResult.RTDistance = OBBCollider.GetObbRTDistance(ResultGo, this.gameObject);

        if (mcResult.SizeDistance > 0.1f)
        {
            IsGetInfoSuccess = false;
        }

        TransformHelper.ClearChildren(ResultGo);

        Debug.Log($"CheckResult mcResult:{mcResult}");
    }

    public void SetCheckResult(PipeModelCheckResult r)
    {
        mcResult = r;
        if (mcResult.SizeDistance > 0.1f)
        {
            IsGetInfoSuccess = false;
        }
    }

    public void ClearCheckDistance()
    {
        mcResult = new PipeModelCheckResult();
    }
}

public struct PipeModelCheckResult
{
    public float ObbDistance;

    public float MeshDistance;

    public float SizeDistance;

    public float RTDistance;

    public override string ToString()
    {
        float dis = SizeDistance + ObbDistance + MeshDistance;

        //return $"{ObbDistance:F5}_{MeshDistance:F5}_{a}_{radius:00000}_{this.GetType().Name}_{this.VertexCount}";
        return $"(s:{SizeDistance:F5}_[d:{dis:F5}]_o:{ObbDistance:F5}_r:{RTDistance:F5}_m:{MeshDistance:F5})";
    }
}
