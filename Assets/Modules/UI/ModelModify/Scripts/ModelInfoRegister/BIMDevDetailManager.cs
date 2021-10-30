using NavisPlugins.Infos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIMDevDetailManager : SingletonBehaviour<BIMDevDetailManager>
{
    public BIMDevDetailList DetailList = new BIMDevDetailList();

    public TextAsset PipeAsset;

    public TextAsset ValveAsset;

    [ContextMenu("LoadDetailsList")]
    public void LoadDetailsList()
    {
        DetailList = new BIMDevDetailList();
        //DetailList.LoadPipeFromText(PipeAsset.text);
        //DetailList.LoadValvesFromText(ValveAsset.text);
    }
}
