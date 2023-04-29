using UnityEngine;
using UnityEngine.ProBuilder;

namespace Battlehub.ProBuilderIntegration
{
    public enum PBShapeType
    {
        Cube = 0,
        Stair = 1,
        CurvedStair = 2,
        Prism = 3,
        Cylinder = 4,
        Plane = 5,
        Door = 6,
        Pipe = 7,
        Cone = 8,
        Sprite = 9,
        Arch = 10,
        Sphere = 11,
        Torus = 12
    }

    public static class PBShapeGenerator 
    {
        public static GameObject CreateShape(PBShapeType shapeType)
        {
            Debug.Log("PBShapeGenerator.CreateShape start "+shapeType);

            Material defaultMaterial=null;
            Debug.Log("defaultMaterial0 : "+defaultMaterial);
            //UnityEngine.ProBuilder.BuiltinMaterials.DefaultMaterial
            defaultMaterial = PBBuiltinMaterials.DefaultMaterial;
            Debug.Log("defaultMaterial1 : "+defaultMaterial);

           GameObject shape=null;
            try
            {
                var sp=ShapeGenerator.CreateShape((ShapeType)shapeType, PivotLocation.Center);
                Debug.Log("sp : "+sp);
                shape = sp.gameObject;
                Renderer renderer = shape.GetComponent<Renderer>();

                Debug.Log("renderer : "+renderer);
                renderer.sharedMaterial = defaultMaterial;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("PBShapeGenerator.CreateShape : "+ex);
                GameObject.Destroy(shape);
                shape=null;
            }
            Debug.Log("PBShapeGenerator.CreateShape end "+shape);
            return shape;
            
        }
    }
}


