using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTCommon;
using UnityEngine.InputSystem;

public class RTSelectionTool : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Camera camera;

    public GameObject hitObj;

    private Vector3 mousePos;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0)){
            if (IsClickUGUIorNGUI.Instance && IsClickUGUIorNGUI.Instance.isOverUI) 
            {
                Debug.Log("RTSelectionTool.Update IsClickUGUIorNGUI.Instance.isOverUI");
                return;
            }
            if(camera==null)
            {
                camera=Camera.main;
            }           
#if UNITY_INPUTSYSTEM
            mousePos = Mouse.current.position.ReadValue(); ;
#else
            mousePos = Input.mousePosition;
#endif
            Ray ray =Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray,out hitInfo)){

                hitObj=hitInfo.collider.gameObject;
                Debug.Log($"RTSelectionTool Hit:{hitObj} path:{hitObj.transform.GetPath()}");
                ExposeToEditor expose=hitObj.GetComponent<ExposeToEditor>();
                if(expose==null){
                    expose=hitObj.AddComponent<ExposeToEditor>();
                }
            }
            else{
                hitObj=null;
            }
        }
        
    }
}
