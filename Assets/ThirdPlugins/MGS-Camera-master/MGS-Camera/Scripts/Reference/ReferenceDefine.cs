/*************************************************************************
 *  Copyright © 2017-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  ReferenceDefine.cs
 *  Description  :  Define reference data for camera control.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/8/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Mogoson.CameraExtension
{
    /// <summary>
    /// Settings of mouse input.
    /// </summary>
    [Serializable]
    public struct MouseSettings
    {
        /// <summary>
        /// ID of mouse button.
        /// </summary>
        public int mouseButtonID;

        /// <summary>
        /// Sensitivity of mouse pointer.
        /// </summary>
        public float pointerSensitivity;

        /// <summary>
        /// Sensitivity of mouse ScrollWheel.
        /// </summary>
        public float wheelSensitivity;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mouseButtonID">ID of mouse button.</param>
        /// <param name="pointerSensitivity">Sensitivity of mouse pointer.</param>
        /// <param name="wheelSensitivity">Sensitivity of mouse ScrollWheel.</param>
        public MouseSettings(int mouseButtonID, float pointerSensitivity, float wheelSensitivity)
        {
            this.mouseButtonID = mouseButtonID;
            this.pointerSensitivity = pointerSensitivity;
            this.wheelSensitivity = wheelSensitivity;
        }
    }

    /// <summary>
    /// Range form min to max.
    /// </summary>
    [Serializable]
    public struct Range
    {
        /// <summary>
        /// Min value of range.
        /// </summary>
        public float min;

        /// <summary>
        /// Max value of range.
        /// </summary>
        public float max;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="min">Min value of range.</param>
        /// <param name="max">Max value of range.</param>
        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    [Serializable]
    public class TransformPos
    {
        /// <summary>
        /// Center of area.
        /// </summary>
        public string name;

        /// <summary>
        /// Center of area.
        /// </summary>
        public Transform transform;

        /// <summary>
        /// Center of area.
        /// </summary>
        public Vector3 position;

        public Vector3 getPositon()
        {
            // position=GetCenter(transform);
            // return position;

            if (transform)
            {
                return transform.position;
            }
            else//建筑可能被替换，同时建筑一般是不会移动的
            {
                return position;
            }
        }

        public void SetPosition(Vector3 pos){
            if (transform)
            {
                transform.position=pos;
            }
            this.position=pos;
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
            if (transform)
            {
                name = transform.name;
                position = GetCenter(transform);
            }
            else
            {
                name = "";
                position = Vector3.zero;
            }
        }

        public TransformPos(Transform transform)
        {
            this.transform = transform;
            if (transform)
            {
                name = transform.name;
                position = GetCenter(transform);
            }
            else
            {
                name = "";
                position = Vector3.zero;
            }
        }

        public static Dictionary<Transform,Vector3> t2c=new Dictionary<Transform, Vector3>();

        public static Vector3 GetCenter(Transform t )
        {
            Vector3 c=t.position;
            // if(t2c.ContainsKey(t)){
            //     c=t2c[t];
            // }
            // else{
            //     var maxMin=t.gameObject.GetBoundsMaxMinPoints();
            //     c=(maxMin[0]+maxMin[1])/2;
            //     t2c.Add(t,c);
            // }
            return c;
        }
    }

    /// <summary>
    /// Rectangle area on plane.
    /// </summary>
    [Serializable]
    public struct PlaneArea
    {
        public TransformPos centerPos;

        public Vector3 GetPos()
        {
            return centerPos.getPositon();
        }

        /// <summary>
        /// Center of area.
        /// </summary>
        public Transform center;

        ///// <summary>
        ///// Center of area.
        ///// </summary>
        //public string centerName;

        ///// <summary>
        ///// Center of area.
        ///// </summary>
        //public Vector3 centerPos;

        //public Vector3 GetPos()
        //{
        //    if (center)
        //    {
        //        return center.position;
        //    }
        //    else//建筑可能被替换，同时建筑一般是不会移动的
        //    {
        //        return centerPos;
        //    }
        //}

        public void SetCenter(Transform center)
        {
            this.center = center;
            this.centerPos = new TransformPos(center);
            Debug.Log($"PlaneArea.SetCenter1 centerPos:{centerPos.getPositon()} center:{center} centerPos:{center.position}");
        }

        public void SetCenter(TransformPos centerPos)
        {
            this.centerPos = centerPos;
            this.center = centerPos.transform;
            Debug.Log($"PlaneArea.SetCenter2 centerPos:{centerPos.getPositon()} center:{center} centerPos:{center.position}");
        }

        /// <summary>
        /// Width of area.
        /// </summary>
        public float width;

        /// <summary>
        /// Length of area.
        /// </summary>
        public float length;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="center">Center of area.</param>
        /// <param name="width">Width of area.</param>
        /// <param name="length">Length of area.</param>
        public PlaneArea(Transform center, float width, float length)
        {
            this.center = center;
            this.centerPos = new TransformPos(center);
            this.width = width;
            this.length = length;
        }
    }

    /// <summary>
    /// Target of camera align.
    /// </summary>
    [Serializable]
    public struct AlignTarget
    {
        public string GetLogText()
        {
            return string.Format("AlignVeiwToTarget center:{0},angles:{1},distance:{2} go:{3}",center.position,angles,distance, center);
        }

        //public bool IsNone;
        /// <summary>
        /// Center of align target.
        /// </summary>
        public Transform center;

        /// <summary>
        /// Center of align target.
        /// </summary>
        public TransformPos centerPos;

        public void SetCenter(Transform center)
        {
            this.center = center;
            this.centerPos = new TransformPos(center);
        }

        public void SetCenter(TransformPos centerPos)
        {
            this.center = centerPos.transform;
            this.centerPos = centerPos;
        }

        /// <summary>
        /// Angles of align.
        /// </summary>
        public Vector2 angles;

        public bool enableAngles;

        /// <summary>
        /// Distance from camera to target center.
        /// </summary>
        public float distance;

        /// <summary>
        /// Range limit of angle.
        /// </summary>
        public Range angleRange;

        /// <summary>
        /// Range limit of distance.
        /// </summary>
        public Range distanceRange;

        public override string ToString()
        {
            return $"angles:{angles};distance:{distance}";
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="center">Center of align target.</param>
        /// <param name="angles">Angles of align.</param>
        /// <param name="distance">Distance from camera to target center.</param>
        /// <param name="angleRange">Range limit of angle.</param>
        /// <param name="distanceRange">Range limit of distance.</param>
        public AlignTarget(Transform center, Vector2 angles, float distance, Range angleRange, Range distanceRange)
        {
            this.center = center;
            this.centerPos = new TransformPos(center);
            this.angles = angles;
            this.distance = distance;
            this.angleRange = angleRange;
            this.distanceRange = distanceRange;
            this.enableAngles = false;
            //this.IsNone = false;
        }
    }
}