using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDisInfo : MonoBehaviour,IComparable<ModelDisInfo>
{
    public RendererInfo info;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("ShowTestInfo")]
    public void ShowTestInfo()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = this.transform.position;
        cube.transform.parent = this.transform;
        SetMaterial(cube);
    }

    private void SetMaterial(GameObject obj)
    {
        ModelManagerSettings settings = GameObject.FindObjectOfType<ModelManagerSettings>();
        if (settings != null && settings.TransparentMaterial != null)
        {
            obj.GetComponent<MeshRenderer>().material = settings.TransparentMaterial;
        }
    }

    private void CreateCubeInternal(float p){
         GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = this.transform.position;
        cube.transform.localScale=info.renderer.bounds.size*p;
        SetMaterial(cube);
    }

    public float TestPower=10;

    [ContextMenu("CreateCube")]
    public void CreateCube(){
        CreateCubeInternal(1f);
    }

     [ContextMenu("CreateCubeEx")]
    public void CreateCubeEx(){
         CreateCubeInternal(TestPower);
    }

    private GameObject CreateSphereInternal(float radius){
        Vector3 scale=new Vector3(radius,radius,radius);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cube.transform.position = this.transform.position;
        cube.transform.localScale=scale;
        SetMaterial(cube);
        return cube;
    }


    [ContextMenu("CreateSphereFull")]
    public GameObject CreateSphereFull(){
       return CreateSphereInternal(info.fullDiameter);
    }

    [ContextMenu("CreateSphere")]
    public GameObject CreateSphere(){
       return CreateSphereInternal(info.diameter);
    }

    [ContextMenu("CreateSphereEx")]
    public GameObject CreateSphereEx(){
        return CreateSphereInternal(info.diameter*TestPower);
    }

    public int CompareTo(ModelDisInfo other)
    {
        return other.info.CompareTo(this.info);
    }
}
