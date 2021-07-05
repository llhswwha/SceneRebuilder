using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GlobalMaterialManagerUI : MonoBehaviour
{
    public GlobalMaterialManager manager;

    public Text txtResult;

    public Toggle toggleIsAll;


    void Start()
    {
        manager=GameObject.FindObjectOfType<GlobalMaterialManager>(true);
    }

    public void GetMats()
    {
        txtResult.text=manager.InitMaterials();
    }

    public void CombineMats()
    {
        manager.SetCombineMaterial();
    }

    public void SetColorShader()
    {
        manager.ReplaceShader(toggleIsAll.isOn);
    }

    public void SetDefaultShader()
    {
        manager.ReplaceShaderDefault();
    }

    public void SetAllOneMaterial()
    {
        manager.SetAllOneMaterial();
    }
}
