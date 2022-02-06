using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public bool IsDebug;

    public bool CanOpen=true;

    public GameObject Terrains;

    public GameObject Reiver;

    public void HideTerrains(){
        if(Terrains==null){
            Terrains=GameObject.Find("Terrains");
        }
        if(Terrains==null){
            Terrains=GameObject.Find("Terrains(Clone)");
        }
        if(Terrains!=null){
            if(Terrains.activeSelf){
                Terrains.SetActive(false);
            }
            else{
                Terrains.SetActive(true);
            }
        }
        
    }

    public void HideReiver(){
        if(Reiver==null){
            Reiver=GameObject.Find("Reiver");
        }
        if(Reiver==null){
            Reiver=GameObject.Find("Reiver(Clone)");
        }
        if(Reiver!=null){
            if(Reiver.activeSelf){
                Reiver.SetActive(false);
            }
            else{
                Reiver.SetActive(true);
            }
        }
    }

    //void Awake(){
    //    SystemSettingHelper.AfterSetttingLoaded+=OnAfterSetttingLoaded;
    //}
    
    //void OnDestroyed(){
    //    SystemSettingHelper.AfterSetttingLoaded-=OnAfterSetttingLoaded;
    //}

    //void OnAfterSetttingLoaded()
    //{
    //    Log.Error("DebugPanel.OnAfterSetttingLoaded");
    //    // IsDebug=SystemSettingHelper.systemSetting.IsDebug;
    //    IsDebug=SystemSettingHelper.IsDebug();
    //    if(IsDebug==false){
    //        SetChildrenActive(false);
    //    }
    //}

    // void Start(){
        
    // }

    private void SetChildrenActive(bool isA){
        for(int i=0;i<transform.childCount;i++){
            transform.GetChild(i).gameObject.SetActive(isA);
        }
    }

    [ContextMenu("Close")]
    public void Close(){
        SetChildrenActive(false);
    }

    [ContextMenu("Open")]
    public void Open(){
        if(CanOpen==false)return;
        SetChildrenActive(true);
    }

    public void Update(){
        // if(Input.GetKeyDown(KeyCode.LeftControl))
        // {
        //     Debug.Log("DebugPanel GetKeyDown Ctr");
        // }
        //  if(Input.GetKey(KeyCode.LeftControl))
        // {
        //     Debug.Log("DebugPanel GetKey Ctr");
        // }
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("DebugPanel Ctr+D");
            Open();
        }
    }
}
