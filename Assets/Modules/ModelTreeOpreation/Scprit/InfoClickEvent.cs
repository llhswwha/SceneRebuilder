using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Y_UIFramework;

public class InfoClickEvent : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (InfoEditManager.IsEdit) 
            {
                Debug.LogError("Name：" + this.transform.gameObject.name);
                InfoCacheManager.Instance.CacheInfo = this.transform.gameObject;//缓存选中的属性对象
                //GetTreeNodeInfo();//获取选中的节点
                //SelectNodeFouse();//聚焦节点
                UIManager.GetInstance().ShowUIPanel(typeof(InfoOperationPanel).Name);//节点操作界面
                SetOperationWindow();
            }
            else
                UGUIMessageBox.Show("编辑功能未激活,请先点击编辑按钮!");
        }
        //rightClick.Invoke();
    }
    void Start()
    {
        
    }
    /// <summary>
    /// 设置增删改界面的位置
    /// </summary>
    public void SetOperationWindow()
    {
        GameObject window = GameObject.Find("InfoOperationPanel(Clone)");
        float X = Input.mousePosition.x - Screen.width / 2f;
        float Y = Input.mousePosition.y - Screen.height / 2f;
        Vector2 tranPos = new Vector2(X, Y);
        window.transform.GetComponent<RectTransform>().localPosition = tranPos;
    }
    
}
