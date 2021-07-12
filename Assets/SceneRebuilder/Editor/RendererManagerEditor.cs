using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(RendererManager))]
public class RendererManagerEditor : BaseEditor<RendererManager>
{
    public override void OnToolLayout(RendererManager item)
    {
        base.OnToolLayout(item);
        
       if(GUILayout.Button("InitRenderers_All"))
       {
           item.InitRenderers_All();
       }

        if(GUILayout.Button("InitIds"))
       {
           item.InitIds();
       }

        if(GUILayout.Button("CheckRendererParent"))
       {
           item.CheckRendererParent();
       }
       
    }
}
