using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MonitorRange;
using Y_UIFramework;
using System;
using Assets.M_Plugins.Helpers.Utils;
using DbModel.Location.Pipes;
using Mogoson.CameraExtension;

public class PipeSystemTreePanel : UIBasePanel
{
    //public static AreaDevTreePanel Instance;
    //public string RootName;
    /// <summary>
    /// 滑动条
    /// </summary>
    public ScrollRect scrollRect;
    public ChangeTreeView Tree;
    ObservableList<TreeNode<TreeViewItem>> nodes;
    public GameObject Window;
    public Button hideBtn;
    /// <summary>
    /// 对应区域图片列表
    /// </summary>
    public List<Sprite> Icons;

    public Button btnAddPipeSystem;

    public Button btnDeleteAllPipeSystems;

    public Toggle toggleIsEditoPipeSystemMode;

    public Toggle toggleIsShowSettingWindow;

    void OnAddPipeSystem()
    {
        Debug.Log($"OnAddPipeSystem");
        UIManager.GetInstance().ShowUIPanel(typeof(PipeSystemAddPanel).Name);
    }

    void Awake()
    {
        PipeSystemSettingWindow.Instance.InitData();

        //窗体性质
        CurrentUIType.UIPanels_Type = UIPanelType.Normal;  //普通窗体
        //CurrentUIType.UIPanel_LucencyType = UIPanelTransparentType.Pentrate;//透明，可以穿透
        CurrentUIType.UIPanels_ShowMode = UIPanelShowMode.Normal;//普通，可以和其他窗体共存

        if(btnAddPipeSystem)btnAddPipeSystem.onClick.AddListener(OnAddPipeSystem);

        if(btnDeleteAllPipeSystems)btnDeleteAllPipeSystems.onClick.AddListener(DeleteAllPipeSystems);

        if(hideBtn!=null)hideBtn.onClick.AddListener(HideWindow);

        if(toggleIsEditoPipeSystemMode)toggleIsEditoPipeSystemMode.onValueChanged.AddListener(OnPipeEditModeChanged);
        if(toggleIsShowSettingWindow)toggleIsShowSettingWindow.onValueChanged.AddListener(OnShowSettingWindow);

        Debug.LogError("PipeSystemTreePanel.Awake");
        RegisterMsgListener(MsgType.PipeSystemTreePanelMsg.TypeName,
            obj =>
            {
                //Debug.Log($"PipeSystemTreePanel.RegisterMsg obj.Key:{obj.Key} obj.Values:{obj.Values}");
                if (obj.Key == MsgType.PipeSystemTreePanelMsg.DeleteNode)
                {
                    DeleteNode(obj.Values as PipeSystem);
                }
                else if (obj.Key == MsgType.PipeSystemTreePanelMsg.EditNode)
                {
                    EditNode(obj.Values as PipeSystem);
                }
                else if (obj.Key == MsgType.PipeSystemTreePanelMsg.AddNode)
                {
                    AddNode(obj.Values as PipeSystem);
                }
                else if (obj.Key == MsgType.PipeSystemTreePanelMsg.SelectNode)
                {
                    SelectNode(obj.Values as PipeSystem);
                }
                else if (obj.Key == MsgType.PipeSystemTreePanelMsg.ReshowWindow)
                {
                    ReShowWindow();
                }
                else
                {
                    Debug.LogError($"PipeSystemTreePanel.RegisterMsg obj.Key:{obj.Key} obj.Values:{obj.Values}");
                }
            }
       );
    }

    private void SelectNode(PipeSystem pipeSystem)
    {
        Debug.Log($"PipeSystemTreePanel.SelectNode pipeSystem:{pipeSystem}");
        if(sysNodeDict.ContainsKey(pipeSystem.Id))
        {
            TreeNode<TreeViewItem> node=sysNodeDict[pipeSystem.Id];
            if(node.Parent!=null){
                node.Parent.IsExpanded=true;
            }
            Tree.SelectedNodes=new List<TreeNode<TreeViewItem>>(){node};
        }
        else{
            Debug.LogError($"PipeSystemTreePanel.SelectNode sysNodeDict.ContainsKey==false pipeSystem:{pipeSystem}");
        }
    }

    private void OnShowSettingWindow(bool v)
    {
        if(v){
            PipeSystemSettingWindow.Instance.ShowWindow();
        }
        else{
            PipeSystemSettingWindow.Instance.HideWindow();
        }
    }

    private void OnPipeEditModeChanged(bool v)
    {
        PipeSystemBuilder.Instance.SetIsPipeEditMode(v);
    }

