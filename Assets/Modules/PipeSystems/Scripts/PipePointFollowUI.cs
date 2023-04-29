using System.Collections;
using System.Collections.Generic;
using DbModel.Location.Pipes;
using Mogoson.CameraExtension;
using UnityEngine;
using UnityEngine.UI;

public class PipePointFollowUI : MonoBehaviour
{
    public static void SetIsEditMode(bool isEdit)
    {
        Debug.Log($"PipePointFollowUI.SetIsEditMode isEdit:{isEdit}");
        PipePointFollowUI[] uiList=GameObject.FindObjectsOfType<PipePointFollowUI>(true);
        foreach(var item in uiList){
            item.SetEditPart(isEdit);
        }
    }

    private void SetEditPart(bool isActive){
        if(editPart){
            editPart.SetActive(isActive);
        }
    }

    private PipePoint point;

    public GameObject pipeObj;

    public Text txtNumber;

    public Text txtVector3;

    public GameObject editPart;

    public Button btnAddPoint;

    public Button btnDelPoint;

    void Awake()
    {
        if(btnAddPoint)btnAddPoint.onClick.AddListener(AddPoint);
        if(btnDelPoint)btnDelPoint.onClick.AddListener(DeletePoint);
    }

    private void AddPoint(){
        
    }

    private void DeletePoint(){
        PipeSystemEditUIPanel.Instance.DeletePoint(point);
    }

    [ContextMenu("FocusPoint")]
    public void FocusPoint(){
        Debug.Log($"FocusPoint this:{this} 11  ");
        PipeSystemUtils.FocusPipePointObj(pipeObj);
        PipeSystemUtils.LocatePipePoint(point);
    }

    public void InitData(PipePoint pipePoint,GameObject pipeObj)
    {
        this.point=pipePoint;
        this.pipeObj=pipeObj;
        InitData(pipePoint);
    }

    public void InitData(PipePoint pipePoint)
    {
        txtNumber.text=pipePoint.Num.ToString();
        txtVector3.text=$"{pipePoint.X:F3}\n{pipePoint.Y:F3}\n{pipePoint.Z:F3}";
    }
}
