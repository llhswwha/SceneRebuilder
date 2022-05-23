using UnityEngine;

public class ProgressBarHelper : MonoBehaviour
{
    public static void ClearProgressBar()
    {
        if (EnableProgressBar == false) return;

        if (EnableProgressLog)
        {
            Debug.LogError($"[progress:]\tProgressBarHelper.ClearProgressBar ");
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif
    }

    public static void DisplayProgressBar(string title, string info, float progress)
    {
        if (EnableProgressBar == false) return;

        if (EnableProgressLog)
        {
            Debug.Log($"[progress:{progress:F2}]  info:{info}\t title:{title}\tProgressBarHelper.DisplayCancelableProgressBar ");
        }

        //Debug.Log($"info:{info}\tprogress:{progress:F2} title:{title}\tProgressBarHelper.DisplayProgressBar ");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayProgressBar(title, info, progress);
#endif  
    }

    public static bool EnableProgressBar = true;

    public static bool EnableProgressLog = false;

    public static bool DisplayCancelableProgressBar(string title, string info, float progress)
    {
        //if(float.IsNaN(progress))
        //{

        //}

        if (EnableProgressBar == false) return false;

        //if (EnableProgressLog)
        //{
        // Debug.Log($"[progress:{progress:P2}]  info:{info}\t title:{title}\tProgressBarHelper.DisplayCancelableProgressBar ");
        //}



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

    public static void DisplayProgressBar(string title, IProgressArg p)
    {
        DisplayProgressBar(title + "_" + p.GetTitle(), $":{p}", p.GetProgress());
    }

    public static void DisplayProgressBar(IProgressArg p)
    {
        DisplayProgressBar(p.GetTitle(), $":{p}", p.GetProgress());
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

    public static bool DisplayCancelableProgressBar(string title, IProgressArg p)
    {
        return DisplayCancelableProgressBar(title + "_" + p.GetTitle(), $":{p}", p.GetProgress());
    }

    public static float lastProgress = 0;

    public static bool DisplayCancelableProgressBar(IProgressArg p)
    {
        if (EnableProgressBar == false) return false;

        //if (EnableProgressLog)
        //{
        //    float newProgress= p.GetProgress();
        //    if (lastProgress > newProgress)
        //    {
        //        Debug.LogError($"[progress:{p.GetProgress():P2}]  info:{p}\t title:{p.GetTitle()}\tProgressBarHelper.DisplayCancelableProgressBar ");
        //    }
        //    lastProgress = newProgress;
        //}

        return DisplayCancelableProgressBar(p.GetTitle(), $":{p}", p.GetProgress());
    }

    public static bool DisplayCancelableProgressBar(IProgressArg p,bool enable)
    {
        bool oldState = EnableProgressBar;
        EnableProgressBar = enable;
        bool r= DisplayCancelableProgressBar(p);
        EnableProgressBar = oldState;
        return r;
    }

    public static bool DisplayCancelableProgressBar(string title, int i1, int count1,object t=null)
    {
        return DisplayCancelableProgressBar(new ProgressArg(title,i1,count1,t));
    }

    public static bool DisplayCancelableProgressBar(string title, int i1, int count1, int i2, int count2)
    {

        float progress2 = (float)i2 / count2;
        float progress1 = (float)(i1 + progress2) / count1;
        return DisplayCancelableProgressBar(title, $":{i1}/{count1} {i2}/{count2} {progress1:P1}", progress1);
    }


}
