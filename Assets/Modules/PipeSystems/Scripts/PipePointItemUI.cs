using System;
using System.Collections;
using System.Collections.Generic;
using DbModel.Location.Pipes;
using UnityEngine;
using UnityEngine.UI;

public class PipePointItemUI : MonoBehaviour
{
    private PipePoint point;
    public Text txtInfo;

    //public Button btnLocate;

    public Toggle toggleLocate;

    public Button btnDelete;

    public Toggle toggleSelect;

    public event Action<PipePoint> OnLocateEvent;

    public event Action<PipePoint> OnDeleteEvent;

    public event Action<PipePoint> OnSelectEvent;

    public static PipePointItemUI SelectedItemUI;

    public static List<PipePointItemUI> checkedItems=new List<PipePointItemUI>();

    void Awake()
    {
        //btnLocate.onClick.AddListener(OnLocatePoint);
        if(toggleLocate)
            toggleLocate.onValueChanged.AddListener(OnLocateChanged);
        else{
            Debug.LogError($"PipePointItemUI.Awake toggleLocate==null point:{point}");
        }
        if(btnDelete){
            btnDelete.onClick.AddListener(OnDeletePoint);
        }
        else{
            Debug.LogError($"PipePointItemUI.Awake btnDelete==null point:{point}");
        }
        
        //btnSelect.onClick.AddListener(OnSelectPoint);

        if(toggleSelect){
            toggleSelect.onValueChanged.AddListener(OnSelectChanged);
        }
        else{
            Debug.LogError($"PipePointItemUI.Awake toggleSelect==null point:{point}");
        }
    }

    private void OnLocatePoint(){
        OnSelectPoint();
        if(OnLocateEvent!=null){
            OnLocateEvent(point);
        }
    }

    public void SetIsLocated()
    {
        if(SelectedItemUI==this) return;
        isEventValid=false;
        this.toggleSelect.isOn=true;
        this.toggleLocate.isOn=true;
        SetCurrentChecked();
        isEventValid=true;

        OnSelectPoint();
        SelectedItemUI=this;
    }

    private void SetCurrentChecked()
    {
        if(SelectedItemUI!=this){
            SelectedItemUI=this;
        }
        foreach(var item in checkedItems){
            item.toggleSelect.isOn=false;
            item.toggleLocate.isOn=false;
        }
        checkedItems.Clear();
        checkedItems.Add(this);
    }

    private static bool isEventValid=true;

    private void OnLocateChanged(bool v){
        Debug.Log($"PipePointItemUI.OnLocateChanged[{point}] v:{v} isEventValid:{isEventValid}");
         if(isEventValid==false)return;
        if(v){
            // if(SelectedItemUI!=this){
            //     SelectedItemUI=this;
            // }
            // foreach(var item in checkedItems){
            //     item.toggleSelect.isOn=false;
            //     item.toggleLocate.isOn=false;
            // }
            // checkedItems.Clear();
            // checkedItems.Add(this);
            SetCurrentChecked();

            this.toggleSelect.isOn=true;

            OnLocatePoint();
        }
    }

    private void OnDeletePoint(){
        
        if(OnDeleteEvent!=null){
            OnDeleteEvent(point);
        }
    }

    private void OnSelectPoint(){
        if(OnSelectEvent!=null){
            OnSelectEvent(point);
        }
    }

    private void OnSelectChanged(bool v)
    {
        Debug.Log($"PipePointItemUI.OnSelectChanged[{point}] v:{v} isEventValid:{isEventValid}");
        if(isEventValid==false)return;
        if(v){
            if(SelectedItemUI != null){
                SelectedItemUI.toggleSelect.isOn=false;
            }
            SelectedItemUI=this;

            OnSelectPoint();
        }
    }


    public void InitData(PipePoint point){
        this.point=point;
        txtInfo.text=point.ToString();
    }

    // void Start()
    // {
    //     this.transform.localRotation=Quaternion.Euler(30,0,0);
    // }

    [ContextMenu("SetRotation1")]
    private void SetRotation1()
    {
        this.transform.localRotation=Quaternion.Euler(10,20,30);
    }

    [ContextMenu("SetRotation2")]
    private void SetRotation2()
    {
        this.transform.localRotation=Quaternion.Euler(-30,0,0);
    }

    [ContextMenu("SetRotation3")]
    private void SetRotation3()
    {
        this.transform.rotation=Quaternion.Euler(30,0,0);
    }

    [ContextMenu("SetRotation4")]
    private void SetRotation4()
    {
        RectTransform rect=this.GetComponent<RectTransform>();
        rect.rotation=Quaternion.Euler(0,0,0);
    }
}