    public void ReShowWindow()
    {
        //OpenUIPanel(typeof(AreaDevTreePanel).Name);//如果调用show,再调用CloseUIPanel，关闭不了界面
        OpenUIPanel(this.GetType().Name);//如果调用show,再调用CloseUIPanel，关闭不了界面
        //TweenPlayForward();
        // MessageCenter.SendMsg(MsgType.SmallMapPanelMsg.TypeName, MsgType.SmallMapPanelMsg.TweenForward, null);
        MessageCenter.SendMsg(MsgType.LeftMapWindowPanelMsg.TypeName, MsgType.LeftMapWindowPanelMsg.TweenBack, false);
    }

    public void HideWindow()
    {
        Debug.Log("HideWindow 1");
        CloseUIPanel();
        // //TweenPlayBackwards();
        // MessageCenter.SendMsg(MsgType.SmallMapPanelMsg.TypeName, MsgType.SmallMapPanelMsg.TweenBack, null);
        // MessageCenter.SendMsg(MsgType.ModuleToolbarMsg.TypeName, MsgType.ModuleToolbarMsg.HideTreeWindow, true);

        UIManager.GetInstance().ShowUIPanel(typeof(LeftMapWindowPanel).Name);
        MessageCenter.SendMsg(MsgType.LeftMapWindowPanelMsg.TypeName, MsgType.LeftMapWindowPanelMsg.TweenForward, false);

        Debug.Log("HideWindow 2");

        
    }

    private void DeleteNode(PipeSystem pipeSystem)
    {
        Debug.Log($"PipeSystemTreePanel.DeleteNode pipeSystem:{pipeSystem}");
        if(sysNodeDict.ContainsKey(pipeSystem.Id))
        {
            TreeNode<TreeViewItem> node=sysNodeDict[pipeSystem.Id];
            node.Parent.Nodes.Remove(node);
        }
        else{
            Debug.LogError($"PipeSystemTreePanel.DeleteNode sysNodeDict.ContainsKey==false pipeSystem:{pipeSystem}");
        }
    }

    
    private void AddNode(PipeSystem pipeSystem)
    {
        if(pipeSystemTree==null){
            Debug.LogError("AddNode pipeSystemTree==null pipeSystem:{pipeSystem}");
            return;
        }
        if(!sysNodeDict.ContainsKey(pipeSystem.Id)){
            TreeNode<TreeViewItem> node=CreatePipeSystemNode(pipeSystem,null,0);
            Debug.Log($"PipeSystemTreePanel.AddNode_1 pipeSystem:{pipeSystem} Name:{node.Item.Name}");
            //node.Item.Name=pipeSystem.Name;
            //if(node.Parent.Item.Name!=pipeSystem.Type)
            {
                if(sysTypeNodeDict.ContainsKey(pipeSystem.Type)){
                    TreeNode<TreeViewItem> typeNode=sysTypeNodeDict[pipeSystem.Type];
                    Debug.Log($"PipeSystemTreePanel.AddNode_2 pipeSystem:{pipeSystem}");
                    
                    // node.Parent.Nodes.Remove(node);
                    // node.Parent.IsExpanded=false;
                    typeNode.Nodes.Add(node);
                    typeNode.IsExpanded=true;
                }
                else{
                    Debug.Log($"PipeSystemTreePanel.AddNode_3 pipeSystem:{pipeSystem}");
                    PipeSystemTreeNode pipeSysNode=new PipeSystemTreeNode(pipeSystem);
                    TreeNode<TreeViewItem> typeNode=CreatePipeTypeNode(pipeSysNode,pipeSystemTree.RootNode,0,false);
                    nodes.Add(typeNode);

                    typeNode.Nodes.Add(node);
                    typeNode.IsExpanded=true;
                }
            }
            
        }
        else{
            Debug.LogError($"PipeSystemTreePanel.MoveNode_3 pipeSystem:{pipeSystem} ");
        }
    }

