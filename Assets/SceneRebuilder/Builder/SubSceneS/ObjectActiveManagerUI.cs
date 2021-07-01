using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectActiveManagerUI : MonoBehaviour
{
    public List<GameObject> Objects=new List<GameObject>();

    public ObjectActiveItemUI activeItemUI;

    public List<ObjectActiveItemUI> Items=new List<ObjectActiveItemUI>();

    public RectTransform ObjectsListPanel;

    public void Start()
    {
        InitList();
    }

    public void InitList()
    {
        foreach(var obj in Objects)
        {
            if(obj==null)continue;
            var uiItem = GameObject.Instantiate(activeItemUI);
            Items.Add(uiItem);
            uiItem.SetObject(obj);
            uiItem.gameObject.SetActive(true);
            uiItem.transform.SetParent(ObjectsListPanel);
        }
    }

}
