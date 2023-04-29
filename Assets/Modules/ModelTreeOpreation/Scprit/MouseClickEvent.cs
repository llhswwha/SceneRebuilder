
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Y_UIFramework;

public class MouseClickEvent : MonoBehaviour, IPointerClickHandler 
{
    //public static MouseClickEvent Instance;
    public UnityEvent leftClick;
    public UnityEvent middleClick;
    public UnityEvent rightClick;
    private void Awake()
    {
        //Instance = this;
    }

    private void Start()
    {
        //leftClick.AddListener(new UnityAction(ButtonLeftClick));
        //middleClick.AddListener(new UnityAction(ButtonMiddleClick));
        //rightClick.AddListener(new UnityAction(ButtonRightClick));
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (ModeTreeEditManager.IsEdit)
            {
                GetTreeNodeInfo();//获取选中的节点
                SelectNodeFouse();//聚焦节点
                UIManager.GetInstance().ShowUIPanel(typeof(ModelTreeOperatePanel).Name);//节点操作界面
                SetOperationWindow();
            }
            else
                UGUIMessageBox.Show("编辑功能未激活,请先点击编辑按钮!");
        }
            //rightClick.Invoke();
    }
    //TreeNode<TreeViewItem> cacheNode;//缓存节点
    /// <summary>
    /// 获取树节点信息
    /// </summary>
    public void GetTreeNodeInfo()
    {
       var cacheNode = GetComponent<TreeViewComponent>().Node;
        NodeCacheManager.Instance.cacheNode = cacheNode;
        //Debug.LogError("data: " + data.Item.Name);
        //MessageCenter.SendMsg(MsgType.ModelTreeOperatePanelMsg.TypeName, MsgType.ModelTreeOperatePanelMsg.RemoveTreeNode, data) ;
    }
    /// <summary>
    /// 设置增删改界面的位置
    /// </summary>
    public void SetOperationWindow()
    {
        GameObject window = GameObject.Find("ModelTreeOperatePanel(Clone)");
        float X = Input.mousePosition.x - Screen.width / 2f;
        float Y = Input.mousePosition.y - Screen.height / 2f;
        Vector2 tranPos = new Vector2(X, Y);
        window.transform.GetComponent<RectTransform>().localPosition = tranPos;
    }
    /// <summary>
    /// 右键选中的节点高亮
    /// </summary>
    public void SelectNodeFouse()
    {
        ModelSystemTreeView tree = GameObject.Find("TreeView").transform.GetComponent<ModelSystemTreeView>();
        if (tree == null) return;
        tree.FindSelectNode(NodeCacheManager.Instance.cacheNode, false);
    }
}

