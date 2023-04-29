using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using NavisPlugins.Infos;
using LitJson;

public class NodeCacheManager : MonoBehaviour
{
    public static NodeCacheManager Instance;
    [HideInInspector]
    public TreeNode<TreeViewItem> cacheNode;
    [HideInInspector]
    public List<PropertyCategoryInfo> infoList;
    [HideInInspector]
    public List<TreeNode<TreeViewItem>> ListNode;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }
    public void InitInfoList(TreeNode<TreeViewItem> Node)
    {
        ModelItemInfo info = Node.Item.Tag as ModelItemInfo;
        string modelGuid = info.UId;
        CommunicationObject.Instance.GetSmartModelInfo(modelGuid, infoT =>
        {
            if (infoT != null)
            {
                infoList = JsonMapper.ToObject<List<PropertyCategoryInfo>>(infoT.Categories);
            }
        });
    }
    
    void Update()
    {
        
    }
}
