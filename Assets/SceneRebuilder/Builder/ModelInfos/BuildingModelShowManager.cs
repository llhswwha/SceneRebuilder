using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingModelShowManager : MonoBehaviour
{
    public List<Camera> cameras = new List<Camera>();
    public List<BuildingModelInfo> Buildings = new List<BuildingModelInfo>();

    public List<Bounds> BuildingBounds = new List<Bounds>();
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        DateTime start = DateTime.Now;

        cameras.Clear();
        cameras.AddRange(GameObject.FindObjectsOfType<Camera>());

        Buildings.Clear();
        Buildings.AddRange(GameObject.FindObjectsOfType<BuildingModelInfo>());

        BuildingBounds.Clear();
        foreach (BuildingModelInfo b in Buildings)
        {
            Bounds bounds = ColliderHelper.CaculateBounds(b.OutPart0);
            BuildingBounds.Add(bounds);
        }

        Debug.LogError($"BuildingModelShowManager.Init Count:{Buildings.Count} \t{(DateTime.Now - start).ToString()}");
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < BuildingBounds.Count; i++)
        {
            Bounds b = BuildingBounds[i];

            for(int j=0;j<cameras.Count;j++)
            {

            }
            //var p=b.ClosestPoint()
        }
    }
}
