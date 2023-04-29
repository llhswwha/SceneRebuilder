using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeTreeEditManager : MonoBehaviour
{
    public static bool IsEdit = false;
    public Button EditBtn;//编辑按钮
    void Start()
    {
        EditBtn.onClick.AddListener(EnableEditMode);
    }
    public void EnableEditMode()
    {
        IsEdit = !IsEdit;
        if (IsEdit)
            UGUIMessageBox.Show("模型树编辑功能开启!");
        else
            UGUIMessageBox.Show("模型树编辑功能关闭!");
    }
}
