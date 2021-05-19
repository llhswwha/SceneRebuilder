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

    public static void CreateDefaultLODS(int nLevels, AutomaticLOD root, bool bRecurseIntoChildren)
    {
        List<AutomaticLOD.LODLevelData> listLODLevels = new List<AutomaticLOD.LODLevelData>();

        for (int i = 0; i < nLevels; i++)
        {
            AutomaticLOD.LODLevelData data = new AutomaticLOD.LODLevelData();

            float oneminust = (float)(nLevels - i) / (float)nLevels;

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

    public static bool SaveMeshAssetsRecursive(GameObject root, GameObject gameObject, string strFile, bool bRecurseIntoChildren, bool bAssetAlreadyCreated, ref int nProgressElementsCounter)
    {
        Debug.LogError($"SaveMeshAssetsRecursive root:{root},gameObject:{gameObject},strFile:{strFile},{bRecurseIntoChildren},{bAssetAlreadyCreated},{nProgressElementsCounter}");
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
                if (automaticLOD.m_listLODLevels[nLOD].m_mesh != null && AutomaticLOD.HasValidMeshData(automaticLOD.gameObject))
                {
                    float fT = (float)nProgressElementsCounter / (float)nTotalProgressElements;
                    Progress("Saving meshes to asset file", automaticLOD.name + " LOD " + nLOD, fT);

                    if (Simplifier.Cancelled)
                    {
                        return bAssetAlreadyCreated;
                    }

                    if (bAssetAlreadyCreated == false && UnityEditor.AssetDatabase.Contains(automaticLOD.m_listLODLevels[nLOD].m_mesh) == false)
                    {
                        Debug.LogError($"CreateAsset:{strFile}");
                        UnityEditor.AssetDatabase.CreateAsset(automaticLOD.m_listLODLevels[nLOD].m_mesh, strFile);
                        bAssetAlreadyCreated = true;
                    }
                    else
                    {
                        if (UnityEditor.AssetDatabase.Contains(automaticLOD.m_listLODLevels[nLOD].m_mesh) == false)
                        {
                            Debug.LogError($"AddObjectToAsset:{strFile}");
                            UnityEditor.AssetDatabase.AddObjectToAsset(automaticLOD.m_listLODLevels[nLOD].m_mesh, strFile);
                            UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(automaticLOD.m_listLODLevels[nLOD].m_mesh));
                        }
                    }

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

            if (ProgressBarHelper.DisplayCancelableProgressBar(strTitle, strMessage, fT))
            {
                Simplifier.Cancelled = true;
            }
        }
    }

    static int s_nLastProgress = -1;
    static string s_strLastTitle = "";
    static string s_strLastMessage = "";

    public static void CreateLOD(GameObject go, Material[] mats)
    {
        AutomaticLOD aLOD = go.GetComponent<AutomaticLOD>();
        if (aLOD == null)
        {
            aLOD = go.AddComponent<AutomaticLOD>();
        }
        bool bRecurseIntoChildren = true;
        AutomaticLODHelper.CreateDefaultLODS(4, aLOD, bRecurseIntoChildren);
        aLOD.ComputeLODData(bRecurseIntoChildren, Progress);
        aLOD.ComputeAllLODMeshes(bRecurseIntoChildren, Progress);
        string meshPath = "Assets/Models/Instances/Prefabs/" + go.name+go.GetInstanceID() + ".asset";
        int nCounter = 0;
        bool bAssetAlreadyCreated = System.IO.File.Exists(meshPath);
        Debug.LogError($"bAssetAlreadyCreated£º{bAssetAlreadyCreated}");
        if (bAssetAlreadyCreated == true)
        {
            System.IO.File.Delete(meshPath);
            bAssetAlreadyCreated = System.IO.File.Exists(meshPath);
        }
        AutomaticLODHelper.SaveMeshAssetsRecursive(go, go, meshPath, true, bAssetAlreadyCreated, ref nCounter);
        if(mats!=null&& mats.Length > go.transform.childCount)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i);
                MeshRenderer mr = child.GetComponent<MeshRenderer>();

                mr.material = mats[i];
            }

            MeshRenderer mr0 = go.GetComponent<MeshRenderer>();
            mr0.material = mats[0];
        }

    }
}
