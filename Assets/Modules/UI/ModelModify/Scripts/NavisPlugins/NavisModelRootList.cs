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

    public LODTwoRenderersList ModelRendersWaiting_New_BIM;

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


        var bimList1 = Root1.GetBims(null,null);
        var bimList2 = Root2.GetBims(null, null);
        BIMModelInfoDictionary dict2 = new BIMModelInfoDictionary(bimList2, null);
        //var bimList3 = bimList2.FindSameList(bimList1);
        List<BIMModelInfo> bimList3 = new List<BIMModelInfo>();
        ModelUpdateManager updater = ModelUpdateManager.Instance;
        //updater.SetOldNewModel(Root1.gameObject, Root2.gameObject);

        ModelRendersWaiting_Old_BIM = new LODTwoRenderersList("BIM_Old");
        //ModelRendersWaiting_Old_BIM.InitList(bimList1);
        //ModelRendersWaiting_New_BIM = new LODTwoRenderersList("BIM_New");
        //ModelRendersWaiting_New_BIM.InitList(bimList3);
        //updater.SetTargetList(ModelRendersWaiting_Old_BIM, ModelRendersWaiting_New_BIM);
        foreach(var bim1 in bimList1)
        {
            if(isIncludeStructure==false && (InitNavisFileInfoByModelSetting.Instance.IsStructrue(bim1.name)|| InitNavisFileInfoByModelSetting.Instance.IsStructrue(bim1.MName)))
            {
                continue;
            }
            BIMModelInfo bim2 = dict2.GetBIMModelByGuid(bim1.Guid);
            LODTwoRenderers twoRenderers = new LODTwoRenderers(bim1.gameObject);
            if (bim2 != null)
            {
                twoRenderers.SetLOD1(bim2.gameObject);
            }
            else // ==null;
            {
                bimList3.Add(bim2);
            }
            ModelRendersWaiting_Old_BIM.Add(twoRenderers);
        }
        Debug.LogError($"CompareModelByBIM bimList1:{bimList1.Count} bimList2:{bimList3.Count}");

        //var notFoundTransforms1 = Root1.GetCurrentTransformList();
        //if (Root1.ModelList == null)
        //{
        //    Root1.BindBimInfo();
        //}
        //var notFoundModels1 = Root1.ModelList.allModels_drawable_nozero;


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
        //var bimList3 = bimList2.FindSameList(bimList1);
        List<BIMModelInfo> bimList3 = new List<BIMModelInfo>();
        ModelUpdateManager updater = ModelUpdateManager.Instance;
        //updater.SetOldNewModel(Root1.gameObject, Root2.gameObject);

        ModelRendersWaiting_Old_BIM = new LODTwoRenderersList("BIM_Old");
        ModelRendersWaiting_Old_BIM.InitList(bimList1);
        ModelRendersWaiting_New_BIM = new LODTwoRenderersList("BIM_New");
        ModelRendersWaiting_New_BIM.InitList(bimList3);
        updater.SetTargetList(ModelRendersWaiting_Old_BIM, ModelRendersWaiting_New_BIM);

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
