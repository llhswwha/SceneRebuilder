using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Battlehub.RTEditor;
//using Battlehub.RTSL;

namespace StardardShader
{
    /// <summary>
    /// StardardShader相关帮助类
    /// </summary>
    public class StardardShaderMatHelper : MonoBehaviour
    {
        public static StardardShaderMatHelper Instance;

        public static Material Transparent_Mat;
        //public Material mat;

        public static bool EnableTransparent=true;

        public Material transparent_Mat;
        //public Material mat;

        public bool enableTransparent=true;

        public bool isChanged=false;

        void Start()
        {
            //SetMaterialRenderingMode(mat, RenderingMode.Fade);
            Instance=this;
            EnableTransparent=enableTransparent;
            Transparent_Mat=transparent_Mat;
        }

        void Update()
        {
            if(isChanged){
                isChanged=false;
                EnableTransparent=enableTransparent;
                Transparent_Mat=transparent_Mat;
            }
        }


        public static void SetMaterialColor(Material mat,Color c){
            // mat.color = c;
            mat.SetColor("_BaseColor",c);
        }


        //设置材质的渲染模式
        public static void SetMaterialRenderingMode(Material material
        , RenderingMode renderingMode
        ,RenderPipeline pipelineType
        ,Color color0)
        {
            //Debug.Log("SetMaterialRenderingMode:"+renderingMode+"|"+color0);
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    if(pipelineType == RenderPipeline.Standard)
                    {
                        material.SetFloat("_Mode", 0);
                    }
                    else if(pipelineType==RenderPipeline.URP)
                    {
                        material.SetFloat("_Surface", 0);//Material面板上的SurfaceType
                    }  
                    else if(pipelineType==RenderPipeline.HDRP)
                    {
                        material.SetFloat("_SurfaceType", 0);//Material面板上的SurfaceType
                    }                  
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case RenderingMode.Cutout:
                    if (pipelineType == RenderPipeline.Standard)
                    {
                        material.SetFloat("_Mode", 1);
                    }
                    else if (pipelineType == RenderPipeline.URP)
                    {
                        material.SetFloat("_Surface", 1);//Material面板上的SurfaceType
                    }
                    else if(pipelineType==RenderPipeline.HDRP)
                    {
                        material.SetFloat("_SurfaceType", 1);//Material面板上的SurfaceType
                    } 
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case RenderingMode.Fade:
                    if (pipelineType == RenderPipeline.Standard)
                    {
                        material.SetFloat("_Mode", 2);
                    }
                    else if (pipelineType == RenderPipeline.URP)
                    {
                        material.SetFloat("_Surface", 1);//Material面板上的SurfaceType
                    }
                    else if(pipelineType==RenderPipeline.HDRP)
                    {
                        material.SetFloat("_SurfaceType", 1);//Material面板上的SurfaceType
                    } 

                    // material.SetFloat("_Mode", 2);

                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case RenderingMode.Transparent:

                    //Debug.Log("RenderingMode.Transparent:"+material);

                    var color=material.GetColor("_BaseColor");
                    var texture=material.GetTexture("_BaseColorMap");

                    if(Transparent_Mat==null){
                        Transparent_Mat=Resources.Load<Material>("HDRPLit_Transparent_Double");
                    }

                    if(EnableTransparent==true){
                        Material m=GameObject.Instantiate(Transparent_Mat);
                        material.CopyPropertiesFromMaterial(m);
                        material.SetColor("_BaseColor", color);
                        material.SetTexture("_BaseColorMap", texture);
                    }

                    

                //     // Debug.Log("_SurfaceType:"+material.HasProperty("_SurfaceType")+"|"+material.GetFloat("_SurfaceType"));
                //     // Debug.Log("_Surface:"+material.HasProperty("_Surface")+"|"+material.GetFloat("_Surface"));
                //     // Debug.Log("_Mode:"+material.HasProperty("_Mode")+"|"+material.GetFloat("_Mode"));

                //     if (pipelineType == RenderPipeline.Standard)
                //     {
                //         material.SetFloat("_Mode", 3);
                //     }
                //     else if (pipelineType == RenderPipeline.URP)
                //     {
                //         material.SetFloat("_Surface", 1);//Material面板上的SurfaceType
                //     }
                //     else if(pipelineType==RenderPipeline.HDRP)
                //     {
                //         material.SetFloat("_SurfaceType", 1);//Material面板上的SurfaceType
                //     } 

                //     // material.SetFloat("_Mode", 3);

                //     // material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                //     // material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                //     // material.SetInt("_ZWrite", 0);
                //     // material.DisableKeyword("_ALPHATEST_ON");
                //     // material.DisableKeyword("_ALPHABLEND_ON");
                //     // material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                //     // material.renderQueue = 3000;

                //  material.SetOverrideTag("RenderType", "Transparent");
                //  material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                //  material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                //  material.SetInt("_ZWrite", 0);
                //  material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                //  material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                //  material.SetShaderPassEnabled("ShadowCaster", false);

                    break;
            }
        }

        /// <summary>
        /// 获取标准材质实体
        /// </summary>
        public static StardardMaterialEntity GetStardardMaterialBackups(Material mat)
        {
            StardardMaterialEntity entity = new StardardMaterialEntity(mat);
            return entity;
        }

        ///// <summary>
        ///// 获取标准材质实体
        ///// </summary>
        //public static StardardMaterialEntity GetStardardMaterialEntity(Material material)
        //{
        //    StardardMaterialEntity entity = new StardardMaterialEntity();
        //    if (material.renderQueue == 2000)
        //    {
        //        entity.mode = RenderingMode.Opaque;
        //    }
        //    else if (material.renderQueue == 2450)
        //    {
        //        entity.mode = RenderingMode.Cutout;
        //    }
        //    else if (material.renderQueue == 3000)
        //    {
        //        int n = material.GetInt("_SrcBlend");
        //        if (n == (int)UnityEngine.Rendering.BlendMode.SrcAlpha)
        //        {
        //            entity.mode = RenderingMode.Fade;
        //        }
        //        else if (n == (int)UnityEngine.Rendering.BlendMode.One)
        //        {
        //            entity.mode = RenderingMode.Transparent;
        //        }

        //    }

        //    entity.color = material.color;

        //    Debug.Log(string.Format("mode:{0},color:{1}", entity.mode, entity.color));

        //    return entity;
        //}

        ///// <summary>
        ///// 设置材质的相关信息，通过材质实体类
        ///// </summary>
        //public static void SetMaterialByEntity(Material mat, StardardMaterialEntity entityT)
        //{
        //    SetMaterialRenderingMode(mat, entityT.mode);
        //    mat.color = entityT.color;
        //}
    }

    public enum RenderPipeline
    {
        Standard,
        URP,
        HDRP
    }

    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }
    //public class StardardMaterialEntity
    //{
    //    public RenderingMode mode;//模式
    //    public Color color;//透明度值
    //}

    /// <summary>
    /// 对应材质备份实体
    /// </summary>
    [Serializable]
    public class StardardMaterialEntity
    {
        public Material mat;//材质
        [SerializeField]
        private Material matBackups;//材质备份，不可修改

        public StardardMaterialEntity(Material m)
        {
            mat = m;
            matBackups = new Material(m);
        }

        public void SetBackup()
        {
            matBackups = new Material(mat);
        }

        /// <summary>
        /// 恢复原有材质
        /// </summary>
        public void Recover()
        {
            if (matBackups != null && mat != null)
            {
                mat.CopyPropertiesFromMaterial(matBackups);
            }
        }
    }
}
