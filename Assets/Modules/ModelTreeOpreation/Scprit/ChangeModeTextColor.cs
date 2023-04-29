using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeModeTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    private void OnEnable() 
    {
        ChangeTextColorWhite(transform.GetChild(0).GetComponent<Text>(),this.transform.GetChild(1).GetComponent<Image>());
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeTextColorGreen(transform.GetChild(0).GetComponent<Text>(), this.transform.GetChild(1).GetComponent<Image>());
        //Debug.LogError("eventData.pointerEnter.transform: " + eventData.pointerEnter.transform.name);
        //if (eventData.pointerEnter.transform.GetComponent<Text>() != null)
        //{
            
        //}
        //if (eventData.pointerEnter.transform.GetComponent<Button>() != null)
        //{
        //    Debug.LogError("eventData.pointerEnter.transform: " + eventData.pointerEnter.transform.name);
        //}
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.LogError("OnPointerExit");
        ChangeTextColorWhite(this.transform.GetChild(0).GetComponent<Text>(), this.transform.GetChild(1).GetComponent<Image>());
    }

    void Update()
    {
        
    }
    public void ChangeTextColorGreen(Text text,Image img)
    {
        text.color = new Color(109 / 255f, 236 / 255f, 254 / 255f, 1);
        img.color = new Color(109 / 255f, 236 / 255f, 254 / 255f, 1);
    }
    public void ChangeTextColorWhite(Text text, Image img)
    {
        text.color = new Color(1, 1, 1, 1);
        img.color = new Color(1, 1, 1, 1);
    }
}
