//using HighlightingSystem;
//using HighlightPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StardardShader;
using Knife.HDRPOutline.Core;
//using DG.Tweening;

public class HightlightModuleBase : MonoBehaviour
{

    //private Highlighter highlightStandard;
    //private HighlightEffect highlightURP;

    private RenderPipeline renderType;
    private bool isInit;

    public static List<HightlightModuleBase> highlightObjs;

    private void Awake()
    {
        Init();
    }
    #region Public Methods

    public void Init()
    {
        if (isInit) return;
        renderType = RenderPipeline.HDRP;

        //if (RoomFactory.Instance)
        //{
        //    renderType = RoomFactory.Instance.RenderPipelineType;
        //    if (renderType == RenderPipeline.URP)
        //    {
        //        highlightURP = gameObject.AddMissingComponent<HighlightEffect>();
        //    }
        //    else if (renderType == RenderPipeline.HDRP)
        //    {
        //        highlightURP = gameObject.AddMissingComponent<HighlightEffect>();
        //    }
        //    else
        //    {
        //        highlightStandard = gameObject.AddMissingComponent<Highlighter>();
        //    }
        //}
        //else if (HighlightManage.Instance)
        //{
        //    renderType = HighlightManage.Instance.RenderPipelineType;
        //}
        //else
        //{
        //    highlightStandard = gameObject.AddMissingComponent<Highlighter>();
        //}
    }

    ///// <summary>
    ///// Renderers reinitialization. 
    ///// Call this method if your highlighted object has changed it's materials, renderers or child objects.
    ///// Can be called multiple times per update - renderers reinitialization will occur only once.
    ///// </summary>
    //public void ReinitMaterials()
    //{
    //    if(renderType==RenderPipeline.Standard)
    //    {
    //        highlightStandard.ReinitMaterials();
    //    }
    //}  
    //public void SeeThrough(bool isOn)
    //{
    //    if (renderType == RenderPipeline.Standard)
    //    {
    //        highlightStandard.seeThrough=isOn;
    //    }
    //}
    //private Tween flashTween;
    /// <summary>
    /// Turn on flashing.
    /// </summary>
    public void FlashingOn()
    {
        Init();
        //if (renderType == RenderPipeline.Standard)
        //{
        //    highlightStandard.FlashingOn();
        //}
        isFlashing = true;
        AddToCache();
    }

    /// <summary>
    /// Turn on flashing from color1 to color2.
    /// </summary>
    /// <param name='color1'>
    /// Starting color.
    /// </param>
    /// <param name='color2'>
    /// Ending color.
    /// </param>
    public void FlashingOn(Color color1, Color color2)
    {
        Init();
        //if (renderType == RenderPipeline.Standard && highlightStandard!=null)
        //{
        //    highlightStandard.FlashingOn(color1,color2);
        //}else if(renderType==RenderPipeline.URP && highlightURP!=null)
        //{
        //    highlightURP.glow = 0;

        //    highlightURP.overlayMinIntensity = 0.5f;
        //    highlightURP.overlay = 1;
        //    highlightURP.overlayColor = color2;
        //    highlightURP.overlayAnimationSpeed = 2;

        //    highlightURP.outline = 0.1f;
        //    highlightURP.outlineColor = color2;
        //    highlightURP.outlineVisibility = Visibility.AlwaysOnTop;
        //    highlightURP.seeThrough = SeeThroughMode.Never;
        //    highlightURP.highlighted = true;
        //}
        //else if (renderType == RenderPipeline.HDRP)
        {
            if (gameObject.GetComponent<Renderer>() == null) return;
            OutlineObject obj = gameObject.AddMissingComponent<OutlineObject>();
            if (obj)
            {
                Shader s = Shader.Find("Knife/Knife-HDRPOutline_Unlit");
                if (s)
                {
                    obj.Material = new Material(s);
                    obj.Color = color2;
                    //if (flashTween != null) flashTween.Kill();
                    //flashTween = DOTween.ToAlpha(() => obj.Color, x => obj.Color = x, 0.3f, 0.5f).SetEase(Ease.Linear).SetLoops(-1);
                }
            }
        }
        isFlashing = true;
        AddToCache();
    }
    private bool isFlashing;

