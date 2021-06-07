using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unity.ComnLib.Helpers
{
    public static class CameraHelper
    {
        /// <summary>
        /// 鼠标是否在摄像头的范围内
        /// </summary>
        /// <param name="c"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static bool IsMouseInCamera(this Camera c, float offset = 0)
        {
            Vector2 pos = Input.mousePosition;
            Vector3 v2 = c.ScreenToViewportPoint(pos);
            //Debug.LogInfo("CameraHelper.IsMouseInCamera:" + v2);
            if (v2.x >= offset && v2.x <= 1 - offset && v2.y >= offset && v2.y <= 1 - offset)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取当前鼠标所在的摄像头
        /// </summary>
        /// <returns></returns>
        public static Camera GetCurrentCamera(this List<Camera> cameras)
        {
            float offset = 0f;
            foreach (Camera c in cameras)
            {
                if (c.IsMouseInCamera())
                    return c;
            }
            return null;
        }
    }
}
