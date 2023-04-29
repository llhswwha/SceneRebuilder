using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Y_UIFramework;

public class MeshSelection : MonoBehaviour
{
    public static MeshSelection Instance;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //if (camera == null)
        //{
        //    camera = GameObject.FindObjectOfType<Camera>();
        //}
    }

    public Camera camera;

    public string testRIdText = "";

    public RendererId testRId = null;

    public void TestSelectObjectByRId()
    {
        string rId = testRIdText;
        if (string.IsNullOrEmpty(rId) && testRId != null)
        {
            rId = testRId.Id;
        }

        //DateTime 
        SelectObjectByRId(rId,go=>
        {
            if (go != null)
            {
                Debug.LogError($"SelectObjectByRId rId:{rId} result:{go} path:{go.transform.GetPath()}");
            }
            else
            {
                Debug.LogError($"SelectObjectByRId rId:{rId} result:{go}");
            }
            
        });
    }

    public static void SelectObjectsByRIdList(List<string>idList,Action<Dictionary<string,GameObject>>callback)
    {
        Dictionary<string, GameObject> dicT = new Dictionary<string, GameObject>();
        if(idList==null||idList.Count==0)
        {
            if (callback != null) callback(dicT);
            return;
        }
        int callBackCount = 0;
        for(int i=0;i<idList.Count;i++)
        {
            string rid = idList[i];
            bool isLast = i == idList.Count - 1;
            int tempIndex = i;
            Debug.LogFormat("WK.1.rid:{0} i:{1} isLast:{2}",rid,i,isLast);
            SelectObjectByRId(rid, obj=> 
            {
                callBackCount++;
                if (obj != null && !dicT.ContainsKey(rid))
                {
                    dicT.Add(rid, obj);
                }
                Debug.LogFormat("WK.2 rid:{0} Obj:{1} isLast:{2} TempIndex:{3}",rid,obj==null?"Null":obj.name,isLast,tempIndex);
                //if(isLast&& callback != null) callback(dicT);

                //不能用最后一个Index判断，必须等所有rid处理完毕，再执行回调
                if(callBackCount==idList.Count)
                {
                    if (callback != null) callback(dicT);
                }
            });
        }
    }

    public static AreaTreeNode LastSelectNode = null;

    public static void SelectObjectByRId(string id,Action<GameObject> callback, DevInfo devInfo=null)
    {
        //Debug.Log($"SelectObjectByRId rId:{rId}");
        //if (string.IsNullOrEmpty(rId))
        //{
        //    Debug.LogError("SelectObjectByRId string.IsNullOrEmpty(rId)");
        //    if (callback != null)
        //    {
        //        callback(null);
        //    }
        //}
        //IdDictionary.Inid
        DateTime start = DateTime.Now;
        RendererId rId = IdDictionary.GetId(id,null,true,false);

        if (rId == null)
        {
            Debug.LogError($"SelectObjectByRId rId == null id:{id}");
        }
        else
        {
            Debug.Log($"SelectObjectByRId  id:{id} rId:{rId.Id} name:{rId.name}");
        }

        if (rId == null)
        {
            var node = AreaTreeHelper.GetNodeById(id);

            if(node!=null && LastSelectNode == node)
            {
                Debug.LogError($"MeshSelection LastSelectNode== node id:{id} node:{node} path{node.transform.GetPath()}");
                GameObject go = IdDictionary.GetGo(id);
                if (callback != null) callback(go);
                return;
            }

            if (LastSelectNode != null)
            {
                SubSceneShowManager.Instance.AddScenes(LastSelectNode.GetScenes());
            }

            LastSelectNode = node;

            if (LastSelectNode != null)
            {
                SubSceneShowManager.Instance.RemoveScenes(LastSelectNode.GetScenes());
            }

            if (node != null)
            {
                Debug.Log($"SelectObjectByRId id:{id} node:{node} path:{node.transform.GetPath()}");
                node.LoadAndSwitchToRenderers(b =>
                {
                    GameObject go = IdDictionary.GetGo(id);
                    if (go == null)
                    {
                        Debug.LogError($"SelectObjectByRId go==null id:{id} node:{node}");
                        if (callback != null)
                        {
                            callback(null);
                        }
                    }
                    else
                    {
                        Debug.Log($"SelectObjectByRId time:{(DateTime.Now - start).TotalMilliseconds}ms go:{go} id:{id} node:{node} ");
                        if (callback != null)
                        {
                            callback(go);
                        }
                    }
                });
            }
            else
            {
                if (devInfo != null)
                {
                    string bimUid = devInfo.Abutment_DevID;
                    BIMModelInfo bimModel = BIMModelInfo.GetBIMModel(bimUid);
                    if (bimModel != null)
                    {
                        GameObject go = bimModel.gameObject;

                        if (go == null)
                        {
                            if (devInfo.Pos != null)
                            {
                                Debug.LogError($"SelectObjectByRId1 node==null id:{id} devInfo:{devInfo.Name},[id:{devInfo.DevID},{devInfo.Abutment_DevID},{devInfo.Abutment_Id},{devInfo.Local_CabinetID}],pos:({devInfo.Pos.PosX},{devInfo.Pos.PosY},{devInfo.Pos.PosZ})");
                            }
                            else
                            {
                                Debug.LogError($"SelectObjectByRId2 node==null id:{id} devInfo:{devInfo.Name},[id:{devInfo.DevID},{devInfo.Abutment_DevID},{devInfo.Abutment_Id},{devInfo.Local_CabinetID}],{devInfo.Pos}");
                            }
                            if (callback != null)
                            {
                                callback(null);
                            }
                        }
                        else
                        {
                            Debug.Log($"SelectObjectByRId3 bimUid:{bimUid} time:{(DateTime.Now - start).TotalMilliseconds}ms go:{go} id:{id} node:{node} ");
                            if (callback != null)
                            {
                                callback(go);
                            }
                        }
                    }
                    else
                    {
                        if (devInfo.Pos != null)
                        {
                            Debug.LogError($"SelectObjectByRId4 node==null id:{id} devInfo:{devInfo.Name},[id:{devInfo.DevID},{devInfo.Abutment_DevID},{devInfo.Abutment_Id},{devInfo.Local_CabinetID}],pos:({devInfo.Pos.PosX},{devInfo.Pos.PosY},{devInfo.Pos.PosZ})");
                        }
                        else
                        {
                            Debug.LogError($"SelectObjectByRId5 node==null id:{id} devInfo:{devInfo.Name},[id:{devInfo.DevID},{devInfo.Abutment_DevID},{devInfo.Abutment_Id},{devInfo.Local_CabinetID}],{devInfo.Pos}");
                        }
                        if (callback != null)
                        {
                            callback(null);
                        }
                    }
                }
                else
                {
                    Debug.LogError($"SelectObjectByRId6 node==null id:{id} devInfo:{devInfo}");
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                
                
            }
        }
        else if (rId.childrenIds.Count == 0)
        {
            if(rId.mr==null)
            {
                rId.mr = rId.transform.GetComponent<MeshRenderer>();
            }
            if (rId.mr != null)
            {
                var node = AreaTreeHelper.GetNodeById(id);
                //Debug.LogError($"SelectObjectByRId rId == null id:{id}");
                Debug.Log($"SelectObjectByRId time:{(DateTime.Now - start).TotalMilliseconds}ms go:{rId.gameObject} id:{id} node:{node} ");
                if (callback != null)
                {
                    callback(rId.gameObject);
                }
            }
            else
            {
                var rIds = rId.GetComponentsInChildren<RendererId>(true);
                var nodes = AreaTreeHelper.GetNodesByChildrens(rIds);
                if(nodes==null||nodes.Count==0)
                {
                    if (callback != null)
                    {
                        callback(rId.gameObject);
                    }
                }
                else
                {
                    SelectObjectByRId(id, callback, start, nodes);
                }                
            }
          
        }
        else //if (rId.childrenIds.Count > 0)
        {
            var nodes = AreaTreeHelper.GetNodesByChildrens(rId);
            SelectObjectByRId(id, callback, start, nodes);
        }

    }

    private static void SelectObjectByRId(string id, Action<GameObject> callback, DateTime start, List<AreaTreeNode> nodes)
    {
        var scenes = SubSceneHelper.GetScenes(nodes);
        if(scenes==null||scenes.Count==0)
        {
            Debug.LogError($"SelectObjectByRId go==null id:{id} nodes:{nodes.Count}");
            if (callback != null)
            {
                callback(null);
            }
            return;
        }
        Debug.Log($"SelectObjectByRId nodes:{nodes.Count} scenes:{scenes.Count} scene1:{scenes[0]} id:{id} ");
        SubSceneShowManager.Instance.LoadScenes(scenes, p =>
        {
            foreach(var node in nodes)
            {
                node.LoadRenderers();
                node.SwitchToRenderers();
            }
            if (p.isAllFinished)
            {
                GameObject go = IdDictionary.GetGo(id);
                if (go == null)
                {
                    Debug.LogError($"SelectObjectByRId go==null id:{id}");
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Debug.Log($"SelectObjectByRId time:{(DateTime.Now - start).TotalMilliseconds:F1}ms go:{go} id:{id} ");
                    if (callback != null)
                    {
                        callback(go);
                    }
                }
            }
        }, "SelectObjectByRId");
    }

    public bool IsUpdateCombined = false;

    private void HitTest(int count)
    {
        if (IsClickUGUIorNGUI.Instance&&(IsClickUGUIorNGUI.Instance.isOverUI|| IsClickUGUIorNGUI.Instance.isClickedUI)) return;
        //Debug.Log("HitTest:" + count);
        if (count > 2)
        {
            return;
        }
#if UNITY_INPUTSYSTEM
         Vector3 mousePos = Mouse.current.position.ReadValue(); ;
#else
          Vector3 mousePos = Input.mousePosition;
#endif
        if (camera == null) return;
        var ray = camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var hitGo = hit.collider.gameObject;
            string hitParentName = "";
            if (hitGo.transform.parent!=null)
                hitParentName = hitGo.transform.parent.name;
            Debug.Log($"Hit Object[{count}]:{hitGo}|{hitGo.transform.parent}");
            BuildingController building = hitGo.GetComponent<BuildingController>();
            if (building != null)
            {
                FloorController[] floors = building.GetComponentsInChildren<FloorController>(true);
                if (floors.Length == 0)
                {
                    Collider collider = hitGo.GetComponent<Collider>();
                    if (collider != null)
                    {
                        collider.enabled = false;
                        HitTest(count + 1);
                    }
                }
            }

            if (hitGo.name== "Culling Collider")
            {
                hitGo = hitGo.transform.parent.gameObject;
            }

            if (lastRenderer != null)
            {
                //lastRenderer.material=lastRendererMat;
            }

            lastRenderer = hitGo.GetComponent<MeshRenderer>();

            if (lastRenderer)
            {
                if(hitParentName.EndsWith("_Combined"))
                {
                    if (!AreaTreeHelper.combined2NodeDict.ContainsKey(lastRenderer))
                    {
                        var treeNode = lastRenderer.GetComponentInParent<AreaTreeNode>();
                        Debug.LogError($"FindTreeNode renderer:{lastRenderer} node:{treeNode}");
                        if (treeNode)
                        {
                            treeNode.CreateDictionary();
                        }
                    }
                    var treeNode0 = AreaTreeHelper.GetNodeByCombined(lastRenderer);
                    if (treeNode0!=null)//点中了合并的模型
                    {
                        var treeNode1 = treeNode0;
                        Debug.LogWarning("Hit Tree Node1:" + treeNode1);
                        if (lastNode != null && lastNode != treeNode1)
                        {
                            lastNode.UpdateCombined();//如果发生修改，则更新合并的模型
                            lastNode.SwitchToCombined();
                        }
                        lastNode = treeNode1;
                        //lastNode.SwitchToRenderers();
                        //HitTest(count + 1);//重新点击

                        lastNode.LoadAndSwitchToRenderers(b =>
                        {
                            HitTest(count + 1);//重新点击
                        });
                    }
                    else
                    {
                        Debug.LogError("Hit Tree Node Error :" + lastRenderer);
                        //lastRendererMat = lastRenderer.material;
                        //lastRenderer.material = selectedMat;
                        var treeNode2 = AreaTreeHelper.GetNodeByRenderer(lastRenderer);
                        if (treeNode2!=null)
                        {
                            Debug.LogError("Hit Tree Node2:" + treeNode2);
                            if (lastNode != null && lastNode != treeNode2)
                            {
                                if(IsUpdateCombined)
                                    lastNode.UpdateCombined();//如果发生修改，则更新合并的模型
                                lastNode.SwitchToCombined();
                            }
                            lastNode = treeNode2;
                            lastNode.SwitchToRenderers();
                        }
                        else
                        {
                            Debug.LogError("No TreeNode :" + lastRenderer);
                        }
                    }
                }
                else
                {
                    SelectBimModel(lastRenderer.transform);
                }
                //lastRendererMat = lastRenderer.material;
                //lastRenderer.material = selectedMat;
                //if (AreaTreeHelper.render2NodeDict.ContainsKey(lastRenderer))
                //{
                //    var treeNode = AreaTreeHelper.render2NodeDict[lastRenderer];
                //    Debug.LogError("Hit Node:" + treeNode);
                //    if (lastNode != null && lastNode != treeNode)
                //    {
                //        lastNode.UpdateCombined();//如果发生修改，则更新合并的模型
                //        lastNode.SwitchToCombined();
                //    }
                //    lastNode = treeNode;
                //    lastNode.SwitchToRenderers();
                //}
                //else
                //{
                //    Debug.LogError("No TreeNode :" + lastRenderer);
                //}
            }
            else
            {
                if(hitGo.GetComponent<BIMModelInfo>())SelectBimModel(hitGo.transform);
            }
        }
        else
        {
            if (IsShowDebugLog)
            {
                Debug.LogError($"Hit NULL lastRenderer:{lastRenderer}");
            }


            ClearLastRenderer();
        }
    }
    private void ClearLastRenderer()
    {
        if (lastRenderer != null)
        {
            if (lastRendererMat != null)
            {
                lastRenderer.sharedMaterial = lastRendererMat;
            }
            else
            {
                Debug.LogError($"ClearLastRenderer lastRendererMat == null lastRenderer:{lastRenderer}");
            }
            GPUInstanceTest.SAddPrefabInstance(lastRenderer);
        }
    }
    private void SelectBimModel(MeshRenderer hitRenderer)
    {
        if (hitRenderer == null) return;

        ClearLastRenderer();

        lastRendererMat = hitRenderer.sharedMaterial;
        lastScale = hitRenderer.transform.localScale;

        hitRenderer.sharedMaterial = selectedMat;
        hitRenderer.enabled = true;

        lastRenderer = hitRenderer;

        GPUInstanceTest.SRemovePrefabInstance(hitRenderer.gameObject);
    }

    public bool IsShowDebugLog = false;

    private void SelectBimModel(Transform rayModel)
    {
        if (IsClickUGUIorNGUI.Instance && IsClickUGUIorNGUI.Instance.isOverUI) return;
        //if (DevSubsystemManage.Instance && DevSubsystemManage.Instance.RoamToggle.isOn) return;

        Action<ViewState> callback = stateT =>
        {
            if (stateT == ViewState.设备定位||stateT==ViewState.移动巡检)
            {
                
                //lastRendererMat = lastRenderer.material;
                //lastRenderer.material = selectedMat;
                if (rayModel != null)
                {
                    //Debug.Log($"SelectBimModel name:{rayModel.name} path:{rayModel.GetPath()}");
                    BIMModelInfo bimInfo=rayModel.GetComponent<BIMModelInfo>();
                    if (bimInfo!= null)
                    {
                        Debug.Log($"SelectBimModel>SelectDevOnClick1 [bimInfo:{bimInfo}] name:{rayModel.name} path:{rayModel.GetPath()}");
                        bimInfo.SelectDevOnClick(false);
                    }
                    else{
                        
                        if (rayModel.parent != null)
                        {
                            
                            Transform parent = rayModel.parent;
                            //RendererId rid0 = rayModel.GetComponent<RendererId>();
                            //RendererId ridP1 = parent.GetComponent<RendererId>();
                            RendererId rid0 = RendererId.GetRId(rayModel.gameObject);
                            RendererId ridP1 = RendererId.GetRId(parent.gameObject);
                            if (rid0 != null && ridP1 != null)
                            {
                                if (IsShowDebugLog)
                                {
                                    Debug.LogError($"SelectBimModel rid0:{rid0.Id} ridP1:{ridP1.Id} name:{rayModel.name} path:{rayModel.GetPath()}");
                                }
                                if (rid0.parentId != ridP1.Id)
                                {
                                    rid0.RecoverParent();
                                    parent = rayModel.parent;
                                }
                            }
                            else
                            {
                                if (IsShowDebugLog)
                                {
                                    Debug.LogError($"SelectBimModel rid0==null||ridP1==null name:{rayModel.name} path:{rayModel.GetPath()}");
                                }
                            }

                            if (parent!=null && parent.GetComponent<BIMModelInfo>() != null)
                            {
                                if (rayModel.name.Contains("Culling") || rayModel.name.Contains("Geometry") || rayModel.name.Contains("_LOD"))
                                {
                                    if (IsShowDebugLog)
                                    {
                                        Debug.Log($"SelectBimModel>SelectDevOnClick2 name:{rayModel.name} parent:{parent.name} path:{rayModel.GetPath()}");
                                    }
                                    parent.GetComponent<BIMModelInfo>().SelectDevOnClick(false);
                                }
                                else
                                {
                                    parent.GetComponent<BIMModelInfo>().SelectDevOnClick(false);
                                    if (IsShowDebugLog)
                                    {
                                        Debug.LogWarning($"SelectBimModel>SelectDevOnClick3 [Not Culling Or Geometry] name:{rayModel.name} parent:{parent.name} path:{rayModel.GetPath()}");
                                    }
                                }
                            }
                            else
                            {
                                if (IsShowDebugLog)
                                {
                                    Debug.LogError($"SelectBimModel BIMModelInfo==null name:{rayModel.name} path:{rayModel.GetPath()}");
                                }
                                
                            }
                        }
                        else
                        {
                            if (IsShowDebugLog)
                            {
                                Debug.LogError($"SelectBimModel parent==null name:{rayModel.name} path:{rayModel.GetPath()}");
                            }
                        }
                    }                                             
                }
                else
                {
                    if (IsShowDebugLog)
                    {
                        Debug.LogError("SelectBimModel rayModel==null!!");
                    }
                }
            }
        };
        MessageCenter.SendMsg(MsgType.ModuleToolbarMsg.TypeName, MsgType.ModuleToolbarMsg.GetCurrentState, callback);
    }

    private void ClearLastInfo()
    {
        UIManager.GetInstance().CloseUIPanels(typeof(DeviceDocumentationbar).Name);
    }
    private void highlightObj(GameObject obj)
    {
        if(obj!=null&&obj.activeInHierarchy)
        {
            HightlightModuleBase.ClearHighlightOff();
            HightlightModuleBase hightlightT = obj.AddMissingComponent<HightlightModuleBase>();
            hightlightT.ConstantOn(Color.green);
        }
    }

    public void Update()
    {
        bool mouseUp = false;
#if UNITY_INPUTSYSTEM
        mouseUp = Mouse.current.leftButton.wasReleasedThisFrame;
#else
        mouseUp =Input.GetMouseButtonUp(0);
#endif
        if (mouseUp && enableSelection)
        {
            if(camera==null){
                camera=Camera.main;
            }
            if (camera == null)
            {
                camera = GameObject.FindObjectOfType<Camera>();
            }
            HitTest(0);
        }

    }

    public AreaTreeNode lastNode;

    public MeshRenderer lastRenderer;

    public Material lastRendererMat;

    public Vector3 lastScale = Vector3.one;

    public Material selectedMat;

    public bool enableSelection;
}
