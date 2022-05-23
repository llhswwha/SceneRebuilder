using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MeshCombinerSetting : SingletonBehaviour<MeshCombinerSetting>
{
    //private static MeshCombinerSetting _instance;
    //public static MeshCombinerSetting Instance
    //{
    //    get{
    //        if(_instance==null){
    //            _instance=GameObject.FindObjectOfType<MeshCombinerSetting>();
    //        }
    //        return _instance;
    //    }
    //}

    public float MaxVertex=10;

    public UnityEngine.Rendering.IndexFormat indexFormat=UnityEngine.Rendering.IndexFormat.UInt16;

    public bool NoLimit=false;

    public Text TextLog=null;

    
    public bool IsDestroySource=false;
    public bool IsCoroutine=false;

    public int WaitCount=10000;

    void Awake(){
        //Instance=this;

        SetSetting();
    }

    [ContextMenu("SetSetting")]
    public void SetSetting()
    {
        if (NoLimit)
        {
            this.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            //MaxVertex = int.MaxValue;

            CombinedMesh.indexFormat = this.indexFormat;
            CombinedMesh.MaxVertex = int.MaxValue;
        }
        else
        {
            if (this.MaxVertex > 6.5535f)
            {
                this.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }
        }
        CombinedMesh.indexFormat = this.indexFormat;
        CombinedMesh.MaxVertex = (int)(this.MaxVertex*10000);
        if (CombinedMesh.MaxVertex < 0)
        {
            CombinedMesh.MaxVertex= int.MaxValue;
        }
        //Debug.Log($"MeshCombinerSetting.SetSetting MaxVertex:{CombinedMesh.MaxVertex} indexFormat:{CombinedMesh.indexFormat}");
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
