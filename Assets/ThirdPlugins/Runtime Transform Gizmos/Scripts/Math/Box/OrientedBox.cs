﻿using UnityEngine;
using System;
using System.Collections.Generic;

namespace RTEditor
{
    public class OrientedBox
    {
        #region Private Variables
        private Box _modelSpaceBox;

        private Vector3 _center = Vector3.zero;
        private Quaternion _rotation = Quaternion.identity;
        private Vector3 _scale = Vector3.one;

        private bool _allowNegativeScale = false;
        #endregion

        #region Public Properties
        public Box ModelSpaceBox { get { return _modelSpaceBox; } }
        public Vector3 ModelSpaceExtents { get { return _modelSpaceBox.Extents; } }
        public Vector3 ScaledExtents { get { return Vector3.Scale(ModelSpaceExtents, Scale); } }
        public Vector3 ModelSpaceSize { get { return _modelSpaceBox.Size; } set { _modelSpaceBox.Size = value; } }
        public Vector3 ScaledSize { get { return Vector3.Scale(_modelSpaceBox.Size, Scale); } }
        public Quaternion Rotation { get { return _rotation; } set { _rotation = value; } }
        public Vector3 Center { get { return _center; } set { _center = value; } }
        public Matrix4x4 TransformMatrix     
        { 
            get 
            {
                Vector3 translation = Center - Rotation * Vector3.Scale(_modelSpaceBox.Center, Scale);
                return Matrix4x4.TRS(translation, Rotation, Scale);
            } 
        }
        public bool AllowNegativeScale 
        { 
            get { return _allowNegativeScale; }  
            set
            {
                _allowNegativeScale = value;
                if (!_allowNegativeScale) Scale = Scale.GetVectorWithAbsComponents();
            }
        }
        public Vector3 Scale
        {
            get { return _allowNegativeScale ? _scale : _scale.GetVectorWithAbsComponents(); }
            set { _scale = value; }
        }
        #endregion

        #region Constructors
        public OrientedBox()
        {
        }

        public OrientedBox(Box modelSpaceBox)
        {
            _modelSpaceBox = modelSpaceBox;
            Center = _modelSpaceBox.Center;
        }

        public OrientedBox(Box modelSpaceBox, Quaternion rotation)
        {
            _modelSpaceBox = modelSpaceBox;
            Center = _modelSpaceBox.Center;
            Rotation = rotation;
        }

        public OrientedBox(Box modelSpaceBox, Transform transform)
        {
            _modelSpaceBox = modelSpaceBox;
            _center = _modelSpaceBox.Center;
            Transform(transform);
        }

        public OrientedBox(OrientedBox source)
        {
            _modelSpaceBox = source.ModelSpaceBox;
            _center = source.Center;
            _rotation = source.Rotation;
            _scale = source._scale;
            _allowNegativeScale = source.AllowNegativeScale;
        }
        #endregion

        #region Public Static Functions
        public static OrientedBox GetInvalid()
        {
            var orientedBox = new OrientedBox();
            orientedBox.MakeInvalid();

            return orientedBox;
        }
        #endregion

        #region Public Methods
        public void Transform(Transform transform)
        {
            Rotation = transform.rotation * Rotation;
            _scale = Vector3.Scale(transform.lossyScale, _scale);
            Center = transform.localToWorldMatrix.MultiplyPoint(Center);
        }

        public void Transform(Matrix4x4 transformMatrix)
        {
            Rotation = transformMatrix.GetRotation() * Rotation;
            _scale = Vector3.Scale(transformMatrix.GetXYZScale(), _scale);
            Center = transformMatrix.MultiplyPoint(Center);
        }

        public void Encapsulate(OrientedBox orientedBox)
        {
            List<Vector3> orientedBoxPoints = orientedBox.GetCenterAndCornerPoints();
            foreach(Vector3 point in orientedBoxPoints)
            {
                AddPoint(point);
            }
        }

        public Sphere3D GetEncapsulatingSphere()
        {
            return new Sphere3D(Center, ScaledExtents.magnitude);
        }

        public Box GetEncapsulatingBox()
        {
            List<Vector3> centerAndCornerPoints = GetCenterAndCornerPoints();
            return Box.FromPoints(centerAndCornerPoints);
        }

        public void AddPoint(Vector3 point)
        {
            _modelSpaceBox.AddPoint(GetPointInModelSpace(point));
        }


