using System;
using UnityEngine;

namespace Unity.ComnLib
{
    public class TimeCounter
    {
        public static TimeSpan Run(Action action)
        {
            DateTime time = DateTime.Now;
            if (action != null)
            {
                action();
            }
            TimeSpan span = DateTime.Now - time;
            return span;
        }

        public static TimeSpan Run(Action action, string msg)
        {
            TimeSpan t = Run(action);
            Debug.Log(msg + ":" + t.TotalMilliseconds);
            return t;
        }
    }
}
