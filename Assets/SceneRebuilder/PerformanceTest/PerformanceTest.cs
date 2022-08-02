#if UNITY_EDITOR
using CodeStage.AdvancedFPSCounter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PerformanceTest : MonoBehaviour
{
    public List<string> SettingList = new List<string>();

    [ContextMenu("AddSetting")]
    public void AddSetting()
    {
        foreach(var setting in SettingList)
        {
            string clone = setting + "(Clone)";
            GameObject go1 = GameObject.Find(clone);
            if (go1 == null)
            {
                //Debug.LogError($"Not Found1 name:{clone}");
            }
            if (go1 != null)
            {
                if (!TargetList.Contains(go1))
                {
                    TargetList.Add(go1);
                    continue;
                }
            }

            GameObject go2 = GameObject.Find(setting);
            if (go2 == null)
            {
                //Debug.LogError($"Not Found2 name:{setting}");
                //continue;
            }
            if (go2 != null)
            {
                if (!TargetList.Contains(go2))
                {
                    TargetList.Add(go2);
                    continue;
                }
            }
            if(go1==null&& go2 == null)
            {
                Debug.LogError($"Not Found name:{setting}");
            }
        }
        Debug.Log($"AddSetting SettingList:{SettingList.Count} TargetList:{TargetList.Count}");
    }

    public List<GameObject> TargetList = new List<GameObject>();

    [ContextMenu("AddSelection")]
    public void AddSelection()
    {
        var gos = Selection.gameObjects;
        foreach(var go in gos)
        {
            if (!TargetList.Contains(go))
            {
                TargetList.Add(go);
            }
        }
        Debug.Log($"AddSelection gos:{gos.Length} TargetList:{TargetList.Count}");
    }

    [ContextMenu("AddChildren")]
    public void AddChildren()
    {
        var go = Selection.activeGameObject;
        for (int i = 0; i < go.transform.childCount; i++)
        {
            var child = go.transform.GetChild(i).gameObject;
            if (!TargetList.Contains(child))
            {
                TargetList.Add(child);
            }
        }
        Debug.Log($"AddSelection go:{go} childCount:{go.transform.childCount} TargetList:{TargetList.Count}");
    }

    public int HideId = 0;

    public PerformanceData Data = new PerformanceData();

    public List<PerformanceData> ReportData = new List<PerformanceData>();

    public float FPSWaitTime = 60;//60s

    public float StartTime;

    public float TimeSpane;

    public float AllTimeSpane;

    public bool IsAutoTest = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        Data.Update();

        if (IsTesting)
        {
            if (StartTime != 0)
            {
                TimeSpane = Time.time - StartTime;
                if (TimeSpane > FPSWaitTime)
                {
                    AddToReport();
                }
            }
        }

        if (IsAutoTest)
        {
            AllTimeSpane= Time.time - AutoStartTime;
            if (IsTesting == false)
            {
                TestHideOne();
            }
        }
    }

    //[ContextMenu("AddToReport")]
    private void AddToReport()
    {
        Debug.Log($"AddToReport Data:{Data}");
        ReportData.Add(Data);
        Data = new PerformanceData();
        IsTesting = false;
    }

    public bool IsTesting = false;

    public GameObject CurrentTarget = null;

    [ContextMenu("TestHideOne")]
    public void TestHideOne()
    {
        TargetList.RemoveAll(i => i == null);

        if (HideId < 0 || HideId > TargetList.Count - 1)
        {
            if (IsAutoTest)
            {
                IsAutoTest = false;
                GetResult();
            }

            IsTesting = false;
            return;
        }

        CurrentTarget = TargetList[HideId];
        Data.Name = CurrentTarget.name;
        TargetList[HideId].SetActive(false);
        StartTime = Time.time;
        IsTesting = true;
        HideId++;

        Debug.Log($"TestHideOne[{HideId}] CurrentTarget:{CurrentTarget}");
    }

    [ContextMenu("GetResult")]
    private void GetResult()
    {
        string result = "";
        foreach (var item in ReportData)
        {
            result += item + "\n";
        }
        Debug.LogError($"TestAutoHide count:{TargetList.Count} Time:{AllTimeSpane} result:\n{result}");
    }

    public float AutoStartTime;

    [ContextMenu("TestAutoHide")]
    public void TestAutoHide()
    {
        TargetList.RemoveAll(i => i == null);
        IsAutoTest = true;
        AutoStartTime= Time.time;
    }

    [ContextMenu("ShowAll")]
    public void ShowAll()
    {
        foreach(var target in TargetList)
        {
            if (target == null) continue;
            target.SetActive(true);
        }
    }

    [ContextMenu("HideAll")]
    public void HideAll()
    {
        foreach (var target in TargetList)
        {
            if (target == null) continue;
            target.SetActive(false);
        }
    }

    [Serializable]
    public class PerformanceData
    {
        public PerformanceData()
        {

        }

        public PerformanceData(string n)
        {
            this.Name = n;
        }

        public string Name = "";

        public float FPS;

        public float batches;

        public int setPassCalls;

        public int triangles;

        //public int vertices;

        public int shadowCasters;

        public int drawCalls;

        public void Update()
        {
            AFPSCounter counter = AFPSCounter.Instance;
            if (counter == null)
            {
                counter = GameObject.FindObjectOfType<AFPSCounter>(true);
                if (counter != null)
                {
                    counter.gameObject.SetActive(true);
                }
            }
            if (counter != null)
            {
               
                FPS = counter.fpsCounter.LastAverageValue;
            }
            
            batches = UnityEditor.UnityStats.batches;
            setPassCalls = UnityEditor.UnityStats.setPassCalls;
            triangles = UnityEditor.UnityStats.triangles;
            shadowCasters = UnityEditor.UnityStats.shadowCasters;
            drawCalls = UnityEditor.UnityStats.drawCalls;
        }

        public override string ToString()
        {
            return $"{Name}\t{batches}\t0\t{triangles}\t0\t{setPassCalls}\t0\t{FPS}\t0\t{shadowCasters}\t0\t{drawCalls}";
        }
    }
}
#endif