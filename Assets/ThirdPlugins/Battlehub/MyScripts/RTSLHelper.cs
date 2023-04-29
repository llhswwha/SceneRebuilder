using Battlehub.RTCommon;
using Battlehub.RTSL;
using Battlehub.RTSL.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

public class RTSLHelper : MonoBehaviour
{
    public GameObject TargetRoot;

    public Transform targetParent;

    [SerializeField]
    private string m_sceneDir = "Scenes/DemoScenes/";

    [SerializeField]
    private string m_sceneName = "Model1";

    public RTSLInfo Info = new RTSLInfo();

    [ContextMenu("SaveSceneClick")]
    public void SaveSceneClick()
    {
        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(SaveScene());
        }
        else
        {
            Debug.LogError($"RTSLHelper.SaveSceneClick this.gameObject.activeInHierarchy==false");
        }
    }

    public RTSLInfo SaveModelObject(GameObject obj, bool isSave)
    {
        return SaveModelObject(obj, obj.name, isSave);
    }

    public RTSLInfo SaveModelObject(GameObject obj,string sceneName, bool isSave)
    {
        TargetRoot = obj;
        m_sceneName = sceneName;
        Info.ScenePath = m_sceneDir + m_sceneName;
        if (isSave)
        {
            SaveSceneClick();
        }

        return Info;
    }

    public List<RTSLIgnore> sceneIgnores = new List<RTSLIgnore>();

    private void ClearRTSLIgnores()
    {
        foreach(var item in sceneIgnores)
        {
            GameObject.DestroyImmediate(item);
        }
    }



    IEnumerator SaveScene()
    {
        targetParent = TargetRoot.transform.parent;
        RTSLInfo.SetTargetOthersIgnore(TargetRoot);

        yield return Info.SaveScene();

        if (targetParent != null && TargetRoot!=null)
        {
            TargetRoot.transform.SetParent(targetParent);
        }
    }

    [ContextMenu("LoadSceneClick")]
    public void LoadSceneClick()
    {
        //IProject m_project = IOC.Resolve<IProject>();
        //if (m_project.Exist<Scene>(m_scenePath))
        //{
        //    StartCoroutine(LoadScene());
        //}
        DateTime startT = DateTime.Now;
        StartCoroutine(LoadScene(result=> {
            result.SetRootActive(false);
            Debug.Log($"RTSLHelper.LoadSceneClick [{(DateTime.Now - startT).TotalMilliseconds:F2}ms] result:{result} root:{result.GetRoot()}");
        }));
    }

    private void Awake()
    {
        //RTSLAdditive rtslAdditive = this.GetComponent<RTSLAdditive>();
        //if (rtslAdditive == null)
        //{
        //    rtslAdditive = this.gameObject.AddComponent<RTSLAdditive>();
        //}
    }

    IEnumerator LoadScene(Action<LoadModelResult> afterLoadedAction)
    {
        //RTSLInfo.SetTargetOthersIgnore(TargetRoot);
        yield return Info.LoadScene(afterLoadedAction);
    }

    //static IEnumerator LoadScene(GameObject target)
    //{
    //    RTSLInfo.SetTargetOthersIgnore(target);
    //    yield return Info.LoadScene();
    //}

    [ContextMenu("OpenProjectTest")]
    public void OpenProjectTest()
    {
        StartCoroutine(OpenProject());
    }

    public IEnumerator OpenProject()
    {
        IProject m_project = IOC.Resolve<IProject>();

        if (m_project.IsOpened == false || m_project.ProjectInfo.Name != Info.ProjectName)
        {
            Debug.Log($"OpenProject ProjectName:{Info.ProjectName}");
            yield return m_project.OpenProject(Info.ProjectName);
        }
        else
        {
            Debug.Log($"OpenProject m_project:{m_project}");
        }
    }

}

[Serializable]
public class RTSLInfo
{
    public string ProjectName = "ImportModels";

    public string ScenePath = "Scenes/DemoScenes/Model1";

    public static void AddRTSLIgnore(GameObject obj)
    {
        RTSLIgnore ignore = obj.GetComponent<RTSLIgnore>();
        if (ignore == null)
        {
            obj.AddComponent<RTSLIgnore>();
        }
        //sceneIgnores.Add(ignore);
    }

    public static void SetTargetOthersIgnore(GameObject target)
    {
        var scene = SceneManager.GetActiveScene();
        GameObject[] rootObjs = scene.GetRootGameObjects();

        if (target == null)
        {
            for (int i = 0; i < rootObjs.Length; i++)
            {
                AddRTSLIgnore(rootObjs[i]);
            }
            return;
        }

        target.transform.parent = null;
        if (target.transform.parent == null)
        {
            for (int i = 0; i < rootObjs.Length; i++)
            {
                if (rootObjs[i] != target)
                {
                    AddRTSLIgnore(rootObjs[i]);
                }
            }
        }
    }

