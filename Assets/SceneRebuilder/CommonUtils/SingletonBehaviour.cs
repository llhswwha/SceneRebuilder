using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T :MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if(_instance==null)
            {
                _instance = GameObject.FindObjectOfType<T>();
            }
            if(_instance==null)
            {
                GameObject insGo=new GameObject(typeof(T).Name);
                _instance = insGo.AddComponent<T>();
            }
            return _instance;
        }
    }
}
