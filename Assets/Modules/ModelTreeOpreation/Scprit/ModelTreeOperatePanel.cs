using Location.WCFServiceReferences.LocationServices;
using NavisPlugins.Infos;
using System;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Y_UIFramework;


public class ModelTreeOperatePanel : UIBasePanel
{
    public Button AddChildNodeBtn;
    public Button AddBroNodeBtn;
    public Button RemoveBtn;
    public Button EditBtn;
    public Button AddNodeInfoBtn;
    ModelSystemTreeView tree;
    [HideInInspector]
    public List<TreeNode<TreeViewItem>> AddProNodeList;//已经加了属性面板的列表
    private void Awake()
    {
        CurrentUIType.UIPanels_Type = UIPanelType.Fixed;  //普通窗体
        CurrentUIType.UIPanel_LucencyType = UIPanelTransparentType.Pentrate;//透明，可以穿透
        CurrentUIType.UIPanels_ShowMode = UIPanelShowMode.ReverseChange;//普通，可以和其他窗体共存

        RegisterMsgListener(MsgType.ModelTreeOperatePanelMsg.TypeName,
        obj =>
        {
            if (obj.Key == MsgType.ModelGroupPanelMsg.RemoveDevGroup)
            {
                RemoveTreeNode(obj.Values as TreeNode<TreeViewItem>);
            }
        });
    }
    private void Start()
    {
        if(tree == null) tree = GameObject.Find("TreeView").transform.GetComponent<ModelSystemTreeView>();
        AddChildNodeBtn.onClick.AddListener(delegate { AddTreeChildNode(NodeCacheManager.Instance.cacheNode); } );
        AddBroNodeBtn.onClick.AddListener(delegate { AddTreeBroNode(NodeCacheManager.Instance.cacheNode); }); 
        RemoveBtn.onClick.AddListener(delegate { RemoveTreeNode(NodeCacheManager.Instance.cacheNode); });
        EditBtn.onClick.AddListener(delegate { EditTreeNode(NodeCacheManager.Instance.cacheNode); });
        AddNodeInfoBtn.onClick.AddListener(delegate { AddTreeNodeInfo(NodeCacheManager.Instance.cacheNode); });
        tree.NodeSelected.AddListener(NodeSelectedInfo);
    }
    /// <summary>
    /// 增加树子节点
    /// </summary>
    public void AddTreeChildNode(TreeNode<TreeViewItem> node) 
    {
        //if(tree == null)
        //    tree = GameObject.Find("TreeView").transform.GetComponent<ModelSystemTreeView>(); 
        var new_item1 = new TreeViewItem("新节点");
        var new_node1 = new TreeNode<TreeViewItem>(new_item1);
        NodeCacheManager.Instance.ListNode.Add(new_node1);
        if (node == null) Debug.LogError("node is null");
        else Debug.LogError("nodeName: " + node.Item.Name);
        if (node.Nodes == null) 
            node.Nodes= new ObservableList<TreeNode<TreeViewItem>>();
        node.Nodes.Add(new_node1);
        if (tree == null) return;
        tree.FindSelectNode(new_node1);
        NodeCacheManager.Instance.cacheNode = new_node1;

        this.transform.gameObject.SetActive(false);
        UIManager.GetInstance().ShowUIPanel(typeof(EditTreeNodeNamePanel).Name);
    }
    /// <summary>
    /// 增加同级节点
    /// </summary>
    public void AddTreeBroNode(TreeNode<TreeViewItem> node) 
    {
        //if (tree == null)
        //    tree = GameObject.Find("TreeView").transform.GetComponent<ModelSystemTreeView>();
        var new_item1 = new TreeViewItem("新节点");
        var new_node1 = new TreeNode<TreeViewItem>(new_item1);
        if (node == null) Debug.LogError("node is null");
        else Debug.LogError("nodeName: " + node.Item.Name);
        if (node.Nodes == null)
            node.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        node.Parent.Nodes.Add(new_node1);
        if (tree == null) return;
        tree.FindSelectNode(new_node1);
        NodeCacheManager.Instance.cacheNode = new_node1;

        this.transform.gameObject.SetActive(false); //关闭操作界面
        UIManager.GetInstance().ShowUIPanel(typeof(EditTreeNodeNamePanel).Name);
    }

/// <summary>
/// 删除树节点
/// </summary>
    public void RemoveTreeNode(TreeNode<TreeViewItem> node)  
    {
        node.Parent.Nodes.Remove(node);
        node = null;
        //UIManager.GetInstance().CloseUIPanels(typeof(ModelTreeOperatePanel).Name);
        this.transform.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 修改树节点
    /// </summary>
    public void EditTreeNode(TreeNode<TreeViewItem> node)
    {
        UIManager.GetInstance().ShowUIPanel(typeof(EditTreeNodeNamePanel).Name);
        this.transform.gameObject.SetActive(false);
    }
    /// <summary>
    /// 给新建的节点创建属性
    /// </summary>
    public void AddTreeNodeInfo(TreeNode<TreeViewItem> node)
    {
        if(AddProNodeList.Contains(node))
        {
            UGUIMessageBox.Show("该节点属性已添加!");
            return;
        }
        AddProNodeList.Add(node);
        UGUIMessageBox.Show("节点属性已添加成功!", () =>
        {
            ShowInfo(NodeCacheManager.Instance.infoList);
        }, () =>
        {

        });
        //if (NodeCacheManager.Instance.ListNode.Contains(node))
        //{
        //    tree.NodeSelected.AddListener(NodeSelected);
        //}
        //GameObject Node = node.Item.Tag as GameObject;
        //Node.AddComponent<AddNodeInfoPanel>();
    }
    public void NodeSelectedInfo(TreeNode<TreeViewItem> node) 
    {
        //Debug.LogError("AddProNodeList.Contains(node): " + AddProNodeList.Contains(node));
        if(AddProNodeList.Contains(node))
        {
            ShowInfo(NodeCacheManager.Instance.infoList);
        }
        //else if()
        //    UGUIMessageBox.Show("请为节点先添加属性!");

    }
    public void ShowInfo(List<PropertyCategoryInfo> infoList)
    {
        UIManager.GetInstance().ShowUIPanel(typeof(SmartModelInfoPanel).Name);
        MessageCenter.SendMsg(MsgType.SmartModelInfoPanelMsg.TypeName, MsgType.SmartModelInfoPanelMsg.ShowInfo, infoList);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //ShowInfo(NodeCacheManager.Instance.infoList);
        }
        //rightClick.Invoke();
    }
    /// <summary>
    /// 关闭操作界面
    /// </summary>
    public void HideWindow() 
    {
        this.transform.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            HideWindow();
        }
    }
}
