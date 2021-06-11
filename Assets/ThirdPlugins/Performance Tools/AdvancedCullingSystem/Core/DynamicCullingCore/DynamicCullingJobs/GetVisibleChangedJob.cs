using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using System;
using AdvancedCullingSystem.StaticCullingCore;

namespace AdvancedCullingSystem.DynamicCullingCore
{
    [BurstCompile]
    public struct GetVisibleChangedJob : IJob
    {
        public NativeList<float> toHideList;

        public NativeList<float> toShowList;

        [ReadOnly]
        public NativeList<int> visibleObjects;

        public NativeList<float> timers;

        public float objectsLifetime;

        public NativeHashMap<int,int> HideRenders;

        public NativeHashMap<int,int> ShowRenders;

        public void Execute()
        {
            int c=0;
                while (c < visibleObjects.Length)//根据碰撞检测和时间，显示或者隐藏物体
                {
                    int id = visibleObjects[c];
                    // try
                    // {
                        // var renderer=_indexToRenderer[id];
                        if (timers[c] > objectsLifetime) //隐藏
                        {
                            // if(IsSetVisible)
                            //     RendererHelper.HideRenderer(renderer);//隐藏物体

                            // _visibleObjects.RemoveAtSwapBack(c);
                            // _timers.RemoveAtSwapBack(c);

                            // if (!HideRenders.ContainsKey(id))
                            //     HideRenders.Add(id,id);

                            if(!HideRenders.ContainsKey(id)){
                                HideRenders.Add(id,id);
                                ShowRenders.Remove(id);
                                toHideList.Add(id);
                            }
                        }
                        else
                        {
                            //visibleRenderers.Add(renderer);

                            // if (IsSetVisible)
                            //     RendererHelper.ShowRenderer(renderer);

                            // c++;

                            // if (!ShowRenders.ContainsKey(id))
                            //     ShowRenders.Add(id,id);

                            if(!ShowRenders.ContainsKey(id)){
                                ShowRenders.Add(id,id);
                                HideRenders.Remove(id);
                                toShowList.Add(id);
                            }
                        }
                    // }
                    // catch (MissingReferenceException)
                    // {
                    //     // _renderersForRemoveIDs.Add(id);
                    //     // c++;
                    // }

                    c++;
                }
        }
    }
}
