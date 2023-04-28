using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SceneText : MonoBehaviour
{
    public Font font;

    public List<Text> AllText=new List<Text>();
    public List<Text> TextList1 = new List<Text>();
    public List<Text> TextList2 = new List<Text>();
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [ContextMenu("SetFont")]
    public void SetFont()
    {
        foreach (var text in AllText)
        {
            text.font = font;
        }
    }

    [ContextMenu("GetAllText")]
    public void GetAllText()
    {
        //Zibo:5106
        AllText.Clear();
        //var texts = GameObject.FindObjectsOfTypeAll(typeof(Text));//4999
        var texts = Resources.FindObjectsOfTypeAll<Text>();//不运行4999，运行5049
        if (texts != null && texts.Length > 0) AllText.AddRange(texts); 

        //重复添加也不影响，最终要做的，是把所有text字体都换掉
        Canvas[] sceneCanvas = Resources.FindObjectsOfTypeAll<Canvas>();
        Debug.Log("Canvas Count:"+sceneCanvas.Length);
        foreach(var canvasTemp in sceneCanvas)
        {
            Text[] ts = canvasTemp.GetComponentsInChildren<Text>(true);
            if (ts != null && ts.Length > 0) AllText.AddRange(ts);
        }
        Debug.Log("GetAllText:"+ AllText.Count);
    }

    [ContextMenu("ChangeToChineseInputs")]
    public void SetSceneInputFieldToWebGL()
    {
        Canvas[] sceneCanvas = Resources.FindObjectsOfTypeAll<Canvas>();
        Debug.Log("Canvas Count:" + sceneCanvas.Length);
        List<InputField> inputs = new List<InputField>();
        foreach (var canvasTemp in sceneCanvas)
        {
            InputField[] ts = canvasTemp.GetComponentsInChildren<InputField>(true);
            inputs.AddRange(ts); 
        }
        foreach(var item in inputs)
        {
            item.gameObject.AddMissingComponent<InputField_WebGL>();
        }
        Debug.Log("输入框添加中文输入,Count:" + inputs.Count);
    }

}
