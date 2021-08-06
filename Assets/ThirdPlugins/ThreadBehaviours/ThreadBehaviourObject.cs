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
#if UNITY_WEBGL
        StartCoroutine(ExcuteThreadInWebGL<T>(mainAction));
#else
        ThreadPool.QueueUserWorkItem(obj =>
        {
            try
            {
                if (mainAction != null)
                {
                    //Log.Info("Run MainAction", taskName);
                    Result = mainAction();
                }
            }
            catch (Exception ex)
            {
                Log.Error("ThreadBehaviour", taskName+"|"+ex.ToString());
            }
            IsFinished = true;
            //Debug.Log("Finished");
        });
#endif
    }
    /// <summary>
    /// WEBGL中，暂时以协程代替
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator ExcuteThreadInWebGL<T>(Func<T> action)
    {
        Debug.Log("ThreadBehaviourObject.ExcuteThreadInWebGL...");
        yield return null;
        Result = action();
        IsFinished = true;
    }
    public object Result;

    protected override void AfterFinished()
    {
        if (uiAction != null)
        {
            //Log.Info("Run UIAction",TaskName);
            uiAction(Result);
        }
    }
}
