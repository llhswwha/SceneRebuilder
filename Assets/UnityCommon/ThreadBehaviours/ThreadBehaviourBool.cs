using System;
using UnityEngine;
using System.Threading;

public class ThreadBehaviourBool : ThreadBehaviourBase
{
    private Action trueAction;
    private Action falseAction;
    public bool Result;

    public void Run(Func<bool> mainAction, Action trueAction, Action falseAction, string threadName)
    {
        BeforeRun();

        this.trueAction = trueAction;
        this.falseAction = falseAction;
        ThreadPool.QueueUserWorkItem(obj =>
        {
            try
            {
                if (mainAction != null)
                {
                    if (!string.IsNullOrEmpty(threadName))
                    {
                        LogInfo.Info("Run MainAction", threadName);
                    }
                    Result = mainAction();
                }
            }
            catch (Exception ex)
            {
                LogInfo.Error("ThreadBehaviour", ex.ToString());
            }
            IsFinished = true;
            //Debug.LogInfo("Finished:" + Result);
        });
    }

    protected override void AfterFinished()
    {
        if (Result)
        {
            if (trueAction != null)
            {
                //Debug.LogInfo("Run True Action");
                trueAction();
            }
        }
        else
        {
            if (falseAction != null)
            {
                //Debug.LogInfo("Run False Action");
                falseAction();
            }
        }
    }
}