    private void EditNode(PipeSystem pipeSystem)
    {
        if(sysNodeDict.ContainsKey(pipeSystem.Id)){
            TreeNode<TreeViewItem> node=sysNodeDict[pipeSystem.Id];
            Debug.Log($"PipeSystemTreePanel.EditNode_1 pipeSystem:{pipeSystem} Name:{node.Item.Name}");
            node.Item.Name=pipeSystem.Name;

            if(node.Parent.Item.Name!=pipeSystem.Type){
                if(sysTypeNodeDict.ContainsKey(pipeSystem.Type)){
                    TreeNode<TreeViewItem> typeNode=sysTypeNodeDict[pipeSystem.Type];
                    Debug.Log($"PipeSystemTreePanel.EditNode_2 pipeSystem:{pipeSystem}");
                    
                    node.Parent.Nodes.Remove(node);
                    node.Parent.IsExpanded=false;
                    typeNode.Nodes.Add(node);
                    typeNode.IsExpanded=true;
                }
                else{
                    Debug.Log($"PipeSystemTreePanel.EditNode_3 pipeSystem:{pipeSystem}");
                    PipeSystemTreeNode pipeSysNode=new PipeSystemTreeNode(pipeSystem);
                    TreeNode<TreeViewItem> typeNode=CreatePipeTypeNode(pipeSysNode,pipeSystemTree.RootNode,0,false);
                    nodes.Add(typeNode);

                    typeNode.Nodes.Add(node);
                    typeNode.IsExpanded=true;
                }
            }
            else{
                //不用修改父节点
            }
            
        }
        else{
            Debug.LogError($"PipeSystemTreePanel.MoveNode_3 pipeSystem:{pipeSystem} ");
        }
    }

    void Start()
    {
        SetListeners();
        GetTopoTree();
    }
    public override void Show()
    {
        base.Show();
        //GetTopoTree();
        SetAllPipeSystemActive(true);
        //FunctionSwitchBar.DXTransparentToggle_OnValueChanged(true);
        UIManager.GetInstance().ShowUIPanel(typeof(FunctionSwitchBar).Name);
        // MessageCenter.SendMsg(MsgType.FunctionSwitchBarMsg.TypeName, MsgType.FunctionSwitchBarMsg.SetPeiDianInfoToggle, true);
        //MessageCenter.SendMsg(MsgType.FunctionSwitchBarMsg.TypeName, MsgType.FunctionSwitchBarMsg.SetDXTransparentToggleIsOn_Active, true);
        MessageCenter.SendMsg(MsgType.FunctionSwitchBarMsg.TypeName, MsgType.FunctionSwitchBarMsg.SetToggleHide, "摄像头");
        // MessageCenter.SendMsg(MsgType.FunctionSwitchBarMsg.TypeName, MsgType.FunctionSwitchBarMsg.SetbuidingPersonNumToggleIsOn_Active, false);
        // MessageCenter.SendMsg(MsgType.FunctionSwitchBarMsg.TypeName, MsgType.FunctionSwitchBarMsg.SetToggleHide, "构件");
        // MessageCenter.SendMsg(MsgType.FunctionSwitchBarMsg.TypeName, MsgType.FunctionSwitchBarMsg.SetToggleHide, "进度");
        // MessageCenter.SendMsg(MsgType.FunctionSwitchBarMsg.TypeName, MsgType.FunctionSwitchBarMsg.SetToggleHide, "设备");
        // MessageCenter.SendMsg(MsgType.FunctionSwitchBarMsg.TypeName, MsgType.FunctionSwitchBarMsg.SetToggleHide, "灯光");
    }

    public override void Hide()
    {
        base.Hide();
        PipeSystemUtils.HidePipeSystemUI();
        SetAllPipeSystemActive(false);
        PipeSystemComponent.ClearLastPipeSystem();
        //FunctionSwitchBar.DXTransparentToggle_OnValueChanged(false);
        UIManager.GetInstance().CloseUIPanels(typeof(FunctionSwitchBar).Name);
        CameraSceneManager.Instance.ReturnToDefaultAlign();

        //ObjectsEditManage.Instance.CloseDevEdit();
        PipeSystemBuilder.Instance.SetIsPipeEditMode(false);
    }

    void OnDisable()
    {
        Debug.Log($"PipeSystemTreePanel.OnDisable");
        //HidePipeSystemUI();
    }

        public void SetListeners()
    {
        Tree.NodeSelected.AddListener(NodeSelected);
        Tree.NodeDeselected.AddListener(NodeDeselected);
        Tree.NodeToggle.AddListener(OnNodeToggled);
    }


    public void NodeSelected(TreeNode<TreeViewItem> node)
    {
        Debug.Log($"AreaDevTreePanel.NodeSelected node:{node.Item.Name} visibleCount:{node.GetVisibleCount()} level:{node.Level} tag:{node.Item.Tag} children:{node.Nodes} ");
        if(node.Item.Tag is PipeSystem){
            PipeSystem system=node.Item.Tag as PipeSystem;
            ShowPipeSystem(system);
        }
    }

    public bool IsOnlyShowOnePipeSystem=true;

