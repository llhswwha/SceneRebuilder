using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LODManagerUI : MonoBehaviour
{
    public LODManager LODManager;

    public Text txtLog;

    public Toggle toggleIsUpdate;

    public float UpdateInternal = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        LODManager=GameObject.FindObjectOfType<LODManager>();
        StartCoroutine(UpdateCoroutine());
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (toggleIsUpdate.isOn)
                GetLODInfo();
            yield return new WaitForSeconds(UpdateInternal);
        }
        //yield return null;
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if(toggleIsUpdate.isOn)
    //        GetLODInfo();
    //}

    [ContextMenu("SetLODMatColor")]
    public void SetLODMatColor()
    {
        LODManager.SetLODMatColor();
    }

    [ContextMenu("DisableLOD")]
    public void DisableLOD()
    {
        LODManager.DisableLOD();
    }

    [ContextMenu("EnableLOD")]
    public void EnableLOD()
    {
        LODManager.EnableLOD();
    }

    [ContextMenu("ActiveLOD")]
    public void ActiveLOD()
    {
        LODManager.SetLODActive(true);
    }

    [ContextMenu("InactiveLOD")]
    public void InactiveLOD()
    {
        LODManager.SetLODActive(false);
    }

    [ContextMenu("GetLODInfo")]
    public void GetLODInfo()
    {
        txtLog.text=LODManager.GetLODInfo();
        Debug.Log("LODManagerUI.GetLODInfo");
    }
}
