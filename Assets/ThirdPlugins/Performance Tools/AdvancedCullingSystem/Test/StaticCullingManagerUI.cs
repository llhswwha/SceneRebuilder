using AdvancedCullingSystem.DynamicCullingCore;
using AdvancedCullingSystem.StaticCullingCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticCullingManagerUI : MonoBehaviour
{

    public int a;
    public StaticCullingManager manager;

    public CullingSetting setting;

    public StaticCulling staticCulling;

    public DynamicCulling dynamicCulling;

    public Button btnBake;

    public Button BtnEnable_Disable;

    public Text Text_EnableDisable;

    public Text txtResult;

    public Text txtCullingState;

    public InputField InputField_CellSize;

    public InputField InputField_DirectionCount;

    public InputField InputField_JobsPerObject;

    public Toggle Toggle_Fast;

    public Toggle Toggle_OptimizeTree;

    public Toggle Toggle_Frustum;

    public Toggle Toggle_WithDynamic;

    public Toggle Toggle_WithStatic;

    private void GetCullings()
    {
        manager = StaticCullingManager.Instance;
        if (manager == null)
        {
            manager = this.gameObject.AddComponent<StaticCullingManager>();
        }
        if (staticCulling == null)
        {
            staticCulling = manager.staticCulling;
        }
        if (staticCulling == null)
        {
            staticCulling = GameObject.FindObjectOfType<StaticCulling>();
        }
        if (staticCulling != null)
        {
            setting = staticCulling.cullingMaster._cullingSetting;
        }

        if (dynamicCulling == null)
        {
            dynamicCulling = DynamicCulling.Instance;
        }
        if (dynamicCulling == null)
        {
            dynamicCulling = GameObject.FindObjectOfType<DynamicCulling>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetCullings();

        btnBake.onClick.AddListener(Bake);
        BtnEnable_Disable.onClick.AddListener(EnableDisable);

        InputField_CellSize.text = setting.CellSize.ToString();
        InputField_DirectionCount.text = setting.DirectionCount.ToString();
        InputField_JobsPerObject.text = setting.JobsPerObject.ToString();
        Toggle_Fast.isOn = setting.FastBake;
        Toggle_OptimizeTree.isOn = setting.IsOptimaizeTree;

        Toggle_Frustum.onValueChanged.AddListener(FrustumChanged);

        if (staticCulling)
        {
            Toggle_Frustum.isOn = staticCulling._enableFrustum;

            Toggle_WithDynamic.isOn = staticCulling.IsWithDynamicCulling;
            Toggle_WithDynamic.onValueChanged.AddListener(
                isOn => 
                {
                    
                    // if(isOn==false)
                    // {
                        
                    // }
                    //staticCulling.enabled=isOn;
                    //staticCulling.isen
                    if(isOn)
                    {
                        if(Toggle_WithStatic.isOn==false)
                        {
                            dynamicCulling.EnableUpdateInfo=true;
                        }
                        else{
                            dynamicCulling.EnableUpdateInfo=false;
                            staticCulling.IsWithDynamicCulling = isOn;
                        }
                    }
                    else
                    {
                        dynamicCulling.EnableUpdateInfo=false;
                            staticCulling.IsWithDynamicCulling = false;
                    }
                }
           
            );
        
            Toggle_WithStatic.isOn=staticCulling.enabled;
            Toggle_WithStatic.onValueChanged.AddListener(isOn=>
            {
                
                staticCulling.IsUpdateInfo=isOn;

                if(isOn==false&&Toggle_WithDynamic.isOn==true&&dynamicCulling!=null)
                {
                    dynamicCulling.enabled=true;
                    dynamicCulling.EnableUpdateInfo=true;
                }
            }
            );
        }
        else
        {
            Toggle_Frustum.enabled = false;
        }


        //Debug.Log("_enableFrustum:" + manager.staticCulling._enableFrustum + "|" + Toggle_Frustum.isOn);
    }

    private void FrustumChanged(bool isOn)
    {
         if (staticCulling)
        {
            staticCulling._enableFrustum = isOn;
        }
    }

    public bool IsEnableCulling = true;
    
    private void EnableDisable()
    {
        IsEnableCulling = !IsEnableCulling;
        if (IsEnableCulling == false)
        {
            Text_EnableDisable.text = "Enable";
            manager.staticCulling.Disable();
        }
        else
        {
            Text_EnableDisable.text = "Disable";
            manager.staticCulling.Enable();
        }
    }

    void Bake()
    {
        setting.CellSize = int.Parse(InputField_CellSize.text);
        setting.DirectionCount = int.Parse(InputField_DirectionCount.text);
        setting.JobsPerObject = int.Parse(InputField_JobsPerObject.text);
        setting.FastBake = Toggle_Fast.isOn;
        setting.IsOptimaizeTree = Toggle_OptimizeTree.isOn;
        manager._cullingSetting = setting;
        manager.Bake();
        txtResult.text = manager._cullingSetting.ToString();
    }

    public bool EnableUpdateCullingInfo = true;

    public float UpdateInfoInterval = 0.5f;

    //private IEnumerator UpdateInfo()
    //{
    //    while (EnableUpdateCullingInfo)
    //    {
    //        if (EnableUpdateCullingInfo)
    //        {
    //            if (staticCulling == null)
    //            {
    //                staticCulling = GameObject.FindObjectOfType<StaticCulling>();
    //            }
    //            if (staticCulling != null)
    //            {
    //                int c1 = staticCulling.VisibleCount;
    //                int c2 = staticCulling.SelectedCount;
    //                float p = c1 * 100f / c2;
    //                //txtCullingState.text = $"{c1}/{c2}({p:F2})({staticCulling.UpdateTime:F1}ms)";
    //            }

    //            if (dynamicCulling == null)
    //            {
    //                dynamicCulling = GameObject.FindObjectOfType<DynamicCulling>();
    //            }

    //            if (dynamicCulling != null)
    //            {
    //                int c1 = dynamicCulling.VisibleCount;
    //                int c2 = dynamicCulling.SelectedCount;
    //                float p = c1 * 100f / c2;
    //                txtCullingState.text = $"{c1}/{c2}({p:F2})({dynamicCulling.UpdateTime:F1}ms)";
    //            }
    //        }
    //        yield return new WaitForSeconds(UpdateInfoInterval);
    //    }

    //}

    // Update is called once per frame
    void Update()
    {
        if(EnableUpdateCullingInfo)
        {
            //if (staticCulling == null)
            //{
            //    staticCulling = GameObject.FindObjectOfType<StaticCulling>();
            //}
            string log = "";
            if (staticCulling != null)
            {
                int c1 = staticCulling.VisibleCount;
                int c2 = staticCulling.SelectedCount;
                float p = c1 * 100f / c2;
                if(staticCulling.IsWithDynamicCulling)
                {
                    log += $"{staticCulling.DymaicAddCount}/{c1}/{c2}({p:F2})({staticCulling.UpdateTime:F1}ms)";
                }
                else{
                    log += $"{c1}/{c2}({p:F2})({staticCulling.UpdateTime:F1}ms)";
                }
                
            }

            //if (dynamicCulling == null)
            //{
            //    dynamicCulling = GameObject.FindObjectOfType<DynamicCulling>();//?a��?��??��2��?��o��o?��?����?��
            //}

            if (dynamicCulling != null)
            {
                int c1 = dynamicCulling.VisibleCount;
                int c2 = dynamicCulling.SelectedCount;
                int c3 = dynamicCulling.HideCount;
                float p = c1 * 100f / c2;
                log += $"{c1}+{c3}={c2}({p:F2})({dynamicCulling.UpdateTimeTotal:F1}ms)";
            }

            if(!string.IsNullOrEmpty(log))
            {
                txtCullingState.text = log;
            }
        }
    }
}
