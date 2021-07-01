using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectActiveItemUI : MonoBehaviour
{
    public Toggle ToggleIsActive;

    public Text ToggleName;

    public GameObject Object;

    public void SetObject(GameObject go)
    {
        Object=go;
        ToggleIsActive.isOn=go.activeSelf;
        ToggleName.text=go.name;
    }

    // Start is called before the first frame update
    void Start()
    {
        ToggleIsActive.onValueChanged.AddListener(ChangeActive);
    }

    void ChangeActive(bool isOn)
    {
        Debug.Log($"ChangeActive [{Object.name}][{isOn}]");
        Object.SetActive(isOn);
    }
}
