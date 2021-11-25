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
}
