using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LODManagerUI : MonoBehaviour
{
    public LODManager LODManager;

    public Text txtLog;

    public Toggle toggleIsUpdate;

    // Start is called before the first frame update
    void Start()
    {
        LODManager=GameObject.FindObjectOfType<LODManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(toggleIsUpdate.isOn)
            GetLODInfo();
    }

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

    [ContextMenu("GetLODInfo")]
    public void GetLODInfo()
    {
        txtLog.text=LODManager.GetLODInfo();
    }
}