    private void ShowPipeSystem(PipeSystem system){
        Debug.Log($"ShowPipeSystem system:{system}");
        // if(currentPipeSysObj){
        //     GameObject.DestroyImmediate(currentPipeSysObj);
        // }
        PipeSystemBuilder builder=PipeSystemBuilder.Instance;
        GameObject pipeObj=builder.ShowPipeSystem(system);
        //builder.FocusPipeSystemObject(pipeObj);

        // if(IsOnlyShowOnePipeSystem && builder.currentPipeSysObj){
        //     GameObject.DestroyImmediate(builder.currentPipeSysObj);
        // }
        //else
        {
            builder.FocusPipeSystemObject(pipeObj);
        }

        // UIManager.GetInstance().ShowUIPanel("PipeSystemInfoPanel");
        // MessageCenter.SendMsg(MsgType.PipeSystemInfoPanelMsg.TypeName, MsgType.PipeSystemInfoPanelMsg.InitData, system);

        // UIManager.GetInstance().ShowUIPanel("PipeSystemEditUIPanel");
        // MessageCenter.SendMsg(MsgType.PipeSystemEditPanelMsg.TypeName, MsgType.PipeSystemEditPanelMsg.InitData, system);

        PipeSystemComponent pipeSysCom=pipeObj.GetComponent<PipeSystemComponent>();
        pipeSysCom.OnClick();

        //builder.currentPipeSysObj=pipeObj;
    }



    public void NodeDeselected(TreeNode<TreeViewItem> node)
    {
        PipeSystemUtils.HidePipeSystemUI();
    }

    public void OnNodeToggled(TreeNode<TreeViewItem> node)
    {
    }

 [ContextMenu("InitClear")]
    private void InitClear()
    {
        nodes = new ObservableList<TreeNode<TreeViewItem>>();
        sysNodeDict=new Dictionary<int, TreeNode<TreeViewItem>>();
        sysTypeNodeDict=new Dictionary<string, TreeNode<TreeViewItem>>();
        pipeSystems=new List<PipeSystem>();
    }


    public PipeSystemTree pipeSystemTree;

    public void GetTopoTree(){
        Debug.Log("PipeSystemTreePanel.GetTopoTree Start");
        CommunicationObject.Instance.GetPipeSystemTree(tree=>{
            Debug.Log($"PipeSystemTreePanel.GetTopoTree End tree:{tree}");
            pipeSystemTree=tree;

            ShowTopoTree();
            ShowAllPipeSystems();
        });
    }

    public void ShowTopoTree(){

        Debug.Log("PipeSystemTreePanel.ShowTopoTree");
        if(pipeSystemTree==null){
            Debug.LogError("ShowTopoTree pipeSystemTree==null  ");
            return;
        }

        InitClear();

        nodes.BeginUpdate();
        //TreeNode<TreeViewItem> treeNode=new TreeNode<TreeViewItem>();
        if(pipeSystemTree.RootNode!=null && pipeSystemTree.RootNode.Children !=null)
        {
            foreach(var pipeSysNode in pipeSystemTree.RootNode.Children){
                TreeNode<TreeViewItem> topoNode=CreatePipeTypeNode(pipeSysNode,pipeSystemTree.RootNode,0,true);
                nodes.Add(topoNode);
            }
        }
        
        nodes.EndUpdate();

        Tree.Start();
        Tree.Nodes = nodes;
        Tree.Resize();

        //ShowAllPipeSystems();
    }

    private Dictionary<int,TreeNode<TreeViewItem>> sysNodeDict=new Dictionary<int, TreeNode<TreeViewItem>>();

    private Dictionary<string,TreeNode<TreeViewItem>> sysTypeNodeDict=new Dictionary<string, TreeNode<TreeViewItem>>();

    private TreeNode<TreeViewItem> CreatePipeTypeNode(PipeSystemTreeNode topoNode, PipeSystemTreeNode parentNode,int level,bool isInitSubItems)
    {
        Sprite icon = GetTopoIcon(topoNode);
        var item = new TreeViewItem(topoNode.Name, icon);
        
        //var item = new TreeViewItem(string.Format("{0}[{1}]",topoNode.Name,topoNode.Transfrom!=null), icon);
        item.Tag = topoNode;
        var node = new TreeNode<TreeViewItem>(item);
        node.Level = level; 
        node.Nodes=new ObservableList<TreeNode<TreeViewItem>>();

        if(isInitSubItems){
            node.Nodes.BeginUpdate();
            foreach(var pipeSystem in topoNode.Items)
            {
                TreeNode<TreeViewItem> subNode=CreatePipeSystemNode(pipeSystem,null,0);
                node.Nodes.Add(subNode);
                // if(sysNodeDict.ContainsKey(pipeSystem.Id))
                // {
                //     Debug.LogError($"CreatePipeTypeNode sysNodeDict.ContainsKey pipeSystem:{pipeSystem}");
                //     continue;
                // }
                // sysNodeDict.Add(pipeSystem.Id,subNode);
                
            }
            node.Nodes.EndUpdate();
        }

        if(sysTypeNodeDict.ContainsKey(topoNode.Name))
        {
            Debug.LogError($"CreatePipeTypeNode sysTypeNodeDict.ContainsKey topoNode:{topoNode.Name}");
        }
        else{
            sysTypeNodeDict.Add(topoNode.Name,node);
        }
        
        return node;
    }