        private static Vector3[] A = new Vector3[3];
        private static Vector3[] B = new Vector3[3];
        private static float[,] R = new float[3, 3];
        private static float[,] absR = new float[3, 3];
        public bool Intersects(OrientedBox otherBox)
        {
            Vector3 thisScale = Scale;
            Vector3 otherScale = otherBox.Scale;

            // Negative scale causes problems
            Scale = thisScale.GetVectorWithAbsComponents();
            otherBox.Scale = otherScale.GetVectorWithAbsComponents();

            Matrix4x4 transformMatrix = TransformMatrix;
            A[0] = transformMatrix.GetAxis(0);
            A[1] = transformMatrix.GetAxis(1);
            A[2] = transformMatrix.GetAxis(2);
        
            Matrix4x4 otherTransformMatrix = otherBox.TransformMatrix;
            B[0] = otherTransformMatrix.GetAxis(0);
            B[1] = otherTransformMatrix.GetAxis(1);
            B[2] = otherTransformMatrix.GetAxis(2);

            // Note: We're using column major matrices.
            for(int row = 0; row < 3; ++row)
            {
                for(int column = 0; column < 3; ++column)
                {
                    R[row, column] = Vector3.Dot(A[row], B[column]);
                }
            }

            Vector3 scaledExtents = ScaledExtents;
            Vector3 AEx = new Vector3(scaledExtents.x, scaledExtents.y, scaledExtents.z);
            scaledExtents = otherBox.ScaledExtents;
            Vector3 BEx = new Vector3(scaledExtents.x, scaledExtents.y, scaledExtents.z);

            // Construct absolute rotation error matrix to account for cases when 2 local axes are parallel
            const float epsilon = 1e-4f;
            for(int row = 0; row < 3; ++row)
            {
                for(int column = 0; column < 3; ++column)
                {
                    absR[row, column] = Mathf.Abs(R[row, column]) + epsilon;
                }
            }

            Vector3 trVector = otherBox.Center - Center;
            Vector3 t = new Vector3(Vector3.Dot(trVector, A[0]), Vector3.Dot(trVector, A[1]), Vector3.Dot(trVector, A[2]));

            // Test extents projection on this box's local axes (A0, A1, A2)
            for(int axisIndex = 0; axisIndex < 3; ++axisIndex)
            {
                float bExtents = BEx[0] * absR[axisIndex, 0] + BEx[1] * absR[axisIndex, 1] + BEx[2] * absR[axisIndex, 2];
                if (Mathf.Abs(t[axisIndex]) > AEx[axisIndex] + bExtents) return false;
            }

            // Test extents projection on the other box's local axes (B0, B1, B2)
            for(int axisIndex = 0; axisIndex < 3; ++axisIndex)
            {
                float aExtents = AEx[0] * absR[0, axisIndex] + AEx[1] * absR[1, axisIndex] + AEx[2] * absR[2, axisIndex];
                if (Mathf.Abs(t[0] * R[0, axisIndex] +
                              t[1] * R[1, axisIndex] +
                              t[2] * R[2, axisIndex]) > aExtents + BEx[axisIndex]) return false;
            }

            // Test axis A0 x B0
            float ra = AEx[1] * absR[2, 0] + AEx[2] * absR[1, 0];
            float rb = BEx[1] * absR[0, 2] + BEx[2] * absR[0, 1];
            if (Mathf.Abs(t[2] * R[1, 0] - t[1] * R[2, 0]) > ra + rb) return false;

            // Test axis A0 x B1
            ra = AEx[1] * absR[2, 1] + AEx[2] * absR[1, 1];
            rb = BEx[0] * absR[0, 2] + BEx[2] * absR[0, 0];
            if (Mathf.Abs(t[2] * R[1, 1] - t[1] * R[2, 1]) > ra + rb) return false;

            // Test axis A0 x B2
            ra = AEx[1] * absR[2, 2] + AEx[2] * absR[1, 2];
            rb = BEx[0] * absR[0, 1] + BEx[1] * absR[0, 0];
            if (Mathf.Abs(t[2] * R[1, 2] - t[1] * R[2, 2]) > ra + rb) return false;

            // Test axis A1 x B0
            ra = AEx[0] * absR[2, 0] + AEx[2] * absR[0, 0];
            rb = BEx[1] * absR[1, 2] + BEx[2] * absR[1, 1];
            if (Mathf.Abs(t[0] * R[2, 0] - t[2] * R[0, 0]) > ra + rb) return false;

            // Test axis A1 x B1
            ra = AEx[0] * absR[2, 1] + AEx[2] * absR[0, 1];
            rb = BEx[0] * absR[1, 2] + BEx[2] * absR[1, 0];
            if (Mathf.Abs(t[0] * R[2, 1] - t[2] * R[0, 1]) > ra + rb) return false;

            // Test axis A1 x B2
            ra = AEx[0] * absR[2, 2] + AEx[2] * absR[0, 2];
            rb = BEx[0] * absR[1, 1] + BEx[1] * absR[1, 0];
            if (Mathf.Abs(t[0] * R[2, 2] - t[2] * R[0, 2]) > ra + rb) return false;

            // Test axis A2 x B0
            ra = AEx[0] * absR[1, 0] + AEx[1] * absR[0, 0];
            rb = BEx[1] * absR[2, 2] + BEx[2] * absR[2, 1];
            if (Math.Abs(t[1] * R[0, 0] - t[0] * R[1, 0]) > ra + rb) return false;

            // Test axis A2 x B1
            ra = AEx[0] * absR[1, 1] + AEx[1] * absR[0, 1];
            rb = BEx[0] * absR[2, 2] + BEx[2] * absR[2, 0];
            if (Math.Abs(t[1] * R[0, 1] - t[0] * R[1, 1]) > ra + rb) return false;

            // Test axis A2 x B2
            ra = AEx[0] * absR[1, 2] + AEx[1] * absR[0, 2];
            rb = BEx[0] * absR[2, 1] + BEx[1] * absR[2, 0];
            if (Math.Abs(t[1] * R[0, 2] - t[0] * R[1, 2]) > ra + rb) return false;

            Scale = thisScale;
            otherBox.Scale = otherScale;

            return true;
        }

