using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshFilterInfo : MonoBehaviour
{
    public MeshFilter meshFilter;
    public Mesh mesh;

    public int[] Triangles;

    public Vector3[] Vertices;
    public Vector2[] UV;


    // Start is called before the first frame update
    void Start()
    {
        GetInfo();
        //ShowVertex();
    }

    [ContextMenu("GetInfo")]
    public void GetInfo(){
        meshFilter=gameObject.GetComponent<MeshFilter>();
        mesh=meshFilter.sharedMesh;
        if(mesh!=null){
            Vertices = mesh.vertices;
            UV = mesh.uv;
            Triangles = mesh.triangles;
        }
        else{
            Debug.LogError("MeshInfo.GetInfo mesh==null!:"+gameObject);
        }
    }

    public float vertScale=0.03f;

    public bool IsAbsolute=false;

    public List<GameObject> vertObjs=new List<GameObject>();

    [ContextMenu("ShowVertex")]
    public void ShowVertex()
    {
        vertObjs.Clear();
        for(int i=0;i<Vertices.Length;i++)
        {
            Vector3 vector=Vertices[i];
            GameObject newGo=GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newGo.name=string.Format("[{0}]{1}",i,vector);
            newGo.transform.transform.localScale=new Vector3(vertScale,vertScale,vertScale);
            if(IsAbsolute){
                newGo.transform.localPosition=vector;
            }
            else{
                newGo.transform.position=transform.position+vector;
            }
            newGo.transform.SetParent(this.transform);
            vertObjs.Add(newGo);
        }
    }

    [ContextMenu("ClearVertexObjects")]
    public void ClearVertexObjects()
    {
        foreach(var go in vertObjs){
            GameObject.DestroyImmediate(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
