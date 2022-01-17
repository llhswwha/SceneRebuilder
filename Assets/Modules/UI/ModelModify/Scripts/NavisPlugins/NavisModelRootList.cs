using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavisModelRootList : SingletonBehaviour<NavisModelRootList>
{
    public List<NavisModelRoot> ModelRoots = new List<NavisModelRoot>();

    [ContextMenu("GetModelRoots")]
    public void GetModelRootObjects()
    {
        ModelRoots = GameObject.FindObjectsOfType<NavisModelRoot>(true).ToList();
        //ModelRoots.Sort((a, b) => { return a.bimInfos.Count.CompareTo(b.bimInfos.Count); });
        foreach(var root in ModelRoots)
        {
            root.RefreshBIMList();
            if (root.resultCount.GetSumCount() == 0)
            {
                root.SetResultCount();
            }
        }
        ModelRoots.Sort((a, b) => { return b.resultCount.NotFoundCount.CompareTo(a.resultCount.NotFoundCount); });
    }


    public bool EnableProgressLog = false;
    public ProgressArg currentProgress;

    [ContextMenu("RootsBindBim")]
    public void RootsBindBim()
    {
        ProgressBarHelper.EnableProgressLog = EnableProgressLog;

        //SceneRebuildManager.Instance.LoadScenes();

        for (int i = 0; i < ModelRoots.Count; i++)
        {
            var root = ModelRoots[i];
            currentProgress = new ProgressArg("RootsBindBim", i, ModelRoots.Count, root.name);
            
            if (ProgressBarHelper.DisplayCancelableProgressBar(currentProgress,true))
            {
                break;
            }
            //ProgressBarHelper.EnableProgressBar = false;
            root.BindBimInfo(currentProgress);
        }

        ProgressBarHelper.EnableProgressBar = true;
        ProgressBarHelper.ClearProgressBar();
        currentProgress = null;
    }

    public NavisModelRoot Root1;

    public NavisModelRoot Root2;

    public LODTwoRenderersList ModelRendersWaiting_Old_BIM;

    public LODTwoRenderersList ModelRenders1;

    public LODTwoRenderersList ModelRenders2;

    public LODTwoRenderersList ModelRenders3;

    public LODTwoRenderersList ModelRendersNoConnectedBims;

    public LODTwoRenderersList ModelRendersNoConnectedTransform;

    public void CompareModelByBIM(bool isIncludeStructure)
    {
        if (Root1 == null || Root2 == null)
        {
            Debug.LogError($"Root1 == null || Root2 == null Root1:{Root1} Root2:{Root2}");
            return;
        }
        if (Root1.ModelName != Root2.ModelName)
        {
            Debug.LogError($"Root1.ModelName != Root2.ModelName Root1:{Root1.ModelName} Root2:{Root2.ModelName}");
            return;
        }


        BIMModelInfoList bimListPart = Root1.GetBims(null, null);
        BIMModelInfoList bimListAll = Root2.GetBims(null, null);
        BIMModelInfoDictionary bimDictAll = new BIMModelInfoDictionary(bimListAll, null);
        //var bimList3 = bimList2.FindSameList(bimList1);
        ModelUpdateManager updater = ModelUpdateManager.Instance;
        //updater.SetOldNewModel(Root1.gameObject, Root2.gameObject);
        BIMModelInfoList bimListNoConnected = new BIMModelInfoList(bimListAll);

        LODTwoRenderersList ModelRendersWaiting_New_BIM_All = new LODTwoRenderersList("BIM_New_All");
        ModelRendersWaiting_New_BIM_All.InitList(bimListAll);

        ModelRendersWaiting_Old_BIM = new LODTwoRenderersList("BIM_Old");
        foreach (var bimOfPart in bimListPart)
        {
            if (isIncludeStructure == false && (InitNavisFileInfoByModelSetting.Instance.IsStructrue(bimOfPart.name) || InitNavisFileInfoByModelSetting.Instance.IsStructrue(bimOfPart.MName)))
            {
                continue;
            }
            BIMModelInfo bimOfAll = bimDictAll.GetBIMModelByGuid(bimOfPart.Guid);
            LODTwoRenderers twoRenderers = new LODTwoRenderers(bimOfPart.gameObject);
            if (bimOfAll != null)
            {
                twoRenderers.SetLOD1(bimOfAll.gameObject);
                bimListNoConnected.Remove(bimOfAll);
            }
            else // ==null;
            {
                //bimListNoConnected.Add(bimOfAll);
            }
            ModelRendersWaiting_Old_BIM.Add(twoRenderers);
        }
        Debug.LogError($"CompareModelByBIM bimListPart:{bimListPart.Count} bimDictAll:{bimListAll.Count} bimListNoConnected:{bimListNoConnected.Count}");
        ModelUpdateManager.Instance.SetTargetList(ModelRendersWaiting_Old_BIM, ModelRendersWaiting_New_BIM_All);


        ModelRendersNoConnectedBims = new LODTwoRenderersList("bimListNoConnected");
        ModelRendersNoConnectedBims.InitList(bimListNoConnected);

        var notFoundTransforms1 = Root1.GetCurrentTransformList();//老的模型的未分配的模型对象
        Debug.LogError("notFoundTransforms1:" + notFoundTransforms1.Count);
        if (notFoundTransforms1.Count == 0)
        {
            Root1.BindBimInfo();
            notFoundTransforms1 = Root1.GetCurrentTransformList();//老的模型的未分配的模型对象
            Debug.LogError("notFoundTransforms1:" + notFoundTransforms1.Count);
        }
        if (Root1.ModelList == null)
        {
            Root1.BindBimInfo();
            notFoundTransforms1 = Root1.GetCurrentTransformList();//老的模型的未分配的模型对象
            Debug.LogError("notFoundTransforms1:" + notFoundTransforms1.Count);
        }
        

        ModelRendersNoConnectedBims = new LODTwoRenderersList("NoConnectedBims");
        ModelRendersNoConnectedBims.InitList(bimListNoConnected);

        ModelRendersNoConnectedTransform = new LODTwoRenderersList("NoConnectedTransform");
        ModelRendersNoConnectedTransform.InitList(notFoundTransforms1);
        ModelUpdateManager.Instance.SetTargetList(ModelRendersNoConnectedBims, ModelRendersNoConnectedTransform);


        //ModelRenders1 = new LODTwoRenderersList("ModelList1");
        //ModelRenders1 = new LODTwoRenderersList("ModelList2");

        //var notFoundModels1 = Root1.ModelList.allModels_drawable_nozero;
        //1.把Root2中的Root1已经BIM关联的模型都删除了
        //2.剩下的模型看看能不能找到对应的老模型，能找到则替换，不能找到，则添加，添加到对应的楼层中。
        List<BIMModelInfo> bimList22 = new List<BIMModelInfo>();
        DictionaryList1ToN<Transform, BIMModelInfo> dictTransform2Bims = new DictionaryList1ToN<Transform, BIMModelInfo>();
        for (int i = 0; i < bimListNoConnected.Count; i++)
        {
            BIMModelInfo bim2 = bimListNoConnected[i];
            if (notFoundTransforms1.Count == 0)
            {
                break;
            }
            var closedTransform = bim2.FindClosedTransform(notFoundTransforms1);
            if (closedTransform == null)
            {
                Debug.LogError($"closedTransform == null notFoundTransforms1:{notFoundTransforms1.Count} bim2:{bim2}");
                continue;
            }
            float distance = Vector3.Distance(bim2.transform.position, closedTransform.position);
            bool isSameName = TransformHelper.IsSameName(bim2.name, closedTransform.name);

            //notFoundTransforms1.Remove(closedTransform);//避免重复

            //LODTwoRenderers twoRenderers = new LODTwoRenderers(bim2.gameObject);
            //twoRenderers.SetLOD1(closedTransform.gameObject);
            //ModelRenders1.Add(twoRenderers);

            dictTransform2Bims.AddItem(closedTransform, bim2);
        }

        

        BIMModelInfoList bimListNoConnected_11 = new BIMModelInfoList();
        BIMModelInfoList bimListNoConnected_1N = new BIMModelInfoList();

        ModelRenders1 = new LODTwoRenderersList("ModelList1");
        //ModelRenders1.InitList(bimListNoConnected_11);


        List<Transform> notFoundTransforms2 = new List<Transform>(notFoundTransforms1);

        foreach (var t in dictTransform2Bims.Keys)
        {
            notFoundTransforms2.Remove(t);

            var list = dictTransform2Bims[t];
            if (list.Count == 1)
            {
                LODTwoRenderers twoRenderers = new LODTwoRenderers(list[0].gameObject);
                twoRenderers.SetLOD1(t.gameObject);
                ModelRenders1.Add(twoRenderers);

                //bimListNoConnected.Remove(list[0]);

                bimListNoConnected_11.Add(list[0]);
            }
            else
            {
                var closedBim = TransformHelper.FindClosedComponent(list, t.position);
                //bimListNoConnected.Remove(closedBim);
                bimListNoConnected_11.Add(closedBim);

                LODTwoRenderers twoRenderers = new LODTwoRenderers(closedBim.gameObject);
                twoRenderers.SetLOD1(t.gameObject);
                ModelRenders1.Add(twoRenderers);

                list.Remove(closedBim);
                bimListNoConnected_1N.AddRange(list);
            }
        }

        ModelRenders2 = new LODTwoRenderersList("ModelList2");
        ModelRenders2.InitList(bimListNoConnected_1N);

        ModelRenders3 = new LODTwoRenderersList("ModelList3");
        ModelRenders3.InitList(notFoundTransforms2);
        ModelUpdateManager.Instance.SetTargetList(ModelRenders2, ModelRenders3);

        //for (int i = 0; i < bimList2.Count; i++)
        //{
        //    BIMModelInfo bim2 = bimList2[i];
        //}

        ProgressBarHelper.ClearProgressBar();
    }

    public void CompareModelByBIM_Position()
    {
        if (Root1 == null || Root2 == null)
        {
            Debug.LogError($"Root1 == null || Root2 == null Root1:{Root1} Root2:{Root2}");
            return;
        }
        if (Root1.ModelName != Root2.ModelName)
        {
            Debug.LogError($"Root1.ModelName != Root2.ModelName Root1:{Root1.ModelName} Root2:{Root2.ModelName}");
            return;
        }


        var bimList1 = Root1.GetBims(null, null);
        var bimList2 = Root2.GetBims(null, null);
        BIMModelInfoDictionary dict2 = new BIMModelInfoDictionary(bimList2, null);
        var bimList3 = bimList2.FindSameList(bimList1);
        //List<BIMModelInfo> bimList3 = new List<BIMModelInfo>();
        ModelUpdateManager updater = ModelUpdateManager.Instance;
        //updater.SetOldNewModel(Root1.gameObject, Root2.gameObject);

        ModelRendersWaiting_Old_BIM = new LODTwoRenderersList("BIM_Old");
        ModelRendersWaiting_Old_BIM.InitList(bimList1);
        ModelRenders1 = new LODTwoRenderersList("BIM_New");
        ModelRenders1.InitList(bimList3);
        updater.SetTargetList(ModelRendersWaiting_Old_BIM, ModelRenders1);

        //foreach (var bim1 in bimList1)
        //{
        //    if (isIncludeStructure == false && (InitNavisFileInfoByModelSetting.Instance.IsStructrue(bim1.name) || InitNavisFileInfoByModelSetting.Instance.IsStructrue(bim1.MName)))
        //    {
        //        continue;
        //    }
        //    BIMModelInfo bim2 = dict2.GetBIMModelByGuid(bim1.Guid);
        //    LODTwoRenderers twoRenderers = new LODTwoRenderers(bim1.gameObject);
        //    if (bim2 != null)
        //    {
        //        twoRenderers.SetLOD1(bim2.gameObject);
        //    }
        //    else // ==null;
        //    {
        //        bimList3.Add(bim2);
        //    }
        //    ModelRendersWaiting_Old_BIM.Add(twoRenderers);
        //}
        Debug.LogError($"CompareModelByBIM bimList1:{bimList1.Count} bimList2:{bimList3.Count}");
    }
}