        public bool AreAllBoxPointsOnOrInFrontOfAnyFacePlane(OrientedBox otherBox)
        {
            List<Vector3> otherBoxPoints = otherBox.GetCenterAndCornerPoints();
            List<Plane> allFacePlanes = GetBoxFacePlanes();
            foreach(Plane plane in allFacePlanes)
            {
                if (PlaneExtensions.AreAllPointsInFrontOrOnPlane(plane, otherBoxPoints)) return true;
            }

            return false;
        }

        public Vector3 GetClosestPointToPoint(Vector3 point)
        {
            Vector3 fromCenterToPoint = point - Center;
            Vector3 closestPoint = Center;
            Vector3 scaledExtents = ScaledExtents;

            Vector3[] localAxes = TransformMatrix.GetAllAxes();
            for(int axisIndex = 0; axisIndex < 3; ++axisIndex)
            {
                Vector3 localAxis = localAxes[axisIndex];
                float axisExtent = scaledExtents[axisIndex];

                float projection = Vector3.Dot(localAxis, fromCenterToPoint);
                if (projection > axisExtent) projection =axisExtent;
                else if (projection < -axisExtent) projection = -axisExtent;

                closestPoint += localAxis * projection;
            }
        
            return closestPoint;
        }

        public Vector3 GetPointInModelSpace(Vector3 point)
        {
            return TransformMatrix.inverse.MultiplyPoint(point);
        }

        public Vector3 GetDirectionInModelSpace(Vector3 direction)
        {
            return TransformMatrix.inverse.MultiplyVector(direction);
        }

        public Vector3 GetRotatedAndScaledSize()
        {
            return TransformMatrix.MultiplyVector(ModelSpaceSize);
        }

        public float GetRotatedAndScaledSizeAlongDirection(Vector3 direction)
        {
            direction.Normalize();
            return Mathf.Abs(Vector3.Dot(GetRotatedAndScaledSize(), direction));
        }

        public float GetSizeAlongDirection(Vector3 direction)
        {
            // ToDO: This is not actually correct (or is it? :D ). A better solution would probably be to
            //       cast a ray from the origin of the box towards the face whose normal is most aligned
            //       with the direction. The distance from the center of the box to the intersection points
            //       gives us half the size.
            direction.Normalize();
            return direction.GetAbsDot(GetRotatedAndScaledSize());
        }

        public List<Vector3> GetCenterAndCornerPoints()
        {
            List<Vector3> points = _modelSpaceBox.GetCenterAndCornerPoints();
            return Vector3Extensions.GetTransformedPoints(points, TransformMatrix);
        }

        public List<Vector3> GetCornerPoints()
        {
            List<Vector3> points = _modelSpaceBox.GetCornerPoints();
            return Vector3Extensions.GetTransformedPoints(points, TransformMatrix);
        }

        public List<Vector3> GetCornerPointsProjectedOnPlane(Plane plane)
        {
            return plane.ProjectAllPoints(GetCornerPoints());
        }

