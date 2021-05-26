using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 组件相关辅助类
/// </summary>
public static class ComponentHelper
{
    static public T AddMissingComponent<T>(this GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();
        }
        return comp;
    }
}
