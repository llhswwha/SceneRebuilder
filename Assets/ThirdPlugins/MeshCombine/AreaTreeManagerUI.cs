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
        Manager.ToReanderers();
    }

    public void ToCombined()
    {
        Manager.ToCombined();
    }

    public void CreateDictionary()
    {
        Manager.CreateDictionary();
    }

    public void HideLeafNodes()
    {
        Manager.HideLeafNodes();
    }

    public void ShowLeafNodes()
    {
        Manager.ShowLeafNodes();
    }
}
