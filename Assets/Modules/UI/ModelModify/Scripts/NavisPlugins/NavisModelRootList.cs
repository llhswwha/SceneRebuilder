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

    public void UpdateModelByBIM()
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

        var notFoundTransforms1 = Root1.GetCurrentTransformList();
        var notFoundModels1 = Root1.ModelList.allModels_drawable_nozero;
        var bimList1 = Root1.bimInfos;
        var bimList2 = Root2.bimInfos;

        ModelUpdateManager.Instance.SetOldNewModel(Root1.gameObject, Root2.gameObject);

        LODTwoRenderersList ModelRendersWaiting_Old_BIM = new LODTwoRenderersList("BIM_Old");
        LODTwoRenderersList ModelRendersWaiting_New_BIM = new LODTwoRenderersList("BIM_New");
    }
}
