using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PipeSystemSettingWindow : SingletonBehaviour<PipeSystemSettingWindow>
{
    public InputField inputOffsetHeight;

    public InputField inputPipeSizePow;

    public Button btnClose;

    public Button btnSaveSetting;

    

    void Awake()
    {
        btnClose.onClick.AddListener(HideWindow);
        btnSaveSetting.onClick.AddListener(SaveSetting);
        
        //InitData();
    }

    public void HideWindow()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowWindow()
    {
        this.gameObject.SetActive(true);
    }

    void SaveSetting()
    {
        SetData();
        PipeSystemBuilder.Instance.ReShowAllPipeSystems();
    }

    private void LoadData(Action<PipeSystemSetting> callback)
    {
        CommunicationObject.Instance.GetPipeSystemSettingePoints(setting=>{
            pipeSysSetting=setting;
            if(callback!=null){
                callback(setting);
            }
        });
    }

    PipeSystemSetting pipeSysSetting;

    public void InitData()
    {
        Debug.Log($"PipeSystemSettingWindow.InitData 1");
        LoadData(setting=>{
            this.pipeSysSetting=setting;
            Debug.Log($"PipeSystemSettingWindow.InitData 2 setting:{setting}");
            SetValues();
        });
    }

    private void SetValues()
    {
        try
        {
            PipeSystemSetting setting=pipeSysSetting;
            PipeSystemPointConverter converter=PipeSystemPointConverter.Instance;
            if(setting!=null)
            {
                if(setting.IsOriginal()){
                    SaveData();
                    Debug.Log($"PipeSystemSettingWindow.InitData 3 setting:{setting}");
                }
                else{
                    converter.CADOffset.z=setting.OffsetHeight;
                    PipeSystemBuilder.Instance.pipeSizePow=setting.PipeSizePow;
                    var pos=setting.GetOffPos();
                    converter.SetOffsetPos(pos);
                    var scale=setting.GetOffScale();
                    converter.scale=scale;
                    Debug.Log($"PipeSystemSettingWindow.InitData 4 setting:{setting} pos:{pos} scale:{scale}");
                }
            }
            

            inputOffsetHeight.text=converter.CADOffset.z+"";
            inputPipeSizePow.text=PipeSystemBuilder.Instance.pipeSizePow+"";
            var posOffset=converter.GetOffsetPos();
            SetValue(inputPosX,posOffset.x);
            SetValue(inputPosY,posOffset.y);
            SetValue(inputPosZ,posOffset.z);
            SetValue(inputScaleX,converter.scale.x);
            SetValue(inputScaleY,converter.scale.y);
            SetValue(inputScaleZ,converter.scale.z);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"PipeSystemSettingWindow.SetValues Exception:{ex} ");
        }
        
    }

    public void SetData()
    {
        try
        {
            PipeSystemPointConverter converter=PipeSystemPointConverter.Instance;

            converter.CADOffset.z=float.Parse(inputOffsetHeight.text);
            PipeSystemBuilder.Instance.pipeSizePow=float.Parse(inputPipeSizePow.text);

            converter.scale=new Vector3(GetValue(inputScaleX,1),GetValue(inputScaleY,1),GetValue(inputScaleZ,1));
            var pos=new Vector3(GetValue(inputPosX),GetValue(inputPosY),GetValue(inputPosZ));
            //converter.axisZero=pos;
            converter.SetOffsetPos(pos);
            SaveData();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"PipeSystemSettingWindow.SaveData Exception:{ex}");
        }
        
    }

    private void SaveData()
    {
        Debug.Log($"PipeSystemSettingWindow.SaveData 1");
        PipeSystemPointConverter converter=PipeSystemPointConverter.Instance;
        if(pipeSysSetting!=null){
            pipeSysSetting.OffsetHeight=converter.CADOffset.z;
            pipeSysSetting.PipeSizePow=PipeSystemBuilder.Instance.pipeSizePow;
            pipeSysSetting.SetOffPos(converter.GetOffsetPos());
            pipeSysSetting.SetOffScale(converter.scale);
        }
        Debug.Log($"PipeSystemSettingWindow.SaveData 2");
        CommunicationObject.Instance.SetPipeSystemSettingePoints(pipeSysSetting,result=>{
            Debug.Log($"PipeSystemSettingWindow.SaveData 3 result:{result}");
        });
    }

    public InputField inputPosX;

    public InputField inputPosY;

    public InputField inputPosZ;

    public InputField inputScaleX;

    public InputField inputScaleY;

    public InputField inputScaleZ;

    private float GetValue(InputField input,float defaultV=0){
        if(input==null) return defaultV;
        if(string.IsNullOrEmpty(input.text)){
            return defaultV;
        }
        return float.Parse(input.text);
    }

    private void SetValue(InputField input,float v)
    {
        if(input){
            input.text=v+"";
        }
        else{
            Debug.LogError($"input==null v:{v}");
        }
    }

    private void SetValue(InputField input,float v,float defaultV)
    {
        if(input){
            if(v==0){
                v=defaultV;
            }
            input.text=v+"";
        }
        else{
            Debug.LogError($"input==null v:{v}");
        }
    }
}
