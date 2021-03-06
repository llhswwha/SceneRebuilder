using System.Collections;
using System.Collections.Generic;
using UltimateGameTools.MeshSimplifier;
using UnityEngine;

public static class AutomaticLODHelper 
{
    const float COVERAGEPOWER = 6.0f;
    public static float SliderToScreenCoverage(float fSlider)
    {
        return Mathf.Pow(1.0f - fSlider, COVERAGEPOWER);
    }

    public static float ScreenCoverageToSlider(float fCoverage)
    {
        return 1.0f - Mathf.Pow(fCoverage, 1.0f / COVERAGEPOWER);
    }

    public static void CreateDefaultLODS(int nLevels, AutomaticLOD root, bool bRecurseIntoChildren, float[] lodVertexPercents)
    {
        List<AutomaticLOD.LODLevelData> listLODLevels = new List<AutomaticLOD.LODLevelData>();

        for (int i = 0; i < nLevels; i++)
        {
            AutomaticLOD.LODLevelData data = new AutomaticLOD.LODLevelData();

            float oneminust = (float)(nLevels - i) / (float)nLevels;
            if(lodVertexPercents!=null){
                oneminust=lodVertexPercents[i];
            }

            // Debug.LogError($"CreateDefaultLODS [{i}] oneminust:{oneminust}");

            data.m_fScreenCoverage = AutomaticLODHelper.SliderToScreenCoverage(1.0f - oneminust);
            data.m_fMaxCameraDistance = i == 0 ? 0.0f : i * 100.0f;
            data.m_fMeshVerticesAmount = oneminust;
            data.m_mesh = null;
            data.m_bUsesOriginalMesh = false;
            data.m_nColorEditorBarIndex = i;

            listLODLevels.Add(data);
        }

        root.SetLODLevels(listLODLevels, AutomaticLOD.EvalMode.ScreenCoverage, 1000.0f, bRecurseIntoChildren);
    }

