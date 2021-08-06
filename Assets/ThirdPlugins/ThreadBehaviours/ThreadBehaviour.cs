using System;
using UnityEngine;
using System.Collections;
using System.Threading;

public class ThreadBehaviour : ThreadBehaviourBase
{
    public string Id;

    void Start()
    {
        Id = Guid.NewGuid().ToString();
    }

    private Action uiAction;

    public string threadName;

    public string mainActionText;

    public string uiActionText;

    public void Run(Action uiAction, string threadName)
    {
        Run(null, uiAction, threadName);
    }

    public void Run(Action mainAction, Action uiAction,string threadName)
    {
        BeforeRun();

        this.threadName = threadName;
        this.uiAction = uiAction;

        mainActionText = "";
        if (mainAction != null)
        {
            mainActionText = mainAction.Target + "." + mainAction.Method;
        }

        uiActionText = "";
        if (uiAction != null)
        {
            uiActionText = uiAction.Target + "." + uiAction.Method;
        }

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
                        //Log.Info("ThreadBehaviour", string.Format("[{0}]Run MainAction:{1}", DateTime.Now.ToLongTimeString(), threadName));
                    }
                    mainAction();
                }
            }
            catch (Exception ex)
            {
                Log.Error("ThreadBehaviour", ex.ToString());
            }
            IsFinished = true;
        });
#endif
    }
    /// <summary>
    /// WEBGL中，暂时以协程代替
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator ExcuteThreadInWebGL(Action action)
    {
        Debug.Log("ThreadBehaviour.ExcuteThreadInWebGL...");
        yield return null;
        if (action != null) action();
        IsFinished = true;
    }
    protected override void AfterFinished()
    {
        if (uiAction != null)
        {
            if (!string.IsNullOrEmpty(threadName))
            {
                //Log.Info("ThreadBehaviour", string.Format("[{0}]Run UIAction:{1}", DateTime.Now.ToLongTimeString(), threadName));
            }
            uiAction();
            uiAction = null;
        }
    }
}
