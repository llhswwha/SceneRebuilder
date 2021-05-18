
using Unity.Mathematics;

public static class MathematicsDistance 
{
    public static float3 GetMinDistancePoint(float3 p1, float3[] points)
    {
        //DateTime start = DateTime.Now;

        float distance = float.MaxValue;
        float3 result = float3.zero;
        int index = 0;
        for (int i = 0; i < points.Length; i++)
        {
            float3 p2 = points[i];
            float dis = math.distance(p1,p2);
            if (dis < distance)
            {
                distance = dis;
                result = p2;
                index = i;
            }
        }
        //if(distance<zero)
        //    DebugLog(string.Format("GetMinDistancePoint 用时:{0}s，距离:{1}，序号:{2}", (DateTime.Now - start).TotalSeconds, distance, index));
        return result;
    }

    public static float GetDistance(float3[] points1,float3[] points2,bool showLog=false)
    {
        //Debug.Log("GetDistance:"+showLog);
        //DateTime start=DateTime.Now;
        //float dis=-1;
        float disSum=0;

        float distance = 0;
        // var points1 = vertices1;
        // var points2 = vertices2World;
        //List<float> disList = new List<float>();
        int zeroCount = 0;
        int i = 0;
        for (; i < points1.Length; i++)
        {
            float3 p1 = points1[i];
            float3 p2 = GetMinDistancePoint(p1, points2);
            float d = math.distance(p1, p2);
            disSum+=d;//不做处理，直接累计
            if (d <= DistanceSetting.zero)
            {
                //if(showLog)Debug.Log($"GetDistance1[{i}]d1:{d}|{distance}");
                zeroCount++;
                if (zeroCount > DistanceSetting.zeroMax)//没必要计算完，大概100个都位置相同的话，就是可以的了。
                {
                    //return distance;//不能返回0哦
                    break;
                }
                else
                {

                }
                d=0;//不考虑累计，小于zero就是0了。，比如10个E-06就E-05，100个就是E-04，1000个就是E-03了，0.001了，那我就不是很有把握是不是重合了。
            }
            else{
                //if(showLog)Debug.Log($"GetDistance2[{i}]d2:{d}|{distance}");
            }
            //disList.Add(d);
            distance += d;

            if (distance > DistanceSetting.maxDistance)//没必要计算完，整体距离很大的话，就是已经是不行的了
            {
                break;
            }
        }
        //if(showLog)Debug.Log($"GetVertexDistanceEx 用时:{(DateTime.Now - start).TotalMilliseconds:F2}ms，累计:{disSum:F7},结果:{distance:F7},序号:{i}/{points1.Length}");
        return distance;
    }
}
