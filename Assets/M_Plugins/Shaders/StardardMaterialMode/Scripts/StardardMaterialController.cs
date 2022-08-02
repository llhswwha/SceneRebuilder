using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
//using UnityEngine.Rendering.PostProcessing;

namespace StardardShader
{
    /// <summary>
    /// StardardMaterial控制脚本
    /// </summary>
    public class StardardMaterialController : MonoBehaviour
    {
        public bool isEditorRecover = true;//编辑器状态下是否自动恢复材质
        public List<StardardMaterialEntity> matEntitys;//材质信息,包括材质备份
        private bool isCanGetMaterials = true;

        public GameObject Target;

        public bool UseSharedMaterial=true;

        private string StandardRPShaderName = "Standard";

        public static string URPShaderName = "Universal Render Pipeline/Lit";

        public static string HDRPShaderName = "HDRP/Lit";

        public static RenderPipeline defaultPipeline=RenderPipeline.HDRP;

        private Dictionary<int, string> unTransLayerDic;
        public static bool IsURPOrHDRP(string sName){
            return sName==URPShaderName || sName == HDRPShaderName;
        }


        //PostProcessLayer postP;//滤镜

        //public PostProcessLayer PostP
        //{
        //    get {
        //        if (postP == null&& Camera.main)
        //        {
        //            //postP = GameObject.FindObjectOfType<PostProcessLayer>();
        //            postP = Camera.main.GetComponent<PostProcessLayer>();
        //        }
        //        return postP;
        //    }
        //}
        //public Color color;
        public bool isStart = false;
        // Use this for initialization
        public void Start()
        {
            if (isStart) return;
            if (Target == null)
            {
                Target = this.gameObject;
            }
            InitUnTransparentLayer();
            AnewGetMaterials();
            isStart = true;
        }
        
        private void InitUnTransparentLayer()
        {
            unTransLayerDic = new Dictionary<int, string>();
            int personLayer = LayerMask.NameToLayer("Person");
            if (personLayer > 0) unTransLayerDic.Add(personLayer,"Person");
        }

        public static StardardMaterialController Add(GameObject o)
        {
            StardardMaterialController controller = o.AddMissingComponent<StardardMaterialController>();
            controller.AnewGetMaterials();
            return controller;
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (isEditorRecover)
            {
                RecoverMaterials();
            }

#endif
        }

        [ContextMenu("SetMatsTransparent")]
        public void SetMatsTransparent()
        {
            Color color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            SetRenderingMode(RenderingMode.Transparent, color, true);
        }

        [ContextMenu("SetMatsTransparent")]
        public void SetMatsTransparent(float transparent)
        {
            Color color = new Color(0.5f, 0.5f, 0.5f, transparent);
            SetRenderingMode(RenderingMode.Transparent, color, true);
        }

        [ContextMenu("SetMatsOpaque")]
        public void SetMatsOpaque()
        {
            SetRenderingMode(RenderingMode.Opaque);
        }

        [ContextMenu("SetMatsCutout")]
        public void SetMatsCutout()
        {
            SetRenderingMode(RenderingMode.Cutout);
        }

        [ContextMenu("SetMatsFade")]
        public void SetMatsFade()
        {
            SetRenderingMode(RenderingMode.Fade);
        }

        //public void SetMatsTransparent(float aphaT)
        //{
        //    Color colorT = new Color(0.2f, 0.2f, 0.2f, aphaT);
        //    SetRenderingMode(RenderingMode.Transparent, colorT, true);
        //}

        public void SetMatsTransparent(Color colorT, bool isClearMainTexture = true)
        {
            //if (PostP)
            //{
            //    PostP.enabled = false;
            //}
            SetRenderingMode(RenderingMode.Transparent, colorT, isClearMainTexture);
        }

        /// <summary>
        /// 设置材质列表
        /// </summary>
        /// <param name="valueT"></param>

        public void SetRenderingMode(RenderingMode mode)
        {
            if (matEntitys == null) return;
            foreach (StardardMaterialEntity entity in matEntitys)
            {
                Material mat = entity.mat;
                if (mat.shader.name == "Standard")
                {
                    //Debug.Log(mat.shader.name);
                    //Debug.Log(mat.renderQueue);
                    StardardShaderMatHelper.SetMaterialRenderingMode(mat, mode,defaultPipeline,mat.color);
                    //mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, valueT);
                }
            }
        }

        /// <summary>
        /// 设置材质列表
        /// </summary>
        /// <param name="valueT"></param>

        public void SetRenderingMode(RenderingMode mode, float valueT = 0.5F)
        {
            if (matEntitys == null) return;
            foreach (StardardMaterialEntity entity in matEntitys)
            {
                Material mat = entity.mat;
                //if (mat.shader.name == "Standard")
                {
                    //Debug.Log(mat.shader.name);
                    //Debug.Log(mat.renderQueue);
                    Color newColor=new Color(mat.color.r, mat.color.g, mat.color.b, valueT);
                    StardardShaderMatHelper.SetMaterialRenderingMode(mat, mode,defaultPipeline,newColor);
                    SetMaterialColor(mat,newColor);
                }
            }
        }

