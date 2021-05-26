using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MeshCombinerSetting : MonoBehaviour
{
    public static MeshCombinerSetting Instance;

    public int MaxVertex=65535;

    public UnityEngine.Rendering.IndexFormat indexFormat=UnityEngine.Rendering.IndexFormat.UInt16;

    public bool NoLimit=false;

    public Text TextLog=null;

    
    public bool IsDestroySource=false;
    public bool IsCoroutine=false;

    public int WaitCount=10000;

    void Awake(){
        Instance=this;

        if(NoLimit){
            indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;
            MaxVertex=int.MaxValue;
        }
        else{
            if(MaxVertex>65535){
                indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;
            }
        }
        CombinedMesh.indexFormat=indexFormat;
        CombinedMesh.MaxVertex=MaxVertex;
    }

    public void WriteLog(string log){
        if(TextLog!=null){
            TextLog.text=log+"\n"+TextLog.text;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
