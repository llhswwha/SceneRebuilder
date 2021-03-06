using NavisPlugins.Infos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class TransformDictionary 
{
    //public List<Transform> list = new List<Transform>();

    public Dictionary<Transform,Transform> dict = new Dictionary<Transform, Transform>();

    public DictionaryList1ToN<Transform> nameListDict0 = new DictionaryList1ToN<Transform>();
    public DictionaryList1ToN<Transform> nameListDict1 = new DictionaryList1ToN<Transform>();
    public DictionaryList1ToN<Transform> nameListDict2 = new DictionaryList1ToN<Transform>();
    public DictionaryList1ToN<Transform> uidListDict = new DictionaryList1ToN<Transform>();
    public Dictionary<string, Transform> nameDict = new Dictionary<string, Transform>();
    public Dictionary<string, Transform> uidDict = new Dictionary<string, Transform>();

    public PositionDictionaryList<Transform> positionDictionaryList = new PositionDictionaryList<Transform>();

    public PositionDictionaryList<Transform> positionDictionaryList2 = new PositionDictionaryList<Transform>();

    public List<Transform> ToList()
    {
        return dict.Keys.ToList();
    }

    public int Count
    {
        get
        {
            return dict.Count;
        }
    }

    public TransformDictionary(List<Transform> lst)
    {
        //this.list.AddRange(lst);
        foreach(var item in lst)
        {
            if(!dict.ContainsKey(item))
            {
                dict.Add(item, item);
            }
            else
            {
                Debug.LogError("TransformDictionary.Init Repeated Item:"+item);
            }
        }
        InitDict();
        GetTransformNames();
        CheckUidRepeated();
    }

    public TransformDictionary(List<MeshRenderer> lst)
    {
        //this.list.AddRange(lst);
        foreach (var renderer in lst)
        {
            var item = renderer.transform;
            if (!dict.ContainsKey(item))
            {
                dict.Add(item, item);
            }
            else
            {
                Debug.LogError("TransformDictionary.Init Repeated Item:" + item);
            }
        }
        InitDict();
        GetTransformNames();
        CheckUidRepeated();
    }

    public void Remove(Transform t)
    {
        dict.Remove(t);
    }

    public void InitDict()
    {
        nameListDict1 = new DictionaryList1ToN<Transform>();
        uidListDict = new DictionaryList1ToN<Transform>();
        nameDict = new Dictionary<string, Transform>();
        uidDict = new Dictionary<string, Transform>();

        positionDictionaryList = new PositionDictionaryList<Transform>();
        positionDictionaryList2 = new PositionDictionaryList<Transform>();

        InitNameDict();
        InitPosDict();
        InitPosDict2();
    }

    private void GetTransformNames()
    {
        List<string> uids = new List<string>();
        List<string> otherNames = new List<string>();
        List<string> kks = new List<string>();

        Dictionary<string, List<Transform>> nameDict = new Dictionary<string, List<Transform>>();

        StringBuilder errorInfo = new StringBuilder();
        foreach (var t in dict.Keys)
        {
            string n = t.name;
            if (n.Contains("_New"))
            {
                n = n.Replace("_New", "");
                t.name = n;
            }

            if (!nameDict.ContainsKey(n))
            {
                nameDict.Add(n, new List<Transform>());
                //names.Add(n);

                if (IsUID(n, errorInfo))
                {
                    uids.Add(n);
                }
                else if (IsKKS(n))
                {
                    kks.Add(n);
                }
                else
                {
                    if (!n.Contains("Degree_Direction_Change") &&
                        !n.Contains("Concentric_Reducer") &&
                        !n.Contains("Flange-") &&
                        !n.Contains("Undefined") &&
                        !n.Contains("MemberPartPrismatic") &&
                        !n.Contains("Undefined") &&
                        !n.Contains("??????????????????????") &&
                        !n.Contains("????????????????????????????") && 
                        !n.Contains("3203????????????????????")
                        )
                        otherNames.Add(n);
                }
            }
            nameDict[n].Add(t);

        }

        if (errorInfo.Length > 0)
        {
            Debug.LogWarning(errorInfo.ToString());
        }

        uids.Sort();
        string txt = "";
        foreach (var uid in uids)
        {
            txt += uid + ";\t";
        }

        otherNames.Sort();
        string txt2 = "";
        foreach (var name in otherNames)
        {
            txt2 += name + ";\t";
        }

        kks.Sort();
        string txt3 = "";
        foreach (var name in kks)
        {
            txt3 += name + ";\t";
        }
        Debug.Log($"uids({uids.Count}):\n{txt}");
        Debug.Log($"kks({kks.Count}):\n{txt3}");
        Debug.Log($"otherNames({otherNames.Count}):\n{txt2}");
    }

    private void InitPosDict()
    {
        foreach (var t in dict.Keys)
        {
            AddItemPos(t);
        }
        positionDictionaryList.ShowCount("TransformDictionary");
    }

    public void AddItemPos(Transform t)
    {
        MeshRenderer mr = t.GetComponent<MeshRenderer>();
        if (mr == null) return;
        var pos = t.position;
        positionDictionaryList.Add(pos, t);
    }

    private void InitPosDict2()
    {
        foreach (var t in dict.Keys)
        {
            AddItemPos2(t);
        }
        positionDictionaryList2.ShowCount("TransformDictionary2");
    }

    public void AddItemPos2(Transform t)
    {
        MeshRenderer mr = t.GetComponent<MeshRenderer>();
        if (mr == null) return;
        var pos = MeshRendererInfo.GetCenterPos(t.gameObject);
        positionDictionaryList2.Add(pos, t);
    }

    int uidCount = 0;

    private void InitNameDict()
    {
        int allCount = dict.Count;

        StringBuilder errorInfo = new StringBuilder();
        foreach (var t in dict.Keys)
        {
            AddItem(t, errorInfo);
        }
        if (errorInfo.Length > 0)
        {
            Debug.LogWarning(errorInfo.ToString());
        }
        Debug.Log($"TransformDictionary allCount:{allCount} nameCount:{nameListDict1.Count} uidCount:{uidCount}");
    }

    public void AddItemEx(Transform t)
    {
        AddItem(t,null);
        AddItemPos(t);
        AddItemPos2(t);
    }

    public void AddItem(Transform t, StringBuilder errorInfo)
    {
        string n = t.name;
        if (n.Contains("_New3"))
        {
            n = n.Replace("_New3", "");
            t.name = n;
        }
        else if (n.Contains("_New2"))
        {
            n = n.Replace("_New2", "");
            t.name = n;
        }
        else if (n.Contains("_New"))
        {
            n = n.Replace("_New", "");
            t.name = n;
        }

        nameListDict0.AddItem(n, t);

        if (n.Contains(" "))
        {
            int id = n.LastIndexOf(" ");
            n = n.Substring(0, id);
        }

        string prefix = TransformHelper.GetPrefix(n);

        nameListDict1.AddItem(n, t);
        nameListDict2.AddItem(prefix, t);

        if (IsUID(n, errorInfo))
        {
            uidCount++;
            uidListDict.AddItem(n, t);
        }
    }

    public void CheckUidRepeated()
    {
        int repeatedCount = 0;
        List<string> repeatedUid = new List<string>();
        foreach(var uid in uidListDict.Keys)
        {
            var list = uidListDict[uid];
            if (list.Count > 1)
            {
                repeatedCount++;
                repeatedUid.Add(uid);
            }
            else
            {
                uidDict.Add(uid, list[0]);
            }
        }
        repeatedUid.Sort();
        string txt = "";
        foreach(var uid in repeatedUid)
        {
            txt += uid + ";\t";
        }
        if(repeatedCount>0)
            Debug.LogError($"[TransformDictionary.CheckUidRepeated] repeatedCount:{repeatedCount} repeatedUid:{txt}");
    }


    public static bool IsKKS(string n)
    {
        //J0QFF10AA401
        //HH-2-0LDB-11 12
        //20LDB10BR001 12
        //20LDB10BR 9

        //if(n.Contains("_"))
        int l = n.Length;
        if (l < 9 || l > 12)
        {
            return false;
        }
        if (IsNumAndUpperEnCh(n))
        {
            return true;
        }
        else
        {
            return false;
        }

        //return false;
    }

    public static bool IsEnCh(string input)
    {
        string pattern = @"^[A-Za-z]+$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(input);
    }

    public static bool IsNum(string input)
    {
        string pattern = @"^[0-9]+$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(input);
    }

    /// <summary>
    /// ????????????????????????????????????????
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsNumAndUpperEnCh(string input)
    {
        string pattern = @"^[A-Z0-9]+$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(input);
    }

    public static bool IsLetterOrDigit(string s)
    {
        s = s.Replace("-", "");
        return IsNumAndUpperEnCh(s);
    }

    public static bool IsUID(string n, StringBuilder errorInfo=null)
    {
        //            <ItemInfo Id="H????????.nwc_6_433" Name="H????????????????????????" UId="0028-140039-306069491919619191" X="-180.3085" Y="143.6485" Z="7.421375" Type="P3DEquipment" Drawable="true" Visible="0" AreaId="0">

        var length = n.Length;
        bool result = false;

        if (n.StartsWith("PIPE_GEN_DBLPLATE_DN2400")) return false;
        if (length == 30)
        {
            string[] parts = n.Split('-');
            if (parts.Length == 3)
            {
                if (parts[0].Length == 4 && parts[1].Length == 6 && parts[2].Length == 18)
                {
                    result = true;
                }
                else
                {
                    string error = $"IsUID NotUID s:{n} length:{length} {parts[0].Length},{parts[1].Length},{parts[2].Length}";
                    if (errorInfo != null)
                    {
                        errorInfo.AppendLine(error);
                    }
                    else
                    {
                        Debug.LogWarning(error);
                    }
                }
            }
            else
            {
                string error = $"IsUID NotUID s:{n} length:{length} parts.Length != 3";
                if (errorInfo != null)
                {
                    errorInfo.AppendLine(error);
                }
                else
                {
                    Debug.LogWarning(error);
                }
            }
        }
        return result;
    }

    public TransformDictionary()
    {

    }

    public Transform GetTransformByName(string n)
    {
        //if (nameListDict.ContainsKey(n))
        //{
        //    var list = nameListDict[n];
        //    if (list.Count == 1)
        //    {
        //        return list[0];
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        //return null;

        var list = GetTransformsByName(n);
        if (list.Count == 1)
        {
            return list[0];
        }
        else
        {
            return null;
        }
    }

    public List<Transform> GetTransformsByName(string n)
    {
        if (nameListDict0.ContainsKey(n))
        {
            var list = nameListDict0[n];
            return list;
        }

        if (nameListDict1.ContainsKey(n))
        {
            var list = nameListDict1[n];
            return list;
        }

        if (n.Contains(" "))
        {
            int id = n.LastIndexOf(" ");
            n = n.Substring(0, id);
        }
        if (nameListDict1.ContainsKey(n))
        {
            var list = nameListDict1[n];
            return list;
        }

        //if (n.Contains(" ")||n.Contains("*"))
        if (n.Contains("*"))
        {
            string n2 = n.Replace(" ", "_");
            n2 = n2.Replace("*", "_x_");
            if (nameListDict1.ContainsKey(n2))
            {
                var list = nameListDict1[n2];
                return list;
            }
        }


        return new List<Transform>();
    }

    public Transform GetTransformByUid(string n)
    {
        if (uidListDict.ContainsKey(n))
        {
            var list = uidListDict[n];
            if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }
        return null;
    }

    public Transform GetTransformByName_Debug(string n)
    {
        Debug.Log($"GetTransformByName_Debug n:{n}");
        if (nameListDict1.ContainsKey(n))
        {
            var list = nameListDict1[n];
            Debug.Log($"GetTransformByName_Debug n:{n}");
            if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }
        return null;
    }

    public List<Transform> RemovedItems = new List<Transform>();

    public void RemoveTransform(Transform transform)
    {
        //list.Remove(transform);

        //string n = transform.name;

        if (dict.ContainsKey(transform))
        {
            dict.Remove(transform);
            RemovedItems.Add(transform);
            positionDictionaryList.Remove(transform.position, transform);

            var center = MeshRendererInfo.GetCenterPos(transform.gameObject);
            positionDictionaryList2.Remove(center, transform);

            string n = transform.name;
            if(n.Contains(" "))
            {
                int id = n.LastIndexOf(" ");
                n = n.Substring(0, id);
            }
            string prefix = TransformHelper.GetPrefix(n);
            nameListDict1.RemoveItem(n, transform);
            nameListDict2.RemoveItem(prefix, transform);
            if (IsUID(n))
            {
                uidListDict.RemoveItem(n, transform);
            }
        }
        else
        {
            if (RemovedItems.Contains(transform))
            {
                Debug.LogWarning("TransformDictionary.RemoveTransform Already Removed :" + transform);
            }
            else
            {
                Debug.LogWarning("TransformDictionary.RemoveTransform Not Contains :" + transform);
            }
            
        }
    }

    internal Transform FindObjectByUID(string uId)
    {
        return GetTransformByName(uId);
    }

    internal Transform FindObjectByPos(ModelItemInfo model)
    {
        Vector3 pos = model.GetPosition();
        int listId = 0;
        var t=positionDictionaryList.GetItem(pos, out listId);
        return t;
    }

    public List<Transform> FindModelsByPosAndName(ModelItemInfo model)
    {
        Vector3 pos = model.GetPosition();
        int listId = 0;
        //var t = positionDictionaryList.GetItem(pos, out listId);
        //return t;

        var ms = positionDictionaryList.GetItems(pos, out listId);
        if (ms == null) return null;
        if (ms.Count == 0) return null;

        if (ms.Count == 1)
        {
            return ms;
        }
        else
        {
            List<Transform> sameNameList = new List<Transform>();
            foreach (var m in ms)
            {
                if (model.IsSameName(m))
                {
                    sameNameList.Add(m);
                }
            }
            if (sameNameList.Count == 0)
            {
                return ms;
            }
            else if (sameNameList.Count == 1)
            {
                return sameNameList;
            }
            else
            {

                //List<Transform> sameParentNameList = new List<Transform>();
                //foreach (var m in sameNameList)
                //{
                //    string tParentName = m.parent.name;
                //    string mParentName = model.GetParent().Name;
                //    if (tParentName== mParentName)
                //    {
                //        sameParentNameList.Add(m);
                //    }
                //}
                //if (sameParentNameList.Count == 1)
                //{
                //    return sameParentNameList[0];
                //}
                //else
                //{
                //    return null;
                //}

                //return ms;

                return sameNameList;
            }
        }
    }

    public List<Transform> FindModelsByPosAndName(IdInfo model)
    {
        Vector3 pos = model.GetPosition();
        int listId = 0;
        //var t = positionDictionaryList.GetItem(pos, out listId);
        //return t;

        var ms = positionDictionaryList.GetItems(pos, out listId);
        if (ms == null) return null;
        if (ms.Count == 0) return null;

        if (ms.Count == 1)
        {
            return ms;
        }
        else
        {
            List<Transform> sameNameList = new List<Transform>();
            foreach (var m in ms)
            {
                //if (model.IsSameName(m))
                if(m.name==model.name)
                {
                    sameNameList.Add(m);
                }
            }
            if (sameNameList.Count == 0)
            {
                return ms;
            }
            else if (sameNameList.Count == 1)
            {
                return sameNameList;
            }
            else
            {

                //List<Transform> sameParentNameList = new List<Transform>();
                //foreach (var m in sameNameList)
                //{
                //    string tParentName = m.parent.name;
                //    string mParentName = model.GetParent().Name;
                //    if (tParentName== mParentName)
                //    {
                //        sameParentNameList.Add(m);
                //    }
                //}
                //if (sameParentNameList.Count == 1)
                //{
                //    return sameParentNameList[0];
                //}
                //else
                //{
                //    return null;
                //}

                //return ms;

                return sameNameList;
            }
        }
    }

    //public List<Transform> FindModelsByPosAndName(Vector3 pos, Vector3 center, string name, bool hasMesh, bool isFindByName)
    public List<Transform> FindModelsByPosAndName(IdInfo idInfo,bool isFindByName)
    {
        Vector3 pos = idInfo.GetPosition();
        Vector3 center = idInfo.GetCenter();
        string name = idInfo.name;
        bool hasMesh = idInfo.HasMesh;
        //Vector3 pos = model.GetPosition();
        int listId = 0;
        //var t = positionDictionaryList.GetItem(pos, out listId);
        //return t;
        List<Transform> posItemList = null;
        if (hasMesh)
        {
            posItemList = positionDictionaryList.GetItems(pos, out listId);

            if (posItemList == null || posItemList.Count == 0)
            {
                posItemList = positionDictionaryList2.GetItems(center, out listId);
            }

            List<Transform> posItemList2 = positionDictionaryList2.GetItems(center, out listId);
            if (posItemList2 != null)
            {
                if (posItemList == null)
                {
                    posItemList = posItemList2;
                }
                else
                {
                    foreach (var item2 in posItemList2)
                    {
                        if (!posItemList.Contains(item2))
                        {
                            posItemList.Add(item2);
                        }
                    }
                } 
            }

            List<Transform> nameItemList = GetTransformsByName(name);
            if (nameItemList != null)
            {
                if (posItemList == null)
                {
                    posItemList = nameItemList;
                }
                else
                {
                    foreach (var item2 in nameItemList)
                    {
                        if (!posItemList.Contains(item2))
                        {
                            posItemList.Add(item2);
                        }
                    }
                }
            }
        }

        //var posItemList = positionDictionaryList2.GetItems(center, out listId); 
        //if (posItemList == null || posItemList.Count == 0)
        //{
        //    posItemList = positionDictionaryList.GetItems(pos, out listId);
        //}

        if (posItemList == null)
        {
            List<Transform> nameItemList = GetTransformsByName(name);

            if (hasMesh == false)
            {
                List<Transform> nameItemList2 = GetTransformsByName(idInfo.parent+"_"+name);
                nameItemList.AddRange(nameItemList2);
            }

            if (nameItemList.Count == 0)
            {
                Debug.LogError($"FindModelsByPosAndName nameItemList.Count != 1 ??{idInfo},path:{idInfo.GetPath()}?? | nameItemList:{nameItemList.Count} ");
                return null;
            }
            else if (nameItemList.Count == 1)
            {
                var nameItem0 = nameItemList[0];

                MeshRenderer mr = nameItem0.GetComponent<MeshRenderer>();
                if(mr==null && hasMesh == false)
                {
                    return nameItemList;
                }

                var disOfPos = Vector3.Distance(pos, nameItem0.position);
                var disOfCenter = Vector3.Distance(center, MeshRendererInfo.GetCenterPos(nameItem0.gameObject));
                if (nameItem0.name == name)
                {
                    if (disOfCenter < 1f)
                    {
                        Debug.LogWarning($"FindModelsByPosAndName Error11 ??{idInfo},path:{idInfo.GetPath()}?? | nameItem0:{nameItem0} disOfPos:{disOfPos} disOfCenter:{disOfCenter}| path:{nameItem0.GetPath()}");
                        return nameItemList;
                    }
                    else
                    {
                        Debug.LogError($"FindModelsByPosAndName Error11 ??{idInfo},path:{idInfo.GetPath()}?? | nameItem0:{nameItem0} disOfPos:{disOfPos} disOfCenter:{disOfCenter}| path:{nameItem0.GetPath()}");
                        return null;
                    }
                }
                else
                {
                    if (disOfCenter < 0.6f)
                    {
                        Debug.LogWarning($"FindModelsByPosAndName Error12 ??{idInfo},path:{idInfo.GetPath()}?? | nameItem0:{nameItem0} disOfPos:{disOfPos} disOfCenter:{disOfCenter}| path:{nameItem0.GetPath()}");
                        return nameItemList;
                    }
                    else
                    {
                        Debug.LogError($"FindModelsByPosAndName Error12 ??{idInfo},path:{idInfo.GetPath()}?? | nameItem0:{nameItem0} disOfPos:{disOfPos} disOfCenter:{disOfCenter}| path:{nameItem0.GetPath()}");
                        return null;
                    }
                }
                
            }
            else
            {
                if (hasMesh == false)
                {
                    Transform t = TransformHelper.FindClosedTransform(nameItemList, pos, false);

                    var disOfPos = Vector3.Distance(pos, t.position);
                    var disOfCenter = Vector3.Distance(center, MeshRendererInfo.GetCenterPos(t.gameObject));
                    if (disOfCenter < 0.6f)
                    {
                        Debug.LogWarning($"FindModelsByPosAndName Error21 ??{idInfo},path:{idInfo.GetPath()}?? | t:{t} disOfPos:{disOfPos} disOfCenter:{disOfCenter} | path:{t.GetPath()}");
                        return new List<Transform>() { t };
                    }
                    else
                    {
                        //Debug.LogError($"FindModelsByPosAndName Error11 ??{idInfo},path:{idInfo.GetPath()}?? | t:{t} disOfPos:{disOfPos} disOfCenter:{disOfCenter}| path:{TransformHelper.GetPath(t)}");
                        IdInfo idP1 = idInfo.pId;
                        if (idP1 != null)
                        {
                            IdInfo idP2 = idP1.pId;
                            if (idP2 != null)
                            {
                                Transform result0 = null;
                                for (int i = 0; i < nameItemList.Count; i++)
                                {
                                    Transform item = nameItemList[i];
                                    //Debug.LogError($"FindModelsByPosAndName[{i}] Error11 item:{item.name} path:{TransformHelper.GetPath(item)}");
                                    Transform tp1 = item.parent;
                                    if (tp1 != null)
                                    {
                                        Transform tp2 = tp1.parent;
                                        if (tp2 != null)
                                        {
                                            if(tp2.name.Contains(idP2.name) && tp1.name.Contains(idP1.name))
                                            {
                                                //Debug.LogError($"--FindModelsByPosAndName[{i}] Found!! item:{item.name} path:{TransformHelper.GetPath(item)} ??{idInfo},path:{idInfo.GetPath()}??");
                                                //return new List<Transform>() { item };
                                                result0 = item;
                                                break;
                                            }
                                        }
                                        else
                                        {

                                        }
                                    }
                                    else
                                    {

                                    }
                                }

                                if (result0 != null)
                                {
                                    return new List<Transform>() { result0 };
                                }
                                else
                                {
                                    for (int i = 0; i < nameItemList.Count; i++)
                                    {
                                        Transform item = nameItemList[i];
                                        Debug.LogError($"FindModelsByPosAndName[{i}] Error11 item:{item.name} path:{item.GetPath()}");
                                    }
                                    Debug.LogError($"FindModelsByPosAndName Error22 ??{idInfo},path:{idInfo.GetPath()}?? | t:{t} disOfPos:{disOfPos} disOfCenter:{disOfCenter}| path:{t.GetPath()}");
                                    return null;
                                }
                            }
                            else
                            {
                                Debug.LogWarning($"FindModelsByPosAndName Error23 idP2==null ??{idInfo},path:{idInfo.GetPath()}?? | t:{t} disOfPos:{disOfPos} disOfCenter:{disOfCenter} | path:{t.GetPath()}");
                                return null;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < nameItemList.Count; i++)
                            {
                                Transform item = nameItemList[i];
                                Debug.LogError($"FindModelsByPosAndName[{i}] Error24 idP1==null item:{item.name} path:{item.GetPath()}");
                                Transform tp1 = item.parent;
                                Transform tp2 = tp1.parent;

                            }
                            return null;
                        }
                    }


                }
                else
                {
                    if (isFindByName)
                    {
                        Transform t = TransformHelper.FindClosedTransform(nameItemList, pos, true);

                        var disOfPos = Vector3.Distance(pos, t.position);
                        var disOfCenter = Vector3.Distance(center, MeshRendererInfo.GetCenterPos(t.gameObject));
                        if (disOfCenter < 0.6f)
                        {
                            Debug.LogWarning($"FindModelsByPosAndName Error31 ??{idInfo},path:{idInfo.GetPath()}?? | t:{t} disOfPos:{disOfPos} disOfCenter:{disOfCenter}| path:{t.GetPath()}"); ;
                            return new List<Transform>() { t };
                        }
                        else
                        {
                            Debug.LogError($"FindModelsByPosAndName Error32 ??{idInfo},path:{idInfo.GetPath()}?? | t:{t} disOfPos:{disOfPos} disOfCenter:{disOfCenter}| path:{t.GetPath()}");
                            return null;
                        }
                    }
                    else
                    {
                        Debug.LogError($"FindModelsByPosAndName Error33 nameItemList.Count != 1 ??{idInfo},path:{idInfo.GetPath()}?? | nameItemList:{nameItemList.Count} ");
                        return null;
                    }
                }

               
            }
        }

        if (posItemList.Count == 0)
        {
            return posItemList;
        }
        else if (posItemList.Count == 1)
        {
            return posItemList;
        }
        else
        {
            List<Transform> sameNameList = new List<Transform>();
            foreach (var m in posItemList)
            {
                //if (model.IsSameName(m))
                if (m.name == name)
                {
                    sameNameList.Add(m);
                }
            }
            if (sameNameList.Count == 0)
            {
                return posItemList;
            }
            else if (sameNameList.Count == 1)
            {
                return sameNameList;
            }
            else
            {

                //List<Transform> sameParentNameList = new List<Transform>();
                //foreach (var m in sameNameList)
                //{
                //    string tParentName = m.parent.name;
                //    string mParentName = model.GetParent().Name;
                //    if (tParentName== mParentName)
                //    {
                //        sameParentNameList.Add(m);
                //    }
                //}
                //if (sameParentNameList.Count == 1)
                //{
                //    return sameParentNameList[0];
                //}
                //else
                //{
                //    return null;
                //}

                //return ms;

                return sameNameList;
            }
        }
    }
}