    private static bool SaveAsset(Object assetObj,string strFile,bool bAssetAlreadyCreated)
    {
#if UNITY_EDITOR
        if (bAssetAlreadyCreated == false && UnityEditor.AssetDatabase.Contains(assetObj) == false)
        {
            Debug.LogError($"CreateAsset:{strFile}");
            UnityEditor.AssetDatabase.CreateAsset(assetObj, strFile);
            bAssetAlreadyCreated = true;
        }
        else
        {
            if (UnityEditor.AssetDatabase.Contains(assetObj) == false)
            {
                Debug.LogError($"AddObjectToAsset:{strFile}");
                UnityEditor.AssetDatabase.AddObjectToAsset(assetObj, strFile);
                UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(assetObj));
            }
        }
#endif
        return bAssetAlreadyCreated;
    }

    public static bool SaveMeshAssetsRecursive(GameObject root, GameObject gameObject, string strFile, bool bRecurseIntoChildren, bool bAssetAlreadyCreated, ref int nProgressElementsCounter)
    {
#if UNITY_EDITOR
        // Debug.LogError($"SaveMeshAssetsRecursive root:{root},gameObject:{gameObject},strFile:{strFile},{bRecurseIntoChildren},{bAssetAlreadyCreated},{nProgressElementsCounter}");
        if (gameObject == null || Simplifier.Cancelled)
        {
            return bAssetAlreadyCreated;
        }

        AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

        if (automaticLOD != null && automaticLOD.HasLODData() && (automaticLOD.m_LODObjectRoot == null || automaticLOD.m_LODObjectRoot.gameObject == root))
        {
            int nTotalProgressElements = automaticLOD.m_LODObjectRoot != null ? (automaticLOD.m_LODObjectRoot.m_listDependentChildren.Count + 1) : 1;
            nTotalProgressElements *= automaticLOD.m_listLODLevels != null ? automaticLOD.m_listLODLevels.Count : 0;

            Debug.LogError($"automaticLOD.m_listLODLevels.Count:{automaticLOD.m_listLODLevels.Count}");
            for (int nLOD = 0; nLOD < automaticLOD.m_listLODLevels.Count; nLOD++)
            {
                var LODLevel = automaticLOD.m_listLODLevels[nLOD];
                var mesh = LODLevel.m_mesh;
                if (mesh != null && AutomaticLOD.HasValidMeshData(automaticLOD.gameObject))
                {
                    float fT = (float)nProgressElementsCounter / (float)nTotalProgressElements;
                    Progress("Saving meshes to asset file", automaticLOD.name + " LOD " + nLOD, fT);

                    if (Simplifier.Cancelled)
                    {
                        return bAssetAlreadyCreated;
                    }

                    //if (bAssetAlreadyCreated == false && UnityEditor.AssetDatabase.Contains(mesh) == false)
                    //{
                    //    Debug.LogError($"CreateAsset:{strFile}");
                    //    UnityEditor.AssetDatabase.CreateAsset(mesh, strFile);
                    //    bAssetAlreadyCreated = true;
                    //}
                    //else
                    //{
                    //    if (UnityEditor.AssetDatabase.Contains(mesh) == false)
                    //    {
                    //        Debug.LogError($"AddObjectToAsset:{strFile}");
                    //        UnityEditor.AssetDatabase.AddObjectToAsset(mesh, strFile);
                    //        UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(mesh));
                    //    }
                    //}

                    bAssetAlreadyCreated = SaveAsset(mesh, strFile, bAssetAlreadyCreated);

                    nProgressElementsCounter++;
                }
            }
        }

        if (bRecurseIntoChildren)
        {
            for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
            {
                bAssetAlreadyCreated = SaveMeshAssetsRecursive(root, gameObject.transform.GetChild(nChild).gameObject, strFile, bRecurseIntoChildren, bAssetAlreadyCreated, ref nProgressElementsCounter);
            }
        }
#endif
        return bAssetAlreadyCreated;
    }

    public static void Progress(string strTitle, string strMessage, float fT)
    {
        int nPercent = Mathf.RoundToInt(fT * 100.0f);

        if (nPercent != s_nLastProgress || s_strLastTitle != strTitle || s_strLastMessage != strMessage)
        {
            s_strLastTitle = strTitle;
            s_strLastMessage = strMessage;
            s_nLastProgress = nPercent;

            //if (ProgressBarHelper.DisplayCancelableProgressBar(strTitle, strMessage, fT))
            // {
            //     Simplifier.Cancelled = true;
            // }
        }
    }

    static int s_nLastProgress = -1;
    static string s_strLastTitle = "";
    static string s_strLastMessage = "";

    private static void SetDefaultLOD(int nLevels)
    {
        if(nLevels==1){
            AutomaticLOD.DefaultLevelCount=AutomaticLOD.LevelsToGenerate._1;
        }
        else if(nLevels==2){
            AutomaticLOD.DefaultLevelCount=AutomaticLOD.LevelsToGenerate._2;
        }
        else if(nLevels==3){
            AutomaticLOD.DefaultLevelCount=AutomaticLOD.LevelsToGenerate._3;
        }
        else if(nLevels==4){
            AutomaticLOD.DefaultLevelCount=AutomaticLOD.LevelsToGenerate._4;
        }
        else if(nLevels==5){
            AutomaticLOD.DefaultLevelCount=AutomaticLOD.LevelsToGenerate._5;
        }
        else if(nLevels==6){
            AutomaticLOD.DefaultLevelCount=AutomaticLOD.LevelsToGenerate._6;
        }
        else{
            AutomaticLOD.DefaultLevelCount=AutomaticLOD.LevelsToGenerate._3;
        }
    }

    public static void ClearLODAndChildren(GameObject go)
    {
        AutomaticLOD aLOD = go.GetComponent<AutomaticLOD>();
        if(aLOD){
            DeleteLODData(aLOD);
        }
        else{
            LODGroup lODGroup=go.GetComponent<LODGroup>();
            if(lODGroup!=null){
                GameObject.DestroyImmediate(lODGroup);
            }

            List<Transform> children=new List<Transform>();
            for(int i=0;i<go.transform.childCount;i++)
            {
                var child=go.transform.GetChild(i);
                children.Add(child);
            }
            foreach(var child in children){
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
    }

    private static void SetMaterials(GameObject go, Material[] mats)
    {
        if(mats!=null&& mats.Length > go.transform.childCount)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i);
                MeshRenderer mr = child.GetComponent<MeshRenderer>();

                mr.material = mats[i];
            }
            MeshRenderer mr0 = go.GetComponent<MeshRenderer>();
            if(mr0!=null)
                mr0.material = mats[0];
        }
    }

    public static AutomaticLOD CreateNewAutomaticLOD(GameObject go)
    {
        AutomaticLOD aLOD = go.GetComponent<AutomaticLOD>();
        if (aLOD)
        {
            GameObject.DestroyImmediate(aLOD);
        }
        if (aLOD == null)
        {
            aLOD = go.AddComponent<AutomaticLOD>();
        }
        return aLOD;
    }

    public static void CreateAutoLOD(GameObject go, Material[] mats,float[] lvs,float[] lodVertexPercents,bool isDestroyScript=true,bool isSaveAsset=false, System.Action<float> progressChanged=null)
    {
        System.DateTime start= System.DateTime.Now;
        if(lvs==null)
        {
            SetMaterials(go,mats);
            return;
        }
        if (progressChanged != null) progressChanged(0/2f);
        int nLevels=0;
        if(lvs!=null)
        {
            nLevels=lvs.Length;
            AutomaticLOD.defaultLODValues=lvs;
            SetDefaultLOD(nLevels);
        }
        //??????????????????

        AutomaticLOD aLOD = CreateNewAutomaticLOD(go);

        if(go.GetComponent<MeshRenderer>()!=null)
            ClearLODAndChildren(go);

        bool bRecurseIntoChildren = true;
        AutomaticLODHelper.CreateDefaultLODS(nLevels, aLOD, bRecurseIntoChildren,lodVertexPercents);
        aLOD.ComputeLODData(bRecurseIntoChildren, (t,m,p)=>
        {
            //Debug.LogError($"CreateLOD[{go.name}][{go.GetInstanceID()}] ComputeLODData t:{t},m:{m},p:{p}");
            if (progressChanged != null) progressChanged(p*0.4f);
        });

        //Debug.LogError($"CreateLOD time1:{(System.DateTime.Now - start).ToString()}");

        aLOD.ComputeAllLODMeshes(bRecurseIntoChildren, (t, m, p) =>
        {
            //Debug.LogError($"CreateLOD[{go.name}][{go.GetInstanceID()}] ComputeAllLODMeshes t:{t},m:{m},p:{p}");
            if (progressChanged != null) progressChanged(0.4f+p*0.6f);
        });
        //Debug.LogError($"CreateLOD time2:{(System.DateTime.Now - start).ToString()}");


        if (isSaveAsset)
        {
            string meshPath = "Assets/Models/Instances/Prefabs/" + go.name + go.GetInstanceID() + ".asset";
            int nCounter = 0;

            bool bAssetAlreadyCreated = System.IO.File.Exists(meshPath);
            //Debug.LogError($"bAssetAlreadyCreated:{bAssetAlreadyCreated}");
            if (bAssetAlreadyCreated == true)
            {
                System.IO.File.Delete(meshPath);
                bAssetAlreadyCreated = System.IO.File.Exists(meshPath);
            }
            AutomaticLODHelper.SaveMeshAssetsRecursive(go, go, meshPath, true, bAssetAlreadyCreated, ref nCounter);
        }

        SetMaterials(go,mats);

        if(isDestroyScript){
            GameObject.DestroyImmediate(aLOD);

            Simplifier simplifier=go.GetComponent<Simplifier>();
            if(simplifier){
                GameObject.DestroyImmediate(simplifier);
            }
        }

        if (progressChanged != null) progressChanged(1);

        if(aLOD.m_originalMesh==null){
            Debug.LogError($"CreateLOD vertexCount:[ aLOD.m_originalMesh==null ],time:{(System.DateTime.Now - start).ToString()}");
        }
        else{
            Debug.LogError($"CreateLOD vertexCount:{aLOD.m_originalMesh.vertexCount},time:{(System.DateTime.Now - start).ToString()}");
        }
    }

    public static void DeleteLODData(AutomaticLOD automaticLOD)
    {
        if (automaticLOD.m_LODObjectRoot == null)
        {
            DeleteLODDataRecursive(automaticLOD.gameObject, automaticLOD.gameObject, true);
        }
    }

    public static void DeleteLODDataRecursive(GameObject root, GameObject gameObject, bool bRecurseIntoChildren)
    {
        AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

        if (automaticLOD)
        {
            if (automaticLOD.m_LODObjectRoot == null || automaticLOD.m_LODObjectRoot.gameObject == root)
            {
                automaticLOD.RestoreOriginalMesh(true, false);

                if (automaticLOD.m_LODObjectRoot != null)
                {
                    if (Application.isEditor && Application.isPlaying == false)
                    {
                        UnityEngine.Object.DestroyImmediate(automaticLOD);
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(automaticLOD);
                    }
                }
            }

            automaticLOD.m_aRelevanceSpheres = null;
            automaticLOD.m_bEnablePrefabUsage = false;
            automaticLOD.m_strAssetPath = "";
        }

        if (bRecurseIntoChildren)
        {
            for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
            {
                DeleteLODDataRecursive(root, gameObject.transform.GetChild(nChild).gameObject, true);
            }
        }
    }

}
