using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//using Y_UIFramework;

public class MeshSelection : MonoBehaviour
{
    
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
            Debug.LogError($"SelectObjectByRId result:{go}");
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
        for(int i=0;i<idList.Count;i++)
        {
            string rid = idList[i];
            SelectObjectByRId(rid, obj=> 
            {
                if (obj!=null&&!dicT.ContainsKey(rid)) dicT.Add(rid,obj);

                if(i==idList.Count-1)
                {
                    if (callback != null) callback(dicT);
                }
            });
        }
    }

    public static void SelectObjectByRId(string id,Action<GameObject> callback)
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
        RendererId rId = IdDictionary.GetId(id);
        if (rId == null)
        {
            var node = AreaTreeHelper.GetNodeById(id);
            if (node != null)
            {
                Debug.Log($"SelectObjectByRId id:{id} node:{node}");
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
                Debug.LogError($"SelectObjectByRId node==null id:{id}");
                if (callback != null)
                {
                    callback(null);
                }
            }
        }
        else if (rId.childrenIds.Count == 0)
        {
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
                SelectObjectByRId(id, callback, start, nodes);
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
            Debug.LogError($"SelectObjectByRId go==null id:{id}");
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
        });
    }

    public bool IsUpdateCombined = false;

    private void HitTest(int count)
    {
        //if (IsClickUGUIorNGUI.Instance&&(IsClickUGUIorNGUI.Instance.isOverUI|| IsClickUGUIorNGUI.Instance.isClickedUI)) return;
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
        var ray = camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var hitGo = hit.collider.gameObject;
            string hitParentName = "";
            if (hitGo.transform.parent!=null)
                hitParentName = hitGo.transform.parent.name;
            Debug.Log("Hit Object:" + hitGo + "|"+ hitGo.transform.parent);

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
        }
        else
        {
            Debug.LogError("Hit NULL:");
        }
    }

    private void SelectBimModel(Transform rayModel)
    {
        //if (IsClickUGUIorNGUI.Instance && IsClickUGUIorNGUI.Instance.isOverUI) return;
        ////if (DevSubsystemManage.Instance && DevSubsystemManage.Instance.RoamToggle.isOn) return;

        //Action<ViewState> callback = stateT =>
        //{
        //    if (stateT == ViewState.设备定位)
        //    {
        //        Debug.Log("SelectBimModel :" + rayModel.name);
        //        //lastRendererMat = lastRenderer.material;
        //        //lastRenderer.material = selectedMat;
        //        if (rayModel != null)
        //        {
        //            BIMModelInfo bimInfo=rayModel.GetComponent<BIMModelInfo>();
        //            if (bimInfo!= null)
        //            {
        //                bimInfo.OnClick();
        //            }else{
        //                if(rayModel.name.Contains("Culling")&&rayModel.transform.parent!=null&&rayModel.transform.GetComponent<BIMModelInfo>()!=null)
        //                {
        //                    rayModel.transform.GetComponent<BIMModelInfo>().OnClick();
        //                }
        //            }                                             
        //        }
        //    }
        //};
        //MessageCenter.SendMsg(MsgType.ModuleToolbarMsg.TypeName, MsgType.ModuleToolbarMsg.GetCurrentState, callback);
    }

    private void ClearLastInfo()
    {
        //UIManager.GetInstance().CloseUIPanels(typeof(DeviceDocumentationbar).Name);
    }
    private void highlightObj(GameObject obj)
    {
        //if(obj!=null&&obj.activeInHierarchy)
        //{
        //    HightlightModuleBase.ClearHighlightOff();
        //    HightlightModuleBase hightlightT = obj.AddMissingComponent<HightlightModuleBase>();
        //    hightlightT.ConstantOn(Color.green);
        //}
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
            HitTest(0);
        }

    }

    public AreaTreeNode lastNode;

    public MeshRenderer lastRenderer;

    public Material lastRendererMat;

    public Material selectedMat;

    public bool enableSelection;
}