//public class DictionaryList1ToN<T>: DictionaryList1ToN<string, T>// where T :class
//{
    
//}

//public class Key2List<T1,T2>:IComparable<Key2List<T1, T2>>
//{
//    public T1 Key;
//    public List<T2> List;

//    public int Count
//    {
//        get
//        {
//            if (List == null) return -1;
//            return List.Count;
//        }
//    }

//    public Key2List(T1 key ,List<T2> list)
//    {
//        this.Key = key;
//        this.List = list;
//        //this.Count = list.Count;
//    }

//    public int CompareTo(Key2List<T1, T2> other)
//    {
//        return other.Count.CompareTo(this.Count);
//    }
//}

//public class DictionaryList1ToN<T1,T2> : Dictionary<T1, List<T2>> //where T2 : class
//{
//    public List<T2> GetList(T1 key)
//    {
//        if (this.ContainsKey(key))
//        {
//            return this[key];
//        }
//        else
//        {
//            return new List<T2>();
//        }
//    }

//    public int GetListCount(T1 key)
//    {
//        if (this.ContainsKey(key))
//        {
//            return this[key].Count;
//        }
//        else
//        {
//            return 0;
//        }
//    }

//    public List<Key2List<T1,T2>> Key2Lists = new List<Key2List<T1, T2>>();