        public BoxFace GetBoxFaceClosestToPoint(Vector3 point)
        {
            Vector3 modelSpacePoint = GetPointInModelSpace(point);
            return _modelSpaceBox.GetBoxFaceClosestToPoint(modelSpacePoint);
        }

        public List<Plane> GetBoxFacePlanes()
        {
            List<Plane> facePlanes = new List<Plane>();
            Array boxFaces = Enum.GetValues(typeof(BoxFace));

            foreach(BoxFace boxFace in boxFaces)
            {
                facePlanes.Add(GetBoxFacePlane(boxFace));
            }

            return facePlanes;
        }

        public Plane GetBoxFacePlane(BoxFace boxFace)
        {
            Plane modelSpacePlane = _modelSpaceBox.GetBoxFacePlane(boxFace);
            Vector3 modelSpacePointOnPlane = _modelSpaceBox.GetBoxFaceCenter(boxFace);

            return modelSpacePlane.Transform(TransformMatrix, modelSpacePointOnPlane);
        }

        public Vector2 GetBoxFaceSizeAlongFaceLocalXZAxes(BoxFace boxFace)
        {
            return _modelSpaceBox.GetBoxFaceSizeAlongFaceLocalXZAxes(boxFace, _scale);
        }

        public Vector3 GetBoxFaceCenter(BoxFace boxFace)
        {
            Vector3 modelSpaceCenter = _modelSpaceBox.GetBoxFaceCenter(boxFace);
            return TransformMatrix.MultiplyPoint(modelSpaceCenter);
        }

        public BoxFace GetBoxFaceWhichFacesNormal(Vector3 normal)
        {
            List<Plane> facePlanes = GetBoxFacePlanes();
            int planeIndex;
            PlaneExtensions.GetPlaneWhichFacesNormal(facePlanes, normal, out planeIndex);
            
            return (BoxFace)planeIndex;
        }

        public BoxFace GetBoxFaceMostAlignedWithNormal(Vector3 normal)
        {
            List<Plane> facePlanes = GetBoxFacePlanes();
            int planeIndex;
            PlaneExtensions.GetPlaneMostAlignedWithNormal(facePlanes, normal, out planeIndex);

            return (BoxFace)planeIndex;
        }

        public List<Vector3> GetBoxFaceCenterAndCornerPoints(BoxFace boxFace)
        {
            List<Vector3> modelSpacePoints = _modelSpaceBox.GetBoxFaceCenterAndCornerPoints(boxFace);
            return Vector3Extensions.GetTransformedPoints(modelSpacePoints, TransformMatrix);
        }

        public List<Vector3> GetBoxFaceCornerPoints(BoxFace boxFace)
        {
            List<Vector3> modelSpacePoints = _modelSpaceBox.GetBoxFaceCornerPoints(boxFace);
            return Vector3Extensions.GetTransformedPoints(modelSpacePoints, TransformMatrix);
        }

        public bool Raycast(Ray ray, out OrientedBoxRayHit boxRayHit)
        {
            boxRayHit = null;
            float t;

            if(Raycast(ray, out t))
            {
                boxRayHit = new OrientedBoxRayHit(ray, t, this);
                return true;
            }
          
            return false;
        }

        public bool Raycast(Ray ray, out float t)
        {
            Matrix4x4 transformMatrix = TransformMatrix;
            Ray modelSpaceRay = ray.InverseTransform(transformMatrix);
     
            float modelSpaceT;
            if (_modelSpaceBox.Raycast(modelSpaceRay, out modelSpaceT))
            {
                // Note: The intersection offset (i.e. T value) we have calculated so far is expressed in the box model space.
                //       We have to calculate the intersection point in world space and use that to calculate the world space
                //       T value which we will store in the output parameter.
                Vector3 modelSpaceIntersectionPoint = modelSpaceRay.GetPoint(modelSpaceT);
                Vector3 worldIntersectionPoint = transformMatrix.MultiplyPoint(modelSpaceIntersectionPoint);

                t = (ray.origin - worldIntersectionPoint).magnitude;
                return true;
            }
            else
            {
                t = 0.0f;
                return false;
            }
        }

        public bool ContainsPoint(Vector3 point)
        {
            Vector3 modelSpacePoint = GetPointInModelSpace(point);
            return _modelSpaceBox.ContainsPoint(modelSpacePoint);
        }

        public void MakeInvalid()
        {
            _modelSpaceBox.MakeInvalid();
        }

        public bool IsValid()
        {
            return _modelSpaceBox.IsValid();
        }

        public bool IsInvalid()
        {
            return _modelSpaceBox.IsInvalid();
        }
        #endregion
    }
}
