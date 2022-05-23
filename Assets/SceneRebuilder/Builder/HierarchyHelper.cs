using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
using Base.Common;
using System.IO;
using System.Text;

public static class HierarchyHelper
{
    public static IdInfoList CheckHierarchy()
    {
        GameObject go = null;
#if UNITY_EDITOR
        go = Selection.activeGameObject;
#endif
        return CheckHierarchy(go);
    }

    public static IdInfoList CheckHierarchy(GameObject go)
    {
        DateTime start = DateTime.Now;

        //GameObject go = Selection.activeGameObject;
        Transform root = go.transform;
        var transforms = go.GetComponentsInChildren<Transform>(true);
        Dictionary<string, Transform> path2Transform = new Dictionary<string, Transform>();
        for (int i1 = 0; i1 < transforms.Length; i1++)
        {
            Transform t = transforms[i1];
            string path = t.GetPath(root);
            ProgressArg pA = new ProgressArg("CheckHierarchy1", i1, transforms.Length, $"{path}");
            if (path2Transform.ContainsKey(path))
            {
                Transform t0 = path2Transform[path];
                Debug.LogError($"CheckHierarchy[{i1}] t:{t.name} t0:{t0.name} path:{path}");
            }
            else
            {
                path2Transform.Add(path, t);
            }
            
            //if (i1 < 100)
            //{
            //    Debug.Log($"CheckHierarchy1[{i1}] item:{t.name} path:{path}");
            //}
        }
        TimeSpan time1 = DateTime.Now - start;

        IdInfoList idList = LoadXml(go);
        if (idList == null)
        {
            Debug.LogError($"CheckHierarchy idList == null");
            return idList;
        }
        //LoadHierarchy(ts2, false);

        List<IdInfo> notFoundList = new List<IdInfo>();

        List<IdInfo> foundList = new List<IdInfo>();

        var allItems = idList.GetAllItems();
        int i = 0;
        int notFoundCount = 0;
        for (; i < allItems.Count; i++)
        {
            IdInfo idInfo = allItems[i];

            //string idPath = idInfo.GetPath();
            //if (idInfo.HasMesh == false && (idInfo.name.Contains("curve") || idInfo.name.Contains("Geometry")))
            //{
            //    continue;
            //}

            string path = idInfo.GetPath();
            if (path2Transform.ContainsKey(path))
            {
                foundList.Add(idInfo);
            }
            else
            {
                notFoundList.Add(idInfo);
                Debug.LogError($"CheckHierarchy2[{notFoundCount++}] item:{idInfo.name} path:{path}");
            }

            ProgressArg pA = new ProgressArg("CheckHierarchy2", i, allItems.Count, $"{idInfo}");

            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
            //if (i < 100)
            //{
            //    Debug.Log($"CheckHierarchy2[{i}] item:{idInfo.name} path:{path}");
            //}
        }
        ProgressBarHelper.ClearProgressBar();

        Debug.LogError($"CheckHierarchy 【transforms:{transforms.Length} time1:{time1}】 【allItems:{allItems.Count} time:{DateTime.Now-start}】 foundList:{foundList.Count} notFoundList:{notFoundList.Count}");

        idList.notFoundList = notFoundList;

        return idList;
    }

    public static IdInfoList LoadXml(GameObject go)
    {
        //GameObject go = Selection.activeGameObject;
        string path = GetIdInfoListFilePath(go.name);
        if (File.Exists(path) == false)
        {
            Debug.LogError($"LoadHierarchy FileNotFound path:{path}");
            return null;
        }
        string xml = File.ReadAllText(path);
        //Debug.Log($"LoadHierarchy xml:{xml}");
        IdInfoList idList = SerializeHelper.LoadFromText<IdInfoList>(xml);
        return idList;
    }

    public static IdInfoList LoadHierarchy_All(bool isSetParent)
    {
        var ts2 = GameObject.FindObjectsOfType<Transform>(true);
        return LoadHierarchy(ts2, isSetParent);
    }

    public static IdInfoList LoadHierarchy(GameObject go, bool isSetParent)
    {
        var ts2 = go.GetComponentsInChildren<Transform>(true);
        return LoadHierarchy(go, ts2, isSetParent);
    }