    public bool IsFlashing()
    {
        return isFlashing;
    }
    /// <summary>
    /// Turn on flashing from color1 to color2 and specified frequency.
    /// </summary>
    /// <param name='color1'>
    /// Starting color.
    /// </param>
    /// <param name='color2'>
    /// Ending color.
    /// </param>
    /// <param name='freq'>
    /// Flashing frequency (times per second).
    /// </param>
    public void FlashingOn(Color color1, Color color2, float freq)
    {
        Init();
        //if (renderType == RenderPipeline.Standard && highlightStandard!=null)
        //{
        //    highlightStandard.FlashingOn(color1, color2,freq);
        //}
        //else if (renderType == RenderPipeline.URP && highlightURP!=null)
        //{
        //    highlightURP.glow = 0;
        //    highlightURP.overlayMinIntensity = 0.5f;
        //    highlightURP.overlay = 1;
        //    highlightURP.overlayColor = color2;
        //    highlightURP.overlayAnimationSpeed = freq;

        //    highlightURP.outline = 1f;
        //    highlightURP.outlineColor = color2;
        //    highlightURP.outlineVisibility = Visibility.AlwaysOnTop;
        //    highlightURP.seeThrough = SeeThroughMode.Never;
        //    highlightURP.highlighted = true;
        //}
        //else if (renderType == RenderPipeline.HDRP)
        {
            if (gameObject.GetComponent<Renderer>() == null) return;
            OutlineObject obj = gameObject.AddMissingComponent<OutlineObject>();
            if (obj)
            {
                Shader s = Shader.Find("Knife/Knife-HDRPOutline_Unlit");
                if (s)
                {
                    obj.Material = new Material(s);
                    obj.Color = color2;
                    //if (flashTween != null) flashTween.Kill();
                    //flashTween = DOTween.ToAlpha(() => obj.Color, x => obj.Color = x, 0.25f, freq).SetEase(Ease.Linear).SetLoops(-1);
                }
            }
        }
        isFlashing = true;
        AddToCache();
    }

    /// <summary>
    /// Turn off flashing.
    /// </summary>
    public void FlashingOff()
    {
        //if (renderType == RenderPipeline.Standard && highlightStandard!=null)
        //{
        //    highlightStandard.FlashingOff();
        //}
        //else if (renderType == RenderPipeline.URP && highlightURP!=null)
        //{
        //    highlightURP.glow = 0;
        //    highlightURP.overlayMinIntensity = 0;
        //    highlightURP.overlay = 0;
        //    highlightURP.outline = 0;
        //    highlightURP.highlighted = false;
        //}
        //else if (renderType == RenderPipeline.HDRP)
        {
            if (gameObject.GetComponent<Renderer>() == null) return;
            OutlineObject obj = gameObject.GetComponent<OutlineObject>();
            if (obj)
            {
                DestroyImmediate(obj);
            }
            //if (flashTween != null) flashTween.Kill();
        }
        isFlashing = false;
        RemoveFromCache();
    }   

    public static void HighlightOn(GameObject go)
    {
        //HightlightModuleBase h = go.AddMissingComponent<HightlightModuleBase>();
        //h.ConstantOn(Color.green);

        HighlightOn(go, Color.green);
    }

    public static void HighlightOn(GameObject go, Color color)
    {
        //HightlightModuleBase h = go.AddMissingComponent<HightlightModuleBase>();
        //h.ConstantOn(color);
        var renderers = go.GetComponentsInChildren<Renderer>(true);
        foreach(var r in renderers)
        {
            HightlightModuleBase h = r.gameObject.AddMissingComponent<HightlightModuleBase>();
            h.ConstantOn(color);
        }
    }

