using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using System.Text;
using System;
namespace MeshProfilerNS
{
    public class MeshFinder
    {
        public static List<MeshElement> GetMeshElementList(MeshElementType type,GameObject rootObj)
        {
            MeshFilter[] filters = rootObj.GetComponentsInChildren<MeshFilter>(true);
            return GetMeshElementList(type,filters,rootObj);
        }

        public static List<MeshElement> GetMeshElementList(MeshElementType type)
        {
            MeshFilter[] filters = GameObject.FindObjectsOfType<MeshFilter>(true);
            return GetMeshElementList(type,filters,null);
        }

        private static string GetPath(Transform t,int maxlevel)
        {
            if(t.parent==null || maxlevel <= 0 ){
                return t.name;
            }
            else{
                return GetPath(t.parent,maxlevel-1)+"/"+t.name;
            }
        }

        private static string GetRelativePath(Transform t,Transform root,int level)
        {
            List<string> path=new List<string>();
            GetRelativePath(t.parent,root,path);
            path.Reverse();
            string r="";
            for (int i = 0; i < path.Count && i<level; i++)
            {
                string p = path[i];
                r +=p+"/";
            }
            return r;
        }

        private static void GetRelativePath(Transform t,Transform root,List<string> path)
        {
            path.Add(t.name);
            if(t.parent==null || t.parent==root ){
                return ;
            }
            else{
                GetRelativePath(t.parent,root,path);
            }
        }

        private static Transform GetParent(Transform t,Transform root,int level)
        {
            List<Transform> path=new List<Transform>();
            GetParent(t.parent,root,path);
            path.Reverse();
            Transform p=null;
            for (int i = 0; i < path.Count && i<level; i++)
            {
                p = path[i];
            }
            return p;
        }

        private static void GetParent(Transform t,Transform root,List<Transform> path)
        {
            path.Add(t);
            if(t.parent==null || t.parent==root ){
                return ;
            }
            else{
                GetParent(t.parent,root,path);
            }
        }

