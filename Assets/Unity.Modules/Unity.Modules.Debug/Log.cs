using System;
using System.Collections.Generic;
using System.Threading;
using Base.Common;
using UnityEngine;
using LogType = Base.Common.LogType;
public static class Log
{
    public enum LogLevel
    {
        Info, Warning,Error,Debug
    }

    public static int MaxCount = 0;
    public static List<string> Logs = new List<string>();

    public static string Info(object tag)
    {
        return Add(LogLevel.Info, tag, "");
    }
	public static string Info(object tag,object obj)
	{
        return Add(LogLevel.Info, tag, obj);
	}

    public static string Debug(object tag)
    {
        return Add(LogLevel.Debug, tag, "");
    }
    public static string Debug(object tag, object obj)
    {
        return Add(LogLevel.Debug, tag, obj);
    }

    public static string Error(object tag)
    {
        return Add(LogLevel.Error, tag, "");
    }

    public static string Error(object tag, object obj)
    {
        return Add(LogLevel.Error, tag, obj);
    }

    public static string Alarm(object tag)
    {
        return Add(LogLevel.Warning, tag, "");
    }

    public static string Alarm(object tag, object obj)
    {
        return Add(LogLevel.Warning, tag, obj);
    }

    public static string Add(LogArg arg)
    {
        if (arg.Type == LogType.Alarm)
        {
            return Alarm(arg.Tag, arg.Msg);
        }
        else if (arg.Type == LogType.Error)
        {
            return Error(arg.Tag, arg.Msg);
        }
        else if(arg.Type == LogType.Debug)
        {
            return Debug(arg.Tag, arg.Msg);
        }
        else if(arg.Type == LogType.Info)
        {
            return Info(arg.Tag, arg.Msg);
        }
        else
        {
            return Info(arg.Tag, arg.Msg);
        }
    }

    public static Dictionary<string, bool> TagFilter = null;

    public static string Add(LogLevel level, object tag, object obj)
    {
        if (Enable == false) return "";

        string tagT = tag + "";
        if (TagFilter != null)
        {
            if (TagFilter.ContainsKey(tagT))
            {
                bool r = TagFilter[tagT];
                if (r == false)
                {
                    return "";
                }
            }
        }

        string text = string.Format("[{0}]{1}", tag, obj);
        //MonoBehaviour.print(text);
        DateTime now = DateTime.Now;
        text = string.Format("[{0}][{1}] {2}", now.ToString("HH:mm:ss fff"), level, text);

        if (PrintInUnity)
        {
            if (level == LogLevel.Warning)
            {
                UnityEngine.Debug.LogWarning(text);
            }
            else if (level == LogLevel.Error)
            {
                UnityEngine.Debug.LogError(text);
            }
            else if (level == LogLevel.Debug)
            {
                UnityEngine.Debug.LogError(text);
            }
            else// (level == LogLevel.Warning)
            {
                UnityEngine.Debug.Log(text);
            }
        }


        Logs.Add(text);
        if (Logs.Count > 1000)
        {
            Logs.Clear();
        }
        OnLogChanged(text);
        return text;
    }

    public static bool Enable = true;

    public static bool PrintInUnity = true;

    public static bool IsEnableEvent;

    public static event Action<string> LogChanged;
    private static void OnLogChanged(string obj)
    {
        if (!IsEnableEvent) return;
        var handler = LogChanged;
        if (handler != null) handler(obj);
    }

    public static void Clear()
    {
        Logs.Clear();
    }

    public static string GetText()
    {
        string text = "";
        foreach (string s in Logs)
        {
            text += s + "\n";
        }
        return text.TrimEnd();
    }

    public static string GetTextEx()
    {
        string text = "";
        for (int i = Logs.Count-1; i >=0; i--)
        {
            string s = Logs[i];
            text += s + "\n";
        }
        return text.TrimEnd();
    }

    /// <summary>
    /// 获取调用堆栈
    /// </summary>
    /// <returns></returns>
    public static string GetStackTrace()
    {
        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
        System.Diagnostics.StackFrame[] sfs = st.GetFrames();
        string txt = "";
        for (int u = 0; u < sfs.Length; ++u)
        {
            System.Reflection.MethodBase mb = sfs[u].GetMethod();
            txt += string.Format("[CALL STACK][{0}]: {1}.{2} ({3})\n", u, mb.DeclaringType.FullName, mb.Name, sfs[u].GetFileLineNumber());
        }
        return txt.Trim();
    }
}


