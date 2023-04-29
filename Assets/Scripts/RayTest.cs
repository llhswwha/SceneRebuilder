using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Y_UIFramework;

public class RayTest : MonoBehaviour
{
    public static RayTest Instance;
    //public GameObject obj;
    public DateTime oldTime;

    public MeshRenderer lastRenderer;
    public AreaTreeNode lastNode;

    private float timer = 0f;//������
    public float DelayTime = 1.0f;//���ͣ��ʱ��


    private BIMModelInfo lastRayDev;
    private int floorLayer;
    private int wallLayer;
    private void Start()
    {
        floorLayer = LayerMask.NameToLayer("Floor");
        wallLayer = LayerMask.NameToLayer("Wall");
        Instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        //RayCheck();
        RayCheckByTime();
    }
    public bool IsUpdateCombined = false;

    public bool isLoadingModel;


    public void StartRayCheck()
    {
        isEnable = true;
    }

    public void CloseRayCheck()
    {
        isEnable = false;
        ClearInfo();
    }

    private bool isEnable;

    private DateTime lastRayTime;
    private GameObject lastRayObj;
    private BIMModelInfo currentModel;
    public float minCheckTime=0.5f;//����ͣ��0.5�룬�Ÿ���
    public void RayCheckByTime()
    {
        if (!isEnable) return;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (hitInfo.transform.gameObject.layer == floorLayer) return;
            if (hitInfo.transform.gameObject.layer == wallLayer) return;
            if (hitInfo.transform.name.ToLower().Contains("slab")) return;//����һЩ��İ���
            if(lastRayObj != null)
            {
                //�������������һ�����壬�����¿�ʼ��ʱ
                if(lastRayObj!=hitInfo.transform.gameObject)
                {
                    SetHitInfo(hitInfo);
                }
                else
                {
                    //��������ͬһ�����壬�ж��Ƿ��ڸ����壬ͣ�������涨ʱ��
                    if ((DateTime.Now - lastRayTime).TotalSeconds > minCheckTime)
                    {
                        lastRayTime = DateTime.Now;
                        BIMModelInfo infoT = hitInfo.transform.GetComponent<BIMModelInfo>();
                        if (infoT!=null&&infoT.enabled)
                        {
                            if (infoT == currentModel) return;
                            currentModel = infoT;
                            HighlightDev(currentModel);
                        }
                    }
                }
                
            }
            else
            {
                SetHitInfo(hitInfo);
            }
        }
    }


    private void HighlightDev(BIMModelInfo modelT)
    {
        //��ڵ����������ҵ�����۽��豸����λ���ڵ�
        Action<TreeNode<TreeViewItem>> callback = result =>
        {
            if (result != null)
            {
                DevInfo devInfoT = result.Item.Tag as DevInfo;
                if (devInfoT != null)
                {
                    currentModel.HighlightOn(false);
                    UGUITooltip.Instance.ShowTooltip(currentModel.MName);
                }
            }
        };
        object[] objs = new object[] { modelT, callback };
        MessageCenter.SendMsg(MsgType.AreaDevTreePanelMsg.TypeName, MsgType.AreaDevTreePanelMsg.GetDevNodeByRenderId, objs);
    }


    private void  SetHitInfo(RaycastHit hitInfo)
    {
        if (currentModel != null&&currentModel!=BIMModelInfo.currentFocusModel) currentModel.HighlightOff();
        UGUITooltip.Instance.Hide();
        currentModel = null;
        lastRayObj = hitInfo.transform.gameObject;
        lastRayTime = DateTime.Now;
    }
    private void ClearInfo()
    {
        if (currentModel != null && currentModel != BIMModelInfo.currentFocusModel) currentModel.HighlightOff();
        UGUITooltip.Instance.Hide();
        currentModel = null;
        lastRayObj = null;
        lastRayTime = DateTime.Now;
    }


    public void RayCheck()
    {
        if (!isEnable) return;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo) && !EventSystem.current.IsPointerOverGameObject())
        {

            if (hitInfo.transform.gameObject.layer == floorLayer) return;
            if (hitInfo.transform.gameObject.layer == wallLayer) return;
            //obj = hitInfo.transform.gameObject;,10000, ~(1 << 11 | 1 << 16)
            string hitParentName = "";
                //�ϲ�ģ�ʹ���
                if (hitInfo.transform.parent != null) hitParentName = hitInfo.transform.parent.name;
                                
                lastRenderer = hitInfo.transform.gameObject.GetComponent<MeshRenderer>();
                if (lastRenderer)
                {
                    if (hitParentName.EndsWith("_Combined"))
                    {
                        if (isLoadingModel)
                        {
                            Debug.Log("HitCombined multiTime...");
                            return;
                        }
                        isLoadingModel = true;
                        if (!AreaTreeHelper.combined2NodeDict.ContainsKey(lastRenderer))
                        {
                            var treeNode = lastRenderer.GetComponentInParent<AreaTreeNode>();
                            if (treeNode)
                            {
                                treeNode.CreateDictionary();
                            }
                        }
                        var treeNode0 = AreaTreeHelper.GetNodeByCombined(lastRenderer);
                        if (treeNode0 != null)//�����˺ϲ���ģ��
                        {
                            var treeNode1 = treeNode0;
                            if (lastNode != null && lastNode != treeNode1)
                            {
                                lastNode.UpdateCombined();//��������޸ģ�����ºϲ���ģ��
                                lastNode.SwitchToCombined();
                            }
                            lastNode = treeNode1;
                            lastNode.LoadAndSwitchToRenderers(b =>
                            {
                                isLoadingModel = false;
                                RayCheck();                              
                            });
                        }
                        else
                        {
                                Debug.LogError("1111");
                            var treeNode2 = AreaTreeHelper.GetNodeByRenderer(lastRenderer);
                            if (treeNode2 != null)
                            {
                                if (lastNode != null && lastNode != treeNode2)
                                {
                                    if (IsUpdateCombined)
                                        lastNode.UpdateCombined();//��������޸ģ�����ºϲ���ģ��
                                    lastNode.SwitchToCombined();
                                }
                                lastNode = treeNode2;
                                lastNode.SwitchToRenderers();
                            }
                        }
                    }
                  else
                    {
                    if (lastRayDev != null && lastRayDev != BIMModelInfo.currentFocusModel && lastRayDev.gameObject != hitInfo.transform.gameObject)
                    {
                        UGUITooltip.Instance.Hide();
                        lastRayDev.HighlightOff();
                        lastRayDev = null;
                    }
                    if (lastRayDev!=null&&lastRayDev.gameObject== hitInfo.transform.gameObject)
                        {
                            
                        }
                        else
                        {
                            timer += Time.deltaTime;
                            if (timer < DelayTime)
                            {
                                return;
                            }
                            else
                            {
                                Debug.Log("��������");
                                OpenHighLight(hitInfo.transform.gameObject);
                                timer = 0;
                            }                          
                        }
                }
                }               
        }  
    }
    public void OpenHighLight(GameObject o)
    {
        BIMModelInfo bim = o.GetComponent<BIMModelInfo>();
        //��Щǽ�ڵذ壬�ű����ó�disable,�����и���
        if (bim!=null&&bim.enabled)
        {
            //if(lastRayDev!=null&&lastRayDev.gameObject!=o && lastRayDev != BIMModelInfo.currentFocusModel)
            //{
            //    UGUITooltip.Instance.Hide();
            //    lastRayDev.HighlightOff();
            //}
            lastRayDev = o.GetComponent<BIMModelInfo>();
            lastRayDev.HighlightOn(false);
            UGUITooltip.Instance.ShowTooltip(lastRayDev.MName);
        }
        else if (lastRayDev != null && lastRayDev != BIMModelInfo.currentFocusModel)
        {
            UGUITooltip.Instance.Hide();
            lastRayDev.HighlightOff();
            lastRayDev = null;           
        }
    }
}
