using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public static class ChineseInputWebGL
{

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern void InputShow(string GameObjectName, string InputID, string text, string fontsize, string indexStr, string inputRectStr);
 //   [DllImport("__Internal")]
	//public static extern void InputEnd ();    

#else
    public static void InputShow(string GameObjectName, string InputID, string text, string fontsize, string indexStr, string inputRectStr)
    {
        string data = string.Format("{0}|{1}|{2}|{3}|{4}|{5}",GameObjectName,InputID,text,fontsize,indexStr,inputRectStr);
        WebMsgReciver.Instance.SendeMsg("ChineseInput", data);
    }
    //public static void InputEnd() { }
#endif

}
