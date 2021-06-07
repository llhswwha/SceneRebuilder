using System;
using UnityEngine;
using System.Collections;
using System.Threading;

public class ThreadBehaviourObject : ThreadBehaviourBase
{
    private Action<object> uiAction;

    public void Run<T>(Func<T> mainAction, Action<object> uiAction,string taskName)
    {
        BeforeRun();
        this.TaskName = taskName;
        this.uiAction = uiAction;
        ThreadPool.QueueUserWorkItem(obj =>
        {
            try
            {
                if (mainAction != null)
                {
                    //LogInfo.Info("Run MainAction", taskName);
                    Result = mainAction();
                }
            }
            catch (Exception ex)
            {
                LogInfo.Error("ThreadBehaviour", taskName+"|"+ex.ToString());
            }
            IsFinished = true;
            //Debug.LogInfo("Finished");
        });
    }

    public object Result;

    protected override void AfterFinished()
    {
        if (uiAction != null)
        {
            //LogInfo.Info("Run UIAction",TaskName);
            uiAction(Result);
        }
    }
}
