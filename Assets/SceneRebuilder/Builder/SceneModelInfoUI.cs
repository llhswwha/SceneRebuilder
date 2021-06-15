using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneModelInfoUI : MonoBehaviour
{
    public SceneModelInfo sceneModelInfo;

    public InputField InputMaxVertexCount;

    public Text textResult;

    void Start()
    {
        if(sceneModelInfo==null){
            sceneModelInfo=GameObject.FindObjectOfType<SceneModelInfo>();
        }
    }

    [ContextMenu("GetInfo")]
    public void GetInfo()
    {
        sceneModelInfo.MaxVertexCount=int.Parse(InputMaxVertexCount.text);
        sceneModelInfo.GetVertexBigSmall();
        textResult.text=sceneModelInfo.info;
    }
    
    [ContextMenu("ShowAll")]
    public void ShowAll()
    {
        sceneModelInfo.MaxVertexCount=int.Parse(InputMaxVertexCount.text);
        sceneModelInfo.ShowAll();
        textResult.text=sceneModelInfo.info;
    }

    [ContextMenu("HideSmallModels")]
    public void HideSmallModels()
    {
        Debug.Log("HideSmallModels");
        sceneModelInfo.MaxVertexCount=int.Parse(InputMaxVertexCount.text);
        sceneModelInfo.HideSmallModels();
    }

    [ContextMenu("HideBigModels")]
    public void HideBigModels()
    {
        Debug.Log("HideBigModels");
        sceneModelInfo.MaxVertexCount=int.Parse(InputMaxVertexCount.text);
        sceneModelInfo.HideBigModels();
    }


}
