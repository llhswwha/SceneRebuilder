using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

//[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
public class MeshCombiner : MonoBehaviour
{
    public enum MeshCombineMode
    {
        All,//sourceRoot合并为一个模型
        Self,//sourceRoot的子物体分别合并为一个模型
    }
    public MeshCombineMode CombineMode;//0:sourceRoot合并为一个模型，1:sourceRoot的子物体分别合并为一个模型

    public GameObject sourceRoot;

    public List<GameObject> sourceList=new List<GameObject>();

    public List<GameObject> resultList=new List<GameObject>();

    public bool Auto=false;

    public bool IsCombine=false;

    public MeshCombinerSetting Setting;

    // public bool IsDestroySource=false;
    // public bool IsCoroutine=false;

    // public int WaitCount=10000;


    // Start is called before the first frame update
    void Start()
    {

        GetSetting();

        if(Auto)
        {
            CombineByMaterial();
        }
    }

    public MeshCombinerSetting GetSetting()
    {
        if (Setting == null)
        {
            Setting = this.GetComponent<MeshCombinerSetting>();
        }
        if(Setting==null){
            Setting=MeshCombinerSetting.Instance;
        }
        if(Setting==null){
            Setting=this.gameObject.AddComponent<MeshCombinerSetting>();
        }
        Setting.SetSetting();
        return Setting;
    }

    // public  CombinedMesh combinedMesh;


    [ContextMenu("Combine")]

    public void Combine(){
        GetSetting();
        sourceList.Clear();
        CombineEx(2);
    }

    public bool CombineBySub=false;

    public List<MeshCombiner> SubCombiners=new List<MeshCombiner>();

    private void CombineEx(int mode)
    {
        GetSetting();

        Debug.LogError("Start CombineEx mode:"+mode+"|"+gameObject);
        resultList=new List<GameObject>();
        string sourceName="";
        if((sourceList.Count==0 || CombineMode == MeshCombineMode.All)&&sourceRoot!=null){
            sourceList=new List<GameObject>();
            if(CombineMode==MeshCombineMode.All){
                sourceList.Add(sourceRoot);
                sourceName+=sourceRoot.name;
            }
            else{
                for(int i=0;i<sourceRoot.transform.childCount;i++){
                    GameObject child=sourceRoot.transform.GetChild(i).gameObject;
                    sourceList.Add(child);
                    sourceName+=child.name+";";
                }
            }
        }
        Debug.LogError("sourceList:"+sourceName);
        // foreach(var source in sourceList){
        //     if(source==null)continue;

        //     if(CombineBySub){
        //         MeshCombiner subCombiner=gameObject.AddComponent<MeshCombiner>();
        //         subCombiner.Auto=true;
        //         //subCombiner.IsCoroutine=this.IsCoroutine;
        //         subCombiner.CombineBySub=false;
        //         //subCombiner.WaitCount=this.WaitCount;
        //         //subCombiner.IsDestroySource=false;
        //         subCombiner.sourceList.Add(source);
        //         SubCombiners.Add(subCombiner);
        //     }
        //     else{
        //         if(Setting.IsCoroutine){
        //             StartCoroutine(MeshCombineHelper.CombineEx_Coroutine(source,Setting.IsDestroySource,mode,Setting.WaitCount));
        //         }
        //         else{
        //             GameObject target=MeshCombineHelper.CombineEx(source,mode);
        //             resultList.Add(target);
        //             Debug.Log("Combine:"+source+"->"+target);
        //             if(Setting.IsDestroySource){
        //                 GameObject.Destroy(source);
        //             }
        //         }

        //     }
        // }

        if(Setting.IsCoroutine){
            StartCoroutine(CombineEx_Coroutine(mode));
        }
        else{
            for (int i = 0; i < sourceList.Count; i++)
            {
                GameObject source = sourceList[i];
                Debug.LogWarning(string.Format("CombineEx {0} ({1}/{2})",source,i+1,sourceList.Count));
                if(source==null)continue;

                if(CombineBySub){
                    MeshCombiner subCombiner=gameObject.AddComponent<MeshCombiner>();
                    subCombiner.Auto=true;
                    //subCombiner.IsCoroutine=this.IsCoroutine;
                    subCombiner.CombineBySub=false;
                    //subCombiner.WaitCount=this.WaitCount;
                    //subCombiner.IsDestroySource=false;
                    subCombiner.sourceList.Add(source);
                    SubCombiners.Add(subCombiner);
                }
                else{
                    // if(Setting.IsCoroutine){
                    //     StartCoroutine(MeshCombineHelper.CombineEx_Coroutine(source,Setting.IsDestroySource,mode,Setting.WaitCount));
                    // }
                    // else
                    {
                        GameObject target=MeshCombineHelper.CombineEx(source,mode);
                        resultList.Add(target);
                        Debug.Log("Combine:"+source+"->"+target);
                        if(Setting.IsDestroySource){
                            GameObject.Destroy(source);
                        }
                    }

                }
            }
        }
    }
    private IEnumerator CombineEx_Coroutine(int mode)
    {
        DateTime start=DateTime.Now;
        for (int i = 0; i < sourceList.Count; i++)
        {
            GameObject source = (GameObject)sourceList[i];
             Debug.LogError(string.Format("CombineEx {0} ({1}/{2})",source,i+1,sourceList.Count));
            if (source!=null)
            {
                yield return MeshCombineHelper.CombineEx_Coroutine(source,Setting.IsDestroySource,Setting.WaitCount,mode);
            }
        }
        Setting.WriteLog("完成合并 用时:"+(DateTime.Now-start));
        yield return null;
    }


    [ContextMenu("CombineByMaterial")]

    public void CombineByMaterial(){
        GetSetting();
        sourceList.Clear();
        CombineEx(1);
    }


    [ContextMenu("CombineEx")]

    public void CombineEx(){
        GetSetting();
        sourceList.Clear();
        CombineEx(0);
    }

    public bool AutoAdd;

    public List<GameObject> mergedObjs=new List<GameObject>();

    public GameObject mergedObjRoot=null;

    public GameObject mergedObj=null;

    // Update is called once per frame
    void Update()
    {
        // if(Input.GetMouseButtonUp(0)){
        //     Debug.Log("Click");
        //     Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //     if(Physics.Raycast(ray,out hit))
        //     {
        //         GameObject go=hit.collider.gameObject;
        //         Debug.Log("Hit:"+go);
        //         if(AutoAdd){
        //             if(!mergedObjs.Contains(go)){
        //                 if(mergedObjRoot==null){
        //                     mergedObjRoot=new GameObject();
        //                     mergedObjRoot.name="mergedObjRoot";
        //                 }

        //                 mergedObjs.Add(go);
        //                 go.transform.SetParent(mergedObjRoot.transform);

        //                 mergedObj=MeshCombineHelper.SimpleCombine(mergedObjRoot,mergedObj);
        //                 mergedObj.transform.SetParent(this.transform);
        //             }
        //         }
        //         if(AutoRemove){
        //             MeshCombineHelper.RemveGo(go);
        //         }
        //     }
        // }
    }

    
    public bool AutoRemove;

}
