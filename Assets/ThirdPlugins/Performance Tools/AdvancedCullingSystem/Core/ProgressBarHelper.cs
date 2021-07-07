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

    public static bool DisplayCancelableProgressBar(string title, string info, float progress)
    {
        //Debug.Log($"DisplayCancelableProgressBar title:{title}\tinfo:{info}\tprogress:{progress}");
        bool result = false;
#if UNITY_EDITOR
        result= UnityEditor.EditorUtility.DisplayCancelableProgressBar(title, info, progress);
#endif
        return result;
    }
}
