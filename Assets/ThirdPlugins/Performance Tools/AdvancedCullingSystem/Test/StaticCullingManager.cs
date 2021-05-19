using AdvancedCullingSystem.StaticCullingCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCullingManager : MonoBehaviour
{
    public static StaticCullingManager Instance;

    [SerializeField]
    public StaticCulling staticCulling;
    // Start is called before the first frame update

    [SerializeField]
    public CullingSetting _cullingSetting = new CullingSetting();

    private void Awake()
    {
        Instance = this;

        if (staticCulling == null)
        {
            staticCulling = StaticCulling.Instance;
            if (staticCulling == null)
            {
                staticCulling = GameObject.FindObjectOfType<StaticCulling>();
            }
            if (staticCulling != null)
            {
                _cullingSetting = staticCulling.cullingMaster._cullingSetting;
            }
        }
    }

    

    void Start()
    {
       
    }


    [ContextMenu("Bake")]
    public void Bake()
    {
        if (staticCulling != null)
        {
            StaticCulling.Clear();
            staticCulling.cullingMaster._cullingSetting = _cullingSetting;
            staticCulling.cullingMaster.Compute();
            staticCulling = StaticCulling.Instance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
