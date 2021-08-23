using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MeshJobs;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using GPUInstancer;
using System.IO;

public class BigSmallListInfo
    {
        public List<MeshRenderer> bigModels = new List<MeshRenderer>();
        public List<MeshRenderer> smallModels = new List<MeshRenderer>();
        public float sumVertex_Big = 0;
        public float sumVertex_Small = 0;

        public override string ToString()
        {
            return $"bigModels:{bigModels.Count} smallModels:{smallModels.Count} sumVertex_Big:{sumVertex_Big} sumVertex_Small:{sumVertex_Small}";
        }

        public BigSmallListInfo()
        {

        }

        public BigSmallListInfo(MeshPoints[] meshFilters, float maxLength)
        {
            Init(meshFilters, maxLength);
        }

        public BigSmallListInfo(MeshPoints[] meshFilters)
        {
            Init(meshFilters, AcRTAlignJobSetting.Instance.MaxModelLength);
        }

        public BigSmallListInfo(MeshFilter[] meshFilters)
        {
            List<MeshPoints> meshPoints = MeshPoints.GetMeshPoints(meshFilters);
            Init(meshPoints.ToArray(), AcRTAlignJobSetting.Instance.MaxModelLength);
        }

        public BigSmallListInfo(GameObject root)
        {
            var meshFilters = root.GetComponentsInChildren<MeshFilter>(true);
            List<MeshPoints> meshPoints = MeshPoints.GetMeshPoints(meshFilters);
            Init(meshPoints.ToArray(), AcRTAlignJobSetting.Instance.MaxModelLength);
        }

    public void Init(MeshPoints[] meshFilters, float maxLength)
        {
            DateTime start = DateTime.Now;
            //var meshFilters=GetMeshFilters();
            // float minCount=float.MaxValue;
            // float maxCount=0;
            // float sumCount=0;
            // float avgCount=0;
            // List<string> sizeList = new List<string>();
            List<float> lengthList = new List<float>();
            // List<MeshRenderer> bigModels=new List<MeshRenderer>();
            // List<MeshRenderer> smallModels=new List<MeshRenderer>();
            //foreach(MeshFilter mf in meshFilters)
            int sumVertex_Big = 0;
            int sumVertex_Small = 0;
            for (int i = 0; i < meshFilters.Length; i++)
            {
                var mf = meshFilters[i];
                if (mf == null) continue;
                if (mf.sharedMesh == null) continue;

                float progress = (float)i / meshFilters.Length;
                float percents = progress * 100;

                //if(ProgressBarHelper.DisplayCancelableProgressBar("GetBigSmallRenderers", $"{i}/{meshFilters.Length} {percents:F2}% of 100%", progress))
                //{
                //    //ProgressBarHelper.ClearProgressBar();
                //    break;
                //} 
                Bounds bounds = mf.sharedMesh.bounds;
                Vector3 scale = mf.transform.lossyScale;
                scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.y));
                Vector3 size = bounds.size;
                // string strSize=$"({size.x},{size.y},{size.z})";
                float length = size.x * scale.x;
                if (size.y * scale.y > length)
                {
                    length = size.y * scale.y;
                }
                if (size.z * scale.z > length)
                {
                    length = size.z * scale.y;
                }

                if (!lengthList.Contains(length))
                {
                    lengthList.Add(length);
                }

                // if(!sizeList.Contains(strSize))
                // {
                //     sizeList.Add(strSize);
                // }

                MeshRendererInfo rendererInfo = mf.GetComponent<MeshRendererInfo>();
                if (rendererInfo != null)
                {
                    MeshRenderer mr = rendererInfo.meshRenderer;
                    if (rendererInfo.rendererType == MeshRendererType.Structure)
                    {
                        this.bigModels.Add(mr);
                        sumVertex_Big += mf.vertexCount;
                    }
                    else if (rendererInfo.rendererType == MeshRendererType.Detail)
                    {
                        this.smallModels.Add(mr);
                        sumVertex_Small += mf.vertexCount;
                    }
                    else
                    {
                        if (length < maxLength)
                        {
                            this.smallModels.Add(mr);
                            sumVertex_Small += mf.vertexCount;
                        }
                        else
                        {
                            this.bigModels.Add(mr);
                            sumVertex_Big += mf.vertexCount;
                        }
                    }
                }
                else
                {
                    MeshRenderer mr = mf.GetComponent<MeshRenderer>();
                    if (length < maxLength)
                    {
                        this.smallModels.Add(mr);
                        sumVertex_Small += mf.vertexCount;
                    }
                    else
                    {
                        this.bigModels.Add(mr);
                        sumVertex_Big += mf.vertexCount;
                    }
                }
            }

            //ProgressBarHelper.ClearProgressBar();
            this.sumVertex_Small = sumVertex_Small / 10000f;
            this.sumVertex_Big = sumVertex_Big / 10000f;
            //Debug.LogWarning($"GetBigSmallRenderers maxLength:{maxLength},(bigModels:{this.bigModels.Count}+smallModels:{this.smallModels.Count}=BS:{this.bigModels.Count + this.smallModels.Count},Renderers:{meshFilters.Length}),(bigVertex:{this.sumVertex_Big},smallVertex:{this.sumVertex_Small}),Time:{(DateTime.Now - start).TotalMilliseconds:F1}ms");
        }
    }