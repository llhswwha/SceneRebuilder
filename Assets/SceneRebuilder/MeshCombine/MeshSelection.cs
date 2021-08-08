﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSelection : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        if (camera == null)
        {
            camera = GameObject.FindObjectOfType<Camera>();
        }
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
        Debug.Log("HitTest:" + count);
        if (count > 2)
        {
            return;
        }
        var ray = camera.ScreenPointToRay(Input.mousePosition);
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
                    Debug.Log("Hit Renderer2 :" + lastRenderer);
                    lastRendererMat = lastRenderer.material;
                    lastRenderer.material = selectedMat;
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

    public void Update()
    {
        if(Input.GetMouseButtonUp(0)&&enableSelection)
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
