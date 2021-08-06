using System;
using UnityEngine;
using System.Threading;
using System.Collections;
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
#if UNITY_WEBGL
        StartCoroutine(ExcuteThreadInWebGL(mainAction));
#else
        ThreadPool.QueueUserWorkItem(obj =>
        {
            try
            {
                if (mainAction != null)
                {
                    if (!string.IsNullOrEmpty(threadName))
                    {
                        Log.Info("Run MainAction", threadName);
                    }
                    Result = mainAction();
                }
            }
            catch (Exception ex)
            {
                Log.Error("ThreadBehaviour", ex.ToString());
            }
            IsFinished = true;
            //Debug.Log("Finished:" + Result);
        });
#endif
    }
    /// <summary>
    /// WEBGL中，暂时以协程代替
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator ExcuteThreadInWebGL(Func<bool> action)
    {
        Debug.Log("ThreadBehaviourBool.ExcuteThreadInWebGL...");
        yield return null;
        Result = action();
        IsFinished = true;
    }
    protected override void AfterFinished()
    {
        if (Result)
        {
            if (trueAction != null)
            {
                //Debug.Log("Run True Action");
                trueAction();
            }
        }
        else
        {
            if (falseAction != null)
            {
                //Debug.Log("Run False Action");
                falseAction();
            }
        }
    }
}