//    public void AddItem(T1 key, T2 item)
//    {

//        if (!this.ContainsKey(key))
//        {
//            List<T2> list0 = new List<T2>();
//            this.Add(key, list0);
//            Key2Lists.Add(new Key2List<T1, T2>(key, list0));
//        }
//        var list = this[key];
//        if(!list.Contains(item))
//            list.Add(item);

//        //if (item.ToString().Contains("SG0000-Undefined 102") || item.ToString().Contains("FD0000-Undefined 105"))
//        //{
//        //    Debug.LogError($"AddItem item:{item},key:{key} count:{list.Count}");
//        //}
//    }

//    public void RemoveItem(T1 key, T2 item)
//    {
//        var items = GetItems(key);
//        if (items != null)
//        {
//            int count1 = items.Count;
//            items.Remove(item);
//            int count2 = items.Count;
//            if (count1 == count2)
//            {
//                Debug.LogError($"DictionaryList1ToN RemoveItem NotContainsItem key:{key} item:{item}");
//            }

//            if (count2 == 0)
//            {
//                this.Remove(key);
//            }
//        }
//        else
//        {
//            Debug.LogError($"DictionaryList1ToN RemoveItem NotContainsKey key:{key} item:{item}");
//        }

//        //if (item.ToString().Contains("SG0000-Undefined 102") || item.ToString().Contains("FD0000-Undefined 105"))
//        //{
//        //    Debug.LogError($"RemoveItem item:{item},key:{key} count:{items.Count}");
//        //}
//    }

//    public T2 GetItem(T1 key)
//    {
//        if (this.ContainsKey(key))
//        {
//            var list = this[key];
//            if (list.Count == 1)
//            {
//                return list[0];
//            }
//            else
//            {
//                return default(T2);
//            }
//        }
//        return default(T2);
//    }

//    public List<T2> GetItems(T1 key)
//    {
//        if (this.ContainsKey(key))
//        {
//            var list = this[key];
//            return list;
//        }
//        return null;
//    }

//    public List<Key2List<T1, T2>> GetListSortByCount()
//    {
//        Key2Lists.Sort();
//        //List<T1> keys = new List<T1>();
//        //foreach(var item in Key2Lists)
//        //{
//        //    keys.Add(item.Key);
//        //}
//        //return keys;
//        return Key2Lists;
//    }
//}
