using UnityEngine;

public class ProgressBarHelper : MonoBehaviour
{
    public static void ClearProgressBar()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif
    }

    public static void DisplayProgressBar(string title, string info, float progress)
    {
        //Debug.Log($"DisplayProgressBar title:{title}\tinfo:{info}\tprogress:{progress}");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayProgressBar(title, info, progress);
#endif  
    }

    public static void DisplayProgressBar(string title, int i, int count)
    {
        float progress1 = (float)i / count;
        DisplayProgressBar(title, $"Progress {i}/{count} {progress1:P1}", progress1);
    }

    public static void DisplayProgressBar(string title, ProgressArg p)
    {
        DisplayProgressBar(title, $"Progress {p}", p.progress);
    }

    public static void DisplayProgressBar(string title, int i1, int count1, int i2, int count2)
    {

        float progress2 = (float)i2 / count2;
        float progress1 = (float)(i1 + progress2) / count1;
        DisplayProgressBar(title, $"Progress {i1}/{count1} {i2}/{count2} {progress1:P1}", progress1);
    }


    public static bool DisplayCancelableProgressBar(string title, string info, float progress)
    {
        //Debug.Log($"DisplayCancelableProgressBar title:{title}\tinfo:{info}\tprogress:{progress}");
        bool result = false;
#if UNITY_EDITOR
        result= UnityEditor.EditorUtility.DisplayCancelableProgressBar(title, info, progress);
#endif
        return result;
    }

    public static bool DisplayCancelableProgressBar(string title, int i,int count)
    {
        float progress1 = (float)i / count;
        return DisplayCancelableProgressBar(title, $"Progress {i}/{count} {progress1:P1}", progress1);
    }

    public static bool DisplayCancelableProgressBar(string title, ProgressArg p)
    {
        return DisplayCancelableProgressBar(title, $"Progress {p}", p.progress);
    }

    public static bool DisplayCancelableProgressBar(string title, int i1, int count1,int i2,int count2)
    {
        
        float progress2 = (float)i2 / count2;
        float progress1 = (float)(i1+ progress2) / count1;
        return DisplayCancelableProgressBar(title, $"Progress {i1}/{count1} {i2}/{count2} {progress1:P1}", progress1);
    }


}
