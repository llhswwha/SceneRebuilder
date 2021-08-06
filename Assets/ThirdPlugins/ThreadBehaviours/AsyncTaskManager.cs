using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base.Common;
using UnityEngine;

public class AsyncTaskManager:MonoBehaviour,IAsyncTask
{
    public static AsyncTaskManager Instance;

    public void RunTask<T>(Func<T> mainAction, Action<T> uiAction, string taskName)
    {
        ThreadManager.Run<T>(mainAction, uiAction, taskName);
    }

    void Awake()
    {
        Instance = this;
        AsyncTaskHelper.Instance = this;
    }

    void Start()
    {
        
    }
}
