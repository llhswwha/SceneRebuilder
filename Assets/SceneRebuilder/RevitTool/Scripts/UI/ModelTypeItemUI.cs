using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelTypeItemUI : MonoBehaviour
{
    public Toggle ToggleVisible;

    public Text TextInfo;

    public Button BtnCreate;

    public Text TxtCreate;

    // Start is called before the first frame update
    void Start()
    {
        ToggleVisible.onValueChanged.AddListener(VisibleChanged);
        BtnCreate.onClick.AddListener(CreateModel);

        if (TxtCreate == null)
        {
            TxtCreate = BtnCreate.GetComponentInChildren<Text>();
        }
        SetText();
    }

    private void VisibleChanged(bool isOn)
    {
        Debug.Log("VisibleChanged:" + isOn+"|"+TypeInfo);
        if (IsLoad)
        {
            if (isOn == false)
            {
                SceneRebuilder.Instance.HideNodeNodeType(TypeInfo);
            }
            else
            {
                SceneRebuilder.Instance.ShowNodeNodeType(TypeInfo);
            }
           
            MeshTestUI.Instance.ShowMeshVertsCount();
        }
    }

    public static bool isFirst = true;

    public bool IsLoad = false;

    public void SetIsLoad(bool isLoad)
    {
        IsLoad = isLoad;
        SetText();
    }

    public void SetText()
    {
        if (TxtCreate != null)
        {
            if (IsLoad)
            {
                TxtCreate.text = "UnLoad";
            }
            else
            {
                TxtCreate.text = "Load";
            }
        }
        
    }

    private void CreateModel()
    {
        if (TxtCreate.text == "Load")
        {
            IsLoad = true;
            Debug.Log("CreateModel Load:" + TypeInfo);
            TxtCreate.text = "UnLoad";

            MeshTestUI.Instance.SetRebuider();
            SceneRebuilder.Instance.LoadNodeType(TypeInfo, isFirst);
            MeshTestUI.Instance.ShowMeshVertsCount();

            isFirst = false;
        }
        else if (TxtCreate.text == "UnLoad")
        {
            IsLoad = false;
            Debug.Log("CreateModel UnLoad:" + TypeInfo);
            TxtCreate.text = "Load";
            SceneRebuilder.Instance.UnLoadNodeNodeType(TypeInfo);
            MeshTestUI.Instance.ShowMeshVertsCount();
        }
    }

    public NodeTypeInfo TypeInfo;

    internal void SetInfo(NodeTypeInfo item)
    {
        TypeInfo = item;
        TextInfo.text = item.GetInfo();
    }
    
    public bool IsVisible
    {
        get
        {
            return ToggleVisible.isOn;
        }
    }
}
