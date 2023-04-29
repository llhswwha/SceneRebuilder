using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DbModel.Location.Pipes;
using SFB;
//using TriLibCore;
using UnityEngine;
using UnityEngine.UI;
using Y_UIFramework;

public class PipeSystemAddPanel 
    //: Singleton<PipeSystemAddPanel>
    : PipeSystemEditBasePanel
{

    public InputField txtFile;

    public Text txtFileInfo;

    public Button btnClose;

    public Button btnAdd;

    public Button btnEdit;

    public Button btnDelete;
    
    public Button btnGetList;

    public Button btnGetTree;

    public Button btnSelectFile;

    public Button btnGeneratePipeSystem;

    public Button btnDestroyPipeSystem;

    public Button btnChangePipeSystem;

    void Awake()
    {
         //窗体性质
        CurrentUIType.UIPanels_Type = UIPanelType.Normal;  //普通窗体
        //CurrentUIType.UIPanel_LucencyType = UIPanelTransparentType.Pentrate;//透明，可以穿透
        CurrentUIType.UIPanels_ShowMode = UIPanelShowMode.Normal;//普通，可以和其他窗体共存

        btnClose.onClick.AddListener(Hide);
        btnAdd.onClick.AddListener(AddPipeSystem);
        btnEdit.onClick.AddListener(EditPipeSystem);
        btnDelete.onClick.AddListener(DeletePipeSystem);
        btnGetList.onClick.AddListener(GetPipeSystemList);
        btnGetTree.onClick.AddListener(GetPipeSystemTree);
        btnSelectFile.onClick.AddListener(SelectCSVFile);
         if(btnGeneratePipeSystem)btnGeneratePipeSystem.onClick.AddListener(ShowCurrrentPipeSystem);
        if(btnDestroyPipeSystem)btnDestroyPipeSystem.onClick.AddListener(DestroyPipeSystems);
        if(btnChangePipeSystem)btnChangePipeSystem.onClick.AddListener(ModifyPipeSystems);

        InitPipeSystemTypes(true);

        RegisterMsgListener(MsgType.PipeSystemAddPanelMsg.TypeName,
            obj =>
            {
                Debug.Log($"PipeSystemInfoPanel.RegisterMsg obj.Key:{obj.Key} obj.Values:{obj.Values}");
                if (obj.Key == MsgType.PipeSystemAddPanelMsg.InitData)
                {
                    InitData(obj.Values as PipeSystem);
                }
                else
                {
                    Debug.LogError($"PipeSystemInfoPanel.RegisterMsg obj.Key:{obj.Key} obj.Values:{obj.Values}   ");
                }
            }
       );
    }


    void Start()
    {
        
        
    }







    // public string CurrentCSVFile=@"D:\WorkSpace\GitHub3\SceneRebuilder\Assets\Modules\PipeSystems\Files\地下电缆-采场联络路.csv";

    public string CurrentCSVFile = @"Test1.csv";

    protected void ModifyPipeSystems()
    {
        ShowCurrrentPipeSystem();
    }

    private void ShowCurrrentPipeSystem(){
        DestroyPipeSystems();
        //pipeSystem = GetNewPipeSystem();
        //ShowPipeSystem(pipeSystem);
        pipeSystems = GetNewPipeSystemList();
        if(pipeSystems.Count>0){
            pipeSystem=pipeSystems[0];
        }
        if(pipeSystems.Count==1){
            ShowPipeSystem(pipeSystem);
        }
        else{
            ShowAllPipeSystems(pipeSystems);
        }
    }

    public override void Hide()
    {
        Debug.Log($"PipeSystemAddPanel.Hide 12 ");
        base.Hide();
        DestroyPipeSystem();
        if(this==null)return;
        if(this.gameObject==null)return;
        GameObject.DestroyImmediate(this.gameObject);
    }

    public override void Show()
    {
        Debug.Log($"PipeSystemAddPanel.Show");
        base.Show();
        this.transform.localPosition=new Vector3(0,0,0);

        PipeSystemUtils.HidePipeSystemUI();
    }

    [ContextMenu("TestShowPipeSystem")]
    public void TestShowPipeSystem(){
        PipeSystem pipeSys=new PipeSystem();
        if(txtName)
            pipeSys.Name=txtName.text;
        else{
            pipeSys.Name="PipeSystem1";
        }
        if(txtSize)
            pipeSys.SizeX=float.Parse(txtSize.text);
        if(dropType)
            pipeSys.Type=GetCurrentPipeTypeName();
        if(txtCode)
            pipeSys.Code=txtCode.text;
        var points = PipeSystemUtils.ReadCSVFile(CurrentCSVFile);
        pipeSys.Points=points;

        ShowPipeSystem(pipeSys);

        pipeSystem=pipeSys;
    }

    public void GetPipeSystemList(){
         Debug.Log($"GetPipeSystemList Start");
        CommunicationObject.Instance.GetPipeSystemListEx(result=>{
            Debug.Log($"GetPipeSystemList End result:{result}");
            if(result!=null){
                for (int i = 0; i < result.Length; i++)
                {
                    PipeSystem item = result[i];
                     Debug.Log($"GetPipeSystemList[{i}]:{item}");
                }
            }
            
        });
    }

    public void GetPipeSystemTree(){
         Debug.Log($"GetPipeSystemTree Start pipeSystem:{pipeSystem.Name}");
        CommunicationObject.Instance.GetPipeSystemTree(result=>{
            Debug.Log($"GetPipeSystemTree End result:{result}");
            
        });
    }

    /*
    X,Y,Z,属性库1,属性库2,属性库3....
,,,,,
437997.572,4964773.31,584.498,,,
437991.115,4964764.822,583.879,,,
    */
    


    public void SelectCSVFile(){
        Debug.Log($"SelectCSVFile");
        ExtensionFilter[] extensions = new ExtensionFilter[2];
        extensions[1] = new ExtensionFilter("All Files", "*");
        extensions[0] = new ExtensionFilter("Points Files", "csv", "txt");
        //var extensions = "*.csv;*.txt;";
        StandaloneFileBrowser.OpenFilePanelAsync("Select a Points file", null, extensions, true, (itemsWithStream) =>
        {
            try
            {
                if (itemsWithStream != null)
                {
                    Debug.Log($"OnItemsWithStreamSelected itemsWithStream:{itemsWithStream.Length} ");
                    CurrentPipeGroups=new List<List<PipePoint>>();
                    string files="";
                    for (int i = 0; i < itemsWithStream.Length; i++)
                    {
                        string item = itemsWithStream[i];
                        if (string.IsNullOrEmpty(item))
                        {
                            break;
                        }
                        Debug.Log($"OnItemsWithStreamSelected[{i}]:{item}");
                        FileInfo file=new FileInfo(item);
                        
                        CurrentCSVFile=file.Name;
                        files+=file.Name+";";
                        
                        //CurrentPipePoints=PipeSystemUtils.ReadCSVFile(item);

                        var pipeGroups=PipeSystemUtils.ReadMultiPipesCSVFile(item);
                        
                        //CurrentPipeGroups=pipeGroups;
                        CurrentPipeGroups.AddRange(pipeGroups);
                    }

                    if(itemsWithStream.Length==1){
                        //txtFile.text=itemsWithStream[0];
                        txtFile.text=CurrentCSVFile;
                    }
                    else{
                        txtFile.text=$"[{itemsWithStream.Length}个文件]";
                    }

                    ShowPointsInfo();
                }
                else
                {
                    Debug.Log($"OnItemsWithStreamSelected itemsWithStream==null");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"OnItemsWithStreamSelected Exception:{ex}");
            }
            
        });
    }

    private void ShowPointsInfo()
    {
        var pipeGroups=CurrentPipeGroups;
        if(pipeGroups.Count>1){
            //var pipePoints=pipeGroups[0];
            int count=0;
            foreach(var list in pipeGroups)
            {
                    count+=list.Count;
            }
            txtFileInfo.text=$"顶点数:{count} 分段数:{pipeGroups.Count}";
        }
        else if(pipeGroups.Count==1){
            var pipePoints=pipeGroups[0];
            txtFileInfo.text=$"顶点数:{pipePoints.Count}";
        }
        else{
            txtFileInfo.text=$"顶点数:0";
        }
    }

    // AssetLoaderFilePicker assetLoaderFilePicker2;
    // private AssetLoaderFilePicker GetDownloaderFilePicker()
    // {
    //     if (assetLoaderFilePicker2 != null)
    //     {
    //         GameObject.DestroyImmediate(assetLoaderFilePicker2.gameObject);
    //     }
       
    //     assetLoaderFilePicker2 = AssetLoaderFilePicker.Create();
    //     return assetLoaderFilePicker2;
    // }
}