    public static IdInfoList LoadHierarchy(Transform[] ts2, bool isSetParent)
    {
        GameObject go = null;
#if UNITY_EDITOR
        go = Selection.activeGameObject;
#endif
        return LoadHierarchy(go, ts2, isSetParent);
    }

    public static IdInfoList LoadHierarchy(GameObject go,Transform[] ts2, bool isSetParent)
    {
        DateTime start = DateTime.Now;

        //GameObject go = Selection.activeGameObject;
        string path = GetIdInfoListFilePath(go.name);
        if (File.Exists(path) == false)
        {
            Debug.LogError($"LoadHierarchy FileNotFound path:{path}");
            return null;
        }
        string xml = File.ReadAllText(path);
        //Debug.Log($"LoadHierarchy xml:{xml}");
        IdInfoList idList = SerializeHelper.LoadFromText<IdInfoList>(xml);
        var allItems = idList.GetAllItems();
        Debug.Log($"LoadHierarchy idList:{idList} ids:{allItems.Count} xml:{xml} ");

        IdDictionary.InitInfos(null, true, true);

        //var ts2 = GameObject.FindObjectsOfType<Transform>(true);

        //var ts2 = go.GetComponentsInChildren<Transform>(true);
        //Dictionary<Transform, Transform> tDict = new Dictionary<Transform, Transform>();
        //DictionaryList1ToN<Transform> nameListDict = new DictionaryList1ToN<Transform>();

        //foreach (var t in ts2)
        //{
        //    //tDict.Add(t, t);
        //    nameListDict.AddItem(t.name, t);
        //}

        TransformDictionary tDict = new TransformDictionary(ts2.ToList());
        int count1 = tDict.Count;

        List<IdInfo> notFoundList = new List<IdInfo>();
        List<IdInfo> foundList = new List<IdInfo>();
        List<IdInfo> noMeshList = new List<IdInfo>();
        StringBuilder notFoundListSb = new StringBuilder();

        int i = 0;
        for (; i < allItems.Count; i++)
        {
            IdInfo idInfo = allItems[i];
            if (idInfo.name == "厂区雨水回用水管 27")
            {
                Debug.LogError("LoadHierarchy_Debug 厂区雨水回用水管 27 !!!!!");
            }
            string idPath = idInfo.GetPath();
            if (idInfo.HasMesh == false && (idInfo.name.Contains("curve") || idInfo.name.Contains("Geometry")))
            {
                continue;
            }
            ProgressArg pA = new ProgressArg("LoadHierarchy", i, allItems.Count, $"[{tDict.Count}]{idInfo.name}({idInfo.Id})");

            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }

            if (idInfo.HasMesh == false)
            {
                //if(idInfo.parent != "Unknown")
                //{
                //    Debug.LogWarning($"LoadHierarchy[{i}] idInfo.HasMesh==false 【{idInfo} path:{idInfo.GetPath()}】");
                //}
                noMeshList.Add(idInfo);
                continue;
            }

            RendererId idGo = FindRidObject(tDict, idInfo, i, false);
            if (idGo == null)
            {
                notFoundList.Add(idInfo);
                notFoundListSb.AppendLine(idInfo.ToString());
                //break;
                continue;
            }

            //foundList.Add(idInfo);

            MeshRenderer renderer = idGo.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                if (idGo.transform.parent == null)
                {
                    Debug.LogWarning($"LoadHierarchy[{i}]  idGo.transform.parent=null 【{idInfo} path:{idInfo.GetPath()}】 path:{idGo.transform.GetPath()}");
                    continue;
                }
                else if (idGo.transform.parent.name != idInfo.parent)
                {
                    Vector3 pos = idInfo.GetPosition();
                    Vector3 center = idInfo.GetCenter();

                    var disOfPos = Vector3.Distance(pos, idGo.transform.position);
                    var disOfCenter = Vector3.Distance(center, MeshRendererInfo.GetCenterPos(idGo.gameObject));

                    Debug.LogWarning($"LoadHierarchy[{i}] renderer == null 【{idInfo} path:{idInfo.GetPath()}】 parent:{idGo.transform.parent.name} path:{idGo.transform.GetPath()}|disOfPos:{disOfPos} disOfCenter:{disOfCenter}  ");

                    notFoundList.Add(idInfo);
                    notFoundListSb.AppendLine(idInfo.ToString());
                    continue;
                }
                else
                {
                    Debug.LogError($"LoadHierarchy[{i}]  renderer==null 【{idInfo} path:{idInfo.GetPath()}】 path:{idGo.transform.GetPath()}");
                    noMeshList.Add(idInfo);
                    break;
                }
            }

