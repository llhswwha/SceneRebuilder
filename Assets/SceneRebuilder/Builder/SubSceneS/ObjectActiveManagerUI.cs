using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

    public bool IsObjListEmpty()
    {
        foreach (var obj in Objects)
        {
            if (obj != null) return false;
        }
        return true;
    }

    public int MinVertexCount = 20;//10w

    [ContextMenu("ClearList")]
    public void ClearList()
    {
        Objects.Clear();
        foreach(var item in Items)
        {
            if (item == null) continue;
            GameObject.DestroyImmediate(item.gameObject);
        }
        Items.Clear();
    }

    public int AllRendererCount = 0;
    public float AllVertextCount = 0;

    [ContextMenu("InitList")]
    public void InitList()
    {
        if (IsObjListEmpty())
        {
            ClearList();

            var bList = GameObject.FindObjectsOfType<BuildingModelInfoList>(true).ToList();
            bList.ForEach(l => l.UpdateBuildings());
            //bList.Sort((a, b) => b.AllVertextCount.CompareTo(a.AllVertextCount));
            bList.Sort((a, b) => b.ShowVertextCount.CompareTo(a.ShowVertextCount));
            bList.ForEach(i => {
                //if(i.AllVertextCount>MinVertexCount)
                if (i.ShowVertextCount > MinVertexCount)
                {
                    Objects.Add(i.gameObject);
                    CreateItemUI(i.gameObject, $"¡¾BS¡¿{i.name}[{i.ShowVertextCount:F0}/{i.AllVertextCount:F0}w][{i.AllRendererCount}]");
                }
              });

            AllRendererCount = 0;
            AllVertextCount = 0;
            var bs = GameObject.FindObjectsOfType<BuildingModelInfo>(true).ToList();
            //bs.Sort((a, b) => b.AllVertextCount.CompareTo(a.AllVertextCount));
            bs.Sort((a, b) => b.Out0BigVertextCount.CompareTo(a.Out0BigVertextCount));
            bs.ForEach(i => 
            {
                //if (i.AllVertextCount > MinVertexCount)
                if (i.Out0BigVertextCount > MinVertexCount)
                {
                    Objects.Add(i.gameObject);
                    CreateItemUI(i.gameObject, $"¡¾B¡¿{i.name}[{i.Out0BigVertextCount:F0}/{i.AllVertextCount:F0}w][{i.AllRendererCount}]");
                }
                AllRendererCount += i.AllRendererCount ;
                AllVertextCount += i.AllVertextCount;
            }
             );
        }
        else
        {
            foreach (var obj in Objects)
            {

                CreateItemUI(obj, obj.name);
            }
        }
    }

    private void CreateItemUI(GameObject obj,string n)
    {
        if (obj == null) return;
        var uiItem = GameObject.Instantiate(activeItemUI);
        Items.Add(uiItem);
        uiItem.SetObject(obj);
        uiItem.SetTitle(n);
        uiItem.gameObject.SetActive(true);
        uiItem.transform.SetParent(ObjectsListPanel);
    }

}
