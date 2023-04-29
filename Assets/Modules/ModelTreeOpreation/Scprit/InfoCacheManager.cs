using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoCacheManager : MonoBehaviour
{
    public static InfoCacheManager Instance;
    [HideInInspector]
    public GameObject CacheInfo;//缓存的属性
    public void Awake()
    {
        Instance = this;
    }
}
