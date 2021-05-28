using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSelection : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Camera camera;

    public void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if(camera==null){
                camera=Camera.main;
            }
            var ray=camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                Debug.LogError("Hit Object:"+hit.collider.gameObject);

                if(lastRenderer!=null){
                    //lastRenderer.material=lastRendererMat;
                }

                lastRenderer=hit.collider.gameObject.GetComponent<MeshRenderer>();
                
                if(lastRenderer){
                    lastRendererMat=lastRenderer.material;
                    lastRenderer.material=selectedMat;

                    if(AreaTreeHelper.render2NodeDict.ContainsKey(lastRenderer))
                    {
                        var treeNode=AreaTreeHelper.render2NodeDict[lastRenderer];
                        Debug.LogError("Hit Node:"+treeNode);
                        if(lastNode!=null&&lastNode!=treeNode)
                        {
                            lastNode.UpdateCombined();//如果发生修改，则更新合并的模型
                            lastNode.SwitchToCombined();
                        }
                        lastNode=treeNode;
                        lastNode.SwitchToRenderers();
                    }
                    else{
                        Debug.LogError("No TreeNode :"+lastRenderer);
                    }
                }
            }
            else{
                Debug.LogError("Hit NULL:");
            }
        }

    }

    public AreaTreeNode lastNode;

    public MeshRenderer lastRenderer;

    public Material lastRendererMat;

    public Material selectedMat;
}