        public static void SetMaterialColor(Material mat,Color c){
            // mat.color = c;
            mat.SetColor("_BaseColor",c);
        }

        /// <summary>
        /// 设置材质列表
        /// </summary>
        /// <param name="valueT"></param>

        public void SetRenderingMode(RenderingMode mode, Color colorT, bool isClearMainTexture)
        {
            if (matEntitys == null) return;
            foreach (StardardMaterialEntity entity in matEntitys)
            {
                Material mat = entity.mat;
                string matName = mat.shader.name;
                //if (matName == StandardRPShaderName||matName== URPShaderName)
                {
                    RenderPipeline pipeline = RenderPipeline.Standard;
                    if (IsURPOrHDRP(matName)) pipeline = defaultPipeline;
                    StardardShaderMatHelper.SetMaterialRenderingMode(mat, mode,pipeline,colorT);
                    if (IsURPOrHDRP(matName)) mat.SetColor("_BaseColor", colorT);//轻量级渲染管线，普通mat.color改不了颜色
                    else mat.color = colorT;
                    if (isClearMainTexture)
                    {
                        ClearMainTexture(mat);
                    }
                }
            }
        }

        /// <summary>
        /// 清除主贴图
        /// </summary>
        public void ClearMainTexture(Material mat)
        {
            mat.mainTexture = null;
        }

        //[ContextMenu("ShowMatsInfo")]
        //public void ShowMatsInfo()
        //{
        //    foreach (StardardMaterialEntity entity in matEntitys)
        //    {
        //        Material mat = entity.mat;
        //        if (mat.shader.name == "Standard")
        //        {
        //            //Debug.Log(mat.shader.name);
        //            Debug.Log(mat.renderQueue);
        //        }
        //    }
        //}


        /// <summary>
        /// 恢复材质的到原本状态
        /// </summary>
        [ContextMenu("RecoverMaterials")]
        public void RecoverMaterials()
        {
            //if (PostP)
            //{
            //    PostP.enabled = true;
            //}
            //if (matEntitys == null) return;
            if (matEntitys != null)
            {
                foreach (StardardMaterialEntity entity in matEntitys)
                {
                    entity.Recover();
                }
            }

        }

        public List<string> Exceptions=new List<string>();

        /// <summary>
        /// 获取指定文件夹下的所有材质，备份材质
        /// </summary>
        [ContextMenu("GetMaterials")]
        public void GetMaterials()
        {
            if(Exceptions.Count==0){
                Exceptions.Add("Default-Material");
                Exceptions.Add("Ocean");
            }
            if (isCanGetMaterials == false) return;
            isCanGetMaterials = false;
            if (matEntitys == null)
            {
                matEntitys = new List<StardardMaterialEntity>();
            }

            if (Target == null)
            {
                Target = this.gameObject;
            }
            Renderer[] renders = Target.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer r in renders)
            {
                //透明时，定位人物等不透明
                if (unTransLayerDic != null && unTransLayerDic.ContainsKey(r.transform.gameObject.layer)) continue;
                if(UseSharedMaterial){
                    foreach (Material m in r.sharedMaterials)
                    {
                        StardardMaterialEntity entity = matEntitys.Find((item) => item.mat == m);
                        if (entity == null)
                        {
                            if (m == null || Exceptions.Contains(m.name))
                            {
                                continue;
                            }
                            StardardMaterialEntity entityT = new StardardMaterialEntity(m);
                            matEntitys.Add(entityT);
                        }
                    }
                }
                else{
                    foreach (Material m in r.materials)
                    {
                        StardardMaterialEntity entity = matEntitys.Find((item) => item.mat == m);
                        if (entity == null)
                        {
                            if (m == null || Exceptions.Contains(m.name))
                            {
                                continue;
                            }
                            StardardMaterialEntity entityT = new StardardMaterialEntity(m);
                            matEntitys.Add(entityT);
                        }
                    }
                }
            }

            //SkinnedMeshRenderer[] skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            //foreach (Renderer r in skinnedMeshRenderers)
            //{
            //    foreach (Material m in r.sharedMaterials)
            //    {
            //        StardardMaterialEntity entity = matEntitys.Find((item) => item.mat == m);
            //        if (entity == null)
            //        {
            //            if (m == null)
            //            {
            //                continue;
            //            }
            //            StardardMaterialEntity entityT = new StardardMaterialEntity(m);
            //            matEntitys.Add(entityT);
            //        }
            //    }
            //}

        }

        /// <summary>
        /// 重新,获取指定文件夹下的所有材质，并备份材质
        /// </summary>
        [ContextMenu("REGetMaterials")]
        public void AnewGetMaterials()
        {
            RecoverMaterials();//先恢复材质原始颜色
            isCanGetMaterials = true;
            if (matEntitys == null)
            {
                matEntitys = new List<StardardMaterialEntity>();
            }
            else
            {
                matEntitys.Clear();
            }
            GetMaterials();
        }
    }
}
