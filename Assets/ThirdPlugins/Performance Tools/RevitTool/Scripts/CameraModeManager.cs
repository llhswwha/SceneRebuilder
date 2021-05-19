using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using UnityStandardAssets.Characters.FirstPerson;

public class CameraModeManager : MonoBehaviour
{
    // public FirstPersonController fpsController;

    public Toggle ToggleRotate;

    public Toggle ToggleFree;

    // Start is called before the first frame update
    void Start()
    {
        ToggleRotate.onValueChanged.AddListener(ToggleRotateChanged);
        ToggleFree.onValueChanged.AddListener(ToggleFreeChanged);
    }

    void ToggleRotateChanged(bool value)
    {
        print("ToggleRotateChanged:" + value);
    }

    void ToggleFreeChanged(bool value)
    {
        print("ToggleFreeChanged:" + value);
        // fpsController.gameObject.SetActive(value);
        // if (value == false)
        // {
        //     fpsController.RecoverCursorState();
        // }
        // else
        // {
        //     fpsController.ResetCursorState();
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            ToggleRotate.isOn = true;
        }
        if (Input.GetKey(KeyCode.F))
        {
            ToggleFree.isOn = true;
        }
    }
}