    public static void HighlightOff(GameObject go)
    {
        //HightlightModuleBase h = go.AddMissingComponent<HightlightModuleBase>();
        //h.ConstantOff();

        var renderers = go.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            HightlightModuleBase h = r.gameObject.GetComponent<HightlightModuleBase>();
            if(h)
                h.ConstantOff();
        }
    }

    public static void FlashingOn(GameObject go, Color colorFrom,Color colorTo,float frequence)
    {
        //HightlightModuleBase h = go.AddMissingComponent<HightlightModuleBase>();
        //h.ConstantOn(color);
        var renderers = go.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            HightlightModuleBase h = r.gameObject.AddMissingComponent<HightlightModuleBase>();
            h.FlashingOn(colorFrom, colorTo, frequence);
        }
    }

    public static void FlashingOff(GameObject go)
    {
        //HightlightModuleBase h = go.AddMissingComponent<HightlightModuleBase>();
        //h.ConstantOff();

        var renderers = go.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            HightlightModuleBase h = r.gameObject.GetComponent<HightlightModuleBase>();
            if (h)
                h.FlashingOff();
        }
    }

    [ContextMenu("HighlightOn")]
    public void HighlightOn()
    {
        HighlightOn(this.gameObject);
    }

    [ContextMenu("HighlightOff")]
    public void HighlightOff()
    {
        HighlightOff(this.gameObject);
    }

    /// <summary>
    /// Fade in constant highlighting using specified color and transition duration.
    /// </summary>
    /// <param name="color">
    /// Constant highlighting color.
    /// </param>
    /// <param name="time">
    /// Transition duration.
    /// </param>
    public void ConstantOn(Color color, float time = 0.25f)
    {
        //Debug.Log("HightlightModuleBase.ConstantOn renderType:"+renderType);

        Init();
        //if (renderType == RenderPipeline.Standard && highlightStandard!=null)
        //{
        //    highlightStandard.ConstantOn(color,time);
        //}
        //else if (renderType == RenderPipeline.URP && highlightURP!=null)
        //{
        //    highlightURP.glow = 0;
        //    highlightURP.overlayMinIntensity = 0;
        //    highlightURP.overlay = 0;
        //    highlightURP.outline = 1;
        //    highlightURP.outlineColor = color;
        //    highlightURP.outlineVisibility = Visibility.AlwaysOnTop;
        //    highlightURP.seeThrough = SeeThroughMode.Never;
        //    highlightURP.highlighted = true;
        //}
        //else
        {

            if (gameObject.GetComponent<Renderer>() == null) return;
            OutlineObject obj = gameObject.AddMissingComponent<OutlineObject>();
            if (obj)
            {
                if (obj.Material == null)
                {
                    Shader s = Shader.Find("Knife/Knife-HDRPOutline_Unlit");
                    obj.Material = new Material(s);
                }
                obj.Color = color;
            }
        }
        AddToCache();
    }
    /// <summary>
    /// 清除之前所有的高亮
    /// </summary>
    public static void ClearHighlightOff()
    {
        if (highlightObjs == null) return;
        for(int i=highlightObjs.Count-1;i>=0;i--)
        {
            if (highlightObjs[i] != null)
            {
                if (highlightObjs[i].isFlashing) highlightObjs[i].FlashingOff();
                else highlightObjs[i].ConstantOffImmediate();
            }
        }
        highlightObjs.Clear();
    }

    public void AddToCache()
    {
        if (highlightObjs == null) highlightObjs = new List<HightlightModuleBase>();
        if(!highlightObjs.Contains(this))highlightObjs.Add(this);
    }
    public void RemoveFromCache()
    {
        if (highlightObjs == null) highlightObjs = new List<HightlightModuleBase>();
        if (highlightObjs.Contains(this)) highlightObjs.Remove(this);
    }

    public void ConstantOnInnerGlow(Color color,float strength,float width)
    {
        Init();
        //if (renderType == RenderPipeline.URP && highlightURP!=null)
        //{
        //    highlightURP.glow = 0;
        //    highlightURP.overlayMinIntensity = 0;
        //    highlightURP.overlay = 0;
        //    highlightURP.outline = 0;
        //    highlightURP.innerGlow = strength;
        //    highlightURP.innerGlowWidth = width;
        //    highlightURP.seeThrough = SeeThroughMode.Never;
        //    highlightURP.highlighted = true;
        //}
        //else{
        //    //Debug.LogWarning("HightlightModuleBase.ConstantOnInnerGlow renderType:"+renderType);


        //}
        AddToCache();
    }
    /// <summary>
    /// Fade out constant highlighting using specified transition duration.
    /// </summary>
    /// <param name="time">
    /// Transition time.
    /// </param>
    public void ConstantOff(float time = 0.25f)
    {
        //if (renderType == RenderPipeline.Standard && highlightStandard!=null)
        //{
        //    highlightStandard.ConstantOff(time);
        //}
        //else if (renderType == RenderPipeline.URP && highlightURP!=null)
        //{
        //    highlightURP.glow = 0;
        //    highlightURP.overlayMinIntensity = 0;
        //    highlightURP.overlay = 0;
        //    highlightURP.outline = 0;
        //    highlightURP.highlighted = false;
        //}
        //else
        {
            //Debug.LogWarning("HightlightModuleBase.ConstantOff renderType:"+renderType);
            if (gameObject.GetComponent<Renderer>() == null) return;
            OutlineObject objT = gameObject.GetComponent<OutlineObject>();
            if (objT) DestroyImmediate(objT);
        }
        RemoveFromCache();
    }
   
    /// <summary>
    /// Turn on constant highlighting using specified color immediately (without fading in).
    /// </summary>
    /// <param name='color'>
    /// Constant highlighting color.
    /// </param>
    public void ConstantOnImmediate(Color color)
    {
        Init();
        //if (renderType == RenderPipeline.Standard && highlightStandard!=null)
        //{
        //    highlightStandard.ConstantOnImmediate(color);
        //}
        //else if (renderType == RenderPipeline.URP && highlightURP!=null)
        //{
        //    highlightURP.glow = 0;
        //    highlightURP.overlayMinIntensity = 0;
        //    highlightURP.overlay = 0;
        //    highlightURP.outline = 1;
        //    highlightURP.outlineColor = color;
        //    highlightURP.outlineVisibility = Visibility.AlwaysOnTop;
        //    highlightURP.seeThrough = SeeThroughMode.Never;
        //    highlightURP.highlighted = true;
        //}
        //else
        {
            //if (MyOutlineManager.Instance != null)
            //{
            //    MyOutlineManager.Instance.SelectGOs(new List<GameObject> { gameObject}, true);
            //}

            if (gameObject.GetComponent<Renderer>() == null) return;
            OutlineObject obj = gameObject.AddMissingComponent<OutlineObject>();
            if (obj)
            {                
                if(obj.Material==null)
                {
                    Shader s = Shader.Find("Knife/Knife-HDRPOutline_Unlit");
                    obj.Material = new Material(s);
                }
                obj.Color = color;
            }
        }
        AddToCache();
    }

    /// <summary>
    /// Turn off constant highlighting immediately (without fading out).
    /// </summary>
    public void ConstantOffImmediate()
    {
        //if (renderType == RenderPipeline.Standard && highlightStandard!=null)
        //{
        //    highlightStandard.ConstantOffImmediate();
        //}
        //else if (renderType == RenderPipeline.URP && highlightURP!=null)
        //{
        //    highlightURP.glow = 0;
        //    highlightURP.overlayMinIntensity = 0;
        //    highlightURP.overlay = 0;
        //    highlightURP.outline = 0;
        //    highlightURP.innerGlow = 0;
        //    highlightURP.highlighted = false;
        //}
        //else
        {
            //MyOutlineManager.Instance.RecoverGO();
            if (gameObject.GetComponent<Renderer>() == null) return;
            OutlineObject objT = gameObject.GetComponent<OutlineObject>();
            if (objT) DestroyImmediate(objT);
        }
        RemoveFromCache();
    }

    /// <summary>
    /// Destroy this Highlighter component.
    /// </summary>
    public void Die()
    {
        RemoveFromCache();
        Destroy(this);
    }
        #endregion
}
