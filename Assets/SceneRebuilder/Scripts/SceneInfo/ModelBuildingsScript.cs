using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBuildingsScript : MonoBehaviour
{
    public List<ModelBuildingScript> Buildings;

    public Vector3 ExpandOffset = new Vector3(0, 50, 0);

    [ContextMenu("ExpandLevels")]
    public void ExpandLevels()
    {
        Dictionary<double, List<ModelLevelScript>> levels = new Dictionary<double, List<ModelLevelScript>>();
        foreach (var b in Buildings)
        {
            foreach (var l in b.Levels)
            {
                if (!levels.ContainsKey(l.Info.Height))
                {
                    levels.Add(l.Info.Height, new List<ModelLevelScript>());
                }
                List<ModelLevelScript> list = levels[l.Info.Height];
                list.Add(l);
            }
        }

        int i = 0;
        foreach (var ls in levels.Values)
        {
            i++;
            foreach (var l in ls)
            {
                l.gameObject.transform.position += ExpandOffset * i;
            }
        }
    }

    [ContextMenu("ResetLevels")]
    public void ResetLevels()
    {
        foreach (var item in Buildings)
        {
            item.ResetLevels();
        }
    }
}
