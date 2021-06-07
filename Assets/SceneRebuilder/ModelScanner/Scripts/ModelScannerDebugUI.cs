using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelScannerDebugUI : MonoBehaviour
{
    public Dropdown dropdownLevel;

    public ModelScanner modelScanner;

    // Start is called before the first frame update
    void Start()
    {
        modelScanner = ModelScanner.Instance;
        dropdownLevel.value = modelScanner.ScanLevel - 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevel()
    {
        int level = (dropdownLevel.value + 1);
        Debug.Log("SetLevel:" + level);
        modelScanner.InitScanLevel(level);
    }

    public void StopScan()
    {
        Debug.Log("StopScan");
        modelScanner.StopScan();
    }

    public void StartScan()
    {
        Debug.Log("StartScan");
        modelScanner.StartScan();
    }
}
