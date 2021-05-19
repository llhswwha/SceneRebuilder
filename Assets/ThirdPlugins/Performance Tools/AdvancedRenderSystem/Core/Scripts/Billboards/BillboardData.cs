using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace NGS.AdvancedRenderSystem
{
    public struct BillboardData
    {
        public int billboardIndex;
        public float3 position;

        public float lastUpdateTime;
        public float distance;

        public BillboardData(int billboardIndex, float3 position)
        {
            this.billboardIndex = billboardIndex;
            this.position = position;

            lastUpdateTime = 0;
            distance = 0;
        }

        public BillboardData UpdateData(float3 cameraPosition, float deltaTime)
        {
            lastUpdateTime += deltaTime;
            distance = math.distance(position, cameraPosition);

            return this;
        }
    }

    public struct BillboardsUpdateData
    {
        public BaseBillboard billboard;
        public float distance;
        public int idx;

        public BillboardsUpdateData(BaseBillboard billboard, float distance, int idx)
        {
            this.billboard = billboard;
            this.distance = distance;
            this.idx = idx;
        }
    }
}
