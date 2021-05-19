using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Profiling;
using UnityEngine.UI;
public class ProfilerController : MonoBehaviour
{
    /************************************************************************************************************
    * Source: https://docs.unity3d.com/2020.2/Documentation/ScriptReference/Unity.Profiling.ProfilerRecorder.html
    *************************************************************************************************************/
    
    //public static ProfilerMarker UpdatePlayerProfilerMarker = new ProfilerMarker("Player.Update");

    string statsText;
    //ProfilerRecorder mainThreadTimeRecorder;
    //ProfilerRecorder drawCallsCountRecorder;

    public Text ResultText;

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;
        var samples = new List<ProfilerRecorderSample>(samplesCount);
        recorder.CopyTo(samples);
        for (var i = 0; i < samples.Count; ++i)
            r += samples[i].Value;
        r /= samplesCount;
        if (r < 0)
        {
            r = 0;
        }
        return r;
    }

    private Dictionary<string,StatInfo> statDict;

    private Dictionary<StatInfo,ProfilerRecorder> Recorders=new Dictionary<StatInfo,ProfilerRecorder>();

    private void AddProfiler(string statName){
        if(statDict==null){
            statDict=GetAvailableProfilerStats.EnumerateProfilerStats();
        }
        if(statDict.ContainsKey(statName))
        {
            StatInfo info=statDict[statName];
            //Debug.Log("AddProfiler:"+statName)
            ProfilerRecorder recorder = ProfilerRecorder.StartNew(info.Cat, statName);
            Recorders.Add(info,recorder);
        }
        else{
            Debug.LogError("No StatName:"+statName);
        }
    }

    private void AddMemoryProfiler(string statName){
        AddProfiler(statName,ProfilerCategory.Memory,ProfilerMarkerDataUnit.Bytes);
    }

    private void AddProfiler(string statName,ProfilerCategory pc,ProfilerMarkerDataUnit unit){
        if(statDict==null){
            statDict=GetAvailableProfilerStats.EnumerateProfilerStats();
        }
        try
        {
            if (statDict.ContainsKey(statName))
            {
                StatInfo info = statDict[statName];
                //Debug.Log("AddProfiler:"+statName)
                ProfilerRecorder recorder = ProfilerRecorder.StartNew(info.Cat, statName);
                Recorders.Add(info, recorder);
            }
            else
            {
                // StatInfo info=new StatInfo
                // {
                //     Name="*"+statName,
                //     Cat=pc,
                //     Unit=unit
                // };
                // ProfilerRecorder recorder = ProfilerRecorder.StartNew(info.Cat, statName);
                // Recorders.Add(info,recorder);
                Debug.LogError("No StatName:" + statName);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError($"AddProfiler Exception {statName}: {ex.ToString()}");
        }

    }

    void OnEnable()
    {
        //systemMemoryRecorder = ProfilerRecorder.StartNew(new ProfilerCategory("Memory"), "System Used Memory");
        // gcReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        // gcUsedMemoryRecorder= ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Used Memory");
        // totalReservedMemoryRecorder=ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory");
        // totalUsedMemoryRecorder=ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
        //mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        //Recorders.Add(info,mainThreadTimeRecorder);
        //drawCallsCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        InitProfilers();

        StartCoroutine(UpdateInfo());
    }

    private void InitProfilers()
    {
        statDict = GetAvailableProfilerStats.EnumerateProfilerStats();
        
        AddProfiler("Main Thread", ProfilerCategory.Render, ProfilerMarkerDataUnit.TimeNanoseconds);

        if (IsOnlyFPS == false)
        {
            AddProfiler("Render Thread", ProfilerCategory.Internal, ProfilerMarkerDataUnit.TimeNanoseconds);
            AddProfiler("Gfx.WaitForGfxCommandsFromMainThread", ProfilerCategory.Internal, ProfilerMarkerDataUnit.TimeNanoseconds);

            AddProfiler("Camera.Render", ProfilerCategory.Render, ProfilerMarkerDataUnit.TimeNanoseconds);
            AddProfiler("RenderLoop.Draw", ProfilerCategory.Render, ProfilerMarkerDataUnit.TimeNanoseconds);

            AddProfiler("Physics.Raycast", ProfilerCategory.Physics, ProfilerMarkerDataUnit.TimeNanoseconds);
            AddProfiler("Physics.RaycastAll", ProfilerCategory.Physics, ProfilerMarkerDataUnit.TimeNanoseconds);
            AddProfiler("Physics2D.Raycast", ProfilerCategory.Physics, ProfilerMarkerDataUnit.TimeNanoseconds);

            AddMemoryProfiler("System Used Memory");
            AddMemoryProfiler("Total Reserved Memory");
            AddMemoryProfiler("Total Used Memory");
            AddMemoryProfiler("GC Reserved Memory");
            AddMemoryProfiler("GC Used Memory");
            AddMemoryProfiler("Gfx Used Memory");
            AddProfiler("Texture Count", ProfilerCategory.Memory, ProfilerMarkerDataUnit.Count);
            AddMemoryProfiler("Texture Memory");
            AddProfiler("Material Count", ProfilerCategory.Memory, ProfilerMarkerDataUnit.Count);
            AddMemoryProfiler("Material Memory");
            AddProfiler("Mesh Count", ProfilerCategory.Memory, ProfilerMarkerDataUnit.Count);
            AddMemoryProfiler("Mesh Memory");
            AddMemoryProfiler("Profiler Used Memory");
            AddProfiler("Game Object Count", ProfilerCategory.Memory, ProfilerMarkerDataUnit.Count);
            AddProfiler("Object Count", ProfilerCategory.Memory, ProfilerMarkerDataUnit.Count);
            AddProfiler("Asset Count", ProfilerCategory.Memory, ProfilerMarkerDataUnit.Count);
            AddProfiler("Scene Object Count", ProfilerCategory.Memory, ProfilerMarkerDataUnit.Count);

            AddProfiler("Draw Calls Count", ProfilerCategory.Render, ProfilerMarkerDataUnit.Count);
            AddProfiler("Batches Count", ProfilerCategory.Render, ProfilerMarkerDataUnit.Count);
            AddProfiler("SetPass Calls Count", ProfilerCategory.Render, ProfilerMarkerDataUnit.Count);
            AddProfiler("Triangles Count", ProfilerCategory.Render, ProfilerMarkerDataUnit.Count);
            AddProfiler("Vertices Count", ProfilerCategory.Render, ProfilerMarkerDataUnit.Count);
            AddProfiler("Used Buffers Count", ProfilerCategory.Render, ProfilerMarkerDataUnit.Count);
            AddProfiler("Used Buffers Bytes", ProfilerCategory.Render, ProfilerMarkerDataUnit.Bytes);
        }
    }

    void OnDisable()
    {
        //mainThreadTimeRecorder.Dispose();
        //drawCallsCountRecorder.Dispose();

        foreach(StatInfo info in Recorders.Keys)
        {
            ProfilerRecorder recorder=Recorders[info];
            recorder.Dispose();
        }
        Recorders.Clear();

        StopCoroutine("UpdateInfo");
    }

    public float UpdateInterval = 0.5f;

    IEnumerator UpdateInfo()
    {
        while (true)
        {
            UpdateStatsText();
            yield return new WaitForSeconds(UpdateInterval);
        }
    }

    //void FixedUpdate()
    //{
    //    UpdateStatsText();
    //}

    public bool IsOnlyFPS = false;

    private void UpdateStatsText()
    {
        var sb = new StringBuilder(500);
        foreach (StatInfo info in Recorders.Keys)
        {
            ProfilerRecorder recorder = Recorders[info];


            if (info.Name == "Main Thread")
            {
                double t = GetRecorderFrameAverage(recorder) * (1e-6f);
                double fps = 1000 / t;
                //sb.AppendLine($"{info.Name}: {t * (1e-6f):F2} ms");
                sb.AppendLine($"{info.Name}: {fps:F2}FPS ({t:F2} ms)");
            }
            else if(IsOnlyFPS==false)
            {
                if (info.Unit == ProfilerMarkerDataUnit.Bytes)
                    sb.AppendLine($"{info.Name}: {recorder.LastValue / (1024f * 1024f):F2} MB");
                else if (info.Unit == ProfilerMarkerDataUnit.TimeNanoseconds)
                    sb.AppendLine($"{info.Name}: {GetRecorderFrameAverage(recorder) * (1e-6f):F3} ms");
                else
                    sb.AppendLine($"{info.Name}: {recorder.LastValue} ");
            }
        }
        //sb.AppendLine($"-----");
        //sb.AppendLine($"Frame Time: {GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-6f):F1} ms");
        // sb.AppendLine($"GC Reserved Memory: {gcReservedMemoryRecorder.LastValue / (1024 * 1024)} MB");
        // sb.AppendLine($"GC Used Memory: {gcUsedMemoryRecorder.LastValue / (1024 * 1024)} MB");
        // sb.AppendLine($"System Used Memory: {systemMemoryRecorder.LastValue / (1024 * 1024)} MB");
        // sb.AppendLine($"Total Reserved Memory: {totalReservedMemoryRecorder.LastValue / (1024 * 1024)} MB");
        // sb.AppendLine($"Total Used Memory: {totalUsedMemoryRecorder.LastValue / (1024 * 1024)} MB");
        //sb.AppendLine($"Draw Calls: {drawCallsCountRecorder.LastValue}");
        statsText = sb.ToString();

        if (ResultText != null)
        {
            ResultText.text = statsText;
        }
    }

    //void OnGUI()
    //{
    //    GUI.TextArea(new Rect(200, 10, 240, 500), statsText);
    //}
}