            IdInfo pid = idInfo.pId;

            RendererId pidGo = FindRidObject(tDict, pid, i, false);

            //var pGo=idGo.SetParent(idInfo.parentId);

            if (pidGo == null)
            {
                IdInfo pid2 = pid.pId;
                if (pid2 == null) continue;
                RendererId pidGo2 = FindRidObject(tDict, pid2, i, false);

                if (pidGo2 == null)
                {
                    if (idInfo.GetPath().Contains(">Welding>"))
                    {
                        continue;
                    }

                    Debug.LogError($"LoadHierarchy[{i}] Error22 pidGo == null pidGo2 == null tDict:{tDict.Count} 【{idInfo} path:{idInfo.GetPath()}】 path:{idGo.transform.GetPath()} | time:{DateTime.Now - start}  ");
                    notFoundList.Add(idInfo);
                    notFoundListSb.AppendLine(idInfo.ToString());
                    break;
                }
                else
                {
                    //GameObject pGo = GameObject.Find(idInfo.parent);
                    RendererId pGo = pidGo2.FindChildByName(pid.name);
                    if (pGo == null)
                    {
                        Debug.LogWarning($"LoadHierarchy[{i}] Error23 pidGo == null tDict:{tDict.Count} 【{idInfo} path:{idInfo.GetPath()}】 path1:{idGo.transform.GetPath()} 【pid2:{pid2}】path2:{pidGo2.transform.GetPath()}  time:{DateTime.Now - start} ");
                        GameObject goNew = new GameObject(pid.name);
                        //goNew.transform.position = idInfo.GetPosition();
                        goNew.transform.position = pid.GetPosition();
                        goNew.transform.SetParent(pidGo2.transform);
                        idGo.transform.SetParent(goNew.transform);
                        //break;
                        tDict.AddItemEx(goNew.transform);
                    }
                    else
                    {
                        if (idInfo.GetPath().Contains("F级汽机、消防管道修改"))
                        {
                            if (isSetParent)
                            {
                                idGo.transform.SetParent(pGo.transform);
                                pGo.SetPid(idInfo.parentId, null);
                            }
                            tDict.RemoveTransform(idGo.transform);
                            foundList.Add(idInfo);
                            continue;
                        }
                        else
                        {
                            Debug.LogError($"LoadHierarchy[{i}] Error23 pidGo == null tDict:{tDict.Count} 【{idInfo} path:{idInfo.GetPath()}】 path1:{idGo.transform.GetPath()} 【pid2:{pid2}】path2:{pidGo2.transform.GetPath()}  path3:{pGo.transform.GetPath()} time:{DateTime.Now - start} ");
                            break;
                        }
                    }
                }
            }
            else
            {
                if (isSetParent)
                {
                    idGo.transform.SetParent(pidGo.transform);
                    pidGo.SetPid(idInfo.parentId, null);
                }
                tDict.RemoveTransform(idGo.transform);
                foundList.Add(idInfo);
            }
        }
        int count2 = tDict.Count;
        ProgressBarHelper.ClearProgressBar();

        Debug.LogError($"LoadHierarchy_Debug go:{go} allItems:{allItems.Count} i:{i} count1:{count1} count2:{count2} notFoundList:{notFoundList.Count} foundList:{foundList.Count} noMeshList:{noMeshList.Count} time:{DateTime.Now - start}\nnotFoundListSb:{notFoundListSb.ToString()}");
        return idList;
    }

    private static RendererId FindRidObject(TransformDictionary ts, IdInfo id, int i, bool showLog)
    {
        RendererId idGo = null;
        RendererId pidGo = null;
        if (!string.IsNullOrEmpty(id.Id))
        {
            idGo = IdDictionary.GetId(id, showLog);
        }
        if (idGo == null)
        {
            if (!string.IsNullOrEmpty(id.parentId))
            {
                pidGo = IdDictionary.GetPId(id, showLog);
            }
            if (pidGo == null)
            {
                //var result=ts.FindModelsByPosAndName(id);
                Transform idGo3 = TransformHelper.FindOneByNameAndPos(ts, id, 0.0005f, false);
                if (idGo3 == null)
                {
                    Debug.LogError($"LoadHierarchy[{i}] Error1 idGo == null pidGo == null idGo3 == null ts:{ts.Count} {id.GetFullString()}");
                    return null;
                }
                else
                {
                    idGo = RendererId.GetRId(idGo3);
                }
            }
            else
            {
                RendererId idGo2 = pidGo.FindChildByName(id.name);
                if (idGo2 == null)
                {
                    Transform idGo3 = TransformHelper.FindOneByNameAndPos(ts, id, 0.0005f, false);
                    if (idGo3 == null)
                    {
                        Debug.LogError($"LoadHierarchy[{i}] Error2 idGo == null idGo2 == null idGo3 == null ts:{ts.Count} {id.GetFullString()}");
                        return null;
                    }
                    else
                    {
                        idGo = RendererId.GetRId(idGo3);
                    }
                }
                else
                {
                    idGo = idGo2;
                }
            }
        }
        //if (idGo != null)
        //{
        //    ts.Remove(idGo.transform);
        //}
        return idGo;
    }


    public static string GetIdInfoListFilePath(string name)
    {
        //string path = Application.dataPath + IdInfoListFilePath;
        string path = $"{Application.dataPath}\\..\\IdInfoList_{name}.XML";
        return path;
    }

    public static IdInfoList SaveHierarchy()
    {
        GameObject go = null;
#if UNITY_EDITOR
        go = Selection.activeGameObject;
#endif
        return SaveHierarchy(go);
    }

    public static IdInfoList SaveHierarchy(GameObject go)
    {
        IdDictionary.InitInfos(null, true, true);

        
        EditorHelper.UnpackPrefab(go);
        var rendrerers = go.GetComponentsInChildren<RendererId>(true);
        IdInfoList idList = new IdInfoList();
        idList.SetRoot(go, true);

        //for (int i = 0; i < rendrerers.Length; i++)
        //{
        //    RendererId renderer = rendrerers[i];
        //    ProgressArg pA = new ProgressArg("SaveHierarchy", i, rendrerers.Length, renderer);
        //    if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
        //    {
        //        break;
        //    }
        //    if (renderer.gameObject == go) continue;
        //    idList.AddId(renderer);
        //}

        ProgressBarHelper.ClearProgressBar();
        //SerializeHelper.Save(idList, "IdInfoList.xml");

        string xml = SerializeHelper.GetXmlText(idList);
        //Debug.Log($"SaveXml xml:{xml}");
        string path = GetIdInfoListFilePath(go.name);
        File.WriteAllText(path, xml);

        Debug.Log($"SaveHierarchy go:{go} rendrerers:{rendrerers.Length} path:{path}");
        return idList;
    }

    public static void InitHierarchy()
    {
        //GameObject go = Selection.activeGameObject;
        ////RendererId.InitId(go);

        GameObject go = null;
#if UNITY_EDITOR
        go = Selection.activeGameObject;
#endif
        var rendrerers = go.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < rendrerers.Length; i++)
        {
            MeshRenderer renderer = rendrerers[i];
            ProgressArg pA = new ProgressArg("InitHierarchy", i, rendrerers.Length, renderer);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
            if (renderer.gameObject == go) continue;
            RendererId.InitId(renderer);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"InitHierarchy rendrerers:{rendrerers.Length}");
    }

    public static void ClearHierarchy()
    {
        GameObject go = null;
#if UNITY_EDITOR
        go = Selection.activeGameObject;
#endif
        EditorHelper.UnpackPrefab(go);
        var rendrerers = go.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < rendrerers.Length; i++)
        {
            MeshRenderer renderer = rendrerers[i];
            ProgressArg pA = new ProgressArg("ClearHierarchy", i, rendrerers.Length, renderer);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
            if (renderer.gameObject == go) continue;
            EditorHelper.UnpackPrefab(renderer.gameObject);
            renderer.transform.SetParent(go.transform);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"ClearHierarchy rendrerers:{rendrerers.Length}");
    }
}
