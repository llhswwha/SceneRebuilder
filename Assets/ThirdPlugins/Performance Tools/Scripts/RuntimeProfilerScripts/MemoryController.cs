using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
public class MemoryController : MonoBehaviour
{
    public static MemoryController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<UnityEngine.Object> TestObjects = new List<UnityEngine.Object>();

    public List<Mesh> meshes = new List<Mesh>();


    string runtimeMemoryText;

    public bool IsUpdateMemory = true;

    public Text ResultText;

    public float UpdateInterval = 0.5f;

    private IEnumerator UpdateMemoryInfo()
    {
        IsUpdateMemory = true;
        while (IsUpdateMemory)
        {
            GetObjectsRuntimeMemory(false);
            yield return new WaitForSeconds(UpdateInterval);
        }
    }

    public string GetObjectsRuntimeMemory(bool isLog)
    {
        var sb = new StringBuilder(500);

        var sb2 = new StringBuilder(500);
        var objs = GameObject.FindObjectsOfType<Object>();
        long sum = 0;
        long meshMemory = 0;
        long transformMemory=0;
        int transformCount=0;
        long vertexCount = 0;
        long triangleCount = 0;
        int meshCount = 0;
        int goCount = 0;
        List<Mesh> meshList=new List<Mesh>();
        foreach (var obj in objs)
        {
            long size = Profiler.GetRuntimeMemorySizeLong(obj);
            sum += size;
            if (size > 500)
            {
                sb2.AppendLine($"({obj.GetType()}){obj.name}:{size}");
            }

            if(obj is MeshFilter)
            {
                MeshFilter meshFilter = obj as MeshFilter;
                //Mesh mesh = meshFilter.mesh;//�ᴴ��Teapot005 Instance

                Mesh mesh = meshFilter.sharedMesh;//�ᴴ��Teapot005 Instance
                //if(!meshList.Contains(mesh))
                {
                    meshList.Add(mesh);
                    long size2 = Profiler.GetRuntimeMemorySizeLong(mesh);
                    sum += size2;
                    sb2.AppendLine($"--({mesh.GetType()}){mesh.name}:{size2}");
                    vertexCount += mesh.vertexCount;
                    //triangleCount += mesh.triangles.Length;
                    meshCount++;
                    meshMemory += size2;
                }

            }
            else if(obj is GameObject)
            {
                goCount++;
            }
            else if(obj is Transform)
            {
                transformMemory+=size;
                transformCount++;
            }
            else
            {

            }
        }
        string allObjs = sb2.ToString();
        // if(isLog)
        //     Debug.Log(allObjs);

        sb.AppendLine($"{objs.Length}_All: {GetSizeString(sum)}");
        sb.AppendLine($"GameObjectCount: {goCount}");
        
        sb.AppendLine($"MeshCount: {meshCount}");
        sb.AppendLine($"MeshMemory: {GetSizeString(meshMemory)}");
        sb.AppendLine($"MeshVertexCount: {vertexCount}");
        sb.AppendLine($"MeshTriangleCount: {triangleCount}");
        sb.AppendLine($"TransformCount: {transformCount}");
        sb.AppendLine($"TransformMemory: {GetSizeString(transformMemory)}");

        foreach (var obj in TestObjects)
        {
            if (obj != null)
            {
                long size = Profiler.GetRuntimeMemorySizeLong(obj);
                sb.AppendLine($"{obj.GetType()}_{obj.name}: {GetSizeString(size)}");
            }
        }

        foreach (var mesh in meshes)
        {
            if(mesh==null)continue;
            long size = Profiler.GetRuntimeMemorySizeLong(mesh);
            sb.AppendLine($"{mesh.GetType()}_{mesh.name}: {GetSizeString(size)}");
        }
        runtimeMemoryText = sb.ToString();

        if (ResultText != null)
        {
            ResultText.text = runtimeMemoryText;
        }
        return runtimeMemoryText;
    }



    // Start is called before the first frame update
    void Start()
    {
        GetObjectsRuntimeMemory(true);
        //StartCoroutine(UpdateMemoryInfo());
    }

    private void OnDisable()
    {
        StopCoroutine("UpdateMemoryInfo");
        IsUpdateMemory = false;
    }

    // Update is called once per frame
    //void OnGUI()
    //{
    //    GUI.TextArea(new Rect(450, 10, 300, 500), runtimeMemoryText);
    //}

    private void FixedUpdate()
    {
        //GetObjectsRuntimeMemory(false);
        //Debug.LogError(Time.time);//0.02s
    }

    public static string GetSizeString(long size)
    {
        if (size < 1024)
        {
            return $"{size} B";
        }
        else if (size < 1048576)//1024*1024
        {
            return $"{size / (1024f):F2} KB";
        }
        else
        {
            return $"{size / (1048576f):F2} MB";
        }
    }
}