    public bool AutoGenerateAllPipe=true;

    private TreeNode<TreeViewItem> CreatePipeSystemNode(PipeSystem topoNode, PipeSystemTreeNode parentNode,int level)
    {
        pipeSystems.Add(topoNode);

        //Sprite icon = GetTopoIcon(topoNode);
        Sprite icon=null;
        var item = new TreeViewItem(topoNode.Name, icon);
        item.Tag = topoNode;
        var node = new TreeNode<TreeViewItem>(item);
        node.Level = level; 

        // node.Nodes=new ObservableList<TreeNode<TreeViewItem>>();
        // node.Nodes.BeginUpdate();
        // if(topoNode.Points!=null)
        //     foreach(var point in topoNode.Points)
        //     {
        //         TreeNode<TreeViewItem> subNode=CreatePipePointNode(point,null,0);
        //         node.Nodes.Add(subNode);
        //     }
        // node.Nodes.EndUpdate();

        if(sysNodeDict.ContainsKey(topoNode.Id))
        {
            Debug.LogError($"CreatePipeTypeNode sysNodeDict.ContainsKey pipeSystem:{topoNode} ");
            sysNodeDict[topoNode.Id]=node;
        }
        else{
            sysNodeDict.Add(topoNode.Id,node);
        }
        
        return node;
    }

    private List<PipeSystem> pipeSystems=new List<PipeSystem>();

    private void ShowAllPipeSystems()
    {
        Debug.Log($"PipeSystemTreePanel.ShowAllPipeSystems pipeSystems:{pipeSystems.Count}");
        if(AutoGenerateAllPipe==false)return;

        PipeSystemBuilder.Instance.ShowAllPipeSystems(pipeSystems);
    }

    [ContextMenu("DeleteAllPipeSystems")]
    private void DeleteAllPipeSystems()
    {
        UGUIMessageBox.Show($"确定删除全部({pipeSystems.Count})管线？",()=>{
            PipeSystemBuilder.Instance.DeleteAllPipeSystems(pipeSystems);
        },()=>{

        });
    }


    private void SetAllPipeSystemActive(bool isActive)
    {
        try
        {
            Debug.Log($"PipeSystemTreePanel.SetAllPipeSystemActive pipeSystems:{pipeSystems.Count}");
        
            if(AutoGenerateAllPipe==false)return;
            PipeSystemBuilder builder=PipeSystemBuilder.Instance;
            for (int i = 0; i < pipeSystems.Count; i++)
            {
                PipeSystem pipeSys = pipeSystems[i];
                if (pipeSys==null)continue;
                GameObject pipeObj=builder.GetPipeSystemObj(pipeSys.GetKey());
                if(pipeObj!=null)
                {
                    pipeObj.SetActive(isActive);
                }
                else{
                    Debug.LogError($"PipeSystemTreePanel.SetAllPipeSystemActive[{i}] pipeObj==null pipeSys:{pipeSys}");
                }
            }
        }
        catch(Exception ex)
        {
            Debug.LogError($"PipeSystemTreePanel.HideAllPipeSystems Exception:{ex} ");
        }
        
    }

    private TreeNode<TreeViewItem> CreatePipePointNode(PipePoint topoNode, PipeSystemTreeNode parentNode,int level)
    {
        //Sprite icon = GetTopoIcon(topoNode);
        Sprite icon=null;
        var item = new TreeViewItem(topoNode.ToString(), icon);
        item.Tag = topoNode;
        var node = new TreeNode<TreeViewItem>(item);
        node.Level = level; 
        return node;
    }
    
    public Sprite GetTopoIcon(PipeSystemTreeNode tpNode)
    {
        Sprite icon = null;
        if(Icons==null) return icon;
        return icon;
    }
}

