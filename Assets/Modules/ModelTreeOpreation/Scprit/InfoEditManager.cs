using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoEditManager : MonoBehaviour
{
    public static InfoEditManager Instance;
    public Scrollbar vb;
    public ContentSizeFitter con;
    public VerticalLayoutGroup ver;
    public static bool IsEdit = false;
    public Button EditBtn;//编辑按钮
    public Button SearchBtn;//搜索按钮
    public InputField InputName;//搜索输入框
    public Transform InfoContent;//属性列表
    private string msg;
    void Awake()
    {
        Instance = this;
        
    }
    void Start()
    {
        EditBtn.onClick.AddListener(EnableEditMode);
        SearchBtn.onClick.AddListener(delegate { Search(InputName.text); } );
    }
    /// <summary>
    /// 搜索属性
    /// </summary>
    public void Search(string msg)
    {
        Debug.LogError("msg: " + msg);
        if(msg == "")
        {
            for (int i = 0; i < InfoContent.childCount; i++)
            {
                InfoContent.GetChild(i).gameObject.SetActive(true);
            }
            return;
        }
        for(int i = 0; i < InfoContent.childCount; i++ )
        {
            string str1 = InfoContent.GetChild(i).GetChild(1).GetChild(3).GetComponent<Text>().text;
            string str2 = InfoContent.GetChild(i).GetChild(1).GetComponent<InputField>().text;
            str1 =  str1.ToLower();
            str2 = str2.ToLower();
            //Debug.LogError
            //Debug.LogError("str2: " + str2);
            if (str1.Contains(msg) == false && str2.Contains(msg) == false)
            {
                InfoContent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    public void EnableEditMode()
    {
        IsEdit = !IsEdit;
        if (IsEdit)
            UGUIMessageBox.Show("属性编辑功能开启!");
        else
            UGUIMessageBox.Show("属性编辑功能关闭!");
    }
}