        public static List<MeshElement> GetMeshElementList(MeshElementType type,MeshFilter[] filters,GameObject rootObj)
        {
            DateTime start=DateTime.Now;

            if(filters==null||filters.Length==0){
                filters = GameObject.FindObjectsOfType<MeshFilter>(true);
            }
            Dictionary<string, MeshElement> meshDataDict = new Dictionary<string, MeshElement>();
            //MeshFilter[] filters = GameObject.FindObjectsOfType<MeshFilter>(true);
            // Debug.Log($"MeshFinder.GetMeshElementList filters:{filters.Length},rootObj:{rootObj}");
            int oldInstanceID = -1;
            int errorMesh=0;
            int count1=0;
            int count2=0;
            for (int i = 0; i < filters.Length; i++)
            {
                try
                {
                    if (filters[i].sharedMesh == null)
                    {
                        errorMesh++;
                        continue;
                    }
                        
                    string assetPath = AssetDatabase.GetAssetPath(filters[i].sharedMesh);

                    
                    if ((type==MeshElementType.File || type==MeshElementType.Asset) && (!string.IsNullOrEmpty(assetPath) && !assetPath.Contains("unity default resources")))//is Asset
                    {
                        // Debug.Log($"MeshFinder.GetMeshElementList1 i:{i} meshDataDict:{meshDataDict.Count} assetPath:{assetPath}");

    #if UNITY_2018_3_OR_NEWER
                        GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(filters[i].gameObject);
    #else
                        GameObject root = PrefabUtility.FindPrefabRoot(filters[i].gameObject);
    #endif
                        if (root == null)
                        {
                            if (type==MeshElementType.Asset && !assetPath.EndsWith(".asset"))
                            {
                                //Debug.LogWarning($"MeshFinder [!assetPath.EndsWith(\".asset\")] i:{i} meshDataDict:{meshDataDict.Count} assetPath:{assetPath}");
                                continue;
                            }
                            else
                            {
                                root = filters[i].gameObject;
                            }

                            // root = filters[i].gameObject;
                        }

                        
                        if (!meshDataDict.ContainsKey(assetPath))
                        {
                            GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                            if (obj == null)
                            {
                                obj = root;
                            }
                            MeshElement element = new MeshElement(obj, true);
                            meshDataDict.Add(assetPath, element);
                        }

                        int newInstanceID = root.GetInstanceID();
                        if (newInstanceID != oldInstanceID)
                        {
                            meshDataDict[assetPath].refList.Add(root);
                            oldInstanceID = newInstanceID;
                        }
                        count1++;
                    }
                    else if(type==MeshElementType.GameObject)
                    {
                        string path=GetRelativePath(filters[i].gameObject.transform,rootObj.transform,2);
                        if (!meshDataDict.ContainsKey(path))
                        {
                            MeshElement element = new MeshElement(filters[i].gameObject, false);
                            meshDataDict.Add(path, element);
                            element.refList.Add(filters[i].gameObject);
                            element.AllVertsNum += filters[i].sharedMesh.vertexCount;

                            element.name=path;
                            element.rootObj=GetParent(filters[i].gameObject.transform,rootObj.transform,2).gameObject;
                            //element.name=element.rootObj.name;
                            element.isGroup=true;
                        }
                        else
                        {

                            MeshElement element = meshDataDict[path];
                            element.refList.Add(filters[i].gameObject);
                            element.AllVertsNum += filters[i].sharedMesh.vertexCount;
                        }
                    }
                    else
                    {
                        // Debug.Log($"MeshFinder.GetMeshElementList2 i:{i} meshDataDict:{meshDataDict.Count} assetPath:{assetPath}");

                        count2++;
                        string id = filters[i].sharedMesh.GetInstanceID().ToString();
                        if (!meshDataDict.ContainsKey(id))
                        {
                            MeshElement element = new MeshElement(filters[i].gameObject, false);
                            meshDataDict.Add(id, element);
                            element.refList.Add(filters[i].gameObject);
                            element.AllVertsNum += filters[i].sharedMesh.vertexCount;
                        }

                        else
                        {
                            MeshElement element = meshDataDict[id];
                            element.refList.Add(filters[i].gameObject);
                            element.AllVertsNum += filters[i].sharedMesh.vertexCount;
                        }

                    }
                }
                catch(System.Exception ex)
                {
                    Debug.Log($"Exception:{ex}");
                }
            }
            // Debug.Log($"count1:{count1}");
            // Debug.Log($"count2:{count2}");
            // Debug.Log($"errorMesh:{errorMesh}");
            // Debug.Log($"meshDataDict:{meshDataDict.Count}");
            
            //search skins
            SkinnedMeshRenderer[] skins = GameObject.FindObjectsOfType<SkinnedMeshRenderer>(true);            
            // Debug.Log($"skins:{skins.Length}");
            for (int i = 0; i < skins.Length; i++)
            {
                if (skins[i].sharedMesh == null)
                    continue;
                string assetPath = AssetDatabase.GetAssetPath(skins[i].sharedMesh);
                if ((!string.IsNullOrEmpty(assetPath) && !assetPath.Contains("unity default resources")))//is Asset
                {
#if UNITY_2018_3_OR_NEWER
                    GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(skins[i].gameObject);
#else
                    GameObject root = PrefabUtility.FindPrefabRoot(skins[i].gameObject);
#endif
                    if (root == null)
                    {
                        continue;
                    }
                    int newInstanceID = root.GetInstanceID();
                    if (!meshDataDict.ContainsKey(assetPath))
                    {
                        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                        if (obj == null)
                        {
                            obj = root;
                        }
                        MeshElement element = new MeshElement(obj, true,true);
                        meshDataDict.Add(assetPath, element);

                    }
                    if (newInstanceID != oldInstanceID)
                    {
                        meshDataDict[assetPath].refList.Add(root);
                        oldInstanceID = newInstanceID;
                    }

                }
                else
                {
                    string id = skins[i].sharedMesh.GetInstanceID().ToString();
                    if (!meshDataDict.ContainsKey(id))
                    {
                        MeshElement element = new MeshElement(skins[i].gameObject, false,true);
                        meshDataDict.Add(id, element);
                        element.refList.Add(skins[i].gameObject);
                        element.AllVertsNum += skins[i].sharedMesh.vertexCount;

                       
                    }

                    else
                    {
                        MeshElement element = meshDataDict[id];
                        element.refList.Add(skins[i].gameObject);
                        element.AllVertsNum += skins[i].sharedMesh.vertexCount;

                    }

                }
            }
            // Debug.Log($"meshDataDict:{meshDataDict.Count}");
            List<MeshElement> meshDataList = new List<MeshElement>();
            foreach (string key in meshDataDict.Keys)
            {
                var ele=meshDataDict[key];
                if(type==MeshElementType.GameObject)
                {
                    if(ele.rootObj.transform.parent==rootObj.transform){
                        continue;
                    }
                    var eleNew=new MeshElement(ele.rootObj,false,false);
                     meshDataList.Add(eleNew);
                     eleNew.name=ele.name;
                     //eleNew.name=GetRelativePath(ele.rootObj.transform,rootObj.transform,3);
                }
                else{
                    ele.AllVertsNum = ele.rootMeshValue.vertCount * ele.refList.Count;
                    meshDataList.Add(ele);
                }
            }

            Debug.Log($"GetMeshElementList time:{(DateTime.Now-start).ToString()}");
            return meshDataList;
        }
    }
}
