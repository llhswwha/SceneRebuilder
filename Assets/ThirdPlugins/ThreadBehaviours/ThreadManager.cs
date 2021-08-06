using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using Base.Common;
using UnityEngine;

public class ThreadManager : MonoBehaviour, IAsyncTask
{
    public void RunTask<T>(Func<T> mainAction, Action<T> uiAction, string taskName)
    {
        Run<T>(mainAction, uiAction, taskName);
    }

    private static GameObject GameObj;

    private static void InitGameObject()
    {
        if (GameObj == null)
        {
            GameObj = new GameObject("ThreadManager");
            Instance=GameObj.AddComponent<ThreadManager>();
            Instance.InitThreadPool();
        }
    }

    public static ThreadManager Instance;

    void Awake()
    {
        Instance = this;
    }

    private int PoolCount = 10;

    public static void SetPoolCount(int count)
    {
        InitGameObject();
        Instance.PoolCount = count;
        if (!Instance.IsBusy())
        {
            Instance.InitThreadPool();
        }
    }

    private bool IsBusy()
    {
        foreach (ThreadBehaviourBase thread in Threads)
        {
            if (thread.IsBusy)
            {
                return true;
            }
        }
        return false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        for (int i = 0; i < ThreadInfos.Count; i++)
        {
            ThreadBehaviour threadBehaviour = GetThread();
            if (threadBehaviour == null)
            {
                Log.Info("Wait Thread");
                return;
            }
            ThreadInfo info = ThreadInfos[i];
            ThreadInfos.RemoveAt(i);
            i--;
            threadBehaviour.Run(info.MainAction, info.UiAction, info.Name);
            Log.Info("Run A Thread");
        }
        InfoCount = ThreadInfos.Count;
    }

    public void InitThreadPool()
    {
        ThreadBehaviour[] threads=gameObject.GetComponents<ThreadBehaviour>();
        foreach (ThreadBehaviour thread in threads)
        {
            GameObject.Destroy(thread);
        }
        Threads.Clear();
        for (int i = 0; i < PoolCount; i++)
        {
            ThreadBehaviour threadBehaviour = gameObject.AddComponent<ThreadBehaviour>();
            Threads.Add(threadBehaviour);
        }
    }

    private ThreadBehaviour GetThread()
    {
        foreach (ThreadBehaviour thread in Threads)
        {
            if (!thread.IsBusy)
            {
                return thread;
            }
        }
        return null;
    }

    public List<ThreadBehaviourBase> Threads = new List<ThreadBehaviourBase>();

    public List<ThreadInfo> ThreadInfos = new List<ThreadInfo>();

    public int InfoCount;

    private void RunThread(Action mainAction, Action uiAction, string threadName)
    {
        //ThreadBehaviour threadBehaviour = = GetThread();

        //threadBehaviour.Run(mainAction, uiAction, threadName);
        ThreadInfo info = new ThreadInfo(mainAction, uiAction, threadName);
        ThreadInfos.Add(info);
        InfoCount = ThreadInfos.Count;
    }


    ///// <summary>
    ///// 在子线程中运行代码
    ///// </summary>
    ///// <param name="mainAction">耗时的代码</param>
    ///// <param name="finished">修改主界面的代码</param>
    //public static void Run(Action uiAction)
    //{
    //    InitGameObject();
    //    ThreadBehaviour threadBehaviour = GameObj.AddComponent<ThreadBehaviour>();
    //    threadBehaviour.Run(uiAction);
    //}

    /// <summary>
    /// 在子线程中运行代码
    /// </summary>
    /// <param name="mainAction">耗时的代码</param>
    /// <param name="finished">修改主界面的代码</param>
    public static void Run(Action mainAction, Action uiAction, string taskName)
    {
        InitGameObject();
        if (IsUsePool)
        {
            Instance.RunThread(mainAction, uiAction, taskName);
        }
        else
        {
            ThreadBehaviour threadBehaviour = GameObj.AddComponent<ThreadBehaviour>();
            threadBehaviour.IsOnce = true;
            threadBehaviour.Run(mainAction, uiAction, taskName);
        }
    }

    //public static void Run<T>(Func<T> mainAction, Action<object> uiAction, string taskName)
    //{
    //    InitGameObject();
    //    ThreadBehaviourObject threadBehaviour = GameObj.AddComponent<ThreadBehaviourObject>();
    //    threadBehaviour.Run<T>(mainAction, uiAction);
    //}

    public static void Run<T>(Func<T> mainAction, Action<T> uiAction, string taskName)
    {
        InitGameObject();
        ThreadBehaviourObject threadBehaviour = GameObj.AddComponent<ThreadBehaviourObject>();
        threadBehaviour.Run<T>(mainAction, (obj)=>
        {
            T t = default(T);
            if(obj is T)
            {
                t = (T)obj;
            }
            else
            {
                Debug.LogWarning(string.Format("ThreadManager.Run: [obj is not T] taskName:{0}",taskName));
            }

            if (uiAction != null)
            {
                uiAction(t);
            }
        },taskName);
    }

    public static bool IsUsePool;

    public static void Run(Func<object> mainAction, Action<object> uiAction)
    {
        //InitGameObject();
        //ThreadBehaviourObject threadBehaviour = GameObj.AddComponent<ThreadBehaviourObject>();
        //threadBehaviour.Run(mainAction, uiAction);
        Run<object>(mainAction, uiAction, "");
    }


    /// <summary>
    /// 在子线程中运行代码
    /// </summary>
    public static void Run(Func<bool> main, Action trueAction, Action falseAction, string threadName = "")
    {
        Log.Info("创建线程:" + threadName);
        InitGameObject();
        ThreadBehaviourBool threadBehaviour = GameObj.AddComponent<ThreadBehaviourBool>();
        threadBehaviour.Run(main, trueAction, falseAction, threadName);
    }
}
