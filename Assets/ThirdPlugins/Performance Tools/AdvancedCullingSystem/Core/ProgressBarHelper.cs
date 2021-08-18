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
        //Debug.Log($"info:{info}\tprogress:{progress:F2} title:{title}\tProgressBarHelper.DisplayProgressBar ");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayProgressBar(title, info, progress);
#endif  
    }

    public static bool DisplayCancelableProgressBar(string title, string info, float progress)
    {
        //if(float.IsNaN(progress))
        //{

        //}
        //Debug.Log($"info:{info}\tprogress:{progress:F2} title:{title}\tProgressBarHelper.DisplayCancelableProgressBar ");
        bool result = false;
#if UNITY_EDITOR
        result = UnityEditor.EditorUtility.DisplayCancelableProgressBar(title, info, progress);
#endif
        return result;
    }

    public static void DisplayProgressBar(string title, int i, int count)
    {
        float progress1 = (float)i / count;
        DisplayProgressBar(title, $":{i}/{count} {progress1:P1}", progress1);
    }

    public static void DisplayProgressBar(string title, ProgressArg p)
    {
        DisplayProgressBar(title + "_" + p.GetTitle(), $":{p}", p.progress);
    }

    public static void DisplayProgressBar(ProgressArg p)
    {
        DisplayProgressBar(p.GetTitle(), $":{p}", p.progress);
    }

    public static void DisplayProgressBar(string title, int i1, int count1, int i2, int count2)
    {

        float progress2 = (float)i2 / count2;
        float progress1 = (float)(i1 + progress2) / count1;
        DisplayProgressBar(title, $":{i1}/{count1} {i2}/{count2} {progress1:P1}", progress1);
    }

    public static bool DisplayCancelableProgressBar(string title, int i, int count)
    {
        float progress1 = (float)i / count;
        return DisplayCancelableProgressBar(title, $":{i}/{count} {progress1:P1}", progress1);
    }

    public static bool DisplayCancelableProgressBar(string title, ProgressArg p)
    {
        return DisplayCancelableProgressBar(title + "_" + p.GetTitle(), $":{p}", p.progress);
    }

    public static bool DisplayCancelableProgressBar(ProgressArg p)
    {
        return DisplayCancelableProgressBar(p.GetTitle(), $":{p}", p.progress);
    }

    public static bool DisplayCancelableProgressBar(string title, int i1, int count1, int i2, int count2)
    {

        float progress2 = (float)i2 / count2;
        float progress1 = (float)(i1 + progress2) / count1;
        return DisplayCancelableProgressBar(title, $":{i1}/{count1} {i2}/{count2} {progress1:P1}", progress1);
    }


}
