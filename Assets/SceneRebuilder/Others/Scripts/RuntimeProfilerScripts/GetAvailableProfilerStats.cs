using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;

public class GetAvailableProfilerStats
{
    /***********************************************************************************************************************************************
    * Source: https://docs.unity3d.com/2020.2/Documentation/ScriptReference/Unity.Profiling.LowLevel.Unsafe.ProfilerRecorderHandle.GetAvailable.html
    ************************************************************************************************************************************************/
    public static Dictionary<string,StatInfo> EnumerateProfilerStats()
    {
        var availableStatHandles = new List<ProfilerRecorderHandle>();
        ProfilerRecorderHandle.GetAvailable(availableStatHandles);

        Dictionary<string,StatInfo> statDict=new Dictionary<string, StatInfo>();
        var availableStats = new List<StatInfo>(availableStatHandles.Count);
        foreach (var h in availableStatHandles)
        {
            var statDesc = ProfilerRecorderHandle.GetDescription(h);
            var statInfo = new StatInfo()
            {
                Cat = statDesc.Category,
                Name = statDesc.Name,
                Unit = statDesc.UnitType
            };
            availableStats.Add(statInfo);
            //string key=statDesc.Category+"_"+statInfo.Name;
            string key=statInfo.Name;
            if(statDict.ContainsKey(key)){
                //Debug.LogError("statDict.ContainsKey(key):"+key);
            }
            else{
                statDict.Add(key,statInfo);
            }
           
        }
        availableStats.Sort((a, b) =>
        {
            var result = string.Compare(a.Cat.ToString(), b.Cat.ToString());
            if (result != 0)
                return result;

            return string.Compare(a.Name, b.Name);
        });

        var sb = new StringBuilder("Available stats:\n");
        foreach (var s in availableStats)
        {
            sb.AppendLine($"{s.Cat.ToString()}\t\t - {s.Name}\t\t - {s.Unit}");
        }

        FileManager.WriteToFile("E:\\AvailableStats.txt", sb.ToString());
        //Debug.Log(sb.ToString());
        return statDict;
    }
}

    public struct StatInfo
    {
        public ProfilerCategory Cat;
        public string Name;
        public ProfilerMarkerDataUnit Unit;
    }