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
                    if (AreaTreeHelper.combined2NodeDict.ContainsKey(lastRenderer))//点中了合并的模型
                    {
                        var treeNode = AreaTreeHelper.combined2NodeDict[lastRenderer];
                        Debug.LogWarning("Hit Tree Node1:" + treeNode);
                        if (lastNode != null && lastNode != treeNode)
                        {
                            lastNode.UpdateCombined();//如果发生修改，则更新合并的模型
                            lastNode.SwitchToCombined();
                        }
                        lastNode = treeNode;
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

                        if (AreaTreeHelper.render2NodeDict.ContainsKey(lastRenderer))
                        {
                            var treeNode = AreaTreeHelper.render2NodeDict[lastRenderer];
                            Debug.LogError("Hit Tree Node2:" + treeNode);
                            if (lastNode != null && lastNode != treeNode)
                            {
                                lastNode.UpdateCombined();//如果发生修改，则更新合并的模型
                                lastNode.SwitchToCombined();
                            }
                            lastNode = treeNode;
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