    public static void SetRootsIgnore()
    {
        var scene = SceneManager.GetActiveScene();
        GameObject[] rootObjs = scene.GetRootGameObjects();
        for (int i = 0; i < rootObjs.Length; i++)
        {
            AddRTSLIgnore(rootObjs[i]);
        }
    }

    public IEnumerator SaveScene()
    {
        IProject m_project = IOC.Resolve<IProject>();

        if (m_project.IsOpened == false || m_project.ProjectInfo.Name != ProjectName)
        {
            yield return m_project.OpenProject(ProjectName);
        }
        yield return m_project.CreateFolder(Path.GetDirectoryName(ScenePath).Replace(@"\", "/"));
        ProjectAsyncOperation ao = m_project.Save(ScenePath, SceneManager.GetActiveScene());
        yield return ao;
        bool isSceneExist = m_project.Exist<Scene>(ScenePath);
    }

    private Action<LoadModelResult> afterLoadedAction;

    private IProject m_project;

    public IEnumerator<ProjectAsyncOperation> LoadScene(Action<LoadModelResult> afterLoadedAction)
    {
        SetRootsIgnore();

        this.afterLoadedAction = afterLoadedAction;

        if (m_project == null)
        {
            m_project = IOC.Resolve<IProject>();
        }


        IsCallbackUsed = false;
        m_project.LoadCompleted -= M_project_LoadCompleted;
        m_project.LoadCompleted += M_project_LoadCompleted;

        if (m_project.IsOpened == false || m_project.ProjectInfo.Name != ProjectName)
        {
            yield return m_project.OpenProject(ProjectName);
        }
        yield return m_project.CreateFolder(Path.GetDirectoryName(ScenePath).Replace(@"\", "/"));
        if (m_project.Exist<Scene>(ScenePath))
        {
            ProjectAsyncOperation<UnityObject[]> ao = m_project.Load<Scene>(ScenePath);
            //ao.MoveNext();
            //LoadModelResult modelResult = new LoadModelResult(ao.Result);
            //if (afterLoadedAction != null)
            //{
            //    afterLoadedAction(modelResult);
            //}
            yield return ao;
        }
        else
        {
            if (afterLoadedAction != null)
            {
                afterLoadedAction(null);
            }
            yield return null;
        }
    }

    public bool IsCallbackUsed = false;

    private void M_project_LoadCompleted(Error error, AssetItem[] result, UnityObject[] result2)
    {
        if (m_project == null)
        {
            m_project = IOC.Resolve<IProject>();
        }
        m_project.LoadCompleted -= M_project_LoadCompleted;
        if (IsCallbackUsed==false)
        {
            IsCallbackUsed = true;

            Debug.Log($"M_project_LoadCompleted!!!!!! 1 IsCallbackUsed:{IsCallbackUsed} ScenePath:{ScenePath}");

            if (afterLoadedAction != null)
            {
                LoadModelResult modelResult = new LoadModelResult(result2);
                afterLoadedAction(modelResult);
            }
        }
        else
        {
            Debug.Log($"M_project_LoadCompleted!!!!!! 2 IsCallbackUsed:{IsCallbackUsed} ScenePath:{ScenePath}");
        }

        
    }
}

public class LoadModelResult
{
    public LoadModelResult(UnityObject[] result2)
    {
        List<GameObject> roots = new List<GameObject>();
        if (result2 != null)
        {
            for (int i = 0; i < result2.Length; i++)
            {
                var item = result2[i] as GameObject;
                if (item != null && item.transform.parent == null)
                {
                    roots.Add(item);
                    //item.SetActive(false);
                }
            }
        }

        gameObjects = result2;
        rootObjects = roots.ToArray();
    }

    public UnityObject[] gameObjects;

    public int GameObjectCount {
        get {
            if (gameObjects == null)
            {
                return 0;
            }
            return gameObjects.Length;
        }
    }

    public GameObject[] rootObjects;

    public int RootObjectCount {
        get {
            if (rootObjects == null)
            {
                return 0;
            }
            return rootObjects.Length;
        }
    }

    public GameObject GetRoot()
    {
        if (RootObjectCount == 1)
        {
            return rootObjects[0];
        }
        else if(RootObjectCount>1)
        {
            Debug.LogWarning($"LoadModelResult.GetRoot RootObjectCount>1 RootObjectCount:{RootObjectCount}");
            return rootObjects[0];
        }
        else
        {
            return null;
        }
    }

    public void SetRootActive(bool isActive)
    {
        GameObject root = GetRoot() as GameObject;
        Debug.Log($"SetRootActive root:{root}");
        if (root)
        {
            root.SetActive(isActive);
        }
    }

    public override string ToString()
    {
        return $"[GameObjectCount:{GameObjectCount} RootObjectCount:{RootObjectCount}]";
    }
}
