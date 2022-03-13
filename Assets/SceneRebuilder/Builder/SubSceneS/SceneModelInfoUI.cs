using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneModelInfoUI : MonoBehaviour
{
    public SceneModelInfo sceneModelInfo;

    public InputField InputMaxVertexCount;

    public Text textResult;

    public float UpdateInterval = 0.5f;

    public Toggle toggleEnableUpdate;

    void Start()
    {
        if(sceneModelInfo==null){
            sceneModelInfo=GameObject.FindObjectOfType<SceneModelInfo>();
        }
        StartCoroutine(UpdateInfo());
    }

    private IEnumerator UpdateInfo()
    {
        while (true)
        {
            try
            {
                if (toggleEnableUpdate.isOn)
                    GetInfo();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SceneModelInfoUI.UpdateInfo text:{InputMaxVertexCount.text} Exception:{ex} ");
            }
            
            yield return new WaitForSeconds(UpdateInterval);
        }
        yield return null;
    }

    [ContextMenu("GetInfo")]
    public void GetInfo()
    {
        try
        {
            sceneModelInfo.MaxVertexCount = int.Parse(InputMaxVertexCount.text);
            sceneModelInfo.GetVertexBigSmall();
            textResult.text = sceneModelInfo.info;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SceneModelInfoUI.GetInfo text:{InputMaxVertexCount.text} Exception:{ex} ");
        }
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
