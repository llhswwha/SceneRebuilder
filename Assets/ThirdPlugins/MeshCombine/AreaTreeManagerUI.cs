using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTreeManagerUI : MonoBehaviour
{
    public AreaTreeManager Manager;

    public void Start()
    {
        if(Manager==null){
            Manager=GameObject.FindObjectOfType<AreaTreeManager>();
        }
    }

    public void ToReanderers()
    {
        Debug.Log("AreaTreeManagerUI.ToReanderers");
        Manager.ToReanderers();
    }

    public void ToCombined()
    {
        Debug.Log("AreaTreeManagerUI.ToCombined");
        Manager.ToCombined();
    }

    public void CreateDictionary()
    {
        Debug.Log("AreaTreeManagerUI.CreateDictionary");
        Manager.CreateDictionary();
    }

    public void HideLeafNodes()
    {
        Debug.Log("AreaTreeManagerUI.HideLeafNodes");
        Manager.HideLeafNodes();
    }

    public void ShowLeafNodes()
    {
        Debug.Log("AreaTreeManagerUI.ShowLeafNodes");
        Manager.ShowLeafNodes();
    }
}
