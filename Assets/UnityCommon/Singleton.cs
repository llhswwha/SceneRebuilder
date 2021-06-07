using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance;

    protected void Awake()
    {
        //print("Awake:" + this);
        Instance = this as T;
    }

    public static void Hide()
    {
        Hide(Instance);
    }

    public static void Show()
    {
        Show(Instance);
    }

    public static void Hide(T instacne)
    {
        if (instacne)
        {
            instacne.gameObject.SetActive(false);
        }
    }

    public static void Show(T instacne)
    {
        if (instacne)
        {
            instacne.gameObject.SetActive(true);
        }
    }
}
